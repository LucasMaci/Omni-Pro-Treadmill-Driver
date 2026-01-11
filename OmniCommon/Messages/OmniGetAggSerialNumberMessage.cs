// Decompiled with JetBrains decompiler
// Type: OmniCommon.Messages.OmniGetAggSerialNumberMessage
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

using System.Text;

#nullable disable
namespace OmniCommon.Messages;

public class OmniGetAggSerialNumberMessage : OmniBaseMessage
{
  private const int SerialNumberLength = 16 /*0x10*/;

  public string SerialNumber
  {
    get
    {
      return OmniGetAggSerialNumberMessage.EqualsMax(this.Payload) ? "Unknown" : Encoding.ASCII.GetString(this.Payload);
    }
  }

  public OmniGetAggSerialNumberMessage()
    : base(MessageType.OmniGetAggSerialNumberMessage)
  {
    this.Payload = new byte[0];
  }

  public OmniGetAggSerialNumberMessage(OmniBaseMessage msg)
    : base(msg)
  {
    this.MsgType = MessageType.OmniGetAggSerialNumberMessage;
  }

  private static bool EqualsMax(byte[] vals)
  {
    foreach (byte val in vals)
    {
      if (val != byte.MaxValue)
        return false;
    }
    return true;
  }
}
