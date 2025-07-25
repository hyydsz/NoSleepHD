using NoSleepHD.Core.Global;
using NoSleepHD.Core.Language;
using NoSleepHD.Core.Manager;
using System.ComponentModel;

namespace NoSleepHD.Core
{
    public partial class HideForm : Form
    {
        public HideForm()
        {
            InitializeComponent();

            Opacity = 0;
            ShowInTaskbar = false;
        }

        private void LoadLanguage()
        {
            LanguageModel language = LanguageCoreManager.Language;
            OpenToolStripMenuItem.Text = language.NotifyOpenText;
            CloseToolStripMenuItem.Text = language.NotifyCloseText;
        }

        private void LoadRegistry()
        {
            // Default 2.5 Minutes
            object interval = MainGlobal.NoSleepHDReg.GetValue("Interval", 2.5 * 60 * 1000);
            readTimer.Interval = Convert.ToInt32(interval);
        }

        private void readTimer_Tick(object sender, EventArgs e)
        {
            foreach (string disk in MainGlobal.Disks)
            {
                try
                {
                    // 读取防止休眠
                    if (Directory.Exists(disk))
                        Directory.GetFileSystemEntries(disk);
                }
                catch { }
            }
        }

        private void timingTimer_Tick(object sender, EventArgs e)
        {
            if (MainGlobal.OnTiming)
            {
                DateTime now = DateTime.Now;
                TimeSpan start = TimeSpan.FromHours(MainGlobal.StartHour) + TimeSpan.FromMinutes(MainGlobal.StartMinute);
                TimeSpan end = TimeSpan.FromHours(MainGlobal.EndHour) + TimeSpan.FromMinutes(MainGlobal.EndMinute);

                if (start != end)
                {
                    bool enabled = false;

                    if (start > end)
                    {
                        if (now.TimeOfDay > end)
                        {
                            enabled = false;
                        }

                        if (now.TimeOfDay > start)
                        {
                            enabled = true;
                        }
                    }
                    else
                    {
                        if (now.TimeOfDay > start)
                        {
                            enabled = true;
                        }

                        if (now.TimeOfDay > end)
                        {
                            enabled = false;
                        }
                    }

                    readTimer.Enabled = enabled;
                }
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            readTimer.Stop();
            timingTimer.Stop();
        }

        protected override void OnShown(EventArgs e)
        {
            Hide();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            LoadLanguage();
            LoadRegistry();

            readTimer.Start();
            timingTimer.Start();
        }

        private void OpenToolStripClick(object sender, EventArgs e)
        {
            CoreManager.OpenMain();
        }

        private void CloseToolStripClick(object sender, EventArgs e)
        {
            CoreManager.CloseMain();
            Application.Exit();
        }
    }
}
