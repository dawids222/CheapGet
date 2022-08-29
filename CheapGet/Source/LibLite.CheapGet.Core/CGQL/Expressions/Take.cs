namespace LibLite.CheapGet.Core.CGQL.Expressions
{
    public class Take : Expression
    {
        public Integer Value { get; set; }

        public Take(Integer value)
        {
            Value = value;
        }

        public override bool Equals(object obj)
        {
            return obj is Take take &&
                   EqualityComparer<Integer>.Default.Equals(Value, take.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value);
        }
    }
}
