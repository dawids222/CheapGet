namespace LibLite.CheapGet.DAL.Clients.Games.PlayStationStore.Responses
{
    public class PlayStationStoreGetDiscountedProductsResponse
    {
        public ProductData Data { get; set; }

        public class ProductData
        {
            public CategoryGrid CategoryGridRetrieve { get; set; }
        }

        public class CategoryGrid
        {
            public IEnumerable<Product> Products { get; set; }
        }

        public class Product
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public Price Price { get; set; }
            public IEnumerable<Media> Media { get; set; }
        }

        public class Media
        {
            public string Role { get; set; }
            public string Type { get; set; }
            public string Url { get; set; }
        }

        public class Price
        {
            public string BasePrice { get; set; }
            public string DiscountedPrice { get; set; }
        }
    }
}
