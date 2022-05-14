using LibLite.CheapGet.Core.Extensions;
using NUnit.Framework;
using System.Collections.Generic;

namespace LibLite.CheapGet.Core.Tests.Extensions
{
    [TestFixture]
    public class TypeExtensionsTests
    {
        [TestCaseSource(nameof(_isNumericTypeTestCases))]
        public void IsNumericType_ReturnsIfValueIsOfNumericType(IsNumericTypeTestCase testCase)
        {
            var result = testCase.Value.GetType().IsNumericType();

            Assert.AreEqual(testCase.IsNumericType, result);
        }

        private static readonly IEnumerable<IsNumericTypeTestCase> _isNumericTypeTestCases = new IsNumericTypeTestCase[]
        {
            new IsNumericTypeTestCase(new byte(), true),
            new IsNumericTypeTestCase(new sbyte(), true),
            new IsNumericTypeTestCase(new ushort(), true),
            new IsNumericTypeTestCase(new uint(), true),
            new IsNumericTypeTestCase(new ulong(), true),
            new IsNumericTypeTestCase(new short(), true),
            new IsNumericTypeTestCase(new int(), true),
            new IsNumericTypeTestCase(new long(), true),
            new IsNumericTypeTestCase(new decimal(), true),
            new IsNumericTypeTestCase(new double(), true),
            new IsNumericTypeTestCase(new float(), true),

            new IsNumericTypeTestCase(new char(), false),
            new IsNumericTypeTestCase(string.Empty, false),
            new IsNumericTypeTestCase(true, false),
            new IsNumericTypeTestCase(new TypeExtensionsTests(), false),
            new IsNumericTypeTestCase(new byte[] { 1,2,3,4 }, false),
        };

        public class IsNumericTypeTestCase
        {
            public object Value { get; }
            public bool IsNumericType { get; }

            public IsNumericTypeTestCase(object value, bool isNumericType)
            {
                Value = value;
                IsNumericType = isNumericType;
            }
        }
    }
}
