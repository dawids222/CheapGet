namespace LibLite.CheapGet.Core.Stores
{
    public interface IStoreClient
    {
        Task<IEnumerable<Product>> GetDiscountedProductsAsync(int start, int count, CancellationToken token);
        Task<IEnumerable<Product>> GetDiscountedProductsAsync(int count, CancellationToken token) => GetDiscountedProductsAsync(0, count, token);
    }
}
