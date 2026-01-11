// Decompiled with JetBrains decompiler
// Type: OmniCommon.Messages.OmniVersionInfoMessage
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

using System;

#nullable disable
namespace OmniCommon.Messages;

public class OmniVersionInfoMessage : OmniBaseMessage
{
  public string HardwareVersion
  {
    get => $"{this.Payload[1].ToString("X2")}.{this.Payload[0].ToString("X2")}";
  }

  public string FirmwareVersion
  {
    get => $"{this.Payload[3].ToString("X2")}.{this.Payload[2].ToString("X2")}";
  }

  public string BuildDate => $"{BitConverter.ToInt32(this.Payload, 4)}";

  public OmniVersionInfoMessage()
    : base(MessageType.OmniVersionInfoMessage)
  {
    this.Payload = new byte[0];
  }

  public OmniVersionInfoMessage(OmniBaseMessage msg)
    : base(msg)
  {
    this.MsgType = MessageType.OmniVersionInfoMessage;
  }

  public OmniVersionInfo GetInfo()
  {
    return new OmniVersionInfo()
    {
      HardwareVersion = this.HardwareVersion,
      FirmwareVersion = this.FirmwareVersion,
      BuildDate = this.BuildDate
    };
  }
}
