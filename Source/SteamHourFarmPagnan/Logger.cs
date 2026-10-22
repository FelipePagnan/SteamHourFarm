using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace SteamHourFarmPagnan
{
    /// <summary>
    /// Logger simples em arquivo, usado para diagnóstico local da aplicação.
    /// </summary>
    public static class Logger
    {
        private static readonly object LogLock = new object();
        private static readonly string LogPath = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            "steam_hour_farm.log");

        public static void LogInfo(string message) => Write("INFO", message);
        public static void LogWarning(string message) => Write("AVISO", message);
        public static void LogDebug(string message) => Write("DEBUG", message);

        public static void LogError(string message, Exception ex = null)
        {
            string details = ex != null ? $"{message}{Environment.NewLine}{ex}" : message;
            Write("ERRO", details);
        }

        private static void Write(string level, string message)
        {
            string line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [{level}] {message}{Environment.NewLine}";
            try
            {
                lock (LogLock)
                {
                    File.AppendAllText(LogPath, line, Encoding.UTF8);
                }
            }
            catch
            {
                // Ignora falhas de escrita de log (ex.: sem permissão no diretório).
            }
        }
    }
}
