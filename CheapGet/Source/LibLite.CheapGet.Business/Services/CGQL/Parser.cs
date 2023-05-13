using LibLite.CheapGet.Business.Consts.CGQL;
using LibLite.CheapGet.Business.Exceptions.CGQL;
using LibLite.CheapGet.Core.CGQL.Enums;
using LibLite.CheapGet.Core.CGQL.Expressions;
using LibLite.CheapGet.Core.CGQL.Models;
using LibLite.CheapGet.Core.CGQL.Services;

namespace LibLite.CheapGet.Business.Services.CGQL
{
    // TODO: Consolidate error messages
    public class Parser : IParser
    {
        public static readonly TokenType[] ROOT_TOKEN_TYPES = new[] { TokenType.SELECT, TokenType.WISHLIST, TokenType.LOAD, TokenType.CLS, TokenType.EXIT };
        public static readonly TokenType[] LITERAL_TOKEN_TYPES = new[] { TokenType.TEXT, TokenType.INTEGER, TokenType.FLOATING };
        public static readonly TokenType[] NUMERIC_TOKEN_TYPES = new[] { TokenType.INTEGER, TokenType.FLOATING };
        public static readonly TokenType[] SELECT_EXPECTED_TOKEN_TYPES = new TokenType[] { TokenType.FROM, TokenType.FILTER, TokenType.SORT, TokenType.TAKE, TokenType.EOF };
        public static readonly TokenType[] WISHLIST_EXPECTED_TOKEN_TYPES = new TokenType[] { TokenType.FROM, TokenType.WISH, TokenType.MAX, TokenType.EOF };

        private IList<Token> _tokens;

        public Expression Parse(IEnumerable<Token> tokens)
        {
            _tokens = tokens.ToList();
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
                TokenType.WISHLIST => ParseWishlist(token),
                TokenType.WISH => ParseWish(token),
                TokenType.MAX => ParseMax(token),
                TokenType.LOAD => ParseLoad(token),
                TokenType.CLS => ParseCls(token),
                TokenType.EXIT => ParseExit(token),
                _ => throw new UnsupportedTokenException(token),
            };
        }

        private Token Eat(params TokenType[] types)
        {
            var token = _tokens.First();

            if (token.Type == TokenType.UNRECOGNISED)
            {
                throw new AggregateException(
                    new UnrecognisedTokenException(token.Value, token.Position),
                    new UnexpectedTokenException(token, types));
            }
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
            var text = EatCategory();
            return new From(new Text(text.Value));
        }

        private Token EatCategory()
        {
            var category = Eat(TokenType.TEXT);
            if (!Categories.IsCategory(category.Value))
            {
                throw new UnexpectedValueException(category, Categories.ALL);
            }
            return category;
        }

        private Filter ParseFilter(Token _)
        {
            var property = EatProperty();
            var comparison = EatComparison(property);
            var literal = EatLiteral(property);
            return new Filter(
                new Text(property.Value),
                new Comparison(comparison.Value),
                ParseLiteral(literal));
        }

        private Sort ParseSort(Token _)
        {
            var property = EatProperty();
            var direction = Eat(TokenType.SORT_DIRECTION);
            return new Sort(
                new Text(property.Value),
                new SortDirection(direction.Value));
        }

        private Token EatProperty()
        {
            var property = Eat(TokenType.TEXT);
            if (!Properties.IsProperty(property.Value))
            {
                throw new UnexpectedValueException(property, Properties.ALL);
            }
            return property;
        }

        private Token EatComparison(Token property)
        {
            var comparison = Eat(TokenType.COMPARISON);
            if (Properties.IsTextProperty(property.Value) &&
                !Comparisons.IsTextComparison(comparison.Value))
            {
                throw new UnexpectedValueException(comparison, Comparisons.TEXT_COMPARISONS);
            }
            if (Properties.IsNumericProperty(property.Value) &&
                !Comparisons.IsNumericComparison(comparison.Value))
            {
                throw new UnexpectedValueException(comparison, Comparisons.NUMERIC_COMPARISONS);
            }
            return comparison;
        }

        private Token EatLiteral(Token property)
        {
            return Properties.IsNumericProperty(property.Value)
                ? Eat(NUMERIC_TOKEN_TYPES)
                : Eat(TokenType.TEXT);
        }

        private Literal ParseLiteral(Token token)
        {
            if (!LITERAL_TOKEN_TYPES.Contains(token.Type))
            {
                throw new UnexpectedTokenException(token, LITERAL_TOKEN_TYPES);
            }
            return Parse(token) as Literal;
        }

        private Take ParseTake(Token _)
        {
            var token = Eat(TokenType.INTEGER);
            var value = int.Parse(token.Value);
            return new Take(new Integer(value));
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

        private Wishlist ParseWishlist(Token _)
        {
            var result = new Wishlist();
            while (true)
            {
                var token = Eat(WISHLIST_EXPECTED_TOKEN_TYPES);
                if (token.Type == TokenType.EOF) { break; }

                var expression = Parse(token);
                if (token.Type == TokenType.FROM) { result.From = expression as From; }
                if (token.Type == TokenType.MAX) { result.Max = expression as Max; }
                if (token.Type == TokenType.WISH) { result.Wishes.Add(expression as Wish); }
            }
            return result;
        }

        private Wish ParseWish(Token _)
        {
            var result = new Wish();
            var token = TryEatFilter();
            while (token is not null)
            {
                var filter = ParseFilter(token);
                result.Filters.Add(filter);
                token = TryEatFilter();
            }
            return result;
        }

        private Token TryEatFilter()
        {
            Token token;
            try { token = Eat(TokenType.FILTER); }
            catch (Exception) { return null; }
            return token;
        }

        private Max ParseMax(Token _)
        {
            var token = Eat(TokenType.INTEGER);
            var value = ParseInteger(token);
            return new Max(value);
        }

        private Load ParseLoad(Token _)
        {
            var source = Eat(TokenType.TEXT);
            var text = new Text(source.Value);
            return new Load(text);
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
