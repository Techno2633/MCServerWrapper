using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using MCServerWrapperLib.Models;
using MCServerWrapperSettingsApp.Classes;
using Newtonsoft.Json;
using Ookii.Dialogs;

namespace MCServerWrapperSettingsApp.Forms
{
    public partial class Main : Form
    {
        private Settings CurrentSettings;

        public Main()
        {
            InitializeComponent();

            CompressionLevelCbo.Items.AddRange(new object[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });

            ConsoleColorCbo.DataSource = Enum.GetValues(typeof(ConsoleColor)).Cast<ConsoleColor>();

            ulong totalmemB = 0;
            MEMORYSTATUSEX memStatus = new MEMORYSTATUSEX();
            if (GlobalMemoryStatusEx(memStatus))
            {
                totalmemB = memStatus.ullTotalPhys;
            }

            int usableMemory;

            try
            {
                usableMemory = Convert.ToInt32((totalmemB / (1024 * 1024)) - ((totalmemB / (1024 * 1024)) % 256));
            }
            catch
            {
                usableMemory = int.MaxValue - (int.MaxValue % 256);
            }

            MinRamSlider.Maximum = usableMemory / 256;
            MinRamSlider.Minimum = 0;
            MinRamSlider.SmallChange = 1;
            MinRamSlider.TickFrequency = 1;
            MinRamText.Text = (MinRamSlider.Value * 256).ToString();

            MaxRamSlider.Maximum = usableMemory / 256;
            MaxRamSlider.Minimum = 0;
            MaxRamSlider.SmallChange = 1;
            MaxRamSlider.TickFrequency = 1;
            MaxRamText.Text = (MinRamSlider.Value * 256).ToString();

            CurrentSettings = new Settings();

            try
            {
                if (!Directory.Exists(@"Wrapper"))
                    Directory.CreateDirectory(@"Wrapper");
            }
            catch (Exception ex)
            {
                ExceptionMessage.PrintException(ex, "Error creating wrapper directory.");
                return;
            }

            CurrentSettings = new Settings();

            ServerPath.Text = CurrentSettings.ServerPath;
            MinRamSlider.Value = CurrentSettings.MinRam / 256;
            MinRamText.Text = CurrentSettings.MinRam + " MB";
            MaxRamSlider.Value = CurrentSettings.MaxRam / 256;
            MaxRamText.Text = CurrentSettings.MaxRam + " MB";
            SameMaxMin.Checked = CurrentSettings.SameMaxMin;
            BackupSource.Text = CurrentSettings.BackupSource;
            BackupLocation.Text = CurrentSettings.BackupLocation;
            BackupInterval.Value = CurrentSettings.BackupInterval;
            BackupNumber.Value = CurrentSettings.BackupNumber;
            ConsoleColorCbo.SelectedItem = CurrentSettings.WrapperColor;
            CompressionLevelCbo.SelectedItem = CurrentSettings.ZipCompressionLevel;
            ShowRamCpuUsage.Checked = CurrentSettings.ShowCpuRamUsage;

            if (string.IsNullOrWhiteSpace(ServerPath.Text))
            {
                string[] files = Directory.GetFiles(Path.GetDirectoryName(Application.ExecutablePath));
                foreach (string file in files)
                {
                    if (file.ToLower().Contains("minecraft_server") && file.ToLower().EndsWith(".jar"))
                    {
                        ServerPath.Text = file;
                        return;
                    }
                }
            }
        }

        private void UpdateSilders(bool minSlider)
        {
            if (SameMaxMin.Checked)
            {
                if (minSlider)
                    MaxRamSlider.Value = MinRamSlider.Value;
                else
                    MinRamSlider.Value = MaxRamSlider.Value;
            }

            if (minSlider && MinRamSlider.Value > MaxRamSlider.Value)
                MaxRamSlider.Value = MinRamSlider.Value;


            if (!minSlider && MinRamSlider.Value > MaxRamSlider.Value)
                MinRamSlider.Value = MaxRamSlider.Value;

            MinRamText.Text = (MinRamSlider.Value * 256) + " MB";
            MaxRamText.Text = (MaxRamSlider.Value * 256) + " MB";
        }

        private void MinRamSlider_Scroll(object sender, EventArgs e)
        {
            UpdateSilders(true);
        }

        private void MaxRamSlider_Scroll(object sender, EventArgs e)
        {
            UpdateSilders(false);
        }

        private void BrowseServerPath_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = "Jar Files (*.jar)|*.jar",
                InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath)
            };
            if (DialogResult.OK == ofd.ShowDialog())
            {
                if (Path.GetDirectoryName(ofd.FileName) != Path.GetDirectoryName(Application.ExecutablePath))
                {
                    MessageBox.Show("The server you have selected is not in the same directory as the wrapper.\nPlease put the wrapper in the same directory as the server.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                ServerPath.Text = ofd.FileName;
            }
        }

        private void BrowseBackupSource_Click(object sender, EventArgs e)
        {
            if (!VistaFolderBrowserDialog.IsVistaFolderDialogSupported)
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog()
                {
                    RootFolder = Environment.SpecialFolder.MyComputer,
                };
                if (DialogResult.OK == fbd.ShowDialog())
                {
                    BackupSource.Text = fbd.SelectedPath;
                }
            }
            else
            {
                VistaFolderBrowserDialog vfbd = new VistaFolderBrowserDialog()
                {
                    Description = "Select a backup source",
                    RootFolder = Environment.SpecialFolder.MyComputer,
                    ShowNewFolderButton = false,
                    UseDescriptionForTitle = false,
                };
                if (vfbd.ShowDialog(this) == DialogResult.OK)
                {
                    BackupSource.Text = vfbd.SelectedPath;
                }
            }

        }

        private void BrowseBackupLocation_Click(object sender, EventArgs e)
        {
            if (!VistaFolderBrowserDialog.IsVistaFolderDialogSupported)
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog()
                {
                    RootFolder = Environment.SpecialFolder.MyComputer,
                };
                if (DialogResult.OK == fbd.ShowDialog())
                {
                    BackupLocation.Text = fbd.SelectedPath;
                }
            }
            else
            {
                VistaFolderBrowserDialog vfbd = new VistaFolderBrowserDialog()
                {
                    Description = "Select a backup location",
                    RootFolder = Environment.SpecialFolder.MyComputer,
                    ShowNewFolderButton = true,
                    UseDescriptionForTitle = false,
                };
                if (vfbd.ShowDialog(this) == DialogResult.OK)
                {
                    BackupLocation.Text = vfbd.SelectedPath;
                }
            }
        }

        private void GenSettings_Click(object sender, EventArgs e)
        {
            List<char> invalidCharsList = new List<char>(Path.GetInvalidPathChars());
            invalidCharsList.Add(' ');
            char[] invalidChars = invalidCharsList.ToArray();

            if (!File.Exists(ServerPath.Text))
            {
                MessageBox.Show("The server file path cannot be found or does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (ServerPath.Text.IndexOfAny(invalidChars) != -1)
            {
                MessageBox.Show("The server file path contains invalid characters.\nPlease remove all invalid characters.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (!FindBackupSource.Checked)
            {
                if (!Directory.Exists(BackupSource.Text))
                {
                    MessageBox.Show("The backup source directory cannot be found or does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (BackupSource.Text.IndexOfAny(invalidChars) != -1)
                {
                    MessageBox.Show("The backup source directory path contains invalid characters.\nPlease remove all invalid characters.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            if (!Directory.Exists(BackupLocation.Text))
            {
                MessageBox.Show("The backup location directory cannot be found or does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (BackupLocation.Text.IndexOfAny(invalidChars) != -1)
            {
                MessageBox.Show("The backup location directory path contains invalid characters.\nPlease remove all invalid characters.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Settings newSettings = new Settings()
            {
                ServerPath = ServerPath.Text,
                MinRam = MinRamSlider.Value * 256,
                MaxRam = MaxRamSlider.Value * 256,
                SameMaxMin = SameMaxMin.Checked,
                BackupSource = BackupSource.Text,
                BackupLocation = BackupLocation.Text,
                BackupInterval = (int)BackupInterval.Value,
                BackupNumber = (int)BackupNumber.Value,
                WrapperColor = (ConsoleColor)ConsoleColorCbo.SelectedItem,
                ZipCompressionLevel = CurrentSettings.ZipCompressionLevel,
                ShowCpuRamUsage = ShowRamCpuUsage.Checked,
                LaunchFlags = LaunchFlags.Text,
                AutoFindBackupSource = FindBackupSource.Checked,
            };

            try
            {
                if (File.Exists(@"Wrapper\Settings.json"))
                {
                    File.Delete(@"Wrapper\Settings.json");
                }
            }
            catch (Exception ex)
            {
                ExceptionMessage.PrintException(ex, "Error deleting old Settings.json while generating new Settings.json.");
                return;
            }

            try
            {
                string json = JsonConvert.SerializeObject(newSettings, Formatting.Indented);
                File.WriteAllText(@"Wrapper\Settings.json", json);
            }
            catch (Exception ex)
            {
                ExceptionMessage.PrintException(ex, "Error creating Settings.json while generating new Settings.json.");
                return;
            }

            MessageBox.Show("Successfully generated Settings.json.", "Success", MessageBoxButtons.OK, MessageBoxIcon.None);
        }

        private void CompressionLevelCbo_SelectedValueChanged(object sender, EventArgs e)
        {
            if (CompressionLevelCbo.SelectedItem == null)
                return;

            CurrentSettings.ZipCompressionLevel = (int)CompressionLevelCbo.SelectedItem;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private class MEMORYSTATUSEX
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;
            public MEMORYSTATUSEX()
            {
                this.dwLength = (uint)Marshal.SizeOf(this);
            }
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);
    }
}
