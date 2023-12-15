using System.Collections.Generic;
using NUnit.Framework;
using RangePrimitive;
using UnityEngine;

namespace Tests
{
    public class Vector2IntRangeTests
    {
        private static IEnumerable<Vector2Int> TestValues => new Vector2Int[] { new(-150, -5), new(-2, -1), new(0, 0), new(1, 1), new(3, 5), new(50, -50), new(100, 1005) };
        private static IEnumerable<Vector2Int> ValuesForEmptyRange => new Vector2Int[] { new(0, 0), new(1, 0), new(99, -99) };

        private const int RandomSeed = 128;
        private readonly System.Random random = new(RandomSeed);

        #region Reorder

        [Test]
        public void ReorderPerComponent_DifferentRanges_MinIsSmallerThanOrEqualToMax([ValueSource(nameof(TestValues))] Vector2Int min, [ValueSource(nameof(TestValues))] Vector2Int max)
        {
            Range<Vector2Int> range = new Range<Vector2Int>(min, max);

            range = range.ReorderPerComponent();

            Assert.IsTrue(range.Min.x <= range.Max.x && range.Min.y <= range.Max.y);
        }

        #endregion

        #region Random

        [Test]
        public void Random_DifferentRanges_GeneratedValueIsContainedInRange([ValueSource(nameof(TestValues))] Vector2Int min, [ValueSource(nameof(TestValues))] Vector2Int max)
        {
            Range<Vector2Int> range = new Range<Vector2Int>(min, max);

            Vector2Int value = range.Random();

            Assert.IsTrue(range.Contains(value), $"Generated value: {value}");
        }

        #endregion

        #region Lerp

        [Test]
        public void Lerp_FactorOne_ReturnsMax([ValueSource(nameof(TestValues))] Vector2Int min, [ValueSource(nameof(TestValues))] Vector2Int max)
        {
            Range<Vector2Int> range = new Range<Vector2Int>(min, max);

            Vector2 result = range.Lerp(1);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, max), $"Result: {result}");
        }

        [Test]
        public void Lerp_FactorZero_ReturnsMin([ValueSource(nameof(TestValues))] Vector2Int min, [ValueSource(nameof(TestValues))] Vector2Int max)
        {
            Range<Vector2Int> range = new Range<Vector2Int>(min, max);

            Vector2 result = range.Lerp(0);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, min), $"Result: {result}");
        }

        [Test]
        [TestCase(0, 0, 0)]
        [TestCase(1, 0, 0.5f)]
        [TestCase(99, -99, 1)]
        public void Lerp_RangeWithMinEqualToMax_ReturnsMin(int minMaxX, int minMaxY, float t)
        {
            Vector2Int minMax = new Vector2Int(minMaxX, minMaxY);
            Range<Vector2Int> range = new Range<Vector2Int>(minMax, minMax);

            Vector2 result = range.Lerp(t);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, minMax), $"Result: {result}");
        }

        [Test]
        [TestCase(99, -5, 99, -5, 0.25f, 73, 73)]
        [TestCase(1, 2, -2, -1, 0.25f, 1.25f, -1.75f)]
        [TestCase(2, 1, 50, 5, 0.5f, 1.5f, 27.5f)]
        [TestCase(1, 2, 1, 2, 0.5f, 1.5f, 1.5f)]
        [TestCase(-10, -192, 192, 10, 0.75f, -146.5f, 55.5f)]
        [TestCase(120, 194, 50, 99, 0.81f, 179.94f, 89.69f)]
        public void Lerp_DifferentRanges_ReturnsExpectedResult(int minX, int maxX, int minY, int maxY, float t, float expectedResultX, float expectedResultY)
        {
            Range<Vector2Int> range = new Range<Vector2Int>(new Vector2Int(minX, minY), new Vector2Int(maxX, maxY));
            Vector2 expectedResult = new Vector2(expectedResultX, expectedResultY);

            Vector2 result = range.Lerp(t);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, expectedResult), $"Result: {result}");
        }

        [Test]
        [TestCase(0, 1, -1, -5, -1)]
        [TestCase(99, 5, 99, 5, -2)]
        [TestCase(-5, -1, 100, -1, -0.5f)]
        public void Lerp_ValueSmallerThanZero_ReturnsMin(int minX, int maxX, int minY, int maxY, float t)
        {
            Range<Vector2Int> range = new Range<Vector2Int>(new Vector2Int(minX, minY), new Vector2Int(maxX, maxY));

            Vector2 result = range.Lerp(t);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, range.Min), $"Result: {result}");
        }

        [Test]
        [TestCase(0, 1, -1, -5, 1.5f)]
        [TestCase(99, 5, 99, 5, 2)]
        [TestCase(-5, -1, 100, -1, 99.9f)]
        public void Lerp_ValueLargerThanOne_ReturnsMax(int minX, int maxX, int minY, int maxY, float t)
        {
            Range<Vector2Int> range = new Range<Vector2Int>(new Vector2Int(minX, minY), new Vector2Int(maxX, maxY));

            Vector2 result = range.Lerp(t);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, range.Max), $"Result: {result}");
        }

        #endregion

        #region InverseLerp

        [Test]
        [TestCase(0, 1, -1, 0)]
        [TestCase(-5, 99, -5, 99)]
        [TestCase(-105, 1, 99, 5)]
        public void InverseLerp_MaxValue_ReturnsOne(int minX, int maxX, int minY, int maxY)
        {
            Range<Vector2Int> range = new Range<Vector2Int>(new Vector2Int(minX, minY), new Vector2Int(maxX, maxY));

            Vector2 result = range.InverseLerp(range.Max);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, Vector2.one), $"Result: {result}");
        }

        [Test]
        public void InverseLerp_MinValue_ReturnsZero([ValueSource(nameof(TestValues))] Vector2Int min, [ValueSource(nameof(TestValues))] Vector2Int max)
        {
            Range<Vector2Int> range = new Range<Vector2Int>(min, max);

            Vector2 result = range.InverseLerp(min);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, Vector2.zero), $"Result: {result}");
        }

        [Test]
        [TestCase(0, 0, 0, 0)]
        [TestCase(1, 0, 1, 0)]
        [TestCase(99, -99, 100, -5.5f)]
        public void InverseLerp_RangeWithMinEqualToMax_ReturnsZero(int minMaxX, int minMaxY, float valueX, float valueY)
        {
            Vector2Int minMax = new Vector2Int(minMaxX, minMaxY);
            Range<Vector2Int> range = new Range<Vector2Int>(minMax, minMax);
            Vector2 value = new Vector2(valueX, valueY);

            Vector2 result = range.InverseLerp(value);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, Vector2.zero), $"Result: {result}");
        }

        [Test]
        [TestCase(99, -5, 99, -5, 73, 73f, 0.25f)]
        [TestCase(1, 2, -2, -1, 1.25f, -1.75f, 0.25f)]
        [TestCase(2, 1, 1, 2, 1.5f, 1.5f, 0.5f)]
        [TestCase(1, 2, 1, 2, 1.5f, 1.5f, 0.5f)]
        [TestCase(-10, -192, 50, 5, -146.5f, 16.25f, 0.75f)]
        [TestCase(120, 194, 50, 99, 179.94f, 89.69f, 0.81f)]
        public void InverseLerp_DifferentRanges_ReturnsExpectedResult(int minX, int maxX, int minY, int maxY, float valueX, float valueY, float expectedResult)
        {
            Range<Vector2Int> range = new Range<Vector2Int>(new Vector2Int(minX, minY), new Vector2Int(maxX, maxY));
            Vector2 value = new Vector2(valueX, valueY);

            Vector2 result = range.InverseLerp(value);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, Vector2.one * expectedResult), $"Result: {result}");
        }

        [Test]
        [TestCase(10, 50, 50, 10, 100, 100, 1, 0)]
        [TestCase(-99, -50, 1, 3, 100, 100, 1, 1)]
        [TestCase(-100, -10, 100, 10, -110, -110, 0, 1)]
        [TestCase(100, 150, -1, -5, -1, 0, 0, 0)]
        public void InverseLerp_ValueOutsideRange_ReturnsCloserBoundary(int minX, int maxX, int minY, int maxY, float valueX, float valueY, float expectedResultX, float expectedResultY)
        {
            Range<Vector2Int> range = new Range<Vector2Int>(new Vector2Int(minX, minY), new Vector2Int(maxX, maxY));
            Vector2 expectedResult = new Vector2(expectedResultX, expectedResultY);
            Vector2 value = new Vector2(valueX, valueY);

            Vector2 result = range.InverseLerp(value);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, expectedResult), $"Result: {result}");
        }

        #endregion

        #region SmoothStep

        [Test]
        public void SmoothStep_FactorOne_ReturnsMax([ValueSource(nameof(TestValues))] Vector2Int min, [ValueSource(nameof(TestValues))] Vector2Int max)
        {
            Range<Vector2Int> range = new Range<Vector2Int>(min, max);

            Vector2 result = range.SmoothStep(1);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, max), $"Result: {result}");
        }

        [Test]
        public void SmoothStep_FactorZero_ReturnsMin([ValueSource(nameof(TestValues))] Vector2Int min, [ValueSource(nameof(TestValues))] Vector2Int max)
        {
            Range<Vector2Int> range = new Range<Vector2Int>(min, max);

            Vector2 result = range.SmoothStep(0);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, min), $"Result: {result}");
        }

        [Test]
        [TestCase(0, 0, 0)]
        [TestCase(-5, 5, 0.25f)]
        [TestCase(99, -99, 0.5f)]
        [TestCase(1, 0, 1)]
        public void SmoothStep_RangeWithMinEqualToMax_ReturnsMin(int minMaxX, int minMaxY, float t)
        {
            Vector2Int minMax = new Vector2Int(minMaxX, minMaxY);
            Range<Vector2Int> range = new Range<Vector2Int>(minMax, minMax);

            Vector2 result = range.SmoothStep(t);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, minMax), $"Result: {result}");
        }

        [Test]
        [TestCase(1, 0, 1, 2, 0.1f, 0.972f, 1.028f)]
        [TestCase(1, 5, 1, 5, 0.25f, 1.625f, 1.625f)]
        [TestCase(100, 5, -100, -5, 0.5f, 52.5f, -52.5f)]
        [TestCase(-10, 10, -10, 10, 0.75f, 6.875f, 6.875f)]
        [TestCase(50, 5, 190, 580, 0.9f, 6.26f, 569.08f)]
        public void SmoothStep_DifferentRanges_ReturnsExpectedResult(int minX, int maxX, int minY, int maxY, float t, float expectedResultX, float expectedResultY)
        {
            Range<Vector2Int> range = new Range<Vector2Int>(new Vector2Int(minX, minY), new Vector2Int(maxX, maxY));
            Vector2 expectedResult = new Vector2(expectedResultX, expectedResultY);

            Vector2 result = range.SmoothStep(t);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, expectedResult), $"Result: {result}");
        }

        [Test]
        [TestCase(0, 1, -1, -5, -1)]
        [TestCase(99, 5, 99, 5, -2)]
        [TestCase(-5, -1, 100, -1, -0.5f)]
        public void SmoothStep_ValueSmallerThanZero_ReturnsMin(int minX, int maxX, int minY, int maxY, float t)
        {
            Range<Vector2Int> range = new Range<Vector2Int>(new Vector2Int(minX, minY), new Vector2Int(maxX, maxY));

            Vector2 result = range.SmoothStep(t);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, range.Min), $"Result: {result}");
        }

        [Test]
        [TestCase(0, 1, -1, -5, 1)]
        [TestCase(99, 5, 99, 5, 2)]
        [TestCase(-5, -1, 100, -1, 1.5f)]
        public void SmoothStep_ValueLargerThanOne_ReturnsMax(int minX, int maxX, int minY, int maxY, float t)
        {
            Range<Vector2Int> range = new Range<Vector2Int>(new Vector2Int(minX, minY), new Vector2Int(maxX, maxY));

            Vector2 result = range.SmoothStep(t);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, range.Max), $"Result: {result}");
        }

        #endregion

        #region Delta

        [Test]
        public void Delta_RangeWithMinEqualToMax_ReturnsZero([ValueSource(nameof(ValuesForEmptyRange))] Vector2Int minMax)
        {
            Range<Vector2Int> range = new Range<Vector2Int>(minMax, minMax);

            Vector2Int result = range.Delta();

            Assert.IsTrue(result == Vector2Int.zero, $"Result: {result}");
        }

        [Test]
        [TestCase(2, 1, 1, 2, -1, 1)]
        [TestCase(-10, 10, 100, 5, 20, -95)]
        [TestCase(-150, -580, -150, -100, -430, 50)]
        public void Delta_DifferentRanges_ReturnsExpectedValue(int minX, int maxX, int minY, int maxY, int expectedResultX, int expectedResultY)
        {
            Range<Vector2Int> range = new Range<Vector2Int>(new Vector2Int(minX, minY), new Vector2Int(maxX, maxY));
            Vector2Int expectedResult = new Vector2Int(expectedResultX, expectedResultY);

            Vector2Int result = range.Delta();

            Assert.IsTrue(result == expectedResult, $"Result: {result}");
        }

        #endregion

        #region Size

        [Test]
        public void Size_RangeWithMinEqualToMax_ReturnsZero([ValueSource(nameof(ValuesForEmptyRange))] Vector2Int minMax)
        {
            Range<Vector2Int> range = new Range<Vector2Int>(minMax, minMax);

            Vector2Int result = range.Size();

            Assert.IsTrue(result == Vector2Int.zero, $"Result: {result}");
        }

        [Test]
        [TestCase(2, 1, 1, 2, 1, 1)]
        [TestCase(100, -5, -10, 10, 105, 20)]
        [TestCase(-580, -150, -50, -99, 430, 49)]
        public void Size_DifferentRanges_ReturnsExpectedResult(int minX, int maxX, int minY, int maxY, int expectedResultX, int expectedResultY)
        {
            Range<Vector2Int> range = new Range<Vector2Int>(new Vector2Int(minX, minY), new Vector2Int(maxX, maxY));
            Vector2Int expectedResult = new Vector2Int(expectedResultX, expectedResultY);

            Vector2Int result = range.Size();

            Assert.IsTrue(result == expectedResult, $"Result: {result}");
        }

        #endregion

        #region Clamp

        [Test]
        [TestCase(0, 0, 0, 0, 0, 0)]
        [TestCase(1, 2, 2, 1, 2, 2)]
        [TestCase(1, 2, 1, 2, 1, 1)]
        [TestCase(200, -1, -5, 5, 2, 4)]
        [TestCase(-10, -192, 5, 99, -120, 50)]
        [TestCase(120, 194, -120, -194, 186, -186)]
        public void Clamp_Vector2IntValueInRange_ReturnsUnchangedValue(int minX, int maxX, int minY, int maxY, int valueX, int valueY)
        {
            Range<Vector2Int> range = new Range<Vector2Int>(new Vector2Int(minX, minY), new Vector2Int(maxX, maxY));
            Vector2Int value = new Vector2Int(valueX, valueY);

            Vector2Int clampedValue = range.Clamp(value);

            Assert.IsTrue(clampedValue == value, $"Result: {clampedValue}");
        }
        
        [Test]
        [TestCase(0, 0, 0, 0, 0, 0)]
        [TestCase(1, 2, 2, 1, 1.5f, 1.5f)]
        [TestCase(1, 2, 1, 2, 1.5f, 1)]
        [TestCase(200, -1, -5, 5, 2, 4.1f)]
        [TestCase(-10, -192, 5, 99, -120, 50)]
        [TestCase(120, 194, -120, -194, 186.8f, -186.9f)]
        public void Clamp_Vector2ValueInRange_ReturnsUnchangedValue(int minX, int maxX, int minY, int maxY, float valueX, float valueY)
        {
            Range<Vector2Int> range = new Range<Vector2Int>(new Vector2Int(minX, minY), new Vector2Int(maxX, maxY));
            Vector2 value = new Vector2(valueX, valueY);

            Vector2 clampedValue = range.Clamp(value);

            Assert.IsTrue(TestHelper.EqualsApproximately(clampedValue, value), $"Result: {clampedValue}");
        }

        [Test]
        [TestCase(0, 0, 5, -5)]
        [TestCase(-5, 5, 99, 99)]
        [TestCase(99, -99, 0, 50)]
        [TestCase(1, 0, 100, 1)]
        public void Clamp_RangeWithMinEqualToMaxAndVector2IntValue_ReturnsMin(int minMaxX, int minMaxY, int valueX, int valueY)
        {
            Vector2Int minMax = new Vector2Int(minMaxX, minMaxY);
            Range<Vector2Int> range = new Range<Vector2Int>(minMax, minMax);
            Vector2Int value = new Vector2Int(valueX, valueY);

            Vector2Int clampedValue = range.Clamp(value);

            Assert.IsTrue(clampedValue == range.Min, $"Result: {clampedValue}");
        }
        
        [Test]
        [TestCase(0, 0, 5, -5)]
        [TestCase(-5, 5, 99.9f, 99.9f)]
        [TestCase(99, -99, 0, 50.5f)]
        [TestCase(1, 0, 100.1f, 1)]
        public void Clamp_RangeWithMinEqualToMaxAndVector2Value_ReturnsMin(int minMaxX, int minMaxY, float valueX, float valueY)
        {
            Vector2Int minMax = new Vector2Int(minMaxX, minMaxY);
            Range<Vector2Int> range = new Range<Vector2Int>(minMax, minMax);
            Vector2 value = new Vector2(valueX, valueY);

            Vector2 clampedValue = range.Clamp(value);

            Assert.IsTrue(TestHelper.EqualsApproximately(clampedValue, range.Min), $"Result: {clampedValue}");
        }

        [Test]
        [TestCase(1, 5, 1, 5, 100, 10)]
        [TestCase(100, -1, -5, 99, 1000, 100)]
        [TestCase(-5, 10, 0, 0, 99, 1)]
        [TestCase(-100, -99, 100, 99, -98, 101)]
        public void Clamp_Vector2IntValueLargerThanMax_ReturnsMax(int minX, int maxX, int minY, int maxY, int valueX, int valueY)
        {
            Range<Vector2Int> range = new Range<Vector2Int>(new Vector2Int(minX, minY), new Vector2Int(maxX, maxY));
            Vector2Int value = new Vector2Int(valueX, valueY);
            Vector2Int expectedResult = new Vector2Int(Mathf.Max(range.Min.x, range.Max.x), Mathf.Max(range.Min.y, range.Max.y));

            Vector2Int clampedValue = range.Clamp(value);

            Assert.IsTrue(clampedValue == expectedResult, $"Result: {clampedValue}");
        }
        
        [Test]
        [TestCase(1, 5, 1, 5, 100, 10)]
        [TestCase(100, -1, -5, 99, 1000.1f, 100.1f)]
        [TestCase(-5, 10, 0, 0, 99.9f, 1)]
        [TestCase(-100, -99, 100, 99, -98, 101.5f)]
        public void Clamp_Vector2ValueLargerThanMax_ReturnsMax(int minX, int maxX, int minY, int maxY, float valueX, float valueY)
        {
            Range<Vector2Int> range = new Range<Vector2Int>(new Vector2Int(minX, minY), new Vector2Int(maxX, maxY));
            Vector2 value = new Vector2(valueX, valueY);
            Vector2Int expectedResult = new Vector2Int(Mathf.Max(range.Min.x, range.Max.x), Mathf.Max(range.Min.y, range.Max.y));

            Vector2 clampedValue = range.Clamp(value);

            Assert.IsTrue(TestHelper.EqualsApproximately(clampedValue, expectedResult), $"Result: {clampedValue}");
        }

        [Test]
        [TestCase(1, 5, 1, 5, -10, -10)]
        [TestCase(10, 50, 50, 10, -1, -1)]
        [TestCase(5, 1, 99, -99, 0, -110)]
        [TestCase(-100, -10, 5, 5, -101, -5)]
        [TestCase(100, -1, 0, 0, -5, 0)]
        public void Clamp_Vector2IntValueSmallerThanMin_ReturnsMin(int minX, int maxX, int minY, int maxY, int valueX, int valueY)
        {
            Range<Vector2Int> range = new Range<Vector2Int>(new Vector2Int(minX, minY), new Vector2Int(maxX, maxY));
            Vector2Int value = new Vector2Int(valueX, valueY);
            Vector2Int expectedResult = new Vector2Int(Mathf.Min(range.Min.x, range.Max.x), Mathf.Min(range.Min.y, range.Max.y));

            Vector2Int clampedValue = range.Clamp(value);

            Assert.IsTrue(clampedValue == expectedResult, $"Result: {clampedValue}");
        }
        
        [Test]
        [TestCase(1, 5, 1, 5, -10, -10)]
        [TestCase(10, 50, 50, 10, -1.5f, -1.5f)]
        [TestCase(5, 1, 99, -99, 0.1f, -110)]
        [TestCase(-100, -10, 5, 5, -101, -5.5f)]
        [TestCase(100, -1, 0, 0, -5, 0.9f)]
        public void Clamp_Vector2ValueSmallerThanMin_ReturnsMin(int minX, int maxX, int minY, int maxY, float valueX, float valueY)
        {
            Range<Vector2Int> range = new Range<Vector2Int>(new Vector2Int(minX, minY), new Vector2Int(maxX, maxY));
            Vector2 value = new Vector2(valueX, valueY);
            Vector2Int expectedResult = new Vector2Int(Mathf.Min(range.Min.x, range.Max.x), Mathf.Min(range.Min.y, range.Max.y));

            Vector2 clampedValue = range.Clamp(value);

            Assert.IsTrue(TestHelper.EqualsApproximately(clampedValue, expectedResult), $"Result: {clampedValue}");
        }

        #endregion

        #region Contains

        [Test]
        public void Contains_Vector2IntValueInsideOfRange_ReturnsTrue([ValueSource(nameof(TestValues))] Vector2Int min, [ValueSource(nameof(TestValues))] Vector2Int max)
        {
            Range<Vector2Int> range = new Range<Vector2Int>(min, max);
            Vector2Int value = new Vector2Int(TestHelper.GenerateRandomValueInRange(random, min.x, max.x), TestHelper.GenerateRandomValueInRange(random, min.y, max.y));
            
            bool contains = range.Contains(value);

            Assert.IsTrue(contains, $"Tested value: {value}");
        }
        
        [Test]
        public void Contains_Vector2ValueInsideOfRange_ReturnsTrue([ValueSource(nameof(TestValues))] Vector2Int min, [ValueSource(nameof(TestValues))] Vector2Int max)
        {
            Range<Vector2Int> range = new Range<Vector2Int>(min, max);
            Vector2 value = new Vector2(TestHelper.GenerateRandomValueInRange(random, (float) min.x, max.x), TestHelper.GenerateRandomValueInRange(random, (float) min.y, max.y));

            bool contains = range.Contains(value);

            Assert.IsTrue(contains, $"Tested value: {value}");
        }

        [Test]
        [TestCase(0, 0, 0, 0, 1, 1)]
        [TestCase(1, 5, 5, 1, 10, 0)]
        [TestCase(10, 50, -5, -1, -1, 1)]
        [TestCase(5, 1, 0, 0, 0, 0)]
        [TestCase(-100, -10, 100, 10, -101, -101)]
        [TestCase(1, 2, 100, -1, 3, 1000)]
        public void Contains_Vector2IntValueOutsideOfRange_ReturnsFalse(int minX, int maxX, int minY, int maxY, int valueX, int valueY)
        {
            Range<Vector2Int> range = new Range<Vector2Int>(new Vector2Int(minX, minY), new Vector2Int(maxX, maxY));
            Vector2Int valueOutsideRange = new Vector2Int(valueX, valueY);

            bool contains = range.Contains(valueOutsideRange);

            Assert.IsFalse(contains, $"Tested value: {valueOutsideRange}");
        }
        
        [Test]
        [TestCase(0, 0, 0, 0, 1, 1)]
        [TestCase(1, 5, 5, 1, 10.5f, 0.5f)]
        [TestCase(10, 50, -5, -1, -1, 1)]
        [TestCase(5, 1, 0, 0, 0, 0)]
        [TestCase(-100, -10, 100, 10, -101.1f, -101.9f)]
        [TestCase(1, 2, 100, -1, 3.5f, 1000)]
        public void Contains_Vector2ValueOutsideOfRange_ReturnsFalse(int minX, int maxX, int minY, int maxY, float valueX, float valueY)
        {
            Range<Vector2Int> range = new Range<Vector2Int>(new Vector2Int(minX, minY), new Vector2Int(maxX, maxY));
            Vector2 valueOutsideRange = new Vector2(valueX, valueY);

            bool contains = range.Contains(valueOutsideRange);

            Assert.IsFalse(contains, $"Tested value: {valueOutsideRange}");
        }

        [Test]
        public void Contains_ValueAtMinBoundaryOfRange_ReturnsTrue([ValueSource(nameof(TestValues))] Vector2Int min, [ValueSource(nameof(TestValues))] Vector2Int max)
        {
            Range<Vector2Int> range = new Range<Vector2Int>(min, max);

            bool contains = range.Contains(min);

            Assert.IsTrue(contains, $"Tested value: {min}");
        }
        
        [Test]
        public void Contains_Vector2ValueAtMinBoundaryOfRange_ReturnsTrue([ValueSource(nameof(TestValues))] Vector2Int min, [ValueSource(nameof(TestValues))] Vector2Int max)
        {
            Range<Vector2Int> range = new Range<Vector2Int>(min, max);

            bool contains = range.Contains((Vector2) min);

            Assert.IsTrue(contains, $"Tested value: {min}");
        }

        [Test]
        public void Contains_ValueAtMaxBoundaryOfRange_ReturnsTrue([ValueSource(nameof(TestValues))] Vector2Int min, [ValueSource(nameof(TestValues))] Vector2Int max)
        {
            Range<Vector2Int> range = new Range<Vector2Int>(min, max);

            bool contains = range.Contains(max);

            Assert.IsTrue(contains, $"Tested value: {max}");
        }
        
        [Test]
        public void Contains_Vector2ValueAtMaxBoundaryOfRange_ReturnsTrue([ValueSource(nameof(TestValues))] Vector2Int min, [ValueSource(nameof(TestValues))] Vector2Int max)
        {
            Range<Vector2Int> range = new Range<Vector2Int>(min, max);

            bool contains = range.Contains((Vector2) max);

            Assert.IsTrue(contains, $"Tested value: {max}");
        }

        #endregion

        #region Center

        [Test]
        public void Center_RangeWithMinEqualToMax_ReturnsMinValue([ValueSource(nameof(ValuesForEmptyRange))] Vector2Int minMax)
        {
            Range<Vector2Int> range = new Range<Vector2Int>(minMax, minMax);

            Vector2 center = range.Center();

            Assert.IsTrue(TestHelper.EqualsApproximately(center, range.Min), $"Result: {center}");
        }

        [Test]
        [TestCase(5, 0, 100, -10, 2.5f, 45)]
        [TestCase(-5, -99, 0, 5, -52, 2.5f)]
        [TestCase(-99, 150, -100, -10, 25.5f, -55)]
        public void Center_DifferentRanges_ReturnsExpectedCenter(int minX, int maxX, int minY, int maxY, float expectedResultX, float expectedResultY)
        {
            Range<Vector2Int> range = new Range<Vector2Int>(new Vector2Int(minX, minY), new Vector2Int(maxX, maxY));
            Vector2 expectedResult = new Vector2(expectedResultX, expectedResultY);

            Vector2 center = range.Center();

            Assert.IsTrue(TestHelper.EqualsApproximately(center, expectedResult), $"Result: {center}");
        }

        [Test]
        public void Center_DifferentRanges_CenterHasEqualDistanceFromMinAndMax([ValueSource(nameof(TestValues))] Vector2Int min, [ValueSource(nameof(TestValues))] Vector2Int max)
        {
            Range<Vector2Int> range = new Range<Vector2Int>(min, max);

            Vector2 center = range.Center();
            Vector2 distanceToMin = center - min;
            Vector2 distanceToMax = max - center;

            Assert.IsTrue(TestHelper.EqualsApproximately(distanceToMin, distanceToMax), $"Distance to min: {distanceToMin}, distance to max: {distanceToMax}");
        }

        #endregion
    }
}