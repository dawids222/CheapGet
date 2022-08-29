namespace LibLite.CheapGet.Core.CGQL.Expressions
{
    public class Comparison : Expression
    {
        public string Value { get; set; }

        public Comparison(string value)
        {
            Value = value;
        }

        public override bool Equals(object obj)
        {
            return obj is Comparison comparison &&
                   Value == comparison.Value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value);
        }
    }
}
