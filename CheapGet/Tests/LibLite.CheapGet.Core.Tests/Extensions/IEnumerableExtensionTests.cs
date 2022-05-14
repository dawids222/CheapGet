using LibLite.CheapGet.Core.Extensions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibLite.CheapGet.Core.Tests.Extensions
{
    [TestFixture]
    public class IEnumerableExtensionTests
    {
        [Test]
        public void ForEach_IteratesOverEveryElement()
        {
            var value = 0;
            var items = (IEnumerable<int>)Array.CreateInstance(typeof(int), 10);

            items.ForEach(item => value++);

            Assert.AreEqual(items.Count(), value);
        }
    }
}
