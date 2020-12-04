using System.Collections.Generic;

namespace NLIIS_Autoreferer.Services.KeywordSpecific
{
    public class SummarizedDocument
    {
        public List<string> Concepts { get; set; }

        internal SummarizedDocument()
        {
            Concepts = new List<string>();
        }
    }
}
