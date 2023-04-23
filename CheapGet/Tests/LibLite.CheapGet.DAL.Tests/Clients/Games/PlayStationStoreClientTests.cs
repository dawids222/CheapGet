using LibLite.CheapGet.Core.Extensions;
using LibLite.CheapGet.Core.Stores.Games.PlayStationStore;
using LibLite.CheapGet.DAL.Clients.Games.PlayStationStore;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Media = LibLite.CheapGet.DAL.Clients.Games.PlayStationStore.Responses.PlayStationStoreGetDiscountedProductsResponse.Media;
using Price = LibLite.CheapGet.DAL.Clients.Games.PlayStationStore.Responses.PlayStationStoreGetDiscountedProductsResponse.Price;
using Response = LibLite.CheapGet.DAL.Clients.Games.PlayStationStore.Responses.PlayStationStoreGetDiscountedProductsResponse;
using ResponseProduct = LibLite.CheapGet.DAL.Clients.Games.PlayStationStore.Responses.PlayStationStoreGetDiscountedProductsResponse.Product;

namespace LibLite.CheapGet.DAL.Tests.Clients.Games
{
    [TestFixture]
    public class PlayStationStoreClientTests : StoreClientTests<PlayStationStoreClient>
    {
        private string _authToken;
        private string _authTokenResponse;
        private Dictionary<string, string> _headers;

        protected override PlayStationStoreClient CreateClient()
        {
            return new(_httpClientMock.Object);
        }

        public override void SetUp()
        {
            base.SetUp();

            _authToken = "803cee19-e5a1-4d59-a463-0b6b2701bf7c";
            _authTokenResponse = "{{\"locale\":\"pl-pl\",\"categoryId\":\"" + _authToken + "\",\"page\":1,\"batarangs\":{}}}";
            _headers = new() { { "x-psn-store-locale-override", "pl-PL" } };

            _httpClientMock
                .Setup(x => x.GetStringAsync(It.IsAny<string>(), _token))
                .ReturnsAsync(_authTokenResponse);
        }

        [Test]
        public async Task GetDiscountedProductsAsync_Start0Count1_Returns1Sale()
        {
            var response = GenerateRandomResponse(1);
            var expected = GetExpectedProducts(response);
            var url = "https://web.np.playstation.com/api/graphql/v1//op?operationName=categoryGridRetrieve&variables={\"id\":\"" + _authToken + "\",\"pageArgs\":{\"size\":100,\"offset\":0},\"sortBy\":{\"name\":\"sales30\",\"isAscending\":false},\"filterBy\":[],\"facetOptions\":[]}&extensions={\"persistedQuery\":{\"version\":1,\"sha256Hash\":\"4ce7d410a4db2c8b635a48c1dcec375906ff63b19dadd87e073f8fd0c0481d35\"}}";
            _httpClientMock
                .Setup(x => x.GetAsync<Response>(url, _headers, _token))
                .ReturnsAsync(response);

            var result = await _client.GetDiscountedProductsAsync(0, 1, _token);

            AssertAreEqual(expected, result);
        }

        [Test]
        public async Task GetDiscountedProductsAsync_Start0Count100_Returns100Sales()
        {
            var response = GenerateRandomResponse(100);
            var expected = GetExpectedProducts(response);
            var url = "https://web.np.playstation.com/api/graphql/v1//op?operationName=categoryGridRetrieve&variables={\"id\":\"" + _authToken + "\",\"pageArgs\":{\"size\":100,\"offset\":0},\"sortBy\":{\"name\":\"sales30\",\"isAscending\":false},\"filterBy\":[],\"facetOptions\":[]}&extensions={\"persistedQuery\":{\"version\":1,\"sha256Hash\":\"4ce7d410a4db2c8b635a48c1dcec375906ff63b19dadd87e073f8fd0c0481d35\"}}";
            _httpClientMock
                .Setup(x => x.GetAsync<Response>(url, _headers, _token))
                .ReturnsAsync(response);

            var result = await _client.GetDiscountedProductsAsync(0, 100, _token);

            AssertAreEqual(expected, result);
        }

        [Test]
        public async Task GetDiscountedProductsAsync_Start25Count50_Returns50Sales()
        {
            var response = GenerateRandomResponse(100);
            var expected = GetExpectedProducts(response, 25, 50);
            var url = "https://web.np.playstation.com/api/graphql/v1//op?operationName=categoryGridRetrieve&variables={\"id\":\"" + _authToken + "\",\"pageArgs\":{\"size\":100,\"offset\":0},\"sortBy\":{\"name\":\"sales30\",\"isAscending\":false},\"filterBy\":[],\"facetOptions\":[]}&extensions={\"persistedQuery\":{\"version\":1,\"sha256Hash\":\"4ce7d410a4db2c8b635a48c1dcec375906ff63b19dadd87e073f8fd0c0481d35\"}}";
            _httpClientMock
                .Setup(x => x.GetAsync<Response>(url, _headers, _token))
                .ReturnsAsync(response);

            var result = await _client.GetDiscountedProductsAsync(25, 50, _token);

            AssertAreEqual(expected, result);
        }

        [Test]
        public async Task GetDiscountedProductsAsync_Start25Count250_Returns250Sales()
        {
            var expected = new List<PlayStationStoreProduct>();
            Enumerable
                .Range(0, 3)
                .ForEach(x =>
                {
                    var response = GenerateRandomResponse(100);
                    expected.AddRange(GetExpectedProducts(response));
                    var url = "https://web.np.playstation.com/api/graphql/v1//op?operationName=categoryGridRetrieve&variables={\"id\":\"" + _authToken + "\",\"pageArgs\":{\"size\":100,\"offset\":" + 100 * x + "},\"sortBy\":{\"name\":\"sales30\",\"isAscending\":false},\"filterBy\":[],\"facetOptions\":[]}&extensions={\"persistedQuery\":{\"version\":1,\"sha256Hash\":\"4ce7d410a4db2c8b635a48c1dcec375906ff63b19dadd87e073f8fd0c0481d35\"}}";
                    _httpClientMock
                        .Setup(x => x.GetAsync<Response>(url, _headers, _token))
                        .ReturnsAsync(response);
                });
            expected = expected.Skip(25).Take(250).ToList();

            var result = await _client.GetDiscountedProductsAsync(25, 250, _token);

            AssertAreEqual(expected, result);
        }

        [Test]
        public void GetDiscountedProductsAsync_GetProducts_HttpClientThrows_ThrowsTheSameException()
        {
            _httpClientMock
                .Setup(x => x.GetAsync<Response>(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), _token))
                .ThrowsAsync(_exception);

            Task act() => _client.GetDiscountedProductsAsync(0, 1, _token);

            Assert.ThrowsAsync<Exception>(act, _exception.Message);
        }

        [Test]
        public void GetDiscountedProductsAsync_GetAuthToken_HttpClientThrows_ThrowsTheSameException()
        {
            _httpClientMock
                .Setup(x => x.GetStringAsync(It.IsAny<string>(), _token))
                .ThrowsAsync(_exception);

            Task act() => _client.GetDiscountedProductsAsync(0, 1, _token);

            Assert.ThrowsAsync<Exception>(act, _exception.Message);
        }

        [TestCase("")]
        [TestCase("this is not auth token")]
        public void GetDiscountedProductsAsync_GetAuthToken_TokenNotFound_ThrowsException(string response)
        {
            _httpClientMock
                .Setup(x => x.GetStringAsync(It.IsAny<string>(), _token))
                .ReturnsAsync(response);

            Task act() => _client.GetDiscountedProductsAsync(0, 1, _token);

            var exception = Assert.ThrowsAsync<Exception>(act);
            Assert.That(exception.Message, Contains.Substring(nameof(PlayStationStoreClient)));
            Assert.That(exception.Message, Contains.Substring("auth token"));
        }

        private static Response GenerateRandomResponse(int productsCount)
        {
            return new Response
            {
                Data = new()
                {
                    CategoryGridRetrieve = new()
                    {
                        Products = Enumerable
                        .Range(0, productsCount)
                        .Select(x => GenerateRandomResponseProduct())
                        .ToList(),
                    },
                },
            };
        }

        private static ResponseProduct GenerateRandomResponseProduct()
        {
            var baseAmount = _random.Next(10, 400);
            var title = GenerateRandomTitle();
            return new ResponseProduct
            {
                Name = title,
                Id = title.Replace(' ', '-'),
                Media = new Media[]
                {
                    new Media()
                    {
                        Role = "NOT_MASTER",
                    },
                    new Media()
                    {
                        Role = "MASTER",
                        Type = "image",
                        Url = "img_url",
                    },
                    new Media()
                    {
                        Role = "STILL_NOT",
                    },
                },
                Price = new Price
                {
                    BasePrice = baseAmount.ToString(),
                    DiscountedPrice = (baseAmount - _random.Next(1, baseAmount - 1)).ToString(),
                },
            };
        }

        private static IEnumerable<PlayStationStoreProduct> GetExpectedProducts(Response response, int? skip = null, int? take = null)
        {
            var query = response
                .Data
                .CategoryGridRetrieve
                .Products;

            if (skip.HasValue) { query = query.Skip(skip.Value); }
            if (take.HasValue) { query = query.Take(take.Value); }

            return query
                .Select(x => ToPlayStationStoreProduct(x))
                .ToList();
        }

        private static PlayStationStoreProduct ToPlayStationStoreProduct(ResponseProduct product)
        {
            return new PlayStationStoreProduct(
                product.Name,
                double.Parse(product.Price.BasePrice),
                double.Parse(product.Price.DiscountedPrice),
                product.Media.FirstOrDefault(x => x.Role == "MASTER")?.Url,
                $"https://store.playstation.com/pl-pl/product/{product.Id}");
        }
    }
}
