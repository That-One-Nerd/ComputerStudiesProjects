namespace AirTrajectoryBuilder.Forms
{
    partial class SweepInfoViewer
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
            SweepProgress = new System.Windows.Forms.ProgressBar();
            ProgressLabel = new System.Windows.Forms.Label();
            ResultsPanel = new System.Windows.Forms.Panel();
            ResultsLabel = new System.Windows.Forms.Label();
            ResultsPanel.SuspendLayout();
            SuspendLayout();
            // 
            // SweepProgress
            // 
            SweepProgress.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            SweepProgress.Location = new System.Drawing.Point(12, 229);
            SweepProgress.Name = "SweepProgress";
            SweepProgress.Size = new System.Drawing.Size(673, 20);
            SweepProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            SweepProgress.TabIndex = 0;
            SweepProgress.Value = 50;
            // 
            // ProgressLabel
            // 
            ProgressLabel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            ProgressLabel.Location = new System.Drawing.Point(12, 194);
            ProgressLabel.Name = "ProgressLabel";
            ProgressLabel.Size = new System.Drawing.Size(673, 32);
            ProgressLabel.TabIndex = 1;
            ProgressLabel.Text = "Conducting Sweep...";
            ProgressLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ResultsPanel
            // 
            ResultsPanel.AutoScroll = true;
            ResultsPanel.Controls.Add(ResultsLabel);
            ResultsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            ResultsPanel.Location = new System.Drawing.Point(0, 0);
            ResultsPanel.Name = "ResultsPanel";
            ResultsPanel.Size = new System.Drawing.Size(697, 405);
            ResultsPanel.TabIndex = 2;
            ResultsPanel.Visible = false;
            // 
            // ResultsLabel
            // 
            ResultsLabel.Location = new System.Drawing.Point(12, 9);
            ResultsLabel.Name = "ResultsLabel";
            ResultsLabel.Size = new System.Drawing.Size(682, 387);
            ResultsLabel.TabIndex = 0;
            ResultsLabel.Text = "label1";
            // 
            // SweepInfoViewer
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(697, 405);
            Controls.Add(ResultsPanel);
            Controls.Add(ProgressLabel);
            Controls.Add(SweepProgress);
            Name = "SweepInfoViewer";
            StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            Text = "Simulation Results";
            ResultsPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.ProgressBar SweepProgress;
        private System.Windows.Forms.Label ProgressLabel;
        private System.Windows.Forms.Panel ResultsPanel;
        private System.Windows.Forms.Label ResultsLabel;
    }
}