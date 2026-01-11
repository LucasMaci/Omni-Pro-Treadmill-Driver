// Decompiled with JetBrains decompiler
// Type: OmniCommon.Messages.OmniSetSensitivityMessage
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

using System;

#nullable disable
namespace OmniCommon.Messages;

public class OmniSetSensitivityMessage : OmniBaseMessage
{
  public float Sensitivity
  {
    get => BitConverter.ToSingle(this.Payload, 0);
    set => this.Payload = BitConverter.GetBytes(value);
  }

  public OmniSetSensitivityMessage(float sensitivity)
    : base(MessageType.OmniSetSensitivityMessage)
  {
    this.Payload = new byte[4];
    this.Sensitivity = sensitivity;
  }

  public OmniSetSensitivityMessage(OmniBaseMessage msg)
    : base(msg)
  {
    this.MsgType = MessageType.OmniSetSensitivityMessage;
  }
}
