using LibLite.CheapGet.Core.Collections;

namespace LibLite.CheapGet.Core.Stores.Models
{
    public class GetProductsRequest
    {
        public int Count { get; init; }
        public IEnumerable<ICollectionFilter<Product>> Filters { get; init; }
        public IEnumerable<ICollectionSort<Product>> Sorts { get; init; }

        public GetProductsRequest()
        {
            Filters = new List<ICollectionFilter<Product>>();
            Sorts = new List<ICollectionSort<Product>>();
        }

        public GetProductsRequest(
            int count,
            IEnumerable<ICollectionFilter<Product>> filters,
            IEnumerable<ICollectionSort<Product>> sorts)
        {
            Count = count;
            Filters = filters;
            Sorts = sorts;
        }
    }
}
