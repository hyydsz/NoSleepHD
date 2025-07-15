using NoSleepHD.Global;
using System;
using System.Linq;
using System.Windows;

namespace NoSleepHD
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (e.Args.Contains("--slient"))
                MainGlobal.IsSlientRun = true;

            LanguageGlobal.InitLanguage();
        }
    }
}
