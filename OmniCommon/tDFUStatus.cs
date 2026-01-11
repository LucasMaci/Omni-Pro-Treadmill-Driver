// Decompiled with JetBrains decompiler
// Type: OmniCommon.tDFUStatus
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

#nullable disable
namespace OmniCommon;

public enum tDFUStatus
{
  STATUS_OK,
  STATUS_ERR_TARGET,
  STATUS_ERR_FILE,
  STATUS_ERR_WRITE,
  STATUS_ERR_ERASE,
  STATUS_ERR_CHECK_ERASED,
  STATUS_ERR_PROG,
  STATUS_ERR_VERIFY,
  STATUS_ERR_ADDRESS,
  STATUS_ERR_NOTDONE,
  STATUS_ERR_FIRMWARE,
  STATUS_ERR_VENDOR,
  STATUS_ERR_USBR,
  STATUS_ERR_POR,
  STATUS_ERR_UNKNOWN,
  STATUS_ERR_STALLEDPKT,
}
