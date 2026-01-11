// Decompiled with JetBrains decompiler
// Type: OmniCommon.MinMax
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

#nullable disable
namespace OmniCommon;

public class MinMax
{
  public uint Min;
  public uint Max;

  public uint GetLength(bool alignMax = false)
  {
    return (alignMax ? MinMax.word_align_4(this.Max) : this.Max) - this.Min;
  }

  public static uint word_align_4(uint input)
  {
    return ((int) input & 3) == 0 ? input : (uint) (((int) input & 65532) + 4);
  }
}
