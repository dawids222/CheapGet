using LibLite.CheapGet.Business.Consts.CGQL;
using LibLite.CheapGet.Business.Exceptions.CGQL;
using LibLite.CheapGet.Business.Services.CGQL;
using LibLite.CheapGet.Core.CGQL.Enums;
using LibLite.CheapGet.Core.CGQL.Expressions;
using LibLite.CheapGet.Core.CGQL.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibLite.CheapGet.Business.Tests.Services.CGQL
{
    [TestFixture]
    public class ParserTests
    {
        private Parser _parser;

        [SetUp]
        public void SetUp()
        {
            _parser = new();
        }

        [TestCaseSource(nameof(_parseValidTestCases))]
        public void Parse_ParsesValidInput_ReturnsExpectedExpression(ParseValidTestCase test)
        {
            var result = _parser.Parse(test.Tokens);

            Assert.AreEqual(test.Expected, result);
        }

        private static readonly IEnumerable<ParseValidTestCase> _parseValidTestCases = new List<ParseValidTestCase>
        {
            new ParseValidTestCase
            {
                Tokens = new List<Token>
                {
                    new Token(TokenType.SELECT, Keywords.SELECT, 0),
                    new Token(TokenType.EOF, string.Empty, 6),
                },
                Expected = new Select(),
            },
            new ParseValidTestCase
            {
                Tokens = new List<Token>
                {
                    new Token(TokenType.CLS, Keywords.CLS, 0),
                    new Token(TokenType.EOF, string.Empty, 3),
                },
                Expected = new Cls(),
            },
            new ParseValidTestCase
            {
                Tokens = new List<Token>
                {
                    new Token(TokenType.EXIT, Keywords.EXIT, 0),
                    new Token(TokenType.EOF, string.Empty, 4),
                },
                Expected = new Exit(),
            },
            new ParseValidTestCase
            {
                Tokens = new List<Token>
                {
                    new Token(TokenType.LOAD, Keywords.LOAD, 0),
                    new Token(TokenType.TEXT, "query.cgql", 5),
                    new Token(TokenType.EOF, string.Empty, 17),
                },
                Expected = new Load(new Text("query.cgql")),
            },
            new ParseValidTestCase
            {
                Tokens = new List<Token>
                {
                    new Token(TokenType.SELECT, Keywords.SELECT, 0),
                    new Token(TokenType.FROM, Keywords.FROM, 7),
                    new Token(TokenType.TEXT, Categories.GAMES, 12),
                    new Token(TokenType.FILTER, Keywords.FILTER, 17),
                    new Token(TokenType.TEXT, Properties.BASE_PRICE, 24),
                    new Token(TokenType.COMPARISON, Comparisons.GREATER_OR_EQUAL, 35),
                    new Token(TokenType.FLOATING, "49,99", 38),
                    new Token(TokenType.FILTER, Keywords.FILTER, 44),
                    new Token(TokenType.TEXT, Properties.DISCOUNTED_PRICE, 51),
                    new Token(TokenType.COMPARISON, Comparisons.LESS_OR_EQUAL, 68),
                    new Token(TokenType.INTEGER, "100", 71),
                    new Token(TokenType.SORT, Keywords.SORT, 75),
                    new Token(TokenType.TEXT, Properties.NAME, 80),
                    new Token(TokenType.SORT_DIRECTION, Keywords.DESC, 85),
                    new Token(TokenType.SORT, Keywords.SORT, 90),
                    new Token(TokenType.TEXT, Properties.STORE_NAME, 95),
                    new Token(TokenType.SORT_DIRECTION, Keywords.ASC, 105),
                    new Token(TokenType.TAKE, Keywords.TAKE, 109),
                    new Token(TokenType.INTEGER, "50", 114),
                    new Token(TokenType.EOF, string.Empty, 115),
                },
                Expected = new Select()
                {
                    From = new From(new Text(Categories.GAMES)),
                    Take = new Take(new Integer(50)),
                    Filters = new List<Filter>
                    {
                        new Filter(new Text(Properties.BASE_PRICE), new Comparison(Comparisons.GREATER_OR_EQUAL), new Floating(49.99)),
                        new Filter(new Text(Properties.DISCOUNTED_PRICE), new Comparison(Comparisons.LESS_OR_EQUAL), new Integer(100)),
                    },
                    Sorts = new List<Sort>
                    {
                        new Sort(new Text(Properties.NAME), new SortDirection(Keywords.DESC)),
                        new Sort(new Text(Properties.STORE_NAME), new SortDirection(Keywords.ASC)),
                    },
                },
            },
            new ParseValidTestCase
            {
                Tokens = new List<Token>
                {
                    new Token(TokenType.SELECT, Keywords.SELECT, 0),
                    new Token(TokenType.SORT, Keywords.SORT, 7),
                    new Token(TokenType.TEXT, Properties.NAME, 12),
                    new Token(TokenType.SORT_DIRECTION, Keywords.DESC, 17),
                    new Token(TokenType.TAKE, Keywords.TAKE, 22),
                    new Token(TokenType.INTEGER, "50", 27),
                    new Token(TokenType.FILTER, Keywords.FILTER, 30),
                    new Token(TokenType.TEXT, Properties.BASE_PRICE, 37),
                    new Token(TokenType.COMPARISON, Comparisons.GREATER_OR_EQUAL, 48),
                    new Token(TokenType.FLOATING, "49,99", 51),
                    new Token(TokenType.FROM, Keywords.FROM, 57),
                    new Token(TokenType.TEXT, Categories.GAMES, 62),
                    new Token(TokenType.SORT, Keywords.SORT, 67),
                    new Token(TokenType.TEXT, Properties.STORE_NAME, 72),
                    new Token(TokenType.SORT_DIRECTION, Keywords.ASC, 83),
                    new Token(TokenType.FILTER, Keywords.FILTER, 87),
                    new Token(TokenType.TEXT, Properties.DISCOUNTED_PRICE, 94),
                    new Token(TokenType.COMPARISON, Comparisons.LESS_OR_EQUAL, 111),
                    new Token(TokenType.INTEGER, "100", 114),
                    new Token(TokenType.EOF, string.Empty, 115),
                },
                Expected = new Select()
                {
                    From = new From(new Text(Categories.GAMES)),
                    Take = new Take(new Integer(50)),
                    Filters = new List<Filter>
                    {
                        new Filter(new Text(Properties.BASE_PRICE), new Comparison(Comparisons.GREATER_OR_EQUAL), new Floating(49.99)),
                        new Filter(new Text(Properties.DISCOUNTED_PRICE), new Comparison(Comparisons.LESS_OR_EQUAL), new Integer(100)),
                    },
                    Sorts = new List<Sort>
                    {
                        new Sort(new Text(Properties.NAME), new SortDirection(Keywords.DESC)),
                        new Sort(new Text(Properties.STORE_NAME), new SortDirection(Keywords.ASC)),
                    },
                },
            },
            new ParseValidTestCase
            {
                Tokens = new List<Token>
                {
                    new Token(TokenType.WISHLIST, "wishlist", 0),
                    new Token(TokenType.FROM, "from", 9),
                    new Token(TokenType.TEXT, "Games", 14),
                    new Token(TokenType.WISH, "wish", 22),
                    new Token(TokenType.FILTER, "filter", 27),
                    new Token(TokenType.TEXT, "name", 34),
                    new Token(TokenType.COMPARISON, "<>", 41),
                    new Token(TokenType.TEXT, "south park", 44),
                    new Token(TokenType.WISH, "wish", 57),
                    new Token(TokenType.FILTER, "filter", 62),
                    new Token(TokenType.TEXT, "name", 69),
                    new Token(TokenType.COMPARISON, "<>", 76),
                    new Token(TokenType.TEXT, "cyberpunk 2077", 79),
                    new Token(TokenType.FILTER, "filter", 96),
                    new Token(TokenType.TEXT, "discounted_price", 103),
                    new Token(TokenType.COMPARISON, "<=", 122),
                    new Token(TokenType.FLOATING, "60,1", 125),
                    new Token(TokenType.MAX, "max", 130),
                    new Token(TokenType.INTEGER, "300", 134),
                    new Token(TokenType.EOF, "", 137),
                },
                Expected = new Wishlist()
                {
                    From = new From(new Text(Categories.GAMES)),
                    Max = new Max(new Integer(300)),
                    Wishes = new()
                    {
                        new Wish
                        {
                            Filters = new()
                            {
                                new(new Text(Properties.NAME), new Comparison(Comparisons.CONTAIN), new Text("south park")),
                            },
                        },
                        new Wish
                        {
                            Filters = new()
                            {
                                new(new Text(Properties.NAME), new Comparison(Comparisons.CONTAIN), new Text("cyberpunk 2077")),
                                new(new Text(Properties.DISCOUNTED_PRICE), new Comparison(Comparisons.LESS_OR_EQUAL), new Floating(60.1)),
                            },
                        },
                    },
                },
            },
            new ParseValidTestCase
            {
                Tokens = new List<Token>()
                {
                    new Token(TokenType.COMBINE, "combine", 0),
                    new Token(TokenType.SELECT, "select", 9),
                    new Token(TokenType.TAKE, "take", 16),
                    new Token(TokenType.INTEGER, "100", 21),
                    new Token(TokenType.FILTER, "filter", 25),
                    new Token(TokenType.TEXT, "store_name", 32),
                    new Token(TokenType.COMPARISON, "<>", 45),
                    new Token(TokenType.TEXT, "steam", 48),
                    new Token(TokenType.FILTER, "filter", 56),
                    new Token(TokenType.TEXT, "discount_percentage", 63),
                    new Token(TokenType.COMPARISON, ">=", 85),
                    new Token(TokenType.INTEGER, "49", 88),
                    new Token(TokenType.SORT, "sort", 91),
                    new Token(TokenType.TEXT, "discount_percentage", 96),
                    new Token(TokenType.SORT_DIRECTION, "desc", 118),
                    new Token(TokenType.SELECT, "select", 124),
                    new Token(TokenType.TAKE, "take", 131),
                    new Token(TokenType.INTEGER, "100", 136),
                    new Token(TokenType.FILTER, "filter", 140),
                    new Token(TokenType.TEXT, "store_name", 147),
                    new Token(TokenType.COMPARISON, "<>", 160),
                    new Token(TokenType.TEXT, "gog", 163),
                    new Token(TokenType.FILTER, "filter", 169),
                    new Token(TokenType.TEXT, "discount_percentage", 176),
                    new Token(TokenType.COMPARISON, ">=", 198),
                    new Token(TokenType.INTEGER, "49", 201),
                    new Token(TokenType.SORT, "sort", 204),
                    new Token(TokenType.TEXT, "discount_percentage", 209),
                    new Token(TokenType.SORT_DIRECTION, "desc", 231),
                    new Token(TokenType.SELECT, "select", 237),
                    new Token(TokenType.TAKE, "take", 244),
                    new Token(TokenType.INTEGER, "100", 249),
                    new Token(TokenType.FILTER, "filter", 253),
                    new Token(TokenType.TEXT, "store_name", 260),
                    new Token(TokenType.COMPARISON, "<>", 273),
                    new Token(TokenType.TEXT, "playstationstore", 276),
                    new Token(TokenType.FILTER, "filter", 295),
                    new Token(TokenType.TEXT, "discount_percentage", 302),
                    new Token(TokenType.COMPARISON, ">=", 324),
                    new Token(TokenType.INTEGER, "49", 327),
                    new Token(TokenType.SORT, "sort", 330),
                    new Token(TokenType.TEXT, "discount_percentage", 335),
                    new Token(TokenType.SORT_DIRECTION, "desc", 357),
                    new Token(TokenType.EOF, "", 361),
                },
                Expected = new Combine
                {
                    Selects = new List<Select>
                    {
                        new Select
                        {
                            Take = new Take(new Integer(100)),
                            From = new From(new Text(Categories.GAMES)),
                            Filters = new List<Filter>
                            {
                                new Filter(new Text(Properties.STORE_NAME), new Comparison(Comparisons.CONTAIN), new Text("steam")),
                                new Filter(new Text(Properties.DISCOUNT_PERCENTAGE), new Comparison(Comparisons.GREATER_OR_EQUAL), new Integer(49)),
                            },
                            Sorts = new List<Sort>
                            {
                                new Sort(new Text(Properties.DISCOUNT_PERCENTAGE), new SortDirection(Keywords.DESC)),
                            },
                        },
                        new Select
                        {
                            Take = new Take(new Integer(100)),
                            From = new From(new Text(Categories.GAMES)),
                            Filters = new List<Filter>
                            {
                                new Filter(new Text(Properties.STORE_NAME), new Comparison(Comparisons.CONTAIN), new Text("gog")),
                                new Filter(new Text(Properties.DISCOUNT_PERCENTAGE), new Comparison(Comparisons.GREATER_OR_EQUAL), new Integer(49)),
                            },
                            Sorts = new List<Sort>
                            {
                                new Sort(new Text(Properties.DISCOUNT_PERCENTAGE), new SortDirection(Keywords.DESC)),
                            },
                        },
                        new Select
                        {
                            Take = new Take(new Integer(100)),
                            From = new From(new Text(Categories.GAMES)),
                            Filters = new List<Filter>
                            {
                                new Filter(new Text(Properties.STORE_NAME), new Comparison(Comparisons.CONTAIN), new Text("playstationstore")),
                                new Filter(new Text(Properties.DISCOUNT_PERCENTAGE), new Comparison(Comparisons.GREATER_OR_EQUAL), new Integer(49)),
                            },
                            Sorts = new List<Sort>
                            {
                                new Sort(new Text(Properties.DISCOUNT_PERCENTAGE), new SortDirection(Keywords.DESC)),
                            },
                        },
                    }
                },
            }
        };

        public class ParseValidTestCase
        {
            public IEnumerable<Token> Tokens { get; init; }
            public Expression Expected { get; init; }
        }

        [TestCaseSource(nameof(_parseInvalidTestCases))]
        public void Parse_ParsesInvalidInput_ThrowsException<TException>(ParseInvalidTestCase<TException> test)
            where TException : Exception
        {
            void act() => _parser.Parse(test.Tokens);

            var exception = Assert.Throws<TException>(act);
            Assert.AreEqual(test.Exception.GetType(), exception.GetType());
            Assert.AreEqual(test.Exception.Message, exception.Message);
        }

        private static readonly IEnumerable<object> _parseInvalidTestCases =
            new IEnumerable<object>[]
            {
                GetExpectedRootTokenFailureTestCases(),
                GetSelectExpectedTokenFailureTestCases(),
                GetLoadExpectedTokenFailureTestCases(),
                GetCombineExpectedTokenFailureTestCases(),
                GetValueFailureManualTestCases(),
                GetValueFailureGeneratedTestCases(),
                GetUnrecognisedTokenExceptionTestCases(),
                new List<object>
                {
                    new ParseInvalidTestCase<ArgumentNullException>
                    {
                        Tokens = null,
                        Exception = new ArgumentNullException("source"),
                    },
                },
            }.SelectMany(x => x);

        private static IEnumerable<ParseInvalidTestCase<UnexpectedTokenException>> GetExpectedRootTokenFailureTestCases()
        {
            var unrecognisedTokenType = new TokenType[] { TokenType.UNRECOGNISED };
            return Enum
                .GetValues(typeof(TokenType))
                .Cast<TokenType>()
                .Except(Parser.ROOT_TOKEN_TYPES)
                .Except(unrecognisedTokenType)
                .Select(type =>
                {
                    var token = new Token(type, type.ToString(), 0);
                    return new ParseInvalidTestCase<UnexpectedTokenException>
                    {
                        Tokens = new Token[] { token },
                        Exception = new UnexpectedTokenException(token, Parser.ROOT_TOKEN_TYPES),
                    };
                })
            .ToList();
        }

        private static IEnumerable<ParseInvalidTestCase<UnexpectedTokenException>> GetSelectExpectedTokenFailureTestCases()
        {
            var unrecognisedTokenType = new TokenType[] { TokenType.UNRECOGNISED };
            return Enum
                .GetValues(typeof(TokenType))
                .Cast<TokenType>()
                .Except(Parser.SELECT_EXPECTED_TOKEN_TYPES)
                .Except(unrecognisedTokenType)
                .Select(type =>
                {
                    var select = new Token(TokenType.SELECT, Keywords.SELECT, 0);
                    var token = new Token(type, type.ToString(), 7);
                    return new ParseInvalidTestCase<UnexpectedTokenException>
                    {
                        Tokens = new Token[] { select, token },
                        Exception = new UnexpectedTokenException(token, Parser.SELECT_EXPECTED_TOKEN_TYPES),
                    };
                })
            .ToList();
        }

        private static IEnumerable<ParseInvalidTestCase<UnexpectedTokenException>> GetLoadExpectedTokenFailureTestCases()
        {
            var textTokenType = new TokenType[] { TokenType.TEXT };
            var unrecognisedTokenType = new TokenType[] { TokenType.UNRECOGNISED };
            return Enum
                .GetValues(typeof(TokenType))
                .Cast<TokenType>()
                .Except(textTokenType)
                .Except(unrecognisedTokenType)
                .Select(type =>
                {
                    var load = new Token(TokenType.LOAD, Keywords.LOAD, 0);
                    var token = new Token(type, type.ToString(), 5);
                    return new ParseInvalidTestCase<UnexpectedTokenException>
                    {
                        Tokens = new Token[] { load, token },
                        Exception = new UnexpectedTokenException(token, textTokenType),
                    };
                })
            .ToList();
        }

        private static IEnumerable<ParseInvalidTestCase<UnexpectedTokenException>> GetCombineExpectedTokenFailureTestCases()
        {
            var expectedTokenTypes = new TokenType[] { TokenType.SELECT, TokenType.EOF };
            var unrecognisedTokenType = new TokenType[] { TokenType.UNRECOGNISED };
            return Enum
                .GetValues(typeof(TokenType))
                .Cast<TokenType>()
                .Except(expectedTokenTypes)
                .Except(unrecognisedTokenType)
                .Select(type =>
                {
                    var combine = new Token(TokenType.COMBINE, Keywords.COMBINE, 0);
                    var token = new Token(type, type.ToString(), 9);
                    return new ParseInvalidTestCase<UnexpectedTokenException>
                    {
                        Tokens = new Token[] { combine, token },
                        Exception = new UnexpectedTokenException(token, expectedTokenTypes),
                    };
                })
            .ToList();
        }

        private static IEnumerable<ParseInvalidTestCase<UnexpectedValueException>> GetValueFailureManualTestCases()
        {
            return new ParseInvalidTestCase<UnexpectedValueException>[]
            {
                new ParseInvalidTestCase<UnexpectedValueException>
                {
                    Tokens = new Token[]
                    {
                        new Token(TokenType.SELECT, Keywords.SELECT, 0),
                        new Token(TokenType.FROM, Keywords.FROM, 7),
                        new Token(TokenType.TEXT, "invalid", 12),
                    },
                    Exception = new UnexpectedValueException(
                        new Token(TokenType.TEXT, "invalid", 12),
                        Categories.ALL),
                },
                new ParseInvalidTestCase<UnexpectedValueException>
                {
                    Tokens = new Token[]
                    {
                        new Token(TokenType.SELECT, Keywords.SELECT, 0),
                        new Token(TokenType.FILTER, Keywords.FILTER, 7),
                        new Token(TokenType.TEXT, "invalid", 14),
                    },
                    Exception = new UnexpectedValueException(
                        new Token(TokenType.TEXT, "invalid", 14),
                        Properties.ALL),
                },
                new ParseInvalidTestCase<UnexpectedValueException>
                {
                    Tokens = new Token[]
                    {
                        new Token(TokenType.SELECT, Keywords.SELECT, 0),
                        new Token(TokenType.SORT, Keywords.SORT, 7),
                        new Token(TokenType.TEXT, "invalid", 12),
                    },
                    Exception = new UnexpectedValueException(
                        new Token(TokenType.TEXT, "invalid", 12),
                        Properties.ALL),
                },
            };
        }

        private static IEnumerable<ParseInvalidTestCase<UnexpectedValueException>> GetValueFailureGeneratedTestCases()
        {
            var tests = new List<ParseInvalidTestCase<UnexpectedValueException>>();
            var cases = new (IEnumerable<string> Properties, IEnumerable<string> Comparisons)[]
            {
                (Properties.TEXT_PROPERTIES, Comparisons.TEXT_COMPARISONS),
                (Properties.NUMERIC_PROPERTIES, Comparisons.NUMERIC_COMPARISONS),
            };
            foreach (var @case in cases)
                foreach (var property in @case.Properties)
                    foreach (var comparison in Comparisons.ALL.Except(@case.Comparisons))
                    {
                        var test = new ParseInvalidTestCase<UnexpectedValueException>
                        {
                            Tokens = new Token[]
                            {
                                new Token(TokenType.SELECT, Keywords.SELECT, 0),
                                new Token(TokenType.FILTER, Keywords.FILTER, 7),
                                new Token(TokenType.TEXT, property, 14),
                                new Token(TokenType.COMPARISON, comparison, 15 + property.Length),
                            },
                            Exception = new UnexpectedValueException(
                                new Token(TokenType.COMPARISON, comparison, 15 + property.Length),
                                @case.Comparisons),
                        };
                        tests.Add(test);
                    }
            return tests;
        }

        private static IEnumerable<ParseInvalidTestCase<AggregateException>> GetUnrecognisedTokenExceptionTestCases()
        {
            var token = new Token(TokenType.UNRECOGNISED, "", 0);
            var tests = new List<ParseInvalidTestCase<AggregateException>>
            {
                new ParseInvalidTestCase<AggregateException>
                {
                    Tokens = new Token[] { token },
                    Exception = new AggregateException(
                        new UnrecognisedTokenException(token.Value, token.Position),
                        new UnexpectedTokenException(token, Parser.ROOT_TOKEN_TYPES)),
                }
            };
            return tests;
        }

        public class ParseInvalidTestCase<TException>
            where TException : Exception
        {
            public IEnumerable<Token> Tokens { get; init; }
            public TException Exception { get; init; }
        }
    }
}
