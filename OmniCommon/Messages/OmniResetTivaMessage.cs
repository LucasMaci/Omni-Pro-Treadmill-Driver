// Decompiled with JetBrains decompiler
// Type: OmniCommon.Messages.OmniResetTivaMessage
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

#nullable disable
namespace OmniCommon.Messages;

public class OmniResetTivaMessage : OmniBaseMessage
{
  public OmniResetTivaMessage()
    : base(MessageType.OmniResetTivaMessage)
  {
    this.Payload = new byte[0];
  }

  public OmniResetTivaMessage(OmniBaseMessage msg)
    : base(msg)
  {
    this.MsgType = MessageType.OmniResetTivaMessage;
  }
}
