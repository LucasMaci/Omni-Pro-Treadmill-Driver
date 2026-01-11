// Decompiled with JetBrains decompiler
// Type: OmniCommon.Messages.OmniRawDataMessage
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace OmniCommon.Messages;

public class OmniRawDataMessage : OmniBaseMessage
{
  public int Count => (int) this.Payload[0] >> 4 & 15;

  public bool EnableTimestamp => ((int) this.Payload[0] & 1) == 1;

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

  public OmniRawData GetRawData()
  {
    OmniRawData rawData = new OmniRawData()
    {
      Count = this.Count,
      EnableTimestamp = this.EnableTimestamp,
      Pods = this.PodModes
    };
    rawData.PodData = new List<PodRawData>();
    int startIndex = (int) Math.Ceiling(1.0 + (double) this.Count / 2.0);
    if (this.EnableTimestamp)
    {
      rawData.EnableTimestamp = true;
      rawData.Timestamp = BitConverter.ToUInt32(this.Payload, startIndex);
      startIndex += 4;
    }
    foreach (RawPodDataMode pod in rawData.Pods)
    {
      int dataUsed = 0;
      rawData.PodData.Add(new PodRawData(pod, this.Payload, startIndex, out dataUsed));
      startIndex += dataUsed;
    }
    return rawData;
  }

  public OmniRawDataMessage(OmniBaseMessage msg)
    : base(msg)
  {
    this.MsgType = MessageType.OmniRawDataMessage;
  }
}
