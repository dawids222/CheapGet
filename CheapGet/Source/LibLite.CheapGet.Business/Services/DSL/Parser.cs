namespace LibLite.CheapGet.Business.Services.DSL
{
    // TODO: Add MORE erorr handling!
    public class Parser : IParser
    {
        private static readonly TokenType[] ROOT_TOKEN_TYPES = new[] { TokenType.SELECT, TokenType.CLS, TokenType.EXIT };
        private static readonly TokenType[] LITERAL_TOKEN_TYPES = new[] { TokenType.TEXT, TokenType.INTEGER, TokenType.DECIMAL };

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
                TokenType.CLS => ParseCls(token),
                TokenType.EXIT => ParseExit(token),
                _ => throw new Exception("ERROR!"), // TODO: Provide meaninigful error
            };
        }

        private Token Eat(TokenType type)
        {
            var token = _tokens.First();
            if (token.Type != type)
            {
                // TODO: Add decicated exception
                var message = $"Expected token type '{type}' but got '{token.Type}' at position {token.Position}";
                throw new Exception(message);
            }
            _tokens.RemoveAt(0);
            return token;
        }

        private Token Eat(IEnumerable<TokenType> types)
        {
            var token = _tokens.First();
            if (!types.Contains(token.Type))
            {
                // TODO: Add decicated exception
                var message = $"Expected token type '{string.Join(", ", types)}' but got '{token.Type}' at position {token.Position}";
                throw new Exception(message);
            }
            _tokens.RemoveAt(0);
            return token;
        }

        private Select ParseSelect(Token select)
        {
            var result = new Select();
            while (true)
            {
                var token = Eat(new TokenType[] { TokenType.FROM, TokenType.FILTER, TokenType.SORT, TokenType.TAKE, TokenType.EOF });
                if (token.Type == TokenType.EOF) { break; }

                var expression = Parse(token);

                if (token.Type == TokenType.FROM) { result.From = expression as From; }
                if (token.Type == TokenType.TAKE) { result.Take = expression as Take; }
                if (token.Type == TokenType.FILTER) { result.Filters.Add(expression as Filter); }
                if (token.Type == TokenType.SORT) { result.Sorts.Add(expression as Sort); }
            }
            return result;
        }

        private From ParseFrom(Token from)
        {
            var text = Eat(TokenType.TEXT);
            return new From(new Text(text.Value));
        }

        private Filter ParseFilter(Token filter)
        {
            var property = Eat(TokenType.TEXT);
            var comparison = Eat(TokenType.COMPARISON);
            var literal = Eat(LITERAL_TOKEN_TYPES);
            var value = ParseLiteral(literal);
            return new Filter(
                new Text(property.Value),
                new Comparison(comparison.Value),
                value);
        }

        private Sort ParseSort(Token sort)
        {
            var property = Eat(TokenType.TEXT);
            var direction = Eat(TokenType.SORT_DIRECTION);
            return new Sort(
                new Text(property.Value),
                new SortDirection(direction.Value));
        }

        private Take ParseTake(Token take)
        {
            var value = Eat(TokenType.INTEGER);
            return new Take(new Integer(int.Parse(value.Value)));
        }

        private static Literal ParseLiteral(Token token)
        {
            return token.Type switch
            {
                TokenType.TEXT => ParseText(token),
                TokenType.INTEGER => ParseInteger(token),
                TokenType.DECIMAL => ParseDecimal(token),
                _ => throw new Exception("ERROR!"), // TODO: Provide meaningful error
            };
        }

        private static Text ParseText(Token text)
        {
            return new Text(text.Value);
        }

        private static Integer ParseInteger(Token integer)
        {
            return new Integer(int.Parse(integer.Value));
        }

        private static Decimal ParseDecimal(Token @decimal)
        {
            return new Decimal(double.Parse(@decimal.Value));
        }

        private Cls ParseCls(Token cls)
        {
            Eat(TokenType.EOF);
            return new Cls();
        }

        private Exit ParseExit(Token exit)
        {
            Eat(TokenType.EOF);
            return new Exit();
        }
    }

    public abstract class Expression { }

    public class Select : Expression
    {
        public From From { get; set; } = new From(new Text("Games"));
        public Take Take { get; set; } = new Take(new Integer(100));
        public List<Filter> Filters { get; set; } = new();
        public List<Sort> Sorts { get; set; } = new();
    }

    public class From : Expression
    {
        public Text Text { get; set; }

        public From(Text text)
        {
            Text = text;
        }
    }

    public class Filter : Expression
    {
        public Text Property { get; set; }
        public Comparison Comparison { get; set; }
        public Literal Value { get; set; }

        public Filter(Text property, Comparison comparison, Literal value)
        {
            Property = property;
            Comparison = comparison;
            Value = value;
        }
    }

    public class Sort : Expression
    {
        public Text Property { get; set; }
        public SortDirection Direction { get; set; }

        public Sort(Text property, SortDirection direction)
        {
            Property = property;
            Direction = direction;
        }
    }

    public class Take : Expression
    {
        public Integer Value { get; set; }

        public Take(Integer value)
        {
            Value = value;
        }
    }

    public class SortDirection : Expression
    {
        public string Value { get; set; }

        public SortDirection(string value)
        {
            Value = value;
        }
    }

    public class Comparison : Expression
    {
        public string Value { get; set; }

        public Comparison(string value)
        {
            Value = value;
        }
    }

    public abstract class Literal : Expression
    {
        public abstract TokenType Type { get; }

        public Text AsText() => this as Text;
        public Integer AsInteger() => this as Integer;
        public Decimal AsDecimal() => this as Decimal;
    }

    public class Text : Literal
    {
        public override TokenType Type => TokenType.TEXT;

        public string Value { get; set; }

        public Text(string value)
        {
            Value = value;
        }
    }

    public class Decimal : Literal
    {
        public override TokenType Type => TokenType.DECIMAL;

        public double Value { get; set; }

        public Decimal(double value)
        {
            Value = value;
        }
    }

    public class Integer : Literal
    {
        public override TokenType Type => TokenType.INTEGER;

        public int Value { get; set; }

        public Integer(int value)
        {
            Value = value;
        }
    }

    public class Cls : Expression { }

    public class Exit : Expression { }
}
