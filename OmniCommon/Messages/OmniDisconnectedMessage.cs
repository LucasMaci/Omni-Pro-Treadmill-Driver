// Decompiled with JetBrains decompiler
// Type: OmniCommon.Messages.OmniDisconnectedMessage
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

#nullable disable
namespace OmniCommon.Messages;

public class OmniDisconnectedMessage : OmniBaseMessage
{
  public bool Errored = false;

  public OmniDisconnectedMessage()
    : base(MessageType.OmniDisconnectedMessage)
  {
  }
}
