using System.Collections.Generic;
using System.Linq;

namespace NLIIS_Autoreferer.Services
{
    public class KeywordReferer : IReferer
    {
        /*public string GenerateRefer(string text)
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
        }*/

        private IDictionary<string, int> TermsFrequencies { get; set; }

        public KeywordReferer()
        {
            TermsFrequencies = new Dictionary<string, int>();
        }

        public string GenerateRefer(string text)
        {
            TermsFrequencies = DocumentService.GetTermsFrequencies(text, true);
            var result = string.Empty;
            var mostValuable = TermsFrequencies
                .OrderByDescending(termFrequency => termFrequency.Value)
                .Where(termFrequency => termFrequency.Value > 4)
                .Where(termFrequency => !DocumentService.IsAdjective(termFrequency.Key));
            
            foreach (var (term, _) in mostValuable)
            {
                var phrases = DocumentService.GetPhrases(term, text);
                
                result += $"\n{term}";
                result = phrases.Aggregate(result, (current, phrase) => current + $"\n\t{phrase.ToLower()}");
            }

            return result;
        }

        public void Clear()
        {
            TermsFrequencies.Clear();
        }
    }
}