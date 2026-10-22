@echo off
REM Abre o app ja compilado, sem passar pelo PowerShell.
REM Use isso (ou um atalho na area de trabalho apontando para este arquivo) no dia a dia.
REM Se o .exe ainda nao existir, compile uma vez com run.ps1 ou compilar.ps1.

set EXE=%~dp0Source\SteamHourFarmPagnan\bin\Release\SteamHourFarmPagnan.exe

if not exist "%EXE%" (
    echo O executavel ainda nao foi compilado.
    echo Rode primeiro: run.ps1 ou compilar.ps1
    pause
    exit /b 1
)

start "" "%EXE%"
