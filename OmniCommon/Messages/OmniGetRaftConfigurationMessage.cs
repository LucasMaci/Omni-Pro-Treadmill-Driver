// Decompiled with JetBrains decompiler
// Type: OmniCommon.Messages.OmniGetRaftConfigurationMessage
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

using System;

#nullable disable
namespace OmniCommon.Messages;

public class OmniGetRaftConfigurationMessage : OmniBaseMessage
{
  public int OmniDefinition => (int) this.Payload[0];

  public int TimeslotPeriod => BitConverter.ToInt32(this.Payload, 4);

  public int TimeslotsPerChannel => BitConverter.ToInt32(this.Payload, 8);

  public int MaxRetransmit => (int) BitConverter.ToInt16(this.Payload, 12);

  public nrf_raft_device_channel_selection_policy_t DeviceChannelSelectionPolicy
  {
    get => (nrf_raft_device_channel_selection_policy_t) this.Payload[14];
  }

  public nrf_raft_tx_power_t OutputPower => (nrf_raft_tx_power_t) this.Payload[15];

  public nrf_raft_datarate_t DataRate => (nrf_raft_datarate_t) this.Payload[16 /*0x10*/];

  public int SyncLife => (int) this.Payload[17];

  public OmniGetRaftConfigurationMessage()
    : base(MessageType.OmniGetRaftConfigurationMessage)
  {
    this.Payload = new byte[0];
    this.ComPort = string.Empty;
  }

  public OmniGetRaftConfigurationMessage(OmniBaseMessage msg)
    : base(msg)
  {
    this.MsgType = MessageType.OmniGetRaftConfigurationMessage;
  }

  public RaftUnivProtocolInfo GetInfo()
  {
    return new RaftUnivProtocolInfo()
    {
      DataRate = this.DataRate,
      DeviceChannelSelectionPolicy = this.DeviceChannelSelectionPolicy,
      MaxRetransmit = this.MaxRetransmit,
      OmniDefinition = this.OmniDefinition,
      OutputPower = this.OutputPower,
      SyncLife = this.SyncLife,
      TimeslotPeriod = this.TimeslotPeriod,
      TimeslotsPerChannel = this.TimeslotsPerChannel
    };
  }
}
