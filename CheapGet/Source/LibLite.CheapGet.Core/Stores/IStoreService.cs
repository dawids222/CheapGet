using LibLite.CheapGet.Core.Stores.Models;

namespace LibLite.CheapGet.Core.Stores
{
    public interface IStoreService
    {
        Task<IEnumerable<Product>> GetDiscountedProductsAsync(GetProductsRequest parameters, CancellationToken token);
    }
}
