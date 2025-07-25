using NoSleepHD.Interface;
using NoSleepHD.ViewModel;
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
        }

        public void Minimize()
        {
            WindowState = WindowState.Minimized;
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
