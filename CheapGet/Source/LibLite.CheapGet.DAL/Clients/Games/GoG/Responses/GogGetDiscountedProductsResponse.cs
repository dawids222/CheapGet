namespace LibLite.CheapGet.DAL.Clients.Games.GoG.Responses
{
    public class GogGetDiscountedProductsResponse
    {
        public int Pages { get; set; }
        public int ProductCount { get; set; }
        public IEnumerable<Product> Products { get; set; }

        public class Product
        {
            public string Title { get; set; }
            public string Slug { get; set; }
            public string CoverHorizontal { get; set; }
            public Price Price { get; set; }
        }

        public class Price
        {
            public Money BaseMoney { get; set; }
            public Money FinalMoney { get; set; }
        }

        public class Money
        {
            public double Amount { get; set; }
        }
    }
}
