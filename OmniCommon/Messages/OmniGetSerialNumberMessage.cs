// Decompiled with JetBrains decompiler
// Type: OmniCommon.Messages.OmniGetSerialNumberMessage
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

using System.Text;

#nullable disable
namespace OmniCommon.Messages;

public class OmniGetSerialNumberMessage : OmniBaseMessage
{
  private const int SerialNumberLength = 16 /*0x10*/;

  public string SerialNumber
  {
    get
    {
      return OmniGetSerialNumberMessage.EqualsMax(this.Payload) ? "Unknown" : Encoding.ASCII.GetString(this.Payload);
    }
  }

  public OmniGetSerialNumberMessage()
    : base(MessageType.OmniGetSerialNumberMessage)
  {
    this.Payload = new byte[0];
  }

  public OmniGetSerialNumberMessage(OmniBaseMessage msg)
    : base(msg)
  {
    this.MsgType = MessageType.OmniGetSerialNumberMessage;
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
