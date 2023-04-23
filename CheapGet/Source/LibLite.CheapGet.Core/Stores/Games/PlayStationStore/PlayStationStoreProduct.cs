namespace LibLite.CheapGet.Core.Stores.Games.PlayStationStore
{
    public class PlayStationStoreProduct : Product
    {
        public override string StoreName => "PlayStationStore";

        public PlayStationStoreProduct(
            string name,
            double basePrice,
            double discountedPrice,
            string imgUrl,
            string url)
            : base(name, basePrice, discountedPrice, imgUrl, url) { }
    }
}
