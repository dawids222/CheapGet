namespace LibLite.CheapGet.Business.Consts.CGQL
{
    public static class Properties
    {
        public const string STORE_NAME = "store_name";
        public const string NAME = "name";
        public const string BASE_PRICE = "base_price";
        public const string DISCOUNTED_PRICE = "discounted_price";
        public const string DISCOUNT_PERCENTAGE = "discount_percentage";
        public const string DISCOUNT_VALUE = "discount_value";

        public static readonly IEnumerable<string> ALL = new[]
        {
            STORE_NAME,
            NAME,
            BASE_PRICE,
            DISCOUNTED_PRICE,
            DISCOUNT_PERCENTAGE,
            DISCOUNT_VALUE,
        };

        public static readonly IEnumerable<string> TEXT_PROPERTIES = new[]
        {
            STORE_NAME,
            NAME,
        };

        public static readonly IEnumerable<string> NUMERIC_PROPERTIES = new[]
        {
            BASE_PRICE,
            DISCOUNTED_PRICE,
            DISCOUNT_PERCENTAGE,
            DISCOUNT_VALUE,
        };

        public static bool IsProperty(string value) => ALL.Contains(value);
        public static bool IsTextProperty(string value) => TEXT_PROPERTIES.Contains(value);
        public static bool IsNumericProperty(string value) => NUMERIC_PROPERTIES.Contains(value);
    }
}
