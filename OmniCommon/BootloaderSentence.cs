// Decompiled with JetBrains decompiler
// Type: OmniCommon.BootloaderSentence
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

using System.Collections.Generic;

#nullable disable
namespace OmniCommon;

public class BootloaderSentence
{
  private const byte Start_Stop_Byte = 192 /*0xC0*/;
  private const uint DATA_INTEGRITY_CHECK_PRESENT = 1;
  private const uint RELIABLE_PACKET = 1;
  private const uint HCI_PACKET_TYPE = 14;
  private static int sequenceNumber = 0;
  public List<byte> data_in = new List<byte>();
  public List<byte> data_converted = new List<byte>();

  public BootloaderSentence()
  {
  }

  public BootloaderSentence(byte[] data)
  {
    this.data_in = new List<byte>();
    this.data_in.AddRange((IEnumerable<byte>) data);
    List<byte> data1 = new List<byte>();
    data1.AddRange((IEnumerable<byte>) this.CreateSentenceHeader((uint) data.Length));
    data1.AddRange((IEnumerable<byte>) data);
    ushort num = BootloaderCRC.CRC(data1.ToArray());
    data1.Add((byte) ((uint) num & (uint) byte.MaxValue));
    data1.Add((byte) (((int) num & 65280) >> 8));
    List<byte> collection = this.ConvertKeyWords(data1);
    this.data_converted.Add((byte) 192 /*0xC0*/);
    this.data_converted.AddRange((IEnumerable<byte>) collection);
    this.data_converted.Add((byte) 192 /*0xC0*/);
  }

  internal static void ResetSequenceNumber() => BootloaderSentence.sequenceNumber = 0;

  private List<byte> ConvertKeyWords(List<byte> data)
  {
    List<byte> byteList = new List<byte>();
    for (int index = 0; index < data.Count; ++index)
    {
      if (data[index] == (byte) 192 /*0xC0*/)
        byteList.AddRange((IEnumerable<byte>) new byte[2]
        {
          (byte) 219,
          (byte) 220
        });
      else if (data[index] == (byte) 219)
        byteList.AddRange((IEnumerable<byte>) new byte[2]
        {
          (byte) 219,
          (byte) 221
        });
      else
        byteList.Add(data[index]);
    }
    return byteList;
  }

  public static List<byte> ConvertKeyWordsBack(List<byte> data)
  {
    List<byte> byteList = new List<byte>();
    for (int index = 0; index < data.Count - 1; ++index)
    {
      if (data[index] == (byte) 219)
      {
        if (data[index + 1] == (byte) 220)
          byteList.Add((byte) 192 /*0xC0*/);
        else if (data[index + 1] == (byte) 221)
          byteList.Add((byte) 219);
        ++index;
      }
      else
        byteList.Add(data[index]);
    }
    byteList.Add(data[data.Count - 1]);
    return byteList;
  }

  private byte[] CreateSentenceHeader(uint dataLength)
  {
    byte[] sentenceHeader = new byte[4];
    BootloaderSentence.sequenceNumber = (BootloaderSentence.sequenceNumber + 1) % 8;
    sentenceHeader[0] = (byte) ((ulong) (BootloaderSentence.sequenceNumber | (BootloaderSentence.sequenceNumber + 1) % 8 << 3) | 64UL /*0x40*/ | 128UL /*0x80*/);
    sentenceHeader[1] = (byte) (14 | ((int) dataLength & 15) << 4);
    sentenceHeader[2] = (byte) ((dataLength & 4080U) >> 4);
    int num = (int) ((long) ((int) sentenceHeader[0] + (int) sentenceHeader[1] + (int) sentenceHeader[2]) ^ (long) uint.MaxValue) + 1;
    sentenceHeader[3] = (byte) (num & (int) byte.MaxValue);
    return sentenceHeader;
  }

  public BootloaderSentence DeepCopy()
  {
    BootloaderSentence bootloaderSentence = new BootloaderSentence();
    foreach (byte num in this.data_in)
      bootloaderSentence.data_in.Add(num);
    foreach (byte num in this.data_converted)
      bootloaderSentence.data_converted.Add(num);
    return bootloaderSentence;
  }
}
