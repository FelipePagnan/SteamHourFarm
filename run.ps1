# Script para compilar e executar o Steam Hour Farm - Pagnan
# Uso: .\run.ps1

param(
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Steam Hour Farm - Pagnan - Build & Run" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$msbuild = $null
$vsPaths = @(
    "${env:ProgramFiles}\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe",
    "${env:ProgramFiles}\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe",
    "${env:ProgramFiles}\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe",
    "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe",
    "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe",
    "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\MSBuild.exe",
    "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe"
)

foreach ($path in $vsPaths) {
    if (Test-Path $path) {
        $msbuild = $path
        break
    }
}

if (-not $msbuild) {
    Write-Host "ERRO: MSBuild nao encontrado!" -ForegroundColor Red
    Write-Host "Instale o Visual Studio ou Visual Studio Build Tools." -ForegroundColor Yellow
    exit 1
}

Write-Host "MSBuild encontrado: $msbuild" -ForegroundColor Gray
Write-Host ""

# 1. Compilar IdleWorker (se necessario)
$idleWorkerExe = "IdleWorker Source\IdleWorker\bin\$Configuration\IdleWorker.exe"
if (-not (Test-Path $idleWorkerExe)) {
    Write-Host "[1/3] Compilando IdleWorker..." -ForegroundColor Yellow
    $idleWorkerProj = "IdleWorker Source\IdleWorker\IdleWorker.csproj"

    if (-not (Test-Path $idleWorkerProj)) {
        Write-Host "AVISO: IdleWorker.csproj nao encontrado. Pulando." -ForegroundColor Yellow
    } else {
        & $msbuild $idleWorkerProj /p:Configuration=$Configuration /p:Platform=AnyCPU /v:minimal /nologo
        if ($LASTEXITCODE -ne 0) {
            Write-Host "ERRO: Falha ao compilar IdleWorker!" -ForegroundColor Red
            exit 1
        }
        Write-Host "OK IdleWorker compilado" -ForegroundColor Green

        $idleWorkerOutputDir = "IdleWorker Source\IdleWorker\bin\$Configuration"

        $dllSource = "Dependencies\steam_api64.dll"
        $dllTarget = "$idleWorkerOutputDir\steam_api64.dll"
        if (Test-Path $dllSource) {
            Copy-Item $dllSource -Destination $dllTarget -Force
            Write-Host "OK steam_api64.dll copiado para IdleWorker" -ForegroundColor Gray
        }

        $steamworksSource = "Dependencies\Steamworks.NET.dll"
        $steamworksTarget = "$idleWorkerOutputDir\Steamworks.NET.dll"
        if (Test-Path $steamworksSource) {
            Copy-Item $steamworksSource -Destination $steamworksTarget -Force
            Write-Host "OK Steamworks.NET.dll copiado para IdleWorker" -ForegroundColor Gray
        } else {
            Write-Host "AVISO: Steamworks.NET.dll nao encontrado em Dependencies!" -ForegroundColor Yellow
        }
    }
    Write-Host ""
}

# 2. Compilar SteamHourFarmPagnan
Write-Host "[2/3] Compilando SteamHourFarmPagnan..." -ForegroundColor Yellow
$mainProj = "Source\SteamHourFarmPagnan\SteamHourFarmPagnan.csproj"

if (-not (Test-Path $mainProj)) {
    Write-Host "ERRO: SteamHourFarmPagnan.csproj nao encontrado!" -ForegroundColor Red
    exit 1
}

& $msbuild $mainProj /p:Configuration=$Configuration /p:Platform=AnyCPU /v:minimal /nologo
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERRO: Falha ao compilar SteamHourFarmPagnan!" -ForegroundColor Red
    exit 1
}
Write-Host "OK SteamHourFarmPagnan compilado" -ForegroundColor Green
Write-Host ""

# 3. Copiar arquivos necessarios
Write-Host "[3/3] Preparando arquivos..." -ForegroundColor Yellow
$targetDir = "Source\SteamHourFarmPagnan\bin\$Configuration"
$idleWorkerDir = "IdleWorker Source\IdleWorker\bin\$Configuration"

if (-not (Test-Path $targetDir)) {
    New-Item -ItemType Directory -Path $targetDir -Force | Out-Null
}

$targetExe = "$targetDir\IdleWorker.exe"
if (Test-Path $idleWorkerExe) {
    Copy-Item $idleWorkerExe -Destination $targetExe -Force
    Write-Host "OK IdleWorker.exe copiado" -ForegroundColor Green
} else {
    Write-Host "AVISO: IdleWorker.exe nao encontrado. A aplicacao pode nao funcionar." -ForegroundColor Yellow
}

$dllSource = "$idleWorkerDir\steam_api64.dll"
if (-not (Test-Path $dllSource)) {
    $dllSource = "Dependencies\steam_api64.dll"
}
$dllTarget = "$targetDir\steam_api64.dll"
if (Test-Path $dllSource) {
    Copy-Item $dllSource -Destination $dllTarget -Force
    Write-Host "OK steam_api64.dll copiado" -ForegroundColor Green
} else {
    Write-Host "AVISO: steam_api64.dll nao encontrado!" -ForegroundColor Yellow
}

$steamworksSource = "$idleWorkerDir\Steamworks.NET.dll"
if (-not (Test-Path $steamworksSource)) {
    $steamworksSource = "Dependencies\Steamworks.NET.dll"
}
$steamworksTarget = "$targetDir\Steamworks.NET.dll"
if (Test-Path $steamworksSource) {
    Copy-Item $steamworksSource -Destination $steamworksTarget -Force
    Write-Host "OK Steamworks.NET.dll copiado" -ForegroundColor Green
} else {
    Write-Host "ERRO: Steamworks.NET.dll nao encontrado! O IdleWorker nao funcionara." -ForegroundColor Red
}

$otherDlls = Get-ChildItem -Path $idleWorkerDir -Filter "*.dll" -ErrorAction SilentlyContinue
foreach ($dll in $otherDlls) {
    $targetDll = "$targetDir\$($dll.Name)"
    if (-not (Test-Path $targetDll)) {
        Copy-Item $dll.FullName -Destination $targetDll -Force
        Write-Host "OK $($dll.Name) copiado" -ForegroundColor Gray
    }
}

Write-Host ""

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Executando Steam Hour Farm - Pagnan..." -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$exePath = "$targetDir\SteamHourFarmPagnan.exe"
if (Test-Path $exePath) {
    & $exePath
} else {
    Write-Host "ERRO: Executavel nao encontrado em: $exePath" -ForegroundColor Red
    exit 1
}
