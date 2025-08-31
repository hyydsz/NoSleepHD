using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using NoSleepHD.Model;
using NoSleepHD.View;
using NoSleepHD.Manager;
using NoSleepHD.Core.Global;
using NoSleepHD.Core.Manager;
using NoSleepHD.Core.Language;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Wpf.Ui;
using Wpf.Ui.Controls;
using System;

namespace NoSleepHD.ViewModel
{
    public partial class WindowViewModel : ObservableObject
    {
        public ObservableCollection<DiskModel> DiskLists { get; }
            = new ObservableCollection<DiskModel>();

        [ObservableProperty]
        private bool isStarted;

        [ObservableProperty]
        private object content;

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

        public bool OnStartup
        {
            get
            {
                return MainGlobal.TryStartupCurrent();
            }
            set
            {
                MainGlobal.TryStartupCurrent(value);
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

        public bool OnTiming
        {
            get
            {
                return MainGlobal.OnTiming;
            }
            set
            {
                MainGlobal.OnTiming = value;
            }
        }

        public int StartHour
        {
            get
            {
                return MainGlobal.StartHour;
            }
            set
            {
                MainGlobal.StartHour = value;
            }
        }

        public int StartMinute
        {
            get
            {
                return MainGlobal.StartMinute;
            }
            set
            {
                MainGlobal.StartMinute = value;
            }
        }

        public int EndHour
        {
            get
            {
                return MainGlobal.EndHour;
            }
            set
            {
                MainGlobal.EndHour = value;
            }
        }

        public int EndMinute
        {
            get
            {
                return MainGlobal.EndMinute;
            }
            set
            {
                MainGlobal.EndMinute = value;
            }
        }

        public int Interval
        {
            get
            {
                return MainGlobal.Interval;
            }
            set
            {
                MainGlobal.Interval = value;
            }
        }

        #endregion

        private List<string> _disks = new List<string>();

        private MainView _mainView = new MainView();
        private SettingView _settingView = new SettingView();

        private ISnackbarService _snackbarService;

        public WindowViewModel(ISnackbarService snackbarService)
        {
            _snackbarService = snackbarService;

            IsStarted = CoreManager.IsCoreRunning();
            Content = _mainView;

            LoadRegistry();
            LoadSSD();
        }

        [RelayCommand]
        private void OnButton(string command)
        {
            switch (command)
            {
                case "Setting":
                    Content = _settingView;
                    break;

                case "BackToMain":
                    Content = _mainView;
                    break;

                case "Switch":

                    if (IsStarted)
                    {
                        StopDiskNoSleep();
                        break;
                    }

                    if (_disks.Count == 0)
                    {
                        _snackbarService.Show
                        (
                            LanguageManager.GetStringByKey("text_warning"),
                            LanguageManager.GetStringByKey("you_have_not_selected_any_hdd"),
                            ControlAppearance.Caution,
                            null,
                            TimeSpan.FromSeconds(3)
                        );

                        break;
                    }

                    StartDiskNoSleep();
                    break;
            }
        }

        [RelayCommand]
        private void OnDiskButton(DiskModel disk)
        {
            if (disk.IsChecked)
            {
                _disks.Add(disk.Path);
            }
            else
            {
                _disks.Remove(disk.Path);
            }

            MainGlobal.Disks = _disks.ToArray();
        }

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
            _disks.Clear();
            _disks.AddRange(MainGlobal.Disks);
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
                _snackbarService.Show
                (
                    LanguageManager.GetStringByKey("text_warning"),
                    LanguageManager.GetStringByKey("text_open_core_failed"),
                    ControlAppearance.Danger,
                    null,
                    TimeSpan.FromSeconds(3)
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
    }
}
