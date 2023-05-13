using LibLite.CheapGet.Business.Collections;
using LibLite.CheapGet.Core.Enums;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibLite.CheapGet.Business.Tests.Collections
{
    [TestFixture]
    public class CollectionStringFilterTests
    {
        private IEnumerable<string> _values;

        [SetUp]
        public void SetUp()
        {
            _values = new string[]
            {
                "test0",
                "test0",
                "test1",
                "test2",
                "tset0",
                "tset1",
                "tset2",
            };
        }

        [TestCaseSource(nameof(_CollectionStringFilterTests))]
        public void Apply_ReturnsFilteredCollection(CollectionStringFilterTest test)
        {
            var expected = _values.Where(test.Func);

            var result = test.Filter.Apply(_values);

            CollectionAssert.AreEqual(expected, result);
            Assert.That(test.Filter.Operator, Is.EqualTo(test.ExpectedOperator));
            Assert.That(test.Filter.Value, Is.EqualTo(test.ExpctedValue));
        }

        private static readonly CollectionStringFilterTest[] _CollectionStringFilterTests = new[]
        {
            new CollectionStringFilterTest
            {
                Filter = new CollectionStringFilter<string>(x => x, StringRelationalOperator.EQUAL, "test0"),
                Func = x => x == "test0",
                ExpectedOperator = StringRelationalOperator.EQUAL,
                ExpctedValue = "test0",
            },
            new CollectionStringFilterTest
            {
                Filter = new CollectionStringFilter<string>(x => x, StringRelationalOperator.CONTAIN,"test"),
                Func = x => x.Contains("test"),
                ExpectedOperator = StringRelationalOperator.CONTAIN,
                ExpctedValue = "test",
            },
            new CollectionStringFilterTest
            {
                Filter = new CollectionStringFilter<string>(x => x, StringRelationalOperator.CONTAIN,"TeSt"),
                Func = x => x.Contains("test", StringComparison.InvariantCultureIgnoreCase),
                ExpectedOperator = StringRelationalOperator.CONTAIN,
                ExpctedValue = "TeSt",
            },
        };

        public class CollectionStringFilterTest
        {
            public CollectionStringFilter<string> Filter { get; init; }
            public Func<string, bool> Func { get; init; }
            public StringRelationalOperator ExpectedOperator { get; init; }
            public string ExpctedValue { get; init; }
        }
    }
}
