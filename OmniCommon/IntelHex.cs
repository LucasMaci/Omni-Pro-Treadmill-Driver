// Decompiled with JetBrains decompiler
// Type: OmniCommon.IntelHex
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

using System;
using System.IO;
using System.Text;

#nullable disable
namespace OmniCommon;

public class IntelHex
{
  private const int IHEX_RECORD_BUFF_SIZE = 768 /*0x0300*/;
  private const int IHEX_COUNT_OFFSET = 1;
  private const int IHEX_COUNT_LEN = 2;
  private const int IHEX_ADDRESS_OFFSET = 3;
  private const int IHEX_ADDRESS_LEN = 4;
  private const int IHEX_TYPE_OFFSET = 7;
  private const int IHEX_TYPE_LEN = 2;
  private const int IHEX_DATA_OFFSET = 9;
  private const int IHEX_CHECKSUM_LEN = 2;
  public const int IHEX_MAX_DATA_LEN = 512 /*0x0200*/;
  private const int IHEX_ASCII_HEX_BYTE_LEN = 2;
  private const int IHEX_START_CODE_OFFSET = 0;
  private const char IHEX_START_CODE = ':';
  private const int IHEX_OK = 0;
  private const int IHEX_ERROR_FILE = -1;
  private const int IHEX_ERROR_EOF = -2;
  private const int IHEX_ERROR_INVALID_RECORD = -3;
  private const int IHEX_ERROR_INVALID_ARGUMENTS = -4;
  private const int IHEX_ERROR_NEWLINE = -5;
  private const int IHEX_ERROR_INVALID_STRUCTURE = -6;
  private const int IHEX_TYPE_00 = 0;
  private const int IHEX_TYPE_01 = 1;
  private const int IHEX_TYPE_02 = 2;
  private const int IHEX_TYPE_03 = 3;
  private const int IHEX_TYPE_04 = 4;
  private const int IHEX_TYPE_05 = 5;
  private IntelHexStructure irec = new IntelHexStructure();
  private int status = -4;

  public int Status => this.status;

  public IntelHexStructure NewRecord(int type, ushort address, byte[] data, int dataLen)
  {
    if (dataLen < 0 || dataLen > 256 /*0x0100*/ || this.irec == null)
    {
      this.status = -4;
      return (IntelHexStructure) null;
    }
    this.irec.type = type;
    this.irec.address = (uint) address;
    if (data != null)
      Array.Copy((Array) data, (Array) this.irec.data, (long) dataLen);
    this.irec.dataLen = dataLen;
    this.irec.checksum = this.Checksum();
    this.status = 0;
    return this.irec;
  }

  public IntelHexStructure Read(StreamReader inStream)
  {
    if (this.irec == null || inStream == null)
    {
      this.status = -4;
      return (IntelHexStructure) null;
    }
    string str;
    try
    {
      str = inStream.ReadLine();
    }
    catch (Exception ex)
    {
      this.status = -1;
      return (IntelHexStructure) null;
    }
    if (str == null || str.Length == 0)
    {
      this.status = -5;
      return (IntelHexStructure) null;
    }
    if (str.Length < 9)
    {
      this.status = -3;
      return (IntelHexStructure) null;
    }
    if (str[0] != ':')
    {
      this.status = -3;
      return (IntelHexStructure) null;
    }
    int int16 = (int) Convert.ToInt16(str.Substring(1, 2), 16 /*0x10*/);
    this.irec.address = (uint) Convert.ToUInt16(str.Substring(3, 4), 16 /*0x10*/);
    this.irec.type = (int) Convert.ToInt16(str.Substring(7, 2), 16 /*0x10*/);
    if (str.Length < 9 + int16 * 2 + 2)
    {
      this.status = -3;
      return (IntelHexStructure) null;
    }
    this.irec.data = new byte[int16];
    int index;
    for (index = 0; index < int16; ++index)
      this.irec.data[index] = Convert.ToByte(str.Substring(9 + 2 * index, 2), 16 /*0x10*/);
    for (; index < this.irec.data.Length; ++index)
      this.irec.data[index] = (byte) 0;
    this.irec.dataLen = int16;
    this.irec.checksum = Convert.ToByte(str.Substring(9 + int16 * 2, 2), 16 /*0x10*/);
    if ((int) this.irec.checksum != (int) this.Checksum())
    {
      this.status = -3;
      return (IntelHexStructure) null;
    }
    this.status = 0;
    return this.irec;
  }

  public IntelHexData ReadHexFile(string fileName)
  {
    IntelHexData intelHexData = new IntelHexData();
    StreamReader inStream = new StreamReader(fileName);
    bool flag = true;
    uint num = 0;
    while (flag)
    {
      try
      {
        IntelHexStructure intelHexStructure = this.Read(inStream);
        if (this.status == 0)
        {
          switch (intelHexStructure.type)
          {
            case 0:
              intelHexStructure.address += num;
              intelHexData.AddData(intelHexStructure.DeepCopy());
              break;
            case 1:
              flag = false;
              break;
            case 4:
              num = (uint) (((int) intelHexStructure.data[0] << 8) + (int) intelHexStructure.data[1] << 16 /*0x10*/);
              break;
          }
        }
      }
      catch
      {
        flag = false;
      }
    }
    inStream.Close();
    return intelHexData;
  }

  public IntelHexStructure Read(string line)
  {
    if (this.irec == null)
    {
      this.status = -4;
      return (IntelHexStructure) null;
    }
    if (line == null || line.Length == 0)
    {
      this.status = -5;
      return (IntelHexStructure) null;
    }
    if (line.Length < 9)
    {
      this.status = -3;
      return (IntelHexStructure) null;
    }
    if (line[0] != ':')
    {
      this.status = -3;
      return (IntelHexStructure) null;
    }
    int int16 = (int) Convert.ToInt16(line.Substring(1, 2), 16 /*0x10*/);
    this.irec.address = (uint) Convert.ToUInt16(line.Substring(3, 4), 16 /*0x10*/);
    this.irec.type = (int) Convert.ToInt16(line.Substring(7, 2), 16 /*0x10*/);
    if (line.Length < 9 + int16 * 2 + 2)
    {
      this.status = -3;
      return (IntelHexStructure) null;
    }
    this.irec.data = new byte[int16];
    int index;
    for (index = 0; index < int16; ++index)
      this.irec.data[index] = Convert.ToByte(line.Substring(9 + 2 * index, 2), 16 /*0x10*/);
    for (; index < this.irec.data.Length; ++index)
      this.irec.data[index] = (byte) 0;
    this.irec.dataLen = int16;
    this.irec.checksum = Convert.ToByte(line.Substring(9 + int16 * 2, 2), 16 /*0x10*/);
    if ((int) this.irec.checksum != (int) this.Checksum())
    {
      this.status = -3;
      return (IntelHexStructure) null;
    }
    this.status = 0;
    return this.irec;
  }

  public IntelHexData ReadHexFile(byte[] firmwareData)
  {
    IntelHexData intelHexData = new IntelHexData();
    string[] strArray = Encoding.ASCII.GetString(firmwareData).Split(new char[2]
    {
      '\r',
      '\n'
    }, StringSplitOptions.RemoveEmptyEntries);
    bool flag = true;
    uint num = 0;
    int index = 0;
    while (flag)
    {
      try
      {
        IntelHexStructure intelHexStructure = this.Read(strArray[index]);
        if (this.status == 0)
        {
          switch (intelHexStructure.type)
          {
            case 0:
              intelHexStructure.address += num;
              intelHexData.AddData(intelHexStructure.DeepCopy());
              break;
            case 1:
              flag = false;
              break;
            case 4:
              num = (uint) (((int) intelHexStructure.data[0] << 8) + (int) intelHexStructure.data[1] << 16 /*0x10*/);
              break;
          }
        }
        ++index;
      }
      catch
      {
        flag = false;
      }
    }
    return intelHexData;
  }

  public IntelHexStructure Write(StreamWriter outStream)
  {
    if (this.irec == null || outStream == null)
    {
      this.status = -4;
      return (IntelHexStructure) null;
    }
    if (this.irec.dataLen > 256 /*0x0100*/)
    {
      this.status = -3;
      return (IntelHexStructure) null;
    }
    try
    {
      outStream.Write($"{(ValueType) ':'}{this.irec.dataLen:X2}{this.irec.address:X4}{this.irec.type:X2}");
      for (int index = 0; index < this.irec.dataLen; ++index)
        outStream.Write($"{this.irec.data[index]:X2}");
      outStream.WriteLine($"{this.Checksum():X2}");
    }
    catch (Exception ex)
    {
      this.status = -1;
      return (IntelHexStructure) null;
    }
    this.status = 0;
    return this.irec;
  }

  public string Print(bool verbose = false)
  {
    string str1;
    if (verbose)
    {
      string str2 = $"Intel HEX8 Record Type: \t{this.irec.type}\n" + $"Intel HEX8 Record Address: \t0x{this.irec.address:X4}\n" + string.Format("Intel HEX8 Record Data: \t[");
      for (int index = 0; index < this.irec.dataLen; ++index)
        str2 = index + 1 >= this.irec.dataLen ? str2 + $"0x{this.irec.data[index]:X02}" : str2 + $"0x{this.irec.data[index]:X02}, ";
      str1 = str2 + string.Format("]\n") + $"Intel HEX8 Record Checksum: \t0x{this.irec.checksum:X2}\n";
    }
    else
    {
      string str3 = $"{(ValueType) ':'}{this.irec.dataLen:X2}{this.irec.address:X4}{this.irec.type:X2}";
      for (int index = 0; index < this.irec.dataLen; ++index)
        str3 += $"{this.irec.data[index]:X2}";
      str1 = str3 + $"{this.Checksum():X2}";
    }
    this.status = 0;
    return str1;
  }

  internal byte Checksum()
  {
    byte num = (byte) ((uint) (byte) ((uint) (byte) ((uint) (byte) this.irec.dataLen + (uint) (byte) this.irec.type) + (uint) (byte) this.irec.address) + (uint) (byte) ((this.irec.address & 65280U) >> 8));
    for (int index = 0; index < this.irec.dataLen; ++index)
      num += this.irec.data[index];
    return (byte) ((uint) ~num + 1U);
  }
}
