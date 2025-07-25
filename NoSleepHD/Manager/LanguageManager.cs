using NoSleepHD.Core.Global;
using NoSleepHD.Core.Language;
using System;
using System.Windows;

namespace NoSleepHD.Manager
{
    public static class LanguageManager
    {
        public static LanguageModel? CurrentLanguage { get; private set; }

        public static void InitLanguage()
        {
            UpdateLanguage(LanguageCoreManager.Language, false);
        }

        public static void UpdateLanguage(LanguageModel language, bool registry)
        {
            if (CurrentLanguage == language)
                return;

            CurrentLanguage = language;

            if (registry)
                MainGlobal.NoSleepHDReg.SetValue("Langauge", language.LanguageText);

            string requestedCulture = $"Assets/Lang/{language.LanguagePath}.xaml";
            Application.Current.Resources.MergedDictionaries[1].Source = new Uri(requestedCulture, UriKind.Relative);
        }

        public static string GetStringByKey(string key)
        {
            return (string)Application.Current.FindResource(key);
        }
    }
}
