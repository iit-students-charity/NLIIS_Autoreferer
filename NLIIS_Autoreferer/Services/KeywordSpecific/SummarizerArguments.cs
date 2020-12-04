namespace NLIIS_Autoreferer.Services.KeywordSpecific
{
    public class SummarizerArguments
    {
        public string DictionaryLanguage { get; set; }
        public string InputFile { get; set; }
        public string InputString { get; set; }
        public int DisplayPercent { get; set; }
        public int DisplayLines { get; set; }

        public SummarizerArguments()
        {
            DictionaryLanguage = "en"; //default to english
            DisplayPercent = 10; //default to 10%
            InputString = string.Empty;
            InputFile = string.Empty;
        }
    }
}
