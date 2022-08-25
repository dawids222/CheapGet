using LibLite.CheapGet.Business.Services.DSL;

namespace LibLite.CheapGet.Business.Exceptions.DSL
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
