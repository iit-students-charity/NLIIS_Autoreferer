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
        
        public static IEnumerable<string> GetWords(string text)
        {
            var pattern = GetWordMatchPattern();
            return Regex.Matches(text, pattern).Select(match => match.Value);
        }
        
        public static IEnumerable<string> GetTerms(string text)
        {
            var wordsEntries = GetWords(text);
            var uniqueWords = wordsEntries.ToHashSet().AsEnumerable();
            
            return uniqueWords;
        }
        
        public static IDictionary<string, int> GetTermsFrequencies(string text)
        {
            var wordsEntries = GetWords(text);
            var termsFrequencies = new Dictionary<string, int>();

            foreach (var wordEntry in wordsEntries)
            {
                var currentWeight = 0;
                
                if (termsFrequencies.ContainsKey(wordEntry))
                {
                    termsFrequencies.TryGetValue(wordEntry, out currentWeight);
                    termsFrequencies.Remove(wordEntry);
                }
                
                termsFrequencies.Add(wordEntry, ++currentWeight);
            }

            return termsFrequencies;
        }

        public static string GetWordMatchPattern()
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
