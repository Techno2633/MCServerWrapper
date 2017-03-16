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

namespace MCServerWrapper.Classes
{
    public class ServerProgram
    {
        private static ConcurrentQueue<string> backups;
        private static Settings settings;
        private static Process process;
        private static ConsoleColor consoleColor;

        private static ConsoleEventDelegate handler;

        private static ProcessInfoForm processInfoForm;

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
                    while (!CheckWrapperSettings() && counter > 5)
                    {
                        counter++;
                    }

                    if (counter > 5)
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
            if (Environment.CurrentDirectory != Path.GetDirectoryName(settings.ServerPath))
            {
                ExceptionPrinter.PrintException(new Exception("Server application is not in the same directory as the wrapper.\nPlease put the server wrapper in the same directory as the server file."));
                Close(10);
                return;
            }
        }

        [MTAThread]
        public void Start()
        {
            //Writes starting info to the console
            ConsoleWriter.WriteLine($"Starting", consoleColor);
            ConsoleWriter.WriteLine($"Program Source {settings.ServerPath}", consoleColor);
            ConsoleWriter.WriteLine($"Min RAM: {settings.MinRam}", consoleColor);
            ConsoleWriter.WriteLine($"Max RAM: {settings.MaxRam}", consoleColor);
            ConsoleWriter.WriteLine($"Backup Interval: {settings.BackupInterval} minutes", consoleColor);
            ConsoleWriter.WriteLine($"Number of Backups: {settings.BackupNumber}", consoleColor);
            ConsoleWriter.WriteLine($"Backup Source: {settings.BackupSource}", consoleColor);
            ConsoleWriter.WriteLine($"Backup Destination: {settings.BackupLocation}", consoleColor);

            //initializes the process
            process = new Process();
            process.StartInfo = new ProcessStartInfo("java", $"-Xms{settings.MinRam}M -Xmx{settings.MaxRam}M -jar {settings.ServerPath} nogui")
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
            };

            //Thread that runs the connection between the wrapper and the server
            Thread MainProcess = new Thread(() =>
            {
                // Depending on your application you may either prioritize the IO or the exact opposite
                const ThreadPriority ioPriority = ThreadPriority.AboveNormal;
                var outputThread = new Thread(outputReader) { Name = "ChildIO Output", Priority = ioPriority, IsBackground = true };
                var errorThread = new Thread(errorReader) { Name = "ChildIO Error", Priority = ioPriority, IsBackground = true };
                var inputThread = new Thread(inputReader) { Name = "ChildIO Input", Priority = ioPriority, IsBackground = true };

                // Start the IO threads
                outputThread.Start(process);
                errorThread.Start(process);
                inputThread.Start(process);

                // Signal to end the application
                ManualResetEvent stopApp = new ManualResetEvent(false);

                // Enables the exited event and set the stopApp signal on exited
                process.EnableRaisingEvents = true;
                process.Exited += (e, sender) => { stopApp.Set(); };

                // Wait for the child app to stop
                stopApp.WaitOne();

                // Write output
                ConsoleWriter.WriteLine("Process ended... shutting down host", consoleColor);
                Close(8);
            })
            {
                IsBackground = true,
                Name = "ServerProcess",
                Priority = ThreadPriority.Highest
            };

            //Thread that handles the backing up of the server
            Thread BackupThread = new Thread(() =>
            {
                while (MainProcess.ThreadState != System.Threading.ThreadState.Stopped)
                {
                    Thread.Sleep(settings.BackupInterval * 60000);
                    try
                    {
                        process.StandardInput.WriteLine("save-off");
                        process.StandardInput.WriteLine("say Starting Backup. Server may lag for a bit.");
                        int counter = 0;
                        while (!Backup())
                        {
                            counter++;
                            if (counter > 20)
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
                }
            })
            {
                IsBackground = true,
                Name = "BackupThread",
                Priority = ThreadPriority.AboveNormal,
            };

            //Form Thread
            Thread FormStartupThread = new Thread(() =>
            {
                processInfoForm = new ProcessInfoForm(settings.MaxRam);

                Thread FormUpdateThread = new Thread(() => FormUpdate(processInfoForm))
                {
                    IsBackground = true,
                    Name = "FormUpdateThread",
                    Priority = ThreadPriority.Normal,
                };

                FormUpdateThread.Start();
                Application.Run(processInfoForm);
                FormUpdateThread.Abort();
            })
            {
                IsBackground = true,
                Name = "FormStartupThread",
                Priority = ThreadPriority.Normal,
            };
            FormStartupThread.TrySetApartmentState(ApartmentState.STA);

            //pre-start backup
            try
            {
                ConsoleWriter.WriteLine("Starting pre-start backup", consoleColor);
                int counter = 0;
                while (!Backup())
                {
                    counter++;
                    if (counter > 20)
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

            //Keep at bottom
            process.Start();
            MainProcess.Start();
            BackupThread.Start();

            if (settings.ShowCpuRamUsage)
            {
                FormStartupThread.Start();
            }
            else
            {
                FormStartupThread = null;
            }

            process.WaitForExit();
            MainProcess.Join();
        }

        //worker method for FormUpdateThread
        private void FormUpdate(ProcessInfoForm form)
        {
            PerformanceCounter memPC = new PerformanceCounter("Process", "Working Set - Private", process.ProcessName, true);
            PerformanceCounter cpuPC = new PerformanceCounter("Process", "% Processor Time", process.ProcessName, true);

            while (!process.HasExited)
            {
                form.UpdateMemoryChart(memPC.NextValue());
                form.UpdateCpuChart(cpuPC.NextValue());

                Thread.Sleep(1000);
            }
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
                        if (counter > 20)
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

        private static void passThrough(Stream instream, Stream outstream)
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

        private static void outputReader(object p)
        {
            var process = (Process)p;
            // Pass the standard output of the child to our standard output
            passThrough(process.StandardOutput.BaseStream, Console.OpenStandardOutput());
        }

        private static void errorReader(object p)
        {
            var process = (Process)p;
            // Pass the standard error of the child to our standard error
            passThrough(process.StandardError.BaseStream, Console.OpenStandardError());
        }

        private static void inputReader(object p)
        {
            var process = (Process)p;

            // Pass the standard input into the standard input of the child  
            passThrough(Console.OpenStandardInput(), process.StandardInput.BaseStream);
        }

        private void Close()
        {
            Close(9);
        }

        private void Close(int exitCode)
        {
            try
            {
                if (!process.HasExited)
                {
                    process.StandardInput.WriteLine("stop");
                    process.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                ExceptionPrinter.PrintException(ex, "Failed to exit process");
            }
            Console.ForegroundColor = consoleColor;
            Thread.Sleep(2000);
            Console.Write("Press any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
            Environment.Exit(exitCode);
        }

        private delegate bool ConsoleEventDelegate(int eventType);
        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);
    }
}