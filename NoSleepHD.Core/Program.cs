namespace NoSleepHD.Core
{
    public static class Program
    {
        private static Mutex? _mutex;

        [STAThread]
        private static void Main(string[] args)
        {
            if (args.Length == 0)
                return;

            _mutex = new Mutex(true, "NoSleepHD.Core", out bool createdNew);
            if (createdNew)
            {
                Application.Run(new HideForm());
            }
        }
    }
}