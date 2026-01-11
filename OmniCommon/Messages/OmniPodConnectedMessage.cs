// Decompiled with JetBrains decompiler
// Type: OmniCommon.Messages.OmniPodConnectedMessage
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

using System.Collections.Generic;

#nullable disable
namespace OmniCommon.Messages;

public class OmniPodConnectedMessage : OmniBaseMessage
{
  public List<string> Ports;

  public OmniPodConnectedMessage(List<string> ports)
    : base(MessageType.OmniPodConnectedMessage)
  {
    this.Ports = new List<string>();
    foreach (string port in ports)
      this.Ports.Add(string.Copy(port));
  }
}
