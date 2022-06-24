using LibLite.CheapGet.Business.Collections;
using LibLite.CheapGet.Core.Enums;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibLite.CheapGet.Business.Tests.Collections
{
    [TestFixture]
    public class CollectionDoubleFilterTests
    {
        private IEnumerable<double> _values;

        [SetUp]
        public void SetUp()
        {
            _values = new double[] { -4.5, 3.14, 6.734, -7.231, 23.4575, 3.14, 0, 1, 1.1, 2.8367, -876.45 };
        }

        [TestCaseSource(nameof(_collectionDoubleFilterTests))]
        public void Apply_ReturnsFilteredCollection(CollectionDoubleFilterTest test)
        {
            var expected = _values.Where(test.Func);

            var result = test.Filter.Apply(_values);

            CollectionAssert.AreEqual(expected, result);
        }

        private static readonly CollectionDoubleFilterTest[] _collectionDoubleFilterTests = new[]
        {
            new CollectionDoubleFilterTest
            {
                Filter = new CollectionDoubleFilter<double>(x => x, NumberRelationalOperator.GREATER, 3.14),
                Func = x => x > 3.14,
            },
            new CollectionDoubleFilterTest
            {
                Filter = new CollectionDoubleFilter<double>(x => x, NumberRelationalOperator.GREATER_OR_EQUAL, 3.14),
                Func = x => x >= 3.14,
            },
            new CollectionDoubleFilterTest
            {
                Filter = new CollectionDoubleFilter<double>(x => x, NumberRelationalOperator.EQUAL, 3.14),
                Func = x => x == 3.14,
            },
            new CollectionDoubleFilterTest
            {
                Filter = new CollectionDoubleFilter<double>(x => x, NumberRelationalOperator.NOT_EQUAL, 3.14),
                Func = x => x != 3.14,
            },
            new CollectionDoubleFilterTest
            {
                Filter = new CollectionDoubleFilter<double>(x => x, NumberRelationalOperator.LESS_OR_EQUAL, 3.14),
                Func = x => x <= 3.14,
            },
            new CollectionDoubleFilterTest
            {
                Filter = new CollectionDoubleFilter<double>(x => x, NumberRelationalOperator.LESS, 3.14),
                Func = x => x < 3.14,
            },
        };

        public class CollectionDoubleFilterTest
        {
            public CollectionDoubleFilter<double> Filter { get; init; }
            public Func<double, bool> Func { get; init; }
        }
    }
}
