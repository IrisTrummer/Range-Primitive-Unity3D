using UnityEngine;

namespace Tests
{
    public static class TestHelper
    {
        internal static bool EqualsApproximately(Vector2 v1, Vector2 v2)
        {
            return Mathf.Approximately(v1.x, v2.x) && Mathf.Approximately(v1.y, v2.y);
        }
        
        internal static bool EqualsApproximately(Vector3 v1, Vector3 v2)
        {
            return Mathf.Approximately(v1.x, v2.x) && Mathf.Approximately(v1.y, v2.y) && Mathf.Approximately(v1.z, v2.z);
        }

        internal static float GenerateRandomValueInRange(System.Random random, float min, float max)
        {
            float smaller = Mathf.Min(min, max);
            float larger = Mathf.Max(min, max);
            return (float) random.NextDouble() * (larger - smaller) + smaller;
        }
        
        internal static int GenerateRandomValueInRange(System.Random random, int min, int max)
        {
            return random.Next(Mathf.Min(min, max), Mathf.Max(min, max));
        }
    }
}