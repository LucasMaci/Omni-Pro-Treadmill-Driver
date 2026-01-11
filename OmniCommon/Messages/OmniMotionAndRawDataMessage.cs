// Decompiled with JetBrains decompiler
// Type: OmniCommon.Messages.OmniMotionAndRawDataMessage
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

using System;

#nullable disable
namespace OmniCommon.Messages;

public class OmniMotionAndRawDataMessage : OmniBaseMessage
{
  public uint Timestamp => BitConverter.ToUInt32(this.Payload, 0);

  public uint StepCount => BitConverter.ToUInt32(this.Payload, 4);

  public float RingAngle => BitConverter.ToSingle(this.Payload, 8);

  public byte RingDelta => this.Payload[12];

  public byte Gamepad_X => this.Payload[13];

  public byte Gamepad_Y => this.Payload[14];

  public byte StepTrigger => this.Payload[15];

  public float[] Pod1Quaternions
  {
    get
    {
      return new float[4]
      {
        (float) ((int) (short) ((int) this.Payload[17] << 8) + (int) this.Payload[16 /*0x10*/]) / 16384f,
        (float) ((int) (short) ((int) this.Payload[19] << 8) + (int) this.Payload[18]) / 16384f,
        (float) ((int) (short) ((int) this.Payload[21] << 8) + (int) this.Payload[20]) / 16384f,
        (float) ((int) (short) ((int) this.Payload[23] << 8) + (int) this.Payload[22]) / 16384f
      };
    }
  }

  public float[] Pod1Accelerometer
  {
    get
    {
      return new float[3]
      {
        (float) ((int) (short) ((int) this.Payload[25] << 8) + (int) this.Payload[24]) / 4096f,
        (float) ((int) (short) ((int) this.Payload[27] << 8) + (int) this.Payload[26]) / 4096f,
        (float) ((int) (short) ((int) this.Payload[29] << 8) + (int) this.Payload[28]) / 4096f
      };
    }
  }

  public float[] Pod1Gyroscope
  {
    get
    {
      return new float[3]
      {
        (float) ((int) (short) ((int) this.Payload[31 /*0x1F*/] << 8) + (int) this.Payload[30]) / 1024f,
        (float) ((int) (short) ((int) this.Payload[33] << 8) + (int) this.Payload[32 /*0x20*/]) / 1024f,
        (float) ((int) (short) ((int) this.Payload[35] << 8) + (int) this.Payload[34]) / 1024f
      };
    }
  }

  public float[] Pod2Quaternions
  {
    get
    {
      return new float[4]
      {
        (float) ((int) (short) ((int) this.Payload[37] << 8) + (int) this.Payload[36]) / 16384f,
        (float) ((int) (short) ((int) this.Payload[39] << 8) + (int) this.Payload[38]) / 16384f,
        (float) ((int) (short) ((int) this.Payload[41] << 8) + (int) this.Payload[40]) / 16384f,
        (float) ((int) (short) ((int) this.Payload[43] << 8) + (int) this.Payload[42]) / 16384f
      };
    }
  }

  public float[] Pod2Accelerometer
  {
    get
    {
      return new float[3]
      {
        (float) ((int) (short) ((int) this.Payload[45] << 8) + (int) this.Payload[44]) / 4096f,
        (float) ((int) (short) ((int) this.Payload[47] << 8) + (int) this.Payload[46]) / 4096f,
        (float) ((int) (short) ((int) this.Payload[49] << 8) + (int) this.Payload[48 /*0x30*/]) / 4096f
      };
    }
  }

  public float[] Pod2Gyroscope
  {
    get
    {
      return new float[3]
      {
        (float) ((int) (short) ((int) this.Payload[51] << 8) + (int) this.Payload[50]) / 1024f,
        (float) ((int) (short) ((int) this.Payload[53] << 8) + (int) this.Payload[52]) / 1024f,
        (float) ((int) (short) ((int) this.Payload[55] << 8) + (int) this.Payload[54]) / 1024f
      };
    }
  }

  public OmniMotionAndRawData GetMotionAndRawData()
  {
    return new OmniMotionAndRawData()
    {
      Timestamp = this.Timestamp,
      StepCount = this.StepCount,
      RingAngle = this.RingAngle,
      RingDelta = this.RingDelta,
      GamePad_X = this.Gamepad_X,
      GamePad_Y = this.Gamepad_Y,
      StepTrigger = this.StepTrigger,
      Pod1Quaternions = this.Pod1Quaternions,
      Pod1Accelerometer = this.Pod1Accelerometer,
      Pod1Gyroscope = this.Pod1Gyroscope,
      Pod2Quaternions = this.Pod2Quaternions,
      Pod2Accelerometer = this.Pod2Accelerometer,
      Pod2Gyroscope = this.Pod2Gyroscope
    };
  }

  public OmniMotionData GetMotionData()
  {
    return new OmniMotionData()
    {
      EnableTimestamp = true,
      EnableStepCount = true,
      EnableRingAngle = true,
      EnableRingDelta = true,
      EnableGamePadData = true,
      EnableStepTrigger = true,
      Timestamp = this.Timestamp,
      StepCount = this.StepCount,
      RingAngle = this.RingAngle,
      RingDelta = this.RingDelta,
      GamePad_X = this.Gamepad_X,
      GamePad_Y = this.Gamepad_Y,
      StepTrigger = this.StepTrigger
    };
  }

  public OmniMotionAndRawDataMessage(OmniBaseMessage msg)
    : base(msg)
  {
    this.MsgType = MessageType.OmniMotionAndRawDataMessage;
  }
}
