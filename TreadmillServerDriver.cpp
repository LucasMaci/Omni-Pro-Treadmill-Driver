#include "TreadmillServerDriver.h"
#include "TreadmillDevice.h"
#include <mutex>

extern void Log(const char* fmt, ...);
extern void OnOmniData(float ringAngle, int gamePadX, int gamePadY);

vr::EVRInitError TreadmillServerDriver::Init(vr::IVRDriverContext* pDriverContext) {
    try {
        VR_INIT_SERVER_DRIVER_CONTEXT(pDriverContext);

        // Debug-Flag aus Settings laden
        if (vr::VRSettings()) {
            vr::EVRSettingsError se = vr::VRSettingsError_None;
            bool debugEnabled = vr::VRSettings()->GetBool("driver_treadmill", "debug", &se);
            if (se == vr::VRSettingsError_None) {
                extern std::atomic<bool> g_debug;
                g_debug.store(debugEnabled);
                Log("treadmill: debug flag loaded from settings: %s", debugEnabled ? "true" : "false");
            }
        }
        
        Log("treadmill: Init called");

        // DLL-Pfad aus Settings laden (Standard: hardcoded path)
        char dllPath[512];
        strcpy_s(dllPath, sizeof(dllPath), "C:\\Program Files (x86)\\Steam\\steamapps\\common\\SteamVR\\drivers\\treadmill\\bin\\win64\\OmniBridge.dll");
        
        if (vr::VRSettings()) {
            vr::EVRSettingsError se = vr::VRSettingsError_None;
            vr::VRSettings()->GetString(
                "driver_treadmill", 
                "omnibridge_dll_path", 
                dllPath, 
                sizeof(dllPath), 
                &se
            );
            if (se != vr::VRSettingsError_None) {
                Log("treadmill: omnibridge_dll_path not found in settings, using default path");
                strcpy_s(dllPath, sizeof(dllPath), "C:\\Program Files (x86)\\Steam\\steamapps\\common\\SteamVR\\drivers\\treadmill\\bin\\win64\\OmniBridge.dll");
            }
        }
        
        // Konvertiere char* zu wchar_t* für LoadLibrary
        wchar_t wDllPath[512];
        MultiByteToWideChar(CP_UTF8, 0, dllPath, -1, wDllPath, 512);

        // Lade OmniBridge.dll
        omniReaderLib = LoadLibrary(wDllPath);

        if (!omniReaderLib) {
            DWORD err = GetLastError();
            char buf[256];
            FormatMessageA(FORMAT_MESSAGE_FROM_SYSTEM, nullptr, err, 0, buf, sizeof(buf), nullptr);
            Log("treadmill: LoadLibrary failed for '%s': %s", dllPath, buf);
            return vr::VRInitError_Driver_Failed;
        }
        
        Log("treadmill: OmniBridge.dll loaded from: %s", dllPath);

        // Lade alle Funktionen
        pfnCreate = (PFN_OmniReader_Create)GetProcAddress(omniReaderLib, "OmniReader_Create");
        pfnInitialize = (PFN_OmniReader_Initialize)GetProcAddress(omniReaderLib, "OmniReader_Initialize");
        pfnRegisterCallback = (PFN_OmniReader_RegisterCallback)GetProcAddress(omniReaderLib, "OmniReader_RegisterCallback");
        pfnDisconnect = (PFN_OmniReader_Disconnect)GetProcAddress(omniReaderLib, "OmniReader_Disconnect");
        pfnDestroy = (PFN_OmniReader_Destroy)GetProcAddress(omniReaderLib, "OmniReader_Destroy");

        if (!pfnCreate || !pfnInitialize || !pfnRegisterCallback || !pfnDisconnect || !pfnDestroy) {
            Log("treadmill: Nicht alle Funktionen konnten aus OmniBridge.dll geladen werden");
            FreeLibrary(omniReaderLib);
            omniReaderLib = nullptr;
            return vr::VRInitError_Driver_Failed;
        }

        // Omni Reader initialisieren
        m_omniReader = pfnCreate();
        if (m_omniReader) {
            pfnRegisterCallback(m_omniReader, OnOmniData);
            
            // COM-Port aus Settings laden (Standard: "COM3")
            char comPort[64] = "COM3";
            if (vr::VRSettings()) {
                vr::EVRSettingsError se = vr::VRSettingsError_None;
                vr::VRSettings()->GetString(
                    "driver_treadmill", 
                    "com_port", 
                    comPort, 
                    sizeof(comPort), 
                    &se
                );
                if (se != vr::VRSettingsError_None) {
                    Log("treadmill: com_port not found in settings, using default COM3");
                    strcpy_s(comPort, sizeof(comPort), "COM3");
                }
            }
            
            if (pfnInitialize(m_omniReader, comPort, 0, 115200)) {
                Log("treadmill: Omni Reader connected on %s", comPort);
            } else {
                Log("treadmill: Omni Reader failed to initialize on %s", comPort);
            }
        } else {
            Log("treadmill: OmniReader_Create failed");
        }

        // 1. Treadmill-Controller (unsichtbar, für Inputs)
        m_device = std::make_unique<TreadmillDevice>(0);
        
        vr::IVRServerDriverHost* pDriverHost = vr::VRServerDriverHost();
        if (!pDriverHost) {
            Log("treadmill: Init: VRServerDriverHost() returned null");
            return vr::VRInitError_Driver_Failed;
        }

        bool added = pDriverHost->TrackedDeviceAdded(
            "treadmill_controller", 
            vr::TrackedDeviceClass_Controller, 
            m_device.get()
        );
        Log("treadmill: Controller added: %s", added ? "true" : "false");

        // 2. NEU: Visualisierungs-Tracker (sichtbar)
        m_visualTracker = std::make_unique<TreadmillVisualTracker>();
        
        bool trackerAdded = pDriverHost->TrackedDeviceAdded(
            "treadmill_visual_tracker",
            vr::TrackedDeviceClass_GenericTracker,
            m_visualTracker.get()
        );
        Log("treadmill: Visual Tracker added: %s", trackerAdded ? "true" : "false");

        return vr::VRInitError_None;
    } catch (const std::exception &e) {
        Log("treadmill: Init exception: %s", e.what());
        return vr::VRInitError_Driver_Failed;
    } catch (...) {
        Log("treadmill: Init unknown exception");
        return vr::VRInitError_Driver_Failed;
    }
}

void TreadmillServerDriver::Cleanup() {
    Log("treadmill: Cleanup called");
    
    if (m_omniReader && pfnDisconnect && pfnDestroy) {
        pfnDisconnect(m_omniReader);
        pfnDestroy(m_omniReader);
        m_omniReader = nullptr;
    }
    
    if (omniReaderLib) {
        FreeLibrary(omniReaderLib);
        omniReaderLib = nullptr;
    }
    
    m_visualTracker.reset();
    m_device.reset();
}

const char* const* TreadmillServerDriver::GetInterfaceVersions() {
    return vr::k_InterfaceVersions;
}

void TreadmillServerDriver::RunFrame() {
    // Controller Input Updates
    if (m_device && m_device->m_unObjectId != vr::k_unTrackedDeviceIndexInvalid) {
        m_device->UpdateInputs();
        vr::DriverPose_t pose = m_device->GetPose();
        vr::VRServerDriverHost()->TrackedDevicePoseUpdated(
            m_device->m_unObjectId, pose, sizeof(vr::DriverPose_t));
    }
    
    // NEU: Visual Tracker Pose Updates
    if (m_visualTracker && m_visualTracker->m_unObjectId != vr::k_unTrackedDeviceIndexInvalid) {
        vr::DriverPose_t trackerPose = m_visualTracker->GetPose();
        vr::VRServerDriverHost()->TrackedDevicePoseUpdated(
            m_visualTracker->m_unObjectId, trackerPose, sizeof(vr::DriverPose_t));
    }
}

bool TreadmillServerDriver::ShouldBlockStandbyMode() { return false; }
void TreadmillServerDriver::EnterStandby() {}
void TreadmillServerDriver::LeaveStandby() {}