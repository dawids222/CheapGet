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
            Assert.AreEqual(test.Exception.Message, exception.Message);
        }

        private static readonly IEnumerable<object> _parseInvalidTestCases =
            new IEnumerable<object>[]
            {
                GetExpectedRootTokenFailureTestCases(),
                GetSelectExpectedTokenFailureTestCases(),
                GetLoadExpectedTokenFailureTestCases(),
                GetValueFailureManualTestCases(),
                GetValueFailureGeneratedTestCases(),
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
            return Enum
                .GetValues(typeof(TokenType))
                .Cast<TokenType>()
                .Except(Parser.ROOT_TOKEN_TYPES)
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
            return Enum
                .GetValues(typeof(TokenType))
                .Cast<TokenType>()
                .Except(Parser.SELECT_EXPECTED_TOKEN_TYPES)
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
            return Enum
                .GetValues(typeof(TokenType))
                .Cast<TokenType>()
                .Except(textTokenType)
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

        public class ParseInvalidTestCase<TException>
            where TException : Exception
        {
            public IEnumerable<Token> Tokens { get; init; }
            public TException Exception { get; init; }
        }
    }
}
