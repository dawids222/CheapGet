namespace LibLite.CheapGet.Core.CGQL.Expressions
{
    public class Wish : Expression
    {
        public List<Filter> Filters { get; set; } = new();

        public override bool Equals(object obj)
        {
            return obj is Wish wish &&
                   Filters.SequenceEqual(wish.Filters);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Filters);
        }
    }
}
