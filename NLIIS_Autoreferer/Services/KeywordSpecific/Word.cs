using System;

namespace NLIIS_Autoreferer.Services.KeywordSpecific
{
    internal class Word
    {
        public string Value { get; set; }
        public string Stem { get; set; }
        public double TermFrequency { get; set; }

        public Word() { }
        public Word(string word) { Value = word; }

        public override bool Equals(object obj)
        {
            if (GetType() != obj.GetType())
            {
                return false;
            }

            var arg = (Word)obj;
            return Value.Equals(arg.Value, StringComparison.CurrentCultureIgnoreCase);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
