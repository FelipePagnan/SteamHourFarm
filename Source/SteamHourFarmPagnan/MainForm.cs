using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Security.Principal;
using System.Windows.Forms;

namespace SteamHourFarmPagnan
{
    public class RunningGame
    {
        public Process Process { get; set; }
        public long AppId { get; set; }
        public DateTime StartTime { get; set; }

        public TimeSpan Elapsed => DateTime.Now - StartTime;

        public string ToDisplayText()
        {
            TimeSpan t = Elapsed;
            return $"AppID {AppId}  •  {t.Hours:D2}:{t.Minutes:D2}:{t.Seconds:D2}";
        }
    }

    public partial class MainForm : Form
    {
        private readonly List<RunningGame> runningGames = new List<RunningGame>();
        private string idleWorkerPath = "IdleWorker.exe";

        public MainForm()
        {
            InitializeComponent();

            LoadIcon();
            LoadSavedGameIds();

            Logger.LogInfo("Steam Hour Farm - Pagnan iniciado");
            Logger.LogInfo($"Diretório do executável: {Path.GetDirectoryName(Application.ExecutablePath)}");

            ResolveIdleWorkerPath();
            RefreshStatus();
        }

        private void LoadIcon()
        {
            try
            {
                string iconPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "logo.ico");
                if (File.Exists(iconPath))
                {
                    Icon = new System.Drawing.Icon(iconPath);
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning($"Não foi possível carregar o ícone: {ex.Message}");
            }
        }

        private void ResolveIdleWorkerPath()
        {
            string currentDir = Directory.GetCurrentDirectory();
            string exeDir = Path.GetDirectoryName(Application.ExecutablePath);

            string[] candidatePaths =
            {
                Path.Combine(currentDir, idleWorkerPath),
                Path.Combine(exeDir, idleWorkerPath),
                Path.Combine(exeDir, "..", "..", "..", "IdleWorker Source", "IdleWorker", "bin", "Release", idleWorkerPath),
                Path.Combine(exeDir, "..", "..", "..", "IdleWorker Source", "IdleWorker", "bin", "Debug", idleWorkerPath),
            };

            string found = candidatePaths.FirstOrDefault(File.Exists);
            if (found != null)
            {
                idleWorkerPath = found;
                Logger.LogInfo($"IdleWorker.exe encontrado em: {idleWorkerPath}");
                return;
            }

            Logger.LogWarning("IdleWorker.exe não encontrado em nenhum caminho pesquisado");
            MessageBox.Show(
                "IdleWorker.exe não encontrado!\n\n" +
                "Certifique-se de que o IdleWorker.exe está no mesmo diretório desta aplicação " +
                "ou compile o projeto IdleWorker primeiro.",
                "Aviso",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        private static bool IsSteamRunning()
        {
            try
            {
                return Process.GetProcessesByName("steam").Length > 0;
            }
            catch (Exception ex)
            {
                Logger.LogError("Erro ao verificar se a Steam está rodando", ex);
                return false;
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            SaveGameIds();

            string input = txtGameIds.Text.Trim();
            if (string.IsNullOrWhiteSpace(input))
            {
                MessageBox.Show("Digite pelo menos um ID de jogo.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!IsSteamRunning())
            {
                var choice = MessageBox.Show(
                    "A Steam não parece estar em execução.\n\nContinuar mesmo assim?",
                    "Steam não detectada",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (choice == DialogResult.No)
                {
                    return;
                }
            }

            if (!File.Exists(idleWorkerPath))
            {
                MessageBox.Show(
                    $"IdleWorker.exe não encontrado em:\n{idleWorkerPath}",
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            string[] separators = { ",", ";", " ", "\r\n", "\n", "\r" };
            string[] gameIds = input.Split(separators, StringSplitOptions.RemoveEmptyEntries)
                .Select(id => id.Trim())
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .ToArray();

            if (gameIds.Length == 0)
            {
                MessageBox.Show("Nenhum ID de jogo válido encontrado.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            StopAllGames();

            int success = 0;
            int failed = 0;
            var errors = new List<string>();

            foreach (string rawId in gameIds)
            {
                if (!long.TryParse(rawId, out long appId))
                {
                    errors.Add($"ID inválido: {rawId}");
                    failed++;
                    continue;
                }

                if (TryLaunchGame(appId, out string error))
                {
                    success++;
                }
                else
                {
                    errors.Add(error);
                    failed++;
                }
            }

            RefreshStatus();
            ShowLaunchSummary(success, failed, errors);
        }

        private bool TryLaunchGame(long appId, out string error)
        {
            error = null;
            string workingDir = Path.GetDirectoryName(idleWorkerPath);

            string steamApiDll = Path.Combine(workingDir, "steam_api64.dll");
            string steamworksDll = Path.Combine(workingDir, "Steamworks.NET.dll");

            if (!File.Exists(steamApiDll))
            {
                error = $"steam_api64.dll não encontrada em: {steamApiDll}";
                return false;
            }

            if (!File.Exists(steamworksDll))
            {
                error = $"Steamworks.NET.dll não encontrada em: {steamworksDll}";
                return false;
            }

            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = idleWorkerPath,
                    Arguments = appId.ToString(),
                    WorkingDirectory = workingDir,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };

                Process process = Process.Start(startInfo);
                if (process == null)
                {
                    error = $"Não foi possível iniciar o processo para o AppID {appId}.";
                    return false;
                }

                System.Threading.Thread.Sleep(800);

                if (process.HasExited)
                {
                    error = $"AppID {appId} falhou ao iniciar (código {process.ExitCode}). " +
                            "Verifique se as DLLs da Steam estão presentes e se o .NET Framework 4.8 está instalado.";
                    process.Dispose();
                    return false;
                }

                runningGames.Add(new RunningGame { Process = process, AppId = appId, StartTime = DateTime.Now });
                Logger.LogInfo($"Processo iniciado - PID {process.Id}, AppID {appId}");
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Exceção ao iniciar AppID {appId}", ex);
                error = $"Erro ao iniciar AppID {appId}: {ex.Message}";
                return false;
            }
        }

        private static void ShowLaunchSummary(int success, int failed, List<string> errors)
        {
            if (success > 0 && failed == 0)
            {
                MessageBox.Show($"Jogos iniciados com sucesso! ({success})", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (success > 0 && failed > 0)
            {
                string msg = $"Iniciados: {success}  |  Falhas: {failed}\n\n" + string.Join("\n\n", errors);
                MessageBox.Show(msg, "Resultado parcial", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (failed > 0)
            {
                string msg = "Nenhum jogo foi iniciado.\n\n" + string.Join("\n\n", errors);
                MessageBox.Show(msg, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            StopAllGames();
            RefreshStatus();
            MessageBox.Show("Todos os jogos foram parados.", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void StopAllGames()
        {
            foreach (var game in runningGames)
            {
                try
                {
                    if (!game.Process.HasExited)
                    {
                        game.Process.Kill();
                        game.Process.WaitForExit(1000);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Erro ao parar processo {game.Process.Id}", ex);
                }
                finally
                {
                    game.Process.Dispose();
                }
            }
            runningGames.Clear();

            // Garante que nenhum IdleWorker.exe órfão deste usuário continue rodando.
            try
            {
                string currentUser = WindowsIdentity.GetCurrent().Name;
                foreach (var process in Process.GetProcessesByName("IdleWorker"))
                {
                    try
                    {
                        if (BelongsToUser(process.Id, currentUser))
                        {
                            process.Kill();
                            process.WaitForExit(1000);
                        }
                    }
                    catch
                    {
                        // Ignora processos que já terminaram ou sem permissão de acesso.
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao limpar processos IdleWorker: {ex.Message}");
            }

            lstRunning.Items.Clear();
        }

        private static bool BelongsToUser(int processId, string username)
        {
            var searcher = new ManagementObjectSearcher($"SELECT * FROM Win32_Process WHERE ProcessID = {processId}");
            foreach (ManagementObject obj in searcher.Get())
            {
                string[] argList = { string.Empty, string.Empty };
                int result = Convert.ToInt32(obj.InvokeMethod("GetOwner", argList));
                if (result == 0 && $"{argList[1]}\\{argList[0]}" == username)
                {
                    return true;
                }
            }
            return false;
        }

        private void RefreshStatus()
        {
            int running = runningGames.Count(g => !g.Process.HasExited);
            lblStatus.Text = $"Jogos ativos: {running}";
            btnStop.Enabled = running > 0;
        }

        private void timerStatus_Tick(object sender, EventArgs e)
        {
            var finished = runningGames.Where(g => g.Process.HasExited).ToList();
            foreach (var game in finished)
            {
                Logger.LogWarning($"AppID {game.AppId} (PID {game.Process.Id}) terminou após {game.Elapsed}");
            }
            runningGames.RemoveAll(g => g.Process.HasExited);

            var active = runningGames.Where(g => !g.Process.HasExited).ToList();

            lstRunning.BeginUpdate();
            try
            {
                if (lstRunning.Items.Count != active.Count)
                {
                    lstRunning.Items.Clear();
                    foreach (var game in active)
                    {
                        lstRunning.Items.Add(game.ToDisplayText());
                    }
                }
                else
                {
                    for (int i = 0; i < active.Count; i++)
                    {
                        string text = active[i].ToDisplayText();
                        if (lstRunning.Items[i].ToString() != text)
                        {
                            lstRunning.Items[i] = text;
                        }
                    }
                }
            }
            finally
            {
                lstRunning.EndUpdate();
            }

            RefreshStatus();
        }

        private void LoadSavedGameIds()
        {
            try
            {
                string saved = Properties.Settings.Default.LastGameIds;
                if (!string.IsNullOrEmpty(saved))
                {
                    txtGameIds.Text = saved;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Erro ao carregar IDs salvos", ex);
            }
        }

        private void SaveGameIds()
        {
            try
            {
                Properties.Settings.Default.LastGameIds = txtGameIds.Text.Trim();
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                Logger.LogError("Erro ao salvar IDs", ex);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            SaveGameIds();

            if (runningGames.Any(g => !g.Process.HasExited))
            {
                var choice = MessageBox.Show(
                    "Há jogos rodando. Deseja parar todos antes de fechar?",
                    "Confirmar",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (choice == DialogResult.Yes)
                {
                    StopAllGames();
                }
                else if (choice == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
            }

            base.OnFormClosing(e);
        }
    }
}
