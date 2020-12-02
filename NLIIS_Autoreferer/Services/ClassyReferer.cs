using System;
using System.Collections.Generic;
using System.Linq;

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
        public IEnumerable<string> Terms { get; set; }
        public decimal Weight { get; set; }
    }
    
    public class ClassyReferer : IReferer
    {
        private decimal AllSymbolsCount { get; set; }
        private decimal MaxTermFrequency { get; set; }
        private IEnumerable<Sentence> Sentences { get; set; }
        private IEnumerable<Paragraph> Paragraphs { get; set; }

        public ClassyReferer()
        {
            Paragraphs = new List<Paragraph>();
            Sentences = new List<Sentence>();
        }
        
        public string GenerateRefer(string text)
        {
            AllSymbolsCount = text.Length;

            {
                var paragraphCounter = 0;
                var sentenceCounter = 0;
                Paragraphs = text.Split("\n").Select(paragraphText => 
                    new Paragraph
                    {
                        Number = paragraphCounter++,
                        Text = paragraphText,
                        Sentences = paragraphText.Split(".").Select(sentenceText =>
                        {
                            var newSentence = new Sentence
                            {
                                Number = sentenceCounter++,
                                Text = sentenceText,
                                Terms = DocumentService.GetWords(sentenceText)
                            };
                            Sentences = Sentences.Append(newSentence);
                            
                            return newSentence;
                        })
                    });

                MaxTermFrequency = DocumentService.GetTermsFrequencies(text).Max(wordWeight => wordWeight.Value);
            }

            var sentenceToChoose = new List<Sentence>();

            foreach (var sentence in Sentences)
            {
                var sentenceWeight = PositionInDocument(sentence.Number) * PositionInParagraph(sentence.Number) * Score(sentence);
                sentence.Weight = sentenceWeight;
            }

            sentenceToChoose = Sentences.OrderByDescending(sentence => sentence.Weight).Take(10).ToList();
            return string.Join(".", Sentences.Where(sentence => sentenceToChoose.Any(_ => _.Number == sentence.Number)));
        }

        private int CountSymbolsBeforeInDocument(int sentenceNumber)
        {
            return Sentences
                .TakeWhile(sentence => sentence.Number != sentenceNumber)
                .Sum(sentence => sentence.Text.Length);
        }

        private int CountSymbolsBeforeInParagraph(int sentenceNumber)
        {
            return Paragraphs.Single(paragraph => paragraph.Sentences.Any(sentence => sentence.Number == sentenceNumber))
                .Sentences.TakeWhile(sentence => sentence.Number != sentenceNumber)
                .Sum(sentence => sentence.Text.Length);
        }

        private decimal PositionInDocument(int sentenceNumber)
        {
            return 1 - CountSymbolsBeforeInDocument(sentenceNumber) / AllSymbolsCount;
        }

        private decimal PositionInParagraph(int sentenceNumber)
        {
            return 1 - CountSymbolsBeforeInParagraph(sentenceNumber) / Paragraphs.Single(paragraph => paragraph.Sentences.Any(sentence => sentence.Number == sentenceNumber)).Text.Length;
        }

        private decimal Score(Sentence sentence)
        {
            return sentence.Terms.Sum(term => TermFrequency(term, sentence.Number) * w(term));
        }

        private int TermFrequency(string term, int sentenceNumber)
        {
            var sentence = Sentences.Single(_ => _.Number == sentenceNumber);

            return sentence.Terms.Count(_ => _.Equals(term));
        }

        private int TermFrequency(string term)
        {
            return Sentences.Sum(sentence => sentence.Terms.Count(_ => _.Equals(term)));
        }

        private decimal w(string term)
        {
            return 0.5m * (1m + TermFrequency(term) / MaxTermFrequency) * (decimal)Math.Log(1);
        }
    }
}