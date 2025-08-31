using NoSleepHD.ViewModel;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace NoSleepHD
{
    public partial class MainWindow : FluentWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            SystemThemeWatcher.Watch(this);

            SnackbarService snackbarService = new SnackbarService();
            snackbarService.SetSnackbarPresenter(RootSnackbar);

            DataContext = new WindowViewModel(snackbarService);
        }
    }
}
