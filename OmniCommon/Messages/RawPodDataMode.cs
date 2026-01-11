// Decompiled with JetBrains decompiler
// Type: OmniCommon.Messages.RawPodDataMode
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

#nullable disable
namespace OmniCommon.Messages;

public class RawPodDataMode
{
  public bool Quaternions = false;
  public bool Accelerometer = false;
  public bool Gyroscope = false;
  public bool FrameNumber = false;

  public RawPodDataMode()
  {
  }

  public RawPodDataMode(byte data)
  {
    this.FrameNumber = ((int) data & 1) == 1;
    this.Quaternions = ((int) data & 2) == 2;
    this.Accelerometer = ((int) data & 4) == 4;
    this.Gyroscope = ((int) data & 8) == 8;
  }

  public RawPodDataMode DeepCopy()
  {
    return new RawPodDataMode()
    {
      Quaternions = this.Quaternions,
      Accelerometer = this.Accelerometer,
      Gyroscope = this.Gyroscope,
      FrameNumber = this.FrameNumber
    };
  }

  public byte GetByte()
  {
    return (byte) ((int) (byte) ((int) (byte) ((int) (byte) (0 + (this.FrameNumber ? 1 : 0)) + (this.Quaternions ? 2 : 0)) + (this.Accelerometer ? 4 : 0)) + (this.Gyroscope ? 8 : 0));
  }

  public override string ToString() => $"{this.GetByte():X2}";
}
