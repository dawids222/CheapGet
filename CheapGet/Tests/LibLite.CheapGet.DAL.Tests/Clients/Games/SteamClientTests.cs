using LibLite.CheapGet.Core.Stores.Games.Steam;
using LibLite.CheapGet.DAL.Clients.Games;
using LibLite.CheapGet.DAL.Clients.Games.Steam.Responses;
using Moq;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LibLite.CheapGet.DAL.Tests.Clients.Games
{
    [TestFixture]
    public class SteamClientTests : StoreClientTests<SteamClient>
    {
        protected override SteamClient CreateClient()
        {
            return new(_httpClientMock.Object);
        }

        [Test]
        public async Task GetDiscountedProductsAsync_Start0Count1_Returns1Sale()
        {
            var resultsHtml = File.ReadAllText("Resources\\SteamGetDiscountedProductsStart0Count100Html.txt");
            var url = $"https://store.steampowered.com/search/results/?query&start=0&count=1&dynamic_data=&sort_by=_ASC&specials=1&infinite=1";
            var expected = new SteamProduct(
                name: "Red Dead Redemption 2",
                basePrice: 249.9,
                discountedPrice: 124.95,
                imgUrl: "https://cdn.akamai.steamstatic.com/steam/apps/1174180/capsule_sm_120.jpg?t=1618851907",
                url: "https://store.steampowered.com/app/1174180/Red_Dead_Redemption_2/?snr=1_7_7_2300_150_1");
            var response = new SteamGetDiscountedProductsResponse
            {
                success = 1,
                results_html = resultsHtml,
                start = 0,
                total_count = 1,
            };
            _httpClientMock
                .Setup(x => x.GetAsync<SteamGetDiscountedProductsResponse>(url, _token))
                .ReturnsAsync(response);

            var result = await _client.GetDiscountedProductsAsync(0, 1, _token);

            Assert.AreEqual(1, result.Count());
            var product = result.ElementAt(0);
            AssertAreEqual(expected, product);
        }

        [Test]
        public async Task GetDiscountedProductsAsync_Start0Count100_Returns100Sales()
        {
            var resultsHtml = File.ReadAllText("Resources\\SteamGetDiscountedProductsStart0Count100Html.txt");
            var url = $"https://store.steampowered.com/search/results/?query&start=0&count=100&dynamic_data=&sort_by=_ASC&specials=1&infinite=1";
            var expected0 = new SteamProduct(
                name: "Red Dead Redemption 2",
                basePrice: 249.9,
                discountedPrice: 124.95,
                imgUrl: "https://cdn.akamai.steamstatic.com/steam/apps/1174180/capsule_sm_120.jpg?t=1618851907",
                url: "https://store.steampowered.com/app/1174180/Red_Dead_Redemption_2/?snr=1_7_7_2300_150_1");
            var expected99 = new SteamProduct(
                name: "Forts - High Seas Bundle",
                basePrice: 89.98,
                discountedPrice: 56.68,
                imgUrl: "https://cdn.akamai.steamstatic.com/steam/bundles/25294/46rchgr1oc1lro1n/capsule_sm_120.jpg?t=1647870827",
                url: "https://store.steampowered.com/bundle/25294/Forts__High_Seas_Bundle/?snr=1_7_7_2300_150_1");
            var response = new SteamGetDiscountedProductsResponse
            {
                success = 1,
                results_html = resultsHtml,
                start = 0,
                total_count = 1,
            };
            _httpClientMock
                .Setup(x => x.GetAsync<SteamGetDiscountedProductsResponse>(url, _token))
                .ReturnsAsync(response);

            var result = await _client.GetDiscountedProductsAsync(0, 100, _token);

            Assert.AreEqual(100, result.Count());
            var product = result.ElementAt(0);
            AssertAreEqual(expected0, product);

            product = result.ElementAt(99);
            AssertAreEqual(expected99, product);
        }

        [Test]
        public async Task GetDiscountedProductsAsync_Start25Count50_CallsCorrectUrl()
        {
            var resultsHtml = File.ReadAllText("Resources\\SteamGetDiscountedProductsStart0Count100Html.txt");
            var response = new SteamGetDiscountedProductsResponse
            {
                success = 1,
                results_html = resultsHtml,
                start = 0,
                total_count = 1,
            };
            _httpClientMock
                .Setup(x => x.GetAsync<SteamGetDiscountedProductsResponse>(It.IsAny<string>(), _token))
                .ReturnsAsync(response);

            var result = await _client.GetDiscountedProductsAsync(25, 50, _token);

            _httpClientMock.Verify(x => x.GetAsync<SteamGetDiscountedProductsResponse>("https://store.steampowered.com/search/results/?query&start=25&count=50&dynamic_data=&sort_by=_ASC&specials=1&infinite=1", _token), Times.Once);
        }

        [Test]
        public async Task GetDiscountedProductsAsync_Start25Count250_CallsCorrectUrls()
        {
            var resultsHtml = File.ReadAllText("Resources\\SteamGetDiscountedProductsStart0Count100Html.txt");
            var response = new SteamGetDiscountedProductsResponse
            {
                success = 1,
                results_html = resultsHtml,
                start = 0,
                total_count = 1,
            };
            _httpClientMock
                .Setup(x => x.GetAsync<SteamGetDiscountedProductsResponse>(It.IsAny<string>(), _token))
                .ReturnsAsync(response);

            var result = await _client.GetDiscountedProductsAsync(25, 250, _token);

            _httpClientMock.Verify(x => x.GetAsync<SteamGetDiscountedProductsResponse>("https://store.steampowered.com/search/results/?query&start=25&count=100&dynamic_data=&sort_by=_ASC&specials=1&infinite=1", _token), Times.Once);
            _httpClientMock.Verify(x => x.GetAsync<SteamGetDiscountedProductsResponse>("https://store.steampowered.com/search/results/?query&start=125&count=100&dynamic_data=&sort_by=_ASC&specials=1&infinite=1", _token), Times.Once);
            _httpClientMock.Verify(x => x.GetAsync<SteamGetDiscountedProductsResponse>("https://store.steampowered.com/search/results/?query&start=225&count=50&dynamic_data=&sort_by=_ASC&specials=1&infinite=1", _token), Times.Once);
        }

        [Test]
        public void GetDiscountedProductsAsync_HttpClientThrows_ThrowsTheSameException()
        {
            _httpClientMock
                .Setup(x => x.GetAsync<SteamGetDiscountedProductsResponse>(It.IsAny<string>(), _token))
                .ThrowsAsync(_exception);

            Task act() => _client.GetDiscountedProductsAsync(0, 1, _token);

            Assert.ThrowsAsync<Exception>(act, _exception.Message);
        }
    }
}
