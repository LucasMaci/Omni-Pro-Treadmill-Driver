// Decompiled with JetBrains decompiler
// Type: OmniCommon.Messages.OmniMotionAndRawData
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

#nullable disable
namespace OmniCommon.Messages;

public class OmniMotionAndRawData
{
  public uint Timestamp = 0;
  public uint StepCount = 0;
  public float RingAngle = 0.0f;
  public byte RingDelta = 0;
  public byte GamePad_X = 0;
  public byte GamePad_Y = 0;
  public byte StepTrigger = 0;
  public float[] Pod1Quaternions = (float[]) null;
  public float[] Pod1Accelerometer = (float[]) null;
  public float[] Pod1Gyroscope = (float[]) null;
  public float[] Pod2Quaternions = (float[]) null;
  public float[] Pod2Accelerometer = (float[]) null;
  public float[] Pod2Gyroscope = (float[]) null;

  public OmniMotionAndRawData DeepCopy()
  {
    OmniMotionAndRawData motionAndRawData = new OmniMotionAndRawData()
    {
      Timestamp = this.Timestamp,
      StepCount = this.StepCount,
      RingAngle = this.RingAngle,
      RingDelta = this.RingDelta,
      GamePad_X = this.GamePad_X,
      GamePad_Y = this.GamePad_Y,
      StepTrigger = this.StepTrigger
    };
    if (this.Pod1Quaternions == null)
    {
      motionAndRawData.Pod1Quaternions = (float[]) null;
    }
    else
    {
      motionAndRawData.Pod1Quaternions = new float[this.Pod1Quaternions.Length];
      for (int index = 0; index < this.Pod1Quaternions.Length; ++index)
        motionAndRawData.Pod1Quaternions[index] = this.Pod1Quaternions[index];
    }
    if (this.Pod1Accelerometer == null)
    {
      motionAndRawData.Pod1Accelerometer = (float[]) null;
    }
    else
    {
      motionAndRawData.Pod1Accelerometer = new float[this.Pod1Accelerometer.Length];
      for (int index = 0; index < this.Pod1Accelerometer.Length; ++index)
        motionAndRawData.Pod1Accelerometer[index] = this.Pod1Accelerometer[index];
    }
    if (this.Pod1Gyroscope == null)
    {
      motionAndRawData.Pod1Gyroscope = (float[]) null;
    }
    else
    {
      motionAndRawData.Pod1Gyroscope = new float[this.Pod1Gyroscope.Length];
      for (int index = 0; index < this.Pod1Gyroscope.Length; ++index)
        motionAndRawData.Pod1Gyroscope[index] = this.Pod1Gyroscope[index];
    }
    if (this.Pod2Quaternions == null)
    {
      motionAndRawData.Pod2Quaternions = (float[]) null;
    }
    else
    {
      motionAndRawData.Pod2Quaternions = new float[this.Pod2Quaternions.Length];
      for (int index = 0; index < this.Pod2Quaternions.Length; ++index)
        motionAndRawData.Pod2Quaternions[index] = this.Pod2Quaternions[index];
    }
    if (this.Pod2Accelerometer == null)
    {
      motionAndRawData.Pod2Accelerometer = (float[]) null;
    }
    else
    {
      motionAndRawData.Pod2Accelerometer = new float[this.Pod2Accelerometer.Length];
      for (int index = 0; index < this.Pod2Accelerometer.Length; ++index)
        motionAndRawData.Pod2Accelerometer[index] = this.Pod2Accelerometer[index];
    }
    if (this.Pod2Gyroscope == null)
    {
      motionAndRawData.Pod2Gyroscope = (float[]) null;
    }
    else
    {
      motionAndRawData.Pod2Gyroscope = new float[this.Pod2Gyroscope.Length];
      for (int index = 0; index < this.Pod2Gyroscope.Length; ++index)
        motionAndRawData.Pod2Gyroscope[index] = this.Pod2Gyroscope[index];
    }
    return motionAndRawData;
  }

  public override string ToString()
  {
    string str = $",TS,{this.Timestamp.ToString()},SC,{this.StepCount.ToString()},RA,{this.RingAngle.ToString()},RD,{this.RingDelta},GPD_X,{this.GamePad_X},GPD_Y,{this.GamePad_Y},ST,{this.StepTrigger}";
    if (this.Pod1Quaternions != null && this.Pod1Quaternions.Length == 4)
      str += $",Pod1 Quat,{this.Pod1Quaternions[0]},{this.Pod1Quaternions[1]},{this.Pod1Quaternions[2]},{this.Pod1Quaternions[3]}";
    if (this.Pod1Accelerometer != null && this.Pod1Accelerometer.Length == 3)
      str += $",Pod1 Accel,{this.Pod1Accelerometer[0]},{this.Pod1Accelerometer[1]},{this.Pod1Accelerometer[2]}";
    if (this.Pod1Gyroscope != null && this.Pod1Gyroscope.Length == 3)
      str += $",Pod1 Gyro,{this.Pod1Gyroscope[0]},{this.Pod1Gyroscope[1]},{this.Pod1Gyroscope[2]}";
    if (this.Pod2Quaternions != null && this.Pod2Quaternions.Length == 4)
      str += $",Pod2 Quat,{this.Pod2Quaternions[0]},{this.Pod2Quaternions[1]},{this.Pod2Quaternions[2]},{this.Pod2Quaternions[3]}";
    if (this.Pod2Accelerometer != null && this.Pod2Accelerometer.Length == 3)
      str += $",Pod2 Accel,{this.Pod2Accelerometer[0]},{this.Pod2Accelerometer[1]},{this.Pod2Accelerometer[2]}";
    if (this.Pod2Gyroscope != null && this.Pod2Gyroscope.Length == 3)
      str += $",Pod2 Gyro,{this.Pod2Gyroscope[0]},{this.Pod2Gyroscope[1]},{this.Pod2Gyroscope[2]}";
    return str;
  }
}
