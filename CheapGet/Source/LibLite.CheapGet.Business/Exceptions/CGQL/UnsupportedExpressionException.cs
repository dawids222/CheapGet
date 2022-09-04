using LibLite.CheapGet.Core.CGQL.Expressions;

namespace LibLite.CheapGet.Business.Exceptions.CGQL
{
    public class UnsupportedExpressionException : Exception
    {
        public Expression Expression { get; }

        public UnsupportedExpressionException(Expression expression)
            : base($"Expression of type '{expression.GetType().FullName}' currently is not supported")
        {
            Expression = expression;
        }
    }
}
