using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLIIS_Autoreferer.Services.KeywordSpecific
{
    internal class Article
    {
        public int LineCount { get; set; }
        public List<string> Concepts { get; set; }
        public Dictionary Rules { get; set; }

        public List<Word> ImportantWords { get; set; }
        public List<Word> WordCounts { get; set; }


        public Article(Dictionary rules) { 
            WordCounts = new List<Word>();
            Concepts = new List<string>();
            Rules = rules;
        }

        public void ParseText(string text)
        {
            var words = text.Split(' ', '\r'); //space and line feed characters are the ends of words.
            var originalSentence = new StringBuilder();
            foreach (var word in words)
            {
                var locWord = word;
                if (locWord.StartsWith("\n") && word.Length > 2) locWord = locWord.Replace("\n", "");
                originalSentence.AppendFormat("{0} ", locWord);
                AddWordCount(locWord);
                if (IsSentenceBreak(locWord))
                {
                    originalSentence = new StringBuilder();
                }
            }
        }

        private void AddWordCount(string word)
        {
            var stemmedWord = Stemmer.StemWord(word, this.Rules);
            if (string.IsNullOrEmpty(word) || word == " " || word == "\n" || word == "\t") return;            
            var foundWord = WordCounts.Find(match => match.Stem == stemmedWord.Stem);
            if (foundWord == null)
            {
                WordCounts.Add(stemmedWord);
            }
            else
            {
                foundWord.TermFrequency++;
            }

        }

        private bool IsSentenceBreak(string word)
        {
            if (word.Contains("\r") || word.Contains("\n")) return true;
            bool shouldBreak = (Rules.LinebreakRules.Any(p => word.EndsWith(p, StringComparison.CurrentCultureIgnoreCase)));

            

            if (shouldBreak == false) return shouldBreak;

            shouldBreak = (!Rules.NotALinebreakRules.Any(p => word.StartsWith(p, StringComparison.CurrentCultureIgnoreCase)));


            return shouldBreak;
        }

    }
}
