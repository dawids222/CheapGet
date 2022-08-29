using LibLite.CheapGet.Core.CGQL.Expressions;

namespace LibLite.CheapGet.Core.CGQL.Services
{
    public interface IInterpreter
    {
        Task InterpretAsync(Expression expression);
    }
}
