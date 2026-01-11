// Decompiled with JetBrains decompiler
// Type: OmniCommon.Messages.OmniRawData
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace OmniCommon.Messages;

public class OmniRawData
{
  public int Count = 0;
  public bool EnableTimestamp = false;
  public List<RawPodDataMode> Pods = (List<RawPodDataMode>) null;
  public uint Timestamp = 0;
  public List<PodRawData> PodData = (List<PodRawData>) null;

  public OmniRawData DeepCopy()
  {
    OmniRawData omniRawData = new OmniRawData()
    {
      Count = this.Count,
      EnableTimestamp = this.EnableTimestamp,
      Timestamp = this.Timestamp
    };
    if (this.Pods == null)
    {
      omniRawData.Pods = (List<RawPodDataMode>) null;
    }
    else
    {
      omniRawData.Pods = new List<RawPodDataMode>();
      foreach (RawPodDataMode pod in this.Pods)
        omniRawData.Pods.Add(pod.DeepCopy());
    }
    if (this.PodData == null)
    {
      omniRawData.PodData = (List<PodRawData>) null;
    }
    else
    {
      omniRawData.PodData = new List<PodRawData>();
      foreach (PodRawData podRawData in this.PodData)
        omniRawData.PodData.Add(podRawData.DeepCopy());
    }
    return omniRawData;
  }

  public override string ToString()
  {
    string str = $"Header, {(ValueType) (byte) (((this.Count & 15) << 4) + (this.EnableTimestamp ? 1 : 0)):X2}";
    if (this.Pods != null)
    {
      foreach (RawPodDataMode pod in this.Pods)
        str += $"{pod.GetByte():X1}";
    }
    if (this.EnableTimestamp)
      str += $", TimeStamp, {this.Timestamp}";
    if (this.PodData != null)
    {
      str += ", Data, ";
      for (int index = 0; index < this.PodData.Count; ++index)
      {
        if (index != 0)
          str += ", ";
        str += $"Pod {index + 1}, {this.PodData[index].ToString()}";
      }
    }
    return str;
  }
}
