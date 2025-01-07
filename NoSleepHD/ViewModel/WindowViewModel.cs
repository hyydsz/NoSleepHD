using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Timers;
using System.Windows.Input;
using System.Windows;
using NoSleepHD.Model;
using NoSleepHD.View;
using NoSleepHD.Helper;
using Microsoft.Win32;
using System.Threading.Tasks;
using System.Linq;

namespace NoSleepHD.ViewModel
{
    public class WindowViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<DiskList> DiskLists { get; }

        public bool IsStarted
        {
            get
            {
                return timer.Enabled;
            }
        }

        private object content;
        public object Content
        {
            get
            {
                return content;
            }
            set
            {
                content = value;

                OnPropertyChanged(nameof(Content));
            }
        }

        #region 设置使用的
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

        public List<int> Hour { get; private set; }
        public List<int> Minute { get; private set; }

        private int _StartHour;
        public int StartHour
        {
            get
            {
                _StartHour = (int)SettingView.registry.GetValue("StartHour", 0);
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
        #endregion

        private MainView mainView;
        private SettingView settingView;

        private Timer timer;
        private List<string> disks;

        public WindowViewModel()
        {
            DiskLists = new ObservableCollection<DiskList>();

            settingView = new SettingView();
            mainView = new MainView();

            Content = mainView;

            timer = new Timer();
            timer.Interval = TimeSpan.FromMinutes(2.5).TotalMilliseconds; // 2.5分钟
            timer.Elapsed += ReadFromHDD;

            LoadSetting();
            LoadRegistry();
            LoadSSD();
        }

        public ICommand OnButtonCommand => new RelayCommand<string>(s =>
        {
            switch (s)
            {
                case "TopMini":
                    MainWindow.Instance.WindowState = WindowState.Minimized;
                    break;

                case "TopClose":

                    if (IsStarted)
                    {
                        MainWindow.Instance.SetState(false);
                        MainWindow.Instance.ShowStartMessage();
                    }
                    else
                    {
                        Application.Current.Shutdown();
                    }

                    break;

                case "TopSetting":
                    Content = settingView;
                    break;

                case "BackToMain":
                    Content = mainView;
                    break;

                case "Switch":

                    if (IsStarted)
                    {
                        StopDiskNoSleep();
                    }
                    else
                    {
                        if (disks.Count == 0)
                        {
                            MessageBox.Show(
                                App.getStringByKey("you_have_not_selected_any_hdd"),
                                App.getStringByKey("text_warning"),
                                MessageBoxButton.OK,
                                MessageBoxImage.Error
                            );

                            return;
                        }

                        StartDiskNoSleep();
                    }

                    break;



                case "Open":
                    MainWindow.Instance.SetState(true);
                    break;

                case "Close":
                    Environment.Exit(0);
                    break;
            }
        });

        public ICommand OnDiskButtonCommand => new RelayCommand<DiskList>(s =>
        {
            if (s.IsChecked)
            {
                disks.Add(s.Path);
            }
            else
            {
                disks.Remove(s.Path);
            }

            SettingView.registry.SetValue("Disk_List", disks.ToArray());
        });

        private void LoadSSD()
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.DriveType == DriveType.Fixed)
                {
                    DiskLists.Add(new DiskList(drive.Name, disks.Contains(drive.Name)));
                }
            }
        }

        private void LoadRegistry()
        {
            string[] disklist = (string[])SettingView.registry.GetValue("Disk_List", new string[0]);
            disks = new List<string>(disklist);

            if (MainWindow.args.Contains("--slient"))
            {
                MainWindow.Instance.SetState(false);
                timer.Start();

                UpdateUI();
            }
        }

        public void StartDiskNoSleep()
        {
            if (IsStarted)
                return;

            timer.Start();

            UpdateUI();
        }

        public void StopDiskNoSleep()
        {
            if (!IsStarted)
                return;

            timer.Stop();

            UpdateUI();
        }

        private void ReadFromHDD(object sender, ElapsedEventArgs e)
        {
            foreach (string disk in disks)
            {
                // 读取防止休眠
                if (Directory.Exists(disk))
                    Directory.GetFileSystemEntries(disk);
            }
        }

        private void UpdateUI()
        {
            OnPropertyChanged(nameof(IsStarted));
        }

        #region 设置方法
        private void LoadSetting()
        {
            Hour = new List<int>();
            for (int i = 0; i < 24; i++)
            {
                Hour.Add(i);
            }

            Minute = new List<int>();
            for (int i = 0; i < 60; i++)
            {
                Minute.Add(i);
            }

            StartTask();
        }

        private void StartTask()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    if (_onTiming)
                    {
                        DateTime now = DateTime.Now;

                        TimeSpan start = TimeSpan.FromHours(StartHour) + TimeSpan.FromMinutes(StartMinute);
                        TimeSpan end = TimeSpan.FromHours(EndHour) + TimeSpan.FromMinutes(EndMinute);

                        if (start != end)
                        {
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
                                StartDiskNoSleep();
                            }
                            else
                            {
                                StopDiskNoSleep();
                            }
                        }
                    }

                    Task task = Task.Delay(1000);
                    task.Wait();
                }
            });
        }

        private bool TryStartupCurrent()
        {
            RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
            object value = registryKey.GetValue("NoSleepHD");

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
                registryKey.SetValue("NoSleepHD", $"\"{SettingView.appPath}\" --slient");
            else
                registryKey.DeleteValue("NoSleepHD");
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
