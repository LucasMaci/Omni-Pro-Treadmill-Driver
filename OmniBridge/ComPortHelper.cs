using System.IO.Ports;
using System.Management;
using System.Runtime.Versioning;


/// <summary>
/// Hilfsmethoden für COM-Port-Verwaltung
/// </summary>
public static class ComPortHelper
{
    /// <summary>
    /// Listet alle verfügbaren COM-Ports auf
    /// </summary>
    public static string[] GetAvailablePorts()
    {
        return SerialPort.GetPortNames();
    }

    /// <summary>
    /// Prüft ob ein spezifischer COM-Port verfügbar ist
    /// </summary>
    public static bool IsPortAvailable(string portName)
    {
        return GetAvailablePorts().Contains(portName, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Versucht einen Port zu öffnen um zu testen ob er zugänglich ist
    /// </summary>
    public static bool CanAccessPort(string portName, int baudRate = 115200)
    {
        try
        {
            using var port = new SerialPort(portName, baudRate);
            port.Open();
            port.Close();
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Zeigt alle verfügbaren COM-Ports in der Console an
    /// </summary>
    public static void DisplayAvailablePorts()
    {
        var ports = GetAvailablePorts();
        
        if (ports.Length == 0)
        {
            Console.WriteLine("Keine COM-Ports gefunden!");
            return;
        }

        Console.WriteLine("Verfügbare COM-Ports:");
        Console.WriteLine("=====================");
        
        foreach (var port in ports)
        {
            var accessible = CanAccessPort(port) ? "✓ Zugreifbar" : "✗ Blockiert";
            var description = GetPortDescription(port);
            
            Console.WriteLine($"  {port,-8} {accessible,-15} {description}");
        }
        
        Console.WriteLine();
    }

    /// <summary>
    /// Lässt den Benutzer interaktiv einen COM-Port auswählen
    /// </summary>
    public static string? SelectPortInteractive()
    {
        var ports = GetAvailablePorts();
        
        if (ports.Length == 0)
        {
            Console.WriteLine("Keine COM-Ports gefunden!");
            return null;
        }

        Console.WriteLine("Verfügbare COM-Ports:");
        for (int i = 0; i < ports.Length; i++)
        {
            var accessible = CanAccessPort(ports[i]) ? "✓" : "✗";
            var description = GetPortDescription(ports[i]);
            Console.WriteLine($"  [{i + 1}] {ports[i]} {accessible} - {description}");
        }

        Console.WriteLine();
        Console.Write("Wählen Sie einen Port (1-{0}) oder [Enter] für Standard: ", ports.Length);
        
        var input = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(input))
        {
            return null;
        }

        if (int.TryParse(input, out int selection) && selection >= 1 && selection <= ports.Length)
        {
            return ports[selection - 1];
        }

        Console.WriteLine("Ungültige Auswahl!");
        return null;
    }

    /// <summary>
    /// Versucht eine Beschreibung für den COM-Port zu ermitteln (nur Windows)
    /// </summary>
    [SupportedOSPlatform("windows")]
    private static string GetPortDescriptionWindows(string portName)
    {
        try
        {
            using var searcher = new ManagementObjectSearcher(
                "SELECT * FROM Win32_PnPEntity WHERE Caption LIKE '%(COM%'");
            
            foreach (ManagementObject obj in searcher.Get())
            {
                string? caption = obj["Caption"]?.ToString();
                if (caption != null && caption.Contains(portName, StringComparison.OrdinalIgnoreCase))
                {
                    return caption;
                }
            }
        }
        catch
        {
            // Fehler ignorieren
        }

        return "Unbekanntes Gerät";
    }

    private static string GetPortDescription(string portName)
    {
        if (OperatingSystem.IsWindows())
        {
            return GetPortDescriptionWindows(portName);
        }
        
        return "COM-Port";
    }

    /// <summary>
    /// Findet den ersten verfügbaren und zugänglichen COM-Port
    /// </summary>
    public static string? FindFirstAccessiblePort()
    {
        var ports = GetAvailablePorts();
        
        foreach (var port in ports)
        {
            if (CanAccessPort(port))
            {
                return port;
            }
        }

        return null;
    }
}