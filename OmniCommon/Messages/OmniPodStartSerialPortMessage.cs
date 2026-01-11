// Decompiled with JetBrains decompiler
// Type: OmniCommon.Messages.OmniPodStartSerialPortMessage
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

#nullable disable
namespace OmniCommon.Messages;

public class OmniPodStartSerialPortMessage : OmniBaseMessage
{
  public readonly string Port;
  public readonly int Baud;
  public bool isBootloaderMode = false;

  public OmniPodStartSerialPortMessage(string port, int baud)
    : base(MessageType.OmniPodStartSerialPortMessage)
  {
    this.Port = port;
    this.Baud = baud;
  }
}
