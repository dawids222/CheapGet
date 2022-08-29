namespace LibLite.CheapGet.Core.CGQL.Expressions
{
    public class SortDirection : Expression
    {
        public string Value { get; set; }

        public SortDirection(string value)
        {
            Value = value;
        }

        public override bool Equals(object obj)
        {
            return obj is SortDirection direction &&
                   Value == direction.Value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value);
        }
    }
}
