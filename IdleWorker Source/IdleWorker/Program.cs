using System;
using System.IO;
using System.Windows.Forms;
using Steamworks;

namespace IdleWorker
{
    /// <summary>
    /// Processo auxiliar e independente: recebe um AppID via argumento de linha de
    /// comando, inicializa a Steamworks API para esse AppID e mantém o processo
    /// vivo (com callbacks periódicos) enquanto a janela estiver aberta.
    /// </summary>
    internal static class EntryPoint
    {
        private const string ErrorLogFile = "idleworker-error.log";

        [STAThread]
        private static void Main(string[] args)
        {
            if (!TryParseAppId(args, out long targetAppId))
            {
                return;
            }

            Environment.SetEnvironmentVariable("SteamAppId", targetAppId.ToString());

            if (!TryInitializeSteam(targetAppId))
            {
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormIdleWorker(targetAppId));
        }

        private static bool TryParseAppId(string[] args, out long appId)
        {
            appId = 0;
            return args.Length > 0 && long.TryParse(args[0], out appId);
        }

        private static bool TryInitializeSteam(long appId)
        {
            try
            {
                return SteamAPI.Init();
            }
            catch (Exception ex)
            {
                LogFailure(appId, ex);
                return false;
            }
        }

        internal static void LogFailure(long appId, Exception ex)
        {
            try
            {
                File.AppendAllText(ErrorLogFile,
                    $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | AppId {appId} | {ex}{Environment.NewLine}");
            }
            catch
            {
                // Sem permissão de escrita no diretório atual - ignora silenciosamente.
            }
        }
    }
}
