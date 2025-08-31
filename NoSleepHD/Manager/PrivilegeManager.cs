using NoSleepHD.Core.Global;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace NoSleepHD.Manager
{
    public static class PrivilegeManager
    {
        public static bool IsAdmin()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public static void RunAsAdmin()
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.UseShellExecute = true;
            processStartInfo.FileName = MainGlobal.AppPath;
            processStartInfo.Verb = "runas";

            Process.Start(processStartInfo);
        }
    }
}
