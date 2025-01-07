namespace NoSleepHD.Model
{
    public class DiskList
    {
        public string Path { get; set; }
        public bool IsChecked { get; set; }

        public DiskList(string path, bool isChecked)
        {
            Path = path;
            IsChecked = isChecked;
        }
    }
}
