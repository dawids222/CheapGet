using LibLite.CheapGet.Business.Exceptions.CGQL;
using LibLite.CheapGet.Business.Services.CGQL;
using LibLite.CheapGet.Core.CGQL.Enums;
using LibLite.CheapGet.Core.CGQL.Models;
using NUnit.Framework;
using System.Collections.Generic;

namespace LibLite.CheapGet.Business.Tests.Services.CGQL
{
    [TestFixture]
    public class LexerTests
    {
        private Lexer _lexer;

        [SetUp]
        public void SetUp()
        {
            _lexer = new Lexer();
        }

        [TestCaseSource(nameof(_lexValidTestCases))]
        public void Lex_LexesValidInput_ReturnsExpectedTokenCollection(LexValidTestCase test)
        {
            var result = _lexer.Lex(test.Input);

            CollectionAssert.AreEqual(test.Expected, result);
        }

        private static readonly IEnumerable<LexValidTestCase> _lexValidTestCases = new List<LexValidTestCase>
        {
            new LexValidTestCase
            {
                Input = "select",
                Expected = new List<Token>
                {
                    new Token(TokenType.SELECT, "select", 0),
                    new Token(TokenType.EOF, "", 6),
                },
            },
            new LexValidTestCase
            {
                Input = @"select from ""Games"" filter ""base_price"" >= 49,99 sort ""name"" desc take 50",
                Expected = new List<Token>
                {
                    new Token(TokenType.SELECT, "select", 0),
                    new Token(TokenType.FROM, "from", 7),
                    new Token(TokenType.TEXT, "Games", 12),
                    new Token(TokenType.FILTER, "filter", 20),
                    new Token(TokenType.TEXT, "base_price", 27),
                    new Token(TokenType.COMPARISON, ">=", 40),
                    new Token(TokenType.FLOATING, "49,99", 43),
                    new Token(TokenType.SORT, "sort", 49),
                    new Token(TokenType.TEXT, "name", 54),
                    new Token(TokenType.SORT_DIRECTION, "desc", 61),
                    new Token(TokenType.TAKE, "take", 66),
                    new Token(TokenType.INTEGER, "50", 71),
                    new Token(TokenType.EOF, "", 73),
                },
            },
            new LexValidTestCase
            {
                Input = "cls",
                Expected = new List<Token>
                {
                    new Token(TokenType.CLS, "cls", 0),
                    new Token(TokenType.EOF, "", 3),
                },
            },
            new LexValidTestCase
            {
                Input = "exit",
                Expected = new List<Token>
                {
                    new Token(TokenType.EXIT, "exit", 0),
                    new Token(TokenType.EOF, "", 4),
                },
            },
            new LexValidTestCase
            {
                Input = "load \"query.cgql\"",
                Expected = new List<Token>
                {
                    new Token(TokenType.LOAD, "load", 0),
                    new Token(TokenType.TEXT, "query.cgql", 5),
                    new Token(TokenType.EOF, "", 17),
                },
            },
            new LexValidTestCase
            {
                Input = @"select from filter >= > = != <> < <= sort asc desc take ""text"" 1 1.1 2,2 cls exit",
                Expected = new List<Token>
                {
                    new Token(TokenType.SELECT, "select", 0),
                    new Token(TokenType.FROM, "from", 7),
                    new Token(TokenType.FILTER, "filter", 12),
                    new Token(TokenType.COMPARISON, ">=", 19),
                    new Token(TokenType.COMPARISON, ">", 22),
                    new Token(TokenType.COMPARISON, "=", 24),
                    new Token(TokenType.COMPARISON, "!=", 26),
                    new Token(TokenType.COMPARISON, "<>", 29),
                    new Token(TokenType.COMPARISON, "<", 32),
                    new Token(TokenType.COMPARISON, "<=", 34),
                    new Token(TokenType.SORT, "sort", 37),
                    new Token(TokenType.SORT_DIRECTION, "asc", 42),
                    new Token(TokenType.SORT_DIRECTION, "desc", 46),
                    new Token(TokenType.TAKE, "take", 51),
                    new Token(TokenType.TEXT, "text", 56),
                    new Token(TokenType.INTEGER, "1", 63),
                    new Token(TokenType.FLOATING, "1,1", 65),
                    new Token(TokenType.FLOATING, "2,2", 69),
                    new Token(TokenType.CLS, "cls", 73),
                    new Token(TokenType.EXIT, "exit", 77),
                    new Token(TokenType.EOF, "", 81),
                },
            },
            new LexValidTestCase
            {
                Input = @"sELEct FROM FilteR >= > = != <> < <= sOrT aSc DeSc TAke ""tExT"" 1 1.1 2,2 cLS ExIt",
                Expected = new List<Token>
                {
                    new Token(TokenType.SELECT, "sELEct", 0),
                    new Token(TokenType.FROM, "FROM", 7),
                    new Token(TokenType.FILTER, "FilteR", 12),
                    new Token(TokenType.COMPARISON, ">=", 19),
                    new Token(TokenType.COMPARISON, ">", 22),
                    new Token(TokenType.COMPARISON, "=", 24),
                    new Token(TokenType.COMPARISON, "!=", 26),
                    new Token(TokenType.COMPARISON, "<>", 29),
                    new Token(TokenType.COMPARISON, "<", 32),
                    new Token(TokenType.COMPARISON, "<=", 34),
                    new Token(TokenType.SORT, "sOrT", 37),
                    new Token(TokenType.SORT_DIRECTION, "aSc", 42),
                    new Token(TokenType.SORT_DIRECTION, "DeSc", 46),
                    new Token(TokenType.TAKE, "TAke", 51),
                    new Token(TokenType.TEXT, "tExT", 56),
                    new Token(TokenType.INTEGER, "1", 63),
                    new Token(TokenType.FLOATING, "1,1", 65),
                    new Token(TokenType.FLOATING, "2,2", 69),
                    new Token(TokenType.CLS, "cLS", 73),
                    new Token(TokenType.EXIT, "ExIt", 77),
                    new Token(TokenType.EOF, "", 81),
                },
            },
            new LexValidTestCase
            {
                Input = @"""text with   whitespaces""",
                Expected = new List<Token>
                {
                    new Token(TokenType.TEXT, "text with   whitespaces", 0),
                    new Token(TokenType.EOF, "", 25),
                },
            },
            new LexValidTestCase
            {
                Input = @"select sort  ""name""    asc",
                Expected = new List<Token>
                {
                    new Token(TokenType.SELECT, "select", 0),
                    new Token(TokenType.SORT, "sort", 7),
                    new Token(TokenType.TEXT, "name", 13),
                    new Token(TokenType.SORT_DIRECTION, "asc", 23),
                    new Token(TokenType.EOF, "", 26),
                },
            },
        };

        public class LexValidTestCase
        {
            public string Input { get; init; }
            public IEnumerable<Token> Expected { get; init; }
        }

        [TestCaseSource(nameof(_lexInvalidTestCases))]
        public void Lex_LexesInvalidInput_ThrowsInvalidTokenException(LexInvalidTestCase test)
        {
            void act() => _lexer.Lex(test.Input);

            var exception = Assert.Throws<UnrecognisedTokenException>(act, test.Exception.Message);
            Assert.AreEqual(test.Exception.Token, exception.Token);
            Assert.AreEqual(test.Exception.Position, exception.Position);
        }

        private static readonly IEnumerable<LexInvalidTestCase> _lexInvalidTestCases = new List<LexInvalidTestCase>
        {
            new LexInvalidTestCase
            {
                Input = "invalid",
                Exception= new("invalid", 0),
            },
            new LexInvalidTestCase
            {
                Input = "'text'",
                Exception= new("'text'", 0),
            },
            new LexInvalidTestCase
            {
                Input = @"""""text""",
                Exception= new(@"""""text""", 0),
            },
        };

        public class LexInvalidTestCase
        {
            public string Input { get; init; }
            public UnrecognisedTokenException Exception { get; init; }
        }
    }
}
