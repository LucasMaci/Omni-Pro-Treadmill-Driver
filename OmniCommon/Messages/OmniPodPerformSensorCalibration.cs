// Decompiled with JetBrains decompiler
// Type: OmniCommon.Messages.OmniPodPerformSensorCalibration
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

using System;

#nullable disable
namespace OmniCommon.Messages;

public class OmniPodPerformSensorCalibration : OmniBaseMessage
{
  public byte Averages => this.Payload[0];

  public double AXInitial
  {
    get
    {
      byte[] destinationArray = new byte[2];
      Array.Copy((Array) this.Payload, 0, (Array) destinationArray, 0, 2);
      return (double) BitConverter.ToInt16(destinationArray, 0) / 4096.0;
    }
  }

  public double AYInitial
  {
    get
    {
      byte[] destinationArray = new byte[2];
      Array.Copy((Array) this.Payload, 2, (Array) destinationArray, 0, 2);
      return (double) BitConverter.ToInt16(destinationArray, 0) / 4096.0;
    }
  }

  public double AZInitial
  {
    get
    {
      byte[] destinationArray = new byte[2];
      Array.Copy((Array) this.Payload, 4, (Array) destinationArray, 0, 2);
      return (double) BitConverter.ToInt16(destinationArray, 0) / 4096.0;
    }
  }

  public double AXCalibrated
  {
    get
    {
      byte[] destinationArray = new byte[2];
      Array.Copy((Array) this.Payload, 6, (Array) destinationArray, 0, 2);
      return (double) BitConverter.ToInt16(destinationArray, 0) / 4096.0;
    }
  }

  public double AYCalibrated
  {
    get
    {
      byte[] destinationArray = new byte[2];
      Array.Copy((Array) this.Payload, 8, (Array) destinationArray, 0, 2);
      return (double) BitConverter.ToInt16(destinationArray, 0) / 4096.0;
    }
  }

  public double AZCalibrated
  {
    get
    {
      byte[] destinationArray = new byte[2];
      Array.Copy((Array) this.Payload, 10, (Array) destinationArray, 0, 2);
      return (double) BitConverter.ToInt16(destinationArray, 0) / 4096.0;
    }
  }

  public byte Result => this.Payload[12];

  public OmniPodPerformSensorCalibration(byte Averages)
    : base(MessageType.OmniPodPerformSensorCalibration)
  {
    this.Payload = BitConverter.GetBytes((short) Averages);
  }

  public OmniPodPerformSensorCalibration(OmniBaseMessage msg)
    : base(msg)
  {
    this.MsgType = MessageType.OmniPodPerformSensorCalibration;
  }
}
