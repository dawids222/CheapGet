using LibLite.CheapGet.Core.Collections;
using LibLite.CheapGet.Core.Enums;
using LibLite.CheapGet.Core.Tests.Collections.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibLite.CheapGet.Core.Tests.Collections
{
    [TestFixture]
    public class CollectionSortTests
    {
        private IEnumerable<CollectionOperationModel> _values;

        [SetUp]
        public void SetUp()
        {
            _values = new List<CollectionOperationModel>()
            {
                new CollectionOperationModel("value2", 2),
                new CollectionOperationModel("value2.01", 2.01),
                new CollectionOperationModel("value1", 1),
                new CollectionOperationModel("value1.1", 1.1),
                new CollectionOperationModel("value1.1", 1.11),
                new CollectionOperationModel("value2.2", 2.2),
            };
        }

        [Test]
        public void Apply_SortByDouble_ReturnsCollectionSortedByDouble()
        {
            var sort = new CollectionSort<CollectionOperationModel, double>(x => x.DoubleValue);
            var expected = _values.OrderBy(x => x.DoubleValue);

            var result = sort.Apply(_values);

            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void Apply_SortByDoubleDesc_ReturnsCollectionSortedByDoubleDesc()
        {
            var sort = new CollectionSort<CollectionOperationModel, double>(x => x.DoubleValue, SortDirection.DESC);
            var expected = _values.OrderByDescending(x => x.DoubleValue);

            var result = sort.Apply(_values);

            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void Apply_SortByString_ReturnsCollectionSortedByString()
        {
            var sort = new CollectionSort<CollectionOperationModel, string>(x => x.StringValue);
            var expected = _values.OrderBy(x => x.StringValue);

            var result = sort.Apply(_values);

            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void Apply_SortByStringDesc_ReturnsCollectionSortedByStringDesc()
        {
            var sort = new CollectionSort<CollectionOperationModel, string>(x => x.StringValue, SortDirection.DESC);
            var expected = _values.OrderByDescending(x => x.StringValue);

            var result = sort.Apply(_values);

            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void Apply_SortByDoubleAscThenStringDesc_ReturnsCollectionSortedByDoubleAscThenStringDesc()
        {
            var sorts = new ICollectionSort<CollectionOperationModel>[] {
            new CollectionSort<CollectionOperationModel, double>(x => x.DoubleValue),
            new CollectionSort<CollectionOperationModel, string>(x => x.StringValue, SortDirection.DESC),
            };
            var expected = _values
                .OrderBy(x => x.DoubleValue)
                .ThenByDescending(x => x.StringValue);

            var result = sorts
                .Reverse()
                .Aggregate(
                _values,
                (collection, sort) => sort.Apply(collection));

            CollectionAssert.AreEqual(expected, result);
        }
    }
}
