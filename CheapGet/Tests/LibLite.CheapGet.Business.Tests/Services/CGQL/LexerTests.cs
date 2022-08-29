using LibLite.CheapGet.Business.Services.CGQL;
using LibLite.CheapGet.Core.CGQL.Enums;
using LibLite.CheapGet.Core.CGQL.Models;
using NUnit.Framework;
using System.Collections.Generic;

namespace LibLite.CheapGet.Business.Tests.Services.CGQL
{
    // TODO: Add unexpected token test
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

        private static readonly IEnumerable<LexTestCase> _lexTestCases = new List<LexTestCase>
        {
            new LexTestCase
            {
                Input = "select",
                Expected = new List<Token>
                {
                    new Token(TokenType.SELECT, "select", 0),
                    new Token(TokenType.EOF, "", 6),
                },
            },
            new LexTestCase
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
            new LexTestCase
            {
                Input = "cls",
                Expected = new List<Token>
                {
                    new Token(TokenType.CLS, "cls", 0),
                    new Token(TokenType.EOF, "", 3),
                },
            },
            new LexTestCase
            {
                Input = "exit",
                Expected = new List<Token>
                {
                    new Token(TokenType.EXIT, "exit", 0),
                    new Token(TokenType.EOF, "", 4),
                },
            },
            new LexTestCase
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
            new LexTestCase
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
            new LexTestCase
            {
                Input = @"""text with   whitespaces""",
                Expected = new List<Token>
                {
                    new Token(TokenType.TEXT, "text with   whitespaces", 0),
                    new Token(TokenType.EOF, "", 25),
                },
            },
            new LexTestCase
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

        public class LexTestCase
        {
            public string Input { get; init; }
            public IEnumerable<Token> Expected { get; init; }
        }
    }
}
