using LibLite.CheapGet.Core.Collections;
using LibLite.CheapGet.Core.Tests.Collections.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibLite.CheapGet.Core.Tests.Collections
{
    [TestFixture]
    public class CollectionFilterTests
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

        [TestCaseSource(nameof(_predicateTestCases))]
        public void Apply_AnyPredicate_ReturnsFilteredCollection(Func<CollectionOperationModel, bool> predicate)
        {
            var sort = new CollectionFilter<CollectionOperationModel>(predicate);
            var expected = _values.Where(predicate);

            var result = sort.Apply(_values);

            CollectionAssert.AreEqual(expected, result);
        }

        private static readonly Func<CollectionOperationModel, bool>[] _predicateTestCases = new[]
        {
            (CollectionOperationModel x) => x.DoubleValue > 1.1,
            (CollectionOperationModel x) => x.DoubleValue >= 1.1,
            (CollectionOperationModel x) => x.DoubleValue == 1.1,
            (CollectionOperationModel x) => x.DoubleValue != 1.1,
            (CollectionOperationModel x) => x.DoubleValue <= 1.1,
            (CollectionOperationModel x) => x.DoubleValue < 1.1,
            (CollectionOperationModel x) => x.StringValue.Contains("value1"),
            (CollectionOperationModel x) => x.StringValue == "value2.01",
            (CollectionOperationModel x) => x.StringValue.Length == 8,
        };

        [TestCaseSource(nameof(_complexFilterTestCases))]
        public void Apply_ComplexFilters_ReturnsFilteredCollection(ComplexFilterTestCase test)
        {
            var expected = _values.Where(test.Func);

            var result = test.Filter.Apply(_values);

            CollectionAssert.AreEqual(expected, result);
        }

        private static ComplexFilterTestCase[] _complexFilterTestCases = new[]
        {
            new ComplexFilterTestCase
            {
                Filter = new CollectionFilter<CollectionOperationModel>(x => x.StringValue == "value1.1").And(
                    new CollectionFilter<CollectionOperationModel>(x => x.DoubleValue < 1.11)),
                Func = (x) => x.StringValue == "value1.1" && x.DoubleValue < 1.11,
            },
            new ComplexFilterTestCase
            {
                Filter = new CollectionFilter<CollectionOperationModel>(x => x.DoubleValue > 2).Or(
                    new CollectionFilter<CollectionOperationModel>(x => x.DoubleValue < 1.11)),
                Func = (x) => x.DoubleValue > 2 || x.DoubleValue < 1.11,
            },
            new ComplexFilterTestCase
            {
                Filter = new CollectionFilter<CollectionOperationModel>(x => x.DoubleValue > 2).And(
                    new CollectionFilter<CollectionOperationModel>(x => x.StringValue.Length == 8)).Or(
                    new CollectionFilter<CollectionOperationModel>(x => x.DoubleValue < 1.11).And(
                        new CollectionFilter<CollectionOperationModel>(x => x.StringValue.Length == 6))),
                Func = (x) => (x.DoubleValue > 2 && x.StringValue.Length == 8) || (x.DoubleValue < 1.11 && x.StringValue.Length == 6),
            },
        };

        public class ComplexFilterTestCase
        {
            public ICollectionFilter<CollectionOperationModel> Filter { get; init; }
            public Func<CollectionOperationModel, bool> Func { get; init; }
        }
    }
}
