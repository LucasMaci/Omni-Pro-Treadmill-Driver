// Decompiled with JetBrains decompiler
// Type: OmniCommon.Messages.OmniStartSerialPortMessage
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

#nullable disable
namespace OmniCommon.Messages;

public class OmniStartSerialPortMessage : OmniBaseMessage
{
  public readonly string Port;
  public readonly int Baud;

  public OmniStartSerialPortMessage(string port, int baud)
    : base(MessageType.OmniStartSerialPortMessage)
  {
    this.Port = port;
    this.Baud = baud;
  }
}
