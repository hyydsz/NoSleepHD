using NoSleepHD.Core.Global;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace NoSleepHD.Core.Manager
{
    public static class CoreManager
    {
        [DllImport("user32.dll")]
        public static extern bool SwitchToThisWindow(IntPtr hWnd, bool fAltTab);

        public static bool OpenCore()
        {
            return TryToOpen(MainGlobal.AppCorePath, "Nothing");
        }

        public static void OpenMain()
        {
            if (!IsMainRunningAndActivate())
                TryToOpen(MainGlobal.AppPath, string.Empty);
        }

        public static void CloseCore()
        {
            foreach (Process process in Process.GetProcesses())
            {
                if (process.ProcessName == "NoSleepHD.Core")
                {
                    process.Kill();
                    break;
                }
            }
        }

        public static void CloseMain()
        {
            foreach (Process process in Process.GetProcesses())
            {
                if (process.ProcessName == "NoSleepHD")
                {
                    process.Kill();
                    break;
                }
            }
        }

        public static bool IsCoreRunning()
        {
            foreach (Process process in Process.GetProcesses())
            {
                if (process.ProcessName == "NoSleepHD.Core")
                    return true;
            }

            return false;
        }

        public static bool IsMainRunningAndActivate()
        {
            foreach (Process process in Process.GetProcesses())
            {
                if (process.ProcessName == "NoSleepHD")
                {
                    SwitchToThisWindow(process.MainWindowHandle, true);
                    return true;
                }
            }

            return false;
        }

        private static bool TryToOpen(string path, string argument)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = path,
                Arguments = argument
            };

            try
            {
                Process.Start(startInfo);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
