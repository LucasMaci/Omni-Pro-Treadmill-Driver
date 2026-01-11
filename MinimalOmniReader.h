#pragma once

#ifdef __cplusplus
extern "C" {
#endif

	typedef void (*OmniDataCallback)(float ringAngle, int gamePadX, int gamePadY);

	void* OmniReader_Create();
	bool OmniReader_Initialize(void* handle, const char* comPort, int omniMode, int baudRate);
	void OmniReader_RegisterCallback(void* handle, OmniDataCallback callback);
	void OmniReader_Disconnect(void* handle);
	void OmniReader_Destroy(void* handle);

#ifdef __cplusplus
}
#endif