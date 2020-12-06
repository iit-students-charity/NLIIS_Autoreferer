using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace NLIIS_Autoreferer.Services
{
    public static class DocumentService
    {
        public static string Language { get; set; }
        
        public static string FromFile(string path)
        {
            return File.ReadAllText(path);
        }
        
        public static string ToFile(string text, string name)
        {
            var path = $"D:\\{name}.txt";
            using var file = new StreamWriter(path);
            
            file.WriteLine(text);

            return path;
        }
        
        public static IEnumerable<string> GetWords(string text, bool removeUseless = false)
        {
            var pattern = GetWordMatchPattern();
            var words = Regex.Matches(text, pattern)
                .Select(match => removeUseless ? match.Value.ToLower() : match.Value);

            if (!removeUseless)
            {
                return words;
            }

            var dummies = GetDummyWords();
            words = words.Where(word => !dummies.Contains(word));

            return words;
        }
        
        public static IEnumerable<string> GetPhrases(string term, string text)
        {
            var pattern = GetWordMatchPattern();
            var rawPhrases = Regex.Matches(text, $"{pattern}\\s{term}|{term}\\s{pattern}", RegexOptions.Compiled)
                .Select(match => match.Value);
            
            var phrases = rawPhrases.ToHashSet().AsEnumerable();

            return phrases;
        }
        
        public static IEnumerable<string> GetTerms(string text, bool removeUseless)
        {
            var wordsEntries = GetWords(text, removeUseless);
            var uniqueWords = wordsEntries.ToHashSet().AsEnumerable();
            
            return uniqueWords;
        }
        
        public static IDictionary<string, int> GetTermsFrequencies(string text, bool removeUseless = false)
        {
            var wordsEntries = GetWords(text, removeUseless);
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
                "Russian" => "[а-яА-Я\\-]{3,}",
                "Deutsch" => "[a-zA-ZäöüÄÖÜß\\-]{3,}",
                _ => throw new ArgumentException($"Language {Language} is not supported")
            };
        }

        public static IEnumerable<string> GetDummyWords()
        {
            return Language switch
            {
                "Russian" => new List<string>() { "она", "они", "ему", "как", "где", "что", "кто", "раз", "два",
                    "один", "три", "четыре", "пять", "шесть", "семь", "восемь", "девять", "десять", "кем", "чем",
                    "или", "зачем", "почему", "откуда", "куда", "каким", "лучше", "лучший", "либо", "таким",
                    "напротив", "лишь", "может", "могут", "даже", "немного", "также", "скоро", "кажется", "для",
                    "таких", "более", "того", "чтобы", "это", "тем", "этой", "всей", "иногда", "одной",
                    "например", "кому", "кому-то", "каким-либо", "нужен", "ради", "очень", "гораздо", "точно", "двум",
                    "быть", "которые", "являются", "является", "есть", "обычно", "которая", "которые", "которую", "которым",
                    "тот", "тому", "такой", "третий", "четвертый", "пятый", "другой", "второй", "первый", "зря", },
                "Deutsch" => new List<string>() { "eins", "zwei", "drei", "vier", "fünf", "sechs", "sieben", "acht",
                    "neun", "zehn", "elf", "zwölf", "das", "der", "die", "sein", "tun",
                    "können", "haben", "werden", "sagen", "machen", "brauchen", "bekommen", "tun" },
                _ => throw new ArgumentException($"Language {Language} is not supported")
            };
        }

        public static bool IsAdjective(string term)
        {
            if (term.Length < 5)
            {
                return false;
            }
            
            return Language switch
            {
                "Russian" => term.EndsWith("ый") || term.EndsWith("ая") || term.EndsWith("ого") || term.EndsWith("ой") ||
                             term.EndsWith("ему") || term.EndsWith("ей") || term.EndsWith("его") || term.EndsWith("ую") ||
                             term.EndsWith("им") || term.EndsWith("ем") || term.EndsWith("ом") || term.EndsWith("ой") ||
                             term.EndsWith("ое"),
                "Deutsch" => false,
                _ => throw new ArgumentException($"Language {Language} is not supported")
            };
        }
    }
}
