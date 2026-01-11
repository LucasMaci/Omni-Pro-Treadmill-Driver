// Decompiled with JetBrains decompiler
// Type: OmniCommon.Messages.OmniBaseMessage
// Assembly: OmniCommon, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null
// MVID: 528622BE-7E82-48CF-9835-746D3CF8734A
// Assembly location: C:\Program Files (x86)\Virtuix\Omni Connect\OmniCommon.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace OmniCommon.Messages;

public class OmniBaseMessage
{
  private static readonly OmniBaseMessage.MessageCommandBiDictionary<MessageType, Command> MessageCommandLookup = new OmniBaseMessage.MessageCommandBiDictionary<MessageType, Command>().Add(MessageType.OmniVersionInfoMessage, Command.GET_VERSION_INFO).Add(MessageType.OmniToggleLEDMessage, Command.GET_POD_MOUNTING_MATRIX).Add(MessageType.OmniBluetoothMessage, Command.CHANGE_BLUETOOTH).Add(MessageType.OmniChangeGamepadModeMessage, Command.CHANGE_GAMEPAD_MODE).Add(MessageType.OmniGetGamepadModeMessage, Command.GET_GAMEPAD_MODE).Add(MessageType.OmniBoardInfoMessage, Command.GET_BOARD_INFO).Add(MessageType.OmniGetRaftConfigurationMessage, Command.GET_GAZELL_CONFIG).Add(MessageType.OmniSetRaftConfigurationMessage, Command.SET_GAZELL_CONFIG).Add(MessageType.OmniGetRaftAddressMessage, Command.GET_GAZELL_ADDRESS).Add(MessageType.OmniSetRaftAddressMessage, Command.SET_GAZELL_ADDRESS).Add(MessageType.OmniPodBatteryLifeMessage, Command.GET_BATTERY_STATUS).Add(MessageType.OmniResetRadioMessage, Command.RESET_NORDIC_RADIO).Add(MessageType.OmniStartBootloaderMessage, Command.START_BOOTLOADER).Add(MessageType.OmniUpgradeRadioMessage, Command.UPGRADE_NORDIC_RADIO).Add(MessageType.OmniSetMotionDataMessage, Command.SET_MOTION_DATA_MODE).Add(MessageType.OmniSetRawDataMessage, Command.SET_RAW_DATA_MODE).Add(MessageType.OmniMotionDataMessage, Command.STREAM_MOTION_DATA).Add(MessageType.OmniRawDataMessage, Command.STREAM_RAW_DATA).Add(MessageType.OmniMotionAndRawDataMessage, Command.STREAM_MOTION_AND_RAW_DATA).Add(MessageType.OmniPodHelloMessage, Command.HELLO_MSG).Add(MessageType.OmniFCCTestEnableMessage, Command.ENABLE_BLUETOOTH_TEST).Add(MessageType.OmniFCCTestModeMessage, Command.SET_BLUETOOTH_TEST_MODE).Add(MessageType.OmniGetRSSIMessage, Command.GET_GAZELL_RSSI).Add(MessageType.OmniResetTivaMessage, Command.RESET).Add(MessageType.OmniPodGetSerialNumberMessage, Command.GET_SERIAL_NUMBER).Add(MessageType.OmniGetSerialNumberMessage, Command.GET_SERIAL).Add(MessageType.OmniSetSerialNumberMessage, Command.SET_SERIAL).Add(MessageType.OmniGetAggSerialNumberMessage, Command.GET_SERIAL_AGG).Add(MessageType.OmniSetAggSerialNumberMessage, Command.SET_SERIAL_AGG).Add(MessageType.OmniGetSensitivityMessage, Command.GET_SENSITIVITY).Add(MessageType.OmniSetSensitivityMessage, Command.SET_SENSITIVITY).Add(MessageType.OmniDisableMovement, Command.DISABLE_MOTION).Add(MessageType.OmniEnableChallenge1, Command.ENABLE_MOTION_CHALLENGE1).Add(MessageType.OmniEnableChallenge2, Command.ENABLE_MOTION_CHALLENGE2).Add(MessageType.OmniSetKey1, Command.SET_KEY1).Add(MessageType.OmniSetKey2, Command.SET_KEY2).Add(MessageType.OmniGetKeyState, Command.GET_KEY_STATE).Add(MessageType.OmniPodPerformSensorCalibration, Command.PERFORM_SENSOR_CALIBRATION);
  public MessageType MsgType;
  public byte[] Payload;
  public string ComPort;
  public string Thread;
  public byte PipeStatusByte = 0;

  public int Pipe
  {
    get => (int) this.PipeStatusByte >> 4;
    set
    {
      this.PipeStatusByte = (byte) ((uint) (byte) (value << 4) | (uint) this.PipeStatusByte & 15U);
    }
  }

  public int ErrorCode
  {
    get => (int) this.PipeStatusByte >> 1 & 7;
    set
    {
      this.PipeStatusByte = (byte) ((uint) (byte) (value << 1) | (uint) this.PipeStatusByte & 241U);
    }
  }

  public bool IsResponse
  {
    get => ((int) this.PipeStatusByte & 1) == 1;
    set => this.PipeStatusByte = (byte) ((value ? 1 : 0) | (int) this.PipeStatusByte & 254);
  }

  public OmniBaseMessage(MessageType msgType)
  {
    this.MsgType = msgType;
    this.ComPort = string.Empty;
    this.Thread = string.Empty;
  }

  public OmniBaseMessage(OmniBaseMessage oldMsg)
  {
    this.MsgType = oldMsg.MsgType;
    this.Payload = new byte[oldMsg.Payload.Length];
    oldMsg.Payload.CopyTo((Array) this.Payload, 0);
    this.ComPort = string.Copy(oldMsg.ComPort);
    this.Thread = string.Copy(oldMsg.Thread);
    this.PipeStatusByte = oldMsg.PipeStatusByte;
  }

  public OmniBaseMessage(byte msgType_byte, byte[] payload, byte pipeStatusByte)
  {
    Command second = (Command) msgType_byte;
    if (!OmniBaseMessage.MessageCommandLookup.TryGetMessageTypeFromCommand(second, out this.MsgType))
      this.MsgType = MessageType.Unknown;
    this.Payload = new byte[payload.Length];
    payload.CopyTo((Array) this.Payload, 0);
    this.ComPort = string.Empty;
    this.Thread = string.Empty;
    this.PipeStatusByte = pipeStatusByte;
  }

  public byte[] Encode()
  {
    Command second;
    OmniBaseMessage.MessageCommandLookup.TryGetCommandFromMessageType(this.MsgType, out second);
    return OmniPacketBuilder.buildPacket((byte) second, this.Payload, this.PipeStatusByte);
  }

  private class MessageCommandBiDictionary<TFirst, TSecond>
  {
    private IDictionary<TFirst, TSecond> firstToSecond = (IDictionary<TFirst, TSecond>) new Dictionary<TFirst, TSecond>();
    private IDictionary<TSecond, TFirst> secondToFirst = (IDictionary<TSecond, TFirst>) new Dictionary<TSecond, TFirst>();

    public OmniBaseMessage.MessageCommandBiDictionary<TFirst, TSecond> Add(
      TFirst first,
      TSecond second)
    {
      if (this.firstToSecond.ContainsKey(first) || this.secondToFirst.ContainsKey(second))
        throw new ArgumentException("Duplicate first or second");
      this.firstToSecond.Add(first, second);
      this.secondToFirst.Add(second, first);
      return this;
    }

    public bool TryGetCommandFromMessageType(TFirst first, out TSecond second)
    {
      return this.firstToSecond.TryGetValue(first, out second);
    }

    public bool TryGetMessageTypeFromCommand(TSecond second, out TFirst first)
    {
      return this.secondToFirst.TryGetValue(second, out first);
    }
  }
}
