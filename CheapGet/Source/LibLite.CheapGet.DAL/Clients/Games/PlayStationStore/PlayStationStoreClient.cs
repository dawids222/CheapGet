using LibLite.CheapGet.Core.Services;
using LibLite.CheapGet.Core.Stores;
using LibLite.CheapGet.Core.Stores.Games.PlayStationStore;
using LibLite.CheapGet.DAL.Clients.Games.PlayStationStore.Responses;
using System.Text.RegularExpressions;

namespace LibLite.CheapGet.DAL.Clients.Games.PlayStationStore
{
    public class PlayStationStoreClient : IPlayStationStoreClient
    {
        private const string AUTH_URL = "https://store.playstation.com/pl-pl/pages/deals";
        private const string AUTH_TOKEN_REGEX = "(?<=\"categoryId\":\")(.*?)(?=\",)";

        private const string PRODUCT_PAGE_URL_TEMPLATE = "https://store.playstation.com/pl-pl/product/";
        private const int PRODUCTS_PER_REQUEST = 100;

        private readonly Dictionary<string, string> HEADERS = new() { { "x-psn-store-locale-override", "pl-PL" } };

        private readonly IHttpClient _httpClient;

        public PlayStationStoreClient(IHttpClient httpClient)
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

        private async Task<IEnumerable<PlayStationStoreProduct>> GetDiscountedProductsAsync(int page, CancellationToken token)
        {
            var authToken = await GetAuthTokenAsync(token);
            var url = "https://web.np.playstation.com/api/graphql/v1//op?operationName=categoryGridRetrieve&variables={\"id\":\"" + authToken + "\",\"pageArgs\":{\"size\":" + PRODUCTS_PER_REQUEST + ",\"offset\":" + PRODUCTS_PER_REQUEST * page + "},\"sortBy\":{\"name\":\"sales30\",\"isAscending\":false},\"filterBy\":[],\"facetOptions\":[]}&extensions={\"persistedQuery\":{\"version\":1,\"sha256Hash\":\"4ce7d410a4db2c8b635a48c1dcec375906ff63b19dadd87e073f8fd0c0481d35\"}}";
            var response = await _httpClient.GetAsync<PlayStationStoreGetDiscountedProductsResponse>(url, HEADERS, token);
            return response
            .Data
            .CategoryGridRetrieve
            .Products
            .Select(x => new PlayStationStoreProduct(
                x.Name,
                ToPrice(x.Price.BasePrice),
                ToPrice(x.Price.DiscountedPrice),
                GetImgUrl(x.Media),
                ToProductUrl(x.Id)))
            .ToList();
        }

        private async Task<string> GetAuthTokenAsync(CancellationToken token)
        {
            var result = await _httpClient.GetStringAsync(AUTH_URL, token);
            var regex = new Regex(AUTH_TOKEN_REGEX);
            var match = regex.Match(result);

            if (match is null || !match.Success)
            {
                throw new Exception($"{nameof(PlayStationStoreClient)} could not find auth token");
            }

            return match.Value;
        }

        private static double ToPrice(string value)
        {
            value = value.Replace(".", ",");
            value = Regex.Replace(value, "[^0-9,]", "");
            if (string.IsNullOrWhiteSpace(value)) { return 0; }
            return double.Parse(value);
        }

        private static string GetImgUrl(IEnumerable<PlayStationStoreGetDiscountedProductsResponse.Media> media)
        {
            return media
                .FirstOrDefault(x => x.Role == "MASTER")
                ?.Url;
        }

        private static string ToProductUrl(string id)
        {
            return $"{PRODUCT_PAGE_URL_TEMPLATE}{id}";
        }
    }
}
