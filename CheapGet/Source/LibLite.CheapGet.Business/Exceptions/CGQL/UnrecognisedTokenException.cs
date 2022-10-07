using LibLite.CheapGet.Core.CGQL.Enums;

namespace LibLite.CheapGet.Business.Exceptions.CGQL
{
    public class UnrecognisedTokenException : Exception
    {
        public string Token { get; }
        public int Position { get; }

        public UnrecognisedTokenException(string token, int position)
            : base($"Token '{token}' is of type '{TokenType.UNRECOGNISED}' and can not be processed in any way at position {position}.")
        {
            Token = token;
            Position = position;
        }
    }
}
