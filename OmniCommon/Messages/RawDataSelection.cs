// Decompiled with JetBrains decompiler
// Type: OmniCommon.Messages.RawDataSelection
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace OmniCommon.Messages;

public class RawDataSelection
{
  public int Count = 0;
  public bool Timestamp = false;
  public List<RawPodDataMode> Pods = (List<RawPodDataMode>) null;

  public override string ToString()
  {
    string str = $"Count: {this.Count}; TimeStamp: {this.Timestamp.ToString()}";
    for (int index = 0; index < this.Count && index < this.Pods.Count; ++index)
      str += $"; Pod {index + 1}: {this.Pods[index].ToString()}";
    for (int count = this.Pods.Count; count < this.Count; ++count)
      str += $"; Pod {count + 1}: {new RawPodDataMode().ToString()}";
    return str;
  }

  public RawDataSelection DeepCopy()
  {
    RawDataSelection rawDataSelection = new RawDataSelection()
    {
      Count = this.Count,
      Timestamp = this.Timestamp,
      Pods = new List<RawPodDataMode>()
    };
    foreach (RawPodDataMode pod in this.Pods)
      rawDataSelection.Pods.Add(pod.DeepCopy());
    return rawDataSelection;
  }

  public RawDataSelection()
  {
  }

  public RawDataSelection(byte[] settings)
  {
    this.Count = (int) settings[0] >> 4 & 15;
    this.Timestamp = ((int) settings[0] & 1) == 1;
    this.Pods = new List<RawPodDataMode>();
    for (int index1 = 0; index1 < (settings.Length - 1) * 2; ++index1)
    {
      bool flag = index1 % 2 == 0;
      int index2 = (int) Math.Floor(1.0 + (double) index1 / 2.0);
      this.Pods.Add(new RawPodDataMode(flag ? (byte) ((int) settings[index2] >> 4 & 15) : (byte) ((uint) settings[index2] & 15U)));
    }
  }

  public static RawDataSelection AllOn(int count)
  {
    RawDataSelection rawDataSelection = new RawDataSelection()
    {
      Count = count,
      Timestamp = true,
      Pods = new List<RawPodDataMode>()
    };
    for (int index = 0; index < count; ++index)
      rawDataSelection.Pods.Add(new RawPodDataMode()
      {
        FrameNumber = true,
        Quaternions = true,
        Accelerometer = true,
        Gyroscope = true
      });
    return rawDataSelection;
  }

  public static RawDataSelection AllOff(int count)
  {
    RawDataSelection rawDataSelection = new RawDataSelection()
    {
      Count = count,
      Timestamp = false,
      Pods = new List<RawPodDataMode>()
    };
    for (int index = 0; index < count; ++index)
      rawDataSelection.Pods.Add(new RawPodDataMode()
      {
        FrameNumber = false,
        Quaternions = false,
        Accelerometer = false,
        Gyroscope = false
      });
    return rawDataSelection;
  }

  public static RawDataSelection AllOff()
  {
    return new RawDataSelection()
    {
      Count = 0,
      Timestamp = false,
      Pods = new List<RawPodDataMode>()
    };
  }
}
