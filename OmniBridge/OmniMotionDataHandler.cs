using System.IO.Ports;
using OmniCommon;
using OmniCommon.Messages;

/// <summary>
/// Verwaltet die Konfiguration und den Empfang von OmniMotionDataMessage
/// </summary>
public class OmniMotionDataHandler : IDisposable
{
    private readonly SerialPort _port;
    private readonly Thread _readerThread;
    private bool _isRunning;
    private readonly object _lockObject = new();
    private readonly int _configurationDelayMs;
    private OmniMode _currentMode = OmniMode.DecoupledForwardBackStrafe;
    private int _crcErrorCount = 0;

    /// <summary>
    /// Event wird ausgelöst, wenn Motion Data empfangen wurde
    /// </summary>
    public event EventHandler<OmniMotionData>? MotionDataReceived;

    /// <summary>
    /// Event wird ausgelöst, wenn Raw Hex Daten empfangen wurden
    /// </summary>
    public event EventHandler<RawHexDataEventArgs>? RawHexDataReceived;

    /// <summary>
    /// Event wird ausgelöst, wenn ein Verbindungsfehler auftritt
    /// </summary>
    public event EventHandler<string>? ConnectionError;

    /// <summary>
    /// Event wird ausgelöst, wenn der OmniMode geändert wurde
    /// </summary>
    public event EventHandler<OmniMode>? ModeChanged;

    /// <summary>
    /// Event wird ausgelöst, wenn ein CRC-Fehler auftritt
    /// </summary>
    public event EventHandler? CrcErrorOccurred;

    public bool IsConnected => _port?.IsOpen ?? false;
    public string PortName { get; }
    public int BaudRate { get; }
    public OmniMode CurrentMode => _currentMode;
    
    /// <summary>
    /// Gibt die Anzahl der aufgetretenen CRC-Fehler zurück
    /// </summary>
    public int CrcErrorCount => _crcErrorCount;

    public OmniMotionDataHandler(string comPort = "COM3", int baudRate = 115200, int configurationDelayMs = 100, OmniMode initialMode = OmniMode.ForwardBackStrafe)
    {
        PortName = comPort;
        BaudRate = baudRate;
        _configurationDelayMs = configurationDelayMs;
        _currentMode = initialMode;

        _port = new SerialPort(comPort, baudRate)
        {
            ReadTimeout = 500,
            WriteTimeout = 500
        };

        _readerThread = new Thread(ReadLoop)
        {
            IsBackground = true,
            Name = "OmniMotionDataReader"
        };
    }

    /// <summary>
    /// Verbindet mit dem Omni Treadmill und konfiguriert Motion Data Streaming
    /// </summary>
    /// <param name="selection">Die Motion Data Selection - null für AllOn()</param>
    /// <param name="omniMode">Der OmniMode - null für Standard</param>
    public bool Connect(MotionDataSelection? selection = null, OmniMode? omniMode = null)
    {
        try
        {
            if (_port.IsOpen)
            {
                Console.WriteLine("Port ist bereits geöffnet.");
                return true;
            }

            // Prüfe ob Port existiert
            if (!ComPortHelper.IsPortAvailable(PortName))
            {
                var error = $"COM-Port '{PortName}' existiert nicht auf diesem System!";
                Console.WriteLine(error);
                ConnectionError?.Invoke(this, error);
                return false;
            }

            // Prüfe ob Port zugänglich ist
            if (!ComPortHelper.CanAccessPort(PortName, BaudRate))
            {
                var error = $"COM-Port '{PortName}' ist nicht zugänglich (möglicherweise von einem anderen Programm verwendet)!";
                Console.WriteLine(error);
                ConnectionError?.Invoke(this, error);
                return false;
            }

            _port.Open();
            Console.WriteLine($"Verbunden mit {_port.PortName} @ {_port.BaudRate} baud");

            // Setze OmniMode (verwende Settings-Mode oder behalte aktuellen bei)
            var modeToSet = omniMode ?? _currentMode;
            SetMode(modeToSet);

            Thread.Sleep(_configurationDelayMs); // Warte auf Hardware-Reaktion

            // Konfiguriere Motion Data (Standard: Alles aktivieren)
            var motionSelection = selection ?? MotionDataSelection.AllOn();
            ConfigureMotionData(motionSelection);

            Thread.Sleep(_configurationDelayMs); // Warte auf Hardware-Reaktion

            // Starte Reader-Thread
            _isRunning = true;
            _readerThread.Start();

            Console.WriteLine("Motion Data Streaming aktiviert.");
            return true;
        }
        catch (UnauthorizedAccessException ex)
        {
            var error = $"Zugriff verweigert auf {PortName}: {ex.Message}";
            Console.WriteLine(error);
            Console.WriteLine("Mögliche Ursachen:");
            Console.WriteLine("  - Port wird von einem anderen Programm verwendet");
            Console.WriteLine("  - Unzureichende Berechtigungen");
            Console.WriteLine("  - USB-Gerät nicht richtig angeschlossen");
            ConnectionError?.Invoke(this, error);
            return false;
        }
        catch (IOException ex)
        {
            var error = $"I/O-Fehler beim Öffnen von {PortName}: {ex.Message}";
            Console.WriteLine(error);
            ConnectionError?.Invoke(this, error);
            return false;
        }
        catch (Exception ex)
        {
            var error = $"Verbindungsfehler: {ex.Message}";
            Console.WriteLine(error);
            ConnectionError?.Invoke(this, error);
            return false;
        }
    }

    /// <summary>
    /// Setzt den OmniMode (Bewegungsmodus)
    /// </summary>
    public void SetMode(OmniMode mode)
    {
        if (!_port.IsOpen)
        {
            throw new InvalidOperationException("Port ist nicht geöffnet!");
        }

        var previousMode = _currentMode;
        _currentMode = mode;

        var message = new OmniChangeGamepadModeMessage((byte)mode);
        SendMessage(message);

        Console.WriteLine($"OmniMode gesetzt: {mode} ({(int)mode})");

        ModeChanged?.Invoke(this, mode);
    }

    /// <summary>
    /// Konfiguriert welche Motion-Daten gestreamt werden sollen
    /// </summary>
    public void ConfigureMotionData(MotionDataSelection selection)
    {
        if (!_port.IsOpen)
        {
            throw new InvalidOperationException("Port ist nicht geöffnet!");
        }

        var message = new OmniSetMotionDataMessage(selection);
        SendMessage(message);

        Console.WriteLine("Motion Data Konfiguration gesendet:");
        Console.WriteLine($"  - Timestamp: {selection.Timestamp}");
        Console.WriteLine($"  - StepCount: {selection.StepCount}");
        Console.WriteLine($"  - RingAngle: {selection.RingAngle}");
        Console.WriteLine($"  - RingDelta: {selection.RingDelta}");
        Console.WriteLine($"  - GamePadData: {selection.GamePadData}");
        Console.WriteLine($"  - GunButtonData: {selection.GunButtonData}");
        Console.WriteLine($"  - StepTrigger: {selection.StepTrigger}");
    }

    /// <summary>
    /// Erstellt eine Custom Motion Data Selection
    /// </summary>
    public static MotionDataSelection CreateCustomSelection(
        bool timestamp = true,
        bool stepCount = true,
        bool ringAngle = true,
        bool ringDelta = false,
        bool gamePadData = false,
        bool gunButtonData = false,
        bool stepTrigger = true)
    {
        return new MotionDataSelection
        {
            Timestamp = timestamp,
            StepCount = stepCount,
            RingAngle = ringAngle,
            RingDelta = ringDelta,
            GamePadData = gamePadData,
            GunButtonData = gunButtonData,
            StepTrigger = stepTrigger
        };
    }

    /// <summary>
    /// Trennt die Verbindung zum Treadmill
    /// </summary>
    public void Disconnect()
    {
        _isRunning = false;

        if (_readerThread.IsAlive)
        {
            if (!_readerThread.Join(1000))
            {
                Console.WriteLine("Warnung: Reader-Thread konnte nicht sauber beendet werden.");
            }
        }

        if (_port.IsOpen)
        {
            // Deaktiviere Motion Data Streaming vor dem Schließen
            try
            {
                var offSelection = MotionDataSelection.AllOff();
                var message = new OmniSetMotionDataMessage(offSelection);
                SendMessage(message);
                Thread.Sleep(100);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Deaktivieren des Streamings: {ex.Message}");
            }

            _port.Close();
            Console.WriteLine("Verbindung getrennt.");
        }
    }

    private void SendMessage(OmniBaseMessage message)
    {
        lock (_lockObject)
        {
            byte[] packet = message.Encode();
            _port.Write(packet, 0, packet.Length);
        }
    }

    private void ReadLoop()
    {
        byte[] buffer = new byte[256];

        while (_isRunning)
        {
            try
            {
                if (_port.BytesToRead > 0)
                {
                    int bytesRead;
                    
                    lock (_lockObject)
                    {
                        bytesRead = _port.Read(buffer, 0, Math.Min(_port.BytesToRead, buffer.Length));
                    }

                    // Fire Raw Hex Event
                    OnRawHexDataReceived(buffer, bytesRead);

                    // Dekodiere Packet
                    OmniBaseMessage? msg = OmniPacketBuilder.decodePacket(buffer, bytesRead);

                    if (msg != null)
                    {
                        ProcessMessage(msg);
                    }
                    else
                    {
                        // CRC-Fehler: Zählen statt ausgeben
                        _crcErrorCount++;
                        CrcErrorOccurred?.Invoke(this, EventArgs.Empty);
                    }
                }

                Thread.Sleep(10); // CPU schonen
            }
            catch (TimeoutException)
            {
                // Normal bei ReadTimeout
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lesefehler: {ex.Message}");
                Thread.Sleep(100);
            }
        }
    }

    private void ProcessMessage(OmniBaseMessage msg)
    {
        if (msg.ErrorCode != 0)
        {
            Console.WriteLine($"Hardware-Fehler Code: {msg.ErrorCode}");
        }

        if (msg.MsgType == MessageType.OmniMotionDataMessage)
        {
            var motionMsg = new OmniMotionDataMessage(msg);
            var motionData = motionMsg.GetMotionData();
            OnMotionDataReceived(motionData);
        }
        else
        {
            Console.WriteLine($"Unerwarteter Message-Typ empfangen: {msg.MsgType}");
        }
    }

    private void OnMotionDataReceived(OmniMotionData data)
    {
        MotionDataReceived?.Invoke(this, data);
    }

    private void OnRawHexDataReceived(byte[] buffer, int length)
    {
        RawHexDataReceived?.Invoke(this, new RawHexDataEventArgs(buffer, length));
    }

    public void Dispose()
    {
        Disconnect();
        _port?.Dispose();
    }
}

/// <summary>
/// Event-Argumente für Raw Hex Daten
/// </summary>
public class RawHexDataEventArgs : EventArgs
{
    public byte[] Data { get; }
    public int Length { get; }
    public DateTime Timestamp { get; }

    public RawHexDataEventArgs(byte[] data, int length)
    {
        Data = new byte[length];
        Array.Copy(data, Data, length);
        Length = length;
        Timestamp = DateTime.Now;
    }

    /// <summary>
    /// Konvertiert die Daten zu einem Hex-String
    /// </summary>
    public string ToHexString()
    {
        return BitConverter.ToString(Data, 0, Length).Replace("-", " ");
    }

    /// <summary>
    /// Konvertiert die Daten zu einem formatierten Hex-String mit Offsets
    /// </summary>
    public string ToFormattedHexString()
    {
        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < Length; i++)
        {
            if (i % 16 == 0)
            {
                if (i > 0) sb.AppendLine();
                sb.Append($"{i:X4}:  ");
            }
            sb.Append($"{Data[i]:X2} ");
        }
        return sb.ToString();
    }
}

/// <summary>
/// Extension-Methods für OmniMotionData zur besseren Datenausgabe
/// </summary>
public static class OmniMotionDataExtensions
{
    /// <summary>
    /// Erstellt eine formatierte String-Darstellung der Motion Data
    /// </summary>
    public static string ToFormattedString(this OmniMotionData data)
    {
        var lines = new List<string>();

        if (data.EnableTimestamp)
            lines.Add($"Timestamp: {data.Timestamp} ms");

        if (data.EnableStepCount)
            lines.Add($"Steps: {data.StepCount}");

        if (data.EnableRingAngle)
            lines.Add($"Ring Angle: {data.RingAngle:F2}°");

        if (data.EnableRingDelta)
            lines.Add($"Ring Delta: {data.RingDelta}");

        if (data.EnableGamePadData)
            lines.Add($"GamePad: X={data.GamePad_X}, Y={data.GamePad_Y}");

        if (data.EnableGunButtonData)
            lines.Add($"Gun Button: {data.GunButtonData}");

        if (data.EnableStepTrigger)
            lines.Add($"Step Trigger: {data.StepTrigger}");

        return string.Join(Environment.NewLine, lines);
    }

    /// <summary>
    /// Erstellt eine kompakte einzeilige String-Darstellung
    /// </summary>
    public static string ToCompactString(this OmniMotionData data)
    {
        var parts = new List<string>();

        if (data.EnableTimestamp)
            parts.Add($"T:{data.Timestamp}");

        if (data.EnableStepCount)
            parts.Add($"S:{data.StepCount}");

        if (data.EnableRingAngle)
            parts.Add($"A:{data.RingAngle:F1}°");

        if (data.EnableGamePadData)
            parts.Add($"GP:{data.GamePad_X}/{data.GamePad_Y}");

        return string.Join(" ", parts);
    }
}