// Decompiled with JetBrains decompiler
// Type: OmniCommon.Messages.OmniUpgradeRadioMessage
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

#nullable disable
namespace OmniCommon.Messages;

public class OmniUpgradeRadioMessage : OmniBaseMessage
{
  public RadioNumber Radio
  {
    get => (RadioNumber) this.Payload[0];
    set => this.Payload[0] = (byte) value;
  }

  public OmniUpgradeRadioMessage()
    : base(MessageType.OmniUpgradeRadioMessage)
  {
    this.Payload = new byte[1];
    this.Radio = RadioNumber.Radio2;
    this.ComPort = string.Empty;
  }

  public OmniUpgradeRadioMessage(RadioNumber radio)
    : base(MessageType.OmniUpgradeRadioMessage)
  {
    this.Payload = new byte[1];
    this.Radio = radio;
    this.ComPort = string.Empty;
  }

  public OmniUpgradeRadioMessage(OmniBaseMessage msg)
    : base(msg)
  {
    this.MsgType = MessageType.OmniUpgradeRadioMessage;
  }
}
