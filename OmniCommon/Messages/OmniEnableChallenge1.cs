// Decompiled with JetBrains decompiler
// Type: OmniCommon.Messages.OmniEnableChallenge1
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

using System;

#nullable disable
namespace OmniCommon.Messages;

public class OmniEnableChallenge1 : OmniBaseMessage
{
  public uint Key1Random => BitConverter.ToUInt32(this.Payload, 0);

  public byte[] Key1Hashed
  {
    get
    {
      byte[] destinationArray = new byte[32 /*0x20*/];
      Array.Copy((Array) this.Payload, 0, (Array) destinationArray, 0, 32 /*0x20*/);
      return destinationArray;
    }
  }

  public uint Key2Random => BitConverter.ToUInt32(this.Payload, 32 /*0x20*/);

  public OmniEnableChallenge1(uint randomInput)
    : base(MessageType.OmniEnableChallenge1)
  {
    this.Payload = BitConverter.GetBytes(randomInput);
  }

  public OmniEnableChallenge1(OmniBaseMessage msg)
    : base(msg)
  {
    this.MsgType = MessageType.OmniEnableChallenge1;
  }
}
