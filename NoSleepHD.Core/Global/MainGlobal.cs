using Microsoft.Win32;

namespace NoSleepHD.Core.Global
{
    public static class MainGlobal
    {
        public static readonly RegistryKey NoSleepHDReg = Registry.CurrentUser.CreateSubKey(@"Software\NoSleepHD", true);

        public static readonly string AppDirectory = AppDomain.CurrentDomain.BaseDirectory;

        public static readonly string AppPath = Path.Combine(AppDirectory, "NoSleepHD.exe");
        public static readonly string AppCorePath = Path.Combine(AppDirectory, "NoSleepHD.Core.exe");

        public static readonly string AppStartupPath = $"\"{AppCorePath}\" Nothing";

        public static string[] Disks
        {
            get
            {
                object? disks = NoSleepHDReg.GetValue("Disk_List", new string[0]);
                return disks as string[] ?? Array.Empty<string>();
            }
        }

        public static bool OnTiming
        {
            get
            {
                object? value = NoSleepHDReg.GetValue("onTiming", 0);
                return value is int ? (int)value == 1 : false;
            }
        }

        public static int StartHour
        {
            get
            {
                object? value = NoSleepHDReg.GetValue("StartHour", 0);
                return value is int ? (int)value : 0;
            }
        }

        public static int StartMinute
        {
            get
            {
                object? value = NoSleepHDReg.GetValue("StartMinute", 0);
                return value is int ? (int)value : 0;
            }
        }

        public static int EndHour
        {
            get
            {
                object? value = NoSleepHDReg.GetValue("EndHour", 0);
                return value is int ? (int)value : 0;
            }
        }

        public static int EndMinute
        {
            get
            {
                object? value = NoSleepHDReg.GetValue("EndMinute", 0);
                return value is int ? (int)value : 0;
            }
        }
    }
}
