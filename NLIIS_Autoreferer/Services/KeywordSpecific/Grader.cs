using System.Collections.Generic;
using System.Linq;

namespace NLIIS_Autoreferer.Services.KeywordSpecific
{
    internal static class Grader
    {
        public static void Grade(Article article)
        {
            CreateImportantWordsList(article);

            ExtractArticleConcepts(article);
        }

        private static void ExtractArticleConcepts(Article article)
        {
            article.Concepts = new List<string> ();
            var baseFreq = article.ImportantWords[5].TermFrequency;
            
            if (article.ImportantWords.Count > 5)
            {
                article.Concepts = article.ImportantWords.Where(p => p.TermFrequency >= baseFreq).Select(p => p.Value).ToList();                
            }
            else
            {
                foreach (var word in article.ImportantWords)
                {
                    article.Concepts.Add(word.Value);
                }
            }
        }

        private static void CreateImportantWordsList(Article article)
        {
            var wordsArray = new Word[article.WordCounts.Count];
            article.WordCounts.CopyTo(wordsArray);
            article.ImportantWords = new List<Word>(wordsArray);

            foreach (var foundWord in article.Rules.UnimportantWords.Select(word => article.ImportantWords.Find(match => match.Value.ToLower().Equals(word.Value.ToLower()))).Where(foundWord => foundWord != null))
            {
                article.ImportantWords.Remove(foundWord);
            }
            article.ImportantWords.Sort(CompareWordsByFrequency);
        }

        private static int CompareWordsByFrequency(Word lhs, Word rhs)
        {
            return rhs.TermFrequency.CompareTo(lhs.TermFrequency);
        }
    }
}
