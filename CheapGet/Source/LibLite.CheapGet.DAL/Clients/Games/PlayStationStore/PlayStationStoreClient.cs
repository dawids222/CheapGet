using HtmlAgilityPack;
using LibLite.CheapGet.Core.Services;
using LibLite.CheapGet.Core.Stores;
using LibLite.CheapGet.Core.Stores.Games.PlayStationStore;
using LibLite.CheapGet.DAL.Extensions;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace LibLite.CheapGet.DAL.Clients.Games.PlayStationStore
{
    public class PlayStationStoreClient : IPlayStationStoreClient
    {
        // TODO: Better deduce it based on scrapped html...
        private const int PRODUCTS_PER_REQUEST = 24;

        private readonly IHttpClient _httpClient;

        public PlayStationStoreClient(IHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Product>> GetDiscountedProductsAsync(int start, int count, CancellationToken token)
        {
            var tasks = new List<Task<IEnumerable<PlayStationStoreProduct>>>();

            var iterator = start;
            var end = start + count;

            var pageNumber = 1;
            while (iterator < end)
            {
                var remaining = end - iterator;

                var task = GetDiscountedProductsFromPageAsync(pageNumber++, token);
                tasks.Add(task);

                iterator += PRODUCTS_PER_REQUEST;
            }

            var results = await Task.WhenAll(tasks);
            return results.SelectMany(x => x).ToList();
        }

        private async Task<IEnumerable<PlayStationStoreProduct>> GetDiscountedProductsFromPageAsync(int pageNumber, CancellationToken token)
        {
            var url = $"https://store.playstation.com/pl-pl/category/83a687fe-bed7-448c-909f-310e74a71b39/{pageNumber}";
            var html = await _httpClient.GetStringAsync(url, token);

            var document = new HtmlDocument();
            document.LoadHtml(html);

            return ScrapHtmlDocument(document);
        }

        private static IEnumerable<PlayStationStoreProduct> ScrapHtmlDocument(HtmlDocument document)
        {
            var productListItems = document.DocumentNode
                .SelectSingleNode("//ul[@class='psw-grid-list psw-l-grid']")
                .ChildNodes
                .Where(x => x.Name == "li");

            foreach (var productListItem in productListItems)
            {
                var telemetryMeta = productListItem
                    .FirstChild // div
                    .FirstChild // a
                    .GetAttributeValue("data-telemetry-meta", "")
                    .Replace("&quot;", "\"");
                var productId = JsonSerializer.Deserialize<TelemetryMeta>(telemetryMeta, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                }).Id;
                var url = $"https://store.playstation.com/pl-pl/product/{productId}";

                var productTile = productListItem
                    .FirstChild // div
                    .FirstChild // a
                    .FirstChild;// div

                var imgUrl = productTile
                    .FirstChild
                    .FirstChild
                    .GetFirstChildWithClass("psw-image")
                    .GetFirstChildWithName("img")
                    .GetAttributeValue("src", "");

                var detailsSection = productTile.GetFirstChildWithName("section");
                var name = detailsSection.GetFirstChildWithClass("psw-t-body").GetValue<string>();

                var priceDiv = detailsSection
                    ?.GetFirstChildWithClass("psw-price")
                    ?.FirstChild;

                var basePrice = priceDiv?.GetFirstChildWithName("s")?.GetValue<double>() ?? 0;
                var discountedPrice = priceDiv.GetFirstChildWithName("span")?.GetValue<double>() ?? basePrice;

                yield return new PlayStationStoreProduct(name, basePrice, discountedPrice, imgUrl, url);
            }
        }

        private class TelemetryMeta
        {
            public string Id { get; set; }
        }
    }
}
