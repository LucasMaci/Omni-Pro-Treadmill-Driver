// Decompiled with JetBrains decompiler
// Type: OmniCommon.RaftUnivProtocolInfo
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

#nullable disable
namespace OmniCommon;

public class RaftUnivProtocolInfo
{
  public int OmniDefinition;
  public int TimeslotPeriod;
  public int TimeslotsPerChannel;
  public int MaxRetransmit;
  public nrf_raft_device_channel_selection_policy_t DeviceChannelSelectionPolicy;
  public nrf_raft_tx_power_t OutputPower;
  public nrf_raft_datarate_t DataRate;
  public int SyncLife;
}
