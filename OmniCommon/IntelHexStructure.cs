// Decompiled with JetBrains decompiler
// Type: OmniCommon.IntelHexStructure
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

#nullable disable
namespace OmniCommon;

public class IntelHexStructure
{
  public uint address;
  public byte[] data = (byte[]) null;
  public int dataLen;
  public int type;
  public byte checksum;

  public IntelHexStructure DeepCopy()
  {
    IntelHexStructure intelHexStructure = new IntelHexStructure()
    {
      address = this.address,
      dataLen = this.dataLen,
      type = this.type,
      checksum = this.checksum,
      data = new byte[this.data.Length]
    };
    for (int index = 0; index < this.data.Length; ++index)
      intelHexStructure.data[index] = this.data[index];
    return intelHexStructure;
  }
}
