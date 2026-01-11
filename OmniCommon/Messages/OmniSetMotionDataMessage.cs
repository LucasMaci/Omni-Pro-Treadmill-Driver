// Decompiled with JetBrains decompiler
// Type: OmniCommon.Messages.OmniSetMotionDataMessage
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

#nullable disable
namespace OmniCommon.Messages;

public class OmniSetMotionDataMessage : OmniBaseMessage
{
  public bool Timestamp
  {
    get => ((int) this.Payload[0] & 1) == 1;
    set => this.Payload[0] = (byte) (((int) this.Payload[0] & 254) + (value ? 1 : 0));
  }

  public bool StepCount
  {
    get => ((int) this.Payload[0] & 2) == 2;
    set => this.Payload[0] = (byte) (((int) this.Payload[0] & 253) + (value ? 2 : 0));
  }

  public bool RingAngle
  {
    get => ((int) this.Payload[0] & 4) == 4;
    set => this.Payload[0] = (byte) (((int) this.Payload[0] & 251) + (value ? 4 : 0));
  }

  public bool RingDelta
  {
    get => ((int) this.Payload[0] & 8) == 8;
    set => this.Payload[0] = (byte) (((int) this.Payload[0] & 247) + (value ? 8 : 0));
  }

  public bool GamePadData
  {
    get => ((int) this.Payload[0] & 16 /*0x10*/) == 16 /*0x10*/;
    set => this.Payload[0] = (byte) (((int) this.Payload[0] & 239) + (value ? 16 /*0x10*/ : 0));
  }

  public bool GunButtonData
  {
    get => ((int) this.Payload[0] & 32 /*0x20*/) == 32 /*0x20*/;
    set => this.Payload[0] = (byte) (((int) this.Payload[0] & 223) + (value ? 32 /*0x20*/ : 0));
  }

  public bool StepTrigger
  {
    get => ((int) this.Payload[0] & 64 /*0x40*/) == 64 /*0x40*/;
    set => this.Payload[0] = (byte) (((int) this.Payload[0] & 191) + (value ? 64 /*0x40*/ : 0));
  }

  public OmniSetMotionDataMessage()
    : base(MessageType.OmniSetMotionDataMessage)
  {
    this.Payload = new byte[1]{ (byte) 239 };
    this.ComPort = string.Empty;
  }

  public OmniSetMotionDataMessage(MotionDataSelection selection)
    : base(MessageType.OmniSetMotionDataMessage)
  {
    this.Payload = new byte[1];
    this.Timestamp = selection.Timestamp;
    this.StepCount = selection.StepCount;
    this.RingAngle = selection.RingAngle;
    this.RingDelta = selection.RingDelta;
    this.GamePadData = selection.GamePadData;
    this.GunButtonData = selection.GunButtonData;
    this.StepTrigger = selection.StepTrigger;
    this.ComPort = string.Empty;
  }

  public OmniSetMotionDataMessage(OmniBaseMessage msg)
    : base(msg)
  {
    this.MsgType = MessageType.OmniSetMotionDataMessage;
  }
}
