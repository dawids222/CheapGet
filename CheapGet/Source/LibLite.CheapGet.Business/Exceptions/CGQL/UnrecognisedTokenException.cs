namespace LibLite.CheapGet.Business.Exceptions.CGQL
{
    public class UnrecognisedTokenException : Exception
    {
        public string Token { get; }
        public int Position { get; }

        public UnrecognisedTokenException(string token, int position)
            : base($"'{token}' is not recognised as a valid token at position {position}.")
        {
            Token = token;
            Position = position;
        }
    }
}
