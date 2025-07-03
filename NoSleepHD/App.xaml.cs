using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;

namespace NoSleepHD
{
    public partial class App : Application
    {
        public static List<string> Languages = new List<string>()
        {
            "zh-CN", 
            "en-US",
            "en-GB"
        };

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            NoSleepHD.MainWindow.args = e.Args;

            InitLanguage();
        }

        public static void InitLanguage()
        {
            string lang = System.Globalization.CultureInfo.CurrentCulture.Name;

            if (!Languages.Contains(lang)) {
                return;
            }

            string requestedCulture = $"Assets/Lang/{lang}.xaml";
            Current.Resources.MergedDictionaries[1].Source = new Uri(requestedCulture, UriKind.Relative);
        }

        public static string getStringByKey(string key)
        {
            ResourceDictionary resource = Current.Resources.MergedDictionaries[1];
            IDictionaryEnumerator enumerator = resource.GetEnumerator();

            while (enumerator.MoveNext())
            {
                if (enumerator.Key.ToString() == key)
                {
                    return enumerator.Value?.ToString() ?? "";
                }
            }

            return string.Empty;
        }
    }
}
