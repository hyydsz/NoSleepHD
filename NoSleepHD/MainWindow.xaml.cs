using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NoSleepHD
{
    public partial class MainWindow : Window
    {
        private static bool Started = false;

        public List<string> disks;

        private readonly SettingView settingView;
        private readonly ObservableCollection<DiskList> lists = new ObservableCollection<DiskList>();
        private readonly Timer Timer;

        public MainWindow()
        {
            InitializeComponent();

            Timer = new Timer();
            Timer.Interval = TimeSpan.FromMinutes(2.5f).TotalMilliseconds;
            Timer.Elapsed += WriteToHDD;

            Load_Registry();
            Load_SSD();

            list_ssd.ItemsSource = lists;

            settingView = new SettingView(this);
            DataContext = new WindowViewModel(this);
        }

        public static void HandlerArgs(string[] args)
        {
            foreach (string arg in args)
            {
                if (arg == "--slient")
                {
                    Started = true;
                }
            }
        }

        private void Load_SSD()
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.DriveType == DriveType.Fixed)
                {
                    lists.Add(new DiskList() { text = drive.Name, mchecked = disks == null ? false : disks.Contains(drive.Name) });
                }
            }
        }

        private void Load_Registry()
        {
            string[]? disklist = SettingView.registry.GetValue("Disk_List", new string[0]) as string[];
            if (disklist == null)
            {
                disks = new List<string>();
            }
            else
            {
                disks = disklist.ToList();
            }

            if (Started) 
            {
                SetState(false);

                Timer.Start();
                StateButton.Content = App.getStringByKey("text_nosleep_stop");
            }
        }

        public void StartDiskNoSleep()
        {
            if (Timer.Enabled) {
                return;
            }

            notifyIcon.ShowBalloonTip("", App.getStringByKey("nosleep_already_started"), BalloonIcon.Info);

            Timer.Start();
            StateButton.Content = App.getStringByKey("text_nosleep_stop");
        }

        public void StopDiskNoSleep()
        {
            if (!Timer.Enabled) {
                return;
            }

            Timer.Stop();
            StateButton.Content = App.getStringByKey("text_nosleep_start");
        }

        private void WriteToHDD(object? sender, ElapsedEventArgs e)
        {
            if (disks == null) {
                return;
            }

            foreach (string disk in disks)
            {
                File.WriteAllText(disk + "NoSleepHD", App.getStringByKey("this_is_a_file_to_prevent_hdd_sleep"));
            }
        }

        private void UpdateDiskList(string? diskName, bool mchecked)
        {
            if (diskName == null || disks == null) {
                return;
            }

            if (mchecked)
            {
                disks.Add(diskName);
            }
            else
            {
                disks.Remove(diskName);
            }

            SettingView.registry.SetValue("Disk_List", disks.ToArray());
        }

        private void SetState(bool Show)
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

        public void AllButtonHandler(string CommandParameter) 
        {
            switch (CommandParameter)
            {
                case "TopMini":
                    WindowState = WindowState.Minimized;
                    break;

                case "TopClose":

                    if (Timer.Enabled)
                    {
                        SetState(false);
                    } 
                    else 
                    {
                        Process.GetCurrentProcess().Kill();
                    }

                    break;

                case "TopSetting":
                    settingView.Show();
                    break;

                case "Switch":

                    if (Timer.Enabled)
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
                    SetState(true);
                    break;

                case "Close":
                    Process.GetCurrentProcess().Kill();
                    break;
            }
        }

        private void OnDiskClick(object sender, RoutedEventArgs e)
        {
            CheckBox check = (CheckBox) sender;
            if (check != null)
            {
                UpdateDiskList(check.Content.ToString(), check.IsChecked ?? false);
            }
        }

        private void DragMove(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        public class WindowViewModel
        {
            private MainWindow view;

            public WindowViewModel(MainWindow view)
            {
                this.view = view;
            }

            public ICommand command => new StringCommand(s =>
            {
                view.AllButtonHandler(s);
            });
        }

        public class DiskList
        {
            public string text { get; set; }
            public bool mchecked { get; set; }
        }
    }
}
