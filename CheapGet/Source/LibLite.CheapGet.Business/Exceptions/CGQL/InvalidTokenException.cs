namespace LibLite.CheapGet.Business.Exceptions.CGQL
{
    public class InvalidTokenException : Exception
    {
        public string Token { get; }
        public int Position { get; }

        public InvalidTokenException(string token, int position)
            : base($"'{token}' at position {position} is not recognised as a valid token.")
        {
            Token = token;
            Position = position;
        }
    }
}
