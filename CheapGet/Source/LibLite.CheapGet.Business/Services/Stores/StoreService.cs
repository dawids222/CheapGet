using LibLite.CheapGet.Core.Collections;
using LibLite.CheapGet.Core.Stores;
using LibLite.CheapGet.Core.Stores.Models;

namespace LibLite.CheapGet.Business.Services.Stores
{
    public class StoreService : IStoreService
    {
        public const int MIN_FETCH = 100;
        public const int MAX_FETCH = 1000;

        private int _fetched = 0;

        private readonly IEnumerable<IStoreClient> _stores;
        private readonly List<Product> _products = new();

        public StoreService(IEnumerable<IStoreClient> stores) => _stores = stores;

        public async Task<IEnumerable<Product>> GetDiscountedProductsAsync(GetProductsRequest request, CancellationToken token)
        {
            var products = ApplyCollectionOperations(_products, request.Filters);
            while (products.Count() < request.Count && _fetched < MAX_FETCH)
            {
                var start = _fetched;
                var count = CalculateNumberOfProductsToGet(products.Count(), request.Count);
                var newProducts = await GetProductsAsync(start, count, token);
                _fetched += count;
                _products.AddRange(newProducts);
                products = ApplyCollectionOperations(_products, request.Filters);
            }
            products = ApplyCollectionOperations(products, request.Sorts);
            return products
                .Take(request.Count)
                .ToList();
        }

        private static int CalculateNumberOfProductsToGet(int currentCount, int desiredCount)
        {
            var missingProducts = (double)(desiredCount - currentCount);
            var productsToGet = (int)Math.Ceiling(missingProducts / MIN_FETCH) * MIN_FETCH;
            productsToGet = Math.Max(productsToGet, MIN_FETCH);
            productsToGet = Math.Min(productsToGet, MAX_FETCH);
            return productsToGet;
        }

        private async Task<IEnumerable<Product>> GetProductsAsync(int start, int count, CancellationToken token)
        {
            var tasks = _stores
                .Select(store => Task.Run(() => store.GetDiscountedProductsAsync(start, count, token)))
                .ToList();
            var results = await Task.WhenAll(tasks);
            return results
                .SelectMany(products => products)
                .ToList();
        }

        private static IEnumerable<Product> ApplyCollectionOperations(IEnumerable<Product> products, IEnumerable<ICollectionOperation<Product>> operations)
        {
            return operations
                .Reverse()
                .Aggregate(
                    products,
                    (current, filter) => filter.Apply(current))
                .ToList();
        }

        public async Task<IEnumerable<Product>> GetWishlistProductsAsync(GetWishlistProductsRequest parameters, CancellationToken token)
        {
            if (!parameters.Filters.Any())
            {
                return Array.Empty<Product>();
            }

            var tasks = _stores
                .Select(store => store.GetDiscountedProductsAsync(parameters.Count, token))
                .ToList();

            var results = await Task.WhenAll(tasks);
            var products = results
                .SelectMany(x => x)
                .ToList();

            var filter = parameters.Filters.Aggregate((current, next) => current.Or(next));
            return filter
                .Apply(products)
                .ToList();
        }
    }
}
