// Decompiled with JetBrains decompiler
// Type: OmniCommon.BootloaderUpdater
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace OmniCommon;

public static class BootloaderUpdater
{
  private const uint DFU_START_PACKET = 2;
  private const uint DFU_DATA_PACKET = 3;
  private const uint DFU_END_PACKET = 4;
  private const int DataPerSentence = 512 /*0x0200*/;
  private static MinMax SoftDeviceRange = new MinMax()
  {
    Min = 0,
    Max = 90112 /*0x016000*/
  };
  private static MinMax ApplicationRange = new MinMax()
  {
    Min = 90112 /*0x016000*/,
    Max = 159744 /*0x027000*/
  };
  private static MinMax BootloaderRange = new MinMax()
  {
    Min = 229376 /*0x038000*/,
    Max = 262144 /*0x040000*/
  };

  private static bool ContainsData(MinMax compareRange, IntelHexData data)
  {
    foreach (MinMax minMax in data.Range)
    {
      if (minMax.Min < compareRange.Max && minMax.Max >= compareRange.Min)
        return true;
    }
    return false;
  }

  private static bool ContainsSoftDeviceData(IntelHexData data)
  {
    return BootloaderUpdater.ContainsData(BootloaderUpdater.SoftDeviceRange, data);
  }

  private static bool ContainsApplicationData(IntelHexData data)
  {
    return BootloaderUpdater.ContainsData(BootloaderUpdater.ApplicationRange, data);
  }

  private static bool ContainsBootloaderData(IntelHexData data)
  {
    return BootloaderUpdater.ContainsData(BootloaderUpdater.BootloaderRange, data);
  }

  private static HexType GetHexType(IntelHexData data)
  {
    if (BootloaderUpdater.ContainsSoftDeviceData(data) && BootloaderUpdater.ContainsBootloaderData(data) && !BootloaderUpdater.ContainsApplicationData(data))
      return HexType.SD_BL;
    if (!BootloaderUpdater.ContainsSoftDeviceData(data) && !BootloaderUpdater.ContainsBootloaderData(data) && BootloaderUpdater.ContainsApplicationData(data))
      return HexType.APPLICATION;
    if (!BootloaderUpdater.ContainsSoftDeviceData(data) && BootloaderUpdater.ContainsBootloaderData(data) && !BootloaderUpdater.ContainsApplicationData(data))
      return HexType.BOOTLOADER;
    return BootloaderUpdater.ContainsSoftDeviceData(data) && !BootloaderUpdater.ContainsBootloaderData(data) && !BootloaderUpdater.ContainsApplicationData(data) ? HexType.SOFTDEVICE : HexType.NONE;
  }

  private static BootloaderSentence CreateStartSentence(IntelHexData data)
  {
    List<byte> byteList = new List<byte>();
    byteList.AddRange((IEnumerable<byte>) BootloaderUpdater.convert_uint32_to_array(2U));
    HexType hexType = BootloaderUpdater.GetHexType(data);
    if (hexType == HexType.NONE)
      throw new Exception("Invalid input data");
    byteList.AddRange((IEnumerable<byte>) BootloaderUpdater.convert_uint32_to_array((uint) hexType));
    if (hexType == HexType.SOFTDEVICE || hexType == HexType.SD_BL)
      byteList.AddRange((IEnumerable<byte>) BootloaderUpdater.convert_uint32_to_array(data.GetRange(BootloaderUpdater.SoftDeviceRange).GetLength(true)));
    else
      byteList.AddRange((IEnumerable<byte>) new byte[4]);
    if (hexType == HexType.BOOTLOADER || hexType == HexType.SD_BL)
      byteList.AddRange((IEnumerable<byte>) BootloaderUpdater.convert_uint32_to_array(data.GetRange(BootloaderUpdater.BootloaderRange).GetLength(true)));
    else
      byteList.AddRange((IEnumerable<byte>) new byte[4]);
    if (hexType == HexType.APPLICATION)
      byteList.AddRange((IEnumerable<byte>) BootloaderUpdater.convert_uint32_to_array(data.GetRange(BootloaderUpdater.ApplicationRange).GetLength(true)));
    else
      byteList.AddRange((IEnumerable<byte>) new byte[4]);
    return new BootloaderSentence(byteList.ToArray());
  }

  private static BootloaderSentence CreateStopSentence(IntelHexData data)
  {
    List<byte> byteList = new List<byte>();
    byteList.AddRange((IEnumerable<byte>) BootloaderUpdater.convert_uint32_to_array(4U));
    return new BootloaderSentence(byteList.ToArray());
  }

  private static List<BootloaderSentence> CreateDataSentences(IntelHexData data)
  {
    List<BootloaderSentence> dataSentences = new List<BootloaderSentence>();
    List<byte> byteList = new List<byte>();
    HexType hexType = BootloaderUpdater.GetHexType(data);
    if (hexType == HexType.SOFTDEVICE || hexType == HexType.SD_BL)
      byteList.AddRange((IEnumerable<byte>) data.GetDatainRange(data.GetRanges(BootloaderUpdater.SoftDeviceRange)));
    if (hexType == HexType.BOOTLOADER || hexType == HexType.SD_BL)
      byteList.AddRange((IEnumerable<byte>) data.GetDatainRange(data.GetRanges(BootloaderUpdater.BootloaderRange)));
    if (hexType == HexType.APPLICATION)
      byteList.AddRange((IEnumerable<byte>) data.GetDatainRange(data.GetRanges(BootloaderUpdater.ApplicationRange)));
    for (int index = 0; index < byteList.Count; index += 512 /*0x0200*/)
    {
      byte[] numArray = new byte[4 + Math.Min(512 /*0x0200*/, byteList.Count - index)];
      BootloaderUpdater.convert_uint32_to_array(3U).CopyTo((Array) numArray, 0);
      byteList.CopyTo(index, numArray, 4, Math.Min(512 /*0x0200*/, byteList.Count - index));
      dataSentences.Add(new BootloaderSentence(numArray));
    }
    return dataSentences;
  }

  public static List<BootloaderSentence> GetBootloaderSentences(IntelHexData data)
  {
    List<BootloaderSentence> bootloaderSentences = new List<BootloaderSentence>();
    BootloaderSentence.ResetSequenceNumber();
    bootloaderSentences.Add(BootloaderUpdater.CreateStartSentence(data));
    bootloaderSentences.AddRange((IEnumerable<BootloaderSentence>) BootloaderUpdater.CreateDataSentences(data));
    bootloaderSentences.Add(BootloaderUpdater.CreateStopSentence(data));
    return bootloaderSentences;
  }

  private static byte[] convert_uint32_to_array(uint val)
  {
    return new byte[4]
    {
      (byte) (val & (uint) byte.MaxValue),
      (byte) ((val & 65280U) >> 8),
      (byte) ((val & 16711680U /*0xFF0000*/) >> 16 /*0x10*/),
      (byte) ((val & 4278190080U /*0xFF000000*/) >> 24)
    };
  }
}
