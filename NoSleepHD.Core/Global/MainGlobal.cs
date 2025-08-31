using Microsoft.Win32;
using System.Reflection;

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
            set
            {
                NoSleepHDReg.SetValue("Disk_List", value);
            }
        }

        public static bool OnTiming
        {
            get
            {
                object? value = NoSleepHDReg.GetValue("onTiming", 0);
                return value is int ? (int)value == 1 : false;
            }
            set
            {
                NoSleepHDReg.SetValue("onTiming", value);
            }
        }

        public static int StartHour
        {
            get
            {
                object? value = NoSleepHDReg.GetValue("StartHour", 0);
                return value is int ? (int)value : 0;
            }
            set
            {
                NoSleepHDReg.SetValue("StartHour", value);
            }
        }

        public static int StartMinute
        {
            get
            {
                object? value = NoSleepHDReg.GetValue("StartMinute", 0);
                return value is int ? (int)value : 0;
            }
            set
            {
                NoSleepHDReg.SetValue("StartMinute", value);
            }
        }

        public static int EndHour
        {
            get
            {
                object? value = NoSleepHDReg.GetValue("EndHour", 0);
                return value is int ? (int)value : 0;
            }
            set
            {
                NoSleepHDReg.SetValue("EndHour", value);
            }
        }

        public static int EndMinute
        {
            get
            {
                object? value = NoSleepHDReg.GetValue("EndMinute", 0);
                return value is int ? (int)value : 0;
            }
            set
            {
                NoSleepHDReg.SetValue("EndMinute", value);
            }
        }

        public static int Interval
        {
            get
            {
                object? value = NoSleepHDReg.GetValue("Interval", (int)(2.5 * 60));
                return value is int ? (int)value : 0;
            }
            set
            {
                NoSleepHDReg.SetValue("Interval", value);
            }
        }

        public static bool TryStartupCurrent()
        {
            RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Run", true);
            object? value = registryKey.GetValue("NoSleepHD");

            if (value == null)
                return false;

            return value.ToString() == AppStartupPath;
        }

        public static void TryStartupCurrent(bool startup)
        {
            RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Run", true);
            if (startup)
            {
                registryKey.SetValue("NoSleepHD", AppStartupPath);
            }
            else
            {
                registryKey.DeleteValue("NoSleepHD");
            }
        }
    }
}
