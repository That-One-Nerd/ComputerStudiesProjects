namespace BonusTicTacToe
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
            Toolbar = new MenuStrip();
            ReplayButton = new ToolStripMenuItem();
            CustomizeButton = new ToolStripMenuItem();
            ExitButton = new ToolStripMenuItem();
            Toolbar.SuspendLayout();
            SuspendLayout();
            // 
            // Toolbar
            // 
            Toolbar.AutoSize = false;
            Toolbar.ImageScalingSize = new Size(32, 32);
            Toolbar.Items.AddRange(new ToolStripItem[] { ReplayButton, CustomizeButton, ExitButton });
            Toolbar.Location = new Point(0, 0);
            Toolbar.Name = "Toolbar";
            Toolbar.Size = new Size(974, 56);
            Toolbar.TabIndex = 0;
            Toolbar.Text = "Toolbar";
            // 
            // ReplayButton
            // 
            ReplayButton.Name = "ReplayButton";
            ReplayButton.Size = new Size(104, 52);
            ReplayButton.Text = "Replay";
            ReplayButton.Click += ReplayButton_Click;
            // 
            // ExitButton
            // 
            ExitButton.Name = "ExitButton";
            ExitButton.Size = new Size(71, 52);
            ExitButton.Text = "Exit";
            ExitButton.Click += ExitButton_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(974, 929);
            Controls.Add(Toolbar);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MainMenuStrip = Toolbar;
            Name = "MainForm";
            Text = "Bonus Tic Tac Toe";
            Toolbar.ResumeLayout(false);
            Toolbar.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private MenuStrip Toolbar;
        private ToolStripMenuItem ReplayButton;
        private ToolStripMenuItem CustomizeButton;
        private ToolStripMenuItem ExitButton;
    }
}