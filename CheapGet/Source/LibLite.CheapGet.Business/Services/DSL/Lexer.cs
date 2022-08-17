using System.Text.RegularExpressions;

namespace LibLite.CheapGet.Business.Services.DSL
{
    public class Lexer : ILexer
    {
        // TODO: Move all keywords to consts
        // TODO: Move other classes to separate files
        // TODO: Move this class to another project?
        // TODO: Implement HELP
        private readonly string[] SORT_DIRECTION_TOKENS = new[] { "asc", "desc" };
        private readonly string[] COMPARISON_TOKENS = new[] { ">", ">=", "=", "!=", "<=", "<", "<>" };

        public IEnumerable<Token> Lex(string input)
        {
            var position = 0;
            var tokens = SplitIntoTokens(input);
            foreach (var token in tokens)
            {
                var type = GetTokenType(token, position);
                var value = GetTokenValue(token, type);
                yield return new Token(type, value, position);
                position += GetPositionsToNextToken(token, input, position);
            }
            yield return Token.EOF(input.Length);
        }

        private static IEnumerable<string> SplitIntoTokens(string input)
        {
            var trimmed = input.Trim();
            return Regex
                .Matches(trimmed, @"[\""].+?[\""]|[^ ]+")
                .Select(m => m.Value)
                .ToList();
        }

        private TokenType GetTokenType(string token, int position)
        {
            var lowerToken = token.ToLower();
            if (lowerToken == "select") return TokenType.SELECT;
            if (lowerToken == "from") return TokenType.FROM;
            if (lowerToken == "filter") return TokenType.FILTER;
            if (lowerToken == "sort") return TokenType.SORT;
            if (lowerToken == "take") return TokenType.TAKE;
            if (lowerToken == "cls") return TokenType.CLS;
            if (lowerToken == "exit") return TokenType.EXIT;
            if (IsSortDirectionToken(lowerToken)) return TokenType.SORT_DIRECTION;
            if (IsComparisonToken(lowerToken)) return TokenType.COMPARISON;
            if (IsDecimalToken(lowerToken)) return TokenType.FLOATING;
            if (IsIntegerToken(lowerToken)) return TokenType.INTEGER;
            if (IsTextToken(token)) return TokenType.TEXT;
            throw new NotImplementedException($"'{token}' at position {position} is not recognised as a valid token."); // TODO: Add dedicated exception specifying value and position
        }

        private bool IsSortDirectionToken(string token) => SORT_DIRECTION_TOKENS.Contains(token);
        private bool IsComparisonToken(string token) => COMPARISON_TOKENS.Contains(token);
        private static bool IsDecimalToken(string token) => (token.Contains('.') || token.Contains(',')) && double.TryParse(token.Replace('.', ','), out var _);
        private static bool IsIntegerToken(string token) => int.TryParse(token, out var _);
        private static bool ContainsTwoUnescapedQuotationMarks(string token) => Regex.Matches(token, @"(?<!\\)\""").Count == 2;
        private static bool IsTextToken(string token) => ContainsTwoUnescapedQuotationMarks(token) && token.StartsWith('"') && token.EndsWith('"');
        private static string GetSubstringBetweenFurtherest(string value, char character) => GetSubstringBetweenFurtherest(value, character, character);
        private static string GetSubstringBetweenFurtherest(string value, char start, char end) => Regex.Match(value, $@"\{start}(.*[^{end}]*)\{end}").Groups[1].Value.Replace("\"", "");

        private static string GetTokenValue(string token, TokenType type)
        {
            return type switch
            {
                TokenType.TEXT => GetSubstringBetweenFurtherest(token, '"'),
                TokenType.FLOATING => token.Replace('.', ','),
                _ => token,
            };
        }

        private static int GetPositionsToNextToken(string token, string input, int position)
        {
            var whitespace = input[(position + token.Length)..]
                .TakeWhile(char.IsWhiteSpace)
                .Count();
            return token.Length + whitespace;
        }
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
        FROM,
        FILTER,
        SORT,
        TAKE,
        SORT_DIRECTION,
        COMPARISON,
        TEXT,
        FLOATING,
        INTEGER,
        CLS,
        EXIT,
        EOF,
    }
}
