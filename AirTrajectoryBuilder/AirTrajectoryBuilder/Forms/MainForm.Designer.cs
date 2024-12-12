using System.Drawing;
using System.Windows.Forms;

namespace AirTrajectoryBuilder
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
            Menu = new MenuStrip();
            MenuFile = new ToolStripMenuItem();
            MenuFileNew = new ToolStripMenuItem();
            MenuFileOpen = new ToolStripMenuItem();
            MenuRun = new ToolStripMenuItem();
            MenuRunSweep = new ToolStripMenuItem();
            FileOpener = new OpenFileDialog();
            MenuRunCancel = new ToolStripMenuItem();
            Menu.SuspendLayout();
            SuspendLayout();
            // 
            // Menu
            // 
            Menu.ImageScalingSize = new Size(32, 32);
            Menu.Items.AddRange(new ToolStripItem[] { MenuFile, MenuRun });
            Menu.Location = new Point(0, 0);
            Menu.Name = "Menu";
            Menu.Size = new Size(1263, 42);
            Menu.TabIndex = 0;
            Menu.Text = "menuStrip1";
            // 
            // MenuFile
            // 
            MenuFile.DropDownItems.AddRange(new ToolStripItem[] { MenuFileNew, MenuFileOpen });
            MenuFile.Name = "MenuFile";
            MenuFile.Size = new Size(71, 38);
            MenuFile.Text = "File";
            // 
            // MenuFileNew
            // 
            MenuFileNew.Name = "MenuFileNew";
            MenuFileNew.Size = new Size(221, 44);
            MenuFileNew.Text = "New";
            MenuFileNew.Click += MenuFileNew_Click;
            // 
            // MenuFileOpen
            // 
            MenuFileOpen.Name = "MenuFileOpen";
            MenuFileOpen.Size = new Size(221, 44);
            MenuFileOpen.Text = "Open...";
            MenuFileOpen.Click += MenuFileOpen_Click;
            // 
            // MenuRun
            // 
            MenuRun.DropDownItems.AddRange(new ToolStripItem[] { MenuRunSweep, MenuRunCancel });
            MenuRun.Name = "MenuRun";
            MenuRun.Size = new Size(76, 38);
            MenuRun.Text = "Run";
            // 
            // MenuRunSweep
            // 
            MenuRunSweep.Name = "MenuRunSweep";
            MenuRunSweep.Size = new Size(359, 44);
            MenuRunSweep.Text = "Sweep...";
            MenuRunSweep.Click += MenuRunSweep_Click;
            // 
            // FileOpener
            // 
            FileOpener.Filter = "Scene files|*.sce|All files|*.*";
            // 
            // MenuRunCancel
            // 
            MenuRunCancel.Name = "MenuRunCancel";
            MenuRunCancel.Size = new Size(359, 44);
            MenuRunCancel.Text = "Cancel";
            MenuRunCancel.Click += MenuRunCancel_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1263, 719);
            Controls.Add(Menu);
            MainMenuStrip = Menu;
            Name = "MainForm";
            Text = "MainForm";
            Menu.ResumeLayout(false);
            Menu.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip Menu;
        private ToolStripMenuItem MenuFile;
        private ToolStripMenuItem MenuFileOpen;
        private ToolStripMenuItem MenuFileNew;
        private OpenFileDialog FileOpener;
        private ToolStripMenuItem MenuRun;
        private ToolStripMenuItem MenuRunSweep;
        private ToolStripMenuItem MenuRunCancel;
    }
}