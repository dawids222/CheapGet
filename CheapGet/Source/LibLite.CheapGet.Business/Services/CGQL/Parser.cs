using LibLite.CheapGet.Business.Consts.CGQL;
using LibLite.CheapGet.Business.Exceptions.DSL;
using LibLite.CheapGet.Core.CGQL.Enums;
using LibLite.CheapGet.Core.CGQL.Expressions;
using LibLite.CheapGet.Core.CGQL.Models;
using LibLite.CheapGet.Core.CGQL.Services;

namespace LibLite.CheapGet.Business.Services.CGQL
{
    // TODO: Add MORE erorr handling!
    public class Parser : IParser
    {
        // TODO: Move?
        public static readonly TokenType[] ROOT_TOKEN_TYPES = new[] { TokenType.SELECT, TokenType.CLS, TokenType.EXIT };
        public static readonly TokenType[] LITERAL_TOKEN_TYPES = new[] { TokenType.TEXT, TokenType.INTEGER, TokenType.FLOATING };
        public static readonly TokenType[] SELECT_EXPECTED_TOKEN_TYPES = new TokenType[] { TokenType.FROM, TokenType.FILTER, TokenType.SORT, TokenType.TAKE, TokenType.EOF };

        private List<Token> _tokens;

        public Expression Parse(IEnumerable<Token> tokens)
        {
            _tokens = new List<Token>(tokens);
            var token = Eat(ROOT_TOKEN_TYPES);
            return Parse(token);
        }

        private Expression Parse(Token token)
        {
            return token.Type switch
            {
                TokenType.SELECT => ParseSelect(token),
                TokenType.FROM => ParseFrom(token),
                TokenType.FILTER => ParseFilter(token),
                TokenType.SORT => ParseSort(token),
                TokenType.TAKE => ParseTake(token),
                TokenType.TEXT => ParseText(token),
                TokenType.INTEGER => ParseInteger(token),
                TokenType.FLOATING => ParseFloating(token),
                TokenType.CLS => ParseCls(token),
                TokenType.EXIT => ParseExit(token),
                _ => throw new UnsupportedTokenException(token),
            };
        }

        private Token Eat(TokenType type)
        {
            var token = _tokens.First();
            if (token.Type != type)
            {
                throw new UnexpectedTokenException(token, type);
            }
            _tokens.RemoveAt(0);
            return token;
        }

        private Token Eat(IEnumerable<TokenType> types)
        {
            var token = _tokens.First();
            if (!types.Contains(token.Type))
            {
                throw new UnexpectedTokenException(token, types);
            }
            _tokens.RemoveAt(0);
            return token;
        }

        private Select ParseSelect(Token _)
        {
            var result = new Select();
            while (true)
            {
                var token = Eat(SELECT_EXPECTED_TOKEN_TYPES);
                if (token.Type == TokenType.EOF) { break; }

                var expression = Parse(token);

                if (token.Type == TokenType.FROM) { result.From = expression as From; }
                if (token.Type == TokenType.TAKE) { result.Take = expression as Take; }
                if (token.Type == TokenType.FILTER) { result.Filters.Add(expression as Filter); }
                if (token.Type == TokenType.SORT) { result.Sorts.Add(expression as Sort); }
            }
            return result;
        }

        private From ParseFrom(Token _)
        {
            var text = Eat(TokenType.TEXT);
            return new From(new Text(text.Value));
        }

        private Filter ParseFilter(Token _) // TODO: Validate property, comaprison and value
        {
            var property = Eat(TokenType.TEXT);
            if (!Properties.ALL.Contains(property.Value))
            {
                // TODO: Error!
            }
            var comparison = Eat(TokenType.COMPARISON);
            if (Properties.STRING_PROPERTIES.Contains(property.Value) && !Operators.STRING_OPERATORS.Contains(comparison.Value))
            {
                // TODO: Error!
            }
            if (Properties.DECIMAL_PROPERTIES.Contains(property.Value) && !Operators.DECIMAL_PROPERTIES.Contains(comparison.Value))
            {
                // TODO: Error!
            }
            var literal = Eat(LITERAL_TOKEN_TYPES);
            var value = ParseLiteral(literal);
            if (Properties.STRING_PROPERTIES.Contains(property.Value) && value.Type != TokenType.TEXT)
            {
                // TODO: Error!
            }
            if (Properties.DECIMAL_PROPERTIES.Contains(property.Value) && value.Type != TokenType.INTEGER && value.Type != TokenType.FLOATING)
            {
                // TODO: Error!
            }
            return new Filter(
                new Text(property.Value),
                new Comparison(comparison.Value),
                value);
        }

        private Sort ParseSort(Token _) // TODO: Validate property
        {
            var property = Eat(TokenType.TEXT);
            var direction = Eat(TokenType.SORT_DIRECTION);
            return new Sort(
                new Text(property.Value),
                new SortDirection(direction.Value));
        }

        private Take ParseTake(Token _)
        {
            var value = Eat(TokenType.INTEGER);
            return new Take(new Integer(int.Parse(value.Value)));
        }

        private Literal ParseLiteral(Token token)
        {
            if (!LITERAL_TOKEN_TYPES.Contains(token.Type))
            {
                throw new UnexpectedTokenException(token, LITERAL_TOKEN_TYPES);
            }
            return Parse(token) as Literal;
        }

        private static Text ParseText(Token text)
        {
            return new Text(text.Value);
        }

        private static Integer ParseInteger(Token integer)
        {
            var value = int.Parse(integer.Value);
            return new Integer(value);
        }

        private static Floating ParseFloating(Token floating)
        {
            var value = double.Parse(floating.Value);
            return new Floating(value);
        }

        private Cls ParseCls(Token _)
        {
            Eat(TokenType.EOF);
            return new Cls();
        }

        private Exit ParseExit(Token _)
        {
            Eat(TokenType.EOF);
            return new Exit();
        }
    }
}
