#include "pch.h"
// ============================================================================
// TreadmillOpenVRWrapper - OpenVR Function Pointers and IVRInput Wrapper Impl
// ============================================================================
#include <algorithm>
#include <cstring>
#include <fstream>
#include <mutex>
#include <sstream>

#include "CalibrationOverlay.h"

using namespace TreadmillWrapper;

// ============================================================================
// REAL FUNCTION POINTERS
// ============================================================================

PFN_VR_InitInternal Real_VR_InitInternal = nullptr;
PFN_VR_InitInternal2 Real_VR_InitInternal2 = nullptr;
PFN_VR_ShutdownInternal Real_VR_ShutdownInternal = nullptr;
PFN_VR_GetVRInitErrorAsEnglishDescription Real_VR_GetVRInitErrorAsEnglishDescription = nullptr;
PFN_VR_GetVRInitErrorAsSymbol Real_VR_GetVRInitErrorAsSymbol = nullptr;
PFN_VR_IsHmdPresent Real_VR_IsHmdPresent = nullptr;
PFN_VR_IsRuntimeInstalled Real_VR_IsRuntimeInstalled = nullptr;
PFN_VR_GetRuntimePath Real_VR_GetRuntimePath = nullptr;
PFN_VR_GetStringForHmdError Real_VR_GetStringForHmdError = nullptr;
PFN_VR_GetGenericInterface Real_VR_GetGenericInterface = nullptr;
PFN_VR_IsInterfaceVersionValid Real_VR_IsInterfaceVersionValid = nullptr;
PFN_VR_GetInitToken Real_VR_GetInitToken = nullptr;

bool LoadOpenVRFunctions(HMODULE realDll) {
    if (!realDll) return false;
    
    Real_VR_InitInternal = (PFN_VR_InitInternal)GetProcAddress(realDll, "VR_InitInternal");
    Real_VR_InitInternal2 = (PFN_VR_InitInternal2)GetProcAddress(realDll, "VR_InitInternal2");
    Real_VR_ShutdownInternal = (PFN_VR_ShutdownInternal)GetProcAddress(realDll, "VR_ShutdownInternal");
    Real_VR_GetVRInitErrorAsEnglishDescription = (PFN_VR_GetVRInitErrorAsEnglishDescription)GetProcAddress(realDll, "VR_GetVRInitErrorAsEnglishDescription");
    Real_VR_GetVRInitErrorAsSymbol = (PFN_VR_GetVRInitErrorAsSymbol)GetProcAddress(realDll, "VR_GetVRInitErrorAsSymbol");
    Real_VR_IsHmdPresent = (PFN_VR_IsHmdPresent)GetProcAddress(realDll, "VR_IsHmdPresent");
    Real_VR_IsRuntimeInstalled = (PFN_VR_IsRuntimeInstalled)GetProcAddress(realDll, "VR_IsRuntimeInstalled");
    Real_VR_GetRuntimePath = (PFN_VR_GetRuntimePath)GetProcAddress(realDll, "VR_GetRuntimePath");
    Real_VR_GetStringForHmdError = (PFN_VR_GetStringForHmdError)GetProcAddress(realDll, "VR_GetStringForHmdError");
    Real_VR_GetGenericInterface = (PFN_VR_GetGenericInterface)GetProcAddress(realDll, "VR_GetGenericInterface");
    Real_VR_IsInterfaceVersionValid = (PFN_VR_IsInterfaceVersionValid)GetProcAddress(realDll, "VR_IsInterfaceVersionValid");
    Real_VR_GetInitToken = (PFN_VR_GetInitToken)GetProcAddress(realDll, "VR_GetInitToken");
    
    // At minimum we need VR_GetGenericInterface
    return Real_VR_GetGenericInterface != nullptr;
}

// ============================================================================
// IVRINPUT WRAPPER
// ============================================================================

// Store the real IVRInput interface and action name mappings
static void* g_realIVRInput = nullptr;
static std::unordered_map<VRActionHandle_t, std::string> g_actionNames;
static std::unordered_map<VRActionHandle_t, bool> g_isMovementAction;

// IVRInput vtable function types
typedef EVRInputError (*PFN_GetActionHandle)(void* self, const char* pchActionName, VRActionHandle_t* pHandle);
typedef EVRInputError (*PFN_GetAnalogActionData)(void* self, VRActionHandle_t action, InputAnalogActionData_t* pActionData, uint32_t unActionDataSize, VRInputValueHandle_t ulRestrictToDevice);

// Our wrapped functions
static EVRInputError Wrapped_GetActionHandle(void* self, const char* pchActionName, VRActionHandle_t* pHandle) {
    // Get real vtable
    void** vtable = *(void***)g_realIVRInput;
    auto realFunc = (PFN_GetActionHandle)vtable[IVRInputVTable::GetActionHandle];
    
    EVRInputError result = realFunc(g_realIVRInput, pchActionName, pHandle);
    
    if (result == VRInputError_None && pHandle && pchActionName) {
        // Store action name for later lookup
        g_actionNames[*pHandle] = pchActionName;
        
        // Check if this is a movement action
        bool isMovement = false;
        for (const auto& pattern : g_config.actionPatterns) {
            if (MatchesPattern(pchActionName, pattern)) {
                isMovement = true;
                break;
            }
        }
        g_isMovementAction[*pHandle] = isMovement;
        
        if (isMovement) {
            LogDebug("Detected movement action: %s (handle=0x%llX)", pchActionName, *pHandle);
        }
    }
    
    return result;
}

// ============================================================================
// IVRSYSTEM WRAPPER (LEGACY INPUT)
// ============================================================================

static void* g_realIVRSystem = nullptr;
static void* g_realIVRCompositor = nullptr;

// Function types for IVRSystem
typedef bool (*PFN_GetControllerState)(void* self, TrackedDeviceIndex_t unControllerDeviceIndex, VRControllerState_t* pControllerState, uint32_t unControllerStateSize);
typedef bool (*PFN_GetControllerStateWithPose)(void* self, int eOrigin, TrackedDeviceIndex_t unControllerDeviceIndex, VRControllerState_t* pControllerState, uint32_t unControllerStateSize, void* pTrackedDevicePose);
typedef void (*PFN_GetDeviceToAbsoluteTrackingPose)(void* self, int eOrigin, float fPredictedSecondsToPhotonsFromNow, TrackedDevicePose_t* pTrackedDevicePoseArray, uint32_t unTrackedDevicePoseArrayCount);
typedef int (*PFN_GetControllerRoleForTrackedDeviceIndex)(void* self, TrackedDeviceIndex_t unControllerDeviceIndex);
typedef int (*PFN_WaitGetPoses)(void* self, TrackedDevicePose_t* pRenderPoseArray, uint32_t unRenderPoseArrayCount, TrackedDevicePose_t* pGamePoseArray, uint32_t unGamePoseArrayCount);

namespace {
constexpr float kPi = 3.14159265f;

static float WrapPi(float angleRad) {
    while (angleRad > kPi) angleRad -= 2.0f * kPi;
    while (angleRad < -kPi) angleRad += 2.0f * kPi;
    return angleRad;
}

static float DegToRad(float angleDeg) {
    return angleDeg * (kPi / 180.0f);
}

static float RadToDeg(float angleRad) {
    return angleRad * (180.0f / kPi);
}
}




// Helper: Extract yaw angle from a 3x4 tracking matrix (radians)
// OpenVR: +X=right, +Y=up, -Z=forward. Column 2 = device Z-axis (backward).
// Forward direction in world XZ plane: forward_x = -m[0][2], forward_z = -m[2][2]
//
// We want: yaw=0 when facing forward (-Z), positive yaw when turning right.
// Formula: atan2(-m[0][2], m[2][2])
//   - Facing -Z: atan2(0, 1)  = 0°     (stable, no discontinuity!)
//   - Turned right (+X): atan2(1, 0)  = +90°
//   - Turned left  (-X): atan2(-1, 0) = -90°
//   - Facing +Z (backward): atan2(0, -1) = ±180° (discontinuity here, harmless)
//
// The atan2 discontinuity (±?) is at facing BACKWARD, not forward.
// This prevents wild yaw jumps during normal forward-facing gameplay.
static float ExtractYawFromMatrix34(const float m[3][4]) {
    return std::atan2(-m[0][2], m[2][2]);
}

// Helper: Get HMD yaw from SteamVR (radians, wrapped to [-pi, pi])
static float GetHmdYaw() {
    if (!g_realIVRSystem) return 0.0f;
    
    void** vtable = *(void***)g_realIVRSystem;
    auto getDevicePose = (PFN_GetDeviceToAbsoluteTrackingPose)vtable[IVRSystemVTable::GetDeviceToAbsoluteTrackingPose];
    if (!getDevicePose) return 0.0f;
    
    // Get HMD pose (device index 0 is always HMD)
    TrackedDevicePose_t poses[1];
    getDevicePose(g_realIVRSystem, 1 /* TrackingUniverseStanding */, 0.0f, poses, 1);
    
    
    if (!poses[0].bPoseIsValid) return 0.0f;
    
    // Extract yaw from the HMD matrix - NO OFFSET
    float yaw = WrapPi(ExtractYawFromMatrix34(poses[0].mDeviceToAbsoluteTracking.m));
    
    // Debug: Log matrix values occasionally for debugging
    static uint64_t debugCount = 0;
    if (++debugCount % 500 == 1) {
        const auto& mat = poses[0].mDeviceToAbsoluteTracking.m;
        LogTrace("HMD Matrix: fwd=(%.2f,%.2f,%.2f) -> Yaw=%.1f", 
            mat[0][2], mat[1][2], mat[2][2], RadToDeg(yaw));
    }
    
    return yaw;
}

// Helper: Rotate joystick values by angle (radians)
// Standard 2D rotation: counter-clockwise positive
static void RotateJoystick(float& x, float& y, float angleRad) {
    float cosA = std::cos(angleRad);
    float sinA = std::sin(angleRad);
    
    float newX = x * cosA - y * sinA;
    float newY = x * sinA + y * cosA;
    
    x = newX;
    y = newY;
}

// ============================================================================
// CALIBRATED MOVEMENT (ACTIVE VARIANT ONLY)
// ============================================================================

struct MovementResult {
    float yawRad;
    float rotatedX;
    float rotatedY;
};

struct CalibrationState {
    bool calibrated = false;
    float yawOffsetRad = 0.0f;

    float lastHmdYawRad = 0.0f;
    bool hasLastHmdYaw = false;
};

static CalibrationState g_calibration;
static std::mutex g_calibrationMutex;
static std::wstring g_calibrationPath;
static bool g_recenterLatch = false;

// Track grip/trigger per device index (up to 16 devices)
static constexpr int kMaxTrackedDevices = 16;
static bool g_gripPerDevice[kMaxTrackedDevices] = {};
static bool g_triggerPerDevice[kMaxTrackedDevices] = {};

// Recenter combo hold-duration: combo must be held for this many milliseconds
static constexpr int64_t kRecenterHoldMs = 2000;
static std::chrono::steady_clock::time_point g_comboStartTime{};
static bool g_comboHeld = false;

static MovementResult CalcCalibratedMovement(float treadmillYawRad, float hmdYawRad, float yawOffsetRad, float rawX, float rawY) {
    MovementResult r;
    // TM_world = TM_yaw + offset  (treadmill body direction in game world)
    // Rel = TM_world - HMD        (body direction relative to where player looks)
    //
    // The game interprets joystick as: moveDir = HMD + atan2(joyX, joyY)
    // We want: moveDir = TM_world
    // So we need: atan2(joyX, joyY) = TM_world - HMD = Rel
    //
    // RotateJoystick is CCW (standard math): (0,Y) rotated by angle gives
    //   atan2(newX, newY) = -angle  (note: NEGATIVE!)
    //
    // Therefore we rotate by -Rel to get atan2 = +Rel:
    //   game applies HMD + Rel = HMD + TM_world - HMD = TM_world  ?
    r.yawRad = WrapPi(treadmillYawRad + yawOffsetRad - hmdYawRad);
    r.rotatedX = rawX;
    r.rotatedY = rawY;
    RotateJoystick(r.rotatedX, r.rotatedY, -r.yawRad);
    return r;
}

static void Recenter(CalibrationState& st, float hmdYawRad, float treadmillYawRad) {
    // offset = hmdYaw - tmYaw at calibration time
    // So that tmYaw + offset = hmdYaw (treadmill world direction = HMD direction)
    st.yawOffsetRad = WrapPi(hmdYawRad - treadmillYawRad);
    st.calibrated = true;
}

static void SaveCalibrationState() {
    std::lock_guard<std::mutex> lock(g_calibrationMutex);
    if (g_calibrationPath.empty()) return;

    std::ofstream file(g_calibrationPath, std::ios::trunc);
    if (!file.is_open()) return;

    file.setf(std::ios::fixed);
    file.precision(6);
    file << "{\n";
    file << "  \"calibration\": {\n";
    file << "    \"yawOffsetRad\": " << g_calibration.yawOffsetRad << ",\n";
    file << "    \"calibrated\": " << (g_calibration.calibrated ? "true" : "false") << "\n";
    file << "  }\n";
    file << "}\n";
}

static void LoadCalibrationState() {
    std::lock_guard<std::mutex> lock(g_calibrationMutex);
    if (g_calibrationPath.empty()) return;

    std::ifstream file(g_calibrationPath);
    if (!file.is_open()) return;

    std::stringstream buffer;
    buffer << file.rdbuf();
    std::string content = buffer.str();

    auto extractValue = [&](const std::string& key) -> std::string {
        size_t keyPos = content.find("\"" + key + "\"");
        if (keyPos == std::string::npos) return {};
        size_t colonPos = content.find(':', keyPos);
        if (colonPos == std::string::npos) return {};
        size_t endPos = content.find_first_of(",}", colonPos + 1);
        if (endPos == std::string::npos) return {};
        std::string value = content.substr(colonPos + 1, endPos - colonPos - 1);
        value.erase(0, value.find_first_not_of(" \t\r\n\""));
        value.erase(value.find_last_not_of(" \t\r\n\"") + 1);
        return value;
    };

    std::string yawValue = extractValue("yawOffsetRad");
    if (!yawValue.empty()) {
        g_calibration.yawOffsetRad = std::stof(yawValue);
    }

    std::string calibratedValue = extractValue("calibrated");
    if (!calibratedValue.empty()) {
        std::string lower = calibratedValue;
        std::transform(lower.begin(), lower.end(), lower.begin(), ::tolower);
        g_calibration.calibrated = (lower == "true" || lower == "1");
    }
}

void SetCalibrationPath(const std::wstring& path) {
    g_calibrationPath = path;
    LoadCalibrationState();
}

static uint64_t ButtonMaskFromId(int buttonId) {
    return 1ull << buttonId;
}

static bool IsTreadmillMoving() {
    float treadmillX = g_treadmillState.x.load();
    float treadmillY = g_treadmillState.y.load();
    return std::abs(treadmillX) > 0.05f || std::abs(treadmillY) > 0.05f;
}

static const char* RoleToString(int role) {
    switch (role) {
    case TrackedControllerRole_LeftHand:
        return "Left";
    case TrackedControllerRole_RightHand:
        return "Right";
    default:
        return "Unknown";
    }
}

// Check if recenter combo is active: at least 2 different devices have grip pressed, and at least one has trigger pressed
static bool IsRecenterComboActive() {
    int gripCount = 0;
    bool anyTrigger = false;
    for (int i = 0; i < kMaxTrackedDevices; ++i) {
        if (g_gripPerDevice[i]) {
            ++gripCount;
            if (g_triggerPerDevice[i]) anyTrigger = true;
        }
    }
    return gripCount >= 2 && anyTrigger;
}

// Recenter with hold-duration: returns true only after combo held for kRecenterHoldMs
static int g_comboLastShownSec = 0;

static bool TryRecenterWithHold() {
    bool comboActive = IsRecenterComboActive();
    if (!comboActive) {
        if (g_comboHeld) {
            CalibrationOverlay::Show("Calibration cancelled");
            LogTrace("RECENTER HOLD: Combo released before threshold");
        }
        g_comboHeld = false;
        g_comboLastShownSec = 0;
        g_recenterLatch = false;
        return false;
    }
    if (g_recenterLatch) return false;
    auto now = std::chrono::steady_clock::now();
    if (!g_comboHeld) {
        g_comboHeld = true;
        g_comboStartTime = now;
        g_comboLastShownSec = 0;
        int totalSec = static_cast<int>(kRecenterHoldMs / 1000);
        char buf[128];
        snprintf(buf, sizeof(buf), "Calibrating... hold %ds", totalSec);
        CalibrationOverlay::Show(buf);
        LogTrace("RECENTER HOLD: Combo started, holding for %lldms...", kRecenterHoldMs);
        return false;
    }
    auto elapsed = std::chrono::duration_cast<std::chrono::milliseconds>(now - g_comboStartTime).count();
    int elapsedSec = static_cast<int>(elapsed / 1000);
    int totalSec = static_cast<int>(kRecenterHoldMs / 1000);
    if (elapsedSec > g_comboLastShownSec && elapsed < kRecenterHoldMs) {
        g_comboLastShownSec = elapsedSec;
        int remaining = totalSec - elapsedSec;
        char buf[128];
        snprintf(buf, sizeof(buf), "Calibrating... %ds", remaining);
        CalibrationOverlay::Show(buf);
    }
    if (elapsed < kRecenterHoldMs) {
        return false;
    }
    // Hold duration reached - trigger recenter
    float hmdYawRad = GetHmdYaw();
    float treadmillYawRad = DegToRad(g_treadmillState.yaw.load());
    {
        std::lock_guard<std::mutex> lock(g_calibrationMutex);
        Recenter(g_calibration, hmdYawRad, treadmillYawRad);
    }
    SaveCalibrationState();
    CalibrationOverlay::Show("Treadmill calibration saved!");
    LogTrace("RECENTER: Calibration saved (held %lldms). offset=%.1f", elapsed, RadToDeg(g_calibration.yawOffsetRad));
    g_recenterLatch = true;
    return true;
}

static void LogRecenterInput(uint64_t updateCount, TrackedDeviceIndex_t devIdx, uint64_t buttons, bool gripPressed, bool triggerPressed, bool recenterCombo) {
    if (updateCount % 200 != 0) return;
    LogTrace("RECENTER INPUT: dev=%u buttons=0x%llX grip=%d trigger=%d combo=%d",
        devIdx, buttons, gripPressed ? 1 : 0, triggerPressed ? 1 : 0, recenterCombo ? 1 : 0);
}

// ============================================================================
// WRAPPED FUNCTIONS
// ============================================================================

// Wrapped GetAnalogActionData - injects treadmill input (IVRInput path for modern games)
static EVRInputError Wrapped_GetAnalogActionData(void* self, VRActionHandle_t action, InputAnalogActionData_t* pActionData, uint32_t unActionDataSize, VRInputValueHandle_t ulRestrictToDevice) {
    // Get real vtable
    void** vtable = *(void***)g_realIVRInput;
    auto realFunc = (PFN_GetAnalogActionData)vtable[IVRInputVTable::GetAnalogActionData];
    
    // Call real function first
    EVRInputError result = realFunc(g_realIVRInput, action, pActionData, unActionDataSize, ulRestrictToDevice);
    
    // Inject treadmill data if this is a movement action
    if (result == VRInputError_None && pActionData) {
        auto it = g_isMovementAction.find(action);
        bool isMovement = (it != g_isMovementAction.end() && it->second);
        
        if (isMovement && OmniBridge::IsConnected()) {
            float treadmillX = g_treadmillState.x.load();
            float treadmillY = g_treadmillState.y.load();
            bool treadmillActive = (std::abs(treadmillX) > 0.05f || std::abs(treadmillY) > 0.05f);
            
            float rotatedX = treadmillX;
            float rotatedY = treadmillY;
            if (treadmillActive) {
                float treadmillYawRad = DegToRad(g_treadmillState.yaw.load());
                float hmdYawRad = GetHmdYaw();
                float yawOffsetRad = 0.0f;
                {
                    std::lock_guard<std::mutex> lock(g_calibrationMutex);
                    yawOffsetRad = g_calibration.yawOffsetRad;
                }
                MovementResult mov = CalcCalibratedMovement(treadmillYawRad, hmdYawRad, yawOffsetRad, treadmillX, treadmillY);
                rotatedX = mov.rotatedX;
                rotatedY = mov.rotatedY;
            }

            switch (g_config.inputMode) {
            case Config::InputMode::Override:
                if (treadmillActive) {
                    pActionData->x = rotatedX;
                    pActionData->y = rotatedY;
                    pActionData->bActive = true;
                }
                break;
                
            case Config::InputMode::Additive:
                pActionData->x = std::clamp(pActionData->x + rotatedX, -1.0f, 1.0f);
                pActionData->y = std::clamp(pActionData->y + rotatedY, -1.0f, 1.0f);
                if (treadmillActive) pActionData->bActive = true;
                break;
                
            case Config::InputMode::Smart:
            default:
                if (treadmillActive) {
                    pActionData->x = rotatedX;
                    pActionData->y = rotatedY;
                    pActionData->bActive = true;
                }
                break;
            }
            
            // Debug log occasionally
            static uint64_t callCount = 0;
            if (++callCount % 500 == 0 && treadmillActive) {
                float treadmillYawDeg = g_treadmillState.yaw.load();
                float hmdYawDeg = RadToDeg(GetHmdYaw());
                LogTrace("IVRInput 0x%llX: TM=%.1f HMD=%.1f Raw(%.2f,%.2f) -> Rot(%.2f,%.2f)", 
                    action, treadmillYawDeg, hmdYawDeg, treadmillX, treadmillY, rotatedX, rotatedY);
            }
        }
    }
    
    return result;
}

// Wrapped GetControllerState - injects treadmill input
static bool Wrapped_GetControllerState(void* self, TrackedDeviceIndex_t unControllerDeviceIndex, VRControllerState_t* pControllerState, uint32_t unControllerStateSize) {
    void** vtable = *(void***)g_realIVRSystem;
    auto realFunc = (PFN_GetControllerState)vtable[IVRSystemVTable::GetControllerState];
    
    bool result = realFunc(g_realIVRSystem, unControllerDeviceIndex, pControllerState, unControllerStateSize);
    
    // Track recenter buttons for ALL controllers (before target filter)
    if (result && pControllerState && unControllerDeviceIndex < kMaxTrackedDevices) {
        uint64_t buttons = pControllerState->ulButtonPressed;
        // Detect grip: button bit OR analog axis (Quest Touch sends grip as axis 2)
        bool gripPressed = (buttons & ButtonMaskFromId(k_EButton_Grip)) != 0
            || pControllerState->rAxis[2].x > 0.8f;
        // Detect trigger: button bit OR analog axis (Quest Touch sends trigger as axis 1)
        bool triggerPressed = (buttons & ButtonMaskFromId(k_EButton_SteamVR_Trigger)) != 0
            || pControllerState->rAxis[1].x > 0.8f;
        g_gripPerDevice[unControllerDeviceIndex] = gripPressed;
        g_triggerPerDevice[unControllerDeviceIndex] = triggerPressed;

        uint64_t updateCount = g_treadmillState.updateCount.load();
        bool recenterCombo = IsRecenterComboActive();
        LogRecenterInput(updateCount, unControllerDeviceIndex, buttons, gripPressed, triggerPressed, recenterCombo);
    }

    if (result && pControllerState && OmniBridge::IsConnected()) {
        // Handle recenter combo with hold-duration
        TryRecenterWithHold();

        // Filter by target controller for input injection
        if (g_config.targetControllerIndex >= 0 && 
            static_cast<int>(unControllerDeviceIndex) != g_config.targetControllerIndex) {
            return result;
        }
        
        float treadmillX = g_treadmillState.x.load();
        float treadmillY = g_treadmillState.y.load();
        float treadmillYawDeg = g_treadmillState.yaw.load();
        float treadmillYawRad = DegToRad(treadmillYawDeg);
        bool treadmillActive = IsTreadmillMoving();

        float hmdYawRad = GetHmdYaw();
        uint64_t updateCount = g_treadmillState.updateCount.load();

        if (treadmillActive) {
            float yawOffsetRad = 0.0f;
            {
                std::lock_guard<std::mutex> lock(g_calibrationMutex);
                yawOffsetRad = g_calibration.yawOffsetRad;
            }

            MovementResult activeMovement = CalcCalibratedMovement(treadmillYawRad, hmdYawRad, yawOffsetRad, treadmillX, treadmillY);

            float rotatedX = activeMovement.rotatedX;
            float rotatedY = activeMovement.rotatedY;
            
            switch (g_config.inputMode) {
            case Config::InputMode::Override:
                pControllerState->rAxis[k_EControllerAxis_Joystick].x = rotatedX;
                pControllerState->rAxis[k_EControllerAxis_Joystick].y = rotatedY;
                break;
                
            case Config::InputMode::Additive:
                pControllerState->rAxis[k_EControllerAxis_Joystick].x = 
                    std::clamp(pControllerState->rAxis[k_EControllerAxis_Joystick].x + rotatedX, -1.0f, 1.0f);
                pControllerState->rAxis[k_EControllerAxis_Joystick].y = 
                    std::clamp(pControllerState->rAxis[k_EControllerAxis_Joystick].y + rotatedY, -1.0f, 1.0f);
                break;
                
            case Config::InputMode::Smart:
            default:
                pControllerState->rAxis[k_EControllerAxis_Joystick].x = rotatedX;
                pControllerState->rAxis[k_EControllerAxis_Joystick].y = rotatedY;
                break;
            }
            
            // Debug log (synchronized with Treadmill updates)
            static uint64_t lastActiveLog = 0;
            if (updateCount != lastActiveLog && updateCount % 100 == 0) {
                lastActiveLog = updateCount;
            LogTrace("ACTIVE [LEGACY]: TM=%.1f HMD=%.1f Off=%.1f Rel=%.1f | Raw(%.2f,%.2f) -> Rot(%.2f,%.2f)",
                    treadmillYawDeg, RadToDeg(hmdYawRad), RadToDeg(yawOffsetRad), RadToDeg(activeMovement.yawRad),
                    treadmillX, treadmillY, rotatedX, rotatedY);
            }
        }
    }
    
    return result;
}

static bool Wrapped_GetControllerStateWithPose(void* self, int eOrigin, TrackedDeviceIndex_t unControllerDeviceIndex, VRControllerState_t* pControllerState, uint32_t unControllerStateSize, void* pTrackedDevicePose) {
void** vtable = *(void***)g_realIVRSystem;
auto realFunc = (PFN_GetControllerStateWithPose)vtable[IVRSystemVTable::GetControllerStateWithPose];
    
    
    
    
bool result = realFunc(g_realIVRSystem, eOrigin, unControllerDeviceIndex, pControllerState, unControllerStateSize, pTrackedDevicePose);
    
// Track recenter buttons for ALL controllers (before target filter)
if (result && pControllerState && unControllerDeviceIndex < kMaxTrackedDevices) {
    uint64_t buttons = pControllerState->ulButtonPressed;
    bool gripPressed = (buttons & ButtonMaskFromId(k_EButton_Grip)) != 0
        || pControllerState->rAxis[2].x > 0.8f;
    bool triggerPressed = (buttons & ButtonMaskFromId(k_EButton_SteamVR_Trigger)) != 0
        || pControllerState->rAxis[1].x > 0.8f;
    g_gripPerDevice[unControllerDeviceIndex] = gripPressed;
    g_triggerPerDevice[unControllerDeviceIndex] = triggerPressed;

    bool recenterCombo = IsRecenterComboActive();
    LogRecenterInput(g_treadmillState.updateCount.load(), unControllerDeviceIndex, buttons, gripPressed, triggerPressed, recenterCombo);
}

if (result && pControllerState && OmniBridge::IsConnected()) {
// Handle recenter combo with hold-duration
TryRecenterWithHold();

// Filter by target controller for input injection
if (g_config.targetControllerIndex >= 0 && 
    static_cast<int>(unControllerDeviceIndex) != g_config.targetControllerIndex) {
    return result;
}
        
    float treadmillX = g_treadmillState.x.load();
    float treadmillY = g_treadmillState.y.load();
    float treadmillYawDeg = g_treadmillState.yaw.load();
    float treadmillYawRad = DegToRad(treadmillYawDeg);
    bool treadmillActive = IsTreadmillMoving();

    float hmdYawRad = GetHmdYaw();

    if (treadmillActive) {
        float yawOffsetRad = 0.0f;
        {
            std::lock_guard<std::mutex> lock(g_calibrationMutex);
            yawOffsetRad = g_calibration.yawOffsetRad;
        }

        MovementResult activeMovement = CalcCalibratedMovement(treadmillYawRad, hmdYawRad, yawOffsetRad, treadmillX, treadmillY);
        
        float rotatedX = activeMovement.rotatedX;
        float rotatedY = activeMovement.rotatedY;
            
        switch (g_config.inputMode) {
        case Config::InputMode::Override:
            pControllerState->rAxis[k_EControllerAxis_Joystick].x = rotatedX;
            pControllerState->rAxis[k_EControllerAxis_Joystick].y = rotatedY;
            break;
            
        case Config::InputMode::Additive:
            pControllerState->rAxis[k_EControllerAxis_Joystick].x = 
                std::clamp(pControllerState->rAxis[k_EControllerAxis_Joystick].x + rotatedX, -1.0f, 1.0f);
            pControllerState->rAxis[k_EControllerAxis_Joystick].y = 
                std::clamp(pControllerState->rAxis[k_EControllerAxis_Joystick].y + rotatedY, -1.0f, 1.0f);
            break;
            
        case Config::InputMode::Smart:
        default:
            pControllerState->rAxis[k_EControllerAxis_Joystick].x = rotatedX;
            pControllerState->rAxis[k_EControllerAxis_Joystick].y = rotatedY;
            break;
        }
    }
}
    
return result;
}

static int Wrapped_WaitGetPoses(void* self, TrackedDevicePose_t* pRenderPoseArray, uint32_t unRenderPoseArrayCount, TrackedDevicePose_t* pGamePoseArray, uint32_t unGamePoseArrayCount) {
    void** vtable = *(void***)g_realIVRCompositor;
    auto realFunc = (PFN_WaitGetPoses)vtable[IVRCompositorVTable::WaitGetPoses];

    int result = realFunc(g_realIVRCompositor, pRenderPoseArray, unRenderPoseArrayCount, pGamePoseArray, unGamePoseArrayCount);

    TrackedDevicePose_t* poses = pRenderPoseArray;
    uint32_t poseCount = unRenderPoseArrayCount;
    if ((!poses || poseCount == 0) && pGamePoseArray && unGamePoseArrayCount > 0) {
        poses = pGamePoseArray;
        poseCount = unGamePoseArrayCount;
    }

    if (poses && poseCount > 0 && poses[0].bPoseIsValid) {
        float hmdYawRad = WrapPi(ExtractYawFromMatrix34(poses[0].mDeviceToAbsoluteTracking.m));

        // HMD yaw tracking for diagnostics only
        {
            std::lock_guard<std::mutex> lock(g_calibrationMutex);
            g_calibration.lastHmdYawRad = hmdYawRad;
            g_calibration.hasLastHmdYaw = true;
        }
    }

    return result;
}

// ============================================================================
// VTABLE HOOKING
// ============================================================================

// Our wrapper vtables
static void* g_wrappedInputVTable[64] = { nullptr };
static void* g_inputWrapperInstance = nullptr;

static void* g_wrappedSystemVTable[128] = { nullptr };  // IVRSystem has more functions
static void* g_systemWrapperInstance = nullptr;

static void* g_wrappedCompositorVTable[64] = { nullptr };
static void* g_compositorWrapperInstance = nullptr;

void* WrapIVRInput(void* realInterface) {
    if (!realInterface) return nullptr;
    
    g_realIVRInput = realInterface;
    
    // Copy the real vtable
    void** realVTable = *(void***)realInterface;
    memcpy(g_wrappedInputVTable, realVTable, sizeof(g_wrappedInputVTable));
    
    // Replace functions we want to intercept
    g_wrappedInputVTable[IVRInputVTable::GetActionHandle] = (void*)Wrapped_GetActionHandle;
    g_wrappedInputVTable[IVRInputVTable::GetAnalogActionData] = (void*)Wrapped_GetAnalogActionData;
    
    // Create a fake object that points to our vtable
    static void* wrappedVTablePtr = g_wrappedInputVTable;
    g_inputWrapperInstance = &wrappedVTablePtr;
    
    LogInfo("IVRInput wrapper created");
    
    return g_inputWrapperInstance;
}

void* WrapIVRSystem(void* realInterface) {
    if (!realInterface) return nullptr;
    
    g_realIVRSystem = realInterface;
    
    // Copy the real vtable
    void** realVTable = *(void***)realInterface;
    memcpy(g_wrappedSystemVTable, realVTable, sizeof(g_wrappedSystemVTable));
    
    // Replace controller state functions
    g_wrappedSystemVTable[IVRSystemVTable::GetControllerState] = (void*)Wrapped_GetControllerState;
    g_wrappedSystemVTable[IVRSystemVTable::GetControllerStateWithPose] = (void*)Wrapped_GetControllerStateWithPose;
    
    // Create a fake object that points to our vtable
    static void* wrappedVTablePtr = g_wrappedSystemVTable;
    g_systemWrapperInstance = &wrappedVTablePtr;
    
    LogInfo("IVRSystem wrapper created (legacy input interception enabled)");
    
    return g_systemWrapperInstance;
}

void* WrapIVRCompositor(void* realInterface) {
    if (!realInterface) return nullptr;

    g_realIVRCompositor = realInterface;

    void** realVTable = *(void***)realInterface;
    memcpy(g_wrappedCompositorVTable, realVTable, sizeof(g_wrappedCompositorVTable));

    g_wrappedCompositorVTable[IVRCompositorVTable::WaitGetPoses] = (void*)Wrapped_WaitGetPoses;

    static void* wrappedVTablePtr = g_wrappedCompositorVTable;
    g_compositorWrapperInstance = &wrappedVTablePtr;

    LogInfo("IVRCompositor wrapper created (yaw calibration enabled)");

    return g_compositorWrapperInstance;
}
