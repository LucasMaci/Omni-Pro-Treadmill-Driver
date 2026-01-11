// Decompiled with JetBrains decompiler
// Type: OmniCommon.Messages.OmniMotionData
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

using System;

#nullable disable
namespace OmniCommon.Messages;

public class OmniMotionData
{
  public bool EnableTimestamp = false;
  public bool EnableStepCount = false;
  public bool EnableRingAngle = false;
  public bool EnableRingDelta = false;
  public bool EnableGamePadData = false;
  public bool EnableGunButtonData = false;
  public bool EnableStepTrigger = false;
  public uint Timestamp = 0;
  public uint StepCount = 0;
  public float RingAngle = 0.0f;
  public byte RingDelta = 0;
  public byte GamePad_X = 0;
  public byte GamePad_Y = 0;
  public byte GunButtonData = 0;
  public byte StepTrigger = 0;

  public OmniMotionData DeepCopy()
  {
    return new OmniMotionData()
    {
      EnableTimestamp = this.EnableTimestamp,
      EnableStepCount = this.EnableStepCount,
      EnableRingAngle = this.EnableRingAngle,
      EnableRingDelta = this.EnableRingDelta,
      EnableGamePadData = this.EnableGamePadData,
      EnableGunButtonData = this.EnableGunButtonData,
      EnableStepTrigger = this.EnableStepTrigger,
      Timestamp = this.Timestamp,
      StepCount = this.StepCount,
      RingAngle = this.RingAngle,
      RingDelta = this.RingDelta,
      GamePad_X = this.GamePad_X,
      GamePad_Y = this.GamePad_Y,
      GunButtonData = this.GunButtonData,
      StepTrigger = this.StepTrigger
    };
  }

  public override string ToString()
  {
    string str = $"{(ValueType) (byte) ((int) (byte) ((int) (byte) ((int) (byte) ((int) (byte) ((int) (byte) ((int) (byte) (0 + (this.EnableTimestamp ? 1 : 0)) + (this.EnableStepCount ? 2 : 0)) + (this.EnableRingAngle ? 4 : 0)) + (this.EnableRingDelta ? 8 : 0)) + (this.EnableGamePadData ? 16 /*0x10*/ : 0)) + (this.EnableGunButtonData ? 32 /*0x20*/ : 0)) + (this.EnableStepTrigger ? 64 /*0x40*/ : 0)):X2}";
    if (this.EnableTimestamp)
      str += $", TS,{this.Timestamp.ToString()}";
    if (this.EnableStepCount)
      str += $", SC,{this.StepCount.ToString()}";
    if (this.EnableRingAngle)
      str += $", RA,{this.RingAngle.ToString()}";
    if (this.EnableRingDelta)
      str += $", RD,{this.RingDelta}";
    if (this.EnableGamePadData)
      str += $", GPD_X,{this.GamePad_X}, GPD_Y,{this.GamePad_Y}";
    if (this.EnableGunButtonData)
      str += $", GBD,{this.GunButtonData}";
    if (this.EnableStepTrigger)
      str += $", ST,{this.StepTrigger}";
    return str;
  }
}
