// Decompiled with JetBrains decompiler
// Type: OmniCommon.Messages.OmniFCCTestModeMessage
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

#nullable disable
namespace OmniCommon.Messages;

public class OmniFCCTestModeMessage : OmniBaseMessage
{
  public OmniFCCTestModeMessage()
    : base(MessageType.OmniFCCTestModeMessage)
  {
    this.Payload = new byte[4];
    this.ComPort = string.Empty;
  }

  public OmniFCCTestModeMessage(FCCTestModeSettings mode)
    : base(MessageType.OmniFCCTestModeMessage)
  {
    this.Payload = new byte[4];
    this.Payload[0] = (byte) mode.ModulationType;
    this.Payload[1] = (byte) mode.TestPattern;
    this.Payload[2] = (byte) (mode.FrequencyChannel & (int) sbyte.MaxValue);
    this.Payload[3] = (byte) (mode.PowerLevel & 15);
    this.ComPort = string.Empty;
  }

  public OmniFCCTestModeMessage(OmniBaseMessage msg)
    : base(msg)
  {
    this.MsgType = MessageType.OmniFCCTestModeMessage;
  }

  public FCCTestModeSettings GetMode()
  {
    return new FCCTestModeSettings((ModulationType) this.Payload[0], (TestPattern) this.Payload[1], (int) this.Payload[2], (int) this.Payload[3]);
  }
}
