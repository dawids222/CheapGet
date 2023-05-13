namespace LibLite.CheapGet.Core.CGQL.Expressions
{
    public class Max : Expression
    {
        public Integer Value { get; set; }

        public Max(Integer value)
        {
            Value = value;
        }

        public override bool Equals(object obj)
        {
            return obj is Max max &&
                   EqualityComparer<Integer>.Default.Equals(Value, max.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value);
        }
    }
}
