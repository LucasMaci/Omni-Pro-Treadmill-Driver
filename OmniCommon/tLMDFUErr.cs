// Decompiled with JetBrains decompiler
// Type: OmniCommon.tLMDFUErr
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

#nullable disable
namespace OmniCommon;

public enum tLMDFUErr
{
  DFU_ERR_VERIFY_FAIL = -14, // 0xFFFFFFF2
  DFU_ERR_CANT_VERIFY = -13, // 0xFFFFFFF3
  DFU_ERR_DNLOAD_FAIL = -12, // 0xFFFFFFF4
  DFU_ERR_STALL = -11, // 0xFFFFFFF5
  DFU_ERR_TIMEOUT = -10, // 0xFFFFFFF6
  DFU_ERR_DISCONNECTED = -9, // 0xFFFFFFF7
  DFU_ERR_INVALID_SIZE = -8, // 0xFFFFFFF8
  DFU_ERR_INVALID_ADDR = -7, // 0xFFFFFFF9
  DFU_ERR_INVALID_FORMAT = -6, // 0xFFFFFFFA
  DFU_ERR_UNSUPPORTED = -5, // 0xFFFFFFFB
  DFU_ERR_UNKNOWN = -4, // 0xFFFFFFFC
  DFU_ERR_NOT_FOUND = -3, // 0xFFFFFFFD
  DFU_ERR_MEMORY = -2, // 0xFFFFFFFE
  DFU_ERR_HANDLE = -1, // 0xFFFFFFFF
  DFU_OK = 0,
}
