using LibLite.CheapGet.Business.Services.DSL;
using NUnit.Framework;
using System.Collections.Generic;

namespace LibLite.CheapGet.Business.Tests.Services.DSL
{
    // TODO: Test that _position resets every time
    [TestFixture]
    public class LexerTests
    {
        private Lexer _lexer;

        [SetUp]
        public void SetUp()
        {
            _lexer = new Lexer();
        }

        [TestCaseSource(nameof(_lexTestCases))]
        public void Lex_LexesInput_ReturnsExpectedTokenCollection(LexTestCase test)
        {
            var result = _lexer.Lex(test.Input);

            CollectionAssert.AreEqual(test.Expected, result);
        }

        private static IEnumerable<LexTestCase> _lexTestCases = new List<LexTestCase>
        {
            new LexTestCase
            {
                Input = "select run",
                Expected = new List<Token>
                {
                    new Token(TokenType.SELECT, "select", 0),
                    new Token(TokenType.TERMINATOR, "run", 7),
                    new Token(TokenType.EOF, "", 10),
                },
            },

            new LexTestCase
            {
                Input = @"select from ""Games"" filter ""base_price"" >= 49,99 sort ""name"" desc take 50 run",
                Expected = new List<Token>
                {
                    new Token(TokenType.SELECT, "select", 0),
                    new Token(TokenType.FROM, "from", 7),
                    new Token(TokenType.TEXT, "Games", 12),
                    new Token(TokenType.FILTER, "filter", 20),
                    new Token(TokenType.TEXT, "base_price", 27),
                    new Token(TokenType.COMPARISON, ">=", 40),
                    new Token(TokenType.DECIMAL, "49,99", 43),
                    new Token(TokenType.SORT, "sort", 49),
                    new Token(TokenType.TEXT, "name", 54),
                    new Token(TokenType.SORT_DIRECTION, "desc", 61),
                    new Token(TokenType.TAKE, "take", 66),
                    new Token(TokenType.INTEGER, "50", 71),
                    new Token(TokenType.TERMINATOR, "run", 74),
                    new Token(TokenType.EOF, "", 77),
                },
            },
        };

        public class LexTestCase
        {
            public string Input { get; init; }
            public IEnumerable<Token> Expected { get; init; }
        }
    }
}
