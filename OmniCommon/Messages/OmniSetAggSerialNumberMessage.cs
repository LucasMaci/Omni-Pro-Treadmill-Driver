// Decompiled with JetBrains decompiler
// Type: OmniCommon.Messages.OmniSetAggSerialNumberMessage
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

using System.IO;
using System.Text;

#nullable disable
namespace OmniCommon.Messages;

public class OmniSetAggSerialNumberMessage : OmniBaseMessage
{
  private const int SerialNumberLength = 16 /*0x10*/;

  public string SerialNumber
  {
    get => Encoding.ASCII.GetString(this.Payload);
    set
    {
      if (value.Length > 16 /*0x10*/)
        throw new IOException("Serial number too long");
      this.Payload = value.Length >= 16 /*0x10*/ ? Encoding.ASCII.GetBytes(value) : throw new IOException("Serial number too short");
    }
  }

  public OmniSetAggSerialNumberMessage(string serialNumber)
    : base(MessageType.OmniSetAggSerialNumberMessage)
  {
    this.Payload = new byte[16 /*0x10*/];
    this.SerialNumber = serialNumber;
  }

  public OmniSetAggSerialNumberMessage(OmniBaseMessage msg)
    : base(msg)
  {
    this.MsgType = MessageType.OmniSetAggSerialNumberMessage;
  }
}
