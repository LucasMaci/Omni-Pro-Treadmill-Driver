// Decompiled with JetBrains decompiler
// Type: OmniCommon.Messages.OmniPodBatteryLifeMessage
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

#nullable disable
namespace OmniCommon.Messages;

public class OmniPodBatteryLifeMessage : OmniBaseMessage
{
  public double BatteryLife => (double) ((int) this.Payload[0] & (int) sbyte.MaxValue) / 100.0;

  public bool IsCharging => (int) this.Payload[0] >> 7 == 1;

  public OmniPodBatteryLifeMessage()
    : base(MessageType.OmniPodBatteryLifeMessage)
  {
    this.Payload = new byte[0];
  }

  public OmniPodBatteryLifeMessage(OmniBaseMessage msg)
    : base(msg)
  {
    this.MsgType = MessageType.OmniPodBatteryLifeMessage;
  }

  public BatteryInfo GetInfo()
  {
    return new BatteryInfo()
    {
      BatteryLifeKnown = true,
      BatteryCharging = this.IsCharging,
      BatteryLife = this.BatteryLife
    };
  }
}
