// Decompiled with JetBrains decompiler
// Type: OmniCommon.Messages.MotionDataSelection
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

using System;

#nullable disable
namespace OmniCommon.Messages;

public class MotionDataSelection
{
  public bool Timestamp = false;
  public bool StepCount = false;
  public bool RingAngle = false;
  public bool RingDelta = false;
  public bool GamePadData = false;
  public bool GunButtonData = false;
  public bool StepTrigger = false;

  public override string ToString()
  {
    return $"{(ValueType) (byte) ((int) (byte) ((int) (byte) ((int) (byte) ((int) (byte) ((int) (byte) ((int) (byte) (0 + (this.Timestamp ? 1 : 0)) + (this.StepCount ? 2 : 0)) + (this.RingAngle ? 4 : 0)) + (this.RingDelta ? 8 : 0)) + (this.GamePadData ? 16 /*0x10*/ : 0)) + (this.GunButtonData ? 32 /*0x20*/ : 0)) + (this.StepTrigger ? 64 /*0x40*/ : 0)):X2}";
  }

  public MotionDataSelection()
  {
  }

  public MotionDataSelection(byte settings)
  {
    this.Timestamp = ((int) settings & 1) == 1;
    this.StepCount = ((int) settings & 2) == 2;
    this.RingAngle = ((int) settings & 4) == 4;
    this.RingDelta = ((int) settings & 8) == 8;
    this.GamePadData = ((int) settings & 16 /*0x10*/) == 16 /*0x10*/;
    this.GunButtonData = ((int) settings & 32 /*0x20*/) == 32 /*0x20*/;
    this.StepTrigger = ((int) settings & 64 /*0x40*/) == 64 /*0x40*/;
  }

  public MotionDataSelection DeepCopy()
  {
    return new MotionDataSelection()
    {
      Timestamp = this.Timestamp,
      StepCount = this.StepCount,
      RingAngle = this.RingAngle,
      RingDelta = this.RingDelta,
      GamePadData = this.GamePadData,
      GunButtonData = this.GunButtonData,
      StepTrigger = this.StepTrigger
    };
  }

  public static MotionDataSelection AllOff()
  {
    return new MotionDataSelection()
    {
      Timestamp = false,
      StepCount = false,
      RingAngle = false,
      RingDelta = false,
      GamePadData = false,
      GunButtonData = false,
      StepTrigger = false
    };
  }

  public static MotionDataSelection AllOn()
  {
    return new MotionDataSelection()
    {
      Timestamp = true,
      StepCount = true,
      RingAngle = true,
      RingDelta = true,
      GamePadData = true,
      GunButtonData = true,
      StepTrigger = true
    };
  }

  public static MotionDataSelection StreamingWindowData()
  {
    return new MotionDataSelection()
    {
      Timestamp = true,
      StepCount = true,
      RingAngle = true,
      RingDelta = true,
      GamePadData = true,
      GunButtonData = false,
      StepTrigger = false
    };
  }
}
