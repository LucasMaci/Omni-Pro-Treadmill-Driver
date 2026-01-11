// Decompiled with JetBrains decompiler
// Type: OmniCommon.Messages.OmniChangeGamepadModeMessage
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

#nullable disable
namespace OmniCommon.Messages;

public class OmniChangeGamepadModeMessage : OmniBaseMessage
{
  public byte Mode
  {
    get => this.Payload[0];
    set => this.Payload[0] = value;
  }

  public OmniChangeGamepadModeMessage(byte mode)
    : base(MessageType.OmniChangeGamepadModeMessage)
  {
    this.Payload = new byte[1];
    this.Mode = mode;
  }

  public OmniChangeGamepadModeMessage(OmniBaseMessage msg)
    : base(msg)
  {
    this.MsgType = MessageType.OmniChangeGamepadModeMessage;
  }
}
