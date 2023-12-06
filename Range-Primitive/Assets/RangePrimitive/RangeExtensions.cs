using UnityEngine;

namespace RangePrimitive
{
    public static class RangeExtensions
    {
        # region Random

        /// <summary>
        /// Returns a random value between min and max (both inclusive).
        /// </summary>
        public static int Random(this Range<int> range)
        {
            return UnityEngine.Random.Range(range.Min, range.Max + 1);
        }

        /// <inheritdoc cref="Random(RangePrimitive.Range{int})"/>
        public static float Random(this Range<float> range)
        {
            return UnityEngine.Random.Range(range.Min, range.Max);
        }
        
        /// <summary>
        /// Returns a new vector where each component has a random value between the corresponding min and max component of the range.
        /// </summary>
        public static Vector2 Random(this Range<Vector2> range)
        {
            return new Vector2(UnityEngine.Random.Range(range.Min.x, range.Max.x), UnityEngine.Random.Range(range.Min.y, range.Max.y));
        }
        
        /// <inheritdoc cref="Random(RangePrimitive.Range{Vector2})"/>
        public static Vector2Int Random(this Range<Vector2Int> range)
        {
            return new Vector2Int(UnityEngine.Random.Range(range.Min.x, range.Max.x), UnityEngine.Random.Range(range.Min.y, range.Max.y));
        }

        /// <inheritdoc cref="Random(RangePrimitive.Range{Vector2})"/>
        public static Vector3 Random(this Range<Vector3> range)
        {
            return new Vector3(UnityEngine.Random.Range(range.Min.x, range.Max.x), UnityEngine.Random.Range(range.Min.y, range.Max.y), UnityEngine.Random.Range(range.Min.z, range.Max.z));
        }
        
        /// <inheritdoc cref="Random(RangePrimitive.Range{Vector2})"/>
        public static Vector3Int Random(this Range<Vector3Int> range)
        {
            return new Vector3Int(UnityEngine.Random.Range(range.Min.x, range.Max.x), UnityEngine.Random.Range(range.Min.y, range.Max.y), UnityEngine.Random.Range(range.Min.z, range.Max.z));
        }

        #endregion
        
        # region Lerp
        
        /// <summary>
        /// Returns the linearly interpolated value between min and max by t.
        /// </summary>
        public static int Lerp(this Range<int> range, float t)
        {
            return (int) Mathf.Lerp(range.Min, range.Max, t);
        }

        /// <inheritdoc cref="Lerp(RangePrimitive.Range{int},float)"/>
        public static float Lerp(this Range<float> range, float t)
        {
            return Mathf.Lerp(range.Min, range.Max, t);
        }
        
        /// <summary>
        /// Returns a value between 0 and 1 indicating where the given value lies between min and max.
        /// </summary>
        public static float InverseLerp(this Range<int> range, int value)
        {
            return Mathf.InverseLerp(range.Min, range.Max, value);
        }

        /// <inheritdoc cref="InverseLerp(RangePrimitive.Range{int},int)"/>
        public static float InverseLerp(this Range<float> range, float value)
        {
            return Mathf.InverseLerp(range.Min, range.Max, value);
        }
        
        /// <summary>
        /// Returns the interpolated value between min and max by t with smoothing at the edges.
        /// </summary>
        public static int SmoothStep(this Range<int> range, float t)
        {
            return (int) Mathf.SmoothStep(range.Min, range.Max, t);
        }

        /// <inheritdoc cref="SmoothStep(RangePrimitive.Range{int},float)"/>
        public static float SmoothStep(this Range<float> range, float t)
        {
            return Mathf.SmoothStep(range.Min, range.Max, t);
        }
        
        # endregion
        
        # region Delta
        
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
        /// Returns the absolute difference between min and max, which is equal to the span of the range.
        /// </summary>
        public static int AbsDelta(this Range<int> range)
        {
            return Mathf.Abs(range.Delta());
        }

        /// <inheritdoc cref="AbsDelta(RangePrimitive.Range{int})"/>
        public static float AbsDelta(this Range<float> range)
        {
            return Mathf.Abs(range.Delta());
        }
        
        # endregion

        # region Clamp
        
        /// <summary>
        /// Clamps the given value between the min and max values of the range.
        /// </summary>
        public static int Clamp(this Range<int> range, int value)
        {
            return Mathf.Clamp(value, range.Min, range.Max);
        }
        
        /// <inheritdoc cref="Clamp(RangePrimitive.Range{int},int)"/>
        public static float Clamp(this Range<float> range, float value)
        {
            return Mathf.Clamp(value, range.Min, range.Max);
        }
        
        /// <summary>
        /// Returns a new vector where each component is clamped between the corresponding min and max component of the range.
        /// </summary>
        public static Vector2 Clamp(this Range<Vector2> range, Vector2 value)
        {
            return new Vector2(Mathf.Clamp(value.x, range.Min.x, range.Max.x), Mathf.Clamp(value.y, range.Min.y, range.Max.y));
        }
        
        /// <inheritdoc cref="Clamp(RangePrimitive.Range{Vector2},Vector2)"/>
        public static Vector2Int Clamp(this Range<Vector2Int> range, Vector2Int value)
        {
            return new Vector2Int(Mathf.Clamp(value.x, range.Min.x, range.Max.x), Mathf.Clamp(value.y, range.Min.y, range.Max.y));
        }
        
        /// <inheritdoc cref="Clamp(RangePrimitive.Range{Vector2},Vector2)"/>
        public static Vector3 Clamp(this Range<Vector3> range, Vector3 value)
        {
            return new Vector3(Mathf.Clamp(value.x, range.Min.x, range.Max.x), Mathf.Clamp(value.y, range.Min.y, range.Max.y), Mathf.Clamp(value.z, range.Min.z, range.Max.z));
        }
        
        /// <inheritdoc cref="Clamp(RangePrimitive.Range{Vector2},Vector2)"/>
        public static Vector3Int Clamp(this Range<Vector3Int> range, Vector3Int value)
        {
            return new Vector3Int(Mathf.Clamp(value.x, range.Min.x, range.Max.x), Mathf.Clamp(value.y, range.Min.y, range.Max.y), Mathf.Clamp(value.z, range.Min.z, range.Max.z));
        }
        
        # endregion
    }
}