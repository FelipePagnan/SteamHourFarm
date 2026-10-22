namespace IdleWorker
{
    partial class FormIdleWorker
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.PictureBox picArtwork;

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
            this.picArtwork = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picArtwork)).BeginInit();
            this.SuspendLayout();
            //
            // picArtwork
            //
            this.picArtwork.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picArtwork.Name = "picArtwork";
            this.picArtwork.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picArtwork.TabIndex = 0;
            //
            // FormIdleWorker
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 136);
            this.Controls.Add(this.picArtwork);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormIdleWorker";
            this.ShowInTaskbar = false;
            this.Text = "IdleWorker";
            ((System.ComponentModel.ISupportInitialize)(this.picArtwork)).EndInit();
            this.ResumeLayout(false);
        }
    }
}
