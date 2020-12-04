using NLIIS_Autoreferer.Services.KeywordSpecific;

namespace NLIIS_Autoreferer.Services
{
    public class KeywordReferer : IReferer
    {
        public string GenerateRefer(string text)
        {
            var sumargs = new SummarizerArguments
            {
                DictionaryLanguage = DocumentService.Language,
                DisplayLines = 10,
                DisplayPercent = 0,
                InputString = text
            };
            var doc = Summarizer.Summarize(sumargs);
            
            return string.Empty;
        }

        public void Clean()
        {
        }
    }
}