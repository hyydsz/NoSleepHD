using Microsoft.Win32;
using System;
using System.IO;
using System.Windows.Controls;

namespace NoSleepHD.View
{
    public partial class SettingView : UserControl
    {
        public static readonly RegistryKey registry = Registry.CurrentUser.CreateSubKey("Software\\NoSleepHD", true);

        public static readonly string appName = "NoSleepHD";
        public static readonly string appPath = Path.Combine(Environment.CurrentDirectory, appName + ".exe");

        public SettingView()
        {
            InitializeComponent();
        }
    }
}
