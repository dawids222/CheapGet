using LibLite.CheapGet.Business.Services.DSL;
using NUnit.Framework;
using System.Collections.Generic;

namespace LibLite.CheapGet.Business.Tests.Services.DSL
{
    // TODO: Write tests!!!
    [TestFixture]
    public class ParserTests
    {
        private Parser _parser;

        [SetUp]
        public void SetUp()
        {
            _parser = new();
        }

        [TestCaseSource(nameof(_parseTestCases))]
        public void Parse_ParsesInput_ReturnsExpectedExpression(ParseTestCase test)
        {
            var result = _parser.Parse(test.Tokens);

            Assert.AreEqual(test.Expected, result);
        }

        private static IEnumerable<ParseTestCase> _parseTestCases = new List<ParseTestCase>
        {
            new ParseTestCase
            {
                Tokens = new List<Token>
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
                    new Token(TokenType.EOF, "", 73),
                },
                Expected = new Select(),
            },
        };

        public class ParseTestCase
        {
            public IEnumerable<Token> Tokens { get; init; }
            public Expression Expected { get; init; }
        }
    }
}
