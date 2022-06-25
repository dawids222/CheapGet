using LibLite.CheapGet.Core.Collections;
using StringRelationalOperator = LibLite.CheapGet.Core.Enums.StringRelationalOperator;

namespace LibLite.CheapGet.Business.Collections
{
    public class CollectionStringFilter<T> : CollectionFilter<T>
    {
        public CollectionStringFilter(
            Func<T, string> func,
            StringRelationalOperator @operator,
            string value)
        {
            Predicate = x => ToPredicate(func(x), @operator, value);
        }

        private static bool ToPredicate(string x, StringRelationalOperator @operator, string y)
        {
            return @operator switch
            {
                StringRelationalOperator.EQUAL => x == y,
                StringRelationalOperator.CONTAIN => x.Contains(y),
                _ => throw new NotImplementedException(),
            };
        }
    }
}
