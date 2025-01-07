using Hardcodet.Wpf.TaskbarNotification;
using NoSleepHD.ViewModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace NoSleepHD
{
    public partial class MainWindow : Window
    {
        public static MainWindow Instance { get; private set; }

        public static string[] args;

        public MainWindow()
        {
            InitializeComponent();

            Instance = this;
            DataContext = new WindowViewModel();
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
}
