using LibLite.CheapGet.Core.Enums;

namespace LibLite.CheapGet.Core.Collections
{
    public interface ICollectionSort<T> : ICollectionOperation<T>
    {
        public SortDirection SortDirection { get; }
    }
}
