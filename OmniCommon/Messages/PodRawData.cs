// Decompiled with JetBrains decompiler
// Type: OmniCommon.Messages.PodRawData
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

#nullable disable
namespace OmniCommon.Messages;

public class PodRawData
{
  public bool EnableFrameNumber = false;
  public bool EnableQuaternions = false;
  public bool EnableAccelerometer = false;
  public bool EnableGyroscope = false;
  public ushort FrameNumber = 0;
  public float[] Quaternions = (float[]) null;
  public float[] Accelerometer = (float[]) null;
  public float[] Gyroscope = (float[]) null;

  public PodRawData()
  {
  }

  public PodRawData(RawPodDataMode mode, byte[] data, int startIndex, out int dataUsed)
  {
    this.EnableFrameNumber = mode.FrameNumber;
    this.EnableQuaternions = mode.Quaternions;
    this.EnableAccelerometer = mode.Accelerometer;
    this.EnableGyroscope = mode.Gyroscope;
    int index = startIndex;
    dataUsed = 0;
    if (mode.Quaternions)
    {
      if (data.Length < index + 8)
      {
        this.Quaternions = new float[4]
        {
          -1f,
          -1f,
          -1f,
          -1f
        };
      }
      else
      {
        this.Quaternions = new float[4];
        this.Quaternions[0] = (float) ((int) (short) (((int) data[index + 1] & (int) byte.MaxValue) << 8) + ((int) data[index] & (int) byte.MaxValue)) / 16384f;
        this.Quaternions[1] = (float) ((int) (short) (((int) data[index + 3] & (int) byte.MaxValue) << 8) + ((int) data[index + 2] & (int) byte.MaxValue)) / 16384f;
        this.Quaternions[2] = (float) ((int) (short) (((int) data[index + 5] & (int) byte.MaxValue) << 8) + ((int) data[index + 4] & (int) byte.MaxValue)) / 16384f;
        this.Quaternions[3] = (float) ((int) (short) (((int) data[index + 7] & (int) byte.MaxValue) << 8) + ((int) data[index + 6] & (int) byte.MaxValue)) / 16384f;
        index += 8;
        dataUsed += 8;
      }
    }
    if (mode.Accelerometer)
    {
      if (data.Length < index + 6)
      {
        this.Accelerometer = new float[3]{ -1f, -1f, -1f };
      }
      else
      {
        this.Accelerometer = new float[3];
        this.Accelerometer[0] = (float) ((int) (short) (((int) data[index + 1] & (int) byte.MaxValue) << 8) + ((int) data[index] & (int) byte.MaxValue)) / 4096f;
        this.Accelerometer[1] = (float) ((int) (short) (((int) data[index + 3] & (int) byte.MaxValue) << 8) + ((int) data[index + 2] & (int) byte.MaxValue)) / 4096f;
        this.Accelerometer[2] = (float) ((int) (short) (((int) data[index + 5] & (int) byte.MaxValue) << 8) + ((int) data[index + 4] & (int) byte.MaxValue)) / 4096f;
        index += 6;
        dataUsed += 6;
      }
    }
    if (mode.Gyroscope)
    {
      if (data.Length < index + 6)
      {
        this.Gyroscope = new float[3]{ -1f, -1f, -1f };
      }
      else
      {
        this.Gyroscope = new float[3];
        this.Gyroscope[0] = (float) ((int) (short) (((int) data[index + 1] & (int) byte.MaxValue) << 8) + ((int) data[index] & (int) byte.MaxValue)) / 1024f;
        this.Gyroscope[1] = (float) ((int) (short) (((int) data[index + 3] & (int) byte.MaxValue) << 8) + ((int) data[index + 2] & (int) byte.MaxValue)) / 1024f;
        this.Gyroscope[2] = (float) ((int) (short) (((int) data[index + 5] & (int) byte.MaxValue) << 8) + ((int) data[index + 4] & (int) byte.MaxValue)) / 1024f;
        index += 6;
        dataUsed += 6;
      }
    }
    if (!mode.FrameNumber)
      return;
    if (data.Length < index + 2)
    {
      this.FrameNumber = (ushort) 0;
    }
    else
    {
      this.FrameNumber = (ushort) ((((int) data[index + 1] & (int) byte.MaxValue) << 8) + ((int) data[index] & (int) byte.MaxValue));
      int num = index + 2;
      dataUsed += 2;
    }
  }

  public PodRawData DeepCopy()
  {
    PodRawData podRawData = new PodRawData()
    {
      FrameNumber = this.FrameNumber,
      EnableQuaternions = this.EnableQuaternions,
      EnableAccelerometer = this.EnableAccelerometer,
      EnableGyroscope = this.EnableGyroscope
    };
    podRawData.FrameNumber = this.FrameNumber;
    if (this.Quaternions == null)
    {
      podRawData.Quaternions = (float[]) null;
    }
    else
    {
      podRawData.Quaternions = new float[this.Quaternions.Length];
      for (int index = 0; index < this.Quaternions.Length; ++index)
        podRawData.Quaternions[index] = this.Quaternions[index];
    }
    if (this.Accelerometer == null)
    {
      podRawData.Accelerometer = (float[]) null;
    }
    else
    {
      podRawData.Accelerometer = new float[this.Accelerometer.Length];
      for (int index = 0; index < this.Accelerometer.Length; ++index)
        podRawData.Accelerometer[index] = this.Accelerometer[index];
    }
    if (this.Gyroscope == null)
    {
      podRawData.Gyroscope = (float[]) null;
    }
    else
    {
      podRawData.Gyroscope = new float[this.Gyroscope.Length];
      for (int index = 0; index < this.Gyroscope.Length; ++index)
        podRawData.Gyroscope[index] = this.Gyroscope[index];
    }
    return podRawData;
  }

  public override string ToString()
  {
    if (!this.EnableFrameNumber && !this.EnableQuaternions && !this.EnableAccelerometer && !this.EnableGyroscope)
      return "No Data";
    string str = $"Data, {(this.EnableQuaternions ? (object) "Q" : (object) "")}{(this.EnableAccelerometer ? (object) "A" : (object) "")}{(this.EnableGyroscope ? (object) "G" : (object) "")}{(this.EnableFrameNumber ? (object) "F" : (object) "")}";
    if (this.Quaternions != null && this.Quaternions.Length == 4)
      str += $", Quat,{this.Quaternions[0]},{this.Quaternions[1]},{this.Quaternions[2]},{this.Quaternions[3]}";
    if (this.Accelerometer != null && this.Accelerometer.Length == 3)
      str += $", Accel,{this.Accelerometer[0]},{this.Accelerometer[1]},{this.Accelerometer[2]}";
    if (this.Gyroscope != null && this.Gyroscope.Length == 3)
      str += $", Gyro,{this.Gyroscope[0]},{this.Gyroscope[1]},{this.Gyroscope[2]}";
    if (this.EnableFrameNumber)
      str += $", Frame,{this.FrameNumber}";
    return str;
  }
}
