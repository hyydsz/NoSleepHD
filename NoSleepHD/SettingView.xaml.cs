using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace NoSleepHD
{
    public partial class SettingView : Window
    {
        public static readonly RegistryKey registry = Registry.CurrentUser.CreateSubKey("Software\\NoSleepHD", true);
        public static readonly string appName = "NoSleepHD";
        public static readonly string appPath = Path.Combine(Environment.CurrentDirectory, appName + ".exe");

        public SettingView(MainWindow main)
        {
            InitializeComponent();

            DataContext = new SettingViewModel(main);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            base.OnClosing(e);

            Hide();
        }
    }

    public class SettingViewModel
    {
        private MainWindow main;

        public bool onStartup
        {
            get
            {
                return TryStartupCurrent();
            }
            set
            {
                TryStartupCurrent(value);
            }
        }

        private bool _onTiming;
        public bool onTiming
        {
            get
            {
                _onTiming = (int)SettingView.registry.GetValue("onTiming", 0) == 1;
                return _onTiming;
            }
            set
            {
                _onTiming = value;
                SettingView.registry.SetValue("onTiming", _onTiming ? 1 : 0);
            }
        }

        public List<int> Hour { get; }
        public List<int> Minute { get; }

        private int _StartHour;
        public int StartHour
        {
            get
            {
                _StartHour = (int) SettingView.registry.GetValue("StartHour", 0);
                return _StartHour;
            }
            set
            {
                _StartHour = value;
                SettingView.registry.SetValue("StartHour", _StartHour);
            }
        }

        private int _StartMinute;
        public int StartMinute
        {
            get
            {
                _StartMinute = (int)SettingView.registry.GetValue("StartMinute", 0);
                return _StartMinute;
            }
            set
            {
                _StartMinute = value;
                SettingView.registry.SetValue("StartMinute", _StartMinute);
            }
        }

        private int _EndHour;
        public int EndHour
        {
            get
            {
                _EndHour = (int)SettingView.registry.GetValue("EndHour", 0);
                return _EndHour;
            }
            set
            {
                _EndHour = value;
                SettingView.registry.SetValue("EndHour", _EndHour);
            }
        }

        private int _EndMinute;
        public int EndMinute
        {
            get
            {
                _EndMinute = (int)SettingView.registry.GetValue("EndMinute", 0);
                return _EndMinute;
            }
            set
            {
                _EndMinute = value;
                SettingView.registry.SetValue("EndMinute", _EndMinute);
            }
        }

        public SettingViewModel(MainWindow main)
        {
            this.main = main;

            Hour = new List<int>();
            for (int i = 0; i < 24; i++)
            {
                Hour.Add(i);
            }

            Minute = new List<int>();
            for (int i = 0;i < 60; i++)
            {
                Minute.Add(i);
            }

            StartTask();
        }

        private async void StartTask()
        {
            while (true)
            {
                if (_onTiming)
                {
                    DateTime now = DateTime.Now;

                    TimeSpan start = TimeSpan.FromHours(StartHour) + TimeSpan.FromMinutes(StartMinute);
                    TimeSpan end = TimeSpan.FromHours(EndHour) + TimeSpan.FromMinutes(EndMinute);

                    if (start == end) {
                        continue;
                    }

                    bool enabled = false;

                    if (start > end)
                    {
                        if (now.TimeOfDay > end)
                        {
                            enabled = false;
                        }

                        if (now.TimeOfDay > start)
                        {
                            enabled = true;
                        }
                    }
                    else
                    {
                        if (now.TimeOfDay > start)
                        {
                            enabled = true;
                        }

                        if (now.TimeOfDay > end)
                        {
                            enabled = false;
                        }
                    }
                    

                    if (enabled)
                    {
                        main.StartDiskNoSleep();
                    }
                    else
                    {
                        main.StopDiskNoSleep();
                    }
                }

                await Task.Delay(1000);
            }
        }

        private bool TryStartupCurrent()
        {
            RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
            object? value = registryKey.GetValue("NoSleepHD");

            if (value == null)
            {
                return false;
            }

            return value.ToString() == $"\"{SettingView.appPath}\" --slient";
        }

        private void TryStartupCurrent(bool startup)
        {
            RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);

            if (startup)
            {
                registryKey.SetValue("NoSleepHD", $"\"{SettingView.appPath}\" --slient");
            }
            else
            {
                registryKey.DeleteValue("NoSleepHD");
            }
        }
    }
}
