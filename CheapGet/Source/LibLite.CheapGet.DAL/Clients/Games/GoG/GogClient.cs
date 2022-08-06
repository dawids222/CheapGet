using LibLite.CheapGet.Core.Services;
using LibLite.CheapGet.Core.Stores;
using LibLite.CheapGet.Core.Stores.Games.GoG;
using LibLite.CheapGet.DAL.Clients.Games.GoG.Responses;

namespace LibLite.CheapGet.DAL.Clients.Games.GoG
{
    public class GogClient : IGogClient
    {
        public const string PRODUCT_PAGE_URL_TEMPLATE = "https://www.gog.com/game/";
        private const int PRODUCTS_PER_REQUEST = 100;

        private readonly IHttpClient _httpClient;

        public GogClient(IHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Product>> GetDiscountedProductsAsync(int start, int count, CancellationToken token)
        {
            var startPage = start / PRODUCTS_PER_REQUEST;
            var endPage = (start + count - 1) / PRODUCTS_PER_REQUEST;
            var pagesCount = endPage - startPage + 1;
            var pages = Enumerable.Range(startPage, pagesCount);

            var tasks = pages
                .Select(page => GetDiscountedProductsAsync(page, CancellationToken.None))
                .ToList();

            var results = await Task.WhenAll(tasks);
            var products = results
                .SelectMany(products => products
                    .Select(product => product));

            var skip = start % PRODUCTS_PER_REQUEST;
            return products
                .Skip(skip)
                .Take(count)
                .ToList();
        }

        private async Task<IEnumerable<GogProduct>> GetDiscountedProductsAsync(int page, CancellationToken token)
        {
            var url = $"https://catalog.gog.com/v1/catalog?limit={PRODUCTS_PER_REQUEST}&order=desc%3Atrending&discounted=eq%3Atrue&productType=in%3Agame%2Cpack&page={page + 1}&countryCode=PL&locale=pl-PL&currencyCode=PLN";
            var response = await _httpClient.GetAsync<GogGetDiscountedProductsResponse>(url, CancellationToken.None);

            return response
                .Products
                .Select(x => new GogProduct(
                    x.Title,
                    x.Price.BaseMoney.Amount,
                    x.Price.FinalMoney.Amount,
                    x.CoverHorizontal,
                    ToProductUrl(x.Slug)))
                .ToList();
        }

        private static string ToProductUrl(string slug)
        {
            var name = slug.Replace('-', '_');
            return $"{PRODUCT_PAGE_URL_TEMPLATE}{name}";
        }
    }
}
