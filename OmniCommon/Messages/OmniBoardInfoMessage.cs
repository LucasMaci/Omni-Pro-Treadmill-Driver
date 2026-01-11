// Decompiled with JetBrains decompiler
// Type: OmniCommon.Messages.OmniBoardInfoMessage
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

#nullable disable
namespace OmniCommon.Messages;

public class OmniBoardInfoMessage : OmniBaseMessage
{
  public pod_types LocationAssignment => (pod_types) this.Payload[0];

  public string FirmwareVersion
  {
    get => $"{this.Payload[1].ToString("X2")}.{this.Payload[2].ToString("X2")}";
  }

  public string HardwareVersion
  {
    get
    {
      return $"{this.Payload[3].ToString("X2")}.{(this.Payload[4] < (byte) 15 ? (int) this.Payload[4] + 1 : 0).ToString("X2")}";
    }
  }

  public string RadioSiliconVersion
  {
    get
    {
      return $"{this.Payload[8].ToString("X2")}.{this.Payload[9].ToString("X2")}.{this.Payload[10].ToString("X2")}.{this.Payload[11].ToString("X2")}";
    }
  }

  public string RadioSiliconHardwareVersion
  {
    get => $"{this.Payload[8].ToString("X2")}.{this.Payload[9].ToString("X2")}";
  }

  public string RadioSiliconFirmwareVersion
  {
    get => $"{this.Payload[10].ToString("X2")}.{this.Payload[11].ToString("X2")}";
  }

  public string MACAddress
  {
    get
    {
      return $"{this.Payload[12].ToString("X2")}:{this.Payload[13].ToString("X2")}:{this.Payload[14].ToString("X2")}:{this.Payload[15].ToString("X2")}:{this.Payload[16 /*0x10*/].ToString("X2")}:{this.Payload[17].ToString("X2")}";
    }
  }

  public OmniBoardInfoMessage()
    : base(MessageType.OmniBoardInfoMessage)
  {
    this.Payload = new byte[0];
  }

  public OmniBoardInfoMessage(OmniBaseMessage msg)
    : base(msg)
  {
    this.MsgType = MessageType.OmniBoardInfoMessage;
  }

  public BoardInfo GetInfo()
  {
    return new BoardInfo()
    {
      LocationAssignment = this.LocationAssignment.ToString(),
      FirmwareVersion = this.FirmwareVersion,
      HardwareVersion = this.HardwareVersion,
      RadioSiliconFirmwareVersion = this.RadioSiliconFirmwareVersion,
      RadioSiliconHardwareVersion = this.RadioSiliconHardwareVersion,
      MACAddress = this.MACAddress
    };
  }
}
