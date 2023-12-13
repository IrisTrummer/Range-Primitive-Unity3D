using System.Collections.Generic;
using NUnit.Framework;
using RangePrimitive;
using UnityEngine;

namespace Tests
{
    public class IntRangeTests
    {
        private static IEnumerable<int> TestValues => new[] { -150, -5, -3, -2, -1, 0, 1, 2, 3, 8, 50, 99, 1005 };
        private static IEnumerable<int> ValuesForEmptyRange => new[] { 0, 5, -99 };

        private const int RandomSeed = 128;
        private readonly System.Random random = new(RandomSeed);

        #region Reorder

        [Test]
        public void Reorder_DifferentRanges_MinIsSmallerThanOrEqualToMax([ValueSource(nameof(TestValues))] int min, [ValueSource(nameof(TestValues))] int max)
        {
            Range<int> range = new Range<int>(min, max);

            range = range.Reorder();

            Assert.IsTrue(range.Min <= range.Max);
        }

        #endregion

        #region Random

        [Test]
        public void Random_DifferentRanges_GeneratedValueIsContainedInRange([ValueSource(nameof(TestValues))] int min, [ValueSource(nameof(TestValues))] int max)
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
        [TestCase(0, 0)]
        [TestCase(-5, 0.5f)]
        [TestCase(99, 1)]
        public void Lerp_RangeWithMinEqualToMax_ReturnsMin(int minMax, float t)
        {
            Range<int> range = new Range<int>(minMax, minMax);

            float result = range.Lerp(t);

            Assert.IsTrue(Mathf.Approximately(result, minMax), $"Result: {result}");
        }

        [Test]
        [TestCase(99, -5, 0.25f, 73)]
        [TestCase(2, 1, 0.5f, 1.5f)]
        [TestCase(-10, -192, 0.75f, -146.5f)]
        public void Lerp_RangeWithMinLargerThanMax_ReturnsExpectedResult(int min, int max, float value, float expectedResult)
        {
            Range<int> range = new Range<int>(min, max);

            float result = range.Lerp(value);

            Assert.IsTrue(Mathf.Approximately(result, expectedResult), $"Result: {result}");
        }
        
        [Test]
        [TestCase(1, 2, 0.25f, 1.25f)]
        [TestCase(1, 2, 0.5f, 1.5f)]
        [TestCase(120, 194, 0.81f, 179.94f)]
        public void Lerp_RangeWithMinSmallerThanMax_ReturnsExpectedResult(int min, int max, float value, float expectedResult)
        {
            Range<int> range = new Range<int>(min, max);

            float result = range.Lerp(value);

            Assert.IsTrue(Mathf.Approximately(result, expectedResult), $"Result: {result}");
        }
        
        [Test]
        [TestCase(0, 1, -1)]
        [TestCase(99, 5, -2)]
        [TestCase(-5, -1, -0.5f)]
        public void Lerp_ValueSmallerThanZero_ReturnsMin(int min, int max, float t)
        {
            Range<int> range = new Range<int>(min, max);

            float result = range.Lerp(t);

            Assert.IsTrue(Mathf.Approximately(result, min), $"Result: {result}");
        }
        
        [Test]
        [TestCase(0, 1, 1)]
        [TestCase(99, 5, 2)]
        [TestCase(-5, -1, 1.5f)]
        public void Lerp_ValueLargerThanOne_ReturnsMax(int min, int max, float t)
        {
            Range<int> range = new Range<int>(min, max);

            float result = range.Lerp(t);

            Assert.IsTrue(Mathf.Approximately(result, max), $"Result: {result}");
        }

        #endregion

        #region InverseLerp

        [Test]
        [TestCase(0, 1)]
        [TestCase(-5, 99)]
        [TestCase(-105, 1)]
        public void InverseLerp_MaxValue_ReturnsOne(int min, int max)
        {
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
        [TestCase(0, 0)]
        [TestCase(0, 1)]
        [TestCase(50, -100)]
        [TestCase(-99, 100)]
        public void InverseLerp_RangeWithMinEqualToMax_ReturnsZero(int minMax, int value)
        {
            Range<int> range = new Range<int>(minMax, minMax);

            float result = range.InverseLerp(value);

            Assert.IsTrue(Mathf.Approximately(result, 0), $"Result: {result}");
        }

        [Test]
        [TestCase(99, -5, 73, 0.25f)]
        [TestCase(2, 1, 1.5f, 0.5f)]
        [TestCase(-10, -192, -146.5f, 0.75f)]
        public void InverseLerp_RangeWithMinLargerThanMax_ReturnsExpectedResult(int min, int max, float value, float expectedResult)
        {
            Range<int> range = new Range<int>(min, max);

            float result = range.InverseLerp(value);

            Assert.IsTrue(Mathf.Approximately(result, expectedResult), $"Result: {result}");
        }
        
        [Test]
        [TestCase(1, 2, 1.25f, 0.25f)]
        [TestCase(1, 2, 1.5f, 0.5f)]
        [TestCase(120, 194, 179.94f, 0.81f)]
        public void InverseLerp_RangeWithMinSmallerThanMax_ReturnsExpectedResult(int min, int max, float value, float expectedResult)
        {
            Range<int> range = new Range<int>(min, max);

            float result = range.InverseLerp(value);

            Assert.IsTrue(Mathf.Approximately(result, expectedResult), $"Result: {result}");
        }
        
        [Test]
        [TestCase(10, 50, 100, 1)]
        [TestCase(-99, -50, 100, 1)]
        [TestCase(-100, -10, -110, 0f)]
        [TestCase(100, 150, -1, 0f)]
        public void InverseLerp_ValueOutsideRange_ReturnsCloserBoundary(int min, int max, float value, float expectedResult)
        {
            Range<int> range = new Range<int>(min, max);

            float result = range.InverseLerp(value);

            Assert.IsTrue(Mathf.Approximately(result, expectedResult), $"Result: {result}");
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
        [TestCase(0, 0)]
        [TestCase(-5, 0.5f)]
        [TestCase(99, 1)]
        public void SmoothStep_RangeWithMinEqualToMax_ReturnsMin(int minMax, float t)
        {
            Range<int> range = new Range<int>(minMax, minMax);

            float result = range.SmoothStep(t);

            Assert.IsTrue(Mathf.Approximately(result, minMax), $"Result: {result}");
        }

        [Test]
        [TestCase(1, 0, 0.1f, 0.972f)]
        [TestCase(100, 5, 0.5f, 52.5f)]
        [TestCase(50, 5, 0.9f, 6.26f)]
        public void SmoothStep_RangeWithMinLargerThanMax_ReturnsExpectedResult(int min, int max, float value, float expectedResult)
        {
            Range<int> range = new Range<int>(min, max);

            float result = range.SmoothStep(value);

            Assert.IsTrue(Mathf.Approximately(result, expectedResult), $"Result: {result}");
        }

        [Test]
        [TestCase(1, 2, 0.1f, 1.028f)]
        [TestCase(1, 5, 0.25f, 1.625f)]
        [TestCase(-10, 10, 0.75f, 6.875f)]
        [TestCase(190, 580, 0.9f, 569.08f)]
        public void SmoothStep_RangeWithMinSmallerThanMax_ReturnsExpectedResult(int min, int max, float value, float expectedResult)
        {
            Range<int> range = new Range<int>(min, max);

            float result = range.SmoothStep(value);

            Assert.IsTrue(Mathf.Approximately(result, expectedResult), $"Result: {result}");
        }
        
        [Test]
        [TestCase(0, 1, -1)]
        [TestCase(99, 5, -2)]
        [TestCase(-5, -1, -0.5f)]
        public void SmoothStep_ValueSmallerThanZero_ReturnsMin(int min, int max, float t)
        {
            Range<int> range = new Range<int>(min, max);

            float result = range.SmoothStep(t);

            Assert.IsTrue(Mathf.Approximately(result, min), $"Result: {result}");
        }
        
        [Test]
        [TestCase(0, 1, 1.1f)]
        [TestCase(99, 5, 2)]
        [TestCase(-5, -1, 1.5f)]
        public void SmoothStep_ValueLargerThanOne_ReturnsMax(int min, int max, float t)
        {
            Range<int> range = new Range<int>(min, max);

            float result = range.SmoothStep(t);

            Assert.IsTrue(Mathf.Approximately(result, max), $"Result: {result}");
        }

        #endregion

        #region Delta

        [Test]
        public void Delta_RangeWithMinEqualToMax_ReturnsZero([ValueSource(nameof(ValuesForEmptyRange))] int minMax)
        {
            Range<int> range = new Range<int>(minMax, minMax);

            int result = range.Delta();

            Assert.IsTrue(result == 0, $"Result: {result}");
        }
        
        [Test]
        [TestCase(2, 1, -1)]
        [TestCase(100, 5, -95)]
        [TestCase(-150, -580, -430)]
        public void Delta_RangeWithMinLargerThanMax_ReturnsExpectedValue(int min, int max, int expectedResult)
        {
            Range<int> range = new Range<int>(min, max);

            int result = range.Delta();

            Assert.IsTrue(result == expectedResult, $"Result: {result}");
        }
        
        [Test]
        [TestCase(1, 2, 1)]
        [TestCase(-10, 10, 20)]
        [TestCase(-150, -100, 50)]
        public void Delta_RangeWithMinSmallerThanMax_ReturnsExpectedValue(int min, int max, int expectedResult)
        {
            Range<int> range = new Range<int>(min, max);

            int result = range.Delta();

            Assert.IsTrue(result == expectedResult, $"Result: {result}");
        }

        #endregion

        #region Size

        [Test]
        public void Size_RangeWithMinEqualToMax_ReturnsZero([ValueSource(nameof(ValuesForEmptyRange))] int minMax)
        {
            Range<int> range = new Range<int>(minMax, minMax);

            int result = range.Size();

            Assert.IsTrue(result == 0, $"Result: {result}");
        }
        
        [Test]
        [TestCase(2, 1, 1)]
        [TestCase(100, -5, 105)]
        [TestCase(-50, -99, 49)]
        public void Size_RangeWithMinLargerThanMax_ReturnsExpectedResult(int min, int max, int expectedResult)
        {
            Range<int> range = new Range<int>(min, max);

            int result = range.Size();

            Assert.IsTrue(result == expectedResult, $"Result: {result}");
        }
        
        [Test]
        [TestCase(1, 2, 1)]
        [TestCase(-10, 10, 20)]
        [TestCase(-580, -150, 430)]
        public void Size_RangeWithMinSmallerThanMax_ReturnsExpectedResult(int min, int max, int expectedResult)
        {
            Range<int> range = new Range<int>(min, max);

            int result = range.Size();

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
        [TestCase(5, 5, -10)]
        [TestCase(-99, -99, 1)]
        public void Clamp_RangeWithMinEqualToMax_ReturnsMin(int min, int max, int value)
        {
            Range<int> range = new Range<int>(min, max);

            int clampedValue = range.Clamp(value);

            Assert.IsTrue(clampedValue == range.Min, $"Result: {clampedValue}");
        }

        [Test]
        [TestCase(1, 5, 10)]
        [TestCase(100, -1, 1000)]
        [TestCase(-5, 10, 99)]
        [TestCase(-100, -99, -98)]
        public void Clamp_ValueLargerThanMax_ReturnsMax(int min, int max, int value)
        {
            Range<int> range = new Range<int>(min, max);

            int clampedValue = range.Clamp(value);

            Assert.IsTrue(clampedValue == Mathf.Max(min, max), $"Result: {clampedValue}");
        }
        
        [Test]
        [TestCase(1, 5, -10)]
        [TestCase(10, 50, -1)]
        [TestCase(5, 1, 0)]
        [TestCase(-100, -10, -101)]
        [TestCase(100, -1, -5)]
        public void Clamp_ValueSmallerThanMin_ReturnsMin(int min, int max, int value)
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