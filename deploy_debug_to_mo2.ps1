# deploy_debug_to_mo2.ps1
# Deploys DEBUG builds of TreadmillOpenVRWrapper and OmniBridge to MO2 mod folder
#
# Usage: .\deploy_debug_to_mo2.ps1
#
# This script:
# 1. Builds OmniBridge in Debug mode
# 2. Copies Debug DLLs to MO2 mod folder
# 3. Optionally copies to SteamVR driver folder

param(
    [string]$MO2ModPath = "C:\Modlists\MGON\mods\Treadmill OpenVR Wrapper\Root",
    [string]$SteamVRDriverPath = "C:\Program Files (x86)\Steam\steamapps\common\SteamVR\drivers\treadmill\bin\win64",
    [switch]$BuildWrapper,
    [switch]$DeployToSteamVR
)

$ErrorActionPreference = "Stop"
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path

Write-Host "========================================" -ForegroundColor Magenta
Write-Host "Deploy DEBUG to MO2 Mod Folder" -ForegroundColor Magenta
Write-Host "========================================" -ForegroundColor Magenta
Write-Host ""

# Source paths (DEBUG)
$wrapperSource = Join-Path $scriptDir "TreadmillOpenVRWrapper\x64\Debug\TreadmillOpenVRWrapper.dll"
$omniBridgeSource = Join-Path $scriptDir "OmniBridge\publish_debug\OmniBridge.dll"
$configSource = Join-Path $scriptDir "TreadmillOpenVRWrapper\treadmill_config.json"

# Build OmniBridge Debug
Write-Host "Building OmniBridge (Debug)..." -ForegroundColor Yellow
Push-Location (Join-Path $scriptDir "OmniBridge")
try {
    $buildOutput = dotnet publish -c Debug -o publish_debug 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "  [OK] OmniBridge Debug build successful" -ForegroundColor Green
    } else {
        Write-Host "  [ERROR] OmniBridge build failed:" -ForegroundColor Red
        Write-Host $buildOutput
        exit 1
    }
} finally {
    Pop-Location
}

# Optionally build Wrapper
if ($BuildWrapper) {
    Write-Host "Building Wrapper (Debug)..." -ForegroundColor Yellow
    $msbuild = "C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\amd64\MSBuild.exe"
    $vcxproj = Join-Path $scriptDir "TreadmillOpenVRWrapper\TreadmillOpenVRWrapper.vcxproj"
    
    if (Test-Path $msbuild) {
        & $msbuild $vcxproj /p:Configuration=Debug /p:Platform=x64 /t:Build /v:minimal
        if ($LASTEXITCODE -eq 0) {
            Write-Host "  [OK] Wrapper Debug build successful" -ForegroundColor Green
        } else {
            Write-Host "  [ERROR] Wrapper build failed" -ForegroundColor Red
            exit 1
        }
    } else {
        Write-Host "  [SKIP] MSBuild not found - build manually in Visual Studio" -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host "Deploying to: $MO2ModPath" -ForegroundColor Yellow
Write-Host ""

# Check if MO2 mod folder exists
if (-not (Test-Path $MO2ModPath)) {
    Write-Host "Creating mod folder: $MO2ModPath" -ForegroundColor Yellow
    New-Item -ItemType Directory -Path $MO2ModPath -Force | Out-Null
}

# Deploy Wrapper
Write-Host "Deploying Wrapper (DEBUG)..." -ForegroundColor White
if (Test-Path $wrapperSource) {
    $wrapperInfo = Get-Item $wrapperSource
    Copy-Item $wrapperSource -Destination "$MO2ModPath\openvr_api.dll" -Force
    Write-Host "  [OK] openvr_api.dll ($([math]::Round($wrapperInfo.Length/1KB)) KB, $($wrapperInfo.LastWriteTime))" -ForegroundColor Green
} else {
    Write-Host "  [SKIP] Wrapper not found at $wrapperSource" -ForegroundColor Yellow
    Write-Host "         Build with: -BuildWrapper flag or in Visual Studio (Debug x64)" -ForegroundColor Yellow
}

# Deploy OmniBridge
Write-Host "Deploying OmniBridge (DEBUG)..." -ForegroundColor White
if (Test-Path $omniBridgeSource) {
    $bridgeInfo = Get-Item $omniBridgeSource
    Copy-Item $omniBridgeSource -Destination "$MO2ModPath\OmniBridge.dll" -Force
    Write-Host "  [OK] OmniBridge.dll ($([math]::Round($bridgeInfo.Length/1KB)) KB, $($bridgeInfo.LastWriteTime))" -ForegroundColor Green
} else {
    Write-Host "  [ERROR] OmniBridge not found at $omniBridgeSource" -ForegroundColor Red
    exit 1
}

# Deploy Config
Write-Host "Deploying Config..." -ForegroundColor White
$configDest = "$MO2ModPath\treadmill_config.json"
if (Test-Path $configSource) {
    Copy-Item $configSource -Destination $configDest -Force
    Write-Host "  [OK] treadmill_config.json" -ForegroundColor Green
} else {
    Write-Host "  [SKIP] Config not found" -ForegroundColor Yellow
}

# Optionally deploy to SteamVR
if ($DeployToSteamVR) {
    Write-Host ""
    Write-Host "Deploying to SteamVR driver..." -ForegroundColor Yellow
    
    $steamVROmniBridge = Join-Path $SteamVRDriverPath "OmniBridge.dll"
    
    try {
        Copy-Item $omniBridgeSource -Destination $steamVROmniBridge -Force
        Write-Host "  [OK] OmniBridge.dll -> SteamVR driver" -ForegroundColor Green
    } catch {
        Write-Host "  [ERROR] Could not copy to SteamVR (is SteamVR running?)" -ForegroundColor Red
        Write-Host "         Stop SteamVR and try again, or copy manually:" -ForegroundColor Yellow
        Write-Host "         Copy-Item `"$omniBridgeSource`" -Destination `"$steamVROmniBridge`" -Force" -ForegroundColor Gray
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Magenta
Write-Host "DEBUG Deployment complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Magenta
Write-Host ""
Write-Host "Current mod contents:" -ForegroundColor Yellow
Get-ChildItem $MO2ModPath | Format-Table Name, @{N='Size (KB)';E={[math]::Round($_.Length/1KB)}}, LastWriteTime -AutoSize

Write-Host ""
Write-Host "Log files will be at:" -ForegroundColor Cyan
Write-Host "  - `$env:TEMP\treadmill_wrapper.log" -ForegroundColor Gray
Write-Host "  - `$env:TEMP\OmniBridge.log" -ForegroundColor Gray
Write-Host ""
Write-Host "Quick log check:" -ForegroundColor Cyan
Write-Host "  Get-Content `"`$env:TEMP\treadmill_wrapper.log`" | Select-Object -Last 30" -ForegroundColor Gray
Write-Host "  Get-Content `"`$env:TEMP\OmniBridge.log`" | Select-Object -Last 30" -ForegroundColor Gray
