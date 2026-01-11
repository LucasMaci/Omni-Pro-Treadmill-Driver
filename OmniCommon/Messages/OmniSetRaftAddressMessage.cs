// Decompiled with JetBrains decompiler
// Type: OmniCommon.Messages.OmniSetRaftAddressMessage
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

using System;

#nullable disable
namespace OmniCommon.Messages;

public class OmniSetRaftAddressMessage : OmniBaseMessage
{
  public string BaseAddressP0
  {
    get
    {
      return $"{this.Payload[3].ToString("X2")}.{this.Payload[2].ToString("X2")}.{this.Payload[1].ToString("X2")}.{this.Payload[0].ToString("X2")}";
    }
    set
    {
      string[] strArray = value.Split('.');
      this.Payload[3] = Convert.ToByte(strArray[0], 16 /*0x10*/);
      this.Payload[2] = Convert.ToByte(strArray[1], 16 /*0x10*/);
      this.Payload[1] = Convert.ToByte(strArray[2], 16 /*0x10*/);
      this.Payload[0] = Convert.ToByte(strArray[3], 16 /*0x10*/);
    }
  }

  public string BaseAddressP1
  {
    get
    {
      return $"{this.Payload[7].ToString("X2")}.{this.Payload[6].ToString("X2")}.{this.Payload[5].ToString("X2")}.{this.Payload[4].ToString("X2")}";
    }
    set
    {
      string[] strArray = value.Split('.');
      this.Payload[7] = Convert.ToByte(strArray[0], 16 /*0x10*/);
      this.Payload[6] = Convert.ToByte(strArray[1], 16 /*0x10*/);
      this.Payload[5] = Convert.ToByte(strArray[2], 16 /*0x10*/);
      this.Payload[4] = Convert.ToByte(strArray[3], 16 /*0x10*/);
    }
  }

  public OmniSetRaftAddressMessage()
    : base(MessageType.OmniSetRaftAddressMessage)
  {
    this.Payload = new byte[8];
    this.ComPort = string.Empty;
  }

  public OmniSetRaftAddressMessage(OmniBaseMessage msg)
    : base(msg)
  {
    this.MsgType = MessageType.OmniSetRaftAddressMessage;
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
