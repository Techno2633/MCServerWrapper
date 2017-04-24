using System;

namespace MCServerWrapperLib.Models
{
    public class Settings
    {
        public string ServerPath;
        public int MinRam;
        public int MaxRam;
        public bool SameMaxMin;
        public string BackupSource;
        public string BackupLocation;
        public int BackupInterval;
        public int BackupNumber;
        public ConsoleColor WrapperColor;
        public int ZipCompressionLevel;
        public bool ShowCpuRamUsage;
        public string LaunchFlags;
        public bool AutoFindBackupSource;

        public Settings()
        {
            ServerPath = "";
            MinRam = 1024;
            MaxRam = 1024;
            SameMaxMin = false;
            BackupSource = "";
            BackupLocation = "";
            BackupInterval = 30;
            BackupNumber = 20;
            WrapperColor = ConsoleColor.Yellow;
            ZipCompressionLevel = 6;
            ShowCpuRamUsage = true;
            LaunchFlags = "";
            AutoFindBackupSource = true;
        }
    }
}
