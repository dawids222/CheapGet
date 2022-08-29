namespace LibLite.CheapGet.Business.Consts.CGQL
{
    internal static class SortDirections
    {
        public const string ASC = "asc";
        public const string DESC = "desc";

        public static readonly IEnumerable<string> ALL = new[]
        {
            ASC, DESC,
        };
    }
}
