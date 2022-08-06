namespace LibLite.CheapGet.Core.Stores
{
    public abstract class Product
    {
        public abstract string StoreName { get; }

        public string Name { get; init; }
        public double BasePrice { get; init; }
        public double DiscountedPrice { get; init; }
        public string ImgUrl { get; init; }
        public string Url { get; init; }

        public double DiscountPercentage => CalculateDiscountPercentage();
        public double DiscountValue => CalculateDiscountValue();

        public Product(
            string name,
            double basePrice,
            double discountedPrice,
            string imgUrl,
            string url)
        {
            Name = name;
            BasePrice = basePrice;
            DiscountedPrice = discountedPrice;
            ImgUrl = imgUrl;
            Url = url;
        }

        private double CalculateDiscountPercentage()
        {
            var value = 100 - (100 * DiscountedPrice / BasePrice);
            return Math.Round(value, 2);
        }

        private double CalculateDiscountValue()
        {
            var value = BasePrice - DiscountedPrice;
            return Math.Round(value, 2);
        }
    }
}
