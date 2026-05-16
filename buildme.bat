@echo off
REM MicMute 2 - Build Script
REM Requires .NET Framework 4.0+ installed (C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe)

set CSC=C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe
set SRC=%~dp0micmute\MicMute.cs
set OUT=%~dp0micmute\MicMute2.exe

if not exist "%CSC%" (
    echo ERROR: .NET Framework 4.0 not found.
    echo Expected at: %CSC%
    echo Please install .NET Framework 4.0 or higher.
    pause
    exit /b 1
)

if not exist "%SRC%" (
    echo ERROR: MicMute.cs not found at %SRC%
    pause
    exit /b 1
)

echo Building MicMute2.exe...
"%CSC%" /out:"%OUT%" /target:winexe /platform:anycpu /optimize /nologo "%SRC%"

if exist "%OUT%" (
    echo SUCCESS: MicMute2.exe built successfully.
    echo %OUT%
) else (
    echo FAILED: Build did not produce MicMute2.exe
    pause
    exit /b 1
)

pause