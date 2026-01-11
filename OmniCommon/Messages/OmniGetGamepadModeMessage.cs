// Decompiled with JetBrains decompiler
// Type: OmniCommon.Messages.OmniGetGamepadModeMessage
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

#nullable disable
namespace OmniCommon.Messages;

public class OmniGetGamepadModeMessage : OmniBaseMessage
{
  public OmniMode GameMode => (OmniMode) this.Payload[0];

  public OmniGetGamepadModeMessage()
    : base(MessageType.OmniGetGamepadModeMessage)
  {
    this.Payload = new byte[0];
  }

  public OmniGetGamepadModeMessage(OmniBaseMessage msg)
    : base(msg)
  {
    this.MsgType = MessageType.OmniGetGamepadModeMessage;
  }
}
