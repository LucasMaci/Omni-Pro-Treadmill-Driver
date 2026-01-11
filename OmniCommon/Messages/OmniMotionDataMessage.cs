// Decompiled with JetBrains decompiler
// Type: OmniCommon.Messages.OmniMotionDataMessage
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

using System;

#nullable disable
namespace OmniCommon.Messages;

public class OmniMotionDataMessage : OmniBaseMessage
{
  public bool EnableTimestamp
  {
    get => ((int) this.Payload[0] & 1) == 1;
    set => this.Payload[0] = (byte) (((int) this.Payload[0] & 254) + (value ? 1 : 0));
  }

  public bool EnableStepCount
  {
    get => ((int) this.Payload[0] & 2) == 2;
    set => this.Payload[0] = (byte) (((int) this.Payload[0] & 253) + (value ? 2 : 0));
  }

  public bool EnableRingAngle
  {
    get => ((int) this.Payload[0] & 4) == 4;
    set => this.Payload[0] = (byte) (((int) this.Payload[0] & 251) + (value ? 4 : 0));
  }

  public bool EnableRingDelta
  {
    get => ((int) this.Payload[0] & 8) == 8;
    set => this.Payload[0] = (byte) (((int) this.Payload[0] & 247) + (value ? 8 : 0));
  }

  public bool EnableGamePadData
  {
    get => ((int) this.Payload[0] & 16 /*0x10*/) == 16 /*0x10*/;
    set => this.Payload[0] = (byte) (((int) this.Payload[0] & 239) + (value ? 16 /*0x10*/ : 0));
  }

  public bool EnableGunButtonData
  {
    get => ((int) this.Payload[0] & 32 /*0x20*/) == 32 /*0x20*/;
    set => this.Payload[0] = (byte) (((int) this.Payload[0] & 223) + (value ? 32 /*0x20*/ : 0));
  }

  public bool EnableStepTrigger
  {
    get => ((int) this.Payload[0] & 64 /*0x40*/) == 64 /*0x40*/;
    set => this.Payload[0] = (byte) (((int) this.Payload[0] & 191) + (value ? 64 /*0x40*/ : 0));
  }

  public OmniMotionData GetMotionData()
  {
    OmniMotionData motionData = new OmniMotionData();
    int startIndex = 1;
    if (this.EnableTimestamp)
    {
      motionData.EnableTimestamp = true;
      motionData.Timestamp = BitConverter.ToUInt32(this.Payload, startIndex);
      startIndex += 4;
    }
    if (this.EnableStepCount)
    {
      motionData.EnableStepCount = true;
      motionData.StepCount = BitConverter.ToUInt32(this.Payload, startIndex);
      startIndex += 4;
    }
    if (this.EnableRingAngle)
    {
      motionData.EnableRingAngle = true;
      motionData.RingAngle = BitConverter.ToSingle(this.Payload, startIndex);
      startIndex += 4;
    }
    if (this.EnableRingDelta)
    {
      motionData.EnableRingDelta = true;
      motionData.RingDelta = this.Payload[startIndex];
      ++startIndex;
    }
    if (this.EnableGamePadData)
    {
      motionData.EnableGamePadData = true;
      motionData.GamePad_X = this.Payload[startIndex];
      int index = startIndex + 1;
      motionData.GamePad_Y = this.Payload[index];
      startIndex = index + 1;
    }
    if (this.EnableGunButtonData)
    {
      motionData.EnableGunButtonData = true;
      motionData.GunButtonData = this.Payload[startIndex];
      ++startIndex;
    }
    if (this.EnableStepTrigger)
    {
      motionData.EnableStepTrigger = true;
      motionData.StepTrigger = this.Payload[startIndex];
      int num = startIndex + 1;
    }
    return motionData;
  }

  public OmniMotionDataMessage(OmniBaseMessage msg)
    : base(msg)
  {
    this.MsgType = MessageType.OmniMotionDataMessage;
  }
}
