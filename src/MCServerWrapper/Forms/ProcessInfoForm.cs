using MCServerWrapper.Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MCServerWrapper.Forms
{
    public partial class ProcessInfoForm : Form
    {

        private const string FormSettingsPath = @"Wrapper\FormSettings.json";
        private FormSettings Settings;

        private int[] XValues;
        private int ListMax;

        private System.Timers.Timer FormSettingsSaveTimer;

        public ProcessInfoForm(int listMax)
        {
            InitializeComponent();

            FormSettingsSaveTimer = new System.Timers.Timer(600000);
            FormSettingsSaveTimer.Elapsed += (s, e) =>
            {
                UpdateFormSettings();
            };

            ListMax = listMax;

            ContextMenu OverviewCpuCM = new ContextMenu(new MenuItem[]
            {
                new MenuItem("Resize X-Axis", (s, e) => 
                {
                    int value = new IntegerInput((int)OverviewCpuChart.ChartAreas[0].AxisX.Maximum, ListMax).ShowFormDialog();
                    OverviewCpuChart.ChartAreas[0].AxisX.Maximum = value;
                    Settings.OverviewCpuMax = value;
                })
            });
            OverviewCpuChart.ContextMenu = OverviewCpuCM;

            ContextMenu OverviewMemoryCM = new ContextMenu(new MenuItem[]
            {
                new MenuItem("Resize X-Axis", (s, e) =>
                {
                    int value = new IntegerInput((int)OverviewMemChart.ChartAreas[0].AxisX.Maximum, ListMax).ShowFormDialog();
                    OverviewMemChart.ChartAreas[0].AxisX.Maximum = value;
                    Settings.OverviewMemoryMax = value;
                })
            });
            OverviewMemChart.ContextMenu = OverviewMemoryCM;

            ContextMenu OverviewIOCM = new ContextMenu(new MenuItem[]
            {
                new MenuItem("Resize X-Axis", (s, e) =>
                {
                    int value = new IntegerInput((int)OverviewIoChart.ChartAreas[0].AxisX.Maximum, ListMax).ShowFormDialog();
                    OverviewIoChart.ChartAreas[0].AxisX.Maximum = value;
                    Settings.OverviewIoMax = value;
                })
            });
            OverviewIoChart.ContextMenu = OverviewIOCM;

            ContextMenu MemoryWSCM = new ContextMenu(new MenuItem[]
            {
                new MenuItem("Resize X-Axis", (s, e) =>
                {
                    int value = new IntegerInput((int)MemoryWorkingSetChart.ChartAreas[0].AxisX.Maximum, ListMax).ShowFormDialog();
                    MemoryWorkingSetChart.ChartAreas[0].AxisX.Maximum = value;
                    Settings.MemoryWorkingSetMax = value;
                })
            });
            MemoryWorkingSetChart.ContextMenu = MemoryWSCM;

            ContextMenu MemoryPageFileCM = new ContextMenu(new MenuItem[]
            {
                new MenuItem("Resize X-Axis", (s, e) =>
                {
                    int value = new IntegerInput((int)MemoryPageFileChart.ChartAreas[0].AxisX.Maximum, ListMax).ShowFormDialog();
                    MemoryPageFileChart.ChartAreas[0].AxisX.Maximum = value;
                    Settings.MemoryPageFileMax = value;
                })
            });
            MemoryPageFileChart.ContextMenu = MemoryPageFileCM;

            ContextMenu MemoryVirtualCM = new ContextMenu(new MenuItem[]
            {
                new MenuItem("Resize X-Axis", (s, e) =>
                {
                    int value = new IntegerInput((int)MemoryVirtualChart.ChartAreas[0].AxisX.Maximum, ListMax).ShowFormDialog();
                    MemoryVirtualChart.ChartAreas[0].AxisX.Maximum = value;
                    Settings.MemoryVirtualMax = value;
                })
            });
            MemoryVirtualChart.ContextMenu = MemoryVirtualCM;

            ContextMenu MemoryPoolCM = new ContextMenu(new MenuItem[]
            {
                new MenuItem("Resize X-Axis", (s, e) =>
                {
                    int value = new IntegerInput((int)MemoryPoolChart.ChartAreas[0].AxisX.Maximum, ListMax).ShowFormDialog();
                    MemoryPoolChart.ChartAreas[0].AxisX.Maximum = value;
                    Settings.MemoryPoolMax = value;
                })
            });
            MemoryPoolChart.ContextMenu = MemoryPoolCM;

            ContextMenu IOReadWriteCM = new ContextMenu(new MenuItem[]
            {
                new MenuItem("Resize X-Axis", (s, e) =>
                {
                   int value = new IntegerInput((int)IOReadWriteChart.ChartAreas[0].AxisX.Maximum, ListMax).ShowFormDialog();
                   IOReadWriteChart.ChartAreas[0].AxisX.Maximum = value;
                   Settings.IoReadWriteMax = value;
                })
            });
            IOReadWriteChart.ContextMenu = IOReadWriteCM;

            ContextMenu IODataOtherCM = new ContextMenu(new MenuItem[]
            {
                new MenuItem("Resize X-Axis", (s, e) =>
                {
                    int value = new IntegerInput((int)IODataOtherChart.ChartAreas[0].AxisX.Maximum, ListMax).ShowFormDialog();
                    IODataOtherChart.ChartAreas[0].AxisX.Maximum = value;
                    Settings.IoDataOtherMax = value;
                })
            });
            IODataOtherChart.ContextMenu = IODataOtherCM;

            //OverviewCpuChart.ChartAreas[0].AxisX.Maximum = listMax - 1;
            //OverviewMemChart.ChartAreas[0].AxisX.Maximum = listMax - 1;
            //OverviewIoChart.ChartAreas[0].AxisX.Maximum = listMax - 1;
            //MemoryWorkingSetChart.ChartAreas[0].AxisX.Maximum = listMax - 1;
            //MemoryPageFileChart.ChartAreas[0].AxisX.Maximum = listMax - 1;
            //MemoryVirtualChart.ChartAreas[0].AxisX.Maximum = listMax - 1;
            //MemoryPoolChart.ChartAreas[0].AxisX.Maximum = listMax - 1;
            //IOReadWriteChart.ChartAreas[0].AxisX.Maximum = listMax - 1;
            //IODataOtherChart.ChartAreas[0].AxisX.Maximum = listMax - 1;

            XValues = new int[listMax];

            for (int i = 0; i < XValues.Length; i++)
                XValues[i] = i;

            Exception exception;
            int counter = 0;
            try
            {
                while (!CreateFormSettings(out exception))
                {
                    counter++;
                    if (counter > 4)
                        throw new Exception("Failed to create FormSettings.json after 5 tries", exception);
                }
            }
            catch (Exception ex)
            {
                ExceptionPrinter.PrintException(ex, "Error creating FormSettings.json");
            }

            exception = null;
            counter = 0;
            try
            {
                while(!LoadFormSettings(out exception))
                {
                    counter++;
                    if (counter > 4)
                        throw new Exception("Failed to load form settings from FormSettings.json after 5 tries", exception);
                }
            }
            catch (Exception ex)
            {
                ExceptionPrinter.PrintException(ex, "Error reading settings from FormSettings.json");
            }
        }

        private bool CreateFormSettings(out Exception exception)
        {
            try
            {
                if (!File.Exists(FormSettingsPath))
                {
                    JsonSerializerSettings jsonSettings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto };
                    string json = JsonConvert.SerializeObject(new FormSettings(), Formatting.Indented, jsonSettings);
                    File.WriteAllText(FormSettingsPath, json);
                }
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
            exception = null;
            return true;
        }

        private bool LoadFormSettings(out Exception exception)
        {
            try
            {
                string json = File.ReadAllText(FormSettingsPath);
                Settings = JsonConvert.DeserializeObject<FormSettings>(json);
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
            exception = null;
            return true;
        }

        private bool SaveFormSettings(FormSettings settings, out Exception exception)
        {
            try
            {
                JsonSerializerSettings jsonSettings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto };
                string json = JsonConvert.SerializeObject(settings, Formatting.Indented, jsonSettings);
                File.WriteAllText(FormSettingsPath, json);
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
            exception = null;
            return true;
        }

        private void UpdateFormSettings()
        {
            Exception exception;
            int counter = 0;

            try
            {
                while (!SaveFormSettings(Settings, out exception))
                {
                    counter++;
                    if (counter > 4)
                        throw new Exception("Failed to save to FormSettings.json after 5 tries", exception);
                }
            }
            catch (Exception ex)
            {
                ExceptionPrinter.PrintException(ex, "Error saving settings to FormSettings.json");
            }
        }

        public void UpdateOverviewCPU(float[] processorTime, float[] privilegedTime, float[] userTime)
        {
            if (OverviewCpuChart.InvokeRequired || OverviewCpuUsageLbl.InvokeRequired || OverviewCpuPrivilegedUsageLbl.InvokeRequired || OverviewCpuUserUsageLbl.InvokeRequired)
            {
                OverviewCpuChart.Invoke((MethodInvoker)(() => OverviewCpuChart.Series[0].Points.DataBindXY(XValues, processorTime)));
                OverviewCpuUsageLbl.Invoke((MethodInvoker)(() => OverviewCpuUsageLbl.Text = "Total CPU Usage: " + (int)(processorTime[0] * 100) + "%"));
                OverviewCpuPrivilegedUsageLbl.Invoke((MethodInvoker)(() => OverviewCpuPrivilegedUsageLbl.Text = "Privileged CPU Usage: " + (int)(privilegedTime[0] * 100) + "%"));
                OverviewCpuUserUsageLbl.Invoke((MethodInvoker)(() => OverviewCpuUserUsageLbl.Text = "User CPU Usage: " + (int)(userTime[0] * 100) + "%"));
            }
            else
            {
                OverviewCpuChart.Series[0].Points.DataBindXY(XValues, processorTime);
                OverviewCpuUsageLbl.Text = "Total CPU Usage: " + (int)(processorTime[0] * 100) + "%";
                OverviewCpuPrivilegedUsageLbl.Text = "Privileged CPU Usage: " + (int)(privilegedTime[0] * 100) + "%";
                OverviewCpuUserUsageLbl.Text = "User CPU Usage: " + (int)(userTime[0] * 100) + "%";
            }
        }

        public void UpdateOverviewMemory(float[] workingSetPrivate, float[] workingSetPeak, float[] workingSet)
        {
            if (OverviewMemChart.InvokeRequired || OverviewMemoryWSPrivateLbl.InvokeRequired || OverviewMemoryWSPeakLbl.InvokeRequired || OverviewMemoryWSLbl.InvokeRequired)
            {
                OverviewMemChart.Invoke((MethodInvoker)(() =>
                {
                    OverviewMemChart.Series[2].Points.DataBindXY(XValues, workingSetPrivate);
                    OverviewMemChart.Series[1].Points.DataBindXY(XValues, workingSetPeak);
                    OverviewMemChart.Series[0].Points.DataBindXY(XValues, workingSet);
                }));
                OverviewMemoryWSPrivateLbl.Invoke((MethodInvoker)(() => OverviewMemoryWSPrivateLbl.Text = "Private Working Set: " + (int)workingSetPrivate[0] + "MB"));
                OverviewMemoryWSPeakLbl.Invoke((MethodInvoker)(() => OverviewMemoryWSPeakLbl.Text = "Working Set Peak: " + (int)workingSetPeak[0] + "MB"));
                OverviewMemoryWSLbl.Invoke((MethodInvoker)(() => OverviewMemoryWSLbl.Text = "Working Set: " + (int)workingSet[0] + "MB"));
            }
            else
            {
                OverviewMemChart.Series[2].Points.DataBindXY(XValues, workingSetPrivate);
                OverviewMemChart.Series[1].Points.DataBindXY(XValues, workingSetPeak);
                OverviewMemChart.Series[0].Points.DataBindXY(XValues, workingSet);
                OverviewMemoryWSPrivateLbl.Text = "Private Working Set: " + (int)workingSetPrivate[0] + "MB";
                OverviewMemoryWSPeakLbl.Text = "Working Set Peak: " + (int)workingSetPeak[0] + "MB";
                OverviewMemoryWSLbl.Text = "Working Set: " + (int)workingSet[0] + "MB";
            }
        }

        public void UpdateOverviewIO(float[] read, float[] write)
        {
            if (OverviewIoChart.InvokeRequired || OverviewIoReadLbl.InvokeRequired || OverviewIoWriteLbl.InvokeRequired)
            {
                OverviewIoChart.Invoke((MethodInvoker)(() =>
                {
                    OverviewIoChart.Series[0].Points.DataBindXY(XValues, read);
                    OverviewIoChart.Series[1].Points.DataBindXY(XValues, write);
                }));
                OverviewIoReadLbl.Invoke((MethodInvoker)(() => OverviewIoReadLbl.Text = "Read: " + (int)read[0] + "KB"));
                OverviewIoWriteLbl.Invoke((MethodInvoker)(() => OverviewIoWriteLbl.Text = "Write: " + (int)write[0] + "KB"));
            }
            else
            {
                OverviewIoChart.Series[0].Points.DataBindXY(XValues, read);
                OverviewIoChart.Series[1].Points.DataBindXY(XValues, write);
                OverviewIoReadLbl.Text = "Read: " + (int)read[0] + "KB";
                OverviewIoWriteLbl.Text = "Write: " + (int)write[0] + "KB";
            }
        }

        public void UpdateOverviewInfo(float creatingProcessID, float elapsedTime, float handleCount, float idProcess, float pageFaults, float priorityBase, float privateBytes, float threadCount)
        {
            if (OverviewInfoCreatingProcessIDLbl.InvokeRequired || OverviewInfoElapsedTimeLbl.InvokeRequired || OverviewInfoHandleCountLbl.InvokeRequired || OverviewInfoIDProcessLbl.InvokeRequired || OverviewInfoPageFaultsLbl.InvokeRequired || OverviewInfoPriorityBaseLbl.InvokeRequired || OverviewInfoPrivateBytesLbl.InvokeRequired || OverviewInfoThreadCountLbl.InvokeRequired)
            {
                OverviewInfoCreatingProcessIDLbl.Invoke((MethodInvoker)(() => OverviewInfoCreatingProcessIDLbl.Text = "Creating Process ID: " + creatingProcessID));
                OverviewInfoElapsedTimeLbl.Invoke((MethodInvoker)(() => OverviewInfoElapsedTimeLbl.Text = "Elapsed Time: " + elapsedTime));
                OverviewInfoHandleCountLbl.Invoke((MethodInvoker)(() => OverviewInfoHandleCountLbl.Text = "Handle Count: " + handleCount));
                OverviewInfoHandleCountLbl.Invoke((MethodInvoker)(() => OverviewInfoIDProcessLbl.Text = "Process ID: " + idProcess));
                OverviewInfoPageFaultsLbl.Invoke((MethodInvoker)(() => OverviewInfoPageFaultsLbl.Text = "Page Faults: " + pageFaults));
                OverviewInfoPriorityBaseLbl.Invoke((MethodInvoker)(() => OverviewInfoPriorityBaseLbl.Text = "Priority Base: " + priorityBase));
                OverviewInfoPrivateBytesLbl.Invoke((MethodInvoker)(() => OverviewInfoPrivateBytesLbl.Text = "Private Bytes: " + privateBytes.ToString("#")));
                OverviewInfoThreadCountLbl.Invoke((MethodInvoker)(() => OverviewInfoThreadCountLbl.Text = "Thread Count: " + threadCount));
            }
            else
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
        }

        public void UpdateMemoryWorkingSet(float[] workingSetPrivate, float[] workingSetPeak, float[] workingSet)
        {
            if (MemoryWorkingSetChart.InvokeRequired || MemoryWSPrivateLbl.InvokeRequired || MemoryWSPeakLbl.InvokeRequired || MemoryWSLbl.InvokeRequired)
            {
                MemoryWorkingSetChart.Invoke((MethodInvoker)(() =>
                {
                    MemoryWorkingSetChart.Series[0].Points.DataBindXY(XValues, workingSetPrivate);
                    MemoryWorkingSetChart.Series[1].Points.DataBindXY(XValues, workingSetPeak);
                    MemoryWorkingSetChart.Series[2].Points.DataBindXY(XValues, workingSet);
                }));
                MemoryWSPrivateLbl.Invoke((MethodInvoker)(() => MemoryWSPrivateLbl.Text = "Private Working Set: " + (int)workingSetPrivate[0] + "MB"));
                MemoryWSPeakLbl.Invoke((MethodInvoker)(() => MemoryWSPeakLbl.Text = "Working Set Peak: " + (int)workingSetPeak[0] + "MB"));
                MemoryWSLbl.Invoke((MethodInvoker)(() => MemoryWSLbl.Text = "Working Set: " + (int)workingSet[0] + "MB"));
            }
            else
            {
                MemoryWorkingSetChart.Series[0].Points.DataBindXY(XValues, workingSetPrivate);
                MemoryWorkingSetChart.Series[1].Points.DataBindXY(XValues, workingSetPeak);
                MemoryWorkingSetChart.Series[2].Points.DataBindXY(XValues, workingSet);
                MemoryWSPrivateLbl.Text = "Private Working Set: " + (int)workingSetPrivate[0] + "MB";
                MemoryWSPeakLbl.Text = "Working Set Peak: " + (int)workingSetPeak[0] + "MB";
                MemoryWSLbl.Text = "Working Set: " + (int)workingSet[0] + "MB";
            }
        }

        public void UpdateMemoryPageFileList(float[] pageFile, float[] pageFilePeak)
        {
            if (MemoryPageFileChart.InvokeRequired || MemoryPFLbl.InvokeRequired || MemoryPFPeakLbl.InvokeRequired)
            {
                MemoryPageFileChart.Invoke((MethodInvoker)(() =>
                {
                    MemoryPageFileChart.Series[0].Points.DataBindXY(XValues, pageFile);
                    MemoryPageFileChart.Series[1].Points.DataBindXY(XValues, pageFilePeak);
                }));
                MemoryPFLbl.Invoke((MethodInvoker)(() => MemoryPFLbl.Text = "Page File: " + (int)pageFile[0] + "MB"));
                MemoryPFPeakLbl.Invoke((MethodInvoker)(() => MemoryPFPeakLbl.Text = "Page File Peak: " + (int)pageFilePeak[0] + "MB"));
            }
            else
            {
                MemoryPageFileChart.Series[0].Points.DataBindXY(XValues, pageFile);
                MemoryPageFileChart.Series[1].Points.DataBindXY(XValues, pageFilePeak);
                MemoryPFLbl.Text = "Page File: " + (int)pageFile[0] + "MB";
                MemoryPFPeakLbl.Text = "Page File Peak: " + (int)pageFilePeak[0] + "MB";
            }
        }

        public void UpdateMemoryVirtual(float[] virtualBytes, float[] virtualBytesPeak)
        {
            if (MemoryVirtualChart.InvokeRequired || MemoryVirtualLbl.InvokeRequired || MemoryVirtualPeakLbl.InvokeRequired)
            {
                MemoryVirtualChart.Invoke((MethodInvoker)(() =>
                {
                    MemoryVirtualChart.Series[0].Points.DataBindXY(XValues, virtualBytes);
                    MemoryVirtualChart.Series[1].Points.DataBindXY(XValues, virtualBytesPeak);
                }));
                MemoryVirtualLbl.Invoke((MethodInvoker)(() => MemoryVirtualLbl.Text = "Virtual: " + (int)virtualBytes[0] + "MB"));
                MemoryVirtualPeakLbl.Invoke((MethodInvoker)(() => MemoryVirtualPeakLbl.Text = "Virtual Peak: " + (int)virtualBytesPeak[0] + "MB"));
            }
            else
            {
                MemoryVirtualChart.Series[0].Points.DataBindXY(XValues, virtualBytes);
                MemoryVirtualChart.Series[1].Points.DataBindXY(XValues, virtualBytesPeak);
                MemoryVirtualLbl.Text = "Virtual: " + (int)virtualBytes[0] + "MB";
                MemoryVirtualPeakLbl.Text = "Virtual Peak: " + (int)virtualBytesPeak[0] + "MB";
            }
        }

        public void UpdateMemoryPool(float[] poolPaged, float[] poolNonpaged)
        {
            if (MemoryPoolChart.InvokeRequired || MemoryPoolPagedLbl.InvokeRequired || MemoryPoolNonpagedLbl.InvokeRequired)
            {
                MemoryPoolChart.Invoke((MethodInvoker)(() =>
                {
                    MemoryPoolChart.Series[0].Points.DataBindXY(XValues, poolPaged);
                    MemoryPoolChart.Series[1].Points.DataBindXY(XValues, poolNonpaged);
                }));
                MemoryPoolPagedLbl.Invoke((MethodInvoker)(() => MemoryPoolPagedLbl.Text = "Paged Pool: " + (int)poolPaged[0] + "KB"));
                MemoryPoolNonpagedLbl.Invoke((MethodInvoker)(() => MemoryPoolNonpagedLbl.Text = "Nonpaged Pool: " + (int)poolNonpaged[0] + "KB"));
            }
            else
            {
                MemoryPoolChart.Series[0].Points.DataBindXY(XValues, poolPaged);
                MemoryPoolChart.Series[1].Points.DataBindXY(XValues, poolNonpaged);
                MemoryPoolPagedLbl.Text = "Paged Pool: " + (int)poolPaged[0] + "KB";
                MemoryPoolNonpagedLbl.Text = "Nonpaged Pool: " + (int)poolNonpaged[0] + "KB";
            }
        }

        public void UpdateIOReadWrite(float[] read, float[] write)
        {
            if (IOReadWriteChart.InvokeRequired || IOReadLbl.InvokeRequired || IOWriteLbl.InvokeRequired)
            {
                IOReadWriteChart.Invoke((MethodInvoker)(() =>
                {
                    IOReadWriteChart.Series[0].Points.DataBindXY(XValues, read);
                    IOReadWriteChart.Series[1].Points.DataBindXY(XValues, write);
                }));
                IOReadLbl.Invoke((MethodInvoker)(() => IOReadLbl.Text = "Read: " + (int)read[0] + "KB"));
                IOWriteLbl.Invoke((MethodInvoker)(() => IOWriteLbl.Text = "Write: " + (int)write[0] + "KB"));
            }
            else
            {
                IOReadWriteChart.Series[0].Points.DataBindXY(XValues, read);
                IOReadWriteChart.Series[1].Points.DataBindXY(XValues, write);
                IOReadLbl.Text = "Read: " + (int)read[0] + "KB";
                IOWriteLbl.Text = "Write: " + (int)write[0] + "KB";
            }
        }

        public void UpdateIODataOther(float[] data, float[] other)
        {
            if (IODataOtherChart.InvokeRequired || IODataLbl.InvokeRequired || IOOtherLbl.InvokeRequired)
            {
                IODataOtherChart.Invoke((MethodInvoker)(() =>
                {
                    IODataOtherChart.Series[0].Points.DataBindXY(XValues, data);
                    IODataOtherChart.Series[1].Points.DataBindXY(XValues, other);
                }));
                IODataLbl.Invoke((MethodInvoker)(() => IODataLbl.Text = "Data: " + (int)data[0] + "KB"));
                IOOtherLbl.Invoke((MethodInvoker)(() => IOOtherLbl.Text = "Other: " + (int)other[0] + "KB"));
            }
            else
            {
                IODataOtherChart.Series[0].Points.DataBindXY(XValues, data);
                IODataOtherChart.Series[1].Points.DataBindXY(XValues, other);
                IODataLbl.Text = "Data: " + (int)data[0] + "KB";
                IOOtherLbl.Text = "Other: " + (int)other[0] + "KB";
            }
        }

        private void ProcessInfoForm_Resize(object sender, EventArgs e)
        {
            TabControl.Size = this.ClientSize;
        }

        private void OverviewVerticalSplitContainer_SplitterMoved(object sender, SplitterEventArgs e)
        {
            Settings.OverviewVerticalSplitContainerDivider = OverviewVerticalSplitContainer.SplitterDistance;
            OverviewTabPage.Focus();
        }

        private void OverviewLeftHorizontalSplitContainer_SplitterMoved(object sender, SplitterEventArgs e)
        {
            Settings.OverviewLeftHorizontalSplitContainerDivider = OverviewLeftHorizontalSplitContainer.SplitterDistance;
            OverviewTabPage.Focus();
        }

        private void OverviewRightHorizontalSplitContainer_SplitterMoved(object sender, SplitterEventArgs e)
        {
            Settings.OverviewRightHorizontalSplitContainerDivider = OverviewRightHorizontalSplitContainer.SplitterDistance;
            OverviewTabPage.Focus();
        }

        private void MemoryVerticalSplitContainer_SplitterMoved(object sender, SplitterEventArgs e)
        {
            Settings.MemoryVerticalSplitContainerDivider = MemoryVerticalSplitContainer.SplitterDistance;
            MemoryTabPage.Focus();
        }

        private void MemoryLeftHorizontalSplitContainer_SplitterMoved(object sender, SplitterEventArgs e)
        {
            Settings.MemoryLeftHorizontalSplitContainerDivider = MemoryLeftHorizontalSplitContainer.SplitterDistance;
            MemoryTabPage.Focus();
        }

        private void MemoryRightHorizontalSplitContainer_SplitterMoved(object sender, SplitterEventArgs e)
        {
            Settings.MemoryRightHorizontalSplitContainerDivider = MemoryRightHorizontalSplitContainer.SplitterDistance;
            MemoryTabPage.Focus();
        }

        private void IOHorizontalSplitContainer_SplitterMoved(object sender, SplitterEventArgs e)
        {
            Settings.IOHorizontalSplitContainerDivider = IOHorizontalSplitContainer.SplitterDistance;
            IOTabPage.Focus();
        }

        private class FormSettings
        {
            public int OverviewCpuMax;
            public int OverviewMemoryMax;
            public int OverviewIoMax;
            public int MemoryWorkingSetMax;
            public int MemoryPageFileMax;
            public int MemoryVirtualMax;
            public int MemoryPoolMax;
            public int IoReadWriteMax;
            public int IoDataOtherMax;
            public int OverviewVerticalSplitContainerDivider;
            public int OverviewLeftHorizontalSplitContainerDivider;
            public int OverviewRightHorizontalSplitContainerDivider;
            public int MemoryVerticalSplitContainerDivider;
            public int MemoryLeftHorizontalSplitContainerDivider;
            public int MemoryRightHorizontalSplitContainerDivider;
            public int IOHorizontalSplitContainerDivider;
            public int WindowWidth;
            public int WindowHeight;
            public int WindowX;
            public int WindowY;

            public FormSettings()
            {
                OverviewCpuMax = 90;
                OverviewMemoryMax = 90;
                OverviewIoMax = 90;
                MemoryWorkingSetMax = 90;
                MemoryPageFileMax = 90;
                MemoryVirtualMax = 90;
                MemoryPoolMax = 90;
                IoReadWriteMax = 90;
                IoDataOtherMax = 90;
                OverviewVerticalSplitContainerDivider = 495;
                OverviewLeftHorizontalSplitContainerDivider = 295;
                OverviewRightHorizontalSplitContainerDivider = 295;
                MemoryVerticalSplitContainerDivider = 495;
                MemoryLeftHorizontalSplitContainerDivider = 295;
                MemoryRightHorizontalSplitContainerDivider = 295;
                IOHorizontalSplitContainerDivider = 295;
                WindowWidth = 1017;
                WindowHeight = 659;
                WindowX = 640;
                WindowY = 100;
            }
        }

        private void ProcessInfoForm_Load(object sender, EventArgs e)
        {
            if (Settings == null)
                this.Close();
            else
            {
                this.Size = new Size(Settings.WindowWidth, Settings.WindowHeight);
                this.Location = new Point(Settings.WindowX, Settings.WindowY);
                OverviewCpuChart.ChartAreas[0].AxisX.Maximum = Settings.OverviewCpuMax;
                OverviewMemChart.ChartAreas[0].AxisX.Maximum = Settings.OverviewMemoryMax;
                OverviewIoChart.ChartAreas[0].AxisX.Maximum = Settings.OverviewIoMax;
                MemoryWorkingSetChart.ChartAreas[0].AxisX.Maximum = Settings.MemoryWorkingSetMax;
                MemoryPageFileChart.ChartAreas[0].AxisX.Maximum = Settings.MemoryPageFileMax;
                MemoryVirtualChart.ChartAreas[0].AxisX.Maximum = Settings.MemoryVirtualMax;
                MemoryPoolChart.ChartAreas[0].AxisX.Maximum = Settings.MemoryPoolMax;
                IOReadWriteChart.ChartAreas[0].AxisX.Maximum = Settings.IoReadWriteMax;
                IODataOtherChart.ChartAreas[0].AxisX.Maximum = Settings.IoDataOtherMax;
                OverviewVerticalSplitContainer.SplitterDistance = Settings.OverviewVerticalSplitContainerDivider;
                OverviewLeftHorizontalSplitContainer.SplitterDistance = Settings.OverviewLeftHorizontalSplitContainerDivider;
                OverviewRightHorizontalSplitContainer.SplitterDistance = Settings.OverviewRightHorizontalSplitContainerDivider;
                MemoryVerticalSplitContainer.SplitterDistance = Settings.MemoryVerticalSplitContainerDivider;
                MemoryLeftHorizontalSplitContainer.SplitterDistance = Settings.MemoryLeftHorizontalSplitContainerDivider;
                MemoryRightHorizontalSplitContainer.SplitterDistance = Settings.MemoryRightHorizontalSplitContainerDivider;
                IOHorizontalSplitContainer.SplitterDistance = Settings.IOHorizontalSplitContainerDivider;

                FormSettingsSaveTimer.Start();
            }
        }

        private void ProcessInfoForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            FormSettingsSaveTimer.Stop();
            UpdateFormSettings();
            FormSettingsSaveTimer.Dispose();
        }

        private void ProcessInfoForm_ResizeEnd(object sender, EventArgs e)
        {
            Settings.WindowWidth = this.Size.Width;
            Settings.WindowHeight = this.Size.Height;
        }

        private void ProcessInfoForm_LocationChanged(object sender, EventArgs e)
        {
            Settings.WindowX = this.Location.X;
            Settings.WindowY = this.Location.Y;
        }
    }
}
