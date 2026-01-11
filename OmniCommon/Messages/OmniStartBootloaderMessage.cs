// Decompiled with JetBrains decompiler
// Type: OmniCommon.Messages.OmniStartBootloaderMessage
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

#nullable disable
namespace OmniCommon.Messages;

public class OmniStartBootloaderMessage : OmniBaseMessage
{
  public OmniStartBootloaderMessage()
    : base(MessageType.OmniStartBootloaderMessage)
  {
    this.Payload = new byte[0];
    this.ComPort = string.Empty;
  }

  public OmniStartBootloaderMessage(OmniBaseMessage msg)
    : base(msg)
  {
    this.MsgType = MessageType.OmniStartBootloaderMessage;
  }
}
