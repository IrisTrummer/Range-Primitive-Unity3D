using System.Collections.Generic;
using NUnit.Framework;
using RangePrimitive;
using UnityEngine;

namespace Tests
{
    public class Vector3RangeTests
    {
        private static IEnumerable<Vector3> TestValues => new Vector3[] { new(-150, -5.25f, -1.5f), new(-2, -1, 0f), new(0, 0, 0), new(0.5f, -0.5f, 0.1f), new(1, 2, -5), new(3.5f, 5.2f, 9.9f), new(50, -50, -50), new(8, 99.9f, 1005) };
        private static IEnumerable<Vector3> ValuesForEmptyRange => new Vector3[] { new (0, 0, 0), new (1, 0, -1), new (99.9f, -99.9f, 99.9f) };

        private const int RandomSeed = 128;
        private readonly System.Random random = new(RandomSeed);

        #region Reorder

        [Test]
        public void ReorderPerComponent_DifferentRanges_MinIsSmallerThanOrEqualToMax([ValueSource(nameof(TestValues))] Vector3 min, [ValueSource(nameof(TestValues))] Vector3 max)
        {
            Range<Vector3> range = new Range<Vector3>(min, max);

            range = range.ReorderPerComponent();

            Assert.IsTrue(range.Min.x <= range.Max.x && range.Min.y <= range.Max.y && range.Min.z <= range.Max.z);
        }

        #endregion

        #region Random

        [Test]
        public void Random_DifferentRanges_GeneratedValueIsContainedInRange([ValueSource(nameof(TestValues))] Vector3 min, [ValueSource(nameof(TestValues))] Vector3 max)
        {
            Range<Vector3> range = new Range<Vector3>(min, max);

            Vector3 value = range.Random();

            Assert.IsTrue(range.Contains(value), $"Generated value: {value}");
        }

        #endregion

        #region Lerp

        [Test]
        public void Lerp_FactorOne_ReturnsMax([ValueSource(nameof(TestValues))] Vector3 min, [ValueSource(nameof(TestValues))] Vector3 max)
        {
            Range<Vector3> range = new Range<Vector3>(min, max);

            Vector3 result = range.Lerp(1);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, max), $"Result: {result}");
        }

        [Test]
        public void Lerp_FactorZero_ReturnsMin([ValueSource(nameof(TestValues))] Vector3 min, [ValueSource(nameof(TestValues))] Vector3 max)
        {
            Range<Vector3> range = new Range<Vector3>(min, max);

            Vector3 result = range.Lerp(0);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, min), $"Result: {result}");
        }

        [Test]
        [TestCase(0, 0, 0)]
        [TestCase(1, 0, 0.5f)]
        [TestCase(99.9f, -99.9f, 1)]
        public void Lerp_RangeWithMinEqualToMax_ReturnsMin(float minMaxX, float minMaxY, float t)
        {
            Vector3 minMax = new Vector3(minMaxX, minMaxY);
            Range<Vector3> range = new Range<Vector3>(minMax, minMax);

            Vector3 result = range.Lerp(t);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, minMax), $"Result: {result}");
        }

        [Test]
        [TestCase(99.5f, -5.5f, 99.5f, -5.5f, 99.5f, -5.5f, 0.25f, 73.25f, 73.25f, 73.25f)]
        [TestCase(1, 2, -2, -1, 2, -1, 0.25f, 1.25f, -1.75f, 1.25f)]
        [TestCase(2, 1, 50.5f, 5f, 5, 99, 0.5f, 1.5f, 27.75f, 52)]
        [TestCase(1, 2, 1, 2, 1, 2, 0.5f, 1.5f, 1.5f, 1.5f)]
        [TestCase(-10, -192, 192, 10, 192, -10, 0.75f, -146.5f, 55.5f, 40.5f)]
        [TestCase(120, 194, 50, 99, 0.1f, 10.5f, 0.81f, 179.94f, 89.69f, 8.524f)]
        public void Lerp_DifferentRanges_ReturnsExpectedResult(float minX, float maxX, float minY, float maxY, float minZ, float maxZ, float t, float expectedResultX, float expectedResultY, float expectedResultZ)
        {
            Range<Vector3> range = new Range<Vector3>(new Vector3(minX, minY, minZ), new Vector3(maxX, maxY, maxZ));
            Vector3 expectedResult = new Vector3(expectedResultX, expectedResultY, expectedResultZ);

            Vector3 result = range.Lerp(t);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, expectedResult), $"Result: {result}");
        }

        [Test]
        [TestCase(0, 1, -1, -5, -1, 1, -1)]
        [TestCase(99.9f, 5, 99.9f, 5, -5, 99.9f, -2)]
        [TestCase(-5, -1.1f, 100f, -1f, -100, 1.1f, -0.5f)]
        public void Lerp_ValueSmallerThanZero_ReturnsMin(float minX, float maxX, float minY, float maxY, float minZ, float maxZ, float t)
        {
            Range<Vector3> range = new Range<Vector3>(new Vector3(minX, minY, minZ), new Vector3(maxX, maxY, maxZ));

            Vector3 result = range.Lerp(t);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, range.Min), $"Result: {result}");
        }

        [Test]
        [TestCase(0, 1, -1, -5, -5, 0, 1.5f)]
        [TestCase(99.9f, 5, 99.9f, 5, 99.9f, 5, 2)]
        [TestCase(-5, -1.1f, 100f, -1f, 3, 1, 99.9f)]
        public void Lerp_ValueLargerThanOne_ReturnsMax(float minX, float maxX, float minY, float maxY, float minZ, float maxZ, float t)
        {
            Range<Vector3> range = new Range<Vector3>(new Vector3(minX, minY, minZ), new Vector3(maxX, maxY, maxZ));

            Vector3 result = range.Lerp(t);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, range.Max), $"Result: {result}");
        }

        #endregion

        #region InverseLerp

        [Test]
        [TestCase(0, 1, -1, 0, 1, -1)]
        [TestCase(-5.5f, 99, -5.5f, 99, -5.5f, 99)]
        [TestCase(-105, 1.1f, 99.9f, 5, 3, -99)]
        public void InverseLerp_MaxValue_ReturnsOne(float minX, float maxX, float minY, float maxY, float minZ, float maxZ)
        {
            Range<Vector3> range = new Range<Vector3>(new Vector3(minX, minY, minZ), new Vector3(maxX, maxY, maxZ));

            Vector3 result = range.InverseLerp(range.Max);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, Vector3.one), $"Result: {result}");
        }

        [Test]
        public void InverseLerp_MinValue_ReturnsZero([ValueSource(nameof(TestValues))] Vector3 min, [ValueSource(nameof(TestValues))] Vector3 max)
        {
            Range<Vector3> range = new Range<Vector3>(min, max);

            Vector3 result = range.InverseLerp(min);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, Vector3.zero), $"Result: {result}");
        }

        [Test]
        [TestCase(0, 0, 0, 0, 0, 0)]
        [TestCase(1, 0, 3, 1, 0, 1)]
        [TestCase(99.9f, -99.9f, 50, 100, -5.5f, 3)]
        public void InverseLerp_RangeWithMinEqualToMax_ReturnsZero(float minMaxX, float minMaxY, float minMaxZ, float valueX, float valueY, float valueZ)
        {
            Vector3 minMax = new Vector3(minMaxX, minMaxY, minMaxZ);
            Range<Vector3> range = new Range<Vector3>(minMax, minMax);
            Vector3 value = new Vector3(valueX, valueY, valueZ);

            Vector3 result = range.InverseLerp(value);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, Vector3.zero), $"Result: {result}");
        }

        [Test]
        [TestCase(99, -5, 99.5f, -5.5f, 99.5f, -5.5f, 73, 73.25f, 73.25f, 0.25f)]
        [TestCase(1, 2, -2, -1, 2, -1, 1.25f, -1.75f, 1.25f, 0.25f)]
        [TestCase(2, 1, 50.5f, 5f, 1, 2, 1.5f, 27.75f, 1.5f, 0.5f)]
        [TestCase(1, 2, 1, 2, 1, 2, 1.5f, 1.5f, 1.5f, 0.5f)]
        [TestCase(-10, -192, 50.5f, 5f, 5, 99, -146.5f, 16.375f, 75.5f, 0.75f)]
        [TestCase(120, 194, 50, 99, 0.1f, 10.5f, 179.94f, 89.69f, 8.524f, 0.81f)]
        public void InverseLerp_DifferentRanges_ReturnsExpectedResult(float minX, float maxX, float minY, float maxY, float minZ, float maxZ, float valueX, float valueY, float valueZ, float expectedResult)
        {
            Range<Vector3> range = new Range<Vector3>(new Vector3(minX, minY, minZ), new Vector3(maxX, maxY, maxZ));
            Vector3 value = new Vector3(valueX, valueY, valueZ);

            Vector3 result = range.InverseLerp(value);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, expectedResult * Vector3.one), $"Result: {result}");
        }

        [Test]
        [TestCase(10, 50, 50, 10, 10, -50, 100, 100, 100, 1, 0, 0)]
        [TestCase(-99, -50, 1, 3, -5f, 5f, 100, 100, 100, 1, 1, 1)]
        [TestCase(-100, -10, 100, 10, -10, 100, -110, -110, -110, 0, 1, 0)]
        [TestCase(100, 150, -1, -5, 0.1f, -9.9f, -1, 0, 1, 0, 0, 0)]
        public void InverseLerp_ValueOutsideRange_ReturnsCloserBoundary(float minX, float maxX, float minY, float maxY, float minZ, float maxZ, float valueX, float valueY, float valueZ, float expectedResultX, float expectedResultY, float expectedResultZ)
        {
            Range<Vector3> range = new Range<Vector3>(new Vector3(minX, minY, minZ), new Vector3(maxX, maxY, maxZ));
            Vector3 expectedResult = new Vector3(expectedResultX, expectedResultY, expectedResultZ);
            Vector3 value = new Vector3(valueX, valueY, valueZ);

            Vector3 result = range.InverseLerp(value);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, expectedResult), $"Result: {result}");
        }

        #endregion

        #region SmoothStep

        [Test]
        public void SmoothStep_FactorOne_ReturnsMax([ValueSource(nameof(TestValues))] Vector3 min, [ValueSource(nameof(TestValues))] Vector3 max)
        {
            Range<Vector3> range = new Range<Vector3>(min, max);

            Vector3 result = range.SmoothStep(1);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, max), $"Result: {result}");
        }

        [Test]
        public void SmoothStep_FactorZero_ReturnsMin([ValueSource(nameof(TestValues))] Vector3 min, [ValueSource(nameof(TestValues))] Vector3 max)
        {
            Range<Vector3> range = new Range<Vector3>(min, max);

            Vector3 result = range.SmoothStep(0);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, min), $"Result: {result}");
        }

        [Test]
        [TestCase(0, 0, 0, 0)]
        [TestCase(-5, 5, -5.5f, 0.25f)]
        [TestCase(99.9f, -99.9f, 99f, 0.5f)]
        [TestCase(1, 0, -1, 1)]
        public void SmoothStep_RangeWithMinEqualToMax_ReturnsMin(float minMaxX, float minMaxY, float minMaxZ, float t)
        {
            Vector3 minMax = new Vector3(minMaxX, minMaxY, minMaxZ);
            Range<Vector3> range = new Range<Vector3>(minMax, minMax);

            Vector3 result = range.SmoothStep(t);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, minMax), $"Result: {result}");
        }

        [Test]
        [TestCase(1, 0, 1, 2, -1, 1, 0.1f, 0.972f, 1.028f, -0.944f)]
        [TestCase(1, 5, 1, 5, 1, 5, 0.25f, 1.625f, 1.625f, 1.625f)]
        [TestCase(100, 5, -100, -5, -100, 5, 0.5f, 52.5f, -52.5f, -47.5f)]
        [TestCase(-10, 10, -10, 10, 10, -10, 0.75f, 6.875f, 6.875f, -6.875f)]
        [TestCase(50, 5, 190, 580, 0.5f, -10f, 0.9f, 6.26f, 569.08f, -9.706f)]
        public void SmoothStep_DifferentRanges_ReturnsExpectedResult(float minX, float maxX, float minY, float maxY, float minZ, float maxZ, float t, float expectedResultX, float expectedResultY, float expectedResultZ)
        {
            Range<Vector3> range = new Range<Vector3>(new Vector3(minX, minY, minZ), new Vector3(maxX, maxY, maxZ));
            Vector3 expectedResult = new Vector3(expectedResultX, expectedResultY, expectedResultZ);

            Vector3 result = range.SmoothStep(t);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, expectedResult), $"Result: {result}");
        }

        [Test]
        [TestCase(0, 1, -1, -5, 5, 1, -1)]
        [TestCase(99.9f, 5, 99.9f, 5, 99.9f, 5, -2)]
        [TestCase(-5, -1.1f, 100f, -1f, 50, 5, -0.5f)]
        public void SmoothStep_ValueSmallerThanZero_ReturnsMin(float minX, float maxX, float minY, float maxY, float minZ, float maxZ, float t)
        {
            Range<Vector3> range = new Range<Vector3>(new Vector3(minX, minY, minZ), new Vector3(maxX, maxY, maxZ));

            Vector3 result = range.SmoothStep(t);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, range.Min), $"Result: {result}");
        }

        [Test]
        [TestCase(0, 1, -1, -5, 99.9f, -50, 1)]
        [TestCase(99.9f, 5, 99.9f, 5, 99.9f, 5, 2)]
        [TestCase(-5, -1.1f, 100f, -1f, 99.9f, 3, 1.5f)]
        public void SmoothStep_ValueLargerThanOne_ReturnsMax(float minX, float maxX, float minY, float maxY, float minZ, float maxZ, float t)
        {
            Range<Vector3> range = new Range<Vector3>(new Vector3(minX, minY, minZ), new Vector3(maxX, maxY, maxZ));

            Vector3 result = range.SmoothStep(t);

            Assert.IsTrue(TestHelper.EqualsApproximately(result, range.Max), $"Result: {result}");
        }

        #endregion

        #region Delta

        [Test]
        public void Delta_RangeWithMinEqualToMax_ReturnsZero([ValueSource(nameof(ValuesForEmptyRange))] Vector3 minMax)
        {
            Range<Vector3> range = new Range<Vector3>(minMax, minMax);

            Vector3 result = range.Delta();

            Assert.IsTrue(TestHelper.EqualsApproximately(result, Vector3.zero), $"Result: {result}");
        }

        [Test]
        [TestCase(2, 1, 1, 2, -1, 2, -1, 1, 3)]
        [TestCase(-10.1f, 10, 100.5f, 5, 5.5f, -100, 20.1f, -95.5f, -105.5f)]
        [TestCase(-150, -580.9f, -150.9f, -100, -3.5f, 50, -430.9f, 50.9f, 53.5f)]
        public void Delta_DifferentRanges_ReturnsExpectedValue(float minX, float maxX, float minY, float maxY, float minZ, float maxZ, float expectedResultX, float expectedResultY, float expectedResultZ)
        {
            Range<Vector3> range = new Range<Vector3>(new Vector3(minX, minY, minZ), new Vector3(maxX, maxY, maxZ));
            Vector3 expectedResult = new Vector3(expectedResultX, expectedResultY, expectedResultZ);

            Vector3 result = range.Delta();

            Assert.IsTrue(TestHelper.EqualsApproximately(result, expectedResult), $"Result: {result}");
        }

        #endregion

        #region Size

        [Test]
        public void Size_RangeWithMinEqualToMax_ReturnsZero([ValueSource(nameof(ValuesForEmptyRange))] Vector3 minMax)
        {
            Range<Vector3> range = new Range<Vector3>(minMax, minMax);

            Vector3 result = range.Size();

            Assert.IsTrue(TestHelper.EqualsApproximately(result, Vector3.zero), $"Result: {result}");
        }

        [Test]
        [TestCase(2, 1, 1, 2, -1, 2, 1, 1, 3)]
        [TestCase(100, -5.5f, -10.1f, 10, 5.5f, -100, 105.5f, 20.1f, 105.5f)]
        [TestCase(-580, -150.8f, -50, -99.9f, -3.5f, 50, 429.2f, 49.9f, 53.5f)]
        public void Size_DifferentRanges_ReturnsExpectedResult(float minX, float maxX, float minY, float maxY, float minZ, float maxZ, float expectedResultX, float expectedResultY, float expectedResultZ)
        {
            Range<Vector3> range = new Range<Vector3>(new Vector3(minX, minY, minZ), new Vector3(maxX, maxY, maxZ));
            Vector3 expectedResult = new Vector3(expectedResultX, expectedResultY, expectedResultZ);

            Vector3 result = range.Size();

            Assert.IsTrue(TestHelper.EqualsApproximately(result, expectedResult), $"Result: {result}");
        }

        #endregion

        #region Clamp

        [Test]
        [TestCase(0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(1, 2, 2, 1, -1, -2, 2, 2, -2)]
        [TestCase(1, 2, 1, 2, 1, 2, 1, 1, 1)]
        [TestCase(200.5f, -1, -5f, 5f, -0.1f, 10, 2, 4, 0)]
        [TestCase(-10, -192.9f, 5, 99, -9.9f, 50, -120, 50, 5)]
        [TestCase(120, 194, -120, -194, 120, -194, 186.8f, -186.8f, 5.5f)]
        public void Clamp_ValueInRange_ReturnsUnchangedValue(float minX, float maxX, float minY, float maxY, float minZ, float maxZ, float valueX, float valueY, float valueZ)
        {
            Range<Vector3> range = new Range<Vector3>(new Vector3(minX, minY, minZ), new Vector3(maxX, maxY, maxZ));
            Vector3 value = new Vector3(valueX, valueY, valueZ);

            Vector3 clampedValue = range.Clamp(value);

            Assert.IsTrue(TestHelper.EqualsApproximately(clampedValue, value), $"Result: {clampedValue}");
        }

        [Test]
        [TestCase(0, 0, 0, 5, -5, 2.5f)]
        [TestCase(-5, 5, -5, 99.9f, 99.9f, -99.9f)]
        [TestCase(99.9f, 0.1f, -99.9f, 0.5f, 50, -3)]
        [TestCase(3, -3, 1, 0, 100, 1)]
        public void Clamp_RangeWithMinEqualToMax_ReturnsMin(float minMaxX, float minMaxY, float minMaxZ, float valueX, float valueY, float valueZ)
        {
            Vector3 minMax = new Vector3(minMaxX, minMaxY, minMaxZ);
            Range<Vector3> range = new Range<Vector3>(minMax, minMax);
            Vector3 value = new Vector3(valueX, valueY, valueZ);

            Vector3 clampedValue = range.Clamp(value);

            Assert.IsTrue(TestHelper.EqualsApproximately(clampedValue, range.Min), $"Result: {clampedValue}");
        }

        [Test]
        [TestCase(1, 5, 1, 5, 1, 5, 100, 10, 5.5f)]
        [TestCase(100.1f, -1, -5, 99.9f, 4, 5, 1000, 100, 10)]
        [TestCase(-5, 10.9f, 0.1f, 0.5f, 5, 10.1f, 99, 1, 10.5f)]
        [TestCase(-100, -99, 100, 99, 100, -99, -98.5f, 101, 150)]
        public void Clamp_ValueLargerThanMax_ReturnsMax(float minX, float maxX, float minY, float maxY, float minZ, float maxZ, float valueX, float valueY, float valueZ)
        {
            Range<Vector3> range = new Range<Vector3>(new Vector3(minX, minY, minZ), new Vector3(maxX, maxY, maxZ));
            Vector3 value = new Vector3(valueX, valueY, valueZ);
            Vector3 expectedResult = new Vector3(Mathf.Max(range.Min.x, range.Max.x), Mathf.Max(range.Min.y, range.Max.y), Mathf.Max(range.Min.z, range.Max.z));

            Vector3 clampedValue = range.Clamp(value);

            Assert.IsTrue(TestHelper.EqualsApproximately(clampedValue, expectedResult), $"Result: {clampedValue}");
        }

        [Test]
        [TestCase(1, 5, 1, 5, 1, 5, -10, -10, -10)]
        [TestCase(10, 50, 50, 10, 50, -10, -1, -1, -50)]
        [TestCase(5.5f, 1, 99.9f, -99.9f, 0.1f, 0.5f, 0, -110, 0)]
        [TestCase(-100, -10.9f, 5.5f, 5f, -5.5f, 5.5f, -101, -5, -99)]
        [TestCase(100, -1, 0.1f, 0.5f, 99.9f, 99f, -5.1f, 0, 98)]
        public void Clamp_ValueSmallerThanMin_ReturnsMin(float minX, float maxX, float minY, float maxY, float minZ, float maxZ, float valueX, float valueY, float valueZ)
        {
            Range<Vector3> range = new Range<Vector3>(new Vector3(minX, minY, minZ), new Vector3(maxX, maxY, maxZ));
            Vector3 value = new Vector3(valueX, valueY, valueZ);
            Vector3 expectedResult = new Vector3(Mathf.Min(range.Min.x, range.Max.x), Mathf.Min(range.Min.y, range.Max.y), Mathf.Min(range.Min.z, range.Max.z));

            Vector3 clampedValue = range.Clamp(value);

            Assert.IsTrue(TestHelper.EqualsApproximately(clampedValue, expectedResult), $"Result: {clampedValue}");
        }

        #endregion

        #region Contains

        [Test]
        public void Contains_ValueInsideOfRange_ReturnsTrue([ValueSource(nameof(TestValues))] Vector3 min, [ValueSource(nameof(TestValues))] Vector3 max)
        {
            Range<Vector3> range = new Range<Vector3>(min, max);
            Vector3 value = new Vector3(TestHelper.GenerateRandomValueInRange(random, min.x, max.x), TestHelper.GenerateRandomValueInRange(random, min.y, max.y), TestHelper.GenerateRandomValueInRange(random, min.z, max.z));

            bool contains = range.Contains(value);

            Assert.IsTrue(contains, $"Tested value: {value}");
        }

        [Test]
        [TestCase(0, 0, 0, 0, 0, 0, 1, 1, 1)]
        [TestCase(1, 5, 5, 1, -5, 1, 10, 0, 5)]
        [TestCase(10, 50.5f, -5, -1, 0.1f, 0.5f, -1, 1, 0)]
        [TestCase(5, 1, 0.1f, 0.5f, 99.9f, -99.9f, 0, 0, 100)]
        [TestCase(-100.9f, -10, 100.9f, 10f, -100.9f, 10f, -101, -101, -110)]
        [TestCase(1, 2, 100, -1, 3, 5, 3, 1000.1f, 0)]
        public void Contains_ValueOutsideOfRange_ReturnsFalse(float minX, float maxX, float minY, float maxY, float minZ, float maxZ, float valueX, float valueY, float valueZ)
        {
            Range<Vector3> range = new Range<Vector3>(new Vector3(minX, minY, minZ), new Vector3(maxX, maxY, maxZ));
            Vector3 valueOutsideRange = new Vector3(valueX, valueY, valueZ);

            bool contains = range.Contains(valueOutsideRange);

            Assert.IsFalse(contains, $"Tested value: {valueOutsideRange}");
        }

        [Test]
        public void Contains_ValueAtMinBoundaryOfRange_ReturnsTrue([ValueSource(nameof(TestValues))] Vector3 min, [ValueSource(nameof(TestValues))] Vector3 max)
        {
            Range<Vector3> range = new Range<Vector3>(min, max);

            bool contains = range.Contains(min);

            Assert.IsTrue(contains, $"Tested value: {min}");
        }

        [Test]
        public void Contains_ValueAtMaxBoundaryOfRange_ReturnsTrue([ValueSource(nameof(TestValues))] Vector3 min, [ValueSource(nameof(TestValues))] Vector3 max)
        {
            Range<Vector3> range = new Range<Vector3>(min, max);

            bool contains = range.Contains(max);

            Assert.IsTrue(contains, $"Tested value: {max}");
        }

        #endregion

        #region Center

        [Test]
        public void Center_RangeWithMinEqualToMax_ReturnsMinValue([ValueSource(nameof(ValuesForEmptyRange))] Vector3 minMax)
        {
            Range<Vector3> range = new Range<Vector3>(minMax, minMax);

            Vector3 center = range.Center();

            Assert.IsTrue(TestHelper.EqualsApproximately(center, range.Min), $"Result: {center}");
        }

        [Test]
        [TestCase(5.5f, 0, 100, -10, 1, 0,2.75f, 45, 0.5f)]
        [TestCase(-5, -99.9f, 0, 5, 0.1f, -0.1f, -52.45f, 2.5f, 0)]
        [TestCase(-99, 150, 0, -100, -100.9f, -10, 25.5f, -50, -55.45f)]
        public void Center_DifferentRanges_ReturnsExpectedCenter(float minX, float maxX, float minY, float maxY, float minZ, float maxZ, float expectedResultX, float expectedResultY, float expectedResultZ)
        {
            Range<Vector3> range = new Range<Vector3>(new Vector3(minX, minY, minZ), new Vector3(maxX, maxY, maxZ));
            Vector3 expectedResult = new Vector3(expectedResultX, expectedResultY, expectedResultZ);

            Vector3 center = range.Center();

            Assert.IsTrue(TestHelper.EqualsApproximately(center, expectedResult), $"Result: {center}");
        }

        [Test]
        public void Center_DifferentRanges_CenterHasEqualDistanceFromMinAndMax([ValueSource(nameof(TestValues))] Vector3 min, [ValueSource(nameof(TestValues))] Vector3 max)
        {
            Range<Vector3> range = new Range<Vector3>(min, max);

            Vector3 center = range.Center();
            Vector3 distanceToMin = center - min;
            Vector3 distanceToMax = max - center;

            Assert.IsTrue(TestHelper.EqualsApproximately(distanceToMin, distanceToMax), $"Distance to min: {distanceToMin}, distance to max: {distanceToMax}");
        }

        #endregion
    }
}