// Decompiled with JetBrains decompiler
// Type: OmniCommon.Messages.OmniToggleLEDMessage
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

#nullable disable
namespace OmniCommon.Messages;

public class OmniToggleLEDMessage : OmniBaseMessage
{
  public byte LED
  {
    get => this.Payload[0];
    set => this.Payload[0] = value;
  }

  public OmniToggleLEDMessage(byte led)
    : base(MessageType.OmniToggleLEDMessage)
  {
    this.Payload = new byte[1];
    this.LED = led;
  }

  public OmniToggleLEDMessage(OmniBaseMessage msg)
    : base(msg)
  {
    this.MsgType = MessageType.OmniToggleLEDMessage;
  }
}
