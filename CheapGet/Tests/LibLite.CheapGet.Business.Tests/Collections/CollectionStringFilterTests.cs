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
        }

        private static readonly CollectionStringFilterTest[] _CollectionStringFilterTests = new[]
        {
            new CollectionStringFilterTest
            {
                Filter = new CollectionStringFilter<string>(x => x, StringRelationalOperator.EQUAL, "test0"),
                Func = x => x == "test0",
            },
            new CollectionStringFilterTest
            {
                Filter = new CollectionStringFilter<string>(x => x, StringRelationalOperator.CONTAIN,"test"),
                Func = x => x.Contains("test"),
            },
        };

        public class CollectionStringFilterTest
        {
            public CollectionStringFilter<string> Filter { get; init; }
            public Func<string, bool> Func { get; init; }
        }
    }
}
