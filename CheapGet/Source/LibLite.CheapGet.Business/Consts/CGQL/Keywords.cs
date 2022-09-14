namespace LibLite.CheapGet.Business.Consts.CGQL
{
    public static class Keywords
    {
        public const string SELECT = "select";
        public const string FROM = "from";
        public const string TAKE = "take";
        public const string FILTER = "filter";
        public const string SORT = "sort";
        public const string ASC = "asc";
        public const string DESC = "desc";
        public const string LOAD = "load";
        public const string CLS = "cls";
        public const string EXIT = "exit";

        public static readonly IEnumerable<string> SORT_DIRECTIONS = new[]
        {
            ASC, DESC,
        };
    }
}
