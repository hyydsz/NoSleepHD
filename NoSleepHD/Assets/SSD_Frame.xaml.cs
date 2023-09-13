using System.Windows.Controls;

namespace NoSleepHD.Assets
{
    public partial class SSD_Frame : ListBoxItem
    {
        public SSD_Frame(string name)
        {
            InitializeComponent();

            Check.Content = name;

            if (MainWindow.disks != null)
            {
                foreach (string disk in MainWindow.disks)
                {
                    if (name == disk)
                    {
                        Check.IsChecked = true;
                    }
                }
            }
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
