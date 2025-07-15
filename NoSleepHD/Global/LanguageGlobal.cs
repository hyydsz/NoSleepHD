using NoSleepHD.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace NoSleepHD.Global
{
    public static class LanguageGlobal
    {
        public static event EventHandler OnLanguageChanged;

        public static readonly List<LanguageModel> Languages = new List<LanguageModel>()
        {
            new LanguageModel("中文", "zh-CN"),
            new LanguageModel("English", "en-US"),
        };

        public static LanguageModel CurrentLanguage { get; private set; }

        public static void InitLanguage()
        {
            string languageText = (string) MainGlobal.Registry.GetValue("Langauge", "中文");
            LanguageModel language = Languages.FirstOrDefault(x => x.LanguageText == languageText);
            if (language == null)
                language = Languages.First();

            UpdateLanguage(language, false);
        }

        public static void UpdateLanguage(LanguageModel language, bool registry)
        {
            if (CurrentLanguage == language)
                return;

            CurrentLanguage = language;

            if (registry)
                MainGlobal.Registry.SetValue("Langauge", language.LanguageText);

            string requestedCulture = $"Assets/Lang/{language.LanguagePath}.xaml";
            Application.Current.Resources.MergedDictionaries[1].Source = new Uri(requestedCulture, UriKind.Relative);

            OnLanguageChanged?.Invoke(language, EventArgs.Empty);
        }

        public static string GetStringByKey(string key)
        {
            return (string)Application.Current.FindResource(key);
        }
    }
}
