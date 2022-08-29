using LibLite.CheapGet.Core.CGQL.Enums;

namespace LibLite.CheapGet.Core.CGQL.Expressions
{
    public abstract class Literal : Expression
    {
        public abstract TokenType Type { get; }

        public Text AsText() => this as Text;
        public Integer AsInteger() => this as Integer;
        public Floating AsDecimal() => this as Floating;
    }
}
