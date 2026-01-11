// Decompiled with JetBrains decompiler
// Type: OmniCommon.Messages.OmniResetRadioMessage
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

#nullable disable
namespace OmniCommon.Messages;

public class OmniResetRadioMessage : OmniBaseMessage
{
  public RadioNumber Radio
  {
    get => (RadioNumber) this.Payload[0];
    set => this.Payload[0] = (byte) value;
  }

  public OmniResetRadioMessage()
    : base(MessageType.OmniResetRadioMessage)
  {
    this.Payload = new byte[1];
  }

  public OmniResetRadioMessage(RadioNumber radio)
    : base(MessageType.OmniResetRadioMessage)
  {
    this.Payload = new byte[1];
    this.Radio = radio;
  }

  public OmniResetRadioMessage(OmniBaseMessage msg)
    : base(msg)
  {
    this.MsgType = MessageType.OmniResetRadioMessage;
  }
}
