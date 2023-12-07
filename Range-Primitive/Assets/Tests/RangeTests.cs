using System.Collections.Generic;
using NUnit.Framework;
using RangePrimitive;

namespace Tests
{
    public class RangeTests
    {
        private static IEnumerable<int> IntTestValues => new[] { 0, 1, 2, 3, 8, 50, 99 };
        
        [Test]
        public void Contains_ValueInsideOfPositiveRange_ReturnsTrue([ValueSource(nameof(IntTestValues))] int min, [ValueSource(nameof(IntTestValues))] int max)
        {
            Range<int> range = new Range<int>(min, max);
            int value = new Range<int>(min, max).Random();
            
            bool contains = range.Contains(value);
        
            Assert.IsTrue(contains, $"{value}");
        }
        
        [Test]
        [TestCase(0, 0, 1)]
        [TestCase(1, 5, 10)]
        [TestCase(10, 50, -1)]
        [TestCase(5, 1, 2)]
        public void Contains_ValueOutsideOfPositiveRange_ReturnsFalse(int min, int max, int valueOutsideRange)
        {
            Range<int> range = new Range<int>(min, max);
        
            bool contains = range.Contains(valueOutsideRange);
        
            Assert.IsFalse(contains);
        }
    }
}