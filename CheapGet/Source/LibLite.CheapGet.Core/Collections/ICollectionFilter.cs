namespace LibLite.CheapGet.Core.Collections
{
    public interface ICollectionFilter<T> : ICollectionOperation<T>
    {
        Func<T, bool> Predicate { get; }

        ICollectionFilter<T> And(ICollectionFilter<T> filter);
        ICollectionFilter<T> Or(ICollectionFilter<T> filter);
    }
}
