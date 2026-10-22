using System;
using System.Windows.Forms;
using Steamworks;

namespace IdleWorker
{
    /// <summary>
    /// Janela oculta que mantém a sessão da Steamworks API ativa para um único
    /// AppID, chamando RunCallbacks em intervalos regulares.
    /// </summary>
    public partial class FormIdleWorker : Form
    {
        private readonly long appId;
        private readonly Timer heartbeat;

        public FormIdleWorker(long appId)
        {
            this.appId = appId;

            InitializeComponent();

            Load += OnFormLoad;
            FormClosing += OnFormClosing;

            heartbeat = new Timer { Interval = 1000 };
            heartbeat.Tick += OnHeartbeatTick;
            heartbeat.Start();

            TryLoadArtwork();
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            // Mantém o processo vivo sem poluir a taskbar do usuário.
            ShowInTaskbar = false;
            WindowState = FormWindowState.Minimized;
        }

        private void OnHeartbeatTick(object sender, EventArgs e)
        {
            try
            {
                SteamAPI.RunCallbacks();
            }
            catch (Exception ex)
            {
                EntryPoint.LogFailure(appId, ex);

                // Se a API falhar de forma irrecuperável, encerra este worker.
                BeginInvoke((MethodInvoker)Close);
            }
        }

        private void TryLoadArtwork()
        {
            try
            {
                picArtwork.Load($"https://cdn.akamai.steamstatic.com/steam/apps/{appId}/header_292x136.jpg");
            }
            catch (Exception ex)
            {
                // A arte de capa é apenas decorativa - falha aqui não é crítica.
                EntryPoint.LogFailure(appId, ex);
            }
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            heartbeat.Stop();
            heartbeat.Dispose();
        }
    }
}
