namespace NLIIS_Autoreferer.Services
{
    public class KeywordReferer : IReferer
    {
        public string GenerateRefer(string text)
        {
            var words = DocumentService.GetTerms(text);
            return null;
        }

        public void Clean()
        {
        }
    }
}