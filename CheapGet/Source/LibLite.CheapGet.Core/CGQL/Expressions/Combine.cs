namespace LibLite.CheapGet.Core.CGQL.Expressions
{
    public class Combine : Expression
    {
        public List<Select> Selects { get; set; } = new();

        public override bool Equals(object obj)
        {
            return obj is Combine combine &&
                   Enumerable.SequenceEqual(Selects, combine.Selects);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Selects);
        }
    }
}
