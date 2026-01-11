// Decompiled with JetBrains decompiler
// Type: OmniCommon.IntelHexData
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace OmniCommon;

public class IntelHexData
{
  public List<IntelHexStructure> Data = new List<IntelHexStructure>();
  public List<MinMax> Range = new List<MinMax>();

  public void AddData(IntelHexStructure input)
  {
    this.Data.Add(input);
    if (this.Range.Count == 0)
    {
      this.Range.Add(new MinMax()
      {
        Min = input.address,
        Max = (uint) ((ulong) input.address + (ulong) input.dataLen)
      });
    }
    else
    {
      bool flag = false;
      for (int index = 0; index < this.Range.Count && !flag; ++index)
      {
        if ((long) this.Range[index].Min == (long) input.address + (long) input.dataLen)
        {
          this.Range[index].Min = input.address;
          flag = true;
        }
        else if ((int) this.Range[index].Max == (int) input.address)
        {
          this.Range[index].Max = (uint) ((ulong) input.address + (ulong) input.dataLen);
          flag = true;
        }
      }
      if (!flag)
        this.Range.Add(new MinMax()
        {
          Min = input.address,
          Max = (uint) ((ulong) input.address + (ulong) input.dataLen)
        });
    }
    List<MinMax> minMaxList = new List<MinMax>();
    for (int index1 = 0; index1 < this.Range.Count; ++index1)
    {
      for (int index2 = index1; index2 < this.Range.Count - 1; ++index2)
      {
        if ((int) this.Range[index1].Min == (int) this.Range[index2].Max + 1)
        {
          this.Range[index2].Max = this.Range[index1].Max;
          this.Range[index1].Min = 0U;
          this.Range[index1].Max = 0U;
          minMaxList.Add(this.Range[index1]);
        }
        else if ((int) this.Range[index1].Max + 1 == (int) this.Range[index2].Min)
        {
          this.Range[index1].Max = this.Range[index2].Max;
          this.Range[index2].Min = 0U;
          this.Range[index2].Max = 0U;
          minMaxList.Add(this.Range[index2]);
        }
      }
    }
    for (int index = 0; index < minMaxList.Count; ++index)
      this.Range.Remove(minMaxList[index]);
  }

  public byte[] GetDatainRange(List<MinMax> ranges)
  {
    List<byte> byteList = new List<byte>();
    if (ranges == null || ranges.Count == 0)
      return (byte[]) null;
    ranges.Sort((Comparison<MinMax>) ((a, b) => a.Min.CompareTo(b.Min)));
    uint currentIndex = ranges[0].Min;
    foreach (MinMax range in ranges)
    {
      if (currentIndex < range.Min)
      {
        for (uint index = currentIndex; index < range.Min; ++index)
          byteList.Add(byte.MaxValue);
        currentIndex = range.Min;
      }
      IntelHexStructure intelHexStructure;
      for (; currentIndex < range.Max; currentIndex += (uint) intelHexStructure.dataLen)
      {
        intelHexStructure = this.Data.Find((Predicate<IntelHexStructure>) (it => (int) it.address == (int) currentIndex));
        byteList.AddRange((IEnumerable<byte>) intelHexStructure.data);
      }
    }
    return byteList.ToArray();
  }

  public MinMax GetRange(MinMax targetRange)
  {
    List<MinMax> minMaxList = new List<MinMax>();
    foreach (MinMax range in this.Range)
    {
      if (IntelHexData.IsWithin(targetRange.Min, range) || IntelHexData.IsWithin(targetRange.Max, range) || IntelHexData.IsWithin(range.Min, targetRange) || IntelHexData.IsWithin(range.Max, targetRange))
        minMaxList.Add(new MinMax()
        {
          Min = Math.Max(targetRange.Min, range.Min),
          Max = Math.Min(targetRange.Max, range.Max)
        });
    }
    if (minMaxList.Count == 0)
      return (MinMax) null;
    MinMax range1 = new MinMax()
    {
      Min = minMaxList[0].Min,
      Max = minMaxList[0].Max
    };
    foreach (MinMax minMax in minMaxList)
    {
      range1.Min = Math.Min(range1.Min, minMax.Min);
      range1.Max = Math.Max(range1.Max, minMax.Max);
    }
    return range1;
  }

  public List<MinMax> GetRanges(MinMax targetRange)
  {
    List<MinMax> ranges = new List<MinMax>();
    foreach (MinMax range in this.Range)
    {
      if (IntelHexData.IsWithin(targetRange.Min, range) || IntelHexData.IsWithin(targetRange.Max, range) || IntelHexData.IsWithin(range.Min, targetRange) || IntelHexData.IsWithin(range.Max, targetRange))
        ranges.Add(new MinMax()
        {
          Min = Math.Max(targetRange.Min, range.Min),
          Max = Math.Min(targetRange.Max, range.Max)
        });
    }
    return ranges;
  }

  private static bool IsWithin(uint number, MinMax range)
  {
    return number >= range.Min && number < range.Max;
  }
}
