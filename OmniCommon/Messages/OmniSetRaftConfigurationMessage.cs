// Decompiled with JetBrains decompiler
// Type: OmniCommon.Messages.OmniSetRaftConfigurationMessage
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

using System;

#nullable disable
namespace OmniCommon.Messages;

public class OmniSetRaftConfigurationMessage : OmniBaseMessage
{
  public int OmniDefinition
  {
    get => (int) this.Payload[0];
    set => this.Payload[0] = Convert.ToByte((value - 1) % 13);
  }

  public int OmniDefinition_NoOffset
  {
    get => (int) this.Payload[0];
    set => this.Payload[0] = Convert.ToByte(value);
  }

  public int TimeslotPeriod
  {
    get => BitConverter.ToInt32(this.Payload, 4);
    set => Array.Copy((Array) BitConverter.GetBytes(value), 0, (Array) this.Payload, 4, 4);
  }

  public int TimeslotsPerChannel
  {
    get => BitConverter.ToInt32(this.Payload, 8);
    set => Array.Copy((Array) BitConverter.GetBytes(value), 0, (Array) this.Payload, 8, 4);
  }

  public int MaxRetransmit
  {
    get => (int) BitConverter.ToInt16(this.Payload, 12);
    set => Array.Copy((Array) BitConverter.GetBytes((short) value), 0, (Array) this.Payload, 12, 2);
  }

  public nrf_raft_device_channel_selection_policy_t DeviceChannelSelectionPolicy
  {
    get => (nrf_raft_device_channel_selection_policy_t) this.Payload[14];
    set => this.Payload[14] = (byte) value;
  }

  public nrf_raft_tx_power_t OutputPower
  {
    get => (nrf_raft_tx_power_t) this.Payload[15];
    set => this.Payload[15] = (byte) value;
  }

  public nrf_raft_datarate_t DataRate
  {
    get => (nrf_raft_datarate_t) this.Payload[16 /*0x10*/];
    set => this.Payload[16 /*0x10*/] = (byte) value;
  }

  public int SyncLife
  {
    get => (int) this.Payload[17];
    set => this.Payload[17] = (byte) (value & (int) byte.MaxValue);
  }

  public OmniSetRaftConfigurationMessage(OmniBaseMessage msg)
    : base(msg)
  {
    this.MsgType = MessageType.OmniSetRaftConfigurationMessage;
  }

  public OmniSetRaftConfigurationMessage(
    OmniBaseMessage msg,
    RaftUnivProtocolConfigurationChange change)
    : base(msg)
  {
    this.IsResponse = false;
    this.MsgType = MessageType.OmniSetRaftConfigurationMessage;
    switch (change.UpdateType)
    {
      case RaftUnivProtocolConfigFields.OmniChannel:
        this.OmniDefinition = change.Value;
        break;
      case RaftUnivProtocolConfigFields.TimeSlotPeriod:
        this.TimeslotPeriod = change.Value;
        break;
      case RaftUnivProtocolConfigFields.TimeSlotPerChannel:
        this.TimeslotsPerChannel = change.Value;
        break;
      case RaftUnivProtocolConfigFields.MaxReTransmit:
        this.MaxRetransmit = change.Value;
        break;
      case RaftUnivProtocolConfigFields.DeviceChannelSelectionPolicy:
        this.DeviceChannelSelectionPolicy = change.DeviceChannelSelectionPolicy;
        break;
      case RaftUnivProtocolConfigFields.OutputPower:
        this.OutputPower = change.OutputPower;
        break;
      case RaftUnivProtocolConfigFields.DataRate:
        this.DataRate = change.DataRate;
        break;
      case RaftUnivProtocolConfigFields.SyncLife:
        this.SyncLife = change.Value;
        break;
    }
  }

  public OmniSetRaftConfigurationMessage(
    OmniBaseMessage msg,
    RaftUnivProtocolConfigurationChangeMost change)
    : base(msg)
  {
    this.IsResponse = false;
    this.MsgType = MessageType.OmniSetRaftConfigurationMessage;
    this.TimeslotPeriod = change.TimeSlotPeriod;
    this.TimeslotsPerChannel = change.TimeSlotPerChannel;
    this.MaxRetransmit = change.MaxReTransmit;
    this.DeviceChannelSelectionPolicy = change.DeviceChannelSelectionPolicy;
    this.DataRate = change.DataRate;
    this.SyncLife = change.SyncLife;
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
