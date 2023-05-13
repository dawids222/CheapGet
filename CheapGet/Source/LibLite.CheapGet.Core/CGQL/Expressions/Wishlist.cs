namespace LibLite.CheapGet.Core.CGQL.Expressions
{
    public class Wishlist : Expression
    {
        public const string DEFAULT_FROM = "Games";
        public const int DEFAULT_MAX = 100;

        public From From { get; set; } = new(new Text(DEFAULT_FROM));
        public Max Max { get; set; } = new(new Integer(DEFAULT_MAX));
        public List<Wish> Wishes { get; set; } = new();

        public override bool Equals(object obj)
        {
            return obj is Wishlist wishlist &&
                   EqualityComparer<From>.Default.Equals(From, wishlist.From) &&
                   EqualityComparer<Max>.Default.Equals(Max, wishlist.Max) &&
                   Wishes.SequenceEqual(wishlist.Wishes);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(From, Max, Wishes);
        }
    }
}
