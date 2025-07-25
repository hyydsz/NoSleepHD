namespace NoSleepHD.Model
{
    public class DiskModel
    {
        public string Path { get; set; }
        public bool IsChecked { get; set; }

        public DiskModel(string path, bool isChecked)
        {
            Path = path;
            IsChecked = isChecked;
        }
    }
}
