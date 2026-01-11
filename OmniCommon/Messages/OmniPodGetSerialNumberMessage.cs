// Decompiled with JetBrains decompiler
// Type: OmniCommon.Messages.OmniPodGetSerialNumberMessage
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

using System.Text;

#nullable disable
namespace OmniCommon.Messages;

public class OmniPodGetSerialNumberMessage : OmniBaseMessage
{
  private const int SerialNumberLength = 16 /*0x10*/;

  public string SerialNumber
  {
    get
    {
      if (this.Payload.Length != 16 /*0x10*/)
        return string.Empty;
      byte[] bytes = new byte[16 /*0x10*/];
      for (int index1 = 0; index1 < 4; ++index1)
      {
        for (int index2 = 0; index2 < 4; ++index2)
          bytes[index1 * 4 + index2] = this.Payload[(index1 + 1) * 4 - index2 - 1];
      }
      return Encoding.ASCII.GetString(bytes);
    }
  }

  public OmniPodGetSerialNumberMessage()
    : base(MessageType.OmniPodGetSerialNumberMessage)
  {
    this.Payload = new byte[0];
  }

  public OmniPodGetSerialNumberMessage(OmniBaseMessage msg)
    : base(msg)
  {
    this.MsgType = MessageType.OmniPodGetSerialNumberMessage;
  }
}
