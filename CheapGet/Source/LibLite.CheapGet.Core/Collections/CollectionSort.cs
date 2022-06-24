using LibLite.CheapGet.Core.Enums;

namespace LibLite.CheapGet.Core.Collections
{
    public class CollectionSort<T, R> : ICollectionSort<T>
    {
        public Func<T, R> Predicate { get; }
        public SortDirection SortDirection { get; }

        public CollectionSort(
            Func<T, R> predicate,
            SortDirection sortDirection = SortDirection.ASC)
        {
            Predicate = predicate;
            SortDirection = sortDirection;
        }

        public IEnumerable<T> Apply(IEnumerable<T> collection)
        {
            return SortDirection switch
            {
                SortDirection.ASC => collection.OrderBy(Predicate),
                SortDirection.DESC => collection.OrderByDescending(Predicate),
                _ => throw new NotImplementedException(),
            };
        }
    }
}
