# Script de Compilacao - Steam Hour Farm - Pagnan
# Execute este script no diretorio raiz do projeto

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Compilando Steam Hour Farm - Pagnan" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$msbuild = Get-Command msbuild -ErrorAction SilentlyContinue
if (-not $msbuild) {
    $vsPath = "${env:ProgramFiles}\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"
    if (Test-Path $vsPath) {
        $msbuild = $vsPath
    } else {
        $vsPath = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe"
        if (Test-Path $vsPath) {
            $msbuild = $vsPath
        } else {
            Write-Host "ERRO: MSBuild nao encontrado!" -ForegroundColor Red
            Write-Host "Instale o Visual Studio ou Visual Studio Build Tools." -ForegroundColor Yellow
            exit 1
        }
    }
}

Write-Host "[1/3] Compilando IdleWorker..." -ForegroundColor Yellow
$idleWorkerPath = "IdleWorker Source\IdleWorker\IdleWorker.csproj"
if (-not (Test-Path $idleWorkerPath)) {
    Write-Host "ERRO: IdleWorker.csproj nao encontrado!" -ForegroundColor Red
    exit 1
}
& $msbuild $idleWorkerPath /p:Configuration=Release /p:Platform=AnyCPU /v:minimal /nologo
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERRO: Falha ao compilar IdleWorker!" -ForegroundColor Red
    exit 1
}
Write-Host "OK IdleWorker compilado com sucesso" -ForegroundColor Green
Write-Host ""

Write-Host "[2/3] Compilando SteamHourFarmPagnan..." -ForegroundColor Yellow
$mainPath = "Source\SteamHourFarmPagnan\SteamHourFarmPagnan.csproj"
if (-not (Test-Path $mainPath)) {
    Write-Host "ERRO: SteamHourFarmPagnan.csproj nao encontrado!" -ForegroundColor Red
    exit 1
}
& $msbuild $mainPath /p:Configuration=Release /p:Platform=AnyCPU /v:minimal /nologo
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERRO: Falha ao compilar SteamHourFarmPagnan!" -ForegroundColor Red
    exit 1
}
Write-Host "OK SteamHourFarmPagnan compilado com sucesso" -ForegroundColor Green
Write-Host ""

Write-Host "[3/3] Copiando IdleWorker.exe..." -ForegroundColor Yellow
$sourceExe = "IdleWorker Source\IdleWorker\bin\Release\IdleWorker.exe"
$targetDir = "Source\SteamHourFarmPagnan\bin\Release"
$targetExe = "$targetDir\IdleWorker.exe"

if (Test-Path $sourceExe) {
    if (-not (Test-Path $targetDir)) {
        New-Item -ItemType Directory -Path $targetDir -Force | Out-Null
    }
    Copy-Item $sourceExe -Destination $targetExe -Force
    Write-Host "OK IdleWorker.exe copiado com sucesso" -ForegroundColor Green
} else {
    Write-Host "AVISO: IdleWorker.exe nao encontrado em $sourceExe" -ForegroundColor Yellow
}
Write-Host ""

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Compilacao Concluida!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Executavel gerado em:" -ForegroundColor White
Write-Host "  $targetDir\SteamHourFarmPagnan.exe" -ForegroundColor Cyan
Write-Host ""
