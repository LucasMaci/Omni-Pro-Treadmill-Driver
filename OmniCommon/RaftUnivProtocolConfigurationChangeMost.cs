// Decompiled with JetBrains decompiler
// Type: OmniCommon.RaftUnivProtocolConfigurationChangeMost
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

#nullable disable
namespace OmniCommon;

public class RaftUnivProtocolConfigurationChangeMost
{
  public int TimeSlotPeriod;
  public int TimeSlotPerChannel;
  public int MaxReTransmit;
  public nrf_raft_device_channel_selection_policy_t DeviceChannelSelectionPolicy;
  public nrf_raft_datarate_t DataRate;
  public int SyncLife;

  public RaftUnivProtocolConfigurationChangeMost DeepCopy()
  {
    return new RaftUnivProtocolConfigurationChangeMost()
    {
      TimeSlotPeriod = this.TimeSlotPeriod,
      TimeSlotPerChannel = this.TimeSlotPerChannel,
      MaxReTransmit = this.MaxReTransmit,
      DeviceChannelSelectionPolicy = this.DeviceChannelSelectionPolicy,
      DataRate = this.DataRate,
      SyncLife = this.SyncLife
    };
  }
}
