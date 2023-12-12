using UnityEngine;

namespace RangePrimitive
{
    public static class RangeExtensions
    {
        #region Reorder

        /// <summary>
        /// Returns a new range with the same boundaries, ensuring that min is the smaller value and max is the bigger value.
        /// </summary>
        public static Range<int> Reorder(this Range<int> range)
        {
            return new Range<int>(Mathf.Min(range.Min, range.Max), Mathf.Max(range.Min, range.Max));
        }

        /// <inheritdoc cref="Reorder(RangePrimitive.Range{int})"/>
        public static Range<float> Reorder(this Range<float> range)
        {
            return new Range<float>(Mathf.Min(range.Min, range.Max), Mathf.Max(range.Min, range.Max));
        }

        /// <summary>
        /// Returns a new range with the same boundaries, ensuring that for each vector component min is the smaller value and max is the bigger value.
        /// </summary>
        public static Range<Vector2> ReorderPerComponent(this Range<Vector2> range)
        {
            return new Range<Vector2>(
                new Vector2(Mathf.Min(range.Min.x, range.Max.x), Mathf.Min(range.Min.y, range.Max.y)),
                new Vector2(Mathf.Max(range.Min.x, range.Max.x), Mathf.Max(range.Min.y, range.Max.y)));
        }

        /// <inheritdoc cref="ReorderPerComponent(RangePrimitive.Range{Vector2})"/>
        public static Range<Vector2Int> ReorderPerComponent(this Range<Vector2Int> range)
        {
            return new Range<Vector2Int>(
                new Vector2Int(Mathf.Min(range.Min.x, range.Max.x), Mathf.Min(range.Min.y, range.Max.y)),
                new Vector2Int(Mathf.Max(range.Min.x, range.Max.x), Mathf.Max(range.Min.y, range.Max.y)));
        }

        /// <inheritdoc cref="ReorderPerComponent(RangePrimitive.Range{Vector2})"/>
        public static Range<Vector3> ReorderPerComponent(this Range<Vector3> range)
        {
            return new Range<Vector3>(
                new Vector3(Mathf.Min(range.Min.x, range.Max.x), Mathf.Min(range.Min.y, range.Max.y), Mathf.Min(range.Min.z, range.Max.z)),
                new Vector3(Mathf.Max(range.Min.x, range.Max.x), Mathf.Max(range.Min.y, range.Max.y), Mathf.Max(range.Min.z, range.Max.z)));
        }

        /// <inheritdoc cref="ReorderPerComponent(RangePrimitive.Range{Vector2})"/>
        public static Range<Vector3Int> ReorderPerComponent(this Range<Vector3Int> range)
        {
            return new Range<Vector3Int>(
                new Vector3Int(Mathf.Min(range.Min.x, range.Max.x), Mathf.Min(range.Min.y, range.Max.y), Mathf.Min(range.Min.z, range.Max.z)),
                new Vector3Int(Mathf.Max(range.Min.x, range.Max.x), Mathf.Max(range.Min.y, range.Max.y), Mathf.Max(range.Min.z, range.Max.z)));
        }

        #endregion

        #region Random

        /// <summary>
        /// Returns a random value between min and max (both edges inclusive).
        /// </summary>
        public static int Random(this Range<int> range)
        {
            return Random(range.Min, range.Max);
        }

        /// <inheritdoc cref="Random(RangePrimitive.Range{int})"/>
        public static float Random(this Range<float> range)
        {
            return Random(range.Min, range.Max);
        }

        /// <summary>
        /// Returns a new vector where each component has a random value between the corresponding components of min and max (both edges inclusive).
        /// </summary>
        public static Vector2 Random(this Range<Vector2> range)
        {
            return new Vector2(Random(range.Min.x, range.Max.x), Random(range.Min.y, range.Max.y));
        }

        /// <inheritdoc cref="Random(RangePrimitive.Range{Vector2})"/>
        public static Vector2Int Random(this Range<Vector2Int> range)
        {
            return new Vector2Int(Random(range.Min.x, range.Max.x), Random(range.Min.y, range.Max.y));
        }

        /// <inheritdoc cref="Random(RangePrimitive.Range{Vector2})"/>
        public static Vector3 Random(this Range<Vector3> range)
        {
            return new Vector3(Random(range.Min.x, range.Max.x), Random(range.Min.y, range.Max.y), Random(range.Min.z, range.Max.z));
        }

        /// <inheritdoc cref="Random(RangePrimitive.Range{Vector2})"/>
        public static Vector3Int Random(this Range<Vector3Int> range)
        {
            return new Vector3Int(Random(range.Min.x, range.Max.x), Random(range.Min.y, range.Max.y), Random(range.Min.z, range.Max.z));
        }

        private static int Random(int min, int max)
        {
            return UnityEngine.Random.Range(min, max + 1);
        }
        
        private static float Random(float min, float max)
        {
            return UnityEngine.Random.Range(min, max);
        }

        #endregion

        #region Lerp

        /// <summary>
        /// Returns the linearly interpolated value between min and max by t.
        /// </summary>
        public static float Lerp(this Range<int> range, float t)
        {
            return Lerp(range.Min, range.Max, t);
        }

        /// <inheritdoc cref="Lerp(RangePrimitive.Range{int},float)"/>
        public static float Lerp(this Range<float> range, float t)
        {
            return Lerp(range.Min, range.Max, t);
        }

        /// <summary>
        /// Returns the component-wise linearly interpolated value between min and max by t.
        /// </summary>
        public static Vector2 Lerp(this Range<Vector2> range, float t)
        {
            return new Vector2(Lerp(range.Min.x, range.Max.x, t), Lerp(range.Min.y, range.Max.y, t));
        }

        /// <inheritdoc cref="Lerp(RangePrimitive.Range{Vector2},float)"/>
        public static Vector2 Lerp(this Range<Vector2Int> range, float t)
        {
            return new Vector2(Lerp(range.Min.x, range.Max.x, t), Lerp(range.Min.y, range.Max.y, t));
        }

        /// <inheritdoc cref="Lerp(RangePrimitive.Range{Vector2},float)"/>
        public static Vector3 Lerp(this Range<Vector3> range, float t)
        {
            return new Vector3(Lerp(range.Min.x, range.Max.x, t), Lerp(range.Min.y, range.Max.y, t), Lerp(range.Min.z, range.Max.z, t));
        }

        /// <inheritdoc cref="Lerp(RangePrimitive.Range{Vector2},float)"/>
        public static Vector3 Lerp(this Range<Vector3Int> range, float t)
        {
            return new Vector3(Lerp(range.Min.x, range.Max.x, t), Lerp(range.Min.y, range.Max.y, t), Lerp(range.Min.z, range.Max.z, t));
        }

        private static float Lerp(float min, float max, float t)
        {
            return Mathf.Lerp(min, max, t);
        }

        #endregion

        #region InverseLerp

        /// <summary>
        /// Returns a value between 0 and 1 indicating where the given value lies between min and max.
        /// </summary>
        public static float InverseLerp(this Range<int> range, float value)
        {
            return InverseLerp(range.Min, range.Max, value);
        }

        /// <inheritdoc cref="InverseLerp(RangePrimitive.Range{int},float)"/>
        public static float InverseLerp(this Range<float> range, float value)
        {
            return InverseLerp(range.Min, range.Max, value);
        }
        
        /// <summary>
        /// For each component, returns a value between 0 and 1 indicating where the given value lies between the corresponding component of min and max.
        /// </summary>
        public static Vector2 InverseLerp(this Range<Vector2> range, float value)
        {
            return new Vector2(InverseLerp(range.Min.x, range.Max.x, value), InverseLerp(range.Min.y, range.Max.y, value));
        }
        
        /// <inheritdoc cref="InverseLerp(RangePrimitive.Range{Vector2},float)"/>
        public static Vector2 InverseLerp(this Range<Vector2Int> range, float value)
        {
            return new Vector2(InverseLerp(range.Min.x, range.Max.x, value), InverseLerp(range.Min.y, range.Max.y, value));
        }

        /// <inheritdoc cref="InverseLerp(RangePrimitive.Range{Vector2},float)"/>
        public static Vector3 InverseLerp(this Range<Vector3> range, float value)
        {
            return new Vector3(InverseLerp(range.Min.x, range.Max.x, value), InverseLerp(range.Min.y, range.Max.y, value), InverseLerp(range.Min.z, range.Max.z, value));
        }

        /// <inheritdoc cref="InverseLerp(RangePrimitive.Range{Vector2},float)"/>
        public static Vector3 InverseLerp(this Range<Vector3Int> range, float value)
        {
            return new Vector3(InverseLerp(range.Min.x, range.Max.x, value), InverseLerp(range.Min.y, range.Max.y, value), InverseLerp(range.Min.z, range.Max.z, value));
        }

        private static float InverseLerp(float min, float max, float value)
        {
            return Mathf.InverseLerp(min, max, value);
        }
        
        #endregion

        #region SmoothStep

        /// <summary>
        /// Returns the interpolated value between min and max by t with smoothing at the edges.
        /// </summary>
        public static float SmoothStep(this Range<int> range, float t)
        {
            return Mathf.SmoothStep(range.Min, range.Max, t);
        }

        /// <inheritdoc cref="SmoothStep(RangePrimitive.Range{int},float)"/>
        public static float SmoothStep(this Range<float> range, float t)
        {
            return Mathf.SmoothStep(range.Min, range.Max, t);
        }
        
        /// <summary>
        /// Returns the component-wise interpolated value between min and max by t with smoothing at the edges.
        /// </summary>
        public static Vector2 SmoothStep(this Range<Vector2> range, float t)
        {
            return new Vector2(SmoothStep(range.Min.x, range.Max.x, t), SmoothStep(range.Min.y, range.Max.y, t));
        }

        /// <inheritdoc cref="SmoothStep(RangePrimitive.Range{Vector2},float)"/>
        public static Vector2 SmoothStep(this Range<Vector2Int> range, float t)
        {
            return new Vector2(SmoothStep(range.Min.x, range.Max.x, t), SmoothStep(range.Min.y, range.Max.y, t));
        }

        /// <inheritdoc cref="SmoothStep(RangePrimitive.Range{Vector2},float)"/>
        public static Vector3 SmoothStep(this Range<Vector3> range, float t)
        {
            return new Vector3(SmoothStep(range.Min.x, range.Max.x, t), SmoothStep(range.Min.y, range.Max.y, t), SmoothStep(range.Min.z, range.Max.z, t));
        }

        /// <inheritdoc cref="SmoothStep(RangePrimitive.Range{Vector2},float)"/>
        public static Vector3 SmoothStep(this Range<Vector3Int> range, float t)
        {
            return new Vector3(SmoothStep(range.Min.x, range.Max.x, t), SmoothStep(range.Min.y, range.Max.y, t), SmoothStep(range.Min.z, range.Max.z, t));
        }

        private static float SmoothStep(float min, float max, float t)
        {
            return Mathf.SmoothStep(min, max, t);
        }

        #endregion

        #region Delta

        /// <summary>
        /// Returns the difference between max and min.
        /// </summary>
        public static int Delta(this Range<int> range)
        {
            return range.Max - range.Min;
        }

        /// <inheritdoc cref="Delta(RangePrimitive.Range{int})"/>
        public static float Delta(this Range<float> range)
        {
            return range.Max - range.Min;
        }

        /// <summary>
        /// Returns the component-wise difference between max and min.
        /// </summary>
        public static Vector2 Delta(this Range<Vector2> range)
        {
            return range.Max - range.Min;
        }

        /// <inheritdoc cref="Delta(RangePrimitive.Range{Vector2})"/>
        public static Vector2Int Delta(this Range<Vector2Int> range)
        {
            return range.Max - range.Min;
        }

        /// <inheritdoc cref="Delta(RangePrimitive.Range{Vector2})"/>
        public static Vector3 Delta(this Range<Vector3> range)
        {
            return range.Max - range.Min;
        }

        /// <inheritdoc cref="Delta(RangePrimitive.Range{Vector2})"/>
        public static Vector3Int Delta(this Range<Vector3Int> range)
        {
            return range.Max - range.Min;
        }
        
        #endregion
        
        #region Spread

        /// <summary>
        /// Returns the absolute difference between min and max, which is equal to the spread of the range.
        /// </summary>
        public static int Size(this Range<int> range)
        {
            return Mathf.Abs(range.Delta());
        }

        /// <inheritdoc cref="Size"/>
        public static float Size(this Range<float> range)
        {
            return Mathf.Abs(range.Delta());
        }

        /// <summary>
        /// Returns the absolute, component-wise difference between min and max, which is equal to the spread of the range.
        /// </summary>
        public static Vector2 Size(this Range<Vector2> range)
        {
            Vector2 delta = range.Delta();
            return new Vector2(Mathf.Abs(delta.x), Mathf.Abs(delta.y));
        }

        /// <inheritdoc cref="Size(RangePrimitive.Range{UnityEngine.Vector2})"/>
        public static Vector2Int Size(this Range<Vector2Int> range)
        {
            Vector2Int delta = range.Delta();
            return new Vector2Int(Mathf.Abs(delta.x), Mathf.Abs(delta.y));
        }

        /// <inheritdoc cref="Size(RangePrimitive.Range{UnityEngine.Vector2})"/>
        public static Vector3 Size(this Range<Vector3> range)
        {
            Vector3 delta = range.Delta();
            return new Vector3(Mathf.Abs(delta.x), Mathf.Abs(delta.y), Mathf.Abs(delta.z));
        }

        /// <inheritdoc cref="Size(RangePrimitive.Range{UnityEngine.Vector2})"/>
        public static Vector3Int Size(this Range<Vector3Int> range)
        {
            Vector3Int delta = range.Delta();
            return new Vector3Int(Mathf.Abs(delta.x), Mathf.Abs(delta.y), Mathf.Abs(delta.z));
        }

        #endregion

        #region Clamp

        /// <summary>
        /// Clamps the given value between min and max.
        /// </summary>
        public static int Clamp(this Range<int> range, int value)
        {
            return Clamp(value, range.Min, range.Max);
        }

        /// <inheritdoc cref="Clamp(RangePrimitive.Range{int},int)"/>
        public static float Clamp(this Range<float> range, float value)
        {
            return Clamp(value, range.Min, range.Max);
        }

        /// <summary>
        /// Returns a new vector where each component is clamped between the corresponding components of min and max.
        /// </summary>
        public static Vector2 Clamp(this Range<Vector2> range, Vector2 value)
        {
            return new Vector2(Clamp(value.x, range.Min.x, range.Max.x), Clamp(value.y, range.Min.y, range.Max.y));
        }

        /// <inheritdoc cref="Clamp(RangePrimitive.Range{Vector2},Vector2)"/>
        public static Vector2Int Clamp(this Range<Vector2Int> range, Vector2Int value)
        {
            return new Vector2Int(Clamp(value.x, range.Min.x, range.Max.x), Clamp(value.y, range.Min.y, range.Max.y));
        }

        /// <inheritdoc cref="Clamp(RangePrimitive.Range{Vector2},Vector2)"/>
        public static Vector3 Clamp(this Range<Vector3> range, Vector3 value)
        {
            return new Vector3(Clamp(value.x, range.Min.x, range.Max.x), Clamp(value.y, range.Min.y, range.Max.y), Clamp(value.z, range.Min.z, range.Max.z));
        }

        /// <inheritdoc cref="Clamp(RangePrimitive.Range{Vector2},Vector2)"/>
        public static Vector3Int Clamp(this Range<Vector3Int> range, Vector3Int value)
        {
            return new Vector3Int(Clamp(value.x, range.Min.x, range.Max.x), Clamp(value.y, range.Min.y, range.Max.y), Clamp(value.z, range.Min.z, range.Max.z));
        }

        private static int Clamp(int value, int min, int max)
        {
            return Mathf.Clamp(value, Mathf.Min(min, max), Mathf.Max(min, max));
        }
        
        private static float Clamp(float value, float min, float max)
        {
            return Mathf.Clamp(value, Mathf.Min(min, max), Mathf.Max(min, max));
        }

        #endregion

        #region Contains

        /// <summary>
        /// Returns true if the given values lies between min and max (edges edges inclusive).
        /// </summary>
        public static bool Contains(this Range<int> range, int value)
        {
            return Contains(value, range.Min, range.Max);
        }

        /// <inheritdoc cref="Contains(RangePrimitive.Range{int},int)"/>
        public static bool Contains(this Range<float> range, float value)
        {
            return Contains(value, range.Min, range.Max);
        }

        /// <summary>
        /// Returns true if all components of the given vector lie between the corresponding components of min and max.
        /// </summary>
        public static bool Contains(this Range<Vector2> range, Vector2 value)
        {
            return Contains(value.x, range.Min.x, range.Max.x) && Contains(value.y, range.Min.y, range.Max.y);
        }

        /// <inheritdoc cref="Contains(RangePrimitive.Range{Vector2},Vector2)"/>
        public static bool Contains(this Range<Vector2Int> range, Vector2Int value)
        {
            return Contains(value.x, range.Min.x, range.Max.x) && Contains(value.y, range.Min.y, range.Max.y);
        }

        /// <inheritdoc cref="Contains(RangePrimitive.Range{Vector2},Vector2)"/>
        public static bool Contains(this Range<Vector3> range, Vector3 value)
        {
            return Contains(value.x, range.Min.x, range.Max.x) && Contains(value.y, range.Min.y, range.Max.y) && Contains(value.z, range.Min.z, range.Max.z);
        }

        /// <inheritdoc cref="Contains(RangePrimitive.Range{Vector2},Vector2)"/>
        public static bool Contains(this Range<Vector3Int> range, Vector3Int value)
        {
            return Contains(value.x, range.Min.x, range.Max.x) && Contains(value.y, range.Min.y, range.Max.y) && Contains(value.z, range.Min.z, range.Max.z);
        }

        private static bool Contains(int value, int min, int max)
        {
            return value >= Mathf.Min(min, max) && value <= Mathf.Max(min, max);
        }

        private static bool Contains(float value, float min, float max)
        {
            return value >= Mathf.Min(min, max) && value <= Mathf.Max(min, max);
        }

        #endregion

        #region Center
        
        /// <summary>
        /// Gets the center point of the range, which is equal to the point in the middle between the min and max.
        /// </summary>
        public static float Center(this Range<int> range)
        {
            return range.Lerp(0.5f);
        }
        
        /// <inheritdoc cref="Center(RangePrimitive.Range{int})"/>
        public static float Center(this Range<float> range)
        {
            return range.Lerp(0.5f);
        }

        /// <summary>
        /// Gets the center point of the range, which is equal to the point in the middle between the component-wise min and max.
        /// </summary>
        public static Vector2 Center(this Range<Vector2> range)
        {
            return range.Lerp(0.5f);
        }
        
        /// <inheritdoc cref="Center(RangePrimitive.Range{UnityEngine.Vector2})"/>
        public static Vector2 Center(this Range<Vector2Int> range)
        {
            return range.Lerp(0.5f);
        }

        /// <inheritdoc cref="Center(RangePrimitive.Range{UnityEngine.Vector2})"/>
        public static Vector3 Center(this Range<Vector3> range)
        {
            return range.Lerp(0.5f);
        }
        
        /// <inheritdoc cref="Center(RangePrimitive.Range{UnityEngine.Vector2})"/>
        public static Vector3 Center(this Range<Vector3Int> range)
        {
            return range.Lerp(0.5f);
        }

        #endregion
    }
}