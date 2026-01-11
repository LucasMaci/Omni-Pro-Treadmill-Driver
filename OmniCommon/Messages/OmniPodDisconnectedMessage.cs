// Decompiled with JetBrains decompiler
// Type: OmniCommon.Messages.OmniPodDisconnectedMessage
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

using System.Collections.Generic;

#nullable disable
namespace OmniCommon.Messages;

public class OmniPodDisconnectedMessage : OmniBaseMessage
{
  public List<string> Ports;

  public OmniPodDisconnectedMessage(List<string> ports)
    : base(MessageType.OmniPodDisconnectedMessage)
  {
    this.Ports = new List<string>();
    if (ports == null)
      return;
    foreach (string port in ports)
      this.Ports.Add(string.Copy(port));
  }
}
