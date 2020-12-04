namespace NLIIS_Autoreferer.Services.KeywordSpecific
{
    public static class Summarizer
    {
        public static SummarizedDocument Summarize(SummarizerArguments args)
        {
            var article = ParseDocument(args.InputString, args);
            Grader.Grade(article);
            
            return CreateSummarizedDocument(article);
        }

        private static SummarizedDocument CreateSummarizedDocument(Article article)
        {
            var sumDoc = new SummarizedDocument { Concepts = article.Concepts };
            return sumDoc;
        }

        private static Article ParseDocument(string text, SummarizerArguments args)
        {
            var rules = Dictionary.LoadFromFile(args.DictionaryLanguage);
            var article = new Article(rules);
            article.ParseText(text);
            
            return article;
        }
    }
}
