using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MCServerWrapper.Forms
{
    public partial class ProcessInfoForm : Form
    {
        private Process process;
        private Timer timer;

        //private string processName;

        private readonly PerformanceCounter PrivilegedTime;
        private readonly PerformanceCounter ProcessorTime;
        private readonly PerformanceCounter UserTime;
        private readonly PerformanceCounter CreatingProcessID;
        private readonly PerformanceCounter ElapsedTime;
        private readonly PerformanceCounter HandleCount;
        private readonly PerformanceCounter IDProcess;
        private readonly PerformanceCounter IODataBytes;
        private readonly PerformanceCounter IOOtherBytes;
        private readonly PerformanceCounter IOReadBytes;
        private readonly PerformanceCounter IOWriteBytes;
        private readonly PerformanceCounter PageFaults;
        private readonly PerformanceCounter PageFileBytes;
        private readonly PerformanceCounter PageFileBytesPeak;
        private readonly PerformanceCounter PoolNonpagedBytes;
        private readonly PerformanceCounter PoolPagedBytes;
        private readonly PerformanceCounter PriorityBase;
        private readonly PerformanceCounter PrivateBytes;
        private readonly PerformanceCounter ThreadCount;
        private readonly PerformanceCounter VirtualBytes;
        private readonly PerformanceCounter VirtualBytesPeak;
        private readonly PerformanceCounter WorkingSet;
        private readonly PerformanceCounter WorkingSetPrivate;
        private readonly PerformanceCounter WorkingSetPeak;

        private const int LIST_MAX = 181; // this value is one greater than the maximum of the charts and the length of the lists that display the data
        private int[] XValues;

        private List<float> OverviewProcessorTimeList;
        private List<float> OverviewPrivilegedTimeList;
        private List<float> OverviewUserTimeList;
        private List<float> OverviewWorkingSetPrivateList;
        private List<float> OverviewWorkingSetPeakList;
        private List<float> OverviewWorkingSetList;
        private List<float> OverviewIOReadList;
        private List<float> OverviewIOWriteList;

        private List<float> MemoryWorkingSetPrivateList;
        private List<float> MemoryWorkingSetPeakList;
        private List<float> MemoryWorkingSetList;
        private List<float> MemoryPageFileList;
        private List<float> MemoryPageFilePeakList;
        private List<float> MemoryVirtualList;
        private List<float> MemoryVirtualPeakList;
        private List<float> MemoryPoolPagedList;
        private List<float> MemoryPoolNonpagedList;

        private List<float> IOReadList;
        private List<float> IOWriteList;
        private List<float> IODataList;
        private List<float> IOOtherList;

        public ProcessInfoForm(Process process)
        {
            InitializeComponent();

            OverviewCpuChart.ChartAreas[0].AxisX.Maximum = LIST_MAX - 1;
            OverviewMemChart.ChartAreas[0].AxisX.Maximum = LIST_MAX - 1;
            OverviewIoChart.ChartAreas[0].AxisX.Maximum = LIST_MAX - 1;
            MemoryWorkingSetChart.ChartAreas[0].AxisX.Maximum = LIST_MAX - 1;
            MemoryPageFileChart.ChartAreas[0].AxisX.Maximum = LIST_MAX - 1;
            MemoryVirtualChart.ChartAreas[0].AxisX.Maximum = LIST_MAX - 1;
            MemoryPoolChart.ChartAreas[0].AxisX.Maximum = LIST_MAX - 1;
            IOReadWriteChart.ChartAreas[0].AxisX.Maximum = LIST_MAX - 1;
            IODataOtherChart.ChartAreas[0].AxisX.Maximum = LIST_MAX - 1;

            this.timer = new Timer()
            {
                Interval = 1000,
            };

            timer.Tick += Timer_Tick;

            this.process = process;

            if (process.HasExited)
                return;

            string processName = this.process.ProcessName;

            PrivilegedTime = new PerformanceCounter("Process", "% Privileged Time", processName, true);
            ProcessorTime = new PerformanceCounter("Process", "% Processor Time", processName, true);
            UserTime = new PerformanceCounter("Process", "% User Time", processName, true);
            CreatingProcessID = new PerformanceCounter("Process", "Creating Process ID", processName, true);
            ElapsedTime = new PerformanceCounter("Process", "Elapsed Time", processName, true);
            HandleCount = new PerformanceCounter("Process", "Handle Count", processName, true);
            IDProcess = new PerformanceCounter("Process", "ID Process", processName, true);
            IODataBytes = new PerformanceCounter("Process", "IO Data Bytes/sec", processName, true);
            IOOtherBytes = new PerformanceCounter("Process", "IO Other Bytes/sec", processName, true);
            IOReadBytes = new PerformanceCounter("Process", "IO Read Bytes/sec", processName, true);
            IOWriteBytes = new PerformanceCounter("Process", "IO Write Bytes/sec", processName, true);
            PageFaults = new PerformanceCounter("Process", "Page Faults/sec", processName, true);
            PageFileBytes = new PerformanceCounter("Process", "Page File Bytes", processName, true);
            PageFileBytesPeak = new PerformanceCounter("Process", "Page File Bytes Peak", processName, true);
            PoolNonpagedBytes = new PerformanceCounter("Process", "Pool Nonpaged Bytes", processName, true);
            PoolPagedBytes = new PerformanceCounter("Process", "Pool Paged Bytes", processName, true);
            PriorityBase = new PerformanceCounter("Process", "Priority Base", processName, true);
            PrivateBytes = new PerformanceCounter("Process", "Private Bytes", processName, true);
            ThreadCount = new PerformanceCounter("Process", "Thread Count", processName, true);
            VirtualBytes = new PerformanceCounter("Process", "Virtual Bytes", processName, true);
            VirtualBytesPeak = new PerformanceCounter("Process", "Virtual Bytes Peak", processName, true);
            WorkingSet = new PerformanceCounter("Process", "Working Set", processName, true);
            WorkingSetPrivate = new PerformanceCounter("Process", "Working Set - Private", processName, true);
            WorkingSetPeak = new PerformanceCounter("Process", "Working Set Peak", processName, true);

            float[] filler = new float[LIST_MAX];

            for (int i = 0; i < filler.Length; i++)
                filler[i] = 0;

            OverviewProcessorTimeList = new List<float>(filler);
            OverviewPrivilegedTimeList = new List<float>(filler);
            OverviewUserTimeList = new List<float>(filler);
            OverviewWorkingSetPrivateList = new List<float>(filler);
            OverviewWorkingSetPeakList = new List<float>(filler);
            OverviewWorkingSetList = new List<float>(filler);
            OverviewIOReadList = new List<float>(filler);
            OverviewIOWriteList = new List<float>(filler);

            MemoryWorkingSetPrivateList = new List<float>(filler);
            MemoryWorkingSetPeakList = new List<float>(filler);
            MemoryWorkingSetList = new List<float>(filler);
            MemoryPageFileList = new List<float>(filler);
            MemoryPageFilePeakList = new List<float>(filler);
            MemoryVirtualList = new List<float>(filler);
            MemoryVirtualPeakList = new List<float>(filler);
            MemoryPoolPagedList = new List<float>(filler);
            MemoryPoolNonpagedList = new List<float>(filler);

            IOReadList = new List<float>(filler);
            IOWriteList = new List<float>(filler);
            IODataList = new List<float>(filler);
            IOOtherList = new List<float>(filler);

            XValues = new int[LIST_MAX];

            for (int i = 0; i < XValues.Length; i++)
                XValues[i] = i;

            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (process.HasExited)
                return;

            if (this.IsDisposed)
                return;

            try
            {
                float privilegedTime = PrivilegedTime.NextValue();//
                float processorTime = ProcessorTime.NextValue();//
                float userTime = UserTime.NextValue();//
                float creatingProcessID = CreatingProcessID.NextValue();
                float elapsedTime = ElapsedTime.NextValue();
                float handleCount = HandleCount.NextValue();
                float idProcess = IDProcess.NextValue();
                float ioDataBytes = IODataBytes.NextValue();//
                float ioOtherBytes = IOOtherBytes.NextValue();//
                float ioReadBytes = IOReadBytes.NextValue();//
                float ioWriteBytes = IOWriteBytes.NextValue();//
                float pageFaults = PageFaults.NextValue();
                float pageFileBytes = PageFileBytes.NextValue();//
                float pageFileBytesPeak = PageFileBytesPeak.NextValue();//
                float poolNonpagedBytes = PoolNonpagedBytes.NextValue();//
                float poolPagedBytes = PoolPagedBytes.NextValue();//
                float priorityBase = PriorityBase.NextValue();
                float privateBytes = PrivateBytes.NextValue();
                float threadCount = ThreadCount.NextValue();
                float virtualBytes = VirtualBytes.NextValue();//
                float virtualBytesPeak = VirtualBytesPeak.NextValue();//
                float workingSet = WorkingSet.NextValue();//
                float workingSetPrivate = WorkingSetPrivate.NextValue();//
                float workingSetPeak = WorkingSetPeak.NextValue();//

                UpdateOverviewCPU(processorTime / Environment.ProcessorCount / 100f, privilegedTime / Environment.ProcessorCount / 100f, userTime / Environment.ProcessorCount / 100f);
                UpdateOverviewMemory(workingSetPrivate / 1048576, workingSetPeak / 1048576, workingSet / 1048576);
                UpdateOverviewIO(ioReadBytes / 1024, ioWriteBytes / 1024);
                UpdateOverviewInfo(creatingProcessID, elapsedTime, handleCount, idProcess, pageFaults, priorityBase, privateBytes, threadCount);

                UpdateMemoryWorkingSet(workingSetPrivate / 1048576, workingSetPeak / 1048576, workingSet / 1048576);
                UpdateMemoryPageFile(pageFileBytes / 1048576, pageFileBytesPeak / 1048576);
                UpdateMemoryVirtual(virtualBytes / 1048576, virtualBytesPeak / 1048576);
                UpdateMemoryPool(poolPagedBytes / 1024, poolNonpagedBytes / 1024);

                UpdateIOReadWrite(ioReadBytes / 1024, ioWriteBytes / 1024);
                UpdateIODataOther(ioDataBytes / 1024, ioOtherBytes / 1024);
            }
            catch
            { }
        }

        private void UpdateOverviewCPU(float processorTime, float privilegedTime, float userTime)
        {
            //% Processor Time
            OverviewProcessorTimeList.Insert(0, processorTime);

            while (OverviewProcessorTimeList.Count > LIST_MAX)
                OverviewProcessorTimeList.RemoveAt(LIST_MAX);

            OverviewCpuChart.Series[0].Points.DataBindXY(XValues, OverviewProcessorTimeList);
            OverviewCpuUsageLbl.Text = "Total CPU Usage: " + (int)(processorTime * 100) + "%";
            OverviewCpuPrivilegedUsageLbl.Text = "Privileged CPU Usage: " + (int)(privilegedTime * 100) + "%";
            OverviewCpuUserUsageLbl.Text = "User CPU Usage: " + (int)(userTime * 100) + "%";

            ////% Privileged Time
            //OverviewPrivilegedTimeList.Insert(0, privilegedTime);

            //while (OverviewPrivilegedTimeList.Count > LIST_MAX)
            //    OverviewPrivilegedTimeList.RemoveAt(LIST_MAX);

            //OverviewCpuChart.Series[1].Points.DataBindXY(XValues, OverviewPrivilegedTimeList);

            ////% User Time
            //OverviewUserTimeList.Insert(0, userTime);

            //while (OverviewUserTimeList.Count > LIST_MAX)
            //    OverviewUserTimeList.RemoveAt(LIST_MAX);

            //OverviewCpuChart.Series[2].Points.DataBindXY(XValues, OverviewUserTimeList);
        }

        private void UpdateOverviewMemory(float workingSetPrivate, float workingSetPeak, float workingSet)
        {
            //Private Working Set
            OverviewWorkingSetPrivateList.Insert(0, workingSetPrivate);

            while (OverviewWorkingSetPrivateList.Count > LIST_MAX)
                OverviewWorkingSetPrivateList.RemoveAt(LIST_MAX);

            OverviewMemChart.Series[2].Points.DataBindXY(XValues, OverviewWorkingSetPrivateList);
            OverviewMemoryWSPrivateLbl.Text = "Private Working Set: " + (int)workingSetPrivate + "MB";

            //Peak Working Set
            OverviewWorkingSetPeakList.Insert(0, workingSetPeak);

            while (OverviewWorkingSetPeakList.Count > LIST_MAX)
                OverviewWorkingSetPeakList.RemoveAt(LIST_MAX);

            OverviewMemChart.Series[1].Points.DataBindXY(XValues, OverviewWorkingSetPeakList);
            OverviewMemoryWSPeakLbl.Text = "Working Set Peak: " + (int)workingSetPeak + "MB";

            //Working Set
            OverviewWorkingSetList.Insert(0, workingSet);

            while (OverviewWorkingSetList.Count > LIST_MAX)
                OverviewWorkingSetList.RemoveAt(LIST_MAX);

            OverviewMemChart.Series[0].Points.DataBindXY(XValues, OverviewWorkingSetList);
            OverviewMemoryWSLbl.Text = "Working Set: " + (int)workingSet + "MB";
        }

        private void UpdateOverviewIO(float read, float write)
        {
            //Read Bytes
            OverviewIOReadList.Insert(0, read);

            while (OverviewIOReadList.Count > LIST_MAX)
                OverviewIOReadList.RemoveAt(LIST_MAX);

            OverviewIoChart.Series[0].Points.DataBindXY(XValues, OverviewIOReadList);
            OverviewIoReadLbl.Text = "Read: " + (int)read + "KB";

            //Write Bytes
            OverviewIOWriteList.Insert(0, write);

            while (OverviewIOWriteList.Count > LIST_MAX)
                OverviewIOWriteList.RemoveAt(LIST_MAX);

            OverviewIoChart.Series[1].Points.DataBindXY(XValues, OverviewIOWriteList);
            OverviewIoWriteLbl.Text = "Write: " + (int)write + "KB";
        }

        private void UpdateOverviewInfo(float creatingProcessID, float elapsedTime, float handleCount, float idProcess, float pageFaults, float priorityBase, float privateBytes, float threadCount)
        {
            OverviewInfoCreatingProcessIDLbl.Text = "Creating Process ID: " + creatingProcessID;
            OverviewInfoElapsedTimeLbl.Text = "Elapsed Time: " + elapsedTime;
            OverviewInfoHandleCountLbl.Text = "Handle Count: " + handleCount;
            OverviewInfoIDProcessLbl.Text = "Process ID: " + idProcess;
            OverviewInfoPageFaultsLbl.Text = "Page Faults: " + pageFaults;
            OverviewInfoPriorityBaseLbl.Text = "Priority Base: " + priorityBase;
            OverviewInfoPrivateBytesLbl.Text = "Private Bytes: " + privateBytes.ToString("f");
            OverviewInfoThreadCountLbl.Text = "Thread Count: " + threadCount;
        }

        private void UpdateMemoryWorkingSet(float workingSetPrivate, float workingSetPeak, float workingSet)
        {
            //Private Working Set
            MemoryWorkingSetPrivateList.Insert(0, workingSetPrivate);

            while (MemoryWorkingSetPrivateList.Count > LIST_MAX)
                MemoryWorkingSetPrivateList.RemoveAt(LIST_MAX);

            MemoryWorkingSetChart.Series[0].Points.DataBindXY(XValues, MemoryWorkingSetPrivateList);
            MemoryWSPrivateLbl.Text = "Private Working Set: " + (int)workingSetPrivate + "MB";

            //Peak Working Set
            MemoryWorkingSetPeakList.Insert(0, workingSetPeak);

            while (MemoryWorkingSetPeakList.Count > LIST_MAX)
                MemoryWorkingSetPeakList.RemoveAt(LIST_MAX);

            MemoryWorkingSetChart.Series[1].Points.DataBindXY(XValues, MemoryWorkingSetPeakList);
            MemoryWSPeakLbl.Text = "Working Set Peak: " + (int)workingSetPeak + "MB";

            //Working Set
            MemoryWorkingSetList.Insert(0, workingSet);

            while (MemoryWorkingSetList.Count > LIST_MAX)
                MemoryWorkingSetList.RemoveAt(LIST_MAX);

            MemoryWorkingSetChart.Series[2].Points.DataBindXY(XValues, MemoryWorkingSetList);
            MemoryWSLbl.Text = "Working Set: " + (int)workingSet + "MB";
        }

        private void UpdateMemoryPageFile(float pageFile, float pageFilePeak)
        {
            //Page File
            MemoryPageFileList.Insert(0, pageFile);

            while (MemoryPageFileList.Count > LIST_MAX)
                MemoryPageFileList.RemoveAt(LIST_MAX);

            MemoryPageFileChart.Series[0].Points.DataBindXY(XValues, MemoryPageFileList);
            MemoryPFLbl.Text = "Page File: " + (int)pageFile + "MB";

            //Page File Peak
            MemoryPageFilePeakList.Insert(0, pageFilePeak);

            while (MemoryPageFilePeakList.Count > LIST_MAX)
                MemoryPageFilePeakList.RemoveAt(LIST_MAX);

            MemoryPageFileChart.Series[1].Points.DataBindXY(XValues, MemoryPageFilePeakList);
            MemoryPFPeakLbl.Text = "Page File Peak: " + (int)pageFilePeak + "MB";
        }

        private void UpdateMemoryVirtual(float virtualBytes, float virtualBytesPeak)
        {
            //Virtual Bytes
            MemoryVirtualList.Insert(0, virtualBytes);

            while (MemoryVirtualList.Count > LIST_MAX)
                MemoryVirtualList.RemoveAt(LIST_MAX);

            MemoryVirtualChart.Series[0].Points.DataBindXY(XValues, MemoryVirtualList);
            MemoryVirtualLbl.Text = "Virtual: " + (int)virtualBytes + "MB";

            //Vitual Bytes Peak
            MemoryVirtualPeakList.Insert(0, virtualBytesPeak);

            while (MemoryVirtualPeakList.Count > LIST_MAX)
                MemoryVirtualPeakList.RemoveAt(LIST_MAX);

            MemoryVirtualChart.Series[1].Points.DataBindXY(XValues, MemoryVirtualPeakList);
            MemoryVirtualPeakLbl.Text = "Virtual Peak: " + (int)virtualBytesPeak + "MB";
        }

        private void UpdateMemoryPool(float poolPaged, float poolNonpaged)
        {
            //Pool Paged
            MemoryPoolPagedList.Insert(0, poolPaged);

            while (MemoryPoolPagedList.Count > LIST_MAX)
                MemoryPoolPagedList.RemoveAt(LIST_MAX);

            MemoryPoolChart.Series[0].Points.DataBindXY(XValues, MemoryPoolPagedList);
            MemoryPoolPagedLbl.Text = "Paged Pool: " + (int)poolPaged + "KB";

            //Pool Nonpaged
            MemoryPoolNonpagedList.Insert(0, poolNonpaged);

            while (MemoryPoolNonpagedList.Count > LIST_MAX)
                MemoryPoolNonpagedList.RemoveAt(LIST_MAX);

            MemoryPoolChart.Series[1].Points.DataBindXY(XValues, MemoryPoolNonpagedList);
            MemoryPoolNonpagedLbl.Text = "Nonpaged Pool: " + (int)poolNonpaged + "KB";
        }

        private void UpdateIOReadWrite(float read, float write)
        {
            //Read Bytes
            IOReadList.Insert(0, read);

            while (IOReadList.Count > LIST_MAX)
                IOReadList.RemoveAt(LIST_MAX);

            IOReadWriteChart.Series[0].Points.DataBindXY(XValues, IOReadList);
            IOReadLbl.Text = "Read: " + (int)read + "KB";

            //Write Bytes
            IOWriteList.Insert(0, write);

            while (IOWriteList.Count > LIST_MAX)
                IOWriteList.RemoveAt(LIST_MAX);

            IOReadWriteChart.Series[1].Points.DataBindXY(XValues, IOWriteList);
            IOWriteLbl.Text = "Write: " + (int)write + "KB";
        }

        private void UpdateIODataOther(float data, float other)
        {
            //Data Bytes
            IODataList.Insert(0, data);

            while (IODataList.Count > LIST_MAX)
                IODataList.RemoveAt(LIST_MAX);

            IODataOtherChart.Series[0].Points.DataBindXY(XValues, IODataList);
            IODataLbl.Text = "Data: " + (int)data + "KB";

            //Other Bytes
            IOOtherList.Insert(0, other);

            while (IOOtherList.Count > LIST_MAX)
                IOOtherList.RemoveAt(LIST_MAX);

            IODataOtherChart.Series[1].Points.DataBindXY(XValues, IOOtherList);
            IOOtherLbl.Text = "Other: " + (int)other + "KB";
        }

        private void ProcessInfoForm_Resize(object sender, EventArgs e)
        {
            TabControl.Size = this.ClientSize;
        }

        private void HorizontalSplitContainer_SplitterMoved(object sender, SplitterEventArgs e)
        {
            OverviewTabPage.Focus();
        }

        private void LeftHorizontalSplitContainer_SplitterMoved(object sender, SplitterEventArgs e)
        {
            OverviewTabPage.Focus();
        }

        private void RightHorizontalSplitContainer_SplitterMoved(object sender, SplitterEventArgs e)
        {
            OverviewTabPage.Focus();
        }

        private void MemoryVerticalSplitContainer_SplitterMoved(object sender, SplitterEventArgs e)
        {
            MemoryTabPage.Focus();
        }

        private void MemoryLeftHorizontalSplitContainer_SplitterMoved(object sender, SplitterEventArgs e)
        {
            MemoryTabPage.Focus();
        }

        private void MemoryRightHorizontalSplitContainer_SplitterMoved(object sender, SplitterEventArgs e)
        {
            MemoryTabPage.Focus();
        }

        private void IOVerticalSplitContainer_SplitterMoved(object sender, SplitterEventArgs e)
        {
            IOTabPage.Focus();
        }

        private void ProcessInfoForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            PrivilegedTime.Dispose();
            ProcessorTime.Dispose();
            UserTime.Dispose();
            CreatingProcessID.Dispose();
            ElapsedTime.Dispose();
            HandleCount.Dispose();
            IDProcess.Dispose();
            IODataBytes.Dispose();
            IOOtherBytes.Dispose();
            IOReadBytes.Dispose();
            IOWriteBytes.Dispose();
            PageFaults.Dispose();
            PageFileBytes.Dispose();
            PageFileBytesPeak.Dispose();
            PoolNonpagedBytes.Dispose();
            PoolPagedBytes.Dispose();
            PriorityBase.Dispose();
            PrivateBytes.Dispose();
            ThreadCount.Dispose();
            VirtualBytes.Dispose();
            VirtualBytesPeak.Dispose();
            WorkingSet.Dispose();
            WorkingSetPrivate.Dispose();
            WorkingSetPeak.Dispose();
        }
    }
}
