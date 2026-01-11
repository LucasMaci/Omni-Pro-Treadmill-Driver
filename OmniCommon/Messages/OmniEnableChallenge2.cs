// Decompiled with JetBrains decompiler
// Type: OmniCommon.Messages.OmniEnableChallenge2
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

#nullable disable
namespace OmniCommon.Messages;

public class OmniEnableChallenge2 : OmniBaseMessage
{
  public byte[] Key2Hashed => this.Payload;

  public bool Success => this.Payload[0] > (byte) 0;

  public OmniEnableChallenge2(byte[] key2)
    : base(MessageType.OmniEnableChallenge2)
  {
    this.Payload = key2;
  }

  public OmniEnableChallenge2(OmniBaseMessage msg)
    : base(msg)
  {
    this.MsgType = MessageType.OmniEnableChallenge2;
  }
}
