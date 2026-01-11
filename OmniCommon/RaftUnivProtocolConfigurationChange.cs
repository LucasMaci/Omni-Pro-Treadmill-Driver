// Decompiled with JetBrains decompiler
// Type: OmniCommon.RaftUnivProtocolConfigurationChange
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

#nullable disable
namespace OmniCommon;

public class RaftUnivProtocolConfigurationChange
{
  public RaftUnivProtocolConfigFields UpdateType;
  public int Value = -1;
  public nrf_raft_device_channel_selection_policy_t DeviceChannelSelectionPolicy;
  public nrf_raft_tx_power_t OutputPower;
  public nrf_raft_datarate_t DataRate;
}
