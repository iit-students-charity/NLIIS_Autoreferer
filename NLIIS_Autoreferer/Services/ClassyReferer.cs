using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NLIIS_Autoreferer.Services
{
    public class Paragraph
    {
        public string Text { get; set; }
        public int Number { get; set; }
        public IEnumerable<Sentence> Sentences { get; set; }
    }

    public class Sentence
    {
        public string Text { get; set; }
        public int Number { get; set; }
        public int ParagraphNumber { get; set; }
        public IEnumerable<string> Terms { get; set; }
        public decimal Weight { get; set; }
    }
    
    public class ClassyReferer : IReferer
    {
        private decimal AllSymbolsCount { get; set; }
        private IDictionary<string, int> TermsFrequencies { get; set; }
        private decimal MaxTermFrequency { get; set; }
        private List<Sentence> Sentences { get; set; }
        private List<Paragraph> Paragraphs { get; set; }

        public ClassyReferer()
        {
            Paragraphs = new List<Paragraph>();
            Sentences = new List<Sentence>();
        }
        
        public string GenerateRefer(string text)
        {
            AllSymbolsCount = text.Length;

            {
                var paragraphCounter = -1;
                var sentenceCounter = 0;
                Paragraphs = text
                    .Split(Environment.NewLine)
                    .Where(_ => Regex.Match(_, DocumentService.GetWordMatchPattern()).Success)
                    .Select(paragraphText => 
                    new Paragraph
                    {
                        Number = paragraphCounter++,
                        Text = paragraphText,
                        Sentences = paragraphText
                            .Split(".")
                            .Where(_ => Regex.Match(_, DocumentService.GetWordMatchPattern()).Success)
                            .Select(sentenceText =>
                        {
                            var newSentence = new Sentence
                            {
                                Number = sentenceCounter++,
                                Text = sentenceText,
                                Terms = DocumentService.GetWords(sentenceText),
                                ParagraphNumber = paragraphCounter
                            };
                            
                            return newSentence;
                        })
                    })
                    .ToList();

                foreach (var paragraph in Paragraphs)
                {
                    Sentences.AddRange(paragraph.Sentences);
                }

                TermsFrequencies = DocumentService.GetTermsFrequencies(text);
                MaxTermFrequency = TermsFrequencies.Max(wordWeight => wordWeight.Value);
            }

            foreach (var sentence in Sentences)
            {
                var positionInDocument = PositionInDocument(sentence);
                var positionInParagraph = PositionInParagraph(sentence);
                var score = Score(sentence);
                sentence.Weight = positionInDocument * positionInParagraph * score;
            }

            var sentenceToChoose = Sentences
                .OrderByDescending(sentence => sentence.Weight)
                .Take(10)
                .OrderBy(sentence => sentence.Number)
                .ToList();
            return string.Join(".", sentenceToChoose.Select(sentence => sentence.Text));
        }

        public void Clear()
        {
            AllSymbolsCount = 0;
            TermsFrequencies.Clear();
            MaxTermFrequency = 0;
            Sentences.Clear();
            Paragraphs.Clear();
        }

        private int CountSymbolsBeforeInDocument(Sentence sentence)
        {
            return Sentences
                .TakeWhile(_ => _.Number != sentence.Number)
                .Sum(_ => _.Text.Length);
        }

        private int CountSymbolsBeforeInParagraph(Sentence sentence)
        {
            return Paragraphs.ElementAt(sentence.ParagraphNumber)
                .Sentences.TakeWhile(_ => _.Number != sentence.Number)
                .Sum(_ => _.Text.Length);
        }

        private decimal PositionInDocument(Sentence sentence)
        {
            return 1 - CountSymbolsBeforeInDocument(sentence) / AllSymbolsCount;
        }

        private decimal PositionInParagraph(Sentence sentence)
        {
            return 1 - CountSymbolsBeforeInParagraph(sentence) / Paragraphs.ElementAt(sentence.ParagraphNumber).Text.Length;
        }

        private decimal Score(Sentence sentence)
        {
            return sentence.Terms.Sum(term =>
            {
                var termFrequencyInSentence = TermFrequencyInSentence(term, sentence);
                var w = WEIRD(term);

                return termFrequencyInSentence * w;
            });
        }

        private int TermFrequencyInSentence(string term, Sentence sentence)
        {
            return sentence.Terms.Count(_ => _.Equals(term));
        }

        private int TermFrequencyInDocument(string term)
        {
            return TermsFrequencies[term];
        }

        private decimal WEIRD(string term)
        {
            var a = 1m + TermFrequencyInDocument(term) / MaxTermFrequency;
            var countOfSentencesWithTerm = Sentences.Count(sentence => sentence.Terms.Contains(term)) + 1;
            var log = (decimal)Math.Log(Sentences.Count, countOfSentencesWithTerm);
            
            return a * log / 2;
        }
    }
}