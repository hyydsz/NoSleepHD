using Microsoft.Win32;
using NoSleepHD.Assets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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

        public static string[] disks = null;
        public static RegistryKey registry = Registry.CurrentUser.CreateSubKey("Software\\NoSleepHD", true);

        public static bool Started = false;

        public MainWindow()
        {
            InitializeComponent();

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
                    SSD_Frame frame = new SSD_Frame(drive.Name);
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
                StartupButton.Content = "取消开机自启动";
            }

            if (Started)
            {
                SetState(false);

                StartDiskNoSleep();
            }
        }

        private void StartDiskNoSleep()
        {
            SetState(false);

            notifyIcon.ShowBalloonTip("", "NoSleepHD 已经开始运行", Hardcodet.Wpf.TaskbarNotification.BalloonIcon.None);

            _ = Task.Factory.StartNew(() =>
            {
                while (Started)
                {
                    foreach (string a in disks)
                    {
                        File.WriteAllText(a + "NoSleepHD", "这是一个防止硬盘休眠的文件 ———— NoSleepHD");
                    }

                    Thread.Sleep(TimeSpan.FromMinutes(2.5));
                }
            });
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
        private void AllButtonHandler(object para)
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
                    }
                    else
                    {
                        Process.GetCurrentProcess().Kill();
                    }

                    break;

                case "Dont":
                    Process.GetCurrentProcess().Kill();

                    break;

                case "Start":
                    if (Started)
                    {
                        SetState(false);
                    }
                    else
                    {
                        if (disks.Length == 0)
                        {
                            MessageBox.Show("您没有选择任何硬盘！", "警告", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        Started = true;

                        UpdateDiskList();
                        StartDiskNoSleep();
                    }

                    break;

                case "Open":
                    SetState(true);
                    break;

                case "Startup":

                    RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
                    if ($"\"{path}\" --slient" == registryKey.GetValue("NoSleepHD", "").ToString())
                    {
                        registryKey.DeleteValue("NoSleepHD");
                        StartupButton.Content = "设置开机自启动";
                    }
                    else
                    {
                        registryKey.SetValue("NoSleepHD", $"\"{path}\" --slient");
                        StartupButton.Content = "取消开机自启动";
                    }

                    break;
            }
        }
    }
}
