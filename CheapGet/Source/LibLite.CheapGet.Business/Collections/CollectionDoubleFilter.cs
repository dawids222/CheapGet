using LibLite.CheapGet.Core.Collections;
using LibLite.CheapGet.Core.Enums;

namespace LibLite.CheapGet.Business.Collections
{
    public class CollectionDoubleFilter<T> : CollectionFilter<T>
    {
        public CollectionDoubleFilter(
            Func<T, double> func,
            NumberRelationalOperator @operator,
            double value)
        {
            Predicate = x => ToPredicate(func(x), @operator, value);
        }

        private static bool ToPredicate(double x, NumberRelationalOperator @operator, double y)
        {
            return @operator switch
            {
                NumberRelationalOperator.GREATER => x > y,
                NumberRelationalOperator.GREATER_OR_EQUAL => x >= y,
                NumberRelationalOperator.EQUAL => x == y,
                NumberRelationalOperator.NOT_EQUAL => x != y,
                NumberRelationalOperator.LESS_OR_EQUAL => x <= y,
                NumberRelationalOperator.LESS => x < y,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
