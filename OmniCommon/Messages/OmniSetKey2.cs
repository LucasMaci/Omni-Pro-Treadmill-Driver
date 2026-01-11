// Decompiled with JetBrains decompiler
// Type: OmniCommon.Messages.OmniSetKey2
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

using System;

#nullable disable
namespace OmniCommon.Messages;

public class OmniSetKey2 : OmniBaseMessage
{
  private const int ExactLength = 64 /*0x40*/;

  public OmniSetKey2(byte[] Key)
    : base(MessageType.OmniSetKey2)
  {
    this.Payload = Key.Length == 64 /*0x40*/ ? Key : throw new Exception("Invalid Data length");
  }

  public OmniSetKey2(OmniBaseMessage msg)
    : base(msg)
  {
    this.MsgType = MessageType.OmniSetKey2;
  }
}
