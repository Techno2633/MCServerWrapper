using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Globalization;

namespace MCServerWrapper.Forms
{
    public partial class ProcessInfoForm : Form
    {
        private static List<float> MemList;
        private static int MaxRam;
        //private static int[] XValues;

        private static List<float> CpuList;

        public ProcessInfoForm(int maxRam)
        {
            InitializeComponent();

            MaxRam = maxRam;

            MemList = new List<float>();
            MemList.Insert(0, 0);

            MemChart.ChartAreas[0].AxisX.Interval = 10;
            MemChart.ChartAreas[0].AxisX.Minimum = 0;
            MemChart.ChartAreas[0].AxisX.Maximum = 60;
            MemChart.ChartAreas[0].AxisX.IsReversed = true;
            MemChart.ChartAreas[0].BackColor = Color.White;
            MemChart.ChartAreas[0].AxisX.InterlacedColor = Color.White;
            MemChart.ChartAreas[0].AxisX.Title = "Seconds";
            MemChart.ChartAreas[0].AxisX.TitleAlignment = StringAlignment.Center;
            MemChart.ChartAreas[0].AxisX.MajorTickMark.LineColor = Color.White;

            MemChart.ChartAreas[0].AxisY.Minimum = 0;
            MemChart.ChartAreas[0].AxisY.Maximum = 100;
            MemChart.ChartAreas[0].AxisY.Interval = 10;
            MemChart.ChartAreas[0].AxisY.IntervalOffset = 10;
            MemChart.ChartAreas[0].AxisY.Title = "Percent";
            MemChart.ChartAreas[0].AxisY.TitleAlignment = StringAlignment.Center;
            MemChart.ChartAreas[0].AxisY.MajorTickMark.LineColor = Color.White;

            Title ramTitle = new Title("RAM Usage", Docking.Top);
            MemChart.Titles.Add(ramTitle);

            MemChart.Series[0].Color = Color.FromArgb(49, 130, 187);
            MemChart.Series[0].ChartType = SeriesChartType.Line;
            MemChart.Series[0].BorderWidth = 1;

            //MemChart.Legends.Add(new Legend());
            //MemChart.Legends[0].CellColumns.Add(new LegendCellColumn("RAM", LegendCellColumnType.SeriesSymbol, "RAM Usage", ContentAlignment.MiddleRight));

            //MemChart.Series.Add(new Series("PeakRAM"));

            //MemChart.Legends.Add(new Legend("PeakRAM"));

            //MemChart.Series["PeakRAM"].Legend = "PeakRAM";
            //MemChart.Series["PeakRAM"].IsVisibleInLegend = true;

            MemChart.ChartAreas[0].AxisX.StripLines.Add(new StripLine() { Interval = 10, BorderColor = Color.LightGray });
            MemChart.ChartAreas[0].AxisY.StripLines.Add(new StripLine() { Interval = 10, BorderColor = Color.LightGray });

            //MemChart.Legends.Add(new Legend("CurrentRAM"));

            //MemChart.Series[0].Legend = "CurrentRAM";
            //MemChart.Series[0].IsVisibleInLegend = true;

            CpuList = new List<float>();

            CpuChart.ChartAreas[0].AxisX.Interval = 10;
            CpuChart.ChartAreas[0].AxisX.Minimum = 0;
            CpuChart.ChartAreas[0].AxisX.Maximum = 60;
            CpuChart.ChartAreas[0].AxisX.IsReversed = true;
            CpuChart.ChartAreas[0].BackColor = Color.White;
            CpuChart.ChartAreas[0].AxisX.InterlacedColor = Color.White;
            CpuChart.ChartAreas[0].AxisX.Title = "Seconds";
            CpuChart.ChartAreas[0].AxisX.TitleAlignment = StringAlignment.Center;
            CpuChart.ChartAreas[0].AxisX.MajorTickMark.LineColor = Color.White;

            CpuChart.ChartAreas[0].AxisY.Minimum = 0;
            CpuChart.ChartAreas[0].AxisY.Maximum = 100;
            CpuChart.ChartAreas[0].AxisY.Interval = 10;
            CpuChart.ChartAreas[0].AxisY.IntervalOffset = 10;
            CpuChart.ChartAreas[0].AxisY.Title = "Percent";
            CpuChart.ChartAreas[0].AxisY.TitleAlignment = StringAlignment.Center;
            CpuChart.ChartAreas[0].AxisY.MajorTickMark.LineColor = Color.White;

            Title cpuTitle = new Title("CPU Usage", Docking.Top);
            CpuChart.Titles.Add(cpuTitle);

            CpuChart.Series[0].Color = Color.FromArgb(49, 163, 84);
            CpuChart.Series[0].ChartType = SeriesChartType.Line;
            CpuChart.Series[0].BorderWidth = 1;

            CpuChart.ChartAreas[0].AxisX.StripLines.Add(new StripLine() { Interval = 10, BorderColor = Color.LightGray });
            CpuChart.ChartAreas[0].AxisY.StripLines.Add(new StripLine() { Interval = 10, BorderColor = Color.LightGray });
        }

        public void UpdateCpuChart(float cpu)
        {
            if (CpuChart.InvokeRequired)
            {
                CpuChart.Invoke((MethodInvoker)(() => CpuList.Insert(0, (cpu / Environment.ProcessorCount))));

                while (CpuList.Count > 61)
                {
                    CpuChart.Invoke((MethodInvoker)(() => CpuList.RemoveAt(61)));
                }

                int[] XValues = new int[CpuList.Count];

                for (int i = 0; i < XValues.Length; i++)
                    XValues[i] = i;

                CpuChart.Invoke((MethodInvoker)(() => CpuChart.Series[0].Points.DataBindXY(XValues, CpuList)));
            }
            else
            {
                CpuList.Insert(0, (cpu / Environment.ProcessorCount));

                while (CpuList.Count > 61)
                {
                    CpuList.RemoveAt(61);
                }

                int[] XValues = new int[CpuList.Count];

                for (int i = 0; i < XValues.Length; i++)
                    XValues[i] = i;

                CpuChart.Series[0].Points.DataBindXY(XValues, CpuList);
            }

            if (CpuLbl.InvokeRequired)
            {
                CpuLbl.Invoke((MethodInvoker)(() => CpuLbl.Text = $"CPU Usage: {(cpu / Environment.ProcessorCount).ToString("0.0")}%"));
            }
            else
            {
                CpuLbl.Text = $"CPU Usage: {(cpu / Environment.ProcessorCount).ToString("0.0")}%";
            }
        }

        public void UpdateMemoryChart(float memory)
        {
            if (MemChart.InvokeRequired)
            {
                MemChart.Invoke((MethodInvoker)(() => MemList.Insert(0, ((memory / 1024 / 1024) / (MaxRam)) * 100)));

                while (MemList.Count > 61)
                {
                    MemChart.Invoke((MethodInvoker)(() => MemList.RemoveAt(61)));
                }

                int[] XValues = new int[MemList.Count];

                for (int i = 0; i < XValues.Length; i++)
                    XValues[i] = i;

                MemChart.Invoke((MethodInvoker)(() => MemChart.Series[0].Points.DataBindXY(XValues, MemList)));
            }
            else
            {
                MemList.Insert(0, ((memory / 1024 / 1024) / (MaxRam)) * 100);

                while (MemList.Count > 60)
                {
                    MemList.RemoveAt(60);
                }

                int[] XValues = new int[MemList.Count];

                for (int i = 0; i < XValues.Length; i++)
                    XValues[i] = i;

                MemChart.Series[0].Points.DataBindXY(XValues, MemList);
            }

            if (RamLbl.InvokeRequired)
            {
                RamLbl.Invoke((MethodInvoker)(() => RamLbl.Text = $"RAM Usage: {(memory / 1024 / 1024).ToString("0.0")} MB ({(((memory / 1024 / 1024) / (MaxRam)) * 100).ToString("0.0")}%)"));
            }
            else
            {
                RamLbl.Text = $"RAM Usage: {(memory / 1024 / 1024).ToString("0.0")} MB ({(((memory / 1024 / 1024) / (MaxRam)) * 100).ToString("0.0")}%)";
            }
        }

        private void ProcessInfoForm_Load(object sender, EventArgs e)
        {

        }

        private void ProcessInfoForm_Resize(object sender, EventArgs e)
        {
            //GpuGbx.Size = new Size(GpuGbx.Size.Width, (int)(this.Size.Height * .4448));
            //CpuGbx.Location = new Point(CpuGbx.Location.X, (int)((this.Size.Height * .4448) + 6));
            //CpuGbx.Size = new Size(CpuGbx.Size.Width, (int)(this.Size.Height * .4448));

            GpuGbx.Size = new Size(GpuGbx.Size.Width, (int)(this.ClientSize.Height * .5) - 3 - GpuGbx.Location.Y);
            CpuGbx.Location = new Point(CpuGbx.Location.X, (int)((this.ClientSize.Height * .5) + 3));
            CpuGbx.Size = new Size(CpuGbx.Size.Width, (int)((this.ClientSize.Height)-((this.ClientSize.Height * .5) + 3) - 10));
            //CpuGbx.Size = new Size(CpuGbx.Size.Width, (int)((this.Size.Height * .4448) - 3));
        }

        //public void updatelbl1(string value)
        //{
        //    if (label1.InvokeRequired)
        //        label1.Invoke((MethodInvoker)(() => label1.Text = value));
        //    else
        //        label1.Text = value;
        //}

        //public void updatelbl2(string value)
        //{
        //    if (label2.InvokeRequired)
        //        label2.Invoke((MethodInvoker)(() => label2.Text = value));
        //    else
        //        label2.Text = value;
        //}

        //public void updatelbl3(string value)
        //{
        //    if (label3.InvokeRequired)
        //        label3.Invoke((MethodInvoker)(() => label3.Text = value));
        //    else
        //        label3.Text = value;
        //}

        //public void updatelbl4(string value)
        //{
        //    if (label4.InvokeRequired)
        //        label4.Invoke((MethodInvoker)(() => label4.Text = value));
        //    else
        //        label4.Text = value;
        //}

        //public void updatelbl5(string value)
        //{
        //    if (label5.InvokeRequired)
        //        label5.Invoke((MethodInvoker)(() => label5.Text = value));
        //    else
        //        label5.Text = value;
        //}
    }
}
