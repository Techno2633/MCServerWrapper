namespace MCServerWrapper.Forms
{
    partial class ProcessInfoForm
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.MemChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.RamLbl = new System.Windows.Forms.Label();
            this.CpuLbl = new System.Windows.Forms.Label();
            this.CpuGbx = new System.Windows.Forms.GroupBox();
            this.CpuChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.GpuGbx = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.MemChart)).BeginInit();
            this.CpuGbx.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CpuChart)).BeginInit();
            this.GpuGbx.SuspendLayout();
            this.SuspendLayout();
            // 
            // MemChart
            // 
            this.MemChart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            chartArea1.Name = "ChartArea1";
            this.MemChart.ChartAreas.Add(chartArea1);
            this.MemChart.Location = new System.Drawing.Point(6, 19);
            this.MemChart.Name = "MemChart";
            series1.ChartArea = "ChartArea1";
            series1.Name = "Series1";
            this.MemChart.Series.Add(series1);
            this.MemChart.Size = new System.Drawing.Size(426, 237);
            this.MemChart.TabIndex = 5;
            this.MemChart.Text = "chart1";
            // 
            // RamLbl
            // 
            this.RamLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.RamLbl.AutoSize = true;
            this.RamLbl.Location = new System.Drawing.Point(6, 259);
            this.RamLbl.Name = "RamLbl";
            this.RamLbl.Size = new System.Drawing.Size(71, 13);
            this.RamLbl.TabIndex = 6;
            this.RamLbl.Text = "RAM Usage: ";
            // 
            // CpuLbl
            // 
            this.CpuLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CpuLbl.AutoSize = true;
            this.CpuLbl.Location = new System.Drawing.Point(6, 259);
            this.CpuLbl.Name = "CpuLbl";
            this.CpuLbl.Size = new System.Drawing.Size(66, 13);
            this.CpuLbl.TabIndex = 7;
            this.CpuLbl.Text = "CPU Usage:";
            // 
            // CpuGbx
            // 
            this.CpuGbx.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CpuGbx.Controls.Add(this.CpuChart);
            this.CpuGbx.Controls.Add(this.CpuLbl);
            this.CpuGbx.Location = new System.Drawing.Point(12, 296);
            this.CpuGbx.Name = "CpuGbx";
            this.CpuGbx.Size = new System.Drawing.Size(438, 278);
            this.CpuGbx.TabIndex = 8;
            this.CpuGbx.TabStop = false;
            this.CpuGbx.Text = "CPU Info";
            // 
            // CpuChart
            // 
            this.CpuChart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            chartArea2.Name = "ChartArea1";
            this.CpuChart.ChartAreas.Add(chartArea2);
            this.CpuChart.Location = new System.Drawing.Point(6, 19);
            this.CpuChart.Name = "CpuChart";
            series2.ChartArea = "ChartArea1";
            series2.Name = "Series1";
            this.CpuChart.Series.Add(series2);
            this.CpuChart.Size = new System.Drawing.Size(426, 237);
            this.CpuChart.TabIndex = 8;
            this.CpuChart.Text = "chart1";
            // 
            // GpuGbx
            // 
            this.GpuGbx.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GpuGbx.Controls.Add(this.MemChart);
            this.GpuGbx.Controls.Add(this.RamLbl);
            this.GpuGbx.Location = new System.Drawing.Point(12, 12);
            this.GpuGbx.Name = "GpuGbx";
            this.GpuGbx.Size = new System.Drawing.Size(438, 278);
            this.GpuGbx.TabIndex = 9;
            this.GpuGbx.TabStop = false;
            this.GpuGbx.Text = "RAM Info";
            // 
            // ProcessInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(462, 586);
            this.ControlBox = false;
            this.Controls.Add(this.GpuGbx);
            this.Controls.Add(this.CpuGbx);
            this.Name = "ProcessInfoForm";
            this.Text = "Server Info";
            this.Load += new System.EventHandler(this.ProcessInfoForm_Load);
            this.Resize += new System.EventHandler(this.ProcessInfoForm_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.MemChart)).EndInit();
            this.CpuGbx.ResumeLayout(false);
            this.CpuGbx.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CpuChart)).EndInit();
            this.GpuGbx.ResumeLayout(false);
            this.GpuGbx.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.DataVisualization.Charting.Chart MemChart;
        private System.Windows.Forms.Label RamLbl;
        private System.Windows.Forms.Label CpuLbl;
        private System.Windows.Forms.GroupBox CpuGbx;
        private System.Windows.Forms.DataVisualization.Charting.Chart CpuChart;
        private System.Windows.Forms.GroupBox GpuGbx;
    }
}