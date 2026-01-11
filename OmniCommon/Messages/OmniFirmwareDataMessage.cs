// Decompiled with JetBrains decompiler
// Type: OmniCommon.Messages.OmniFirmwareDataMessage
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace OmniCommon.Messages;

public class OmniFirmwareDataMessage : OmniBaseMessage
{
  public BootloaderSentence Sentence = (BootloaderSentence) null;
  public byte[] Response = (byte[]) null;

  public byte ack
  {
    get
    {
      if (this.Response == null || this.Response.Length <= 3)
        return byte.MaxValue;
      byte[] source = new byte[this.Response.Length - 2];
      for (int index = 0; index < source.Length; ++index)
        source[index] = this.Response[index + 1];
      return (byte) ((int) BootloaderSentence.ConvertKeyWordsBack(((IEnumerable<byte>) source).ToList<byte>()).ToArray()[0] >> 3 & 7);
    }
  }

  public OmniFirmwareDataMessage(BootloaderSentence data)
    : base(MessageType.OmniFirmwareDataMessage)
  {
    this.Sentence = data.DeepCopy();
  }

  public OmniFirmwareDataMessage(byte[] data)
    : base(MessageType.OmniFirmwareDataMessage)
  {
    this.Response = new byte[data.Length];
    data.CopyTo((Array) this.Response, 0);
  }
}
