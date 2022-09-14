namespace LibLite.CheapGet.Core.CGQL.Expressions
{
    public class Load : Expression
    {
        public Text Source { get; set; }

        public Load(Text source)
        {
            Source = source;
        }

        public override bool Equals(object obj)
        {
            return obj is Load load &&
                   EqualityComparer<Text>.Default.Equals(Source, load.Source);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Source);
        }
    }
}
