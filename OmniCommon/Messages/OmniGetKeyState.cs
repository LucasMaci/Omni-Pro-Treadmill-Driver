// Decompiled with JetBrains decompiler
// Type: OmniCommon.Messages.OmniGetKeyState
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

#nullable disable
namespace OmniCommon.Messages;

public class OmniGetKeyState : OmniBaseMessage
{
  public bool Programmed => this.Payload[0] > (byte) 0;

  public OmniGetKeyState()
    : base(MessageType.OmniGetKeyState)
  {
    this.Payload = new byte[0];
  }

  public OmniGetKeyState(OmniBaseMessage msg)
    : base(msg)
  {
    this.MsgType = MessageType.OmniGetKeyState;
  }
}
