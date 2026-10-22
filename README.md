# Steam Hour Farm - Pagnan

Aplicação para gerenciar jogos em idle na Steam, permitindo acumular horas de jogo automaticamente — com interface escura e visual "gamer".

## Funcionalidades

- Interface com tema escuro e acentos neon
- Gerenciamento de múltiplos jogos simultaneamente
- Monitoramento em tempo real do tempo de execução de cada jogo
- Verificação automática de que a Steam está rodando
- Persistência dos IDs digitados (não precisa redigitar a cada abertura)
- Logs locais para diagnóstico

## Requisitos

- Windows 10/11
- .NET Framework 4.8 ou superior
- Steam instalada e rodando

## Como Compilar

### Opção 1: Script automático (recomendado)

```powershell
.\run.ps1
```

### Opção 2: Scripts individuais

```powershell
.\compilar.ps1
.\criar-release.ps1
```

### Opção 3: Visual Studio

1. Abra `Source\SteamHourFarmPagnan.sln`
2. Build → Build Solution
3. Execute `.\criar-release.ps1` para gerar a pasta de release

## Como Usar

1. Certifique-se de que a Steam está rodando
2. Execute `SteamHourFarmPagnan.exe`
3. Digite os AppIDs dos jogos (separados por vírgula, espaço ou quebra de linha)
4. Clique em **Abrir**
5. Acompanhe o status em tempo real na lista de jogos ativos

### Exemplo

```
Digite: 730, 440, 570
```

## Arquitetura

- **SteamHourFarmPagnan.exe** — interface principal em C#/WinForms
- **IdleWorker.exe** — processo auxiliar que mantém a sessão da Steamworks API ativa para cada AppID

## Dependências de terceiros

- `steam_api64.dll` — Steamworks SDK (Valve Corporation)
- `Steamworks.NET.dll` — wrapper .NET open-source (licença MIT) — https://github.com/rlabrecque/Steamworks.NET

## Logs e Diagnóstico

- `steam_hour_farm.log` — log da aplicação principal
- `idleworker-error.log` — log de erros do IdleWorker

## Autor

Pagnan — 2026

## Licença

Código próprio deste projeto distribuído sob licença MIT. As dependências de terceiros listadas acima mantêm suas licenças originais.
"# SteamHourFarm" 
