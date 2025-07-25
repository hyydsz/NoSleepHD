using NoSleepHD.Core.Global;

namespace NoSleepHD.Core.Language
{
    public static class LanguageCoreManager
    {
        public static readonly List<LanguageModel> Languages = new List<LanguageModel>()
        {
            new LanguageModel("中文", "zh-CN", "主页面", "退出"),
            new LanguageModel("English", "en-US", "Open Menu", "Exit"),
        };

        public static LanguageModel Language
        {
            get
            {
                object? value = MainGlobal.NoSleepHDReg.GetValue("Langauge", "中文");
                string languageText = value is string ? (string)value : "中文";

                LanguageModel? language = Languages.FirstOrDefault(x => x.LanguageText == languageText);
                if (language == null)
                    language = Languages.First();

                return language;
            }
        }
    }
}
