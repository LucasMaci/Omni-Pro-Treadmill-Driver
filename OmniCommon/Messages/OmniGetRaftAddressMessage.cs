// Decompiled with JetBrains decompiler
// Type: OmniCommon.Messages.OmniGetRaftAddressMessage
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

#nullable disable
namespace OmniCommon.Messages;

public class OmniGetRaftAddressMessage : OmniBaseMessage
{
  public string BaseAddressP0
  {
    get
    {
      return $"{this.Payload[3].ToString("X2")}.{this.Payload[2].ToString("X2")}.{this.Payload[1].ToString("X2")}.{this.Payload[0].ToString("X2")}";
    }
  }

  public string BaseAddressP1
  {
    get
    {
      return $"{this.Payload[7].ToString("X2")}.{this.Payload[6].ToString("X2")}.{this.Payload[5].ToString("X2")}.{this.Payload[4].ToString("X2")}";
    }
  }

  public OmniGetRaftAddressMessage()
    : base(MessageType.OmniGetRaftAddressMessage)
  {
    this.Payload = new byte[0];
    this.ComPort = string.Empty;
  }

  public OmniGetRaftAddressMessage(OmniBaseMessage msg)
    : base(msg)
  {
    this.MsgType = MessageType.OmniGetRaftAddressMessage;
  }

  public RaftUnivProtocolAddress GetInfo()
  {
    return new RaftUnivProtocolAddress()
    {
      BaseAddress0 = this.BaseAddressP0,
      BaseAddress1 = this.BaseAddressP1
    };
  }
}
