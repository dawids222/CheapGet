using System.Collections.Generic;

namespace LibLite.CheapGet.Client.Console.Consts
{
    public static class Tags
    {
        public static class Stores
        {
            public const string Steam = "Steam";
            public const string GoG = "GoG";

            public static readonly IEnumerable<string> All = new[]
            {
                Steam, GoG,
            };
        }

        public static class StoreServices
        {
            public const string Games = "Games";

            public static readonly IEnumerable<string> All = new[]
            {
                Games,
            };
        }
    }
}
