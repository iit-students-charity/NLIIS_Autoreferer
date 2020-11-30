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
        public static string FromPDF(string path)
        {
            using var pdf = new PdfDocument(path);
            
            return pdf.GetText();
        }
        
        public static IEnumerable<string> GetWordsSet(string text, string language)
        {
            var pattern = GetWordMatchPattern(language);
            var wordsEntries = Regex.Matches(text, pattern);
            var uniqueWords = wordsEntries.Select(match => match.Value).ToHashSet().AsEnumerable();
            
            return uniqueWords;
        }

        private static string GetWordMatchPattern(string language)
        {
            switch (language)
            {
                case "Russian":
                {
                    return "[а-яА-Я\\-]+";
                }
                case "Deutsch":
                {
                    return "[a-zA-ZäöüÄÖÜß\\-]+";
                }
                default:
                {
                    throw new ArgumentException($"Language {language} is not supported");
                }
            }
        }
    }
}
