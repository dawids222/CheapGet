namespace LibLite.CheapGet.Core.Extensions
{
    public static class IEnumerableExtension
    {
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var item in collection) { action.Invoke(item); }
        }
    }
}
