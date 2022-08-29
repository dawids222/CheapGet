using LibLite.CheapGet.Core.CGQL.Enums;
using LibLite.CheapGet.Core.CGQL.Models;

namespace LibLite.CheapGet.Business.Exceptions.DSL
{
    public class UnexpectedTokenException : Exception
    {
        public Token Token { get; }
        public IEnumerable<TokenType> ExpectedTypes { get; }

        public UnexpectedTokenException(Token token, TokenType expectedType)
            : base($"Expected token type '{expectedType}' but got '{token.Type}' at position {token.Position}")
        {
            Token = token;
            ExpectedTypes = new TokenType[] { expectedType };
        }

        public UnexpectedTokenException(Token token, IEnumerable<TokenType> expectedTypes)
            : base($"Expected token type [{string.Join(", ", expectedTypes)}] but got '{token.Type}' at position {token.Position}")
        {
            Token = token;
            ExpectedTypes = expectedTypes;
        }
    }
}
