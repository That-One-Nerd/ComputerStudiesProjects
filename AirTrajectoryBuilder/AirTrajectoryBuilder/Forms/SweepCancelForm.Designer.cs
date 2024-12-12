namespace AirTrajectoryBuilder.Forms
{
    partial class SweepCancelForm
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
            MessageLabel = new System.Windows.Forms.Label();
            YesButton = new System.Windows.Forms.Button();
            button2 = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // MessageLabel
            // 
            MessageLabel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            MessageLabel.Location = new System.Drawing.Point(12, 12);
            MessageLabel.Name = "MessageLabel";
            MessageLabel.Size = new System.Drawing.Size(637, 135);
            MessageLabel.TabIndex = 0;
            MessageLabel.Text = "Are you sure you want to cancel the sweep?";
            MessageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // YesButton
            // 
            YesButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            YesButton.DialogResult = System.Windows.Forms.DialogResult.Yes;
            YesButton.Location = new System.Drawing.Point(178, 150);
            YesButton.Name = "YesButton";
            YesButton.Size = new System.Drawing.Size(150, 46);
            YesButton.TabIndex = 1;
            YesButton.Text = "Yes";
            YesButton.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            button2.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            button2.DialogResult = System.Windows.Forms.DialogResult.No;
            button2.Location = new System.Drawing.Point(334, 150);
            button2.Name = "button2";
            button2.Size = new System.Drawing.Size(150, 46);
            button2.TabIndex = 2;
            button2.Text = "No";
            button2.UseVisualStyleBackColor = true;
            // 
            // SweepCancelForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(661, 208);
            Controls.Add(button2);
            Controls.Add(YesButton);
            Controls.Add(MessageLabel);
            Name = "SweepCancelForm";
            Text = "Cancel the Sweep?";
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label MessageLabel;
        private System.Windows.Forms.Button YesButton;
        private System.Windows.Forms.Button button2;
    }
}