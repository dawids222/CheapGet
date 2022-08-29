namespace LibLite.CheapGet.Core.CGQL.Expressions
{
    public class Sort : Expression
    {
        public Text Property { get; set; }
        public SortDirection Direction { get; set; }

        public Sort(Text property, SortDirection direction)
        {
            Property = property;
            Direction = direction;
        }

        public override bool Equals(object obj)
        {
            return obj is Sort sort &&
                   EqualityComparer<Text>.Default.Equals(Property, sort.Property) &&
                   EqualityComparer<SortDirection>.Default.Equals(Direction, sort.Direction);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Property, Direction);
        }
    }
}
