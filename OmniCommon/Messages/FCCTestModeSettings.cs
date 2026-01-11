// Decompiled with JetBrains decompiler
// Type: OmniCommon.Messages.FCCTestModeSettings
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

using System;

#nullable disable
namespace OmniCommon.Messages;

public class FCCTestModeSettings
{
  public ModulationType ModulationType;
  public TestPattern TestPattern;
  public int FrequencyChannel;
  public int PowerLevel;

  public FCCTestModeSettings(
    ModulationType modulationType,
    TestPattern testPattern,
    int frequencyChannel,
    int powerLevel)
  {
    this.ModulationType = modulationType;
    this.TestPattern = testPattern;
    this.FrequencyChannel = frequencyChannel;
    this.PowerLevel = powerLevel;
  }

  public override string ToString()
  {
    return $"{(ValueType) (byte) this.ModulationType:X2}.{(ValueType) (byte) this.TestPattern:X2}.{this.FrequencyChannel}.{this.PowerLevel}";
  }
}
