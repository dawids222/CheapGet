using HtmlAgilityPack;
using LibLite.CheapGet.Core.Extensions;
using LibLite.CheapGet.Core.Services;
using LibLite.CheapGet.Core.Stores;
using LibLite.CheapGet.Core.Stores.Games.Steam;
using LibLite.CheapGet.DAL.Clients.Games.Steam.Responses;
using LibLite.CheapGet.DAL.Extensions;
using System.Text;

namespace LibLite.CheapGet.DAL.Clients.Games
{
    public class SteamClient : ISteamClient
    {
        private const int MIN_PRODUCTS_PER_REQUEST = 25;
        private const int MAX_PRODUCTS_PER_REQUEST = 100;

        private readonly IHttpClient _httpClient;

        public SteamClient(IHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Product>> GetDiscountedProductsAsync(int start, int count, CancellationToken token)
        {
            var html = await GetHtmlAsync(start, count, token);

            var document = new HtmlDocument();
            document.LoadHtml(html);

            var products = ScrapHtmlDocument(document);

            return products.Take(count).ToList();
        }

        private async Task<string> GetHtmlAsync(int start, int count, CancellationToken token)
        {
            var stringBuilder = new StringBuilder();
            var tasks = new List<Task<string>>();

            var iterator = start;
            var end = start + count;

            while (iterator < end)
            {
                var remaining = end - iterator;
                var amount = Math.Min(remaining, MAX_PRODUCTS_PER_REQUEST);

                var url = GetHtmlUrl(iterator, amount);
                var task = CreateGetHtmlTask(url, token);
                tasks.Add(task);

                iterator += amount;
            }

            var results = await Task.WhenAll(tasks);
            results.ForEach(result => stringBuilder.Append(result));

            return stringBuilder.ToString();
        }

        private static string GetHtmlUrl(int start, int count)
        {
            return $"https://store.steampowered.com/search/results/?query&start={start}&count={count}&dynamic_data=&sort_by=_ASC&specials=1&infinite=1";
        }

        private Task<string> CreateGetHtmlTask(string url, CancellationToken token)
        {
            return Task.Run(async () =>
            {
                var response = await _httpClient.GetAsync<SteamGetDiscountedProductsResponse>(url, token);
                return response.results_html;
            });
        }

        private static IEnumerable<SteamProduct> ScrapHtmlDocument(HtmlDocument document)
        {
            var productRows = document.DocumentNode.Descendants("a");
            foreach (var productRow in productRows)
            {
                var nameCombinedNode = productRow.GetFirstChildWithClass("responsive_search_name_combined");
                var discountCombinedNode = nameCombinedNode.GetFirstChildWithClass("search_price_discount_combined");
                var priceNode = discountCombinedNode.GetFirstChildWithClass("search_price");

                var name = nameCombinedNode
                    ?.GetFirstChildWithClass("search_name")
                    ?.GetFirstChildWithClass("title")
                    ?.GetValue<string>();

                var basePrice = priceNode
                    ?.GetFirstChildWithName("span")
                    ?.GetFirstChildWithName("strike")
                    ?.GetValue<double>() ?? 0;

                var discountedPrice = priceNode
                    ?.GetLastChildWithName("#text")
                    ?.GetValue<double>() ?? 0;

                yield return new SteamProduct
                {
                    Name = name,
                    BasePrice = basePrice,
                    DiscountedPrice = discountedPrice,
                };
            }
        }
    }
}
