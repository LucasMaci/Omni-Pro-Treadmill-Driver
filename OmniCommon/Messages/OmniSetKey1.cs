// Decompiled with JetBrains decompiler
// Type: OmniCommon.Messages.OmniSetKey1
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

using System;

#nullable disable
namespace OmniCommon.Messages;

public class OmniSetKey1 : OmniBaseMessage
{
  private const int ExactLength = 64 /*0x40*/;

  public OmniSetKey1(byte[] Key)
    : base(MessageType.OmniSetKey1)
  {
    this.Payload = Key.Length == 64 /*0x40*/ ? Key : throw new Exception("Invalid Data length");
  }

  public OmniSetKey1(OmniBaseMessage msg)
    : base(msg)
  {
    this.MsgType = MessageType.OmniSetKey1;
  }
}
