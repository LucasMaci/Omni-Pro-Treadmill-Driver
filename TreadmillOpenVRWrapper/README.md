# TreadmillOpenVRWrapper

OpenVR API wrapper that injects Virtuix Omni Pro treadmill input into VR games.

## How It Works

This DLL replaces a game's `openvr_api.dll` and forwards all OpenVR calls to the real DLL. On input queries it injects the treadmill movement data so the game treats walking on the treadmill as joystick input.

```
┌─────────────────────────────────────────────────┐
│  VR Game (SkyrimVR, Fallout 4 VR, etc.)         │
│  Loads: openvr_api.dll (OUR WRAPPER DLL)        │
└──────────────────────┬──────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────┐
│  TreadmillOpenVRWrapper (openvr_api.dll)        │
│  ├─ Loads openvr_api_original.dll               │
│  ├─ Forwards all calls                          │
│  ├─ Intercepts IVRInput::GetAnalogActionData()  │
│  ├─ Intercepts IVRSystem::GetControllerState()  │
│  └─ Injects treadmill X/Y into movement actions │
└──────────────────────┬──────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────┐
│  openvr_api_original.dll (real OpenVR DLL)      │
│  └─ Communicates with SteamVR                   │
└─────────────────────────────────────────────────┘
```

## Installation (Mod Organizer 2)

This is the recommended installation method for SkyrimVR with MO2 (e.g. MGON/VRIK modlists).

### Prerequisites

- SteamVR with the treadmill driver installed and running
- Virtuix Omni Pro treadmill connected via USB
- Mod Organizer 2

### Steps

1. **Download** the latest `TreadmillOpenVRWrapper-vx.x.x.zip` from the [Releases](https://github.com/LucasMaci/Omni-Pro-Treadmill-Driver/releases) page.

2. **Import into MO2**: In Mod Organizer 2, click the archive install button (top-left) and select the downloaded ZIP file. MO2 will create the mod with a `Root` folder structure.

3. **Run the setup script** (one-time only):
   - Right-click the installed mod in MO2 → *Open in Explorer*
   - Open PowerShell in that folder
   - Run:
     ```
     .\setup.ps1
     ```
   - The script automatically finds your SkyrimVR installation and copies the original `openvr_api.dll` into the mod as `openvr_api_original.dll`.

4. **Enable the mod** in MO2's left panel.

5. **Edit the config** (optional): Open `Root\treadmill_config.json` and adjust the COM port and other settings (see [Configuration](#configuration)).

6. **Launch the game**:
   - Start SteamVR first (the treadmill driver becomes the "master")
   - Start SkyrimVR through MO2 (the wrapper connects as "consumer")

> **Note:** The `Root` folder structure tells MO2 to place the files directly into the game's root directory, alongside `SkyrimVR.exe`.

### Manual Installation (without MO2)

1. Navigate to your game folder (e.g. `Steam\steamapps\common\SkyrimVR\`)
2. **Back up** the original DLL:
   ```
   ren openvr_api.dll openvr_api_original.dll
   ```
3. Copy these files into the game folder:
   - `openvr_api.dll` (the wrapper DLL)
   - `OmniBridge.dll`
   - `treadmill_config.json`
4. Edit `treadmill_config.json` and set the correct COM port
5. Start SteamVR, then start the game

## Calibration

The wrapper rotates treadmill movement relative to your HMD direction so that walking forward on the treadmill moves you forward in-game regardless of which way you face.

### How to Calibrate

1. **Face forward** on your treadmill — look in the direction you consider "forward" in-game.
2. **Hold both grips + one trigger** on your VR controllers for **2 seconds**.
3. A SteamVR notification will appear: **"Treadmill calibration saved!"**
4. The calibration is saved to disk and persists across game sessions.

### When to Recalibrate

- After starting a new game session if directions feel off
- If you repositioned yourself on the treadmill
- After SteamVR recenters your playspace

### Known Issue

> **⚠️ The SteamVR notification overlay is currently not working reliably.** You may not see the "Treadmill calibration saved!" message in your headset. The calibration still works — check the log file at `%TEMP%\treadmill_wrapper.log` for the `RECENTER: Calibration saved` entry to confirm.

### Accidental Recalibration Protection

The 2-second hold requirement prevents accidental recalibration during gameplay (e.g. archery in SkyrimVR where you press grip + trigger simultaneously). If you release the buttons before 2 seconds, the calibration is cancelled.

## Configuration

Edit `treadmill_config.json` (located next to `openvr_api.dll`):

```json
{
    "enabled": true,
    "comPort": "COM3",
    "baudRate": 115200,
    "speedMultiplier": 1.5,
    "deadzone": 0.1,
    "smoothing": 0.3,
    "targetControllerIndex": -1,
    "inputMode": "smart",
    "actionPatterns": [
        "*move*",
        "*locomotion*",
        "*walk*",
        "*thumbstick*",
        "/actions/*/in/Move"
    ],
    "debugLog": true
}
```

### Settings

| Setting | Default | Description |
|---------|---------|-------------|
| `enabled` | `true` | Enable/disable the wrapper |
| `comPort` | `"COM3"` | Treadmill COM port (check Device Manager) |
| `baudRate` | `115200` | Serial baud rate |
| `speedMultiplier` | `1.5` | Movement speed multiplier |
| `deadzone` | `0.1` | Ignore small movements below this threshold |
| `smoothing` | `0.3` | Input smoothing factor (0 = no smoothing, 1 = max) |
| `targetControllerIndex` | `-1` | Controller to inject input into (-1 = all) |
| `inputMode` | `"smart"` | Input injection mode (see below) |
| `actionPatterns` | `["*move*", ...]` | OpenVR action name patterns to intercept |
| `debugLog` | `true` | Write debug log to `%TEMP%\treadmill_wrapper.log` |

### Input Modes

- **`override`** — Treadmill replaces controller input when active
- **`additive`** — Treadmill input is added to controller input
- **`smart`** — Treadmill overrides only when treadmill movement is detected (recommended)

## Supported Games

| Game | Input Path | Notes |
|------|-----------|-------|
| SkyrimVR | Legacy (`GetControllerState`) | Works with MGON/VRIK |
| Fallout 4 VR | Legacy | Tested |
| Boneworks | IVRInput (`GetAnalogActionData`) | Should work |
| H3VR | IVRInput | Should work |
| Blade & Sorcery (legacy) | IVRInput | Older versions using OpenVR |

## Debugging

The log file is written to `%TEMP%\treadmill_wrapper.log`.

Key log entries to look for:

```
[INFO]  TreadmillOpenVRWrapper Initializing
[INFO]  Treadmill connected successfully!
[DEBUG] Detected movement action: /actions/gameplay/in/Move (handle=0x...)
[TRACE] ACTIVE [LEGACY]: TM=312.8 HMD=16.6 Off=52.7 Rel=-11.0 | Raw(0.00,1.00) -> Rot(-0.19,0.98)
[TRACE] RECENTER: Calibration saved (held 2034ms). offset=52.7
```

- **TM** = Treadmill yaw (degrees)
- **HMD** = Head-mounted display yaw (degrees)
- **Off** = Calibration offset (degrees)
- **Rel** = Relative angle between treadmill and HMD (should be close to 0 when facing forward)
- **Raw** = Raw treadmill X/Y input
- **Rot** = Rotated (calibrated) X/Y output sent to the game

## Uninstall

### MO2
Simply disable or remove the mod in Mod Organizer 2.

### Manual
1. Delete `openvr_api.dll` (the wrapper)
2. Rename `openvr_api_original.dll` back to `openvr_api.dll`
3. Delete `OmniBridge.dll` and `treadmill_config.json`

## Build

Requirements:
- Visual Studio 2022
- Windows SDK 10.0
- C++20

```bash
# Open TreadmillOpenVRWrapper.sln in Visual Studio
# Build -> Build Solution (x64 Release)
```

## Project Files

| File | Description |
|------|-------------|
| `dllmain.cpp` | DLL entry point, OpenVR function exports |
| `openvr_wrapper.h/cpp` | IVRInput/IVRSystem wrapping, action interception, calibration |
| `CalibrationOverlay.h` | SteamVR notification overlay for calibration feedback |
| `treadmill_input.h/cpp` | OmniBridge integration, state management |
| `treadmill_config.json` | Configuration file |
