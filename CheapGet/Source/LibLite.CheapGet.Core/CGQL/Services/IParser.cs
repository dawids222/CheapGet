using LibLite.CheapGet.Core.CGQL.Expressions;
using LibLite.CheapGet.Core.CGQL.Models;

namespace LibLite.CheapGet.Core.CGQL.Services
{
    public interface IParser
    {
        Expression Parse(IEnumerable<Token> tokens);
    }
}
