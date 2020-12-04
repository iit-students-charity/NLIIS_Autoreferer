using System;
using System.Collections.Generic;
using System.Drawing;
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
        
        public static string ToPDF(string text)
        {
            using var pdf = new PdfDocument();
            
            var page = pdf.Pages[0];
            var textBox = page.AddTextBox("type", 10, 10, 500, 1000);
            textBox.Text = text;
            textBox.FontSize = 14;
            textBox.Multiline = true;
            textBox.BackgroundColor = new PdfRgbColor(Color.Transparent);
            textBox.ShowBorder = false;
            textBox.ReadOnly = true;

            var path = $"D:\\{text.Substring(0, 20)}.pdf";
            pdf.Save(path);
            
            return path;
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
                "Russian" => "[а-яА-Я\\-]{2,}",
                "Deutsch" => "[a-zA-ZäöüÄÖÜß\\-]{2,}",
                _ => throw new ArgumentException($"Language {Language} is not supported")
            };
        }
    }
}
