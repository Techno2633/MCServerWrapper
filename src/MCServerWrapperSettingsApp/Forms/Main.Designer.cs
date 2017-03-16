namespace MCServerWrapperSettingsApp.Forms
{
    partial class Main
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
            this.Prompt1 = new System.Windows.Forms.Label();
            this.ServerPath = new System.Windows.Forms.TextBox();
            this.BrowseServerPath = new System.Windows.Forms.Button();
            this.SameMaxMin = new System.Windows.Forms.CheckBox();
            this.MaxRamText = new System.Windows.Forms.TextBox();
            this.MaxRamSlider = new System.Windows.Forms.TrackBar();
            this.Prompt3 = new System.Windows.Forms.Label();
            this.MinRamText = new System.Windows.Forms.TextBox();
            this.MinRamSlider = new System.Windows.Forms.TrackBar();
            this.Prompt2 = new System.Windows.Forms.Label();
            this.Prompt6 = new System.Windows.Forms.Label();
            this.BackupInterval = new System.Windows.Forms.NumericUpDown();
            this.BackupSource = new System.Windows.Forms.TextBox();
            this.Prompt4 = new System.Windows.Forms.Label();
            this.BrowseBackupSource = new System.Windows.Forms.Button();
            this.BrowseBackupLocation = new System.Windows.Forms.Button();
            this.Prompt5 = new System.Windows.Forms.Label();
            this.BackupLocation = new System.Windows.Forms.TextBox();
            this.BackupNumber = new System.Windows.Forms.NumericUpDown();
            this.Prompt7 = new System.Windows.Forms.Label();
            this.GenSettings = new System.Windows.Forms.Button();
            this.Prompt10 = new System.Windows.Forms.Label();
            this.ConsoleColorCbo = new System.Windows.Forms.ComboBox();
            this.CompressionLevelCbo = new System.Windows.Forms.ComboBox();
            this.Prompt9 = new System.Windows.Forms.Label();
            this.ShowRamCpuUsage = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.MaxRamSlider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MinRamSlider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BackupInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BackupNumber)).BeginInit();
            this.SuspendLayout();
            // 
            // Prompt1
            // 
            this.Prompt1.AutoSize = true;
            this.Prompt1.Location = new System.Drawing.Point(12, 15);
            this.Prompt1.Name = "Prompt1";
            this.Prompt1.Size = new System.Drawing.Size(63, 13);
            this.Prompt1.TabIndex = 0;
            this.Prompt1.Text = "Server Path";
            // 
            // ServerPath
            // 
            this.ServerPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ServerPath.Location = new System.Drawing.Point(81, 12);
            this.ServerPath.Name = "ServerPath";
            this.ServerPath.Size = new System.Drawing.Size(253, 20);
            this.ServerPath.TabIndex = 1;
            // 
            // BrowseServerPath
            // 
            this.BrowseServerPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BrowseServerPath.Location = new System.Drawing.Point(340, 11);
            this.BrowseServerPath.Name = "BrowseServerPath";
            this.BrowseServerPath.Size = new System.Drawing.Size(54, 22);
            this.BrowseServerPath.TabIndex = 2;
            this.BrowseServerPath.Text = "Browse";
            this.BrowseServerPath.UseVisualStyleBackColor = true;
            this.BrowseServerPath.Click += new System.EventHandler(this.BrowseServerPath_Click);
            // 
            // SameMaxMin
            // 
            this.SameMaxMin.AutoSize = true;
            this.SameMaxMin.Location = new System.Drawing.Point(15, 175);
            this.SameMaxMin.Name = "SameMaxMin";
            this.SameMaxMin.Size = new System.Drawing.Size(227, 17);
            this.SameMaxMin.TabIndex = 17;
            this.SameMaxMin.Text = "Set Max and Min values to the same value";
            this.SameMaxMin.UseVisualStyleBackColor = true;
            // 
            // MaxRamText
            // 
            this.MaxRamText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.MaxRamText.Location = new System.Drawing.Point(340, 124);
            this.MaxRamText.Name = "MaxRamText";
            this.MaxRamText.ReadOnly = true;
            this.MaxRamText.Size = new System.Drawing.Size(54, 20);
            this.MaxRamText.TabIndex = 16;
            // 
            // MaxRamSlider
            // 
            this.MaxRamSlider.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MaxRamSlider.Location = new System.Drawing.Point(15, 124);
            this.MaxRamSlider.Name = "MaxRamSlider";
            this.MaxRamSlider.Size = new System.Drawing.Size(319, 45);
            this.MaxRamSlider.TabIndex = 15;
            this.MaxRamSlider.Scroll += new System.EventHandler(this.MaxRamSlider_Scroll);
            // 
            // Prompt3
            // 
            this.Prompt3.AutoSize = true;
            this.Prompt3.Location = new System.Drawing.Point(12, 108);
            this.Prompt3.Name = "Prompt3";
            this.Prompt3.Size = new System.Drawing.Size(78, 13);
            this.Prompt3.TabIndex = 14;
            this.Prompt3.Text = "Maximum RAM";
            // 
            // MinRamText
            // 
            this.MinRamText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.MinRamText.Location = new System.Drawing.Point(340, 60);
            this.MinRamText.Name = "MinRamText";
            this.MinRamText.ReadOnly = true;
            this.MinRamText.Size = new System.Drawing.Size(54, 20);
            this.MinRamText.TabIndex = 13;
            // 
            // MinRamSlider
            // 
            this.MinRamSlider.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MinRamSlider.Location = new System.Drawing.Point(12, 60);
            this.MinRamSlider.Name = "MinRamSlider";
            this.MinRamSlider.Size = new System.Drawing.Size(322, 45);
            this.MinRamSlider.TabIndex = 12;
            this.MinRamSlider.Scroll += new System.EventHandler(this.MinRamSlider_Scroll);
            // 
            // Prompt2
            // 
            this.Prompt2.AutoSize = true;
            this.Prompt2.Location = new System.Drawing.Point(12, 44);
            this.Prompt2.Name = "Prompt2";
            this.Prompt2.Size = new System.Drawing.Size(75, 13);
            this.Prompt2.TabIndex = 11;
            this.Prompt2.Text = "Minimum RAM";
            // 
            // Prompt6
            // 
            this.Prompt6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Prompt6.AutoSize = true;
            this.Prompt6.Location = new System.Drawing.Point(12, 275);
            this.Prompt6.Name = "Prompt6";
            this.Prompt6.Size = new System.Drawing.Size(128, 13);
            this.Prompt6.TabIndex = 18;
            this.Prompt6.Text = "Backup Interval (Minutes)";
            // 
            // BackupInterval
            // 
            this.BackupInterval.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BackupInterval.Location = new System.Drawing.Point(146, 273);
            this.BackupInterval.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.BackupInterval.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.BackupInterval.Name = "BackupInterval";
            this.BackupInterval.Size = new System.Drawing.Size(88, 20);
            this.BackupInterval.TabIndex = 23;
            this.BackupInterval.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // BackupSource
            // 
            this.BackupSource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BackupSource.Location = new System.Drawing.Point(99, 221);
            this.BackupSource.Name = "BackupSource";
            this.BackupSource.Size = new System.Drawing.Size(235, 20);
            this.BackupSource.TabIndex = 19;
            // 
            // Prompt4
            // 
            this.Prompt4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Prompt4.AutoSize = true;
            this.Prompt4.Location = new System.Drawing.Point(12, 224);
            this.Prompt4.Name = "Prompt4";
            this.Prompt4.Size = new System.Drawing.Size(81, 13);
            this.Prompt4.TabIndex = 21;
            this.Prompt4.Text = "Backup Source";
            // 
            // BrowseBackupSource
            // 
            this.BrowseBackupSource.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BrowseBackupSource.Location = new System.Drawing.Point(340, 220);
            this.BrowseBackupSource.Name = "BrowseBackupSource";
            this.BrowseBackupSource.Size = new System.Drawing.Size(54, 22);
            this.BrowseBackupSource.TabIndex = 20;
            this.BrowseBackupSource.Text = "Browse";
            this.BrowseBackupSource.UseVisualStyleBackColor = true;
            this.BrowseBackupSource.Click += new System.EventHandler(this.BrowseBackupSource_Click);
            // 
            // BrowseBackupLocation
            // 
            this.BrowseBackupLocation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BrowseBackupLocation.Location = new System.Drawing.Point(340, 246);
            this.BrowseBackupLocation.Name = "BrowseBackupLocation";
            this.BrowseBackupLocation.Size = new System.Drawing.Size(54, 22);
            this.BrowseBackupLocation.TabIndex = 22;
            this.BrowseBackupLocation.Text = "Browse";
            this.BrowseBackupLocation.UseVisualStyleBackColor = true;
            this.BrowseBackupLocation.Click += new System.EventHandler(this.BrowseBackupLocation_Click);
            // 
            // Prompt5
            // 
            this.Prompt5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Prompt5.AutoSize = true;
            this.Prompt5.Location = new System.Drawing.Point(12, 250);
            this.Prompt5.Name = "Prompt5";
            this.Prompt5.Size = new System.Drawing.Size(88, 13);
            this.Prompt5.TabIndex = 24;
            this.Prompt5.Text = "Backup Location";
            // 
            // BackupLocation
            // 
            this.BackupLocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BackupLocation.Location = new System.Drawing.Point(106, 247);
            this.BackupLocation.Name = "BackupLocation";
            this.BackupLocation.Size = new System.Drawing.Size(228, 20);
            this.BackupLocation.TabIndex = 21;
            // 
            // BackupNumber
            // 
            this.BackupNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BackupNumber.Location = new System.Drawing.Point(119, 299);
            this.BackupNumber.Maximum = new decimal(new int[] {
            120,
            0,
            0,
            0});
            this.BackupNumber.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.BackupNumber.Name = "BackupNumber";
            this.BackupNumber.Size = new System.Drawing.Size(88, 20);
            this.BackupNumber.TabIndex = 27;
            this.BackupNumber.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // Prompt7
            // 
            this.Prompt7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Prompt7.AutoSize = true;
            this.Prompt7.Location = new System.Drawing.Point(12, 301);
            this.Prompt7.Name = "Prompt7";
            this.Prompt7.Size = new System.Drawing.Size(101, 13);
            this.Prompt7.TabIndex = 26;
            this.Prompt7.Text = "Number of Backups";
            // 
            // GenSettings
            // 
            this.GenSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.GenSettings.Location = new System.Drawing.Point(278, 350);
            this.GenSettings.Name = "GenSettings";
            this.GenSettings.Size = new System.Drawing.Size(116, 23);
            this.GenSettings.TabIndex = 31;
            this.GenSettings.Text = "Generate Settings";
            this.GenSettings.UseVisualStyleBackColor = true;
            this.GenSettings.Click += new System.EventHandler(this.GenSettings_Click);
            // 
            // Prompt10
            // 
            this.Prompt10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Prompt10.AutoSize = true;
            this.Prompt10.Location = new System.Drawing.Point(12, 355);
            this.Prompt10.Name = "Prompt10";
            this.Prompt10.Size = new System.Drawing.Size(113, 13);
            this.Prompt10.TabIndex = 29;
            this.Prompt10.Text = "Wrapper Console Text";
            // 
            // ConsoleColorCbo
            // 
            this.ConsoleColorCbo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ConsoleColorCbo.FormattingEnabled = true;
            this.ConsoleColorCbo.Location = new System.Drawing.Point(131, 352);
            this.ConsoleColorCbo.Name = "ConsoleColorCbo";
            this.ConsoleColorCbo.Size = new System.Drawing.Size(128, 21);
            this.ConsoleColorCbo.TabIndex = 30;
            // 
            // CompressionLevelCbo
            // 
            this.CompressionLevelCbo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CompressionLevelCbo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CompressionLevelCbo.FormattingEnabled = true;
            this.CompressionLevelCbo.Location = new System.Drawing.Point(113, 325);
            this.CompressionLevelCbo.Name = "CompressionLevelCbo";
            this.CompressionLevelCbo.Size = new System.Drawing.Size(121, 21);
            this.CompressionLevelCbo.TabIndex = 28;
            this.CompressionLevelCbo.SelectedValueChanged += new System.EventHandler(this.CompressionLevelCbo_SelectedValueChanged);
            // 
            // Prompt9
            // 
            this.Prompt9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Prompt9.AutoSize = true;
            this.Prompt9.Location = new System.Drawing.Point(11, 328);
            this.Prompt9.Name = "Prompt9";
            this.Prompt9.Size = new System.Drawing.Size(96, 13);
            this.Prompt9.TabIndex = 35;
            this.Prompt9.Text = "Compression Level";
            // 
            // ShowRamCpuUsage
            // 
            this.ShowRamCpuUsage.AutoSize = true;
            this.ShowRamCpuUsage.Location = new System.Drawing.Point(15, 198);
            this.ShowRamCpuUsage.Name = "ShowRamCpuUsage";
            this.ShowRamCpuUsage.Size = new System.Drawing.Size(160, 17);
            this.ShowRamCpuUsage.TabIndex = 18;
            this.ShowRamCpuUsage.Text = "Show RAM and CPU Usage";
            this.ShowRamCpuUsage.UseVisualStyleBackColor = true;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(406, 385);
            this.Controls.Add(this.ShowRamCpuUsage);
            this.Controls.Add(this.Prompt9);
            this.Controls.Add(this.CompressionLevelCbo);
            this.Controls.Add(this.ConsoleColorCbo);
            this.Controls.Add(this.Prompt10);
            this.Controls.Add(this.GenSettings);
            this.Controls.Add(this.BackupNumber);
            this.Controls.Add(this.Prompt7);
            this.Controls.Add(this.BrowseBackupLocation);
            this.Controls.Add(this.Prompt5);
            this.Controls.Add(this.BackupLocation);
            this.Controls.Add(this.BrowseBackupSource);
            this.Controls.Add(this.Prompt4);
            this.Controls.Add(this.BackupSource);
            this.Controls.Add(this.BackupInterval);
            this.Controls.Add(this.Prompt6);
            this.Controls.Add(this.SameMaxMin);
            this.Controls.Add(this.MaxRamText);
            this.Controls.Add(this.MaxRamSlider);
            this.Controls.Add(this.Prompt3);
            this.Controls.Add(this.MinRamText);
            this.Controls.Add(this.MinRamSlider);
            this.Controls.Add(this.Prompt2);
            this.Controls.Add(this.BrowseServerPath);
            this.Controls.Add(this.ServerPath);
            this.Controls.Add(this.Prompt1);
            this.MinimumSize = new System.Drawing.Size(422, 424);
            this.Name = "Main";
            this.ShowIcon = false;
            this.Text = "MC Server Wrapper Setting Generator";
            ((System.ComponentModel.ISupportInitialize)(this.MaxRamSlider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MinRamSlider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BackupInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BackupNumber)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Prompt1;
        private System.Windows.Forms.TextBox ServerPath;
        private System.Windows.Forms.Button BrowseServerPath;
        private System.Windows.Forms.CheckBox SameMaxMin;
        private System.Windows.Forms.TextBox MaxRamText;
        private System.Windows.Forms.TrackBar MaxRamSlider;
        private System.Windows.Forms.Label Prompt3;
        private System.Windows.Forms.TextBox MinRamText;
        private System.Windows.Forms.TrackBar MinRamSlider;
        private System.Windows.Forms.Label Prompt2;
        private System.Windows.Forms.Label Prompt6;
        private System.Windows.Forms.NumericUpDown BackupInterval;
        private System.Windows.Forms.TextBox BackupSource;
        private System.Windows.Forms.Label Prompt4;
        private System.Windows.Forms.Button BrowseBackupSource;
        private System.Windows.Forms.Button BrowseBackupLocation;
        private System.Windows.Forms.Label Prompt5;
        private System.Windows.Forms.TextBox BackupLocation;
        private System.Windows.Forms.NumericUpDown BackupNumber;
        private System.Windows.Forms.Label Prompt7;
        private System.Windows.Forms.Button GenSettings;
        private System.Windows.Forms.Label Prompt10;
        private System.Windows.Forms.ComboBox ConsoleColorCbo;
        private System.Windows.Forms.ComboBox CompressionLevelCbo;
        private System.Windows.Forms.Label Prompt9;
        private System.Windows.Forms.CheckBox ShowRamCpuUsage;
    }
}