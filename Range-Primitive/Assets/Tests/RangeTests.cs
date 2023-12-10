using System.Collections.Generic;
using NUnit.Framework;
using RangePrimitive;
using UnityEngine;

namespace Tests
{
    // ✔ Reorder
    // ✔ Random
    // ✔ Lerp
    // ✔ InverseLerp
    // ✔ SmoothStep
    // ✔ Delta
    // ✔ Spread
    // ✔ Clamp
    // ✔ IsContained
    // Center

    public class RangeTests
    {
        private static IEnumerable<int> TestValues => new[] { -150, -5, -3, -2, -1, 0, 1, 2, 3, 8, 50, 99, 1005 };
        private static IEnumerable<int> ValuesForEmptyRange => new[] { 0, 5, -99 };

        private const int RandomSeed = 128;
        private readonly System.Random random = new(RandomSeed);

        #region Reorder

        [Test]
        public void Reorder_MixedValues_MinAndMaxAreInCorrectOrder([ValueSource(nameof(TestValues))] int min, [ValueSource(nameof(TestValues))] int max)
        {
            Range<int> range = new Range<int>(min, max);

            range = range.Reorder();
            
            Assert.IsTrue(range.Min <= range.Max);
        }

        #endregion
        
        #region Random

        [Test]
        public void Random_GeneratedValue_ValueIsInsideOfRange([ValueSource(nameof(TestValues))] int min, [ValueSource(nameof(TestValues))] int max)
        {
            Range<int> range = new Range<int>(min, max);

            int value = range.Random();
            
            Assert.IsTrue(range.Contains(value), $"Generated value: {value}");
        }
        #endregion
        
        #region Lerp

        [Test]
        public void Lerp_FactorOne_ReturnsMax([ValueSource(nameof(TestValues))] int min, [ValueSource(nameof(TestValues))] int max)
        {
            Range<int> range = new Range<int>(min, max);

            float result = range.Lerp(1);
            
            Assert.IsTrue(Mathf.Approximately(result, max), $"Result: {result}");
        }
        
        [Test]
        public void Lerp_FactorZero_ReturnsMin([ValueSource(nameof(TestValues))] int min, [ValueSource(nameof(TestValues))] int max)
        {
            Range<int> range = new Range<int>(min, max);

            float result = range.Lerp(0);
            
            Assert.IsTrue(Mathf.Approximately(result, min), $"Result: {result}");
        }
        
        [Test]
        [TestCase(0, 0, 1, 0)]
        [TestCase(1, 2, 0.25f, 1.25f)]
        [TestCase(1, 2, 0.5f, 1.5f)]
        [TestCase(2, 1, 0.5f, 1.5f)]
        [TestCase(-10, -192, 0.75f, -146.5f)]
        [TestCase(120, 194, 0.81f, 179.94f)]
        [TestCase(10, 50, -1, 10)]
        [TestCase(-100, -10, 100, -10f)]
        public void Lerp_DifferentFactors_ReturnsExpectedResult([ValueSource(nameof(TestValues))] int min, [ValueSource(nameof(TestValues))] int max, float t, float expectedResult)
        {
            Range<int> range = new Range<int>(min, max);

            float result = range.Lerp(t);
            
            Assert.IsTrue(Mathf.Approximately(result, expectedResult), $"Result: {result}");
        }
        
        #endregion
        
        #region InverseLerp
        
        [Test]
        public void InverseLerp_MaxValue_ReturnsOne([ValueSource(nameof(TestValues))] int min, [ValueSource(nameof(TestValues))] int max)
        {
            if (min == max)
                return;
            
            Range<int> range = new Range<int>(min, max);

            float result = range.InverseLerp(max);
            
            Assert.IsTrue(Mathf.Approximately(result, 1), $"Result: {result}");
        }
        
        [Test]
        public void InverseLerp_MinValue_ReturnsZero([ValueSource(nameof(TestValues))] int min, [ValueSource(nameof(TestValues))] int max)
        {
            Range<int> range = new Range<int>(min, max);

            float result = range.InverseLerp(min);
            
            Assert.IsTrue(Mathf.Approximately(result, 0), $"Result: {result}");
        }
        
        [Test]
        [TestCase(0, 0, 0, 0)]
        [TestCase(1, 2, 1.25f, 0.25f)]
        [TestCase(1, 2, 1.5f, 0.5f)]
        [TestCase(2, 1, 1.5f, 0.5f)]
        [TestCase(-10, -192, -146.5f, 0.75f)]
        [TestCase(120, 194, 179.94f, 0.81f)]
        [TestCase(10, 50, 100, 1)]
        [TestCase(-100, -10, -110, 0f)]
        public void InverseLerp_DifferentValues_ReturnsExpectedResult([ValueSource(nameof(TestValues))] int min, [ValueSource(nameof(TestValues))] int max, float value, float expectedTResult)
        {
            Range<int> range = new Range<int>(min, max);

            float result = range.InverseLerp(value);
            
            Assert.IsTrue(Mathf.Approximately(result, expectedTResult), $"Result: {result}");
        }
        
        #endregion
        
        #region SmoothStep
        
        [Test]
        public void SmoothStep_FactorOne_ReturnsMax([ValueSource(nameof(TestValues))] int min, [ValueSource(nameof(TestValues))] int max)
        {
            Range<int> range = new Range<int>(min, max);

            float result = range.SmoothStep(1);
            
            Assert.IsTrue(Mathf.Approximately(result, max), $"Result: {result}");
        }
        
        [Test]
        public void SmoothStep_FactorZero_ReturnsMin([ValueSource(nameof(TestValues))] int min, [ValueSource(nameof(TestValues))] int max)
        {
            Range<int> range = new Range<int>(min, max);

            float result = range.SmoothStep(0);
            
            Assert.IsTrue(Mathf.Approximately(result, min), $"Result: {result}");
        }
        
        [Test]
        [TestCase(0, 0, 1, 0)]
        [TestCase(1, 2, 0.1f, 1.028f)]
        [TestCase(1, 5, 0.25f, 1.625f)]
        [TestCase(100, 5, 0.5f, 52.5f)]
        [TestCase(-10, 10, 0.75f, 6.875f)]
        [TestCase(190, 580, 0.9f, 569.08f)]
        [TestCase(10, 50, -1, 10)]
        [TestCase(-100, -10, 100, -10f)]
        public void SmoothStep_DifferentFactors_ReturnsExpectedResult([ValueSource(nameof(TestValues))] int min, [ValueSource(nameof(TestValues))] int max, float t, float expectedResult)
        {
            Range<int> range = new Range<int>(min, max);

            float result = range.SmoothStep(t);
            
            Assert.IsTrue(Mathf.Approximately(result, expectedResult), $"Result: {result}");
        }
        
        #endregion

        #region Delta
        
        [Test]
        [TestCase(0, 0, 0)]
        [TestCase(1, 2, 1)]
        [TestCase(2, 1, -1)]
        [TestCase(50, 50, 0)]
        [TestCase(100, 5, -95)]
        [TestCase(-10, 10, 20)]
        [TestCase(-150, -580, -430)]
        public void Delta_DifferentValues_ReturnsExpectedResult([ValueSource(nameof(TestValues))] int min, [ValueSource(nameof(TestValues))] int max, int expectedResult)
        {
            Range<int> range = new Range<int>(min, max);

            int result = range.Delta();
            
            Assert.IsTrue(result == expectedResult, $"Result: {result}");
        }
        
        #endregion
        
        #region Spread
        
        [Test]
        [TestCase(0, 0, 0)]
        [TestCase(1, 2, 1)]
        [TestCase(2, 1, 1)]
        [TestCase(50, 50, 0)]
        [TestCase(100, 5, 95)]
        [TestCase(-10, 10, 20)]
        [TestCase(-150, -580, 430)]
        public void Spread_DifferentValues_ReturnsExpectedResult([ValueSource(nameof(TestValues))] int min, [ValueSource(nameof(TestValues))] int max, int expectedResult)
        {
            Range<int> range = new Range<int>(min, max);

            int result = range.Spread();
            
            Assert.IsTrue(result == expectedResult, $"Result: {result}");
        }
        
        #endregion
        
        #region Clamp
        
        [Test]
        [TestCase(0, 0, 0)]
        [TestCase(1, 2, 2)]
        [TestCase(1, 2, 1)]
        [TestCase(200, -1, 2)]
        [TestCase(-10, -192, -120)]
        [TestCase(120, 194, 186)]
        public void Clamp_ValueInRange_ReturnsUnchangedValue(int min, int max, int value)
        {
            Range<int> range = new Range<int>(min, max);

            int clampedValue = range.Clamp(value);
            
            Assert.IsTrue(clampedValue == value, $"Result: {clampedValue}");
        }
        
        [Test]
        [TestCase(0, 0, 1)]
        [TestCase(1, 5, 10)]
        [TestCase(10, 50, -1)]
        [TestCase(5, 1, 0)]
        [TestCase(-100, -10, -101)]
        [TestCase(100, -1, 1000)]
        public void Clamp_ValueOutsideRange_ReturnsValueContainedInRange(int min, int max, int value)
        {
            Range<int> range = new Range<int>(min, max);

            int clampedValue = range.Clamp(value);
            
            Assert.IsTrue(range.Contains(clampedValue), $"Result: {clampedValue}");
        }
        
        #endregion
        
        #region Contains

        [Test]
        public void Contains_ValueInsideOfRange_ReturnsTrue([ValueSource(nameof(TestValues))] int min, [ValueSource(nameof(TestValues))] int max)
        {
            Range<int> range = new Range<int>(min, max);
            int value = random.Next(Mathf.Min(min, max), Mathf.Max(min, max));

            bool contains = range.Contains(value);

            Assert.IsTrue(contains, $"Tested value: {value}");
        }

        [Test]
        [TestCase(0, 0, 1)]
        [TestCase(1, 5, 10)]
        [TestCase(10, 50, -1)]
        [TestCase(5, 1, 0)]
        [TestCase(-100, -10, -101)]
        [TestCase(100, -1, 1000)]
        public void Contains_ValueOutsideOfRange_ReturnsFalse(int min, int max, int valueOutsideRange)
        {
            Range<int> range = new Range<int>(min, max);

            bool contains = range.Contains(valueOutsideRange);

            Assert.IsFalse(contains, $"Tested value: {valueOutsideRange}");
        }

        [Test]
        public void Contains_ValueAtMinBoundaryOfRange_ReturnsTrue([ValueSource(nameof(TestValues))] int min, [ValueSource(nameof(TestValues))] int max)
        {
            Range<int> range = new Range<int>(min, max);

            bool contains = range.Contains(min);

            Assert.IsTrue(contains, $"Tested value: {min}");
        }

        [Test]
        public void Contains_ValueAtMaxBoundaryOfRange_ReturnsTrue([ValueSource(nameof(TestValues))] int min, [ValueSource(nameof(TestValues))] int max)
        {
            Range<int> range = new Range<int>(min, max);

            bool contains = range.Contains(max);

            Assert.IsTrue(contains, $"Tested value: {max}");
        }
        
        #endregion
        
        #region Center

        [Test]
        public void Center_RangeWithMinEqualToMax_ReturnsMinValue([ValueSource(nameof(ValuesForEmptyRange))] int minMax)
        {
            Range<int> range = new Range<int>(minMax, minMax);
            
            float center = range.Center();

            Assert.IsTrue(Mathf.Approximately(center, minMax), $"Result: {center}");
        }
        
        [Test]
        [TestCase(5, 0, 2.5f)]
        [TestCase(100, -10, 45)]
        [TestCase(-5, -99, -52)]
        public void Center_RangeWithMinLargerThanMax_ReturnsExpectedCenter(int min, int max, float expectedResult)
        {
            Range<int> range = new Range<int>(min, max);

            float center = range.Center();

            Assert.IsTrue(Mathf.Approximately(center, expectedResult), $"Result: {center}");
        }
        
        [Test]
        [TestCase(0, 5, 2.5f)]
        [TestCase(-100, -10, -55)]
        [TestCase(-99, 150, 25.5f)]
        public void Center_RangeWithMinSmallerThanMax_ReturnsExpectedCenter(int min, int max, float expectedResult)
        {
            Range<int> range = new Range<int>(min, max);

            float center = range.Center();

            Assert.IsTrue(Mathf.Approximately(center, expectedResult), $"Result: {center}");
        }

        [Test]
        public void Center_DifferentRanges_CenterHasEqualDistanceFromMinAndMax([ValueSource(nameof(TestValues))] int min, [ValueSource(nameof(TestValues))] int max)
        {
            Range<int> range = new Range<int>(min, max);

            float center = range.Center();
            float distanceToMin = center - min;
            float distanceToMax = max - center;

            Assert.IsTrue(Mathf.Approximately(distanceToMin, distanceToMax), $"Distance to min: {distanceToMin}, distance to max: {distanceToMax}");
        }

        #endregion
    }
}