// Decompiled with JetBrains decompiler
// Type: OmniCommon.Messages.GenericMessage
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

using System;

#nullable disable
namespace OmniCommon.Messages;

public class GenericMessage : OmniBaseMessage
{
  public byte CommandToUse;

  public GenericMessage(byte cmd, byte[] payload)
    : base(MessageType.GenericMessage)
  {
    this.CommandToUse = cmd;
    this.Payload = new byte[payload.Length];
    payload.CopyTo((Array) this.Payload, 0);
  }
}
