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
using NoSleepHD.Command;
using Microsoft.Win32;
using System.Threading.Tasks;
using NoSleepHD.Global;
using NoSleepHD.Interface;

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

        #region For Setting

        public List<LanguageModel> Languages
        {
            get
            {
                return LanguageGlobal.Languages;
            }
        }

        public LanguageModel Language
        {
            get
            {
                return LanguageGlobal.CurrentLanguage;
            }
            set
            {
                LanguageGlobal.UpdateLanguage(value, true);
            }
        }

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
                _onTiming = (int)MainGlobal.Registry.GetValue("onTiming", 0) == 1;
                return _onTiming;
            }
            set
            {
                _onTiming = value;
                MainGlobal.Registry.SetValue("onTiming", _onTiming ? 1 : 0);
            }
        }

        public List<int> Hour { get; private set; }
        public List<int> Minute { get; private set; }

        private int _StartHour;
        public int StartHour
        {
            get
            {
                _StartHour = (int)MainGlobal.Registry.GetValue("StartHour", 0);
                return _StartHour;
            }
            set
            {
                _StartHour = value;
                MainGlobal.Registry.SetValue("StartHour", _StartHour);
            }
        }

        private int _StartMinute;
        public int StartMinute
        {
            get
            {
                _StartMinute = (int)MainGlobal.Registry.GetValue("StartMinute", 0);
                return _StartMinute;
            }
            set
            {
                _StartMinute = value;
                MainGlobal.Registry.SetValue("StartMinute", _StartMinute);
            }
        }

        private int _EndHour;
        public int EndHour
        {
            get
            {
                _EndHour = (int)MainGlobal.Registry.GetValue("EndHour", 0);
                return _EndHour;
            }
            set
            {
                _EndHour = value;
                MainGlobal.Registry.SetValue("EndHour", _EndHour);
            }
        }

        private int _EndMinute;
        public int EndMinute
        {
            get
            {
                _EndMinute = (int)MainGlobal.Registry.GetValue("EndMinute", 0);
                return _EndMinute;
            }
            set
            {
                _EndMinute = value;
                MainGlobal.Registry.SetValue("EndMinute", _EndMinute);
            }
        }

        #endregion

        private MainView mainView;
        private SettingView settingView;

        private Timer timer;
        private List<string> disks;

        private IMainWindow mainWindow;

        public WindowViewModel(IMainWindow mainWindow)
        {
            this.mainWindow = mainWindow;

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

        public ICommand OnButtonCommand => new RelayCommand<string>(OnButton);

        private void OnButton(string command)
        {
            switch (command)
            {
                case "TopMini":
                    mainWindow.Minimize();
                    break;

                case "TopClose":

                    if (IsStarted)
                    {
                        mainWindow.ChangeState(false);
                        mainWindow.ShowNotifyMsg();
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
                            MessageBox.Show
                            (
                                LanguageGlobal.GetStringByKey("you_have_not_selected_any_hdd"),
                                LanguageGlobal.GetStringByKey("text_warning"),
                                MessageBoxButton.OK,
                                MessageBoxImage.Error
                            );

                            break;
                        }

                        StartDiskNoSleep();
                    }

                    break;

                case "NotifyOpen":
                    mainWindow.ChangeState(true);
                    break;

                case "NotifyClose":
                    Application.Current.Shutdown();
                    break;
            }
        }

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

            MainGlobal.Registry.SetValue("Disk_List", disks.ToArray());
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
            string[] disklist = (string[])MainGlobal.Registry.GetValue("Disk_List", new string[0]);
            disks = [.. disklist];

            if (MainGlobal.IsSlientRun)
            {
                mainWindow.ChangeState(false);
                timer.Start();

                UpdateIsStarted();
            }
        }

        public void StartDiskNoSleep()
        {
            if (IsStarted)
                return;

            timer.Start();
            UpdateIsStarted();
        }

        public void StopDiskNoSleep()
        {
            if (!IsStarted)
                return;

            timer.Stop();
            UpdateIsStarted();
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

        #region For Setting

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

            return value.ToString() == $"\"{MainGlobal.AppPath}\" --slient";
        }

        private void TryStartupCurrent(bool startup)
        {
            RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);

            if (startup)
                registryKey.SetValue("NoSleepHD", $"\"{MainGlobal.AppPath}\" --slient");
            else
                registryKey.DeleteValue("NoSleepHD");
        }

        #endregion

        private void UpdateIsStarted()
        {
            OnPropertyChanged(nameof(IsStarted));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
