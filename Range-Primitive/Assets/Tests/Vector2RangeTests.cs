using System.Collections.Generic;
using NUnit.Framework;
using RangePrimitive;
using UnityEngine;

namespace Tests
{
    public class Vector2RangeTests
    {
        private static IEnumerable<Vector2> TestValues => new Vector2[] { new(-150, -5.25f), new(-2, -1), new(0, 0), new(0.5f, -0.5f), new(1, 2), new(3.5f, 5.2f), new(50, -50), new(99.9f, 1005) };
        private static IEnumerable<Vector2> ValuesForEmptyRange => new[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(99.9f, -99.9f) };

        private const int RandomSeed = 128;
        private readonly System.Random random = new(RandomSeed);

        #region Reorder

        [Test]
        public void ReorderPerComponent_DifferentRanges_MinIsSmallerThanOrEqualToMax([ValueSource(nameof(TestValues))] Vector2 min, [ValueSource(nameof(TestValues))] Vector2 max)
        {
            Range<Vector2> range = new Range<Vector2>(min, max);

            range = range.ReorderPerComponent();

            Assert.IsTrue(range.Min.x <= range.Max.x && range.Min.y <= range.Max.y);
        }

        #endregion

        #region Random

        [Test]
        public void Random_DifferentRanges_GeneratedValueIsContainedInRange([ValueSource(nameof(TestValues))] Vector2 min, [ValueSource(nameof(TestValues))] Vector2 max)
        {
            Range<Vector2> range = new Range<Vector2>(min, max);

            Vector2 value = range.Random();

            Assert.IsTrue(range.Contains(value), $"Generated value: {value}");
        }

        #endregion

        #region Lerp

        [Test]
        public void Lerp_FactorOne_ReturnsMax([ValueSource(nameof(TestValues))] Vector2 min, [ValueSource(nameof(TestValues))] Vector2 max)
        {
            Range<Vector2> range = new Range<Vector2>(min, max);

            Vector2 result = range.Lerp(1);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, max), $"Result: {result}");
        }

        [Test]
        public void Lerp_FactorZero_ReturnsMin([ValueSource(nameof(TestValues))] Vector2 min, [ValueSource(nameof(TestValues))] Vector2 max)
        {
            Range<Vector2> range = new Range<Vector2>(min, max);

            Vector2 result = range.Lerp(0);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, min), $"Result: {result}");
        }

        [Test]
        [TestCase(0, 0, 0)]
        [TestCase(1, 0, 0.5f)]
        [TestCase(99.9f, -99.9f, 1)]
        public void Lerp_RangeWithMinEqualToMax_ReturnsMin(float minMaxX, float minMaxY, float t)
        {
            Vector2 minMax = new Vector2(minMaxX, minMaxY);
            Range<Vector2> range = new Range<Vector2>(minMax, minMax);

            Vector2 result = range.Lerp(t);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, minMax), $"Result: {result}");
        }

        [Test]
        [TestCase(99.5f, -5.5f, 99.5f, -5.5f, 0.25f, 73.25f, 73.25f)]
        [TestCase(1, 2, -2, -1, 0.25f, 1.25f, -1.75f)]
        [TestCase(2, 1, 50.5f, 5f, 0.5f, 1.5f, 27.75f)]
        [TestCase(1, 2, 1, 2, 0.5f, 1.5f, 1.5f)]
        [TestCase(-10, -192, 192, 10, 0.75f, -146.5f, 55.5f)]
        [TestCase(120, 194, 50, 99, 0.81f, 179.94f, 89.69f)]
        public void Lerp_DifferentRanges_ReturnsExpectedResult(float minX, float maxX, float minY, float maxY, float t, float expectedResultX, float expectedResultY)
        {
            Range<Vector2> range = new Range<Vector2>(new Vector2(minX, minY), new Vector2(maxX, maxY));
            Vector2 expectedResult = new Vector2(expectedResultX, expectedResultY);

            Vector2 result = range.Lerp(t);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, expectedResult), $"Result: {result}");
        }

        [Test]
        [TestCase(0, 1, -1, -5, -1)]
        [TestCase(99.9f, 5, 99.9f, 5, -2)]
        [TestCase(-5, -1.1f, 100f, -1f, -0.5f)]
        public void Lerp_ValueSmallerThanZero_ReturnsMin(float minX, float maxX, float minY, float maxY, float t)
        {
            Range<Vector2> range = new Range<Vector2>(new Vector2(minX, minY), new Vector2(maxX, maxY));

            Vector2 result = range.Lerp(t);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, range.Min), $"Result: {result}");
        }

        [Test]
        [TestCase(0, 1, -1, -5, 1.5f)]
        [TestCase(99.9f, 5, 99.9f, 5, 2)]
        [TestCase(-5, -1.1f, 100f, -1f, 99.9f)]
        public void Lerp_ValueLargerThanOne_ReturnsMax(float minX, float maxX, float minY, float maxY, float t)
        {
            Range<Vector2> range = new Range<Vector2>(new Vector2(minX, minY), new Vector2(maxX, maxY));

            Vector2 result = range.Lerp(t);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, range.Max), $"Result: {result}");
        }

        #endregion

        #region InverseLerp

        [Test]
        [TestCase(0, 1, -1, 0)]
        [TestCase(-5.5f, 99, -5.5f, 99)]
        [TestCase(-105, 1.1f, 99.9f, 5)]
        public void InverseLerp_MaxValue_ReturnsOne(float minX, float maxX, float minY, float maxY)
        {
            Range<Vector2> range = new Range<Vector2>(new Vector2(minX, minY), new Vector2(maxX, maxY));

            Vector2 result = range.InverseLerp(range.Max);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, Vector2.one), $"Result: {result}");
        }

        [Test]
        public void InverseLerp_MinValue_ReturnsZero([ValueSource(nameof(TestValues))] Vector2 min, [ValueSource(nameof(TestValues))] Vector2 max)
        {
            Range<Vector2> range = new Range<Vector2>(min, max);

            Vector2 result = range.InverseLerp(min);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, Vector2.zero), $"Result: {result}");
        }

        [Test]
        [TestCase(0, 0, 0, 0)]
        [TestCase(1, 0, 1, 0)]
        [TestCase(99.9f, -99.9f, 100, -5.5f)]
        public void InverseLerp_RangeWithMinEqualToMax_ReturnsZero(float minMaxX, float minMaxY, float valueX, float valueY)
        {
            Vector2 minMax = new Vector2(minMaxX, minMaxY);
            Range<Vector2> range = new Range<Vector2>(minMax, minMax);
            Vector2 value = new Vector2(valueX, valueY);

            Vector2 result = range.InverseLerp(value);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, Vector2.zero), $"Result: {result}");
        }

        [Test]
        [TestCase(99, -5, 99.5f, -5.5f, 73, 73.25f, 0.25f)]
        [TestCase(1, 2, -2, -1, 1.25f, -1.75f, 0.25f)]
        [TestCase(2, 1, 1, 2, 1.5f, 1.5f, 0.5f)]
        [TestCase(1, 2, 1, 2, 1.5f, 1.5f, 0.5f)]
        [TestCase(-10, -192, 50.5f, 5f, -146.5f, 16.375f, 0.75f)]
        [TestCase(120, 194, 50, 99, 179.94f, 89.69f, 0.81f)]
        public void InverseLerp_DifferentRanges_ReturnsExpectedResult(float minX, float maxX, float minY, float maxY, float valueX, float valueY, float expectedResult)
        {
            Range<Vector2> range = new Range<Vector2>(new Vector2(minX, minY), new Vector2(maxX, maxY));
            Vector2 value = new Vector2(valueX, valueY);

            Vector2 result = range.InverseLerp(value);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, expectedResult * Vector2.one), $"Result: {result}");
        }

        [Test]
        [TestCase(10, 50, 50, 10, 100, 100, 1, 0)]
        [TestCase(-99, -50, 1, 3, 100, 100, 1, 1)]
        [TestCase(-100, -10, 100, 10, -110, -110, 0, 1)]
        [TestCase(100, 150, -1, -5, -1, 0, 0, 0)]
        public void InverseLerp_ValueOutsideRange_ReturnsCloserBoundary(float minX, float maxX, float minY, float maxY, float valueX, float valueY, float expectedResultX, float expectedResultY)
        {
            Range<Vector2> range = new Range<Vector2>(new Vector2(minX, minY), new Vector2(maxX, maxY));
            Vector2 expectedResult = new Vector2(expectedResultX, expectedResultY);
            Vector2 value = new Vector2(valueX, valueY);

            Vector2 result = range.InverseLerp(value);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, expectedResult), $"Result: {result}");
        }

        #endregion

        #region SmoothStep

        [Test]
        public void SmoothStep_FactorOne_ReturnsMax([ValueSource(nameof(TestValues))] Vector2 min, [ValueSource(nameof(TestValues))] Vector2 max)
        {
            Range<Vector2> range = new Range<Vector2>(min, max);

            Vector2 result = range.SmoothStep(1);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, max), $"Result: {result}");
        }

        [Test]
        public void SmoothStep_FactorZero_ReturnsMin([ValueSource(nameof(TestValues))] Vector2 min, [ValueSource(nameof(TestValues))] Vector2 max)
        {
            Range<Vector2> range = new Range<Vector2>(min, max);

            Vector2 result = range.SmoothStep(0);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, min), $"Result: {result}");
        }

        [Test]
        [TestCase(0, 0, 0)]
        [TestCase(-5, 5, 0.25f)]
        [TestCase(99.9f, -99.9f, 0.5f)]
        [TestCase(1, 0, 1)]
        public void SmoothStep_RangeWithMinEqualToMax_ReturnsMin(float minMaxX, float minMaxY, float t)
        {
            Vector2 minMax = new Vector2(minMaxX, minMaxY);
            Range<Vector2> range = new Range<Vector2>(minMax, minMax);

            Vector2 result = range.SmoothStep(t);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, minMax), $"Result: {result}");
        }

        [Test]
        [TestCase(1, 0, 1, 2, 0.1f, 0.972f, 1.028f)]
        [TestCase(1, 5, 1, 5, 0.25f, 1.625f, 1.625f)]
        [TestCase(100, 5, -100, -5, 0.5f, 52.5f, -52.5f)]
        [TestCase(-10, 10, -10, 10, 0.75f, 6.875f, 6.875f)]
        [TestCase(50, 5, 190, 580, 0.9f, 6.26f, 569.08f)]
        public void SmoothStep_DifferentRanges_ReturnsExpectedResult(float minX, float maxX, float minY, float maxY, float t, float expectedResultX, float expectedResultY)
        {
            Range<Vector2> range = new Range<Vector2>(new Vector2(minX, minY), new Vector2(maxX, maxY));
            Vector2 expectedResult = new Vector2(expectedResultX, expectedResultY);

            Vector2 result = range.SmoothStep(t);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, expectedResult), $"Result: {result}");
        }

        [Test]
        [TestCase(0, 1, -1, -5, -1)]
        [TestCase(99.9f, 5, 99.9f, 5, -2)]
        [TestCase(-5, -1.1f, 100f, -1f, -0.5f)]
        public void SmoothStep_ValueSmallerThanZero_ReturnsMin(float minX, float maxX, float minY, float maxY, float t)
        {
            Range<Vector2> range = new Range<Vector2>(new Vector2(minX, minY), new Vector2(maxX, maxY));

            Vector2 result = range.SmoothStep(t);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, range.Min), $"Result: {result}");
        }

        [Test]
        [TestCase(0, 1, -1, -5, 1)]
        [TestCase(99.9f, 5, 99.9f, 5, 2)]
        [TestCase(-5, -1.1f, 100f, -1f, 1.5f)]
        public void SmoothStep_ValueLargerThanOne_ReturnsMax(float minX, float maxX, float minY, float maxY, float t)
        {
            Range<Vector2> range = new Range<Vector2>(new Vector2(minX, minY), new Vector2(maxX, maxY));

            Vector2 result = range.SmoothStep(t);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, range.Max), $"Result: {result}");
        }

        #endregion

        #region Delta

        [Test]
        public void Delta_RangeWithMinEqualToMax_ReturnsZero([ValueSource(nameof(ValuesForEmptyRange))] Vector2 minMax)
        {
            Range<Vector2> range = new Range<Vector2>(minMax, minMax);

            Vector2 result = range.Delta();

            Assert.IsTrue(TestHelper.EqualsApproximately(result, Vector2.zero), $"Result: {result}");
        }

        [Test]
        [TestCase(2, 1, 1, 2, -1, 1)]
        [TestCase(-10.1f, 10, 100.5f, 5, 20.1f, -95.5f)]
        [TestCase(-150, -580.9f, -150.9f, -100, -430.9f, 50.9f)]
        public void Delta_DifferentRanges_ReturnsExpectedValue(float minX, float maxX, float minY, float maxY, float expectedResultX, float expectedResultY)
        {
            Range<Vector2> range = new Range<Vector2>(new Vector2(minX, minY), new Vector2(maxX, maxY));
            Vector2 expectedResult = new Vector2(expectedResultX, expectedResultY);

            Vector2 result = range.Delta();

            Assert.IsTrue(TestHelper.EqualsApproximately(result, expectedResult), $"Result: {result}");
        }

        #endregion

        #region Size

        [Test]
        public void Size_RangeWithMinEqualToMax_ReturnsZero([ValueSource(nameof(ValuesForEmptyRange))] Vector2 minMax)
        {
            Range<Vector2> range = new Range<Vector2>(minMax, minMax);

            Vector2 result = range.Size();

            Assert.IsTrue(TestHelper.EqualsApproximately(result, Vector2.zero), $"Result: {result}");
        }

        [Test]
        [TestCase(2, 1, 1, 2, 1, 1)]
        [TestCase(100, -5.5f, -10.1f, 10, 105.5f, 20.1f)]
        [TestCase(-580, -150.8f, -50, -99.9f, 429.2f, 49.9f)]
        public void Size_DifferentRanges_ReturnsExpectedResult(float minX, float maxX, float minY, float maxY, float expectedResultX, float expectedResultY)
        {
            Range<Vector2> range = new Range<Vector2>(new Vector2(minX, minY), new Vector2(maxX, maxY));
            Vector2 expectedResult = new Vector2(expectedResultX, expectedResultY);

            Vector2 result = range.Size();

            Assert.IsTrue(TestHelper.EqualsApproximately(result, expectedResult), $"Result: {result}");
        }

        #endregion

        #region Clamp

        [Test]
        [TestCase(0, 0, 0, 0, 0, 0)]
        [TestCase(1, 2, 2, 1, 2, 2)]
        [TestCase(1, 2, 1, 2, 1, 1)]
        [TestCase(200.5f, -1, -5f, 5f, 2, 4)]
        [TestCase(-10, -192.9f, 5, 99, -120, 50)]
        [TestCase(120, 194, -120, -194, 186.8f, -186.8f)]
        public void Clamp_ValueInRange_ReturnsUnchangedValue(float minX, float maxX, float minY, float maxY, float valueX, float valueY)
        {
            Range<Vector2> range = new Range<Vector2>(new Vector2(minX, minY), new Vector2(maxX, maxY));
            Vector2 value = new Vector2(valueX, valueY);

            Vector2 clampedValue = range.Clamp(value);

            Assert.IsTrue(TestHelper.EqualsApproximately(clampedValue, value), $"Result: {clampedValue}");
        }

        [Test]
        [TestCase(0, 0, 5, -5)]
        [TestCase(-5, 5, 99.9f, 99.9f)]
        [TestCase(99.9f, -99.9f, 0.5f, 50)]
        [TestCase(1, 0, 100, 1)]
        public void Clamp_RangeWithMinEqualToMax_ReturnsMin(float minMaxX, float minMaxY, float valueX, float valueY)
        {
            Vector2 minMax = new Vector2(minMaxX, minMaxY);
            Range<Vector2> range = new Range<Vector2>(minMax, minMax);
            Vector2 value = new Vector2(valueX, valueY);

            Vector2 clampedValue = range.Clamp(value);

            Assert.IsTrue(TestHelper.EqualsApproximately(clampedValue, range.Min), $"Result: {clampedValue}");
        }

        [Test]
        [TestCase(1, 5, 1, 5, 100, 10)]
        [TestCase(100.1f, -1, -5, 99.9f, 1000, 100)]
        [TestCase(-5, 10.9f, 0.1f, 0.5f, 99, 1)]
        [TestCase(-100, -99, 100, 99, -98.5f, 101)]
        public void Clamp_ValueLargerThanMax_ReturnsMax(float minX, float maxX, float minY, float maxY, float valueX, float valueY)
        {
            Range<Vector2> range = new Range<Vector2>(new Vector2(minX, minY), new Vector2(maxX, maxY));
            Vector2 value = new Vector2(valueX, valueY);
            Vector2 expectedResult = new Vector2(Mathf.Max(range.Min.x, range.Max.x), Mathf.Max(range.Min.y, range.Max.y));

            Vector2 clampedValue = range.Clamp(value);

            Assert.IsTrue(TestHelper.EqualsApproximately(clampedValue, expectedResult), $"Result: {clampedValue}");
        }

        [Test]
        [TestCase(1, 5, 1, 5, -10, -10)]
        [TestCase(10, 50, 50, 10, -1, -1)]
        [TestCase(5.5f, 1, 99.9f, -99.9f, 0, -110)]
        [TestCase(-100, -10.9f, 5.5f, 5f, -101, -5)]
        [TestCase(100, -1, 0.1f, 0.5f, -5.1f, 0)]
        public void Clamp_ValueSmallerThanMin_ReturnsMin(float minX, float maxX, float minY, float maxY, float valueX, float valueY)
        {
            Range<Vector2> range = new Range<Vector2>(new Vector2(minX, minY), new Vector2(maxX, maxY));
            Vector2 value = new Vector2(valueX, valueY);
            Vector2 expectedResult = new Vector2(Mathf.Min(range.Min.x, range.Max.x), Mathf.Min(range.Min.y, range.Max.y));

            Vector2 clampedValue = range.Clamp(value);

            Assert.IsTrue(TestHelper.EqualsApproximately(clampedValue, expectedResult), $"Result: {clampedValue}");
        }

        #endregion

        #region Contains

        [Test]
        public void Contains_ValueInsideOfRange_ReturnsTrue([ValueSource(nameof(TestValues))] Vector2 min, [ValueSource(nameof(TestValues))] Vector2 max)
        {
            Range<Vector2> range = new Range<Vector2>(min, max);
            Vector2 value = new Vector2(TestHelper.GenerateRandomValueInRange(random, min.x, max.x), TestHelper.GenerateRandomValueInRange(random, min.y, max.y));

            bool contains = range.Contains(value);

            Assert.IsTrue(contains, $"Tested value: {value}");
        }

        [Test]
        [TestCase(0, 0, 0, 0, 1, 1)]
        [TestCase(1, 5, 5, 1, 10, 0)]
        [TestCase(10, 50.5f, -5, -1, -1, 1)]
        [TestCase(5, 1, 0.1f, 0.5f, 0, 0)]
        [TestCase(-100.9f, -10, 100.9f, 10f, -101, -101)]
        [TestCase(1, 2, 100, -1, 3, 1000.1f)]
        public void Contains_ValueOutsideOfRange_ReturnsFalse(float minX, float maxX, float minY, float maxY, float valueX, float valueY)
        {
            Range<Vector2> range = new Range<Vector2>(new Vector2(minX, minY), new Vector2(maxX, maxY));
            Vector2 valueOutsideRange = new Vector2(valueX, valueY);

            bool contains = range.Contains(valueOutsideRange);

            Assert.IsFalse(contains, $"Tested value: {valueOutsideRange}");
        }

        [Test]
        public void Contains_ValueAtMinBoundaryOfRange_ReturnsTrue([ValueSource(nameof(TestValues))] Vector2 min, [ValueSource(nameof(TestValues))] Vector2 max)
        {
            Range<Vector2> range = new Range<Vector2>(min, max);

            bool contains = range.Contains(min);

            Assert.IsTrue(contains, $"Tested value: {min}");
        }

        [Test]
        public void Contains_ValueAtMaxBoundaryOfRange_ReturnsTrue([ValueSource(nameof(TestValues))] Vector2 min, [ValueSource(nameof(TestValues))] Vector2 max)
        {
            Range<Vector2> range = new Range<Vector2>(min, max);

            bool contains = range.Contains(max);

            Assert.IsTrue(contains, $"Tested value: {max}");
        }

        #endregion

        #region Center

        [Test]
        public void Center_RangeWithMinEqualToMax_ReturnsMinValue([ValueSource(nameof(ValuesForEmptyRange))] Vector2 minMax)
        {
            Range<Vector2> range = new Range<Vector2>(minMax, minMax);

            Vector2 center = range.Center();

            Assert.IsTrue(TestHelper.EqualsApproximately(center, range.Min), $"Result: {center}");
        }

        [Test]
        [TestCase(5.5f, 0, 100, -10, 2.75f, 45)]
        [TestCase(-5, -99.9f, 0, 5, -52.45f, 2.5f)]
        [TestCase(-99, 150, -100.9f, -10, 25.5f, -55.45f)]
        public void Center_DifferentRanges_ReturnsExpectedCenter(float minX, float maxX, float minY, float maxY, float expectedResultX, float expectedResultY)
        {
            Range<Vector2> range = new Range<Vector2>(new Vector2(minX, minY), new Vector2(maxX, maxY));
            Vector2 expectedResult = new Vector2(expectedResultX, expectedResultY);

            Vector2 center = range.Center();

            Assert.IsTrue(TestHelper.EqualsApproximately(center, expectedResult), $"Result: {center}");
        }

        [Test]
        public void Center_DifferentRanges_CenterHasEqualDistanceFromMinAndMax([ValueSource(nameof(TestValues))] Vector2 min, [ValueSource(nameof(TestValues))] Vector2 max)
        {
            Range<Vector2> range = new Range<Vector2>(min, max);

            Vector2 center = range.Center();
            Vector2 distanceToMin = center - min;
            Vector2 distanceToMax = max - center;

            Assert.IsTrue(TestHelper.EqualsApproximately(distanceToMin, distanceToMax), $"Distance to min: {distanceToMin}, distance to max: {distanceToMax}");
        }

        #endregion
    }
}