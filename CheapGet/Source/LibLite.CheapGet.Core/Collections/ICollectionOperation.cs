namespace LibLite.CheapGet.Core.Collections
{
    public interface ICollectionOperation<T>
    {
        IEnumerable<T> Apply(IEnumerable<T> collection);
    }
}
