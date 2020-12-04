using System.Collections.Generic;
using System.Security.Permissions;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Reflection;

namespace NLIIS_Autoreferer.Services.KeywordSpecific
{
    internal class Dictionary
    {
        public List<Word> UnimportantWords { get; set; }
        public List<string> LinebreakRules { get; set; }
        public List<string> NotALinebreakRules { get; set; }
        public List<string> DepreciateValueRule { get; set; }
        public List<string> TermFreqMultiplierRule { get; set; }
        public Dictionary<string, string> Step1PrefixRules { get; set; } 
        public Dictionary<string, string> Step1SuffixRules { get; set; }
        public Dictionary<string, string> ManualReplacementRules { get; set; }
        public Dictionary<string, string> PrefixRules { get; set; }
        public Dictionary<string, string> SuffixRules { get; set; }
        public Dictionary<string, string> SynonymRules { get; set; }

        [FileIOPermission(SecurityAction.Demand, Read="$AppDir$\\")]
        public static Dictionary LoadFromFile(string dictionaryLanguage)
        {
            var dictionaryFile = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}" +
                                 $"\\Services\\KeywordSpecific\\{dictionaryLanguage}.xml";

            var doc = XElement.Load(dictionaryFile);
            var dict = new Dictionary
            {
                Step1PrefixRules = LoadKeyValueRule(doc, "stemmer", "step1_pre"),
                Step1SuffixRules = LoadKeyValueRule(doc, "stemmer", "step1_post"),
                ManualReplacementRules = LoadKeyValueRule(doc, "stemmer", "manual"),
                PrefixRules = LoadKeyValueRule(doc, "stemmer", "pre"),
                SuffixRules = LoadKeyValueRule(doc, "stemmer", "post"),
                SynonymRules = LoadKeyValueRule(doc, "stemmer", "synonyms"),
                LinebreakRules = LoadValueOnlyRule(doc, "parser", "linebreak"),
                NotALinebreakRules = LoadValueOnlyRule(doc, "parser", "linedontbreak"),
                DepreciateValueRule = LoadValueOnlyRule(doc, "grader-syn", "depreciate"),
                TermFreqMultiplierRule = LoadValueOnlySection(doc, "grader-tf"),
                UnimportantWords = new List<Word>()
            };

            var unimpwords = LoadValueOnlySection(doc, "grader-tc");
            
            foreach (var unimpword in unimpwords)
            {
                dict.UnimportantWords.Add(new Word(unimpword));
            }
            return dict;
        }

        private static List<string> LoadValueOnlySection(XElement doc, string section)
        {
            var step1pre = doc.Elements(section);

            return step1pre.Elements().Select(x => x.Value).ToList();
        }

        private static List<string> LoadValueOnlyRule(XElement doc, string section, string container)
        {
            var step1pre = doc.Elements(section).Elements(container);

            return step1pre.Elements().Select(x => x.Value).ToList();
        }

        private static Dictionary<string, string> LoadKeyValueRule(XElement doc, string section, string container)
        {
            var dictionary = new Dictionary<string, string>();
            var step1pre = doc.Elements(section).Elements(container);
            
            foreach (var x in step1pre.Elements())
            {
                var rule = x.Value;
                var keyvalue = rule.Split('|');
                
                if (!dictionary.ContainsKey(keyvalue[0]))
                {
                    dictionary.Add(keyvalue[0], keyvalue[1]);
                }
            }
            
            return dictionary;
        }
    }
}
