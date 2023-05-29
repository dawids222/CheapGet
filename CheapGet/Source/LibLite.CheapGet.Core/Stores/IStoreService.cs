using LibLite.CheapGet.Core.Stores.Models;

namespace LibLite.CheapGet.Core.Stores
{
    public interface IStoreService
    {
        Task<IEnumerable<Product>> GetDiscountedProductsAsync(GetProductsRequest parameters, CancellationToken token);
        Task<IEnumerable<Product>> GetWishlistProductsAsync(GetWishlistProductsRequest parameters, CancellationToken token);
    }
}
