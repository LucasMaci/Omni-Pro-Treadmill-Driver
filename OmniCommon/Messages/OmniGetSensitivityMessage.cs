// Decompiled with JetBrains decompiler
// Type: OmniCommon.Messages.OmniGetSensitivityMessage
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

using System;

#nullable disable
namespace OmniCommon.Messages;

public class OmniGetSensitivityMessage : OmniBaseMessage
{
  public float Sensitivity
  {
    get => BitConverter.ToSingle(this.Payload, 0);
    set => this.Payload = BitConverter.GetBytes(value);
  }

  public OmniGetSensitivityMessage()
    : base(MessageType.OmniGetSensitivityMessage)
  {
    this.Payload = new byte[0];
  }

  public OmniGetSensitivityMessage(OmniBaseMessage msg)
    : base(msg)
  {
    this.MsgType = MessageType.OmniGetSensitivityMessage;
  }
}
