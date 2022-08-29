using LibLite.CheapGet.Core.CGQL.Enums;

namespace LibLite.CheapGet.Core.CGQL.Expressions
{
    public class Text : Literal
    {
        public override TokenType Type => TokenType.TEXT;

        public string Value { get; set; }

        public Text(string value)
        {
            Value = value;
        }

        public override bool Equals(object obj)
        {
            return obj is Text text &&
                   Type == text.Type &&
                   Value == text.Value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, Type, Value);
        }
    }
}
