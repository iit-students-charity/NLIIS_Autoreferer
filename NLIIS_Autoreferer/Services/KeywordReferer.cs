using OpenTextSummarizer;

namespace NLIIS_Autoreferer.Services
{
    public class KeywordReferer : IReferer
    {
        public string GenerateRefer(string text)
        {
            SummarizerArguments sumargs = new SummarizerArguments
            {
                DictionaryLanguage = "en",
                DisplayLines = sentCount,
                DisplayPercent = 0,
                InputFile = "",
                InputString = OriginalTextBox.Text // here your text
            };
            SummarizedDocument doc = Summarizer.Summarize(sumargs);
            string summary = string.Join("\r\n\r\n", doc.Sentences.ToArray());
// do some stuff with summary. It is your result.
            
            var words = DocumentService.GetTerms(text);
            return null;
        }

        public void Clean()
        {
        }
    }
}