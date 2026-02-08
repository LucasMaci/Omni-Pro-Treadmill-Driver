# deploy_steamvr_driver.ps1
# Deploys the SteamVR treadmill driver DLLs (driver_treadmill.dll + OmniBridge.dll)
#
# Usage:
#   .\deploy_steamvr_driver.ps1                  # Release deploy
#   .\deploy_steamvr_driver.ps1 -Debug           # Debug deploy (builds OmniBridge Debug automatically)
#   .\deploy_steamvr_driver.ps1 -Debug -Build    # Debug deploy + rebuild driver DLL via MSBuild
#
# Prerequisites:
#   - SteamVR must NOT be running
#   - May require Administrator privileges for Program Files
#   - Release: Build TreadmillSteamVR (Release x64) in Visual Studio + dotnet publish OmniBridge
#   - Debug:   Build TreadmillSteamVR (Debug x64) in Visual Studio (OmniBridge is auto-built)

param(
    [string]$DriverPath = "C:\Program Files (x86)\Steam\steamapps\common\SteamVR\drivers\treadmill\bin\win64",
    [switch]$Debug,
    [switch]$Build
)

$ErrorActionPreference = "Stop"
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path

$config = if ($Debug) { "DEBUG" } else { "RELEASE" }
$color  = if ($Debug) { "Magenta" } else { "Cyan" }

Write-Host "========================================" -ForegroundColor $color
Write-Host "Deploy SteamVR Driver ($config)" -ForegroundColor $color
Write-Host "========================================" -ForegroundColor $color
Write-Host ""
Write-Host "Target: $DriverPath" -ForegroundColor Yellow
Write-Host ""

# --- Resolve source paths ---
if ($Debug) {
    $driverSource    = Join-Path $scriptDir "x64\Debug\TreadmillSteamVR.dll"
    $omniBridgeSource = Join-Path $scriptDir "OmniBridge\publish_debug\OmniBridge.dll"
    $msBuildConfig   = "Debug"
    $dotnetConfig    = "Debug"
    $publishDir      = "publish_debug"
} else {
    $driverSource    = Join-Path $scriptDir "x64\Release\driver_treadmill.dll"
    $omniBridgeSource = Join-Path $scriptDir "OmniBridge\publish\OmniBridge.dll"
    $msBuildConfig   = "Release"
    $dotnetConfig    = "Release"
    $publishDir      = "publish"
}

# --- Optional: Build driver DLL via MSBuild ---
if ($Build) {
    Write-Host "Building TreadmillSteamVR ($msBuildConfig x64)..." -ForegroundColor Yellow
    $vcxproj = Join-Path $scriptDir "TreadmillSteamVR.vcxproj"
    $msbuildPaths = @(
        "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\amd64\MSBuild.exe",
        "C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\amd64\MSBuild.exe",
        "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\amd64\MSBuild.exe"
    )
    $msbuild = $msbuildPaths | Where-Object { Test-Path $_ } | Select-Object -First 1
    if ($msbuild) {
        & $msbuild $vcxproj /p:Configuration=$msBuildConfig /p:Platform=x64 /t:Build /v:minimal
        if ($LASTEXITCODE -ne 0) { Write-Host "  [ERROR] MSBuild failed" -ForegroundColor Red; exit 1 }
        Write-Host "  [OK] Driver build successful" -ForegroundColor Green
    } else {
        Write-Host "  [SKIP] MSBuild not found - build manually in Visual Studio" -ForegroundColor Yellow
    }
    Write-Host ""
}

# --- Build OmniBridge ---
Write-Host "Building OmniBridge ($dotnetConfig)..." -ForegroundColor Yellow
Push-Location (Join-Path $scriptDir "OmniBridge")
try {
    $buildOutput = dotnet publish -c $dotnetConfig -o $publishDir 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "  [OK] OmniBridge $dotnetConfig build successful" -ForegroundColor Green
    } else {
        Write-Host "  [ERROR] OmniBridge build failed:" -ForegroundColor Red
        Write-Host $buildOutput
        exit 1
    }
} finally {
    Pop-Location
}
Write-Host ""

# --- Validate sources ---
$missing = $false
Write-Host "Checking build outputs..." -ForegroundColor White
if (Test-Path $driverSource) {
    $info = Get-Item $driverSource
    Write-Host "  [OK] $($info.Name) ($([math]::Round($info.Length/1KB)) KB)" -ForegroundColor Green
} else {
    Write-Host "  [MISSING] $driverSource" -ForegroundColor Red
    Write-Host "            Build TreadmillSteamVR ($msBuildConfig x64) in Visual Studio or use -Build flag" -ForegroundColor Yellow
    $missing = $true
}
if (Test-Path $omniBridgeSource) {
    $info = Get-Item $omniBridgeSource
    Write-Host "  [OK] $($info.Name) ($([math]::Round($info.Length/1KB)) KB)" -ForegroundColor Green
} else {
    Write-Host "  [MISSING] $omniBridgeSource" -ForegroundColor Red
    $missing = $true
}
if ($missing) { Write-Host ""; Write-Host "Aborted - fix missing files first." -ForegroundColor Red; exit 1 }
Write-Host ""

# --- Check target directory ---
if (-not (Test-Path $DriverPath)) {
    Write-Host "[ERROR] Target directory not found: $DriverPath" -ForegroundColor Red
    exit 1
}

# --- Show current state before deploy ---
Write-Host "Current files in target:" -ForegroundColor White
Get-ChildItem $DriverPath -Filter "*.dll" | ForEach-Object { Write-Host "  $($_.Name) ($([math]::Round($_.Length/1KB)) KB, $($_.LastWriteTime))" -ForegroundColor Gray }
Write-Host ""

# --- Deploy ---
Write-Host "Deploying..." -ForegroundColor White

try {
    # driver_treadmill.dll (Debug has different source name, always deploy as driver_treadmill.dll)
    $driverDest = Join-Path $DriverPath "driver_treadmill.dll"
    Copy-Item $driverSource -Destination $driverDest -Force
    $info = Get-Item $driverDest
    Write-Host "  [OK] driver_treadmill.dll ($([math]::Round($info.Length/1KB)) KB)" -ForegroundColor Green

    # OmniBridge.dll
    $bridgeDest = Join-Path $DriverPath "OmniBridge.dll"
    Copy-Item $omniBridgeSource -Destination $bridgeDest -Force
    $info = Get-Item $bridgeDest
    Write-Host "  [OK] OmniBridge.dll ($([math]::Round($info.Length/1KB)) KB)" -ForegroundColor Green
} catch {
    Write-Host ""
    Write-Host "[ERROR] Copy failed!" -ForegroundColor Red
    Write-Host "  Make sure:" -ForegroundColor Yellow
    Write-Host "    1. SteamVR is NOT running" -ForegroundColor Yellow
    Write-Host "    2. You are running as Administrator" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Error: $_" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "========================================" -ForegroundColor $color
Write-Host "$config deployment complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor $color
Write-Host ""
Write-Host "Deployed files:" -ForegroundColor Yellow
Get-ChildItem $DriverPath -Filter "*.dll" | ForEach-Object { Write-Host "  $($_.Name) ($([math]::Round($_.Length/1KB)) KB, $($_.LastWriteTime))" -ForegroundColor White }
Write-Host ""
Write-Host "Log files after SteamVR start:" -ForegroundColor Cyan
Write-Host "  SteamVR driver:  C:\Users\$env:USERNAME\AppData\Local\openvr\log\vrserver.txt" -ForegroundColor Gray
Write-Host "  OmniBridge:      $env:TEMP\OmniBridge.log" -ForegroundColor Gray
