using LibLite.CheapGet.Business.Exceptions.DSL;

namespace LibLite.CheapGet.Business.Services.DSL
{
    // TODO: Add MORE erorr handling!
    public class Parser : IParser
    {
        // TODO: Move
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
                _ => throw new TokenNotSupportedException(token),
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
            var comparison = Eat(TokenType.COMPARISON);
            var literal = Eat(LITERAL_TOKEN_TYPES);
            var value = ParseLiteral(literal);
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

    public abstract class Expression { }

    public class Select : Expression
    {
        public From From { get; set; } = new From(new Text("Games"));
        public Take Take { get; set; } = new Take(new Integer(100));
        public List<Filter> Filters { get; set; } = new();
        public List<Sort> Sorts { get; set; } = new();

        public override bool Equals(object obj)
        {
            return obj is Select select &&
                   EqualityComparer<From>.Default.Equals(From, select.From) &&
                   EqualityComparer<Take>.Default.Equals(Take, select.Take) &&
                   Filters.SequenceEqual(select.Filters) &&
                   Sorts.SequenceEqual(select.Sorts);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(From, Take, Filters, Sorts);
        }
    }

    public class From : Expression
    {
        public Text Text { get; set; }

        public From(Text text)
        {
            Text = text;
        }

        public override bool Equals(object obj)
        {
            return obj is From from &&
                   EqualityComparer<Text>.Default.Equals(Text, from.Text);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Text);
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

        public override bool Equals(object obj)
        {
            return obj is Filter filter &&
                   EqualityComparer<Text>.Default.Equals(Property, filter.Property) &&
                   EqualityComparer<Comparison>.Default.Equals(Comparison, filter.Comparison) &&
                   EqualityComparer<Literal>.Default.Equals(Value, filter.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Property, Comparison, Value);
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

        public override bool Equals(object obj)
        {
            return obj is Sort sort &&
                   EqualityComparer<Text>.Default.Equals(Property, sort.Property) &&
                   EqualityComparer<SortDirection>.Default.Equals(Direction, sort.Direction);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Property, Direction);
        }
    }

    public class Take : Expression
    {
        public Integer Value { get; set; }

        public Take(Integer value)
        {
            Value = value;
        }

        public override bool Equals(object obj)
        {
            return obj is Take take &&
                   EqualityComparer<Integer>.Default.Equals(Value, take.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value);
        }
    }

    public class SortDirection : Expression
    {
        public string Value { get; set; }

        public SortDirection(string value)
        {
            Value = value;
        }

        public override bool Equals(object obj)
        {
            return obj is SortDirection direction &&
                   Value == direction.Value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value);
        }
    }

    public class Comparison : Expression
    {
        public string Value { get; set; }

        public Comparison(string value)
        {
            Value = value;
        }

        public override bool Equals(object obj)
        {
            return obj is Comparison comparison &&
                   Value == comparison.Value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value);
        }
    }

    public abstract class Literal : Expression
    {
        public abstract TokenType Type { get; }

        public Text AsText() => this as Text;
        public Integer AsInteger() => this as Integer;
        public Floating AsDecimal() => this as Floating;
    }

    public class Text : Literal
    {
        public override TokenType Type => TokenType.TEXT;

        public string Value { get; set; }

        public Text(string value)
        {
            Value = value;
        }

        public override bool Equals(object obj)
        {
            return obj is Text text &&
                   Type == text.Type &&
                   Value == text.Value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, Type, Value);
        }
    }

    public class Floating : Literal // TODO: Consider a better name...
    {
        public override TokenType Type => TokenType.FLOATING;

        public double Value { get; set; }

        public Floating(double value)
        {
            Value = value;
        }

        public override bool Equals(object obj)
        {
            return obj is Floating @decimal &&
                   Type == @decimal.Type &&
                   Value == @decimal.Value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, Value);
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

        public override bool Equals(object obj)
        {
            return obj is Integer integer &&
                   Type == integer.Type &&
                   Value == integer.Value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, Value);
        }
    }

    public class Cls : Expression
    {

        public override bool Equals(object obj)
        {
            return obj is Cls;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }

    public class Exit : Expression
    {
        public override bool Equals(object obj)
        {
            return obj is Exit;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
}
