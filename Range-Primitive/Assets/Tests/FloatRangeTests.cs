using System.Collections.Generic;
using NUnit.Framework;
using RangePrimitive;
using UnityEngine;

namespace Tests
{
    public class FloatRangeTests
    {
        private static IEnumerable<float> TestValues => new[] { -150, -5.25f, -2, -1, 0, 0.5f, 1, 2, 3.1f, 50, 99.9f, 1005 };
        private static IEnumerable<float> ValuesForEmptyRange => new[] { 0, 5.5f, -99 };

        private const int RandomSeed = 128;
        private readonly System.Random random = new(RandomSeed);

        #region Reorder

        [Test]
        public void Reorder_DifferentRanges_MinIsSmallerThanOrEqualToMax([ValueSource(nameof(TestValues))] float min, [ValueSource(nameof(TestValues))] float max)
        {
            Range<float> range = new Range<float>(min, max);

            range = range.Reorder();

            Assert.IsTrue(range.Min <= range.Max);
        }

        #endregion

        #region Random

        [Test]
        public void Random_DifferentRanges_GeneratedValueIsContainedInRange([ValueSource(nameof(TestValues))] float min, [ValueSource(nameof(TestValues))] float max)
        {
            Range<float> range = new Range<float>(min, max);

            float value = range.Random();

            Assert.IsTrue(range.Contains(value), $"Generated value: {value}");
        }

        #endregion

        #region Lerp

        [Test]
        public void Lerp_FactorOne_ReturnsMax([ValueSource(nameof(TestValues))] float min, [ValueSource(nameof(TestValues))] float max)
        {
            Range<float> range = new Range<float>(min, max);

            float result = range.Lerp(1);

            Assert.IsTrue(Mathf.Approximately(result, max), $"Result: {result}");
        }

        [Test]
        public void Lerp_FactorZero_ReturnsMin([ValueSource(nameof(TestValues))] float min, [ValueSource(nameof(TestValues))] float max)
        {
            Range<float> range = new Range<float>(min, max);

            float result = range.Lerp(0);

            Assert.IsTrue(Mathf.Approximately(result, min), $"Result: {result}");
        }

        [Test]
        [TestCase(0, 0)]
        [TestCase(-5.5f, 0.5f)]
        [TestCase(99, 1)]
        public void Lerp_RangeWithMinEqualToMax_ReturnsMin(float minMax, float t)
        {
            Range<float> range = new Range<float>(minMax, minMax);

            float result = range.Lerp(t);

            Assert.IsTrue(Mathf.Approximately(result, minMax), $"Result: {result}");
        }

        [Test]
        [TestCase(99.5f, -5.5f, 0.25f, 73.25f)]
        [TestCase(2, 1, 0.5f, 1.5f)]
        [TestCase(-10, -192, 0.75f, -146.5f)]
        public void Lerp_RangeWithMinLargerThanMax_ReturnsExpectedResult(float min, float max, float value, float expectedResult)
        {
            Range<float> range = new Range<float>(min, max);

            float result = range.Lerp(value);

            Assert.IsTrue(Mathf.Approximately(result, expectedResult), $"Result: {result}");
        }
        
        [Test]
        [TestCase(1, 2, 0.25f, 1.25f)]
        [TestCase(1, 2, 0.5f, 1.5f)]
        [TestCase(120, 194, 0.81f, 179.94f)]
        public void Lerp_RangeWithMinSmallerThanMax_ReturnsExpectedResult(float min, float max, float value, float expectedResult)
        {
            Range<float> range = new Range<float>(min, max);

            float result = range.Lerp(value);

            Assert.IsTrue(Mathf.Approximately(result, expectedResult), $"Result: {result}");
        }
        
        [Test]
        [TestCase(0, 1, -1)]
        [TestCase(99.9f, 5, -2)]
        [TestCase(-5, -1.1f, -0.5f)]
        public void Lerp_ValueSmallerThanZero_ReturnsMin(float min, float max, float t)
        {
            Range<float> range = new Range<float>(min, max);

            float result = range.Lerp(t);

            Assert.IsTrue(Mathf.Approximately(result, min), $"Result: {result}");
        }
        
        [Test]
        [TestCase(0, 1, 1)]
        [TestCase(99, 5.5f, 2)]
        [TestCase(-5.1f, -1, 1.5f)]
        public void Lerp_ValueLargerThanOne_ReturnsMax(float min, float max, float t)
        {
            Range<float> range = new Range<float>(min, max);

            float result = range.Lerp(t);

            Assert.IsTrue(Mathf.Approximately(result, max), $"Result: {result}");
        }

        #endregion

        #region InverseLerp

        [Test]
        [TestCase(0, 1)]
        [TestCase(-5.5f, 99)]
        [TestCase(-105, 1.1f)]
        public void InverseLerp_MaxValue_ReturnsOne(float min, float max)
        {
            Range<float> range = new Range<float>(min, max);

            float result = range.InverseLerp(max);

            Assert.IsTrue(Mathf.Approximately(result, 1), $"Result: {result}");
        }

        [Test]
        public void InverseLerp_MinValue_ReturnsZero([ValueSource(nameof(TestValues))] float min, [ValueSource(nameof(TestValues))] float max)
        {
            Range<float> range = new Range<float>(min, max);

            float result = range.InverseLerp(min);

            Assert.IsTrue(Mathf.Approximately(result, 0), $"Result: {result}");
        }
        
        [Test]
        [TestCase(0, 0)]
        [TestCase(0, 1)]
        [TestCase(50, -100)]
        [TestCase(-99.9f, 100)]
        public void InverseLerp_RangeWithMinEqualToMax_ReturnsZero(float minMax, float value)
        {
            Range<float> range = new Range<float>(minMax, minMax);

            float result = range.InverseLerp(value);

            Assert.IsTrue(Mathf.Approximately(result, 0), $"Result: {result}");
        }

        [Test]
        [TestCase(99, -5, 73, 0.25f)]
        [TestCase(2, 1, 1.5f, 0.5f)]
        [TestCase(-10, -192, -146.5f, 0.75f)]
        public void InverseLerp_RangeWithMinLargerThanMax_ReturnsExpectedResult(float min, float max, float value, float expectedResult)
        {
            Range<float> range = new Range<float>(min, max);

            float result = range.InverseLerp(value);

            Assert.IsTrue(Mathf.Approximately(result, expectedResult), $"Result: {result}");
        }
        
        [Test]
        [TestCase(1, 2, 1.25f, 0.25f)]
        [TestCase(1, 2, 1.5f, 0.5f)]
        [TestCase(120, 194, 179.94f, 0.81f)]
        public void InverseLerp_RangeWithMinSmallerThanMax_ReturnsExpectedResult(float min, float max, float value, float expectedResult)
        {
            Range<float> range = new Range<float>(min, max);

            float result = range.InverseLerp(value);

            Assert.IsTrue(Mathf.Approximately(result, expectedResult), $"Result: {result}");
        }
        
        [Test]
        [TestCase(10, 50, 100, 1)]
        [TestCase(-99, -50, 100, 1)]
        [TestCase(-100, -10, -110, 0f)]
        [TestCase(100, 150, -1, 0f)]
        public void InverseLerp_ValueOutsideRange_ReturnsCloserBoundary(float min, float max, float value, float expectedResult)
        {
            Range<float> range = new Range<float>(min, max);

            float result = range.InverseLerp(value);

            Assert.IsTrue(Mathf.Approximately(result, expectedResult), $"Result: {result}");
        }

        #endregion

        #region SmoothStep

        [Test]
        public void SmoothStep_FactorOne_ReturnsMax([ValueSource(nameof(TestValues))] float min, [ValueSource(nameof(TestValues))] float max)
        {
            Range<float> range = new Range<float>(min, max);

            float result = range.SmoothStep(1);

            Assert.IsTrue(Mathf.Approximately(result, max), $"Result: {result}");
        }

        [Test]
        public void SmoothStep_FactorZero_ReturnsMin([ValueSource(nameof(TestValues))] float min, [ValueSource(nameof(TestValues))] float max)
        {
            Range<float> range = new Range<float>(min, max);

            float result = range.SmoothStep(0);

            Assert.IsTrue(Mathf.Approximately(result, min), $"Result: {result}");
        }

        [Test]
        [TestCase(0, 0)]
        [TestCase(-5, 0.5f)]
        [TestCase(99.9f, 1)]
        public void SmoothStep_RangeWithMinEqualToMax_ReturnsMin(float minMax, float t)
        {
            Range<float> range = new Range<float>(minMax, minMax);

            float result = range.SmoothStep(t);

            Assert.IsTrue(Mathf.Approximately(result, minMax), $"Result: {result}");
        }

        [Test]
        [TestCase(1, 0, 0.1f, 0.972f)]
        [TestCase(100, 5, 0.5f, 52.5f)]
        [TestCase(50, 5, 0.9f, 6.26f)]
        public void SmoothStep_RangeWithMinLargerThanMax_ReturnsExpectedResult(float min, float max, float value, float expectedResult)
        {
            Range<float> range = new Range<float>(min, max);

            float result = range.SmoothStep(value);

            Assert.IsTrue(Mathf.Approximately(result, expectedResult), $"Result: {result}");
        }

        [Test]
        [TestCase(1, 2, 0.1f, 1.028f)]
        [TestCase(1, 5, 0.25f, 1.625f)]
        [TestCase(-10, 10, 0.75f, 6.875f)]
        [TestCase(190, 580, 0.9f, 569.08f)]
        public void SmoothStep_RangeWithMinSmallerThanMax_ReturnsExpectedResult(float min, float max, float value, float expectedResult)
        {
            Range<float> range = new Range<float>(min, max);

            float result = range.SmoothStep(value);

            Assert.IsTrue(Mathf.Approximately(result, expectedResult), $"Result: {result}");
        }
        
        [Test]
        [TestCase(0, 1, -1)]
        [TestCase(99, 5, -2)]
        [TestCase(-5.5f, -1, -0.5f)]
        public void SmoothStep_ValueSmallerThanZero_ReturnsMin(float min, float max, float t)
        {
            Range<float> range = new Range<float>(min, max);

            float result = range.SmoothStep(t);

            Assert.IsTrue(Mathf.Approximately(result, min), $"Result: {result}");
        }
        
        [Test]
        [TestCase(0, 1, 1.1f)]
        [TestCase(99, 5, 2)]
        [TestCase(-5, -1, 1.5f)]
        public void SmoothStep_ValueLargerThanOne_ReturnsMax(float min, float max, float t)
        {
            Range<float> range = new Range<float>(min, max);

            float result = range.SmoothStep(t);

            Assert.IsTrue(Mathf.Approximately(result, max), $"Result: {result}");
        }

        #endregion

        #region Delta

        [Test]
        public void Delta_RangeWithMinEqualToMax_ReturnsZero([ValueSource(nameof(ValuesForEmptyRange))] float minMax)
        {
            Range<float> range = new Range<float>(minMax, minMax);

            float result = range.Delta();

            Assert.IsTrue(Mathf.Approximately(result, 0), $"Result: {result}");
        }
        
        [Test]
        [TestCase(2, 1, -1)]
        [TestCase(100.5f, 5, -95.5f)]
        [TestCase(-150, -580.9f, -430.9f)]
        public void Delta_RangeWithMinLargerThanMax_ReturnsExpectedValue(float min, float max, float expectedResult)
        {
            Range<float> range = new Range<float>(min, max);

            float result = range.Delta();

            Assert.IsTrue(Mathf.Approximately(result, expectedResult), $"Result: {result}");
        }
        
        [Test]
        [TestCase(1, 2, 1)]
        [TestCase(-10.1f, 10, 20.1f)]
        [TestCase(-150.9f, -100, 50.9f)]
        public void Delta_RangeWithMinSmallerThanMax_ReturnsExpectedValue(float min, float max, float expectedResult)
        {
            Range<float> range = new Range<float>(min, max);

            float result = range.Delta();

            Assert.IsTrue(Mathf.Approximately(result, expectedResult), $"Result: {result}");
        }

        #endregion

        #region Size

        [Test]
        public void Size_RangeWithMinEqualToMax_ReturnsZero([ValueSource(nameof(ValuesForEmptyRange))] float minMax)
        {
            Range<float> range = new Range<float>(minMax, minMax);

            float result = range.Size();

            Assert.IsTrue(Mathf.Approximately(result, 0), $"Result: {result}");
        }
        
        [Test]
        [TestCase(2, 1, 1)]
        [TestCase(100, -5.5f, 105.5f)]
        [TestCase(-50, -99.9f, 49.9f)]
        public void Size_RangeWithMinLargerThanMax_ReturnsExpectedResult(float min, float max, float expectedResult)
        {
            Range<float> range = new Range<float>(min, max);

            float result = range.Size();

            Assert.IsTrue(Mathf.Approximately(result, expectedResult), $"Result: {result}");
        }
        
        [Test]
        [TestCase(1, 2, 1)]
        [TestCase(-10.1f, 10, 20.1f)]
        [TestCase(-580, -150.8f, 429.2f)]
        public void Size_RangeWithMinSmallerThanMax_ReturnsExpectedResult(float min, float max, float expectedResult)
        {
            Range<float> range = new Range<float>(min, max);

            float result = range.Size();

            Assert.IsTrue(Mathf.Approximately(result, expectedResult), $"Result: {result}");
        }

        #endregion

        #region Clamp

        [Test]
        [TestCase(0, 0, 0)]
        [TestCase(1, 2, 2)]
        [TestCase(1, 2, 1)]
        [TestCase(200.5f, -1, 2)]
        [TestCase(-10, -192.9f, -120)]
        [TestCase(120, 194, 186.8f)]
        public void Clamp_ValueInRange_ReturnsUnchangedValue(float min, float max, float value)
        {
            Range<float> range = new Range<float>(min, max);

            float clampedValue = range.Clamp(value);

            Assert.IsTrue(Mathf.Approximately(clampedValue, value), $"Result: {clampedValue}");
        }

        [Test]
        [TestCase(0, 1)]
        [TestCase(5, 10.5f)]
        [TestCase(-99.9f, 1)]
        public void Clamp_RangeWithMinEqualToMax_ReturnsMin(float minMax, float value)
        {
            Range<float> range = new Range<float>(minMax, minMax);

            float clampedValue = range.Clamp(value);

            Assert.IsTrue(Mathf.Approximately(clampedValue, range.Min), $"Result: {clampedValue}");
        }

        [Test]
        [TestCase(1, 5, 10)]
        [TestCase(100.1f, -1, 1000)]
        [TestCase(-5, 10.9f, 99)]
        [TestCase(-100, -99, -98.5f)]
        public void Clamp_ValueLargerThanMax_ReturnsMax(float min, float max, float value)
        {
            Range<float> range = new Range<float>(min, max);

            float clampedValue = range.Clamp(value);

            Assert.IsTrue(Mathf.Approximately(clampedValue, Mathf.Max(min, max)), $"Result: {clampedValue}");
        }
        
        [Test]
        [TestCase(1, 5, -10)]
        [TestCase(10, 50, -1)]
        [TestCase(5.5f, 1, 0)]
        [TestCase(-100, -10.9f, -101)]
        [TestCase(100, -1, -5.1f)]
        public void Clamp_ValueSmallerThanMin_ReturnsMin(float min, float max, float value)
        {
            Range<float> range = new Range<float>(min, max);

            float clampedValue = range.Clamp(value);

            Assert.IsTrue(Mathf.Approximately(clampedValue, Mathf.Min(min, max)), $"Result: {clampedValue}");
        }

        #endregion

        #region Contains

        [Test]
        public void Contains_ValueInsideOfRange_ReturnsTrue([ValueSource(nameof(TestValues))] float min, [ValueSource(nameof(TestValues))] float max)
        {
            Range<float> range = new Range<float>(min, max);
            float value = TestHelper.GenerateRandomValueInRange(random, min, max);

            bool contains = range.Contains(value);

            Assert.IsTrue(contains, $"Tested value: {value}");
        }

        [Test]
        [TestCase(0, 0, 1)]
        [TestCase(1, 5, 10)]
        [TestCase(10, 50.5f, -1)]
        [TestCase(5, 1, 0)]
        [TestCase(-100.9f, -10, -101)]
        [TestCase(100, -1, 1000.1f)]
        public void Contains_ValueOutsideOfRange_ReturnsFalse(float min, float max, float valueOutsideRange)
        {
            Range<float> range = new Range<float>(min, max);

            bool contains = range.Contains(valueOutsideRange);

            Assert.IsFalse(contains, $"Tested value: {valueOutsideRange}");
        }

        [Test]
        public void Contains_ValueAtMinBoundaryOfRange_ReturnsTrue([ValueSource(nameof(TestValues))] float min, [ValueSource(nameof(TestValues))] float max)
        {
            Range<float> range = new Range<float>(min, max);

            bool contains = range.Contains(min);

            Assert.IsTrue(contains, $"Tested value: {min}");
        }

        [Test]
        public void Contains_ValueAtMaxBoundaryOfRange_ReturnsTrue([ValueSource(nameof(TestValues))] float min, [ValueSource(nameof(TestValues))] float max)
        {
            Range<float> range = new Range<float>(min, max);

            bool contains = range.Contains(max);

            Assert.IsTrue(contains, $"Tested value: {max}");
        }

        #endregion

        #region Center

        [Test]
        public void Center_RangeWithMinEqualToMax_ReturnsMinValue([ValueSource(nameof(ValuesForEmptyRange))] float minMax)
        {
            Range<float> range = new Range<float>(minMax, minMax);

            float center = range.Center();

            Assert.IsTrue(Mathf.Approximately(center, range.Min), $"Result: {center}");
        }

        [Test]
        [TestCase(5.5f, 0, 2.75f)]
        [TestCase(100, -10, 45)]
        [TestCase(-5, -99.9f, -52.45f)]
        public void Center_RangeWithMinLargerThanMax_ReturnsExpectedCenter(float min, float max, float expectedResult)
        {
            Range<float> range = new Range<float>(min, max);

            float center = range.Center();

            Assert.IsTrue(Mathf.Approximately(center, expectedResult), $"Result: {center}");
        }

        [Test]
        [TestCase(0, 5, 2.5f)]
        [TestCase(-100.9f, -10, -55.45f)]
        [TestCase(-99, 150, 25.5f)]
        public void Center_RangeWithMinSmallerThanMax_ReturnsExpectedCenter(float min, float max, float expectedResult)
        {
            Range<float> range = new Range<float>(min, max);

            float center = range.Center();

            Assert.IsTrue(Mathf.Approximately(center, expectedResult), $"Result: {center}");
        }

        [Test]
        public void Center_DifferentRanges_CenterHasEqualDistanceFromMinAndMax([ValueSource(nameof(TestValues))] float min, [ValueSource(nameof(TestValues))] float max)
        {
            Range<float> range = new Range<float>(min, max);

            float center = range.Center();
            float distanceToMin = center - min;
            float distanceToMax = max - center;

            Assert.IsTrue(Mathf.Approximately(distanceToMin, distanceToMax), $"Distance to min: {distanceToMin}, distance to max: {distanceToMax}");
        }

        #endregion
    }
}