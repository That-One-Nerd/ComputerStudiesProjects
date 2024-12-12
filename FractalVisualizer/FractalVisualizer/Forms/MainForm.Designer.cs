namespace FractalVisualizer.Forms
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Viewer = new PictureBox();
            PositionLabel = new Label();
            ((System.ComponentModel.ISupportInitialize)Viewer).BeginInit();
            SuspendLayout();
            // 
            // Viewer
            // 
            Viewer.Dock = DockStyle.Fill;
            Viewer.Location = new Point(0, 0);
            Viewer.Name = "Viewer";
            Viewer.Size = new Size(1366, 822);
            Viewer.TabIndex = 0;
            Viewer.TabStop = false;
            // 
            // PositionLabel
            // 
            PositionLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            PositionLabel.AutoSize = true;
            PositionLabel.Font = new Font("Segoe UI", 16.125F, FontStyle.Bold, GraphicsUnit.Point, 0);
            PositionLabel.Location = new Point(12, 754);
            PositionLabel.Name = "PositionLabel";
            PositionLabel.Size = new Size(295, 59);
            PositionLabel.TabIndex = 1;
            PositionLabel.Text = "PositionLabel";
            PositionLabel.TextAlign = ContentAlignment.BottomLeft;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1366, 822);
            Controls.Add(PositionLabel);
            Controls.Add(Viewer);
            Name = "MainForm";
            Text = "MainForm";
            ((System.ComponentModel.ISupportInitialize)Viewer).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox Viewer;
        private Label PositionLabel;
    }
}