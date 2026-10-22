using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace SteamHourFarmPagnan
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Logger.LogInfo("=== Steam Hour Farm - Pagnan iniciado ===");
            Logger.LogInfo($"Versão: {Assembly.GetExecutingAssembly().GetName().Version}");
            Logger.LogInfo($"Diretório: {Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());

            Logger.LogInfo("=== Steam Hour Farm - Pagnan finalizado ===");
        }
    }
}
