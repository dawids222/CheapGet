namespace LibLite.CheapGet.Core.Stores.Games.GoG
{
    public class GogProduct : Product
    {
        public override string StoreName => "GoG";

        public GogProduct(
            string name,
            double basePrice,
            double discountedPrice,
            string imgUrl)
            : base(name, basePrice, discountedPrice, imgUrl) { }
    }
}
