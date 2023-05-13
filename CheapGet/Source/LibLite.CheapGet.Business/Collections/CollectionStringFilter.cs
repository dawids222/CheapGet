using LibLite.CheapGet.Core.Collections;
using StringRelationalOperator = LibLite.CheapGet.Core.Enums.StringRelationalOperator;

namespace LibLite.CheapGet.Business.Collections
{
    public class CollectionStringFilter<T> : CollectionFilter<T>
    {
        public StringRelationalOperator Operator { get; }
        public string Value { get; }

        public CollectionStringFilter(
            Func<T, string> func,
            StringRelationalOperator @operator,
            string value)
        {
            Operator = @operator;
            Value = value;
            Predicate = x => ToPredicate(func(x), @operator, value);
        }

        private static bool ToPredicate(string x, StringRelationalOperator @operator, string y)
        {
            return @operator switch
            {
                StringRelationalOperator.EQUAL => x == y,
                StringRelationalOperator.CONTAIN => x.Contains(y, StringComparison.InvariantCultureIgnoreCase),
                _ => throw new NotImplementedException(),
            };
        }
    }
}
