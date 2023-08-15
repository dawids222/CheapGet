using LibLite.CheapGet.Core.Extensions;
using LibLite.CheapGet.Core.Stores.Games.GoG;
using LibLite.CheapGet.DAL.Clients.Games.GoG;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Money = LibLite.CheapGet.DAL.Clients.Games.GoG.Responses.GogGetDiscountedProductsResponse.Money;
using Price = LibLite.CheapGet.DAL.Clients.Games.GoG.Responses.GogGetDiscountedProductsResponse.Price;
using Response = LibLite.CheapGet.DAL.Clients.Games.GoG.Responses.GogGetDiscountedProductsResponse;
using ResponseProduct = LibLite.CheapGet.DAL.Clients.Games.GoG.Responses.GogGetDiscountedProductsResponse.Product;

namespace LibLite.CheapGet.DAL.Tests.Clients.Games
{
    [TestFixture]
    public class GogClientTests : StoreClientTests<GogClient>
    {
        protected override GogClient CreateClient()
        {
            return new(_httpClientMock);
        }

        [Test]
        public async Task GetDiscountedProductsAsync_Start0Count1_Returns1Sale()
        {
            var response = new Response
            {
                Products = GenerateRandomResponseProducts(1),
            };
            var expected = response
                .Products
                .Select(x => ToGogProduct(x))
                .ToList();
            var url = $"https://catalog.gog.com/v1/catalog?limit=100&order=desc%3Atrending&discounted=eq%3Atrue&productType=in%3Agame%2Cpack&page=1&countryCode=PL&locale=pl-PL&currencyCode=PLN";
            _httpClientMock
                .GetAsync<Response>(url, _token)
                .Returns(response);

            var result = await _client.GetDiscountedProductsAsync(0, 1, _token);

            AssertAreEqual(expected, result);
        }

        [Test]
        public async Task GetDiscountedProductsAsync_Start0Count100_Returns100Sales()
        {
            var response = new Response
            {
                Products = GenerateRandomResponseProducts(100),
            };
            var expected = response
                .Products
                .Select(x => ToGogProduct(x))
                .ToList();
            var url = $"https://catalog.gog.com/v1/catalog?limit=100&order=desc%3Atrending&discounted=eq%3Atrue&productType=in%3Agame%2Cpack&page=1&countryCode=PL&locale=pl-PL&currencyCode=PLN";
            _httpClientMock
                .GetAsync<Response>(url, _token)
                .Returns(response);

            var result = await _client.GetDiscountedProductsAsync(0, 100, _token);

            AssertAreEqual(expected, result);
        }

        [Test]
        public async Task GetDiscountedProductsAsync_Start25Count50_Returns50Sales()
        {
            var response = new Response
            {
                Products = GenerateRandomResponseProducts(100),
            };
            var expected = response
                .Products
                .Skip(25)
                .Take(50)
                .Select(x => ToGogProduct(x))
                .ToList();
            var url = $"https://catalog.gog.com/v1/catalog?limit=100&order=desc%3Atrending&discounted=eq%3Atrue&productType=in%3Agame%2Cpack&page=1&countryCode=PL&locale=pl-PL&currencyCode=PLN";
            _httpClientMock
                .GetAsync<Response>(url, _token)
                .Returns(response);

            var result = await _client.GetDiscountedProductsAsync(25, 50, _token);

            AssertAreEqual(expected, result);
        }

        [Test]
        public async Task GetDiscountedProductsAsync_Start25Count250_Returns250Sales()
        {
            var expected = new List<GogProduct>();
            Enumerable
                .Range(1, 3)
                .ForEach(x =>
                {
                    var response = new Response
                    {
                        Products = GenerateRandomResponseProducts(100),
                    };
                    expected.AddRange(response
                        .Products
                        .Select(x => ToGogProduct(x))
                        .ToList());
                    var url = $"https://catalog.gog.com/v1/catalog?limit=100&order=desc%3Atrending&discounted=eq%3Atrue&productType=in%3Agame%2Cpack&page={x}&countryCode=PL&locale=pl-PL&currencyCode=PLN";
                    _httpClientMock
                        .GetAsync<Response>(url, _token)
                        .Returns(response);
                });
            expected = expected.Skip(25).Take(250).ToList();

            var result = await _client.GetDiscountedProductsAsync(25, 250, _token);

            AssertAreEqual(expected, result);
        }

        [Test]
        public void GetDiscountedProductsAsync_HttpClientThrows_ThrowsTheSameException()
        {
            _httpClientMock
                .GetAsync<Response>(Arg.Any<string>(), _token)
                .Throws(_exception);

            Task act() => _client.GetDiscountedProductsAsync(0, 1, _token);

            Assert.ThrowsAsync<Exception>(act, _exception.Message);
        }

        private static IEnumerable<ResponseProduct> GenerateRandomResponseProducts(int count)
        {
            return Enumerable
                .Range(0, count)
                .Select(x => GenerateRandomResponseProduct())
                .ToList();
        }

        private static ResponseProduct GenerateRandomResponseProduct()
        {
            var baseAmount = _random.Next(10, 400);
            var title = GenerateRandomTitle();
            return new ResponseProduct
            {
                Title = title,
                Slug = title.Replace(' ', '-'),
                CoverHorizontal = $"https://images.gog-statics.com/{Guid.NewGuid()}.png",
                Price = new Price
                {
                    BaseMoney = new Money { Amount = baseAmount },
                    FinalMoney = new Money { Amount = baseAmount - _random.Next(1, baseAmount - 1) },
                },
            };
        }

        private static GogProduct ToGogProduct(ResponseProduct product)
        {
            return new GogProduct(
                product.Title,
                product.Price.BaseMoney.Amount,
                product.Price.FinalMoney.Amount,
                product.CoverHorizontal,
                ToProductUrl(product.Slug));
        }

        private static string ToProductUrl(string slug)
        {
            var name = slug.Replace('-', '_');
            return $"{GogClient.PRODUCT_PAGE_URL_TEMPLATE}{name}";
        }
    }
}
