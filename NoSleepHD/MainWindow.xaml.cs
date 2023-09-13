using Microsoft.Win32;
using NoSleepHD.Assets;
using System;
using System.Collections.Generic;
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
        public static Action? UpdateDisk;
        public static Action<object>? ButtonHandler;

        public static string path = System.Windows.Forms.Application.ExecutablePath;

        public static bool Started = false;

        public string[] disks = null;
        public RegistryKey registry = Registry.CurrentUser.CreateSubKey("Software\\NoSleepHD", true);

        private Timer Timer;

        public MainWindow()
        {
            InitializeComponent();

            Timer = new Timer();
            Timer.Interval = TimeSpan.FromSeconds(2.5f).Milliseconds;
            Timer.Elapsed += WriteToHDD;

            Load_Registry();
            Load_SSD();

            UpdateDisk = this.UpdateDiskList;
            ButtonHandler = this.AllButtonHandler;

            DataContext = new MainCommand();
        }

        private void Load_SSD()
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.DriveType == DriveType.Fixed)
                {
                    bool check = false;

                    if (disks.Contains(drive.Name)) {
                        check = true;
                    }

                    SSD_Frame frame = new SSD_Frame(drive.Name, check);
                    list_ssd.Items.Add(frame);
                }
            }
        }

        private void Load_Registry()
        {
            disks = registry.GetValue("Disk_List", new string[0]) as string[];

            string gegistry_path = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run").GetValue("NoSleepHD", "").ToString();
            if ($"\"{path}\" --slient"  == gegistry_path)
            {
                StartupButton.Content = App.getStringByKey("text_unset_startup");
            }

            if (Started)
            {
                StartDiskNoSleep();
            }
        }

        private void StartDiskNoSleep()
        {
            SetState(false);

            Timer.Start();
            StateButton.Content = App.getStringByKey("text_nosleep_stop");
        }

        private void WriteToHDD(object sender, ElapsedEventArgs e)
        {
            foreach (string a in disks)
            {
                File.WriteAllText(a + "NoSleepHD", App.getStringByKey("this_is_a_file_to_prevent_hdd_sleep"));
            }
        }

        private void DragMove(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void UpdateDiskList()
        {
            List<string> device_list = new List<string>();
            foreach (ListBoxItem listBoxItem in list_ssd.Items)
            {
                CheckBox check = listBoxItem.Content as CheckBox;
                if (check.IsChecked.Value)
                {
                    device_list.Add(check.Content.ToString());
                }
            }

            registry.SetValue("Disk_List", device_list.ToArray());

            disks = device_list.ToArray();
        }

        private void SetState(bool Show)
        {
            if (Show)
            {
                this.ShowInTaskbar = true;
                this.Visibility = Visibility.Visible;
            }
            else
            {
                this.ShowInTaskbar= false;
                this.Visibility = Visibility.Hidden;
            }
        }
        private async void AllButtonHandler(object para)
        {
            switch (para)
            {
                case "mini":
                    WindowState = WindowState.Minimized;
                    break;

                case "exit":

                    if (Started)
                    {
                        SetState(false);
                    } else {
                        Process.GetCurrentProcess().Kill();
                    }

                    break;

                case "Dont":
                    Process.GetCurrentProcess().Kill();

                    break;

                case "Start":
                    if (Started)
                    {
                        SetState(true);
                        Started = false;

                        Timer.Stop();

                        StateButton.Content = App.getStringByKey("text_nosleep_start");
                    } else {
                        if (disks.Length == 0)
                        {
                            MessageBox.Show(App.getStringByKey("you_have_not_selected_any_hdd"), App.getStringByKey("text_warning"), MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        Started = true;
                        notifyIcon.ShowBalloonTip(string.Empty, App.getStringByKey("nosleep_already_started"), Hardcodet.Wpf.TaskbarNotification.BalloonIcon.None);

                        StartDiskNoSleep();
                    }

                    break;

                case "Open":
                    SetState(true);
                    break;

                case "Startup":
                    RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
                    if ($"\"{path}\" --slient" == registryKey.GetValue("NoSleepHD", string.Empty).ToString())
                    {
                        registryKey.DeleteValue("NoSleepHD");
                        
                        StartupButton.Content = App.getStringByKey("text_set_startup");
                    } else {
                        registryKey.SetValue("NoSleepHD", $"\"{path}\" --slient");
                        StartupButton.Content = App.getStringByKey("text_unset_startup");
                    }

                    break;
            }
        }
    }
}
