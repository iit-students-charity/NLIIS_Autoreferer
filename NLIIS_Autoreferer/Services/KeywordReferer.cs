namespace NLIIS_Autoreferer.Services
{
    public class KeywordReferer : IReferer
    {
        public string GenerateRefer(string text)
        {
            var words = DocumentService.GetWordsSet(text);
            return null;
        }
    }
}