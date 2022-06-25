namespace LibLite.CheapGet.Core.Collections
{
    public class CollectionFilter<T> : ICollectionFilter<T>
    {
        public virtual Func<T, bool> Predicate { get; protected init; }

        protected CollectionFilter() { }
        public CollectionFilter(Func<T, bool> predicate)
        {
            Predicate = predicate;
        }

        public IEnumerable<T> Apply(IEnumerable<T> collection)
        {
            return collection.Where(Predicate);
        }

        public ICollectionFilter<T> And(ICollectionFilter<T> filter)
        {
            var predicate = (T collection) => Predicate(collection) && filter.Predicate(collection);
            return new CollectionFilter<T>(predicate);
        }

        public ICollectionFilter<T> Or(ICollectionFilter<T> filter)
        {
            var predicate = (T collection) => Predicate(collection) || filter.Predicate(collection);
            return new CollectionFilter<T>(predicate);
        }
    }
}
