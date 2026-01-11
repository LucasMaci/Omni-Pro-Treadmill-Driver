#pragma once

#include <windows.h>
#include "openvr_driver.h"
#include "TreadmillDevice.h"
#include <atomic>
#include <thread>
#include <memory>

typedef void (*OmniDataCallback)(float ringAngle, int gamePadX, int gamePadY);

class TreadmillServerDriver : public vr::IServerTrackedDeviceProvider {
public:
    vr::EVRInitError Init(vr::IVRDriverContext* pDriverContext) override;
    void Cleanup() override;
    const char* const* GetInterfaceVersions() override;
    void RunFrame() override;
    bool ShouldBlockStandbyMode() override;
    void EnterStandby() override;
    void LeaveStandby() override;

private:
    HMODULE omniReaderLib = nullptr;
    std::unique_ptr<TreadmillDevice> m_device;
    
    void* m_omniReader = nullptr;
    
    typedef void* (*PFN_OmniReader_Create)();
    typedef bool (*PFN_OmniReader_Initialize)(void*, const char*, int, int);
    typedef void (*PFN_OmniReader_RegisterCallback)(void*, OmniDataCallback);
    typedef void (*PFN_OmniReader_Disconnect)(void*);
    typedef void (*PFN_OmniReader_Destroy)(void*);
    
    PFN_OmniReader_Create pfnCreate = nullptr;
    PFN_OmniReader_Initialize pfnInitialize = nullptr;
    PFN_OmniReader_RegisterCallback pfnRegisterCallback = nullptr;
    PFN_OmniReader_Disconnect pfnDisconnect = nullptr;
    PFN_OmniReader_Destroy pfnDestroy = nullptr;

    std::unique_ptr<TreadmillVisualTracker> m_visualTracker;  // NEU!
};