using LibLite.CheapGet.Core.CGQL.Enums;

namespace LibLite.CheapGet.Core.CGQL.Models
{
    public class Token
    {
        public TokenType Type { get; }
        public string Value { get; }
        public int Position { get; }

        public Token(TokenType type, string value, int position)
        {
            Type = type;
            Value = value;
            Position = position;
        }

        public static Token EOF(int lenght)
        {
            return new Token(TokenType.EOF, "", lenght);
        }

        public override bool Equals(object obj)
        {
            return obj is Token token &&
                   Type == token.Type &&
                   Value == token.Value &&
                   Position == token.Position;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, Value, Position);
        }
    }
}
