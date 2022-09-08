namespace LibLite.CheapGet.Business.Consts.CGQL
{
    public static class Comparisons
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

        public static readonly IEnumerable<string> TEXT_COMPARISONS = new[]
        {
            EQUAL,
            CONTAIN,
        };

        public static readonly IEnumerable<string> NUMERIC_COMPARISONS = new[]
        {
            GREATER_OR_EQUAL,
            GREATER,
            EQUAL,
            NOT_EQUAL,
            LESS,
            LESS_OR_EQUAL,
        };

        public static bool IsComparison(string value) => ALL.Contains(value);
        public static bool IsTextComparison(string value) => TEXT_COMPARISONS.Contains(value);
        public static bool IsNumericComparison(string value) => NUMERIC_COMPARISONS.Contains(value);
    }
}
