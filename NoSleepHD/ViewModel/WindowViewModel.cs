using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using System.Windows;
using NoSleepHD.Model;
using NoSleepHD.View;
using NoSleepHD.Command;
using Microsoft.Win32;
using NoSleepHD.Manager;
using NoSleepHD.Interface;
using NoSleepHD.Core.Global;
using NoSleepHD.Core.Manager;
using NoSleepHD.Core.Language;

namespace NoSleepHD.ViewModel
{
    public class WindowViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<DiskModel> DiskLists { get; }
            = new ObservableCollection<DiskModel>();

        private bool isStarted;
        public bool IsStarted
        {
            get
            {
                return isStarted;
            }
            set
            {
                isStarted = value;
                OnPropertyChanged(nameof(IsStarted));
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
                return LanguageCoreManager.Languages;
            }
        }

        public LanguageModel Language
        {
            get
            {
                return LanguageManager.CurrentLanguage!;
            }
            set
            {
                LanguageManager.UpdateLanguage(value, true);
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
                _onTiming = (int)MainGlobal.NoSleepHDReg.GetValue("onTiming", 0) == 1;
                return _onTiming;
            }
            set
            {
                _onTiming = value;
                MainGlobal.NoSleepHDReg.SetValue("onTiming", _onTiming ? 1 : 0);
            }
        }

        public List<int> Hour
        {
            get
            {
                var hour = new List<int>();
                for (int i = 0; i < 24; i++)
                {
                    hour.Add(i);
                }

                return hour;
            }
        }

        public List<int> Minute
        {
            get
            {
                var minute = new List<int>();
                for (int i = 0; i < 60; i++)
                {
                    minute.Add(i);
                }

                return minute;
            }
        }

        private int _StartHour;
        public int StartHour
        {
            get
            {
                _StartHour = (int)MainGlobal.NoSleepHDReg.GetValue("StartHour", 0);
                return _StartHour;
            }
            set
            {
                _StartHour = value;
                MainGlobal.NoSleepHDReg.SetValue("StartHour", _StartHour);
            }
        }

        private int _StartMinute;
        public int StartMinute
        {
            get
            {
                _StartMinute = (int)MainGlobal.NoSleepHDReg.GetValue("StartMinute", 0);
                return _StartMinute;
            }
            set
            {
                _StartMinute = value;
                MainGlobal.NoSleepHDReg.SetValue("StartMinute", _StartMinute);
            }
        }

        private int _EndHour;
        public int EndHour
        {
            get
            {
                _EndHour = (int)MainGlobal.NoSleepHDReg.GetValue("EndHour", 0);
                return _EndHour;
            }
            set
            {
                _EndHour = value;
                MainGlobal.NoSleepHDReg.SetValue("EndHour", _EndHour);
            }
        }

        private int _EndMinute;
        public int EndMinute
        {
            get
            {
                _EndMinute = (int)MainGlobal.NoSleepHDReg.GetValue("EndMinute", 0);
                return _EndMinute;
            }
            set
            {
                _EndMinute = value;
                MainGlobal.NoSleepHDReg.SetValue("EndMinute", _EndMinute);
            }
        }

        #endregion

        private MainView _mainView = new MainView();
        private SettingView _settingView = new SettingView();

        private List<string> _disks = new List<string>();

        private IMainWindow _mainWindow;

        public WindowViewModel(IMainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            content = _mainView;

            IsStarted = CoreManager.IsCoreRunning();

            LoadRegistry();
            LoadSSD();
        }

        public ICommand OnButtonCommand => new RelayCommand<string>(OnButton);

        private void OnButton(string command)
        {
            switch (command)
            {
                case "TopMini":
                    _mainWindow.Minimize();
                    break;

                case "TopClose":
                    Application.Current.Shutdown();
                    break;

                case "TopSetting":
                    Content = _settingView;
                    break;

                case "BackToMain":
                    Content = _mainView;
                    break;

                case "Switch":

                    if (IsStarted)
                    {
                        StopDiskNoSleep();
                    }
                    else
                    {
                        if (_disks.Count == 0)
                        {
                            MessageBox.Show
                            (
                                LanguageManager.GetStringByKey("you_have_not_selected_any_hdd"),
                                LanguageManager.GetStringByKey("text_warning"),
                                MessageBoxButton.OK,
                                MessageBoxImage.Error
                            );

                            break;
                        }

                        StartDiskNoSleep();
                    }

                    break;
            }
        }

        public ICommand OnDiskButtonCommand => new RelayCommand<DiskModel>(s =>
        {
            if (s.IsChecked)
            {
                _disks.Add(s.Path);
            }
            else
            {
                _disks.Remove(s.Path);
            }

            MainGlobal.NoSleepHDReg.SetValue("Disk_List", _disks.ToArray());
        });

        private void LoadSSD()
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.DriveType == DriveType.Fixed)
                {
                    DiskLists.Add(new DiskModel(drive.Name, _disks.Contains(drive.Name)));
                }
            }
        }

        private void LoadRegistry()
        {
            string[] disklist = (string[])MainGlobal.NoSleepHDReg.GetValue("Disk_List", new string[0]);

            _disks.Clear();
            _disks.AddRange(disklist);
        }

        public void StartDiskNoSleep()
        {
            if (IsStarted)
                return;

            if (CoreManager.OpenCore())
            {
                IsStarted = true;
            }
            else
            {
                MessageBox.Show
                (
                    LanguageManager.GetStringByKey("text_open_core_failed"),
                    LanguageManager.GetStringByKey("text_warning"),
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        public void StopDiskNoSleep()
        {
            if (!IsStarted)
                return;

            CoreManager.CloseCore();
            IsStarted = false;
        }

        #region For Setting

        private bool TryStartupCurrent()
        {
            RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Run", true);
            object? value = registryKey.GetValue("NoSleepHD");

            if (value == null)
                return false;

            return value.ToString() == MainGlobal.AppStartupPath;
        }

        private void TryStartupCurrent(bool startup)
        {
            RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Run", true);
            if (startup)
            {
                registryKey.SetValue("NoSleepHD", MainGlobal.AppStartupPath);
            }
            else
            {
                registryKey.DeleteValue("NoSleepHD");
            }
        }

        #endregion

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
