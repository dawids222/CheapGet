namespace LibLite.CheapGet.Core.Stores.Games.Steam
{
    public class SteamProduct : Product
    {
        public override string StoreName => "Steam";

        public SteamProduct(
            string name,
            double basePrice,
            double discountedPrice,
            string imgUrl,
            string url)
            : base(name, basePrice, discountedPrice, imgUrl, url) { }
    }
}
