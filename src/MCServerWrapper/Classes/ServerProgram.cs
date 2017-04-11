using CompressionLib;
using MCServerWrapper.Forms;
using MCServerWrapperLib.Classes;
using MCServerWrapperLib.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;
using Timer = System.Timers.Timer;

namespace MCServerWrapper.Classes
{
    public class ServerProgram
    {
        private static ConcurrentQueue<string> backups;
        private static Settings settings;
        private static Process process;
        private static ConsoleColor consoleColor = ConsoleColor.Yellow;

        private static List<Command> wrapperCommands;

        private static ConsoleEventDelegate handler;

        private ProcessInfoForm processInfoForm;

        private Timer BackupTimer;
        private Timer FormUpdateTimer;

        private const int LIST_MAX = 181;

        private PerformanceCounter PrivilegedTime;
        private PerformanceCounter ProcessorTime;
        private PerformanceCounter UserTime;
        private PerformanceCounter CreatingProcessID;
        private PerformanceCounter ElapsedTime;
        private PerformanceCounter HandleCount;
        private PerformanceCounter IDProcess;
        private PerformanceCounter IODataBytes;
        private PerformanceCounter IOOtherBytes;
        private PerformanceCounter IOReadBytes;
        private PerformanceCounter IOWriteBytes;
        private PerformanceCounter PageFaults;
        private PerformanceCounter PageFileBytes;
        private PerformanceCounter PageFileBytesPeak;
        private PerformanceCounter PoolNonpagedBytes;
        private PerformanceCounter PoolPagedBytes;
        private PerformanceCounter PriorityBase;
        private PerformanceCounter PrivateBytes;
        private PerformanceCounter ThreadCount;
        private PerformanceCounter VirtualBytes;
        private PerformanceCounter VirtualBytesPeak;
        private PerformanceCounter WorkingSet;
        private PerformanceCounter WorkingSetPrivate;
        private PerformanceCounter WorkingSetPeak;

        private List<float> ProcessorTimeList;
        private List<float> PrivilegedTimeList;
        private List<float> UserTimeList;
        private List<float> WorkingSetPrivateList;
        private List<float> WorkingSetPeakList;
        private List<float> WorkingSetList;
        private List<float> IOReadList;
        private List<float> IOWriteList;
        private List<float> IODataList;
        private List<float> IOOtherList;
        private List<float> PageFileList;
        private List<float> PageFilePeakList;
        private List<float> VirtualList;
        private List<float> VirtualPeakList;
        private List<float> PoolPagedList;
        private List<float> PoolNonpagedList;

        public ServerProgram()
        {
            handler = new ConsoleEventDelegate((eventType) =>
            {
                if (eventType == 2)
                {
                    ConsoleWriter.WriteLine("Caught closing", consoleColor);
                    if (!process.HasExited)
                    {
                        process.StandardInput.WriteLine("stop");
                        process.WaitForExit();
                    }
                }
                return false;
            });

            SetConsoleCtrlHandler(handler, true);

            //Sets event that attempts to close the server when the wrapper application closes unexpecedly
            AppDomain.CurrentDomain.DomainUnload += (s, e) =>
            {
                try
                {
                    if (!process.HasExited || process != null)
                        process.StandardInput.Write("stop");
                }
                catch { }
            };

            //Checks for Wrapper dir and creates it if it doesn't exist
            try
            {
                if (!Directory.Exists(@"Wrapper"))
                    Directory.CreateDirectory(@"Wrapper");
            }
            catch (Exception ex)
            {
                ExceptionPrinter.PrintException(ex, "Error creating wrapper directory.");
                Close(10);
                return;
            }

            //Checks for BackupList.json and creates one if it doesn't exist
            try
            {
                if (!File.Exists(@"Wrapper\BackupList.json"))
                {
                    JsonSerializerSettings jsonSettings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto };
                    string json = JsonConvert.SerializeObject(new ConcurrentQueue<string>().ToArray(), Formatting.Indented, jsonSettings);
                    File.WriteAllText(@"Wrapper\BackupList.json", json);
                }
            }
            catch (Exception ex)
            {
                ExceptionPrinter.PrintException(ex, "Error creating new BackupsList.json");
                Close(10);
                return;
            }

            //Checks for BackupList.json and reads info from it
            try
            {
                if (File.Exists(@"Wrapper\BackupList.json"))
                {
                    JsonSerializerSettings jsonSettings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto };
                    string json = File.ReadAllText(@"Wrapper\BackupList.json");
                    backups = new ConcurrentQueue<string>(JsonConvert.DeserializeObject<string[]>(json, jsonSettings));
                }
            }
            catch (Exception ex)
            {
                ExceptionPrinter.PrintException(ex, "Error reading data from BackupList.json");
                Close(10);
                return;
            }

            //Checks for Settings.json and creates one if not found
            try
            {
                if (!File.Exists(@"Wrapper\Settings.json"))
                {
                    int counter = 0;
                    while (!CheckWrapperSettings() && counter < 4)
                    {
                        counter++;
                    }

                    if (counter >= 4)
                    {
                        throw new Exception("Unable to create Settings.json after 5 tries");
                    }
                }

                string json = File.ReadAllText(@"Wrapper\Settings.json");
                settings = JsonConvert.DeserializeObject<Settings>(json);
            }
            catch (Exception ex)
            {
                ExceptionPrinter.PrintException(ex, "Error reading Settings.json");
                Close(10);
                return;
            }

            //sets the color the wrapper text specific to the wrapper
            consoleColor = settings.WrapperColor;

            //Checks if the server application and server wrapper are in the same directory
            try
            {
                if (Environment.CurrentDirectory != Path.GetDirectoryName(settings.ServerPath))
                {
                    ExceptionPrinter.PrintException(new Exception("Server application is not in the same directory as the wrapper.\nPlease put the server wrapper in the same directory as the server file."));
                    Close(10);
                    return;
                }
            }
            catch (Exception ex)
            {
                ExceptionPrinter.PrintException(ex, "Error checking server path and wrapper path");
                Close(10);
                return;
            }

            Console.Title = Path.GetFileName(settings.ServerPath);

            //initialize and fill lists
            float[] filler = new float[LIST_MAX];

            for (int i = 0; i < filler.Length; i++)
                filler[i] = 0;

            ProcessorTimeList = new List<float>(filler);
            PrivilegedTimeList = new List<float>(filler);
            UserTimeList = new List<float>(filler);
            WorkingSetPrivateList = new List<float>(filler);
            WorkingSetPeakList = new List<float>(filler);
            WorkingSetList = new List<float>(filler);
            IOReadList = new List<float>(filler);
            IOWriteList = new List<float>(filler);
            IODataList = new List<float>(filler);
            IOOtherList = new List<float>(filler);
            PageFileList = new List<float>(filler);
            PageFilePeakList = new List<float>(filler);
            VirtualList = new List<float>(filler);
            VirtualPeakList = new List<float>(filler);
            PoolPagedList = new List<float>(filler);
            PoolNonpagedList = new List<float>(filler);

            //Timer Instatiation
            BackupTimer = new Timer(settings.BackupInterval * 60000);
            FormUpdateTimer = new Timer(1000);

            //maximize console
            ShowWindow(Process.GetCurrentProcess().MainWindowHandle, 3);
        }

        [MTAThread]
        public void Start()
        {
            //Writes starting info to the console
            ConsoleWriter.WriteLine($"Starting", consoleColor);
            ConsoleWriter.WriteLine($"Program Source {settings.ServerPath}", consoleColor);
            ConsoleWriter.WriteLine($"Min RAM: {settings.MinRam}MB", consoleColor);
            ConsoleWriter.WriteLine($"Max RAM: {settings.MaxRam}MB", consoleColor);
            ConsoleWriter.WriteLine($"Backup Interval: {settings.BackupInterval} minutes", consoleColor);
            ConsoleWriter.WriteLine($"Number of Backups: {settings.BackupNumber}", consoleColor);
            ConsoleWriter.WriteLine($"Backup Compression Level: {settings.ZipCompressionLevel}", consoleColor);
            ConsoleWriter.WriteLine($"Backup Source: {settings.BackupSource}", consoleColor);
            ConsoleWriter.WriteLine($"Backup Destination: {settings.BackupLocation}", consoleColor);
            ConsoleWriter.WriteLine($"Show CPU and RAM Usage: {settings.ShowCpuRamUsage}", consoleColor);
            ConsoleWriter.WriteLine($"Launch Flags: {settings.LaunchFlags}", consoleColor);

            //initializes the process
            process = new Process();
            process.StartInfo = new ProcessStartInfo("java", $"-Xms{settings.MinRam}M -Xmx{settings.MaxRam}M {settings.LaunchFlags} -jar {settings.ServerPath} nogui")
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
            };

            //Set Backup Timer
            BackupTimer.Elapsed += (s, e) =>
            {
                if (process.HasExited || process == null)
                    return;

                try
                {
                    process.StandardInput.WriteLine("save-off");
                    process.StandardInput.WriteLine("say Starting Backup. Server may lag for a bit.");
                    int counter = 0;
                    while (!Backup())
                    {
                        counter++;
                        if (counter > 19)
                        {
                            throw new Exception("Unable to complete backup after 20 tries");
                        }
                    }
                    process.StandardInput.WriteLine($"say Backup Successful. Next backup in {settings.BackupInterval} minutes");
                }
                catch (Exception ex)
                {
                    process.StandardInput.WriteLine($"say Backup Failed. {ex.Message}.");
                    ExceptionPrinter.PrintException(ex, "Error while trying to initialize backup.");
                    CleanUp();
                }
                try { process.StandardInput.WriteLine("save-on"); } catch (Exception ex) { ExceptionPrinter.PrintException(ex, "Failed to send \"save-on\" message"); }
            };

            //Set FormUpdateTimer
            FormUpdateTimer.Elapsed += (s, e) =>
            {
                if (process.HasExited)
                    return;

                try
                {
                    float privilegedTime = PrivilegedTime.NextValue();
                    float processorTime = ProcessorTime.NextValue();
                    float userTime = UserTime.NextValue();
                    float creatingProcessID = CreatingProcessID.NextValue();
                    float elapsedTime = ElapsedTime.NextValue();
                    float handleCount = HandleCount.NextValue();
                    float idProcess = IDProcess.NextValue();
                    float ioDataBytes = IODataBytes.NextValue();
                    float ioOtherBytes = IOOtherBytes.NextValue();
                    float ioReadBytes = IOReadBytes.NextValue();
                    float ioWriteBytes = IOWriteBytes.NextValue();
                    float pageFaults = PageFaults.NextValue();
                    float pageFileBytes = PageFileBytes.NextValue();
                    float pageFileBytesPeak = PageFileBytesPeak.NextValue();
                    float poolNonpagedBytes = PoolNonpagedBytes.NextValue();
                    float poolPagedBytes = PoolPagedBytes.NextValue();
                    float priorityBase = PriorityBase.NextValue();
                    float privateBytes = PrivateBytes.NextValue();
                    float threadCount = ThreadCount.NextValue();
                    float virtualBytes = VirtualBytes.NextValue();
                    float virtualBytesPeak = VirtualBytesPeak.NextValue();
                    float workingSet = WorkingSet.NextValue();
                    float workingSetPrivate = WorkingSetPrivate.NextValue();
                    float workingSetPeak = WorkingSetPeak.NextValue();

                    ProcessorTimeList.Insert(0, processorTime / Environment.ProcessorCount / 100f);
                    PrivilegedTimeList.Insert(0, privilegedTime / Environment.ProcessorCount / 100f);
                    UserTimeList.Insert(0, userTime / Environment.ProcessorCount / 100f);
                    WorkingSetPrivateList.Insert(0, workingSetPrivate / 1048576);
                    WorkingSetPeakList.Insert(0, workingSetPeak / 1048576);
                    WorkingSetList.Insert(0, workingSet / 1048576);
                    IOReadList.Insert(0, ioReadBytes / 1024);
                    IOWriteList.Insert(0, ioWriteBytes / 1024);
                    IODataList.Insert(0, ioDataBytes / 1024);
                    IOOtherList.Insert(0, ioOtherBytes / 1024);
                    WorkingSetPrivateList.Insert(0, workingSetPrivate / 1048576);
                    WorkingSetPeakList.Insert(0, workingSetPeak / 1048576);
                    WorkingSetList.Insert(0, workingSet / 1048576);
                    PageFileList.Insert(0, pageFileBytes / 1048576);
                    PageFilePeakList.Insert(0, pageFileBytesPeak / 1048576);
                    VirtualList.Insert(0, virtualBytes / 1048576);
                    VirtualPeakList.Insert(0, virtualBytesPeak / 1048576);
                    PoolPagedList.Insert(0, poolPagedBytes / 1024);
                    PoolNonpagedList.Insert(0, poolNonpagedBytes / 1024);

                    while (ProcessorTimeList.Count > LIST_MAX)
                        ProcessorTimeList.RemoveAt(LIST_MAX);
                    while (PrivilegedTimeList.Count > LIST_MAX)
                        PrivilegedTimeList.RemoveAt(LIST_MAX);
                    while (UserTimeList.Count > LIST_MAX)
                        UserTimeList.RemoveAt(LIST_MAX);
                    while (WorkingSetPrivateList.Count > LIST_MAX)
                        WorkingSetPrivateList.RemoveAt(LIST_MAX);
                    while (WorkingSetPeakList.Count > LIST_MAX)
                        WorkingSetPeakList.RemoveAt(LIST_MAX);
                    while (WorkingSetList.Count > LIST_MAX)
                        WorkingSetList.RemoveAt(LIST_MAX);
                    while (IOReadList.Count > LIST_MAX)
                        IOReadList.RemoveAt(LIST_MAX);
                    while (IOWriteList.Count > LIST_MAX)
                        IOWriteList.RemoveAt(LIST_MAX);
                    while (IODataList.Count > LIST_MAX)
                        IODataList.RemoveAt(LIST_MAX);
                    while (IOOtherList.Count > LIST_MAX)
                        IOOtherList.RemoveAt(LIST_MAX);
                    while (WorkingSetPrivateList.Count > LIST_MAX)
                        WorkingSetPrivateList.RemoveAt(LIST_MAX);
                    while (WorkingSetPeakList.Count > LIST_MAX)
                        WorkingSetPeakList.RemoveAt(LIST_MAX);
                    while (WorkingSetList.Count > LIST_MAX)
                        WorkingSetList.RemoveAt(LIST_MAX);
                    while (PageFileList.Count > LIST_MAX)
                        PageFileList.RemoveAt(LIST_MAX);
                    while (PageFilePeakList.Count > LIST_MAX)
                        PageFilePeakList.RemoveAt(LIST_MAX);
                    while (VirtualList.Count > LIST_MAX)
                        VirtualList.RemoveAt(LIST_MAX);
                    while (VirtualPeakList.Count > LIST_MAX)
                        VirtualPeakList.RemoveAt(LIST_MAX);
                    while (PoolPagedList.Count > LIST_MAX)
                        PoolPagedList.RemoveAt(LIST_MAX);
                    while (PoolNonpagedList.Count > LIST_MAX)
                        PoolNonpagedList.RemoveAt(LIST_MAX);

                    if (processInfoForm.IsDisposed)
                        return;

                    processInfoForm.UpdateOverviewCPU(ProcessorTimeList.ToArray(), PrivilegedTimeList.ToArray(), UserTimeList.ToArray());
                    processInfoForm.UpdateOverviewMemory(WorkingSetPrivateList.ToArray(), WorkingSetPeakList.ToArray(), WorkingSetList.ToArray());
                    processInfoForm.UpdateOverviewIO(IOReadList.ToArray(), IOWriteList.ToArray());
                    processInfoForm.UpdateOverviewInfo(creatingProcessID, elapsedTime, handleCount, idProcess, pageFaults, priorityBase, privateBytes, threadCount);
                    processInfoForm.UpdateMemoryWorkingSet(WorkingSetPrivateList.ToArray(), WorkingSetPeakList.ToArray(), WorkingSetList.ToArray());
                    processInfoForm.UpdateMemoryPageFileList(PageFileList.ToArray(), PageFilePeakList.ToArray());
                    processInfoForm.UpdateMemoryVirtual(VirtualList.ToArray(), VirtualPeakList.ToArray());
                    processInfoForm.UpdateMemoryPool(PoolPagedList.ToArray(), PoolNonpagedList.ToArray());
                    processInfoForm.UpdateIOReadWrite(IOReadList.ToArray(), IOWriteList.ToArray());
                    processInfoForm.UpdateIODataOther(IODataList.ToArray(), IOOtherList.ToArray());
                }
                catch { }
            };

            //Thread that runs the connection between the wrapper and the server
            //Thread MainProcess = new Thread(() =>
            //{
            //    //Set priority and initalize threads
            //    const ThreadPriority ioPriority = ThreadPriority.AboveNormal;
            //    Thread outputThread = new Thread(outputReader) { Name = "ChildIO Output", Priority = ioPriority, IsBackground = true };
            //    Thread errorThread = new Thread(errorReader) { Name = "ChildIO Error", Priority = ioPriority, IsBackground = true };
            //    Thread inputThread = new Thread(inputReader) { Name = "ChildIO Input", Priority = ioPriority, IsBackground = true };

            //    //Start the IO threads
            //    outputThread.Start(process);
            //    errorThread.Start(process);
            //    inputThread.Start(process);

            //    //Signal to end the application
            //    ManualResetEvent stopApp = new ManualResetEvent(false);

            //    //Enables the exited event and set the stopApp signal on exited
            //    process.EnableRaisingEvents = true;
            //    process.Exited += (e, sender) => { stopApp.Set(); };

            //    //Wait for the child app to stop
            //    stopApp.WaitOne();

            //    //Write output
            //    ConsoleWriter.WriteLine("Process ended... shutting down host", consoleColor);

            //    //closes and disposes of the form
            //    //if (!processInfoForm.IsDisposed)
            //    //{
            //    //    try
            //    //    {
            //    //        processInfoForm.Close();
            //    //    }
            //    //    catch { }
            //    //    finally
            //    //    {
            //    //        processInfoForm.Dispose();
            //    //    }
            //    //}

            //    //Stops passthrough
            //    Thread.Sleep(1000);
            //    if (outputThread.IsAlive)
            //        outputThread.Abort();

            //    if (errorThread.IsAlive)
            //        errorThread.Abort();

            //    if (inputThread.IsAlive)
            //        inputThread.Abort();

            //    //close wrapper
            //    Close(8);
            //})
            //{
            //    IsBackground = true,
            //    Name = "ServerProcess",
            //    Priority = ThreadPriority.Highest
            //};

            //Form Thread
            Action FormStartupAction = new Action(() =>
            {
                processInfoForm = new ProcessInfoForm(LIST_MAX);
                FormUpdateTimer.Start();
                Application.Run(processInfoForm);
                FormUpdateTimer.Stop();
            });

            //load commands
            wrapperCommands = new List<Command>();

            wrapperCommands.Add(new Command("showstats", () =>
            {
                Task.Run(FormStartupAction);
            }));

            //pre-start backup
            if (!settings.AutoFindBackupSource)
            {
                try
                {
                    ConsoleWriter.WriteLine("Starting pre-start backup", consoleColor);
                    int counter = 0;
                    while (!Backup())
                    {
                        counter++;
                        if (counter > 19)
                        {
                            throw new Exception("Unable to complete backup after 20 tries");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPrinter.PrintException(ex, "Error while trying to initialize pre-start backup.");
                    CleanUp();
                }
            }

            //Starts the server process
            process.Start();

            //Performance Counter
            PrivilegedTime = new PerformanceCounter("Process", "% Privileged Time", process.ProcessName, true);
            ProcessorTime = new PerformanceCounter("Process", "% Processor Time", process.ProcessName, true);
            UserTime = new PerformanceCounter("Process", "% User Time", process.ProcessName, true);
            CreatingProcessID = new PerformanceCounter("Process", "Creating Process ID", process.ProcessName, true);
            ElapsedTime = new PerformanceCounter("Process", "Elapsed Time", process.ProcessName, true);
            HandleCount = new PerformanceCounter("Process", "Handle Count", process.ProcessName, true);
            IDProcess = new PerformanceCounter("Process", "ID Process", process.ProcessName, true);
            IODataBytes = new PerformanceCounter("Process", "IO Data Bytes/sec", process.ProcessName, true);
            IOOtherBytes = new PerformanceCounter("Process", "IO Other Bytes/sec", process.ProcessName, true);
            IOReadBytes = new PerformanceCounter("Process", "IO Read Bytes/sec", process.ProcessName, true);
            IOWriteBytes = new PerformanceCounter("Process", "IO Write Bytes/sec", process.ProcessName, true);
            PageFaults = new PerformanceCounter("Process", "Page Faults/sec", process.ProcessName, true);
            PageFileBytes = new PerformanceCounter("Process", "Page File Bytes", process.ProcessName, true);
            PageFileBytesPeak = new PerformanceCounter("Process", "Page File Bytes Peak", process.ProcessName, true);
            PoolNonpagedBytes = new PerformanceCounter("Process", "Pool Nonpaged Bytes", process.ProcessName, true);
            PoolPagedBytes = new PerformanceCounter("Process", "Pool Paged Bytes", process.ProcessName, true);
            PriorityBase = new PerformanceCounter("Process", "Priority Base", process.ProcessName, true);
            PrivateBytes = new PerformanceCounter("Process", "Private Bytes", process.ProcessName, true);
            ThreadCount = new PerformanceCounter("Process", "Thread Count", process.ProcessName, true);
            VirtualBytes = new PerformanceCounter("Process", "Virtual Bytes", process.ProcessName, true);
            VirtualBytesPeak = new PerformanceCounter("Process", "Virtual Bytes Peak", process.ProcessName, true);
            WorkingSet = new PerformanceCounter("Process", "Working Set", process.ProcessName, true);
            WorkingSetPrivate = new PerformanceCounter("Process", "Working Set - Private", process.ProcessName, true);
            WorkingSetPeak = new PerformanceCounter("Process", "Working Set Peak", process.ProcessName, true);

            //MainProcess.Start();

            //Set priority and initalize threads
            const ThreadPriority ioPriority = ThreadPriority.AboveNormal;
            Thread outputThread = new Thread(outputReader) { Name = "ChildIO Output", Priority = ioPriority, IsBackground = true };
            Thread errorThread = new Thread(errorReader) { Name = "ChildIO Error", Priority = ioPriority, IsBackground = true };
            Thread inputThread = new Thread(inputReader) { Name = "ChildIO Input", Priority = ioPriority, IsBackground = true };

            //Start the IO threads
            outputThread.Start(process);
            errorThread.Start(process);
            inputThread.Start(process);

            //Signal to end the application
            ManualResetEvent stopApp = new ManualResetEvent(false);

            //Enables the exited event and set the stopApp signal on exited
            process.EnableRaisingEvents = true;
            process.Exited += (e, sender) => { stopApp.Set(); };

            //BackupThread.Start();
            BackupTimer.Start();

            //starts server info form
            if (settings.ShowCpuRamUsage)
                Task.Run(FormStartupAction);

            //Thread.Sleep(10000);
            if (settings.AutoFindBackupSource)
            {
                //ConsoleWriter.WriteLine("Please wait until the post-start backup has finished before closing to prevent backup corruption", consoleColor);
                Task.Run(() => AutoFindBackupSource());

                //Thread.Sleep(20000);
                //try
                //{
                //    ConsoleWriter.WriteLine("Starting post-start backup", consoleColor);
                //    process.StandardInput.WriteLine("save-off");
                //    process.StandardInput.WriteLine("say Starting Backup. Server may lag for a bit.");
                //    int counter = 0;
                //    while (!Backup())
                //    {
                //        counter++;
                //        if (counter > 19)
                //        {
                //            throw new Exception("Unable to complete backup after 20 tries");
                //        }
                //    }
                //    process.StandardInput.WriteLine($"say Backup Successful. Next backup in {settings.BackupInterval - 1} minutes");
                //}
                //catch (Exception ex)
                //{
                //    process.StandardInput.WriteLine($"say Backup Failed. {ex.Message}.");
                //    ExceptionPrinter.PrintException(ex, "Error while trying to initialize backup.");
                //    CleanUp();
                //}
                //try { process.StandardInput.WriteLine("save-on"); } catch (Exception ex) { ExceptionPrinter.PrintException(ex, "Failed to send \"save-on\" message"); }
            }

            process.WaitForExit();
            stopApp.WaitOne();

            ConsoleWriter.WriteLine("Process ended... shutting down host", consoleColor);

            //stops timers
            BackupTimer.Stop();
            FormUpdateTimer.Stop();

            BackupTimer.Dispose();
            FormUpdateTimer.Dispose();

            //closes and disposes of the form
            if (!processInfoForm.IsDisposed)
            {
                try
                {
                    if (processInfoForm.InvokeRequired)
                        processInfoForm.Invoke((MethodInvoker)(() => processInfoForm.Close()));
                    else
                        processInfoForm.Close();
                }
                catch { }
                finally
                {
                    if (!processInfoForm.IsDisposed)
                        if (processInfoForm.InvokeRequired)
                            processInfoForm.Invoke((MethodInvoker)(() => processInfoForm.Dispose()));
                        else
                            processInfoForm.Dispose();
                }
            }

            //disposes of performance counters
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

            if (outputThread.IsAlive)
                outputThread.Abort();

            if (errorThread.IsAlive)
                errorThread.Abort();

            if (inputThread.IsAlive)
                inputThread.Abort();

            ConsoleWriter.WriteLine("Press Enter to continue...", consoleColor);
            Console.ReadLine();
        }

        private void AutoFindBackupSource()
        {
            if (Directory.Exists(Path.Combine(Environment.CurrentDirectory, "world")))
            {
                try
                {
                    Settings newSettings = new Settings()
                    {
                        AutoFindBackupSource = false,
                        BackupInterval = settings.BackupInterval,
                        BackupLocation = settings.BackupLocation,
                        BackupNumber = settings.BackupNumber,
                        BackupSource = Path.Combine(Environment.CurrentDirectory, "world"),
                        LaunchFlags = settings.LaunchFlags,
                        MaxRam = settings.MaxRam,
                        MinRam = settings.MinRam,
                        SameMaxMin = settings.SameMaxMin,
                        ServerPath = settings.ServerPath,
                        ShowCpuRamUsage = settings.ShowCpuRamUsage,
                        WrapperColor = settings.WrapperColor,
                        ZipCompressionLevel = settings.ZipCompressionLevel,
                    };

                    int counter = 0;
                    while (!SaveSettings(newSettings) && counter < 9)
                    {
                        counter++;
                        if (counter >= 9)
                        {
                            throw new Exception("Failed to update Settings.json after 10 tries");
                        }
                    }

                    string json = File.ReadAllText(@"Wrapper\Settings.json");
                    settings = JsonConvert.DeserializeObject<Settings>(json);
                }
                catch (Exception ex)
                {
                    ExceptionPrinter.PrintException(ex);
                }
            }
        }

        private bool SaveSettings(Settings settings)
        {
            try
            {
                if (File.Exists(@"Wrapper\Settings.json"))
                {
                    File.Delete(@"Wrapper\Settings.json");
                }
            }
            catch (Exception ex)
            {
                ExceptionPrinter.PrintException(ex, "Error deleting old Settings.json while generating new Settings.json");
                return false;
            }

            try
            {
                string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
                File.WriteAllText(@"Wrapper\Settings.json", json);
            }
            catch (Exception ex)
            {
                ExceptionPrinter.PrintException(ex, "Error creating Settings.json while generating new Settings.json");
                return false;
            }

            ConsoleWriter.WriteLine("Successfully found the backup source automatically", consoleColor);
            return true;
        }

        private bool Backup()
        {
            Stopwatch sw = new Stopwatch();

            //The name of the backup zip file. Not the full path only the file name
            string zipName = $"{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}_{DateTime.Now.Hour}-{DateTime.Now.Minute}-{DateTime.Now.Second}.zip";

            //Begins Backup
            try
            {
                sw.Start();
                using (FileStream fs = File.Create(Path.Combine(settings.BackupLocation, zipName)))
                {
                    fs.CompressZip(settings.BackupSource, settings.ZipCompressionLevel, true);
                }
                sw.Stop();
                ConsoleWriter.WriteLine("Created backup in " + sw.ElapsedMilliseconds + " ms", consoleColor);
            }
            catch (Exception ex)
            {
                sw.Stop();
                ExceptionPrinter.PrintException(ex, "Error creating backup.");
                if (File.Exists(Path.Combine(settings.BackupLocation, zipName)))
                {
                    File.Delete(Path.Combine(settings.BackupLocation, zipName));
                }
                return false;
            }

            //Enqueues the new backup into settings for saving
            backups.Enqueue(Path.Combine(settings.BackupLocation, zipName));

            CleanUp();

            //save new backuplist queue to BackupList.json
            try
            {
                string json = JsonConvert.SerializeObject(backups.ToArray(), Formatting.Indented);
                File.WriteAllText(@"Wrapper\BackupList.json", json);
            }
            catch (Exception ex)
            {
                ExceptionPrinter.PrintException(ex, "Error writing backups to BackupList.json");
            }

            //cleanUp.Wait();
            return true;
        }

        private void CleanUp()
        {
            Stopwatch sw = new Stopwatch();
            try
            {
                sw.Start();
                while (backups.Count > settings.BackupNumber)
                {
                    string backup;
                    int counter = 0;
                    while (!backups.TryDequeue(out backup))
                    {
                        counter++;
                        if (counter > 19)
                        {
                            throw new Exception("Failed to dequeue the oldest backup for deletion after 20 tries.");
                        }
                    }
                    File.Delete(backup);
                }
                sw.Stop();
                ConsoleWriter.WriteLine("Removed extra backups in " + sw.ElapsedMilliseconds + " ms", consoleColor);
            }
            catch (Exception ex)
            {
                sw.Stop();
                ExceptionPrinter.PrintException(ex, "Failed to remove extra backups.");
            }
        }

        private bool CheckWrapperSettings()
        {
            try
            {
                if (!Directory.Exists(@"Wrapper\Settings.json"))
                {
                    ProcessStartInfo psi = new ProcessStartInfo()
                    {
                        CreateNoWindow = false,
                        UseShellExecute = false,
                        FileName = @"MCServerWrapperSettingsApp.exe",
                        WindowStyle = ProcessWindowStyle.Normal,
                    };

                    using (Process settingsApp = Process.Start(psi))
                    {
                        settingsApp.WaitForExit();
                    }
                }

                if (File.Exists(@"Wrapper\Settings.json"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                ExceptionPrinter.PrintException(ex, $"Error launching {@"MCServerWrapperSettingsApp.exe"}");
                return false;
            }
        }

        private void CreateConsoleSetings()
        {

        }

        private void SaveConsoleSettings()
        {
            
        }

        private void LoadConsoleSettings()
        {

        }

        private static void passThrough(Stream instream, Stream outstream)
        {
            try
            {
                byte[] buffer = new byte[4096];
                while (!process.HasExited)
                {
                    int len;
                    while ((len = instream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        outstream.Write(buffer, 0, len);
                        outstream.Flush();
                    }
                }
            }
            catch (ThreadAbortException)
            {
                return;
            }
        }

        private static void outputReader(object p)
        {
            Process process = (Process)p;
            // Pass the standard output of the child to our standard output
            passThrough(process.StandardOutput.BaseStream, Console.OpenStandardOutput());
        }

        private static void errorReader(object p)
        {
            Process process = (Process)p;
            // Pass the standard error of the child to our standard error
            passThrough(process.StandardError.BaseStream, Console.OpenStandardError());
        }

        private static void inputReader(object p)
        {
            try
            {
                Process process = (Process)p;

                while (!process.HasExited)
                {
                        bool continuing = false;
                        string inStr = Console.ReadLine();

                        if (string.IsNullOrWhiteSpace(inStr))
                            continue;

                        foreach (Command cmd in wrapperCommands)
                        {
                            foreach (string cmdName in cmd.GetCommands())
                            {
                                if (cmdName.ToLower().Equals(inStr))
                                {
                                    cmd.Run();
                                    continuing = true;
                                    break;
                                }
                            }

                            if (continuing)
                                break;
                        }

                        if (continuing)
                            continue;

                        process.StandardInput.WriteLine(inStr);
                }
            }
            catch (ThreadAbortException)
            {
                return;
            }
        }

        //private static void inputReader(Process p)
        //{
        //    Process process = p;
        //    // Pass the standard input into the standard input of the child  
        //    passThrough(Console.OpenStandardInput(), process.StandardInput.BaseStream);
        //}

        private void Close()
        {
            Close(9);
        }

        private void Close(int exitCode)
        {
            try
            {
                if (process != null && !process.HasExited)
                {
                    process.StandardInput.WriteLine("stop");
                    process.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                ExceptionPrinter.PrintException(ex, "Failed to exit process");
            }
            Thread.Sleep(2000);
            ConsoleWriter.WriteLine("Press Enter key to continue...", consoleColor);
            Console.ReadLine();
            Environment.Exit(exitCode);
        }

        private delegate bool ConsoleEventDelegate(int eventType);

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int cmdShow);
    }
}