# PowerShell script to build Proxmox VE Windows GUI and compile the Inno Setup installer
$ErrorActionPreference = "Stop"

Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "   Proxmox VE Windows GUI - Installer Builder   " -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan

# 1. Check for dotnet CLI
Write-Host "`n[1/3] building C# application in Release mode..." -ForegroundColor Yellow
if (Get-Command "dotnet" -ErrorAction SilentlyContinue) {
    try {
        dotnet build -c Release
        Write-Host "Successfully compiled the application." -ForegroundColor Green
    } catch {
        Write-Host "Error: dotnet build failed. Please fix any compilation errors first." -ForegroundColor Red
        Exit 1
    }
} else {
    Write-Host "Error: .NET SDK ('dotnet') not found. Please install the .NET SDK." -ForegroundColor Red
    Exit 1
}

# 2. Find Inno Setup Compiler (ISCC.exe)
Write-Host "`n[2/3] Locating Inno Setup Compiler (ISCC.exe)..." -ForegroundColor Yellow
$isccPath = $null

# Check common paths
$commonPaths = @(
    "C:\Program Files (x86)\Inno Setup 6\ISCC.exe",
    "C:\Program Files\Inno Setup 6\ISCC.exe",
    "C:\Program Files (x86)\Inno Setup 5\ISCC.exe",
    "C:\Program Files\Inno Setup 5\ISCC.exe"
)

foreach ($path in $commonPaths) {
    if (Test-Path $path) {
        $isccPath = $path
        break
    }
}

# If not found in common paths, check PATH environment variable
if (-not $isccPath) {
    $command = Get-Command "ISCC.exe" -ErrorAction SilentlyContinue
    if ($command) {
        $isccPath = $command.Source
    }
}

if (-not $isccPath) {
    Write-Host "Error: Inno Setup Compiler (ISCC.exe) not found." -ForegroundColor Red
    Write-Host "Please download and install Inno Setup 6 from: https://jrsoftware.org/isdl.php" -ForegroundColor Cyan
    Write-Host "Ensure it is installed in standard directories or added to your system PATH." -ForegroundColor Cyan
    Exit 1
}

Write-Host "Found ISCC.exe at: $isccPath" -ForegroundColor Green

# 3. Compile the Installer
Write-Host "`n[3/3] Compiling installer wizard..." -ForegroundColor Yellow
$setupScript = Join-Path $PSScriptRoot "setup.iss"

if (-not (Test-Path $setupScript)) {
    Write-Host "Error: $setupScript not found!" -ForegroundColor Red
    Exit 1
}

try {
    & $isccPath $setupScript
    Write-Host "`n==========================================" -ForegroundColor Green
    Write-Host "Success! Installer generated successfully." -ForegroundColor Green
    Write-Host "Installer package: installer_output\ProxmoxVEGui-Setup.exe" -ForegroundColor Green
    Write-Host "==========================================" -ForegroundColor Green
} catch {
    Write-Host "Error compiling setup script: $_" -ForegroundColor Red
    Exit 1
}
