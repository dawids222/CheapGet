namespace LibLite.CheapGet.Core.Stores.Games.Steam
{
    public class SteamProduct : Product
    {
        public override string StoreName => "Steam";

        public SteamProduct() { }
        public SteamProduct(
            string name,
            double basePrice,
            double discountedPrice)
            : base(name, basePrice, discountedPrice) { }
    }
}
