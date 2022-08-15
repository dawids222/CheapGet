using System.Text.RegularExpressions;

namespace LibLite.CheapGet.Business.Services.DSL
{
    public class Lexer : ILexer
    {
        // TODO: Move all keywords to consts
        private readonly string[] SORT_DIRECTION_TOKENS = new[] { "asc", "desc" };
        private readonly string[] COMPARISON_TOKENS = new[] { ">", ">=", "=", "!=", "<=", "<", "<>" };

        public IEnumerable<Token> Lex(string input)
        {
            var position = 0;
            var tokens = input.Trim().Split(); // TODO: Support literals with whitespaces!!
            foreach (var token in tokens)
            {
                var type = GetTokenType(token, position);
                var value = type != TokenType.TEXT
                    ? token
                    : GetSubstringBetweenFurtherest(token, '"');
                yield return new Token(type, value, position);
                position += token.Length + 1; // TODO: Add the amount of whitespace
            }
            yield return Token.EOF(input.Length);
        }

        private TokenType GetTokenType(string token, int position)
        {
            // TODO: How about making this case insensitive?
            if (token == "select") return TokenType.SELECT;
            if (token == "run") return TokenType.TERMINATOR;
            if (token == "from") return TokenType.FROM;
            if (token == "filter") return TokenType.FILTER;
            if (token == "sort") return TokenType.SORT;
            if (token == "take") return TokenType.TAKE;
            if (token == "cls") return TokenType.CLS;
            if (token == "exit") return TokenType.EXIT;
            if (SORT_DIRECTION_TOKENS.Contains(token)) return TokenType.SORT_DIRECTION;
            if (COMPARISON_TOKENS.Contains(token)) return TokenType.COMPARISON;
            if (ContainsTwoUnescapedQuotationMarks(token) && token.StartsWith('"') && token.EndsWith('"')) return TokenType.TEXT;
            if (token.Contains('.') || token.Contains(',') && double.TryParse(token.Replace('.', ','), out var _)) return TokenType.DECIMAL;
            if (int.TryParse(token, out var _)) return TokenType.INTEGER;
            throw new NotImplementedException($"'{token}' at position {position} is not recognised as a valid token."); // TODO: Add dedicated exception specifying value and position
        }

        private static bool ContainsTwoUnescapedQuotationMarks(string value) => Regex.Matches(value, @"(?<!\\)\""").Count == 2;

        private static string GetSubstringBetweenFurtherest(string value, char character)
            => GetSubstringBetweenFurtherest(value, character, character);

        private static string GetSubstringBetweenFurtherest(string value, char start, char end)
            => Regex.Match(value, $@"\{start}(.*[^{end}]*)\{end}").Groups[1].Value.Replace("\"", "");
    }

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

    public enum TokenType
    {
        SELECT,
        TERMINATOR,
        FROM,
        FILTER,
        SORT,
        TAKE,
        SORT_DIRECTION,
        COMPARISON,
        TEXT,
        DECIMAL,
        INTEGER,
        CLS,
        EXIT,
        EOF,
    }
}
