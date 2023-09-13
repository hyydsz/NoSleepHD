using System.Windows.Controls;

namespace NoSleepHD.Assets
{
    public partial class SSD_Frame : ListBoxItem
    {
        public SSD_Frame(string name, bool isCheck)
        {
            InitializeComponent();

            Check.Content = name;

            Check.IsChecked = isCheck;
        }

        private void Check_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (MainWindow.UpdateDisk != null)
            {
                MainWindow.UpdateDisk();
            }
        }
    }
}
