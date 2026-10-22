using System.Drawing;
using System.Windows.Forms;

namespace SteamHourFarmPagnan
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        private Label lblTitle;
        private Label lblBrand;
        private Label lblInstructions;
        private TextBox txtGameIds;
        private Button btnStart;
        private Button btnStop;
        private Label lblRunningHeader;
        private ListBox lstRunning;
        private Label lblStatus;
        private Timer timerStatus;
        private Panel pnlAccentTop;

        // Paleta estilo Netflix: preto profundo + vermelho de destaque.
        private static readonly Color ColorBackground = Color.FromArgb(20, 20, 20);   // #141414
        private static readonly Color ColorPanel = Color.FromArgb(34, 34, 34);        // #222222
        private static readonly Color ColorPanelBorder = Color.FromArgb(55, 55, 55);  // #373737
        private static readonly Color ColorRed = Color.FromArgb(229, 9, 20);          // #E50914
        private static readonly Color ColorRedDark = Color.FromArgb(180, 7, 16);      // hover/borda
        private static readonly Color ColorTextPrimary = Color.FromArgb(255, 255, 255);
        private static readonly Color ColorTextMuted = Color.FromArgb(160, 160, 160);

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pnlAccentTop = new Panel();
            this.lblTitle = new Label();
            this.lblBrand = new Label();
            this.lblInstructions = new Label();
            this.txtGameIds = new TextBox();
            this.btnStart = new Button();
            this.btnStop = new Button();
            this.lblRunningHeader = new Label();
            this.lstRunning = new ListBox();
            this.lblStatus = new Label();
            this.timerStatus = new Timer(this.components);
            this.SuspendLayout();

            //
            // pnlAccentTop (faixa vermelha decorativa no topo)
            //
            this.pnlAccentTop.BackColor = ColorRed;
            this.pnlAccentTop.Location = new Point(0, 0);
            this.pnlAccentTop.Size = new Size(440, 4);
            this.pnlAccentTop.TabIndex = 0;

            //
            // lblTitle
            //
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new Font("Segoe UI", 19F, FontStyle.Bold);
            this.lblTitle.ForeColor = ColorTextPrimary;
            this.lblTitle.Location = new Point(18, 16);
            this.lblTitle.Text = "STEAM HOUR FARM";

            //
            // lblBrand
            //
            this.lblBrand.AutoSize = true;
            this.lblBrand.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.lblBrand.ForeColor = ColorRed;
            this.lblBrand.Location = new Point(20, 50);
            this.lblBrand.Text = "by Pagnan";

            //
            // lblInstructions
            //
            this.lblInstructions.AutoSize = true;
            this.lblInstructions.Font = new Font("Segoe UI", 9F);
            this.lblInstructions.ForeColor = ColorTextMuted;
            this.lblInstructions.Location = new Point(18, 80);
            this.lblInstructions.MaximumSize = new Size(404, 0);
            this.lblInstructions.Text = "Digite os IDs dos jogos (separados por vírgula, espaço ou quebra de linha):";

            //
            // txtGameIds
            //
            this.txtGameIds.BackColor = ColorPanel;
            this.txtGameIds.BorderStyle = BorderStyle.FixedSingle;
            this.txtGameIds.Font = new Font("Segoe UI", 10F);
            this.txtGameIds.ForeColor = ColorTextPrimary;
            this.txtGameIds.Location = new Point(18, 102);
            this.txtGameIds.Multiline = true;
            this.txtGameIds.ScrollBars = ScrollBars.Vertical;
            this.txtGameIds.Size = new Size(404, 110);

            //
            // btnStart
            //
            this.btnStart.BackColor = ColorRed;
            this.btnStart.FlatAppearance.BorderSize = 0;
            this.btnStart.FlatAppearance.MouseOverBackColor = ColorRedDark;
            this.btnStart.FlatStyle = FlatStyle.Flat;
            this.btnStart.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            this.btnStart.ForeColor = ColorTextPrimary;
            this.btnStart.Location = new Point(18, 224);
            this.btnStart.Size = new Size(195, 38);
            this.btnStart.Text = "▶  INICIAR";
            this.btnStart.UseVisualStyleBackColor = false;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);

            //
            // btnStop
            //
            this.btnStop.BackColor = ColorPanel;
            this.btnStop.Enabled = false;
            this.btnStop.FlatAppearance.BorderColor = ColorPanelBorder;
            this.btnStop.FlatAppearance.BorderSize = 1;
            this.btnStop.FlatAppearance.MouseOverBackColor = ColorPanelBorder;
            this.btnStop.FlatStyle = FlatStyle.Flat;
            this.btnStop.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            this.btnStop.ForeColor = ColorTextPrimary;
            this.btnStop.Location = new Point(227, 224);
            this.btnStop.Size = new Size(195, 38);
            this.btnStop.Text = "■  PARAR";
            this.btnStop.UseVisualStyleBackColor = false;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);

            //
            // lblRunningHeader
            //
            this.lblRunningHeader.AutoSize = true;
            this.lblRunningHeader.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            this.lblRunningHeader.ForeColor = ColorTextPrimary;
            this.lblRunningHeader.Location = new Point(18, 274);
            this.lblRunningHeader.Text = "JOGOS EM EXECUÇÃO";

            //
            // lstRunning
            //
            this.lstRunning.BackColor = ColorPanel;
            this.lstRunning.BorderStyle = BorderStyle.FixedSingle;
            this.lstRunning.Font = new Font("Segoe UI", 9.5F);
            this.lstRunning.ForeColor = ColorTextPrimary;
            this.lstRunning.FormattingEnabled = true;
            this.lstRunning.Location = new Point(18, 296);
            this.lstRunning.Size = new Size(404, 96);

            //
            // lblStatus
            //
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new Font("Segoe UI", 9F);
            this.lblStatus.ForeColor = ColorTextMuted;
            this.lblStatus.Location = new Point(18, 400);
            this.lblStatus.Text = "Jogos ativos: 0";

            //
            // timerStatus
            //
            this.timerStatus.Interval = 1000;
            this.timerStatus.Tick += new System.EventHandler(this.timerStatus_Tick);
            this.timerStatus.Start();

            //
            // MainForm
            //
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = ColorBackground;
            this.ClientSize = new Size(440, 430);
            this.Controls.Add(this.pnlAccentTop);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblBrand);
            this.Controls.Add(this.lblInstructions);
            this.Controls.Add(this.txtGameIds);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.lblRunningHeader);
            this.Controls.Add(this.lstRunning);
            this.Controls.Add(this.lblStatus);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Steam Hour Farm - Pagnan";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
