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

        [TestCaseSource(nameof(_parseSuccessTestCases))]
        public void Parse_Success_ReturnsExpectedExpression(ParseSucessTestCase test)
        {
            var result = _parser.Parse(test.Tokens);

            Assert.AreEqual(test.Expected, result);
        }

        private static readonly IEnumerable<ParseSucessTestCase> _parseSuccessTestCases = new List<ParseSucessTestCase>
        {
            new ParseSucessTestCase
            {
                Tokens = new List<Token>
                {
                    new Token(TokenType.SELECT, "select", 0),
                    new Token(TokenType.EOF, "", 6),
                },
                Expected = new Select(),
            },
            new ParseSucessTestCase
            {
                Tokens = new List<Token>
                {
                    new Token(TokenType.CLS, "cls", 0),
                    new Token(TokenType.EOF, "", 3),
                },
                Expected = new Cls(),
            },
            new ParseSucessTestCase
            {
                Tokens = new List<Token>
                {
                    new Token(TokenType.EXIT, "exit", 0),
                    new Token(TokenType.EOF, "", 4),
                },
                Expected = new Exit(),
            },
            new ParseSucessTestCase
            {
                Tokens = new List<Token>
                {
                    new Token(TokenType.SELECT, "select", 0),
                    new Token(TokenType.FROM, "from", 7),
                    new Token(TokenType.TEXT, "Text", 12),
                    new Token(TokenType.FILTER, "filter", 17),
                    new Token(TokenType.TEXT, "base_price", 24),
                    new Token(TokenType.COMPARISON, ">=", 35),
                    new Token(TokenType.FLOATING, "49,99", 38),
                    new Token(TokenType.FILTER, "filter", 44),
                    new Token(TokenType.TEXT, "discounted_price", 51),
                    new Token(TokenType.COMPARISON, "<=", 68),
                    new Token(TokenType.INTEGER, "100", 71),
                    new Token(TokenType.SORT, "sort", 75),
                    new Token(TokenType.TEXT, "name", 80),
                    new Token(TokenType.SORT_DIRECTION, "desc", 85),
                    new Token(TokenType.SORT, "sort", 90),
                    new Token(TokenType.TEXT, "store_name", 95),
                    new Token(TokenType.SORT_DIRECTION, "asc", 105),
                    new Token(TokenType.TAKE, "take", 109),
                    new Token(TokenType.INTEGER, "50", 114),
                    new Token(TokenType.EOF, "", 115),
                },
                Expected = new Select()
                {
                    From = new From(new Text("Text")),
                    Take = new Take(new Integer(50)),
                    Filters = new List<Filter>
                    {
                        new Filter(new Text("base_price"), new Comparison(">="), new Floating(49.99)),
                        new Filter(new Text("discounted_price"), new Comparison("<="), new Integer(100)),
                    },
                    Sorts = new List<Sort>
                    {
                        new Sort(new Text("name"), new SortDirection("desc")),
                        new Sort(new Text("store_name"), new SortDirection("asc")),
                    },
                },
            },
            new ParseSucessTestCase
            {
                Tokens = new List<Token>
                {
                    new Token(TokenType.SELECT, "select", 0),
                    new Token(TokenType.SORT, "sort", 7),
                    new Token(TokenType.TEXT, "name", 12),
                    new Token(TokenType.SORT_DIRECTION, "desc", 17),
                    new Token(TokenType.TAKE, "take", 22),
                    new Token(TokenType.INTEGER, "50", 27),
                    new Token(TokenType.FILTER, "filter", 30),
                    new Token(TokenType.TEXT, "base_price", 37),
                    new Token(TokenType.COMPARISON, ">=", 48),
                    new Token(TokenType.FLOATING, "49,99", 51),
                    new Token(TokenType.FROM, "from", 57),
                    new Token(TokenType.TEXT, "Text", 62),
                    new Token(TokenType.SORT, "sort", 67),
                    new Token(TokenType.TEXT, "store_name", 72),
                    new Token(TokenType.SORT_DIRECTION, "asc", 83),
                    new Token(TokenType.FILTER, "filter", 87),
                    new Token(TokenType.TEXT, "discounted_price", 94),
                    new Token(TokenType.COMPARISON, "<=", 111),
                    new Token(TokenType.INTEGER, "100", 114),
                    new Token(TokenType.EOF, "", 115),
                },
                Expected = new Select()
                {
                    From = new From(new Text("Text")),
                    Take = new Take(new Integer(50)),
                    Filters = new List<Filter>
                    {
                        new Filter(new Text("base_price"), new Comparison(">="), new Floating(49.99)),
                        new Filter(new Text("discounted_price"), new Comparison("<="), new Integer(100)),
                    },
                    Sorts = new List<Sort>
                    {
                        new Sort(new Text("name"), new SortDirection("desc")),
                        new Sort(new Text("store_name"), new SortDirection("asc")),
                    },
                },
            },
        };

        public class ParseSucessTestCase
        {
            public IEnumerable<Token> Tokens { get; init; }
            public Expression Expected { get; init; }
        }

        [TestCaseSource(nameof(_parseFailureTestCases))]
        public void Parse_Failure_ThrowsException<TException>(ParseFailureTestCase<TException> test)
            where TException : Exception
        {
            void act() => _parser.Parse(test.Tokens);

            Assert.Throws<TException>(act, test.Exception.Message);
        }

        private static readonly IEnumerable<object> _parseFailureTestCases =
            new IEnumerable<object>[]
            {
                GetExpectedRootTokenFailureTestCases(),
                GetSelectExpectedTokenFailureTestCases(),
                new List<object>
                {
                    new ParseFailureTestCase<ArgumentNullException>
                    {
                        Tokens = null,
                        Exception = new ArgumentNullException(),
                    },
                },
            }.SelectMany(x => x);

        private static IEnumerable<ParseFailureTestCase<UnexpectedTokenException>> GetExpectedRootTokenFailureTestCases()
        {
            return Enum
                .GetValues(typeof(TokenType))
                .Cast<TokenType>()
                .Except(Parser.ROOT_TOKEN_TYPES)
                .Select(type =>
                {
                    var token = new Token(type, type.ToString(), 0);
                    return new ParseFailureTestCase<UnexpectedTokenException>
                    {
                        Tokens = new Token[] { token },
                        Exception = new UnexpectedTokenException(token, Parser.ROOT_TOKEN_TYPES),
                    };
                })
            .ToList();
        }

        private static IEnumerable<ParseFailureTestCase<UnexpectedTokenException>> GetSelectExpectedTokenFailureTestCases()
        {
            return Enum
                .GetValues(typeof(TokenType))
                .Cast<TokenType>()
                .Except(Parser.SELECT_EXPECTED_TOKEN_TYPES)
                .Select(type =>
                {
                    var select = new Token(TokenType.SELECT, "select", 0);
                    var token = new Token(type, type.ToString(), 7);
                    return new ParseFailureTestCase<UnexpectedTokenException>
                    {
                        Tokens = new Token[] { select, token },
                        Exception = new UnexpectedTokenException(token, Parser.SELECT_EXPECTED_TOKEN_TYPES),
                    };
                })
            .ToList();
        }

        public class ParseFailureTestCase<TException>
            where TException : Exception
        {
            public IEnumerable<Token> Tokens { get; init; }
            public TException Exception { get; init; }
        }
    }
}
