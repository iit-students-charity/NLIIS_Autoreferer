using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BitMiracle.Docotic.Pdf;

namespace NLIIS_Autoreferer.Services
{
    public static class DocumentService
    {
        public static string Language { get; set; }
        
        public static string FromPDF(string path)
        {
            using var pdf = new PdfDocument(path);
            
            return pdf.GetText();
        }
        
        public static IEnumerable<string> GetWordsSet(string text)
        {
            var pattern = GetWordMatchPattern();
            var wordsEntries = Regex.Matches(text, pattern).Select(match => match.Value);
            var uniqueWords = wordsEntries.ToHashSet().AsEnumerable();
            
            return uniqueWords;
        }
        
        public static IDictionary<string, decimal> GetWordsWeights(string text)
        {
            var pattern = GetWordMatchPattern();
            var wordsEntries = Regex.Matches(text, pattern).Select(match => match.Value);
            var wordsRawWeights = new Dictionary<string, decimal>();

            foreach (var wordEntry in wordsEntries)
            {
                var currentWeight = 0m;
                
                if (wordsRawWeights.ContainsKey(wordEntry))
                {
                    wordsRawWeights.TryGetValue(wordEntry, out currentWeight);
                    wordsRawWeights.Remove(wordEntry);
                }
                
                wordsRawWeights.Add(wordEntry, ++currentWeight);
            }

            var wordsAdjustedWeights = wordsRawWeights.ToDictionary(
                wordWeight => wordWeight.Key,
                wordWeight => wordWeight.Value / wordsEntries.Count());

            return wordsAdjustedWeights;
        }

        private static string GetWordMatchPattern()
        {
            return Language switch
            {
                "Russian" => "[а-яА-Я\\-]+",
                "Deutsch" => "[a-zA-ZäöüÄÖÜß\\-]+",
                _ => throw new ArgumentException($"Language {Language} is not supported")
            };
        }
    }
}
