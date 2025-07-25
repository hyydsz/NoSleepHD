using NoSleepHD.Manager;
using System.Threading;
using System.Windows;

namespace NoSleepHD
{
    public partial class App : Application
    {
        private static Mutex? _mutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            _mutex = new Mutex(true, "NoSleepHD", out bool createdNew);
            if (!createdNew)
            {
                Shutdown();
                return;
            }

            LanguageManager.InitLanguage();
        }
    }
}
