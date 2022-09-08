using LibLite.CheapGet.Core.CGQL.Models;

namespace LibLite.CheapGet.Business.Exceptions.CGQL
{
    public class UnexpectedValueException : Exception
    {
        public Token Token { get; }
        public IEnumerable<string> Expected { get; }

        public UnexpectedValueException(Token token, IEnumerable<string> expected)
            : base($"Expected value {Merge(expected)} but got '{token.Value}' at position {token.Position}")
        {
            Token = token;
            Expected = expected;
        }

        private static string Merge(IEnumerable<string> expected) => $"[{string.Join(", ", expected)}]";
    }
}
