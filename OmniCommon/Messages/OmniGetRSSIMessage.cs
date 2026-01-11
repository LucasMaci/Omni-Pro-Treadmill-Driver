// Decompiled with JetBrains decompiler
// Type: OmniCommon.Messages.OmniGetRSSIMessage
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

using System;

#nullable disable
namespace OmniCommon.Messages;

public class OmniGetRSSIMessage : OmniBaseMessage
{
  public uint TimeStamp => BitConverter.ToUInt32(this.Payload, 0);

  public short RSSI_Pod1 => BitConverter.ToInt16(this.Payload, 4);

  public short RSSI_Pod2 => BitConverter.ToInt16(this.Payload, 6);

  public short RSSI_Pod3 => BitConverter.ToInt16(this.Payload, 8);

  public OmniGetRSSIMessage()
    : base(MessageType.OmniGetRSSIMessage)
  {
    this.Payload = new byte[0];
  }

  public OmniGetRSSIMessage(OmniBaseMessage msg)
    : base(msg)
  {
    this.MsgType = MessageType.OmniGetRSSIMessage;
  }
}
