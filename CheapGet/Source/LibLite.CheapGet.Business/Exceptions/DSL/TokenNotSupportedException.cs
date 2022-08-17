using LibLite.CheapGet.Business.Services.DSL;

namespace LibLite.CheapGet.Business.Exceptions.DSL
{
    public class TokenNotSupportedException : Exception
    {
        public Token Token { get; }

        public TokenNotSupportedException(Token token)
            : base($"{token.Value} of type '{token.Type}' currently is not supported")
        {
            Token = token;
        }
    }
}
