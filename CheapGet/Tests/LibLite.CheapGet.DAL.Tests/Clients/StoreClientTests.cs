using LibLite.CheapGet.Core.Services;
using LibLite.CheapGet.Core.Stores;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace LibLite.CheapGet.DAL.Tests.Clients
{
    public abstract class StoreClientTests<TClient>
        where TClient : IStoreClient
    {
        protected static readonly Random _random = new();

        protected Exception _exception;
        protected CancellationToken _token;
        protected Mock<IHttpClient> _httpClientMock;
        protected TClient _client;

        [SetUp]
        public void SetUp()
        {
            _exception = new Exception("Error!");
            _token = new();
            _httpClientMock = new();
            _client = CreateClient();
        }

        protected abstract TClient CreateClient();

        protected static void AssertAreEqual(Product expected, Product actual)
        {
            Assert.AreEqual(expected.StoreName, actual.StoreName);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.BasePrice, actual.BasePrice);
            Assert.AreEqual(expected.DiscountedPrice, actual.DiscountedPrice);
            Assert.AreEqual(expected.DiscountPercentage, actual.DiscountPercentage);
            Assert.AreEqual(expected.DiscountValue, actual.DiscountValue);
            Assert.AreEqual(expected.ImgUrl, actual.ImgUrl);
            Assert.AreEqual(expected.Url, actual.Url);
        }

        protected static void AssertAreEqual(IEnumerable<Product> expected, IEnumerable<Product> actual)
        {
            Assert.AreEqual(expected.Count(), actual.Count(), $"Inconsistent collection size");
            for (var i = 0; i < expected.Count(); i++)
            {
                AssertAreEqual(expected.ElementAt(i), actual.ElementAt(i));
            }
        }

        protected static string GenerateRandomTitle()
        {
            var segments = _random.Next(1, 5);
            var words = Enumerable
                .Range(0, segments)
                .Select(x => GenerateRandomString(_random.Next(2, 10)))
                .ToList();
            return string.Join(" ", words);
        }

        protected static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(x => x[_random.Next(x.Length)]).ToArray());
        }
    }
}
