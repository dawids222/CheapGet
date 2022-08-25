namespace LibLite.CheapGet.Business.Consts.CGQL
{
    internal static class Operators
    {
        public const string GREATER_OR_EQUAL = ">=";
        public const string GREATER = ">";
        public const string EQUAL = "=";
        public const string NOT_EQUAL = "!=";
        public const string LESS = "<";
        public const string LESS_OR_EQUAL = "<=";
        public const string CONTAIN = "<>";

        public static readonly IEnumerable<string> ALL = new[]
        {
            GREATER_OR_EQUAL,
            GREATER,
            EQUAL,
            NOT_EQUAL,
            LESS,
            LESS_OR_EQUAL,
            CONTAIN,
        };

        public static readonly IEnumerable<string> STRING_OPERATORS = new[]
        {
            EQUAL,
            CONTAIN,
        };

        public static readonly IEnumerable<string> DECIMAL_PROPERTIES = new[]
        {
            GREATER_OR_EQUAL,
            GREATER,
            EQUAL,
            NOT_EQUAL,
            LESS,
            LESS_OR_EQUAL,
        };
    }
}
