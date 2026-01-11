# Omnitreadmill Bibliothek - Vollständige Analyse & Nutzungsanleitung

## 📋 Übersicht

Die **OmniCommon**-Bibliothek ist eine .NET Framework 3.5 Bibliothek zur Kommunikation mit dem Virtuix Omni Treadmill. Sie ermöglicht das Auslesen von Sensordaten der Pods (Fußsensoren), Rinkel-Winkel (Ring Angle) und Gamepad-Daten über eine serielle Schnittstelle.

---

## 🏗️ Architektur

### Kernkomponenten

```
graph TB
    A[Ihre Anwendung] --> B[OmniBaseMessage]
    B --> C[OmniPacketBuilder]
    C --> D[Serielle Schnittstelle]
    D --> E[Omni Treadmill Hardware]
    
    B --> F[Message-Typen]
    F --> G[Motion Data Messages]
    F --> H[Raw Data Messages]
    F --> I[Control Messages]
    
    G --> J[OmniMotionDataMessage]
    G --> K[OmniMotionAndRawDataMessage]
    
    H --> L[OmniRawDataMessage]
    
    I --> M[OmniSetMotionDataMessage]
    I --> N[OmniSetRawDataMessage]
    I --> O[OmniVersionInfoMessage]
```

---

## 📦 Nachrichtentypen (MessageType)

Die Bibliothek arbeitet mit verschiedenen Nachrichtentypen für unterschiedliche Zwecke:

### 1. **Konfigurations-Messages** (Senden an Hardware)
| MessageType | Verwendung | Command |
|-------------|-----------|---------|
| `OmniSetMotionDataMessage` | Konfiguriert welche Motion-Daten gestreamt werden | `SET_MOTION_DATA_MODE` |
| `OmniSetRawDataMessage` | Konfiguriert welche Raw-Daten (Pod-Sensoren) gestreamt werden | `SET_RAW_DATA_MODE` |
| `OmniVersionInfoMessage` | Fragt Versions-Informationen ab | `GET_VERSION_INFO` |
| `OmniGetRSSIMessage` | Fragt Bluetooth-Signalstärke ab | `GET_GAZELL_RSSI` |
| `OmniSetSensitivityMessage` | Setzt Empfindlichkeit | `SET_SENSITIVITY` |

### 2. **Datenempfangs-Messages** (Von Hardware empfangen)
| MessageType | Dateninhalt | Command |
|-------------|-------------|---------|
| `OmniMotionDataMessage` | Bewegungsdaten (Winkel, Schritte, Gamepad) | `STREAM_MOTION_DATA` |
| `OmniRawDataMessage` | Rohdaten der Pod-Sensoren (Quaternions, Accelerometer, Gyroscope) | `STREAM_RAW_DATA` |
| `OmniMotionAndRawDataMessage` | **Beide kombiniert** (Motion + Raw Data) | `STREAM_MOTION_AND_RAW_DATA` |

---

## 🎯 Datentypen

### Motion Data (Bewegungsdaten)

**Verfügbare Felder:**
```
- Timestamp        // uint - Zeitstempel
- StepCount        // uint - Anzahl Schritte
- RingAngle        // float - Winkel des Rings (Drehung des Benutzers)
- RingDelta        // byte - Änderung des Winkels
- GamePad_X        // byte - Gamepad X-Achse
- GamePad_Y        // byte - Gamepad Y-Achse
- GunButtonData    // byte - Gun-Button Status
- StepTrigger      // byte - Schritt-Trigger
```

### Raw Data (Pod-Sensordaten)

**Pro Pod verfügbar:**
```
- Quaternions      // float[4] - Orientierung (W, X, Y, Z)
- Accelerometer    // float[3] - Beschleunigung (X, Y, Z) in g
- Gyroscope        // float[3] - Drehrate (X, Y, Z) in °/s
- FrameNumber      // ushort - Frame-Nummer
```

**Wichtig:** Das Omni hat **2 Pods** (Pod1 = linker Fuß, Pod2 = rechter Fuß)

---

## 🔧 Verwendung - Schritt für Schritt

### Schritt 1: Message erstellen und kodieren

```
using OmniCommon;
using OmniCommon.Messages;

// Beispiel 1: Motion Data konfigurieren (alles aktivieren)
MotionDataSelection motionSelection = MotionDataSelection.AllOn();
OmniSetMotionDataMessage motionConfigMsg = new OmniSetMotionDataMessage(motionSelection);
byte[] packetToSend = motionConfigMsg.Encode();

// Beispiel 2: Nur spezifische Motion-Daten
MotionDataSelection customMotion = new MotionDataSelection
{
    Timestamp = true,
    StepCount = true,
    RingAngle = true,  // ← Wichtig für den Winkel!
    RingDelta = true,
    GamePadData = false,
    GunButtonData = false,
    StepTrigger = false
};
OmniSetMotionDataMessage customMsg = new OmniSetMotionDataMessage(customMotion);
byte[] customPacket = customMsg.Encode();
```

### Schritt 2: Raw Data (Pod-Sensoren) konfigurieren

```
// Beispiel 1: Alle Pod-Daten aktivieren (2 Pods)
RawDataSelection rawSelection = RawDataSelection.AllOn(2);
OmniSetRawDataMessage rawConfigMsg = new OmniSetRawDataMessage(rawSelection);
byte[] rawPacket = rawConfigMsg.Encode();

// Beispiel 2: Nur Quaternions und Accelerometer
RawDataSelection customRaw = new RawDataSelection
{
    Count = 2,  // 2 Pods
    Timestamp = true,
    Pods = new List<RawPodDataMode>
    {
        // Pod 1 (linker Fuß)
        new RawPodDataMode
        {
            Quaternions = true,
            Accelerometer = true,
            Gyroscope = false,
            FrameNumber = false
        },
        // Pod 2 (rechter Fuß)
        new RawPodDataMode
        {
            Quaternions = true,
            Accelerometer = true,
            Gyroscope = false,
            FrameNumber = false
        }
    }
};
OmniSetRawDataMessage customRawMsg = new OmniSetRawDataMessage(customRaw);
byte[] customRawPacket = customRawMsg.Encode();
```

### Schritt 3: Packet senden (über SerialPort)

```
using System.IO.Ports;

SerialPort omniPort = new SerialPort("COM3", 115200); // Anpassen!
omniPort.Open();

// Sende Konfigurations-Packet
omniPort.Write(packetToSend, 0, packetToSend.Length);

// Optional: Warte kurz
System.Threading.Thread.Sleep(100);
```

### Schritt 4: Daten empfangen und dekodieren

```
// Buffer für eingehende Daten
byte[] buffer = new byte[256];
int bytesRead = 0;

// Empfange Daten
if (omniPort.BytesToRead > 0)
{
    bytesRead = omniPort.Read(buffer, 0, buffer.Length);
    
    // Dekodiere das Packet
    OmniBaseMessage receivedMsg = OmniPacketBuilder.decodePacket(buffer, bytesRead);
    
    if (receivedMsg != null)
    {
        // Prüfe Message-Typ und verarbeite
        switch (receivedMsg.MsgType)
        {
            case MessageType.OmniMotionDataMessage:
                ProcessMotionData(receivedMsg);
                break;
                
            case MessageType.OmniRawDataMessage:
                ProcessRawData(receivedMsg);
                break;
                
            case MessageType.OmniMotionAndRawDataMessage:
                ProcessCombinedData(receivedMsg);
                break;
        }
    }
}
```

### Schritt 5: Daten extrahieren

#### A) Motion Data extrahieren

```
void ProcessMotionData(OmniBaseMessage baseMsg)
{
    OmniMotionDataMessage motionMsg = new OmniMotionDataMessage(baseMsg);
    OmniMotionData data = motionMsg.GetMotionData();
    
    if (data.EnableRingAngle)
    {
        float angle = data.RingAngle; // Winkel in Grad (0-360)
        Console.WriteLine($"Ring Angle: {angle}°");
    }
    
    if (data.EnableStepCount)
    {
        uint steps = data.StepCount;
        Console.WriteLine($"Steps: {steps}");
    }
    
    if (data.EnableGamePadData)
    {
        byte x = data.GamePad_X;
        byte y = data.GamePad_Y;
        Console.WriteLine($"Gamepad: X={x}, Y={y}");
    }
}
```

#### B) Raw Data (Pod-Sensoren) extrahieren

```
void ProcessRawData(OmniBaseMessage baseMsg)
{
    OmniRawDataMessage rawMsg = new OmniRawDataMessage(baseMsg);
    OmniRawData data = rawMsg.GetRawData();
    
    // Zugriff auf Pod-Daten
    for (int i = 0; i < data.PodData.Count; i++)
    {
        PodRawData pod = data.PodData[i];
        
        Console.WriteLine($"=== Pod {i + 1} ===");
        
        if (pod.EnableQuaternions && pod.Quaternions != null)
        {
            Console.WriteLine($"Quaternions: W={pod.Quaternions[0]}, " +
                            $"X={pod.Quaternions[1]}, " +
                            $"Y={pod.Quaternions[2]}, " +
                            $"Z={pod.Quaternions[3]}");
        }
        
        if (pod.EnableAccelerometer && pod.Accelerometer != null)
        {
            Console.WriteLine($"Accelerometer: X={pod.Accelerometer[0]}g, " +
                            $"Y={pod.Accelerometer[1]}g, " +
                            $"Z={pod.Accelerometer[2]}g");
        }
        
        if (pod.EnableGyroscope && pod.Gyroscope != null)
        {
            Console.WriteLine($"Gyroscope: X={pod.Gyroscope[0]}°/s, " +
                            $"Y={pod.Gyroscope[1]}°/s, " +
                            $"Z={pod.Gyroscope[2]}°/s");
        }
    }
}
```

#### C) Kombinierte Daten (Motion + Raw)

```
void ProcessCombinedData(OmniBaseMessage baseMsg)
{
    OmniMotionAndRawDataMessage combinedMsg = new OmniMotionAndRawDataMessage(baseMsg);
    OmniMotionAndRawData data = combinedMsg.GetMotionAndRawData();
    
    // Motion-Daten
    Console.WriteLine($"Timestamp: {data.Timestamp}");
    Console.WriteLine($"Steps: {data.StepCount}");
    Console.WriteLine($"Ring Angle: {data.RingAngle}°");
    
    // Pod 1 Daten
    if (data.Pod1Quaternions != null)
    {
        Console.WriteLine($"Pod1 Quaternions: {string.Join(", ", data.Pod1Quaternions)}");
    }
    
    if (data.Pod1Accelerometer != null)
    {
        Console.WriteLine($"Pod1 Accel: {string.Join(", ", data.Pod1Accelerometer)}");
    }
    
    // Pod 2 Daten
    if (data.Pod2Quaternions != null)
    {
        Console.WriteLine($"Pod2 Quaternions: {string.Join(", ", data.Pod2Quaternions)}");
    }
}
```

---

## 🎮 Wichtige Commands

### Steuerungsbefehle

```
// Versions-Info abfragen
OmniVersionInfoMessage versionMsg = new OmniVersionInfoMessage();
byte[] versionPacket = versionMsg.Encode();
omniPort.Write(versionPacket, 0, versionPacket.Length);

// RSSI (Bluetooth-Signalstärke) abfragen
OmniGetRSSIMessage rssiMsg = new OmniGetRSSIMessage();
byte[] rssiPacket = rssiMsg.Encode();
omniPort.Write(rssiPacket, 0, rssiPacket.Length);

// Treadmill zurücksetzen
OmniResetTivaMessage resetMsg = new OmniResetTivaMessage();
byte[] resetPacket = resetMsg.Encode();
omniPort.Write(resetPacket, 0, resetPacket.Length);
```

---

## 🔍 Packet-Struktur

```
┌─────────────────────────────────────────────────┐
│ Byte 0: 0xEF (Start-Marker)                    │
├─────────────────────────────────────────────────┤
│ Byte 1: Packet Length (Payload + 8)            │
├─────────────────────────────────────────────────┤
│ Byte 2: Command/MessageType                     │
├─────────────────────────────────────────────────┤
│ Byte 3: Packet ID                               │
├─────────────────────────────────────────────────┤
│ Byte 4: Pipe/Status Byte                        │
│   - Bits 4-7: Pipe Number                       │
│   - Bits 1-3: Error Code                        │
│   - Bit 0: IsResponse Flag                      │
├─────────────────────────────────────────────────┤
│ Byte 5-N: Payload Data                          │
├─────────────────────────────────────────────────┤
│ Byte N+1, N+2: CRC16 (Little Endian)           │
├─────────────────────────────────────────────────┤
│ Byte N+3: 0xBE (End-Marker)                    │
└─────────────────────────────────────────────────┘
```

---

## 💡 Best Practices

### 1. **Datenumschaltung (Switching)**

```
// Um zwischen Modi zu wechseln, sende neue Konfigurations-Messages

// Motion Data aktivieren
MotionDataSelection motionSel = MotionDataSelection.AllOn();
SendMessage(new OmniSetMotionDataMessage(motionSel));

// Nach Bedarf: Raw Data aktivieren (kann parallel laufen)
RawDataSelection rawSel = RawDataSelection.AllOn(2);
SendMessage(new OmniSetRawDataMessage(rawSel));

// Oder: Alles ausschalten
SendMessage(new OmniSetMotionDataMessage(MotionDataSelection.AllOff()));
SendMessage(new OmniSetRawDataMessage(RawDataSelection.AllOff()));
```

### 2. **Effizienter Datenabruf**

```
// Nur benötigte Daten aktivieren für bessere Performance
MotionDataSelection efficientMotion = new MotionDataSelection
{
    Timestamp = true,
    StepCount = false,  // Nicht benötigt
    RingAngle = true,   // Benötigt für Richtung
    RingDelta = false,
    GamePadData = false,
    GunButtonData = false,
    StepTrigger = true  // Benötigt für Schritterkennung
};
```

### 3. **Error Handling**

```
OmniBaseMessage msg = OmniPacketBuilder.decodePacket(buffer, bytesRead);

if (msg == null)
{
    Console.WriteLine("CRC-Fehler oder ungültiges Packet!");
    return;
}

if (msg.ErrorCode != 0)
{
    Console.WriteLine($"Hardware-Fehler: {msg.ErrorCode}");
}

if (msg.IsResponse)
{
    Console.WriteLine("Dies ist eine Antwort auf einen vorherigen Command");
}
```

### 4. **Continuous Reading Loop**

```
bool isRunning = true;
byte[] buffer = new byte[256];

while (isRunning)
{
    try
    {
        if (omniPort.BytesToRead > 0)
        {
            int bytesRead = omniPort.Read(buffer, 0, buffer.Length);
            OmniBaseMessage msg = OmniPacketBuilder.decodePacket(buffer, bytesRead);
            
            if (msg != null)
            {
                ProcessMessage(msg);
            }
        }
        
        System.Threading.Thread.Sleep(10); // CPU schonen
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Fehler: {ex.Message}");
    }
}
```

---

## 📊 Datenkonvertierung

### Ring Angle
- **Wertebereich:** 0.0 bis 360.0 Grad
- **Typ:** `float`
- **Bedeutung:** Absolute Richtung des Benutzers

### Quaternions (Orientierung)
- **Array:** `float[4]` → `[W, X, Y, Z]`
- **Normalisiert:** Länge sollte ≈ 1.0 sein
- **Konvertierung zu Euler-Winkeln möglich**

### Accelerometer
- **Array:** `float[3]` → `[X, Y, Z]`
- **Einheit:** g (Erdbeschleunigung)
- **Skalierung:** Wert / 4096.0
- **Bereich:** Typisch ±2g bis ±8g

### Gyroscope
- **Array:** `float[3]` → `[X, Y, Z]`
- **Einheit:** Grad pro Sekunde (°/s)
- **Skalierung:** Wert / 1024.0
- **Bereich:** Typisch ±250°/s bis ±2000°/s

---

## 🚀 Zusammenfassung

### Grundlegender Workflow

```
sequenceDiagram
    participant App as Ihre App
    participant Lib as OmniCommon
    participant HW as Omni Hardware
    
    App->>Lib: Erstelle OmniSetMotionDataMessage
    Lib->>App: Encode() → byte[]
    App->>HW: SerialPort.Write(bytes)
    
    Note over HW: Hardware verarbeitet Konfiguration
    
    HW->>App: Sendet Motion Data Stream
    App->>Lib: decodePacket(buffer)
    Lib->>App: OmniBaseMessage
    App->>Lib: GetMotionData()
    Lib->>App: OmniMotionData mit allen Werten
    
    App->>App: Verarbeite RingAngle, Steps, etc.
```

### Schalter-Übersicht

| Was Sie tun möchten | Welche Message | Wichtige Properties |
|---------------------|---------------|---------------------|
| Winkel-Daten erhalten | `OmniSetMotionDataMessage` | `RingAngle = true` |
| Pod-Quaternions erhalten | `OmniSetRawDataMessage` | `Pods[i].Quaternions = true` |
| Beschleunigungsdaten | `OmniSetRawDataMessage` | `Pods[i].Accelerometer = true` |
| Gyroskop-Daten | `OmniSetRawDataMessage` | `Pods[i].Gyroscope = true` |
| Schrittzähler | `OmniSetMotionDataMessage` | `StepCount = true` |
| Gamepad-Daten | `OmniSetMotionDataMessage` | `GamePadData = true` |
| Alles auf einmal | `OmniMotionAndRawDataMessage` | Kombiniert beide Modi |

---

## ⚠️ Wichtige Hinweise

1. **Thread-Safety:** Die Bibliothek ist NICHT thread-safe. Verwenden Sie Locks bei Multi-Threading!

2. **COM-Port:** Der korrekte COM-Port muss ermittelt werden (z.B. "COM3", "COM4", etc.)

3. **Baudrate:** Standardmäßig **115200** für Omni Treadmill

4. **Packet-Größe:** Maximale Payload-Größe beachten (Typ `byte` für Length)

5. **CRC-Prüfung:** Immer prüfen ob `decodePacket()` `null` zurückgibt

6. **Timing:** Nach Konfigurations-Messages kurz warten (100ms) bevor Daten erwartet werden

---

## 📚 Message-Klassen Referenz

### Konfigurations-Klassen

#### MotionDataSelection
```
public class MotionDataSelection
{
    public bool Timestamp;
    public bool StepCount;
    public bool RingAngle;
    public bool RingDelta;
    public bool GamePadData;
    public bool GunButtonData;
    public bool StepTrigger;
    
    // Factory-Methoden
    public static MotionDataSelection AllOn();
    public static MotionDataSelection AllOff();
    public static MotionDataSelection StreamingWindowData();
}
```

#### RawDataSelection
```
public class RawDataSelection
{
    public int Count;  // Anzahl Pods (normalerweise 2)
    public bool Timestamp;
    public List<RawPodDataMode> Pods;
    
    // Factory-Methoden
    public static RawDataSelection AllOn(int count);
    public static RawDataSelection AllOff(int count);
    public static RawDataSelection AllOff();
}
```

#### RawPodDataMode
```
public class RawPodDataMode
{
    public bool Quaternions;
    public bool Accelerometer;
    public bool Gyroscope;
    public bool FrameNumber;
    
    public byte GetByte();  // Konvertiert zu Byte-Repräsentation
}
```

### Daten-Klassen

#### OmniMotionData
```
public class OmniMotionData
{
    public bool EnableTimestamp;
    public bool EnableStepCount;
    public bool EnableRingAngle;
    public bool EnableRingDelta;
    public bool EnableGamePadData;
    public bool EnableGunButtonData;
    public bool EnableStepTrigger;
    
    public uint Timestamp;
    public uint StepCount;
    public float RingAngle;
    public byte RingDelta;
    public byte GamePad_X;
    public byte GamePad_Y;
    public byte GunButtonData;
    public byte StepTrigger;
}
```

#### OmniRawData
```
public class OmniRawData
{
    public int Count;
    public bool EnableTimestamp;
    public uint Timestamp;
    public List<RawPodDataMode> Pods;
    public List<PodRawData> PodData;
}
```

#### PodRawData
```
public class PodRawData
{
    public bool EnableQuaternions;
    public bool EnableAccelerometer;
    public bool EnableGyroscope;
    
    public float[] Quaternions;     // [4] - W, X, Y, Z
    public float[] Accelerometer;   // [3] - X, Y, Z in g
    public float[] Gyroscope;       // [3] - X, Y, Z in °/s
    public ushort FrameNumber;
}
```

#### OmniMotionAndRawData
```
public class OmniMotionAndRawData
{
    // Motion-Daten
    public uint Timestamp;
    public uint StepCount;
    public float RingAngle;
    public byte RingDelta;
    public byte GamePad_X;
    public byte GamePad_Y;
    public byte StepTrigger;
    
    // Pod 1 Raw-Daten
    public float[] Pod1Quaternions;
    public float[] Pod1Accelerometer;
    public float[] Pod1Gyroscope;
    
    // Pod 2 Raw-Daten
    public float[] Pod2Quaternions;
    public float[] Pod2Accelerometer;
    public float[] Pod2Gyroscope;
}
```

---

## 🔧 Erweiterte Beispiele

### Beispiel 1: Vollständige Initialisierung

```
using System;
using System.IO.Ports;
using System.Threading;
using OmniCommon;
using OmniCommon.Messages;

public class OmniTreadmillController
{
    private SerialPort port;
    private Thread readerThread;
    private bool isRunning;
    
    public event EventHandler<OmniMotionData> MotionDataReceived;
    public event EventHandler<OmniRawData> RawDataReceived;
    
    public bool Connect(string comPort)
    {
        try
        {
            port = new SerialPort(comPort, 115200);
            port.Open();
            
            // Konfiguriere Motion Data
            MotionDataSelection motionSel = MotionDataSelection.AllOn();
            SendMessage(new OmniSetMotionDataMessage(motionSel));
            
            Thread.Sleep(100);
            
            // Konfiguriere Raw Data für 2 Pods
            RawDataSelection rawSel = RawDataSelection.AllOn(2);
            SendMessage(new OmniSetRawDataMessage(rawSel));
            
            // Starte Reader-Thread
            isRunning = true;
            readerThread = new Thread(ReadLoop);
            readerThread.Start();
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Verbindungsfehler: {ex.Message}");
            return false;
        }
    }
    
    public void Disconnect()
    {
        isRunning = false;
        
        if (readerThread != null && readerThread.IsAlive)
        {
            readerThread.Join(1000);
        }
        
        if (port != null && port.IsOpen)
        {
            port.Close();
        }
    }
    
    private void SendMessage(OmniBaseMessage message)
    {
        byte[] packet = message.Encode();
        port.Write(packet, 0, packet.Length);
    }
    
    private void ReadLoop()
    {
        byte[] buffer = new byte[256];
        
        while (isRunning)
        {
            try
            {
                if (port.BytesToRead > 0)
                {
                    int bytesRead = port.Read(buffer, 0, buffer.Length);
                    OmniBaseMessage msg = OmniPacketBuilder.decodePacket(buffer, bytesRead);
                    
                    if (msg != null)
                    {
                        ProcessMessage(msg);
                    }
                }
                
                Thread.Sleep(10);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lesefehler: {ex.Message}");
            }
        }
    }
    
    private void ProcessMessage(OmniBaseMessage msg)
    {
        switch (msg.MsgType)
        {
            case MessageType.OmniMotionDataMessage:
                OmniMotionDataMessage motionMsg = new OmniMotionDataMessage(msg);
                OmniMotionData motionData = motionMsg.GetMotionData();
                MotionDataReceived?.Invoke(this, motionData);
                break;
                
            case MessageType.OmniRawDataMessage:
                OmniRawDataMessage rawMsg = new OmniRawDataMessage(msg);
                OmniRawData rawData = rawMsg.GetRawData();
                RawDataReceived?.Invoke(this, rawData);
                break;
                
            case MessageType.OmniMotionAndRawDataMessage:
                OmniMotionAndRawDataMessage combinedMsg = new OmniMotionAndRawDataMessage(msg);
                OmniMotionAndRawData combinedData = combinedMsg.GetMotionAndRawData();
                // Verarbeite kombinierte Daten
                break;
        }
    }
}
```

### Beispiel 2: Verwendung des Controllers

```
class Program
{
    static void Main(string[] args)
    {
        OmniTreadmillController controller = new OmniTreadmillController();
        
        // Event-Handler registrieren
        controller.MotionDataReceived += OnMotionDataReceived;
        controller.RawDataReceived += OnRawDataReceived;
        
        // Verbinden
        if (controller.Connect("COM3"))
        {
            Console.WriteLine("Verbunden mit Omni Treadmill");
            Console.WriteLine("Drücken Sie eine Taste zum Beenden...");
            Console.ReadKey();
            
            controller.Disconnect();
        }
        else
        {
            Console.WriteLine("Verbindung fehlgeschlagen!");
        }
    }
    
    static void OnMotionDataReceived(object sender, OmniMotionData data)
    {
        Console.WriteLine($"Ring Angle: {data.RingAngle:F2}°");
        Console.WriteLine($"Steps: {data.StepCount}");
        Console.WriteLine($"Timestamp: {data.Timestamp}");
        Console.WriteLine();
    }
    
    static void OnRawDataReceived(object sender, OmniRawData data)
    {
        for (int i = 0; i < data.PodData.Count; i++)
        {
            PodRawData pod = data.PodData[i];
            Console.WriteLine($"Pod {i + 1}:");
            
            if (pod.Quaternions != null)
            {
                Console.WriteLine($"  Quat: [{pod.Quaternions[0]:F3}, {pod.Quaternions[1]:F3}, " +
                                $"{pod.Quaternions[2]:F3}, {pod.Quaternions[3]:F3}]");
            }
            
            if (pod.Accelerometer != null)
            {
                Console.WriteLine($"  Accel: [{pod.Accelerometer[0]:F3}g, " +
                                $"{pod.Accelerometer[1]:F3}g, {pod.Accelerometer[2]:F3}g]");
            }
        }
        Console.WriteLine();
    }
}
```

### Beispiel 3: Quaternion zu Euler-Winkel Konvertierung

```
public class QuaternionHelper
{
    public static void QuaternionToEuler(float[] q, out float roll, out float pitch, out float yaw)
    {
        // q = [W, X, Y, Z]
        float w = q[0];
        float x = q[1];
        float y = q[2];
        float z = q[3];
        
        // Roll (X-Achse)
        float sinr_cosp = 2.0f * (w * x + y * z);
        float cosr_cosp = 1.0f - 2.0f * (x * x + y * y);
        roll = (float)Math.Atan2(sinr_cosp, cosr_cosp);
        
        // Pitch (Y-Achse)
        float sinp = 2.0f * (w * y - z * x);
        if (Math.Abs(sinp) >= 1)
            pitch = (float)Math.CopySign(Math.PI / 2, sinp); // ±90°
        else
            pitch = (float)Math.Asin(sinp);
        
        // Yaw (Z-Achse)
        float siny_cosp = 2.0f * (w * z + x * y);
        float cosy_cosp = 1.0f - 2.0f * (y * y + z * z);
        yaw = (float)Math.Atan2(siny_cosp, cosy_cosp);
        
        // Konvertiere zu Grad
        roll = roll * 180.0f / (float)Math.PI;
        pitch = pitch * 180.0f / (float)Math.PI;
        yaw = yaw * 180.0f / (float)Math.PI;
    }
}

// Verwendung
float[] quaternions = podData.Quaternions;
float roll, pitch, yaw;
QuaternionHelper.QuaternionToEuler(quaternions, out roll, out pitch, out yaw);
Console.WriteLine($"Roll: {roll:F2}°, Pitch: {pitch:F2}°, Yaw: {yaw:F2}°");
```

---

## 🐛 Troubleshooting

### Problem: Keine Daten empfangen

**Lösung:**
```
// 1. Prüfen Sie die Verbindung
if (!port.IsOpen)
{
    Console.WriteLine("Port ist nicht geöffnet!");
}

// 2. Prüfen Sie ob Konfigurations-Messages gesendet wurden
MotionDataSelection selection = MotionDataSelection.AllOn();
OmniSetMotionDataMessage msg = new OmniSetMotionDataMessage(selection);
byte[] packet = msg.Encode();
port.Write(packet, 0, packet.Length);
Thread.Sleep(200); // Warten Sie länger

// 3. Prüfen Sie die Baudrate
// Omni verwendet 115200
```

### Problem: CRC-Fehler

**Lösung:**
```
// Stellen Sie sicher, dass Sie die vollständige Message lesen
byte[] buffer = new byte[256];
int bytesRead = 0;

// Warten bis genug Daten verfügbar sind
while (port.BytesToRead < 8) // Minimum-Packet-Größe
{
    Thread.Sleep(10);
}

bytesRead = port.Read(buffer, 0, Math.Min(port.BytesToRead, buffer.Length));

OmniBaseMessage msg = OmniPacketBuilder.decodePacket(buffer, bytesRead);
if (msg == null)
{
    Console.WriteLine("CRC-Fehler oder unvollständige Daten");
    // Buffer leeren
    port.DiscardInBuffer();
}
```

### Problem: Threading-Probleme

**Lösung:**
```
// Verwenden Sie Locks für Thread-Safety
private object lockObject = new object();

private void SendMessage(OmniBaseMessage message)
{
    lock (lockObject)
    {
        byte[] packet = message.Encode();
        port.Write(packet, 0, packet.Length);
    }
}

private void ReadLoop()
{
    while (isRunning)
    {
        lock (lockObject)
        {
            if (port.BytesToRead > 0)
            {
                // Lese Daten...
            }
        }
        Thread.Sleep(10);
    }
}
```

---

## 📖 Glossar

| Begriff | Bedeutung |
|---------|-----------|
| **Pod** | Fußsensor des Omni Treadmill (links/rechts) |
| **Ring Angle** | Drehwinkel des Benutzers auf dem Treadmill (0-360°) |
| **Quaternion** | 4D-Repräsentation der Orientierung (W, X, Y, Z) |
| **Accelerometer** | Beschleunigungssensor (misst lineare Beschleunigung) |
| **Gyroscope** | Drehratensensor (misst Winkelgeschwindigkeit) |
| **CRC16** | Cyclic Redundancy Check - Prüfsumme zur Fehlererkennung |
| **Payload** | Nutzdaten innerhalb eines Packets |
| **Pipe** | Kommunikationskanal (Bits 4-7 des Status-Bytes) |
| **RSSI** | Received Signal Strength Indicator - Signalstärke |

---