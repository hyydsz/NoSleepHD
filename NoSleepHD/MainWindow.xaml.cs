using Hardcodet.Wpf.TaskbarNotification;
using NoSleepHD.Global;
using NoSleepHD.Interface;
using NoSleepHD.ViewModel;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace NoSleepHD
{
    public partial class MainWindow : Window, IMainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new WindowViewModel(this);

            LanguageGlobal.OnLanguageChanged += LanguageGlobal_OnLanguageChanged;
        }

        // Refresh Notify ContextMenu Language
        private void LanguageGlobal_OnLanguageChanged(object sender, EventArgs e)
        {
            NotifyMenu.UpdateDefaultStyle();
        }

        public void Minimize()
        {
            WindowState = WindowState.Minimized;
        }

        public void ChangeState(bool show)
        {
            if (show)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }

        public void ShowNotifyMsg()
        {
            notifyIcon.ShowBalloonTip(null, LanguageGlobal.GetStringByKey("nosleep_already_started"), BalloonIcon.Info);
        }

        private void DragMove(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        // Cancel Close
        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
