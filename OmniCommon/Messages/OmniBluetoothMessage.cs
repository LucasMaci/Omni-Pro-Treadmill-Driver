// Decompiled with JetBrains decompiler
// Type: OmniCommon.Messages.OmniBluetoothMessage
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

#nullable disable
namespace OmniCommon.Messages;

public class OmniBluetoothMessage : OmniBaseMessage
{
  public BluetoothGetOrSet GetSet
  {
    get => (BluetoothGetOrSet) this.Payload[0];
    set => this.Payload[0] = (byte) value;
  }

  public BluetoothState State
  {
    get => (BluetoothState) this.Payload[1];
    set => this.Payload[1] = (byte) value;
  }

  public OmniBluetoothMessage(BluetoothGetOrSet getset, BluetoothState state)
    : base(MessageType.OmniBluetoothMessage)
  {
    this.Payload = new byte[2];
    this.GetSet = getset;
    this.State = state;
  }

  public OmniBluetoothMessage(OmniBaseMessage msg)
    : base(msg)
  {
    this.MsgType = MessageType.OmniBluetoothMessage;
  }
}
