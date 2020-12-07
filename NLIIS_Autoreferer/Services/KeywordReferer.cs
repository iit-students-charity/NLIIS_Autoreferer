using System.Collections.Generic;
using System.Linq;

namespace NLIIS_Autoreferer.Services
{
    public class KeywordReferer : IReferer
    {
        private IDictionary<string, int> TermsFrequencies { get; set; }

        public KeywordReferer()
        {
            TermsFrequencies = new Dictionary<string, int>();
        }

        public string GenerateRefer(string text)
        {
            TermsFrequencies = DocumentService.GetTermsFrequencies(text, true);
            var result = string.Empty;
            var wordValuableFrequency= 5;
            var mostValuable = TermsFrequencies
                .OrderByDescending(termFrequency => termFrequency.Value)
                .Where(termFrequency => termFrequency.Value > wordValuableFrequency)
                .Where(termFrequency => !DocumentService.IsAdjective(termFrequency.Key) && !DocumentService.IsVerb(termFrequency.Key))
                .Select(termFrequency => termFrequency.Key);
            var withoutForms = RemoveForms(mostValuable);
            var valuableWords = TermsFrequencies
                .Where(termFrequency => termFrequency.Value > wordValuableFrequency)
                .Where(termFrequency => !DocumentService.IsAdjective(termFrequency.Key) && !DocumentService.IsVerb(termFrequency.Key))
                .Select(termFrequency => termFrequency.Key);

            var allPhrases = new List<string>();
            
            foreach (var term in withoutForms)
            {
                result += $"\n{term}";
                
                var phrases = DocumentService.GetPhrases(term, text, valuableWords);

                foreach (var phrase in phrases)
                {
                    if (!allPhrases.Contains(phrase))
                    {
                        result += $"\n\t{phrase.ToLower()}";
                        allPhrases.Add(phrase);
                    }
                }
            }

            return result;
        }

        private IEnumerable<string> RemoveForms(IEnumerable<string> words)
        {
            var toExclude = new List<string>();
            
            foreach (var origin in words)
            {
                var haveForms = false;
                
                foreach (var toCompare in words.Except(new [] { origin }))
                {
                    haveForms = origin.Contains(toCompare.Substring(0, toCompare.Length - 3)) &&
                                origin.Length - toCompare.Length < 3;

                    if (haveForms)
                    {
                        break;
                    }
                }

                if (!haveForms && !toExclude.Contains(origin))
                {
                    toExclude.Add(origin);
                }
            }

            return toExclude;
        }

        public void Clear()
        {
            TermsFrequencies.Clear();
        }
    }
}
