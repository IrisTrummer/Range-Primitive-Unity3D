using RangePrimitive;
using UnityEngine;

namespace Examples.Scripts
{
    public class RangeExamples : MonoBehaviour
    {
        [Header("Numerics")]
        [SerializeField, Tooltip("This defines a range between two int values.")]
        private Range<int> intRange = new(0, 5);

        [SerializeField]
        private Range<float> floatRange = new(-0.01f, 0.01f);

        [Header("Vectors")]
        [SerializeField, Tooltip("This defines a range between two vector values.")]
        private Range<Vector2> vector2Range = new(new Vector2(1, 100), new Vector2(99.5f, 100));

        [SerializeField]
        private Range<Vector2Int> vector2IntRange = new(new Vector2Int(10, -100), new Vector2Int(100, 100));

        [Space]
        [SerializeField, Tooltip("This defines a range between two three-dimensional vector values.")]
        private Range<Vector3> vector3Range;

        [SerializeField]
        private Range<Vector3Int> vector3IntRange = new(new Vector3Int(-10, 100, 1), new Vector3Int(1000, 100, 50));

        private void Awake()
        {
            Debug.Log("--- Range Examples ---");

            Debug.Log("-- One-Dimensional Examples --");
            OneDimensionalExamples();

            Debug.Log("-- Two-Dimensional Examples --");
            TwoDimensionalExamples();
        }

        private void OneDimensionalExamples()
        {
            // Generate 5 equally distributed samples between two values
            Range<int> range = new Range<int>(12, 168);

            for (int i = 0; i <= 4; i++)
            {
                Debug.Log(range.Lerp(i / 4f)); // Output: 12, 51, 90, 129, 168
            }

            // Get the spread of the defined range
            Debug.Log(range.Spread()); // Output: 156

            // Calculate at what point the value 37 lies between the start and end of the range
            Debug.Log(range.InverseLerp(37)); // Output: 0.16
        }

        private void TwoDimensionalExamples()
        {
            // Generate 3 random points in a defined 2D area
            Range<Vector2> range = new Range<Vector2>(new Vector2(-10, -10), new Vector2(10, 10));

            for (int i = 0; i < 3; i++)
            {
                Debug.Log(range.Random()); // Example output: (2.59, 4.79), (-4.82, 8.09), (8.21, 9.96)
            }

            // Check whether a point lies within the defined area
            Debug.Log(range.Contains(new Vector2(-8.3f, 15f))); // Output: False
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;

            Range<Vector2> range2D = new Range<Vector2>(new Vector2(-10, -10), new Vector2(10, 10));
            Gizmos.DrawWireCube(range2D.Center(), range2D.Spread());
        }
    }
}