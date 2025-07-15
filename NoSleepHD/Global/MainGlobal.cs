using Microsoft.Win32;
using System;
using System.IO;

namespace NoSleepHD.Global
{
    public static class MainGlobal
    {
        public static readonly RegistryKey Registry = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\\NoSleepHD", true);

        public static readonly string AppName = "NoSleepHD";
        public static readonly string AppPath = Path.Combine(Environment.CurrentDirectory, AppName + ".exe");

        public static bool IsSlientRun = false;
    }
}
