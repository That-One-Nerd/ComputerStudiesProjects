namespace AirTrajectoryBuilder
{
    partial class SweepParametersForm
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
            AngleSweepValue = new System.Windows.Forms.TextBox();
            AngleSweepLabel = new System.Windows.Forms.Label();
            SpeedMinLabel = new System.Windows.Forms.Label();
            SpeedMinValue = new System.Windows.Forms.TextBox();
            SpeedMaxLabel = new System.Windows.Forms.Label();
            SpeedDeltaLabel = new System.Windows.Forms.Label();
            SpeedMaxValue = new System.Windows.Forms.TextBox();
            SpeedDeltaValue = new System.Windows.Forms.TextBox();
            RunButton = new System.Windows.Forms.Button();
            GravityLabel = new System.Windows.Forms.Label();
            GravityValue = new System.Windows.Forms.TextBox();
            TimeDeltaLabel = new System.Windows.Forms.Label();
            TimeDeltaValue = new System.Windows.Forms.TextBox();
            CancelButton = new System.Windows.Forms.Button();
            ProjectileMotionLabel = new System.Windows.Forms.Label();
            AirTrajectoryLabel = new System.Windows.Forms.Label();
            ObjectRadiusValue = new System.Windows.Forms.TextBox();
            ObjectRadiusLabel = new System.Windows.Forms.Label();
            MassValue = new System.Windows.Forms.TextBox();
            MassLabel = new System.Windows.Forms.Label();
            AirDensityValue = new System.Windows.Forms.TextBox();
            AirDensityLabel = new System.Windows.Forms.Label();
            DragCoefficientValue = new System.Windows.Forms.TextBox();
            DragCoefficientLabel = new System.Windows.Forms.Label();
            ResultsLabel = new System.Windows.Forms.Label();
            FileOutputLabel = new System.Windows.Forms.Label();
            FileOutputValue = new System.Windows.Forms.ComboBox();
            SuspendLayout();
            // 
            // AngleSweepValue
            // 
            AngleSweepValue.Location = new System.Drawing.Point(312, 122);
            AngleSweepValue.Name = "AngleSweepValue";
            AngleSweepValue.Size = new System.Drawing.Size(101, 39);
            AngleSweepValue.TabIndex = 0;
            // 
            // AngleSweepLabel
            // 
            AngleSweepLabel.Location = new System.Drawing.Point(82, 122);
            AngleSweepLabel.Name = "AngleSweepLabel";
            AngleSweepLabel.Size = new System.Drawing.Size(224, 39);
            AngleSweepLabel.TabIndex = 1;
            AngleSweepLabel.Text = "Angle Sweep Delta";
            AngleSweepLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // SpeedMinLabel
            // 
            SpeedMinLabel.Location = new System.Drawing.Point(12, 168);
            SpeedMinLabel.Name = "SpeedMinLabel";
            SpeedMinLabel.Size = new System.Drawing.Size(159, 39);
            SpeedMinLabel.TabIndex = 2;
            SpeedMinLabel.Text = "Speed Min";
            SpeedMinLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SpeedMinValue
            // 
            SpeedMinValue.Location = new System.Drawing.Point(36, 210);
            SpeedMinValue.Name = "SpeedMinValue";
            SpeedMinValue.Size = new System.Drawing.Size(101, 39);
            SpeedMinValue.TabIndex = 3;
            // 
            // SpeedMaxLabel
            // 
            SpeedMaxLabel.Location = new System.Drawing.Point(177, 168);
            SpeedMaxLabel.Name = "SpeedMaxLabel";
            SpeedMaxLabel.Size = new System.Drawing.Size(159, 39);
            SpeedMaxLabel.TabIndex = 4;
            SpeedMaxLabel.Text = "Speed Max";
            SpeedMaxLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SpeedDeltaLabel
            // 
            SpeedDeltaLabel.Location = new System.Drawing.Point(342, 168);
            SpeedDeltaLabel.Name = "SpeedDeltaLabel";
            SpeedDeltaLabel.Size = new System.Drawing.Size(159, 39);
            SpeedDeltaLabel.TabIndex = 5;
            SpeedDeltaLabel.Text = "Sweep Delta";
            SpeedDeltaLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SpeedMaxValue
            // 
            SpeedMaxValue.Location = new System.Drawing.Point(204, 210);
            SpeedMaxValue.Name = "SpeedMaxValue";
            SpeedMaxValue.Size = new System.Drawing.Size(101, 39);
            SpeedMaxValue.TabIndex = 6;
            // 
            // SpeedDeltaValue
            // 
            SpeedDeltaValue.Location = new System.Drawing.Point(371, 210);
            SpeedDeltaValue.Name = "SpeedDeltaValue";
            SpeedDeltaValue.Size = new System.Drawing.Size(101, 39);
            SpeedDeltaValue.TabIndex = 7;
            // 
            // RunButton
            // 
            RunButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            RunButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            RunButton.Location = new System.Drawing.Point(198, 707);
            RunButton.Name = "RunButton";
            RunButton.Size = new System.Drawing.Size(150, 46);
            RunButton.TabIndex = 8;
            RunButton.Text = "Sweep";
            RunButton.UseVisualStyleBackColor = true;
            // 
            // GravityLabel
            // 
            GravityLabel.Location = new System.Drawing.Point(138, 258);
            GravityLabel.Name = "GravityLabel";
            GravityLabel.Size = new System.Drawing.Size(113, 39);
            GravityLabel.TabIndex = 10;
            GravityLabel.Text = "Gravity";
            GravityLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // GravityValue
            // 
            GravityValue.Location = new System.Drawing.Point(257, 258);
            GravityValue.Name = "GravityValue";
            GravityValue.Size = new System.Drawing.Size(101, 39);
            GravityValue.TabIndex = 9;
            // 
            // TimeDeltaLabel
            // 
            TimeDeltaLabel.Location = new System.Drawing.Point(131, 75);
            TimeDeltaLabel.Name = "TimeDeltaLabel";
            TimeDeltaLabel.Size = new System.Drawing.Size(137, 39);
            TimeDeltaLabel.TabIndex = 12;
            TimeDeltaLabel.Text = "Time Delta";
            TimeDeltaLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // TimeDeltaValue
            // 
            TimeDeltaValue.Location = new System.Drawing.Point(274, 75);
            TimeDeltaValue.Name = "TimeDeltaValue";
            TimeDeltaValue.Size = new System.Drawing.Size(101, 39);
            TimeDeltaValue.TabIndex = 11;
            // 
            // CancelButton
            // 
            CancelButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            CancelButton.Location = new System.Drawing.Point(354, 707);
            CancelButton.Name = "CancelButton";
            CancelButton.Size = new System.Drawing.Size(150, 46);
            CancelButton.TabIndex = 13;
            CancelButton.Text = "Cancel";
            CancelButton.UseVisualStyleBackColor = true;
            // 
            // ProjectileMotionLabel
            // 
            ProjectileMotionLabel.Font = new System.Drawing.Font("Segoe UI", 10.125F, System.Drawing.FontStyle.Bold);
            ProjectileMotionLabel.Location = new System.Drawing.Point(12, 9);
            ProjectileMotionLabel.Name = "ProjectileMotionLabel";
            ProjectileMotionLabel.Size = new System.Drawing.Size(489, 39);
            ProjectileMotionLabel.TabIndex = 14;
            ProjectileMotionLabel.Text = "General Parameters:";
            ProjectileMotionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // AirTrajectoryLabel
            // 
            AirTrajectoryLabel.Font = new System.Drawing.Font("Segoe UI", 10.125F, System.Drawing.FontStyle.Bold);
            AirTrajectoryLabel.Location = new System.Drawing.Point(14, 327);
            AirTrajectoryLabel.Name = "AirTrajectoryLabel";
            AirTrajectoryLabel.Size = new System.Drawing.Size(489, 39);
            AirTrajectoryLabel.TabIndex = 15;
            AirTrajectoryLabel.Text = "Aerodynamics (Optional):";
            AirTrajectoryLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ObjectRadiusValue
            // 
            ObjectRadiusValue.Location = new System.Drawing.Point(82, 419);
            ObjectRadiusValue.Name = "ObjectRadiusValue";
            ObjectRadiusValue.Size = new System.Drawing.Size(101, 39);
            ObjectRadiusValue.TabIndex = 17;
            // 
            // ObjectRadiusLabel
            // 
            ObjectRadiusLabel.Location = new System.Drawing.Point(12, 377);
            ObjectRadiusLabel.Name = "ObjectRadiusLabel";
            ObjectRadiusLabel.Size = new System.Drawing.Size(239, 39);
            ObjectRadiusLabel.TabIndex = 16;
            ObjectRadiusLabel.Text = "Object Radius";
            ObjectRadiusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MassValue
            // 
            MassValue.Location = new System.Drawing.Point(335, 419);
            MassValue.Name = "MassValue";
            MassValue.Size = new System.Drawing.Size(101, 39);
            MassValue.TabIndex = 19;
            // 
            // MassLabel
            // 
            MassLabel.Location = new System.Drawing.Point(265, 377);
            MassLabel.Name = "MassLabel";
            MassLabel.Size = new System.Drawing.Size(239, 39);
            MassLabel.TabIndex = 18;
            MassLabel.Text = "Object Mass";
            MassLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // AirDensityValue
            // 
            AirDensityValue.Location = new System.Drawing.Point(335, 512);
            AirDensityValue.Name = "AirDensityValue";
            AirDensityValue.Size = new System.Drawing.Size(101, 39);
            AirDensityValue.TabIndex = 23;
            // 
            // AirDensityLabel
            // 
            AirDensityLabel.Location = new System.Drawing.Point(265, 470);
            AirDensityLabel.Name = "AirDensityLabel";
            AirDensityLabel.Size = new System.Drawing.Size(239, 39);
            AirDensityLabel.TabIndex = 22;
            AirDensityLabel.Text = "Air Density";
            AirDensityLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // DragCoefficientValue
            // 
            DragCoefficientValue.Location = new System.Drawing.Point(82, 512);
            DragCoefficientValue.Name = "DragCoefficientValue";
            DragCoefficientValue.Size = new System.Drawing.Size(101, 39);
            DragCoefficientValue.TabIndex = 21;
            // 
            // DragCoefficientLabel
            // 
            DragCoefficientLabel.Location = new System.Drawing.Point(12, 470);
            DragCoefficientLabel.Name = "DragCoefficientLabel";
            DragCoefficientLabel.Size = new System.Drawing.Size(239, 39);
            DragCoefficientLabel.TabIndex = 20;
            DragCoefficientLabel.Text = "Drag Coefficient";
            DragCoefficientLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ResultsLabel
            // 
            ResultsLabel.Font = new System.Drawing.Font("Segoe UI", 10.125F, System.Drawing.FontStyle.Bold);
            ResultsLabel.Location = new System.Drawing.Point(14, 583);
            ResultsLabel.Name = "ResultsLabel";
            ResultsLabel.Size = new System.Drawing.Size(489, 39);
            ResultsLabel.TabIndex = 24;
            ResultsLabel.Text = "Results:";
            ResultsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FileOutputLabel
            // 
            FileOutputLabel.Location = new System.Drawing.Point(37, 622);
            FileOutputLabel.Name = "FileOutputLabel";
            FileOutputLabel.Size = new System.Drawing.Size(156, 64);
            FileOutputLabel.TabIndex = 25;
            FileOutputLabel.Text = "File Output";
            FileOutputLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // FileOutputValue
            // 
            FileOutputValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            FileOutputValue.FormattingEnabled = true;
            FileOutputValue.Items.AddRange(new object[] { "None", "Text - Best", "Text - All", "Json - Best", "Json - All", "Binary - Best", "Binary - All" });
            FileOutputValue.Location = new System.Drawing.Point(199, 635);
            FileOutputValue.MaxLength = 1;
            FileOutputValue.Name = "FileOutputValue";
            FileOutputValue.Size = new System.Drawing.Size(242, 40);
            FileOutputValue.TabIndex = 26;
            // 
            // SweepParametersForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(516, 765);
            Controls.Add(FileOutputValue);
            Controls.Add(FileOutputLabel);
            Controls.Add(ResultsLabel);
            Controls.Add(AirDensityValue);
            Controls.Add(AirDensityLabel);
            Controls.Add(DragCoefficientValue);
            Controls.Add(DragCoefficientLabel);
            Controls.Add(MassValue);
            Controls.Add(MassLabel);
            Controls.Add(ObjectRadiusValue);
            Controls.Add(ObjectRadiusLabel);
            Controls.Add(AirTrajectoryLabel);
            Controls.Add(ProjectileMotionLabel);
            Controls.Add(CancelButton);
            Controls.Add(TimeDeltaLabel);
            Controls.Add(TimeDeltaValue);
            Controls.Add(GravityLabel);
            Controls.Add(GravityValue);
            Controls.Add(RunButton);
            Controls.Add(SpeedDeltaValue);
            Controls.Add(SpeedMaxValue);
            Controls.Add(SpeedDeltaLabel);
            Controls.Add(SpeedMaxLabel);
            Controls.Add(SpeedMinValue);
            Controls.Add(SpeedMinLabel);
            Controls.Add(AngleSweepLabel);
            Controls.Add(AngleSweepValue);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Name = "SweepParametersForm";
            Text = "Set Sweep Parameters";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox AngleSweepValue;
        private System.Windows.Forms.Label AngleSweepLabel;
        private System.Windows.Forms.Label SpeedMinLabel;
        private System.Windows.Forms.TextBox SpeedMinValue;
        private System.Windows.Forms.Label SpeedMaxLabel;
        private System.Windows.Forms.Label SpeedDeltaLabel;
        private System.Windows.Forms.TextBox SpeedMaxValue;
        private System.Windows.Forms.TextBox SpeedDeltaValue;
        private System.Windows.Forms.Button RunButton;
        private System.Windows.Forms.Label GravityLabel;
        private System.Windows.Forms.TextBox GravityValue;
        private System.Windows.Forms.Label TimeDeltaLabel;
        private System.Windows.Forms.TextBox TimeDeltaValue;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Label ProjectileMotionLabel;
        private System.Windows.Forms.Label AirTrajectoryLabel;
        private System.Windows.Forms.TextBox ObjectRadiusValue;
        private System.Windows.Forms.Label ObjectRadiusLabel;
        private System.Windows.Forms.TextBox MassValue;
        private System.Windows.Forms.Label MassLabel;
        private System.Windows.Forms.TextBox AirDensityValue;
        private System.Windows.Forms.Label AirDensityLabel;
        private System.Windows.Forms.TextBox DragCoefficientValue;
        private System.Windows.Forms.Label DragCoefficientLabel;
        private System.Windows.Forms.Label ResultsLabel;
        private System.Windows.Forms.Label FileOutputLabel;
        private System.Windows.Forms.ComboBox FileOutputValue;
    }
}