using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Timers;
using System.Windows;
using System.Windows.Input;

namespace NoSleepHD
{
    public partial class MainWindow : Window
    {
        public static bool NeedStart = false;

        public MainWindow()
        {
            InitializeComponent();

            DataContext = new WindowViewModel(this);
        }

        public static void HandlerArgs(string[] args)
        {
            foreach (string arg in args)
            {
                if (arg == "--slient")
                {
                    NeedStart = true;
                }
            }
        }

        public void SetState(bool Show)
        {
            if (Show)
            {
                ShowInTaskbar = true;
                Visibility = Visibility.Visible;
            }
            else
            {
                ShowInTaskbar = false;
                Visibility = Visibility.Hidden;
            }
        }
        
        public void ShowStartMessage()
        {
            notifyIcon.ShowBalloonTip(null, App.getStringByKey("nosleep_already_started"), BalloonIcon.Info);
        }

        private void DragMove(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;

            base.OnClosing(e);
        }
    }

    public class WindowViewModel : INotifyPropertyChanged
    {
        private MainWindow view;
        private SettingView settingView;

        private Timer timer;
        private List<string> disks;

        public ObservableCollection<DiskList> DiskLists { get; }

        public bool IsStarted
        {
            get
            {
                return timer.Enabled;
            }
        }

        public WindowViewModel(MainWindow view)
        {
            this.view = view;

            DiskLists = new ObservableCollection<DiskList>();

            timer = new Timer();
            timer.Interval = TimeSpan.FromSeconds(2.5).TotalMilliseconds;
            timer.Elapsed += WriteToHDD;

            LoadRegistry();
            LoadSSD();

            settingView = new SettingView(this);
        }

        public ICommand OnButtonCommand => new RelayCommand<string>(s =>
        {
            switch (s)
            {
                case "TopMini":
                    view.WindowState = WindowState.Minimized;
                    break;

                case "TopClose":
                    if (IsStarted)
                    {
                        view.SetState(false);
                    }
                    else
                    {
                        Environment.Exit(0);
                    }
                    break;

                case "TopSetting":
                    settingView.Show();
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
                    view.SetState(true);
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
                disks.Add(s.Text);
            }
            else
            {
                disks.Remove(s.Text);
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

            if (MainWindow.NeedStart)
            {
                view.SetState(false);

                timer.Start();
                OnPropertyChanged(nameof(IsStarted));
            }
        }

        public void StartDiskNoSleep()
        {
            if (IsStarted)
                return;

            view.ShowStartMessage();

            timer.Start();
            OnPropertyChanged(nameof(IsStarted));
        }

        public void StopDiskNoSleep()
        {
            if (!IsStarted)
                return;

            timer.Stop();
            OnPropertyChanged(nameof(IsStarted));
        }

        private void WriteToHDD(object sender, ElapsedEventArgs e)
        {
            foreach (string disk in disks)
            {
                File.WriteAllText(disk + "NoSleepHD", App.getStringByKey("this_is_a_file_to_prevent_hdd_sleep"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class DiskList
    {
        public string Text { get; set; }
        public bool IsChecked { get; set; }

        public DiskList(string text, bool isChecked)
        {
            Text = text;
            IsChecked = isChecked;
        }
    }
}
