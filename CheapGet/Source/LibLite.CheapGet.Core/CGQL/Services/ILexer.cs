using LibLite.CheapGet.Core.CGQL.Models;

namespace LibLite.CheapGet.Core.CGQL.Services
{
    public interface ILexer
    {
        IEnumerable<Token> Lex(string input);
    }
}
