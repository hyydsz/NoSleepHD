namespace NoSleepHD.Core.Language
{
    public class LanguageModel
    {
        public LanguageModel(string languageText, string languagePath, string notifyOpenText, string notifyCloseText)
        {
            LanguageText = languageText;
            LanguagePath = languagePath;

            NotifyOpenText = notifyOpenText;
            NotifyCloseText = notifyCloseText;
        }

        public string NotifyOpenText { get; set; }
        public string NotifyCloseText { get; set; }

        public string LanguageText { get; set; }
        public string LanguagePath { get; set; }
    }
}
