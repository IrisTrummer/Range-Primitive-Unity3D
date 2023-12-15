using System.Collections.Generic;
using NUnit.Framework;
using RangePrimitive;
using UnityEngine;

namespace Tests
{
    public class Vector3IntRangeTests
    {
        private static IEnumerable<Vector3Int> TestValues => new Vector3Int[] { new(-150, -5, -1), new(-2, -1, 0), new(0, 0, 0), new(1, 1, 1), new(3, 5, 10), new(50, -50, -50), new(10, 100, 1005) };
        private static IEnumerable<Vector3Int> ValuesForEmptyRange => new Vector3Int[] { new(0, 0, 0), new(1, 0, -1), new(99, -99, 99) };

        private const int RandomSeed = 128;
        private readonly System.Random random = new(RandomSeed);

        #region Reorder

        [Test]
        public void ReorderPerComponent_DifferentRanges_MinIsSmallerThanOrEqualToMax([ValueSource(nameof(TestValues))] Vector3Int min, [ValueSource(nameof(TestValues))] Vector3Int max)
        {
            Range<Vector3Int> range = new Range<Vector3Int>(min, max);

            range = range.ReorderPerComponent();

            Assert.IsTrue(range.Min.x <= range.Max.x && range.Min.y <= range.Max.y && range.Min.z <= range.Max.z);
        }

        #endregion

        #region Random

        [Test]
        public void Random_DifferentRanges_GeneratedValueIsContainedInRange([ValueSource(nameof(TestValues))] Vector3Int min, [ValueSource(nameof(TestValues))] Vector3Int max)
        {
            Range<Vector3Int> range = new Range<Vector3Int>(min, max);

            Vector3Int value = range.Random();

            Assert.IsTrue(range.Contains(value), $"Generated value: {value}");
        }

        #endregion

        #region Lerp

        [Test]
        public void Lerp_FactorOne_ReturnsMax([ValueSource(nameof(TestValues))] Vector3Int min, [ValueSource(nameof(TestValues))] Vector3Int max)
        {
            Range<Vector3Int> range = new Range<Vector3Int>(min, max);

            Vector3 result = range.Lerp(1);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, max), $"Result: {result}");
        }

        [Test]
        public void Lerp_FactorZero_ReturnsMin([ValueSource(nameof(TestValues))] Vector3Int min, [ValueSource(nameof(TestValues))] Vector3Int max)
        {
            Range<Vector3Int> range = new Range<Vector3Int>(min, max);

            Vector3 result = range.Lerp(0);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, min), $"Result: {result}");
        }

        [Test]
        [TestCase(0, 0, 0, 0)]
        [TestCase(1, 0, -1, 0.5f)]
        [TestCase(99, -99, 99, 1)]
        public void Lerp_RangeWithMinEqualToMax_ReturnsMin(int minMaxX, int minMaxY, int minMaxZ, float t)
        {
            Vector3Int minMax = new Vector3Int(minMaxX, minMaxY, minMaxZ);
            Range<Vector3Int> range = new Range<Vector3Int>(minMax, minMax);

            Vector3 result = range.Lerp(t);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, minMax), $"Result: {result}");
        }

        [Test]
        [TestCase(99, -5, 99, -5, 99, -5, 0.25f, 73, 73, 73)]
        [TestCase(1, 2, -2, -1, 1, -2, 0.25f, 1.25f, -1.75f, 0.25f)]
        [TestCase(2, 1, 50, 5, 102, 51, 0.5f, 1.5f, 27.5f, 76.5f)]
        [TestCase(1, 2, 1, 2, 1, 2, 0.5f, 1.5f, 1.5f, 1.5f)]
        [TestCase(-10, -192, 192, 10, -192, 10, 0.75f, -146.5f, 55.5f, -40.5f)]
        [TestCase(120, 194, 50, 99, 5, -99, 0.81f, 179.94f, 89.69f, -79.24f)]
        public void Lerp_DifferentRanges_ReturnsExpectedResult(int minX, int maxX, int minY, int maxY, int minZ, int maxZ, float t, float expectedResultX, float expectedResultY, float expectedResultZ)
        {
            Range<Vector3Int> range = new Range<Vector3Int>(new Vector3Int(minX, minY, minZ), new Vector3Int(maxX, maxY, maxZ));
            Vector3 expectedResult = new Vector3(expectedResultX, expectedResultY, expectedResultZ);

            Vector3 result = range.Lerp(t);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, expectedResult), $"Result: {result}");
        }

        [Test]
        [TestCase(0, 1, -1, -5, 1, 5, -1)]
        [TestCase(99, 5, 99, 5, 99, 5, -2)]
        [TestCase(-5, -1, 100, -1, 5, 99, -0.5f)]
        public void Lerp_ValueSmallerThanZero_ReturnsMin(int minX, int maxX, int minY, int maxY, int minZ, int maxZ, float t)
        {
            Range<Vector3Int> range = new Range<Vector3Int>(new Vector3Int(minX, minY, minZ), new Vector3Int(maxX, maxY, maxZ));

            Vector3 result = range.Lerp(t);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, range.Min), $"Result: {result}");
        }

        [Test]
        [TestCase(0, 1, -1, -5, 5, 1, 1.5f)]
        [TestCase(99, 5, 99, 5, 99, 5, 2)]
        [TestCase(-5, -1, 100, -1, 99, 101, 99.9f)]
        public void Lerp_ValueLargerThanOne_ReturnsMax(int minX, int maxX, int minY, int maxY, int minZ, int maxZ, float t)
        {
            Range<Vector3Int> range = new Range<Vector3Int>(new Vector3Int(minX, minY, minZ), new Vector3Int(maxX, maxY, maxZ));

            Vector3 result = range.Lerp(t);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, range.Max), $"Result: {result}");
        }

        #endregion

        #region InverseLerp

        [Test]
        [TestCase(0, 1, -1, 0, 1, -1)]
        [TestCase(-5, 99, -5, 99, -5, 99)]
        [TestCase(-105, 1, 99, 5, 3, -99)]
        public void InverseLerp_MaxValue_ReturnsOne(int minX, int maxX, int minY, int maxY, int minZ, int maxZ)
        {
            Range<Vector3Int> range = new Range<Vector3Int>(new Vector3Int(minX, minY, minZ), new Vector3Int(maxX, maxY, maxZ));

            Vector3 result = range.InverseLerp(range.Max);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, Vector3.one), $"Result: {result}");
        }

        [Test]
        public void InverseLerp_MinValue_ReturnsZero([ValueSource(nameof(TestValues))] Vector3Int min, [ValueSource(nameof(TestValues))] Vector3Int max)
        {
            Range<Vector3Int> range = new Range<Vector3Int>(min, max);

            Vector3 result = range.InverseLerp(min);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, Vector3.zero), $"Result: {result}");
        }

        [Test]
        [TestCase(0, 0, 0, 0, 0, 0)]
        [TestCase(1, 0, 3, 1, 0, 1)]
        [TestCase(99, -99, 50, 100, -5.5f, 3)]
        public void InverseLerp_RangeWithMinEqualToMax_ReturnsZero(int minMaxX, int minMaxY, int minMaxZ, float valueX, float valueY, float valueZ)
        {
            Vector3Int minMax = new Vector3Int(minMaxX, minMaxY, minMaxZ);
            Range<Vector3Int> range = new Range<Vector3Int>(minMax, minMax);
            Vector3 value = new Vector3(valueX, valueY, valueZ);

            Vector3 result = range.InverseLerp(value);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, Vector3.zero), $"Result: {result}");
        }

        [Test]
        [TestCase(99, -5, 99, -5, 99, -5, 73, 73, 73, 0.25f)]
        [TestCase(1, 2, -2, -1, 2, -1, 1.25f, -1.75f, 1.25f, 0.25f)]
        [TestCase(2, 1, 1, 2, 1, 2, 1.5f, 1.5f, 1.5f, 0.5f)]
        [TestCase(1, 2, 1, 2, 1, 2, 1.5f, 1.5f, 1.5f, 0.5f)]
        [TestCase(-10, -192, 50, 5, 5, 99, -146.5f, 16.25f, 75.5f, 0.75f)]
        [TestCase(120, 194, 50, 99, 1, 10, 179.94f, 89.69f, 8.29f, 0.81f)]
        public void InverseLerp_DifferentRanges_ReturnsExpectedResult(int minX, int maxX, int minY, int maxY, int minZ, int maxZ, float valueX, float valueY, float valueZ, float expectedResult)
        {
            Range<Vector3Int> range = new Range<Vector3Int>(new Vector3Int(minX, minY, minZ), new Vector3Int(maxX, maxY, maxZ));
            Vector3 value = new Vector3(valueX, valueY, valueZ);

            Vector3 result = range.InverseLerp(value);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, Vector3.one * expectedResult), $"Result: {result}");
        }

        [Test]
        [TestCase(10, 50, 50, 10, -10, 50, 100, 100, 100, 1, 0, 1)]
        [TestCase(-99, -50, 1, 3, -5, 5, 100, 100, 100, 1, 1, 1)]
        [TestCase(-100, -10, 100, 10, -10, 100, -110, -110, -110, 0, 1, 0)]
        [TestCase(100, 150, -1, -5, -1, 0, -9, 0, 0, 0, 0, 1)]
        public void InverseLerp_ValueOutsideRange_ReturnsCloserBoundary(int minX, int maxX, int minY, int maxY, int minZ, int maxZ, float valueX, float valueY, float valueZ, float expectedResultX, float expectedResultY, float expectedResultZ)
        {
            Range<Vector3Int> range = new Range<Vector3Int>(new Vector3Int(minX, minY, minZ), new Vector3Int(maxX, maxY, maxZ));
            Vector3 expectedResult = new Vector3(expectedResultX, expectedResultY, expectedResultZ);
            Vector3 value = new Vector3(valueX, valueY, valueZ);

            Vector3 result = range.InverseLerp(value);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, expectedResult), $"Result: {result}");
        }

        #endregion

        #region SmoothStep

        [Test]
        public void SmoothStep_FactorOne_ReturnsMax([ValueSource(nameof(TestValues))] Vector3Int min, [ValueSource(nameof(TestValues))] Vector3Int max)
        {
            Range<Vector3Int> range = new Range<Vector3Int>(min, max);

            Vector3 result = range.SmoothStep(1);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, max), $"Result: {result}");
        }

        [Test]
        public void SmoothStep_FactorZero_ReturnsMin([ValueSource(nameof(TestValues))] Vector3Int min, [ValueSource(nameof(TestValues))] Vector3Int max)
        {
            Range<Vector3Int> range = new Range<Vector3Int>(min, max);

            Vector3 result = range.SmoothStep(0);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, min), $"Result: {result}");
        }

        [Test]
        [TestCase(0, 0, 0, 0)]
        [TestCase(-5, 5, -3, 0.25f)]
        [TestCase(99, 0, -99, 0.5f)]
        [TestCase(1, 0, -1, 1)]
        public void SmoothStep_RangeWithMinEqualToMax_ReturnsMin(int minMaxX, int minMaxY, int minMaxZ, float t)
        {
            Vector3Int minMax = new Vector3Int(minMaxX, minMaxY, minMaxZ);
            Range<Vector3Int> range = new Range<Vector3Int>(minMax, minMax);

            Vector3 result = range.SmoothStep(t);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, minMax), $"Result: {result}");
        }

        [Test]
        [TestCase(1, 0, 1, 2, -1, 1, 0.1f, 0.972f, 1.028f, -0.944f)]
        [TestCase(1, 5, 1, 5, 1, 5, 0.25f, 1.625f, 1.625f, 1.625f)]
        [TestCase(100, 5, -100, -5, -100, 5, 0.5f, 52.5f, -52.5f, -47.5f)]
        [TestCase(-10, 10, -10, 10, 10, -10, 0.75f, 6.875f, 6.875f, -6.875f)]
        [TestCase(50, 5, 190, 580, 1, -10, 0.9f, 6.26f, 569.08f, -9.692f)]
        public void SmoothStep_DifferentRanges_ReturnsExpectedResult(int minX, int maxX, int minY, int maxY, int minZ, int maxZ, float t, float expectedResultX, float expectedResultY, float expectedResultZ)
        {
            Range<Vector3Int> range = new Range<Vector3Int>(new Vector3Int(minX, minY, minZ), new Vector3Int(maxX, maxY, maxZ));
            Vector3 expectedResult = new Vector3(expectedResultX, expectedResultY, expectedResultZ);

            Vector3 result = range.SmoothStep(t);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, expectedResult), $"Result: {result}");
        }

        [Test]
        [TestCase(0, 1, -1, -5, 5, 1, -1)]
        [TestCase(99, 5, 99, 5, 99, 5, -2)]
        [TestCase(-5, -1, 100, -1, 50, 5, -0.5f)]
        public void SmoothStep_ValueSmallerThanZero_ReturnsMin(int minX, int maxX, int minY, int maxY, int minZ, int maxZ, float t)
        {
            Range<Vector3Int> range = new Range<Vector3Int>(new Vector3Int(minX, minY, minZ), new Vector3Int(maxX, maxY, maxZ));

            Vector3 result = range.SmoothStep(t);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, range.Min), $"Result: {result}");
        }

        [Test]
        [TestCase(0, 1, -1, -5, 99, -50, 1)]
        [TestCase(99, 5, 99, 5, 99, 5, 2)]
        [TestCase(-5, -1, 100, -1, 99, 3, 1.5f)]
        public void SmoothStep_ValueLargerThanOne_ReturnsMax(int minX, int maxX, int minY, int maxY, int minZ, int maxZ, float t)
        {
            Range<Vector3Int> range = new Range<Vector3Int>(new Vector3Int(minX, minY, minZ), new Vector3Int(maxX, maxY, maxZ));

            Vector3 result = range.SmoothStep(t);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, range.Max), $"Result: {result}");
        }

        #endregion

        #region Delta

        [Test]
        public void Delta_RangeWithMinEqualToMax_ReturnsZero([ValueSource(nameof(ValuesForEmptyRange))] Vector3Int minMax)
        {
            Range<Vector3Int> range = new Range<Vector3Int>(minMax, minMax);

            Vector3Int result = range.Delta();

            Assert.IsTrue(result == Vector3Int.zero, $"Result: {result}");
        }

        [Test]
        [TestCase(2, 1, 1, 2, -1, 2, -1, 1, 3)]
        [TestCase(-10, 10, 100, 5, 5, -100, 20, -95, -105)]
        [TestCase(-150, -580, -150, -100, -3, 50, -430, 50, 53)]
        public void Delta_DifferentRanges_ReturnsExpectedValue(int minX, int maxX, int minY, int maxY, int minZ, int maxZ, int expectedResultX, int expectedResultY, int expectedResultZ)
        {
            Range<Vector3Int> range = new Range<Vector3Int>(new Vector3Int(minX, minY, minZ), new Vector3Int(maxX, maxY, maxZ));
            Vector3Int expectedResult = new Vector3Int(expectedResultX, expectedResultY, expectedResultZ);

            Vector3Int result = range.Delta();

            Assert.IsTrue(result == expectedResult, $"Result: {result}");
        }

        #endregion

        #region Size

        [Test]
        public void Size_RangeWithMinEqualToMax_ReturnsZero([ValueSource(nameof(ValuesForEmptyRange))] Vector3Int minMax)
        {
            Range<Vector3Int> range = new Range<Vector3Int>(minMax, minMax);

            Vector3Int result = range.Size();

            Assert.IsTrue(result == Vector3Int.zero, $"Result: {result}");
        }

        [Test]
        [TestCase(2, 1, 1, 2, -1, 2, 1, 1, 3)]
        [TestCase(100, -5, -10, 10, 5, -100, 105, 20, 105)]
        [TestCase(-580, -150, -50, -99, -3, 50, 430, 49, 53)]
        public void Size_DifferentRanges_ReturnsExpectedResult(int minX, int maxX, int minY, int maxY, int minZ, int maxZ, int expectedResultX, int expectedResultY, int expectedResultZ)
        {
            Range<Vector3Int> range = new Range<Vector3Int>(new Vector3Int(minX, minY, minZ), new Vector3Int(maxX, maxY, maxZ));
            Vector3Int expectedResult = new Vector3Int(expectedResultX, expectedResultY, expectedResultZ);

            Vector3Int result = range.Size();

            Assert.IsTrue(result == expectedResult, $"Result: {result}");
        }

        #endregion

        #region Clamp

        [Test]
        [TestCase(0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(1, 2, 2, 1, -1, -2, 2, 2, -2)]
        [TestCase(1, 2, 1, 2, 1, 2, 1, 1, 1)]
        [TestCase(200, -1, -5, 5, 2, 5, 10, 2, 4)]
        [TestCase(-10, -192, 5, 99, -9, 50, -120, 50, 5)]
        [TestCase(120, 194, -120, -194, 120, -194, 190, -190, 5)]
        public void Clamp_Vector3IntValueInRange_ReturnsUnchangedValue(int minX, int maxX, int minY, int maxY, int minZ, int maxZ, int valueX, int valueY, int valueZ)
        {
            Range<Vector3Int> range = new Range<Vector3Int>(new Vector3Int(minX, minY, minZ), new Vector3Int(maxX, maxY, maxZ));
            Vector3Int value = new Vector3Int(valueX, valueY, valueZ);

            Vector3Int clampedValue = range.Clamp(value);

            Assert.IsTrue(clampedValue == value, $"Result: {clampedValue}");
        }

        [Test]
        [TestCase(0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(1, 2, 2, 1, -1, -2, 2, 2, -2)]
        [TestCase(1, 2, 1, 2, 1, 2, 1, 1, 1)]
        [TestCase(200, -1, -5, 5, -0, 10, 2, 4, 0)]
        [TestCase(-10, -192, 5, 99, -9, 50, -120, 50.5f, 5)]
        [TestCase(120, 194, -120, -194, 120, -194, 186.8f, -186.8f, 5.5f)]
        public void Clamp_Vector3ValueInRange_ReturnsUnchangedValue(int minX, int maxX, int minY, int maxY, int minZ, int maxZ, float valueX, float valueY, float valueZ)
        {
            Range<Vector3Int> range = new Range<Vector3Int>(new Vector3Int(minX, minY, minZ), new Vector3Int(maxX, maxY, maxZ));
            Vector3 value = new Vector3(valueX, valueY, valueZ);

            Vector3 clampedValue = range.Clamp(value);

            Assert.IsTrue(TestHelper.EqualsApproximately(clampedValue, value), $"Result: {clampedValue}");
        }

        [Test]
        [TestCase(0, 0, 0, 5, -5, 3)]
        [TestCase(-5, 5, -5, -99, 99, 99)]
        [TestCase(99, 5, 10, -99, 0, 50)]
        [TestCase(1, 0, 5, 100, 1, 99)]
        public void Clamp_RangeWithMinEqualToMaxAndVector3IntValue_ReturnsMin(int minMaxX, int minMaxY, int minMaxZ, int valueX, int valueY, int valueZ)
        {
            Vector3Int minMax = new Vector3Int(minMaxX, minMaxY, minMaxZ);
            Range<Vector3Int> range = new Range<Vector3Int>(minMax, minMax);
            Vector3Int value = new Vector3Int(valueX, valueY, valueZ);

            Vector3Int clampedValue = range.Clamp(value);

            Assert.IsTrue(clampedValue == range.Min, $"Result: {clampedValue}");
        }

        [Test]
        [TestCase(0, 0, 0, 5.5f, -5, 3)]
        [TestCase(-5, 5, -5, -99, 99.9f, 99)]
        [TestCase(99, 5, 10, -99, 0.1f, 50.5f)]
        [TestCase(1, 0, 5, 100, 1, 99)]
        public void Clamp_RangeWithMinEqualToMaxAndVector3Value_ReturnsMin(int minMaxX, int minMaxY, int minMaxZ, float valueX, float valueY, float valueZ)
        {
            Vector3Int minMax = new Vector3Int(minMaxX, minMaxY, minMaxZ);
            Range<Vector3Int> range = new Range<Vector3Int>(minMax, minMax);
            Vector3 value = new Vector3(valueX, valueY, valueZ);

            Vector3 clampedValue = range.Clamp(value);

            Assert.IsTrue(TestHelper.EqualsApproximately(clampedValue, range.Min), $"Result: {clampedValue}");
        }

        [Test]
        [TestCase(1, 5, 1, 5, 1, 5, 100, 1000, 10)]
        [TestCase(100, -1, -5, 3, 99, -99, 1000, 10, 100)]
        [TestCase(-5, 10, 0, 0, -3, -1, 99, 1, 0)]
        [TestCase(-100, -99, 100, 99, 100, -99, -98, 101, 1000)]
        public void Clamp_Vector3IntValueLargerThanMax_ReturnsMax(int minX, int maxX, int minY, int maxY, int minZ, int maxZ, int valueX, int valueY, int valueZ)
        {
            Range<Vector3Int> range = new Range<Vector3Int>(new Vector3Int(minX, minY, minZ), new Vector3Int(maxX, maxY, maxZ));
            Vector3Int value = new Vector3Int(valueX, valueY, valueZ);
            Vector3Int expectedResult = new Vector3Int(Mathf.Max(range.Min.x, range.Max.x), Mathf.Max(range.Min.y, range.Max.y), Mathf.Max(range.Min.z, range.Max.z));

            Vector3Int clampedValue = range.Clamp(value);

            Assert.IsTrue(clampedValue == expectedResult, $"Result: {clampedValue}");
        }

        [Test]
        [TestCase(1, 5, 1, 5, 1, 5, 100, 10, 5.5f)]
        [TestCase(100, -1, -5, 99, 0, 1, 1000.1f, 100.1f, 1.1f)]
        [TestCase(-5, 10, 0, 0, -5, 5, 99.9f, 1, 10)]
        [TestCase(-100, -99, 100, 99, 100, -99, -98, 101.5f, 100.1f)]
        public void Clamp_Vector3ValueLargerThanMax_ReturnsMax(int minX, int maxX, int minY, int maxY, int minZ, int maxZ, float valueX, float valueY, float valueZ)
        {
            Range<Vector3Int> range = new Range<Vector3Int>(new Vector3Int(minX, minY, minZ), new Vector3Int(maxX, maxY, maxZ));
            Vector3 value = new Vector3(valueX, valueY, valueZ);
            Vector3Int expectedResult = new Vector3Int(Mathf.Max(range.Min.x, range.Max.x), Mathf.Max(range.Min.y, range.Max.y), Mathf.Max(range.Min.z, range.Max.z));

            Vector3 clampedValue = range.Clamp(value);

            Assert.IsTrue(TestHelper.EqualsApproximately(clampedValue, expectedResult), $"Result: {clampedValue}");
        }

        [Test]
        [TestCase(1, 5, 1, 5, 1, 5, 0, -1, -5)]
        [TestCase(100, -1, -5, 3, 99, -99, -2, -10, -100)]
        [TestCase(-5, 10, 0, 0, -3, -1, -10, -1, -5)]
        [TestCase(-100, -99, 100, 99, 100, -99, -101, 90, -1000)]
        public void Clamp_Vector3IntValueSmallerThanMin_ReturnsMin(int minX, int maxX, int minY, int maxY, int minZ, int maxZ, int valueX, int valueY, int valueZ)
        {
            Range<Vector3Int> range = new Range<Vector3Int>(new Vector3Int(minX, minY, minZ), new Vector3Int(maxX, maxY, maxZ));
            Vector3Int value = new Vector3Int(valueX, valueY, valueZ);
            Vector3Int expectedResult = new Vector3Int(Mathf.Min(range.Min.x, range.Max.x), Mathf.Min(range.Min.y, range.Max.y), Mathf.Min(range.Min.z, range.Max.z));

            Vector3Int clampedValue = range.Clamp(value);

            Assert.IsTrue(clampedValue == expectedResult, $"Result: {clampedValue}");
        }

        [Test]
        [TestCase(1, 5, 1, 5, 1, 5, -10, -10, -10)]
        [TestCase(10, 50, 50, 10, 50, -10, -1, -1, -50.5f)]
        [TestCase(5, 1, 99, -99, 0, 5, 0, -110, 0)]
        [TestCase(-100, -10, 5, 5, -5, 5, -100.1f, -5, -99)]
        [TestCase(100, -1, 0, 5, 99, 9, -5.1f, 0, 5)]
        public void Clamp_Vector3ValueSmallerThanMin_ReturnsMin(int minX, int maxX, int minY, int maxY, int minZ, int maxZ, float valueX, float valueY, float valueZ)
        {
            Range<Vector3Int> range = new Range<Vector3Int>(new Vector3Int(minX, minY, minZ), new Vector3Int(maxX, maxY, maxZ));
            Vector3 value = new Vector3(valueX, valueY, valueZ);
            Vector3Int expectedResult = new Vector3Int(Mathf.Min(range.Min.x, range.Max.x), Mathf.Min(range.Min.y, range.Max.y), Mathf.Min(range.Min.z, range.Max.z));

            Vector3 clampedValue = range.Clamp(value);

            Assert.IsTrue(TestHelper.EqualsApproximately(clampedValue, expectedResult), $"Result: {clampedValue}");
        }

        #endregion

        #region Contains

        [Test]
        public void Contains_Vector3IntValueInsideOfRange_ReturnsTrue([ValueSource(nameof(TestValues))] Vector3Int min, [ValueSource(nameof(TestValues))] Vector3Int max)
        {
            Range<Vector3Int> range = new Range<Vector3Int>(min, max);
            Vector3Int value = new Vector3Int(TestHelper.GenerateRandomValueInRange(random, min.x, max.x), TestHelper.GenerateRandomValueInRange(random, min.y, max.y), TestHelper.GenerateRandomValueInRange(random, min.z, max.z));

            bool contains = range.Contains(value);

            Assert.IsTrue(contains, $"Tested value: {value}");
        }

        [Test]
        public void Contains_Vector3ValueInsideOfRange_ReturnsTrue([ValueSource(nameof(TestValues))] Vector3Int min, [ValueSource(nameof(TestValues))] Vector3Int max)
        {
            Range<Vector3Int> range = new Range<Vector3Int>(min, max);
            Vector3Int value = new Vector3Int(TestHelper.GenerateRandomValueInRange(random, min.x, max.x), TestHelper.GenerateRandomValueInRange(random, min.y, max.y), TestHelper.GenerateRandomValueInRange(random, min.z, max.z));

            bool contains = range.Contains(value);

            Assert.IsTrue(contains, $"Tested value: {value}");
        }

        [Test]
        [TestCase(0, 0, 0, 0, 0, 0, 1, 1, 1)]
        [TestCase(1, 5, 5, 1, -1, 5, 10, 0, 10)]
        [TestCase(10, 50, -5, -1, -1, 1, -1, 1, -3)]
        [TestCase(5, 1, 0, 0, 3, 5, 0, 1, 1)]
        [TestCase(-100, -10, 100, 10, 100, -10, -101, -101, -11)]
        [TestCase(1, 2, 100, -1, 40, 99, 3, 1000, 0)]
        public void Contains_Vector3IntValueOutsideOfRange_ReturnsFalse(int minX, int maxX, int minY, int maxY, int minZ, int maxZ, int valueX, int valueY, int valueZ)
        {
            Range<Vector3Int> range = new Range<Vector3Int>(new Vector3Int(minX, minY, minZ), new Vector3Int(maxX, maxY, maxZ));
            Vector3Int valueOutsideRange = new Vector3Int(valueX, valueY);

            bool contains = range.Contains(valueOutsideRange);

            Assert.IsFalse(contains, $"Tested value: {valueOutsideRange}");
        }

        [Test]
        [TestCase(0, 0, 0, 0, 0, 0, 0.1f, 1, 0.5f)]
        [TestCase(1, 5, 5, 1, -1, 5, 10, 0, 10)]
        [TestCase(10, 50, -5, -1, -1, 1, -1, 1, -3.5f)]
        [TestCase(5, 1, 0, 0, 3, 5, 0, 1, 1)]
        [TestCase(-100, -10, 100, 10, 100, -10, -100.1f, -101, -11)]
        [TestCase(1, 2, 100, -1, 40, 99, 3, 1000, 0.5f)]
        public void Contains_Vector3ValueOutsideOfRange_ReturnsFalse(int minX, int maxX, int minY, int maxY, int minZ, int maxZ, float valueX, float valueY, float valueZ)
        {
            Range<Vector3Int> range = new Range<Vector3Int>(new Vector3Int(minX, minY, minZ), new Vector3Int(maxX, maxY, maxZ));
            Vector3 valueOutsideRange = new Vector3(valueX, valueY, valueZ);

            bool contains = range.Contains(valueOutsideRange);

            Assert.IsFalse(contains, $"Tested value: {valueOutsideRange}");
        }

        [Test]
        public void Contains_ValueAtMinBoundaryOfRange_ReturnsTrue([ValueSource(nameof(TestValues))] Vector3Int min, [ValueSource(nameof(TestValues))] Vector3Int max)
        {
            Range<Vector3Int> range = new Range<Vector3Int>(min, max);

            bool contains = range.Contains(min);

            Assert.IsTrue(contains, $"Tested value: {min}");
        }

        [Test]
        public void Contains_Vector3ValueAtMinBoundaryOfRange_ReturnsTrue([ValueSource(nameof(TestValues))] Vector3Int min, [ValueSource(nameof(TestValues))] Vector3Int max)
        {
            Range<Vector3Int> range = new Range<Vector3Int>(min, max);

            bool contains = range.Contains((Vector3) min);

            Assert.IsTrue(contains, $"Tested value: {min}");
        }

        [Test]
        public void Contains_ValueAtMaxBoundaryOfRange_ReturnsTrue([ValueSource(nameof(TestValues))] Vector3Int min, [ValueSource(nameof(TestValues))] Vector3Int max)
        {
            Range<Vector3Int> range = new Range<Vector3Int>(min, max);

            bool contains = range.Contains(max);

            Assert.IsTrue(contains, $"Tested value: {max}");
        }

        [Test]
        public void Contains_Vector3ValueAtMaxBoundaryOfRange_ReturnsTrue([ValueSource(nameof(TestValues))] Vector3Int min, [ValueSource(nameof(TestValues))] Vector3Int max)
        {
            Range<Vector3Int> range = new Range<Vector3Int>(min, max);

            bool contains = range.Contains((Vector3) max);

            Assert.IsTrue(contains, $"Tested value: {max}");
        }

        #endregion

        #region Center

        [Test]
        public void Center_RangeWithMinEqualToMax_ReturnsMinValue([ValueSource(nameof(ValuesForEmptyRange))] Vector3Int minMax)
        {
            Range<Vector3Int> range = new Range<Vector3Int>(minMax, minMax);

            Vector3 center = range.Center();

            Assert.IsTrue(TestHelper.EqualsApproximately(center, range.Min), $"Result: {center}");
        }

        [Test]
        [TestCase(5, 0, 100, -10, 1, 0,2.5f, 45, 0.5f)]
        [TestCase(-5, -99, 0, 5, -1, 1, -52, 2.5f, 0)]
        [TestCase(-99, 150, 0, -100, -100, -10, 25.5f, -50, -55)]
        public void Center_DifferentRanges_ReturnsExpectedCenter(int minX, int maxX, int minY, int maxY, int minZ, int maxZ, float expectedResultX, float expectedResultY, float expectedResultZ)
        {
            Range<Vector3Int> range = new Range<Vector3Int>(new Vector3Int(minX, minY, minZ), new Vector3Int(maxX, maxY, maxZ));
            Vector3 expectedResult = new Vector3(expectedResultX, expectedResultY, expectedResultZ);

            Vector3 center = range.Center();

            Assert.IsTrue(TestHelper.EqualsApproximately(center, expectedResult), $"Result: {center}");
        }

        [Test]
        public void Center_DifferentRanges_CenterHasEqualDistanceFromMinAndMax([ValueSource(nameof(TestValues))] Vector3Int min, [ValueSource(nameof(TestValues))] Vector3Int max)
        {
            Range<Vector3Int> range = new Range<Vector3Int>(min, max);

            Vector3 center = range.Center();
            Vector3 distanceToMin = center - min;
            Vector3 distanceToMax = max - center;

            Assert.IsTrue(TestHelper.EqualsApproximately(distanceToMin, distanceToMax), $"Distance to min: {distanceToMin}, distance to max: {distanceToMax}");
        }

        #endregion
    }
}