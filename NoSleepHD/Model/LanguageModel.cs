namespace NoSleepHD.Model
{
    public class LanguageModel
    {
        public LanguageModel(string languageText, string languagePath)
        {
            LanguageText = languageText;
            LanguagePath = languagePath;
        }

        public string LanguageText { get; set; }
        public string LanguagePath { get; set; }
    }
}
