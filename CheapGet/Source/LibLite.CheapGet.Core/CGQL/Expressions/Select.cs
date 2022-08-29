namespace LibLite.CheapGet.Core.CGQL.Expressions
{
    public class Select : Expression
    {
        public From From { get; set; } = new From(new Text("Games"));
        public Take Take { get; set; } = new Take(new Integer(100));
        public List<Filter> Filters { get; set; } = new();
        public List<Sort> Sorts { get; set; } = new();

        public override bool Equals(object obj)
        {
            return obj is Select select &&
                   EqualityComparer<From>.Default.Equals(From, select.From) &&
                   EqualityComparer<Take>.Default.Equals(Take, select.Take) &&
                   Filters.SequenceEqual(select.Filters) &&
                   Sorts.SequenceEqual(select.Sorts);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(From, Take, Filters, Sorts);
        }
    }
}
