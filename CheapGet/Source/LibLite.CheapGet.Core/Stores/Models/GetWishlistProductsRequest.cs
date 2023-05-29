using LibLite.CheapGet.Core.Collections;

namespace LibLite.CheapGet.Core.Stores.Models
{
    public class GetWishlistProductsRequest
    {
        public int Count { get; init; }
        public IEnumerable<ICollectionFilter<Product>> Filters { get; init; }

        public GetWishlistProductsRequest()
        {
            Filters = new List<ICollectionFilter<Product>>();
        }

        public GetWishlistProductsRequest(
            int count,
            IEnumerable<ICollectionFilter<Product>> filters)
        {
            Count = count;
            Filters = filters;
        }
    }
}
