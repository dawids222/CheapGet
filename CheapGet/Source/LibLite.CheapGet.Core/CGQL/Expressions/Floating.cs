using LibLite.CheapGet.Core.CGQL.Enums;

namespace LibLite.CheapGet.Core.CGQL.Expressions
{
    public class Floating : Literal // TODO: Consider a better name...
    {
        public override TokenType Type => TokenType.FLOATING;

        public double Value { get; set; }

        public Floating(double value)
        {
            Value = value;
        }

        public override bool Equals(object obj)
        {
            return obj is Floating @decimal &&
                   Type == @decimal.Type &&
                   Value == @decimal.Value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, Value);
        }
    }
}
