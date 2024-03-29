﻿using LibLite.CheapGet.Business.Services.Stores;
using LibLite.CheapGet.Core.Collections;
using LibLite.CheapGet.Core.Enums;
using LibLite.CheapGet.Core.Stores;
using LibLite.CheapGet.Core.Stores.Models;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LibLite.CheapGet.Business.Tests.Services.Stores
{
    [TestFixture]
    public class StoreServiceTests
    {
        private CancellationToken _token;

        private Mock<IStoreClient> _store1;
        private Mock<IStoreClient> _store2;

        private IEnumerable<Product> _products1;
        private IEnumerable<Product> _products2;

        private StoreService _service;

        [SetUp]
        public void SetUp()
        {
            _token = new();

            _products1 = CreateMockProducts(500);
            _products2 = CreateMockProducts(500);

            _store1 = CreateStoreClientMock(_products1);
            _store2 = CreateStoreClientMock(_products2);
            var stores = new[]
            {
                _store1.Object,
                _store2.Object,
            };

            _service = new(stores);
        }

        [Test]
        public async Task GetDiscountedProductsAsync_Take50WithNoCollectionOperations_Returns50FromFirstStore()
        {
            var expected = _products1.Take(50).ToList();
            var request = new GetProductsRequest()
            {
                Count = 50,
            };

            var result = await _service.GetDiscountedProductsAsync(request, _token);

            CollectionAssert.AreEqual(expected, result);
            _store1.Verify(x => x.GetDiscountedProductsAsync(0, StoreService.MIN_FETCH, _token), Times.Once);
            _store2.Verify(x => x.GetDiscountedProductsAsync(0, StoreService.MIN_FETCH, _token), Times.Once);
        }

        [Test]
        public async Task GetDiscountedProductsAsync_Take50WithBasePriceLessThan25_Returns25FromFirstAnd25FromSecondStore()
        {
            var expected = _products1
                .Take(25)
                .ToList()
                .Concat(_products2.Take(25));
            var request = new GetProductsRequest()
            {
                Count = 50,
                Filters = new CollectionFilter<Product>[]
                {
                    new CollectionFilter<Product>(x => x.BasePrice < 25),
                },
            };

            var result = await _service.GetDiscountedProductsAsync(request, _token);

            CollectionAssert.AreEqual(expected, result);
            _store1.Verify(x => x.GetDiscountedProductsAsync(0, StoreService.MIN_FETCH, _token), Times.Once);
            _store2.Verify(x => x.GetDiscountedProductsAsync(0, StoreService.MIN_FETCH, _token), Times.Once);
        }

        [Test]
        public async Task GetDiscountedProductsAsync_Take100WithNameContaining0_Returns50FromFirstAnd50FromSecondStore()
        {
            var productsSequence = new IEnumerable<Product>[]
            {
                _products1.Take(100).Where(x => x.Name.Contains('0')),
                _products2.Take(100).Where(x => x.Name.Contains('0')),
                _products1.Skip(100).Take(100).Where(x => x.Name.Contains('0')),
                _products2.Skip(100).Take(100).Where(x => x.Name.Contains('0')),
                _products1.Skip(200).Take(100).Where(x => x.Name.Contains('0')),
                _products2.Skip(200).Take(100).Where(x => x.Name.Contains('0')),
            };
            var expected = productsSequence.SelectMany(x => x).Take(100);
            var request = new GetProductsRequest()
            {
                Count = 100,
                Filters = new CollectionFilter<Product>[]
                {
                    new CollectionFilter<Product>(x => x.Name.Contains('0')),
                },
            };

            var result = await _service.GetDiscountedProductsAsync(request, _token);

            CollectionAssert.AreEqual(expected, result);
            foreach (var number in GetRange(0, 200, 100))
            {
                _store1.Verify(x => x.GetDiscountedProductsAsync(number, StoreService.MIN_FETCH, _token), Times.Once);
                _store2.Verify(x => x.GetDiscountedProductsAsync(number, StoreService.MIN_FETCH, _token), Times.Once);
            }
        }

        [Test]
        public async Task GetDiscountedProductsAsync_Take100WithBasePriceGreaterThan475_Returns25FromFirstAnd25FromSecondStoreAndRunsOut()
        {
            var productsSequence = new IEnumerable<Product>[]
            {
                _products1.Where(x => x.BasePrice >= 475),
                _products2.Where(x => x.BasePrice >= 475),
            };
            var expected = productsSequence.SelectMany(x => x).ToList();
            var request = new GetProductsRequest()
            {
                Count = 100,
                Filters = new CollectionFilter<Product>[]
                {
                    new CollectionFilter<Product>(x => x.BasePrice >= 475),
                },
            };

            var result = await _service.GetDiscountedProductsAsync(request, _token);

            CollectionAssert.AreEqual(expected, result);
            Assert.AreEqual(50, result.Count());
            foreach (var number in GetRange(0, 900, 100))
            {
                _store1.Verify(x => x.GetDiscountedProductsAsync(number, StoreService.MIN_FETCH, _token), Times.Once);
                _store2.Verify(x => x.GetDiscountedProductsAsync(number, StoreService.MIN_FETCH, _token), Times.Once);
            }
        }

        [Test]
        public async Task GetDiscountedProductsAsync_Take100SortedByNameDesc_ReturnsSortedProducts()
        {
            var productsSequence = new IEnumerable<Product>[]
            {
                _products1.Take(50),
                _products2.Take(50),
            };
            var expected = productsSequence.SelectMany(x => x).OrderByDescending(x => x.Name);
            var request = new GetProductsRequest()
            {
                Count = 100,
                Sorts = new ICollectionSort<Product>[]
                {
                    new CollectionSort<Product, string>(x => x.Name, SortDirection.DESC),
                },
            };

            var result = await _service.GetDiscountedProductsAsync(request, _token);

            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public async Task GetDiscountedProductsAsync_Take50MultipleTimes_ReturnsCachedProducts()
        {
            var expected = _products1.Take(50).ToList();
            var request = new GetProductsRequest()
            {
                Count = 50,
            };

            await _service.GetDiscountedProductsAsync(request, _token);
            await _service.GetDiscountedProductsAsync(request, _token);
            await _service.GetDiscountedProductsAsync(request, _token);

            _store1.Verify(x => x.GetDiscountedProductsAsync(0, StoreService.MIN_FETCH, _token), Times.Once);
            _store2.Verify(x => x.GetDiscountedProductsAsync(0, StoreService.MIN_FETCH, _token), Times.Once);
        }

        private static IEnumerable<int> GetRange(int first, int last, int step)
        {
            if (step == 0)
                throw new ArgumentException("zero step");
            if (Math.Sign(last - first) * Math.Sign(step) < 0)
                throw new ArgumentException("Cannot reach last in this direction");
            int count = Math.Abs((last - first) / step) + 1;
            return Enumerable
                .Range(0, count)
                .Select(n => first + step * n)
                .ToList();
        }

        private static Mock<IStoreClient> CreateStoreClientMock(IEnumerable<Product> products)
        {
            var mock = new Mock<IStoreClient>();
            mock
                .Setup(x => x.GetDiscountedProductsAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((int start, int count, CancellationToken token) => products.Skip(start).Take(count).ToList());
            return mock;
        }

        private static IEnumerable<Product> CreateMockProducts(int count)
        {
            return Enumerable
                .Range(0, count)
                .Select(x => new MockProduct($"name:{count - x}", x, x / 2))
                .ToList();
        }

        class MockProduct : Product
        {
            public override string StoreName => "Mock";

            public MockProduct(string name, double basePrice, double discountedPrice)
                : base(name, basePrice, discountedPrice, "", "") { }
        }
    }
}
