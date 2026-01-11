// Decompiled with JetBrains decompiler
// Type: OmniCommon.OmniPacketBuilder
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

using OmniCommon.Messages;
using System;

#nullable disable
namespace OmniCommon;

public class OmniPacketBuilder
{
  private const int NonCRCLength = 4;
  private const int CRCOverheadLength = 4;
  private const int NotPayloadLength = 8;
  private static ushort[] crcTable = new ushort[256 /*0x0100*/]
  {
    (ushort) 0,
    (ushort) 49345,
    (ushort) 49537,
    (ushort) 320,
    (ushort) 49921,
    (ushort) 960,
    (ushort) 640,
    (ushort) 49729,
    (ushort) 50689,
    (ushort) 1728,
    (ushort) 1920,
    (ushort) 51009,
    (ushort) 1280 /*0x0500*/,
    (ushort) 50625,
    (ushort) 50305,
    (ushort) 1088,
    (ushort) 52225,
    (ushort) 3264,
    (ushort) 3456,
    (ushort) 52545,
    (ushort) 3840 /*0x0F00*/,
    (ushort) 53185,
    (ushort) 52865,
    (ushort) 3648,
    (ushort) 2560 /*0x0A00*/,
    (ushort) 51905,
    (ushort) 52097,
    (ushort) 2880,
    (ushort) 51457,
    (ushort) 2496,
    (ushort) 2176,
    (ushort) 51265,
    (ushort) 55297,
    (ushort) 6336,
    (ushort) 6528,
    (ushort) 55617,
    (ushort) 6912,
    (ushort) 56257,
    (ushort) 55937,
    (ushort) 6720,
    (ushort) 7680,
    (ushort) 57025,
    (ushort) 57217,
    (ushort) 8000,
    (ushort) 56577,
    (ushort) 7616,
    (ushort) 7296,
    (ushort) 56385,
    (ushort) 5120,
    (ushort) 54465,
    (ushort) 54657,
    (ushort) 5440,
    (ushort) 55041,
    (ushort) 6080,
    (ushort) 5760,
    (ushort) 54849,
    (ushort) 53761,
    (ushort) 4800,
    (ushort) 4992,
    (ushort) 54081,
    (ushort) 4352,
    (ushort) 53697,
    (ushort) 53377,
    (ushort) 4160,
    (ushort) 61441,
    (ushort) 12480,
    (ushort) 12672,
    (ushort) 61761,
    (ushort) 13056,
    (ushort) 62401,
    (ushort) 62081,
    (ushort) 12864,
    (ushort) 13824,
    (ushort) 63169,
    (ushort) 63361,
    (ushort) 14144,
    (ushort) 62721,
    (ushort) 13760,
    (ushort) 13440,
    (ushort) 62529,
    (ushort) 15360,
    (ushort) 64705,
    (ushort) 64897,
    (ushort) 15680,
    (ushort) 65281,
    (ushort) 16320,
    (ushort) 16000,
    (ushort) 65089,
    (ushort) 64001,
    (ushort) 15040,
    (ushort) 15232,
    (ushort) 64321,
    (ushort) 14592,
    (ushort) 63937,
    (ushort) 63617,
    (ushort) 14400,
    (ushort) 10240,
    (ushort) 59585,
    (ushort) 59777,
    (ushort) 10560,
    (ushort) 60161,
    (ushort) 11200,
    (ushort) 10880,
    (ushort) 59969,
    (ushort) 60929,
    (ushort) 11968,
    (ushort) 12160,
    (ushort) 61249,
    (ushort) 11520,
    (ushort) 60865,
    (ushort) 60545,
    (ushort) 11328,
    (ushort) 58369,
    (ushort) 9408,
    (ushort) 9600,
    (ushort) 58689,
    (ushort) 9984,
    (ushort) 59329,
    (ushort) 59009,
    (ushort) 9792,
    (ushort) 8704,
    (ushort) 58049,
    (ushort) 58241,
    (ushort) 9024,
    (ushort) 57601,
    (ushort) 8640,
    (ushort) 8320,
    (ushort) 57409,
    (ushort) 40961,
    (ushort) 24768,
    (ushort) 24960,
    (ushort) 41281,
    (ushort) 25344,
    (ushort) 41921,
    (ushort) 41601,
    (ushort) 25152,
    (ushort) 26112,
    (ushort) 42689,
    (ushort) 42881,
    (ushort) 26432,
    (ushort) 42241,
    (ushort) 26048,
    (ushort) 25728,
    (ushort) 42049,
    (ushort) 27648,
    (ushort) 44225,
    (ushort) 44417,
    (ushort) 27968,
    (ushort) 44801,
    (ushort) 28608,
    (ushort) 28288,
    (ushort) 44609,
    (ushort) 43521,
    (ushort) 27328,
    (ushort) 27520,
    (ushort) 43841,
    (ushort) 26880,
    (ushort) 43457,
    (ushort) 43137,
    (ushort) 26688,
    (ushort) 30720,
    (ushort) 47297,
    (ushort) 47489,
    (ushort) 31040,
    (ushort) 47873,
    (ushort) 31680,
    (ushort) 31360,
    (ushort) 47681,
    (ushort) 48641,
    (ushort) 32448,
    (ushort) 32640,
    (ushort) 48961,
    (ushort) 32000,
    (ushort) 48577,
    (ushort) 48257,
    (ushort) 31808,
    (ushort) 46081,
    (ushort) 29888,
    (ushort) 30080,
    (ushort) 46401,
    (ushort) 30464,
    (ushort) 47041,
    (ushort) 46721,
    (ushort) 30272,
    (ushort) 29184,
    (ushort) 45761,
    (ushort) 45953,
    (ushort) 29504,
    (ushort) 45313,
    (ushort) 29120,
    (ushort) 28800,
    (ushort) 45121,
    (ushort) 20480 /*0x5000*/,
    (ushort) 37057,
    (ushort) 37249,
    (ushort) 20800,
    (ushort) 37633,
    (ushort) 21440,
    (ushort) 21120,
    (ushort) 37441,
    (ushort) 38401,
    (ushort) 22208,
    (ushort) 22400,
    (ushort) 38721,
    (ushort) 21760,
    (ushort) 38337,
    (ushort) 38017,
    (ushort) 21568,
    (ushort) 39937,
    (ushort) 23744,
    (ushort) 23936,
    (ushort) 40257,
    (ushort) 24320,
    (ushort) 40897,
    (ushort) 40577,
    (ushort) 24128,
    (ushort) 23040,
    (ushort) 39617,
    (ushort) 39809,
    (ushort) 23360,
    (ushort) 39169,
    (ushort) 22976,
    (ushort) 22656,
    (ushort) 38977,
    (ushort) 34817,
    (ushort) 18624,
    (ushort) 18816,
    (ushort) 35137,
    (ushort) 19200,
    (ushort) 35777,
    (ushort) 35457,
    (ushort) 19008,
    (ushort) 19968,
    (ushort) 36545,
    (ushort) 36737,
    (ushort) 20288,
    (ushort) 36097,
    (ushort) 19904,
    (ushort) 19584,
    (ushort) 35905,
    (ushort) 17408,
    (ushort) 33985,
    (ushort) 34177,
    (ushort) 17728,
    (ushort) 34561,
    (ushort) 18368,
    (ushort) 18048,
    (ushort) 34369,
    (ushort) 33281,
    (ushort) 17088,
    (ushort) 17280,
    (ushort) 33601,
    (ushort) 16640,
    (ushort) 33217,
    (ushort) 32897,
    (ushort) 16448
  };

  private static ushort computeCRC16(byte[] bytes)
  {
    ushort crC16 = ushort.MaxValue;
    if (bytes == null)
      return 0;
    for (int index = 0; index < bytes.Length; ++index)
    {
      ushort num = (ushort) ((uint) crC16 ^ (uint) bytes[index]);
      crC16 = (ushort) ((uint) crC16 >> 8 ^ (uint) OmniPacketBuilder.crcTable[(int) num & (int) byte.MaxValue]);
    }
    return crC16;
  }

  public static OmniBaseMessage decodePacket(byte[] data)
  {
    byte[] numArray1 = new byte[data.Length - 4];
    Buffer.BlockCopy((Array) data, 1, (Array) numArray1, 0, numArray1.Length);
    if ((int) (ushort) ((uint) data[data.Length - 3] | (uint) data[data.Length - 2] << 8) != (int) OmniPacketBuilder.computeCRC16(numArray1))
      return (OmniBaseMessage) null;
    byte[] numArray2 = new byte[data.Length - 8];
    Buffer.BlockCopy((Array) data, 5, (Array) numArray2, 0, numArray2.Length);
    byte msgType_byte = data[2];
    byte pipeStatusByte = data[4];
    return new OmniBaseMessage(msgType_byte, numArray2, pipeStatusByte);
  }

  public static OmniBaseMessage decodePacket(byte[] data, int length)
  {
    byte[] numArray1 = new byte[length - 4];
    Buffer.BlockCopy((Array) data, 1, (Array) numArray1, 0, numArray1.Length);
    if ((int) (ushort) ((uint) data[length - 3] | (uint) data[length - 2] << 8) != (int) OmniPacketBuilder.computeCRC16(numArray1))
      return (OmniBaseMessage) null;
    byte[] numArray2 = new byte[length - 8];
    Buffer.BlockCopy((Array) data, 5, (Array) numArray2, 0, numArray2.Length);
    byte msgType_byte = data[2];
    byte pipeStatusByte = data[4];
    return new OmniBaseMessage(msgType_byte, numArray2, pipeStatusByte);
  }

  public static byte[] buildPacket(byte command, byte[] payload, byte pipeStatusByte)
  {
    return OmniPacketBuilder.buildPacket(command, (byte) 0, pipeStatusByte, payload);
  }

  private static byte[] buildPacket(byte command, byte packetID, byte status, byte[] payload)
  {
    byte[] bytes = new byte[payload.Length + 4];
    bytes[0] = (byte) (payload.Length + 8);
    bytes[1] = command;
    bytes[2] = packetID;
    bytes[3] = status;
    payload.CopyTo((Array) bytes, 4);
    ushort crC16 = OmniPacketBuilder.computeCRC16(bytes);
    byte[] src1 = new byte[3]
    {
      (byte) crC16,
      (byte) ((uint) crC16 >> 8),
      (byte) 190
    };
    byte[] src2 = new byte[bytes.Length + 1];
    src2[0] = (byte) 239;
    bytes.CopyTo((Array) src2, 1);
    byte[] dst = new byte[src2.Length + src1.Length];
    Buffer.BlockCopy((Array) src2, 0, (Array) dst, 0, src2.Length);
    Buffer.BlockCopy((Array) src1, 0, (Array) dst, src2.Length, src1.Length);
    return dst;
  }
}
