#pragma once

#include <mutex>
#include <cstdio>

#include "openvr_wrapper.h"

namespace TreadmillWrapper {

class CalibrationOverlay {
public:
    static void Show(const char* message) {
        if (!message || !*message) return;

        std::lock_guard<std::mutex> lock(GetMutex());

        VROverlayHandle_t overlayHandle = EnsureOverlay();
        if (overlayHandle == 0) return;

        void* notifIface = GetOrCreateNotificationsInterface();
        if (!notifIface) return;

        void** vtable = *(void***)notifIface;
        auto create = (PFN_CreateNotification)vtable[kCreateNotificationIndex];
        if (!create) return;

        VRNotificationId notificationId = 0;
        int err = create(notifIface,
            overlayHandle,
            0,
            EVRNotificationType_Transient,
            message,
            EVRNotificationStyle_Application,
            nullptr,
            &notificationId);

        if (err != 0) {
            LogDebug("CalibrationOverlay: CreateNotification failed (error=%d) for: %s", err, message);
        }
    }

private:
    typedef uint64_t VROverlayHandle_t;
    typedef uint32_t VRNotificationId;

    enum EVRNotificationType {
        EVRNotificationType_Transient = 0,
        EVRNotificationType_Persistent = 1
    };

    enum EVRNotificationStyle {
        EVRNotificationStyle_None = 0,
        EVRNotificationStyle_Application = 100
    };

    struct NotificationBitmap_t {
        void* m_pImageData;
        int32_t m_nWidth;
        int32_t m_nHeight;
        int32_t m_nBytesPerPixel;
    };

    typedef int (*PFN_CreateNotification)(void* self,
        VROverlayHandle_t overlayHandle,
        uint64_t userValue,
        EVRNotificationType type,
        const char* text,
        EVRNotificationStyle style,
        const NotificationBitmap_t* image,
        VRNotificationId* notificationId);

    // IVROverlay function types
    typedef int (*PFN_CreateOverlay)(void* self,
        const char* overlayKey,
        const char* overlayName,
        VROverlayHandle_t* overlayHandle);

    static constexpr int kCreateNotificationIndex = 0;
    static constexpr int kCreateOverlayIndex = 1;

    static VROverlayHandle_t EnsureOverlay() {
        VROverlayHandle_t& handle = GetOverlayHandle();
        if (handle != 0) return handle;

        void* overlayIface = GetOrCreateOverlayInterface();
        if (!overlayIface) return 0;

        void** vtable = *(void***)overlayIface;
        auto createOverlay = (PFN_CreateOverlay)vtable[kCreateOverlayIndex];
        if (!createOverlay) return 0;

        VROverlayHandle_t newHandle = 0;
        int err = createOverlay(overlayIface,
            "treadmill_wrapper_notifications",
            "Treadmill Wrapper",
            &newHandle);

        if (err != 0 || newHandle == 0) {
            LogDebug("CalibrationOverlay: CreateOverlay failed (error=%d)", err);
            return 0;
        }

        handle = newHandle;
        LogDebug("CalibrationOverlay: Overlay created (handle=0x%llX)", handle);
        return handle;
    }

    static void* GetOrCreateOverlayInterface() {
        static void* overlayIface = nullptr;
        if (overlayIface) return overlayIface;
        if (!Real_VR_GetGenericInterface) return nullptr;

        int error = 0;
        const char* versions[] = {
            "IVROverlay_027", "IVROverlay_026", "IVROverlay_025",
            "IVROverlay_024", "IVROverlay_023", "IVROverlay_022",
            "IVROverlay_021", "IVROverlay_020", "IVROverlay_019",
            "IVROverlay_018"
        };

        for (const char* ver : versions) {
            overlayIface = Real_VR_GetGenericInterface(ver, &error);
            if (overlayIface) {
                LogDebug("CalibrationOverlay: Got %s", ver);
                return overlayIface;
            }
        }

        LogDebug("CalibrationOverlay: No IVROverlay interface available (last error=%d)", error);
        return nullptr;
    }

    static void* GetOrCreateNotificationsInterface() {
        static void* notifIface = nullptr;
        if (notifIface) return notifIface;
        if (!Real_VR_GetGenericInterface) return nullptr;

        int error = 0;
        notifIface = Real_VR_GetGenericInterface("IVRNotifications_002", &error);
        if (!notifIface) {
            notifIface = Real_VR_GetGenericInterface("IVRNotifications_001", &error);
        }

        if (!notifIface) {
            LogDebug("CalibrationOverlay: IVRNotifications not available (error=%d)", error);
        }

        return notifIface;
    }

    static VROverlayHandle_t& GetOverlayHandle() {
        static VROverlayHandle_t handle = 0;
        return handle;
    }

    static std::mutex& GetMutex() {
        static std::mutex overlayMutex;
        return overlayMutex;
    }
};

}
