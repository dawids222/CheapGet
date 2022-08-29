namespace LibLite.CheapGet.Core.CGQL.Expressions
{
    public class Filter : Expression
    {
        public Text Property { get; set; }
        public Comparison Comparison { get; set; }
        public Literal Value { get; set; }

        public Filter(Text property, Comparison comparison, Literal value)
        {
            Property = property;
            Comparison = comparison;
            Value = value;
        }

        public override bool Equals(object obj)
        {
            return obj is Filter filter &&
                   EqualityComparer<Text>.Default.Equals(Property, filter.Property) &&
                   EqualityComparer<Comparison>.Default.Equals(Comparison, filter.Comparison) &&
                   EqualityComparer<Literal>.Default.Equals(Value, filter.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Property, Comparison, Value);
        }
    }
}
