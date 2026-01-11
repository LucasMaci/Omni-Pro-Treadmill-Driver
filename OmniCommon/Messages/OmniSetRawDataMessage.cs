// Decompiled with JetBrains decompiler
// Type: OmniCommon.Messages.OmniSetRawDataMessage
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace OmniCommon.Messages;

public class OmniSetRawDataMessage : OmniBaseMessage
{
  public int Count => (int) this.Payload[0] >> 4 & 15;

  public bool Timestamp => ((int) this.Payload[0] & 1) == 1;

  public List<RawPodDataMode> PodModes
  {
    get
    {
      List<RawPodDataMode> podModes = new List<RawPodDataMode>();
      for (int index1 = 0; index1 < this.Count && index1 < (this.Payload.Length - 1) * 2; ++index1)
      {
        bool flag = index1 % 2 == 0;
        int index2 = (int) Math.Floor(1.0 + (double) index1 / 2.0);
        byte data = (byte) ((flag ? (int) this.Payload[index2] >> 4 : (int) this.Payload[index2]) & 15);
        podModes.Add(new RawPodDataMode(data));
      }
      return podModes;
    }
  }

  public OmniSetRawDataMessage(RawDataSelection selection)
    : base(MessageType.OmniSetRawDataMessage)
  {
    this.Payload = new byte[(int) Math.Ceiling(1.0 + (double) selection.Count / 2.0)];
    this.Payload[0] = (byte) ((selection.Count << 4 & 240 /*0xF0*/) + (selection.Timestamp ? 1 : 0));
    for (int index1 = 0; index1 < selection.Count && index1 < selection.Pods.Count; ++index1)
    {
      bool flag = index1 % 2 == 0;
      int index2 = (int) Math.Floor(1.0 + (double) index1 / 2.0);
      byte num = selection.Pods[index1].GetByte();
      this.Payload[index2] += flag ? (byte) ((int) num << 4) : num;
    }
    for (int count = selection.Pods.Count; count < selection.Count; ++count)
    {
      bool flag = count % 2 == 0;
      int index = (int) Math.Floor(1.0 + (double) count / 2.0);
      byte num = new RawPodDataMode().GetByte();
      this.Payload[index] += flag ? (byte) ((int) num << 4) : num;
    }
    this.ComPort = string.Empty;
  }

  public OmniSetRawDataMessage(OmniBaseMessage msg)
    : base(msg)
  {
    this.MsgType = MessageType.OmniSetRawDataMessage;
  }
}
