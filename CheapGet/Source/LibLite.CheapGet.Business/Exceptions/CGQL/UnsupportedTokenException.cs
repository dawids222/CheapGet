using LibLite.CheapGet.Core.CGQL.Models;

namespace LibLite.CheapGet.Business.Exceptions.CGQL
{
    public class UnsupportedTokenException : Exception
    {
        public Token Token { get; }

        public UnsupportedTokenException(Token token)
            : base($"{token.Value} of type '{token.Type}' currently is not supported at position {token.Position}")
        {
            Token = token;
        }
    }
}
