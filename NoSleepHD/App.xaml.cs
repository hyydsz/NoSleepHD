using System.Windows;

namespace NoSleepHD
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            foreach (string arg in e.Args)
            {
                if (arg == "--slient")
                {
                    (this.MainWindow as MainWindow).Started = true;
                }
            }
        }
    }
}
