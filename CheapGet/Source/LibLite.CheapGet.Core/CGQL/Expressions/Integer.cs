using LibLite.CheapGet.Core.CGQL.Enums;

namespace LibLite.CheapGet.Core.CGQL.Expressions
{
    public class Integer : Literal
    {
        public override TokenType Type => TokenType.INTEGER;

        public int Value { get; set; }

        public Integer(int value)
        {
            Value = value;
        }

        public override bool Equals(object obj)
        {
            return obj is Integer integer &&
                   Type == integer.Type &&
                   Value == integer.Value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, Value);
        }
    }
}
