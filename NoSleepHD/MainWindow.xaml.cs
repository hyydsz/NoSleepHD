using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NoSleepHD
{
    public partial class MainWindow : Window
    {
        public static readonly string path = Assembly.GetExecutingAssembly().Location;
        public static bool Started = false;

        public List<string>? disks = null;
        public RegistryKey registry = Registry.CurrentUser.CreateSubKey("Software\\NoSleepHD", true);

        private ObservableCollection<DiskList> lists = new ObservableCollection<DiskList>();
        private Timer Timer;

        public MainWindow()
        {
            InitializeComponent();

            Timer = new Timer();
            Timer.Interval = TimeSpan.FromMinutes(2.5f).TotalMilliseconds;
            Timer.Elapsed += WriteToHDD;

            Load_Registry();
            Load_SSD();

            list_ssd.ItemsSource = lists;

            DataContext = new WindowDataContext(this);
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
            string[]? disklist = registry.GetValue("Disk_List", new string[0]) as string[];
            disks = disklist?.ToList();

            if (disks == null) {
                disks = new List<string>();
            }

            if (TryStartupCurrent(false)) {
                StartupButton.Content = App.getStringByKey("text_unset_startup");
            }

            if (Started) {
                StartDiskNoSleep();
            }
        }

        private void StartDiskNoSleep()
        {
            SetState(false);

            Timer.Start();
            StateButton.Content = App.getStringByKey("text_nosleep_stop");
        }

        private void StopDiskNoSleep()
        {
            SetState(true);

            Timer.Stop();
            StateButton.Content = App.getStringByKey("text_nosleep_start");
        }

        private void WriteToHDD(object? sender, ElapsedEventArgs e)
        {
            foreach (string disk in disks)
            {
                File.WriteAllText(disk + "NoSleepHD", App.getStringByKey("this_is_a_file_to_prevent_hdd_sleep"));
            }
        }

        private void DragMove(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void UpdateDiskList(string diskName, bool mchecked)
        {
            if (diskName == null) {
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

            registry.SetValue("Disk_List", disks.ToArray());
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
                case "mini":
                    WindowState = WindowState.Minimized;
                    break;

                case "exit":
                    if (Timer.Enabled)
                    {
                        SetState(false);
                    } 
                    else 
                    {
                        Process.GetCurrentProcess().Kill();
                    }

                    break;

                case "Start":
                    if (Timer.Enabled)
                    {
                        StopDiskNoSleep();
                    } 
                    else 
                    {
                        if (disks.Count == 0)
                        {
                            MessageBox.Show(App.getStringByKey("you_have_not_selected_any_hdd"), App.getStringByKey("text_warning"), MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        notifyIcon.ShowBalloonTip(string.Empty, App.getStringByKey("nosleep_already_started"), Hardcodet.Wpf.TaskbarNotification.BalloonIcon.None);
                        StartDiskNoSleep();
                    }
                    break;

                case "Startup":
                    TryStartupCurrent(true);
                    break;

                case "Open":
                    SetState(true);
                    break;

                case "Close":
                    Process.GetCurrentProcess().Kill();
                    break;
            }
        }

        private bool TryStartupCurrent(bool Switch)
        {
            bool current = false;

            RegistryKey? registryKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
            if (registryKey == null) {
                return false;
            }

            if ($"\"{path}\" --slient" == registryKey.GetValue("NoSleepHD", "").ToString())
            {
                current = true;
            }

            if (Switch)
            {
                if (current)
                {
                    registryKey.DeleteValue("NoSleepHD");
                    StartupButton.Content = App.getStringByKey("text_set_startup");
                }
                else
                {
                    registryKey.SetValue("NoSleepHD", $"\"{path}\" --slient");
                    StartupButton.Content = App.getStringByKey("text_unset_startup");
                }
            }

            return current;
        }

        private void OnDiskClick(object sender, RoutedEventArgs e)
        {
            CheckBox check = sender as CheckBox;
            if (check != null)
            {
                UpdateDiskList(check.Content.ToString(), check.IsChecked.Value);
            }
        }

        public class DiskList
        {
            public string text { get; set;}
            public bool mchecked {  get; set;}
        }

        public class WindowDataContext
        {
            public WindowDataContext(MainWindow window)
            {
                command = new RelayCommand(s =>
                {
                    window.AllButtonHandler(s);
                });
            }

            public ICommand command { get; set; }

            private class RelayCommand : ICommand
            {
                public event EventHandler? CanExecuteChanged;
                public Action<string?> ExecuteAction;

                public bool CanExecute(object? parameter)
                {
                    return true;
                }

                public void Execute(object? parameter)
                {
                    ExecuteAction.Invoke(parameter?.ToString());
                }

                public RelayCommand(Action<string?> action)
                {
                    ExecuteAction = action;
                }
            }
        }
    }
}
