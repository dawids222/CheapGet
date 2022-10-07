namespace LibLite.CheapGet.Business.Consts.CGQL
{
    public static class Categories
    {
        public const string GAMES = "Games";

        public static readonly IEnumerable<string> ALL = new[]
        {
            GAMES,
        };

        public static bool IsCategory(string value) => ALL.Contains(value);
    }
}
