# Inicializa o repositorio git deste projeto (novo, do zero, sem historico do projeto antigo)
# Execute dentro desta pasta: D:\Estudos\Projetos\steam-hour-farm\Steam Hour Farm - Pagnan

param(
    [string]$NomeAutor = "Pagnan",
    [string]$EmailAutor = "luiz.pagnan42@gmail.com"
)

$ErrorActionPreference = "Stop"

if (Test-Path ".git") {
    Write-Host "Ja existe um .git aqui. Nada a fazer." -ForegroundColor Yellow
    exit 0
}

Write-Host "Inicializando repositorio git..." -ForegroundColor Cyan
git init -q
git config user.name "$NomeAutor"
git config user.email "$EmailAutor"
git add -A
git commit -q -m "Initial commit: Steam Hour Farm - Pagnan"

Write-Host "OK Repositorio git criado com o primeiro commit." -ForegroundColor Green
Write-Host ""
Write-Host "Para publicar no GitHub, crie um repositorio seu e rode:" -ForegroundColor White
Write-Host '  git remote add origin https://github.com/SEU-USUARIO/SEU-REPOSITORIO.git' -ForegroundColor Cyan
Write-Host '  git push -u origin main' -ForegroundColor Cyan
