using UnityEditor;
using UnityEngine;

namespace RangePrimitive.Editor
{
    [CustomPropertyDrawer(typeof(Range<Vector3>))]
    [CustomPropertyDrawer(typeof(Range<Vector3Int>))]
    public class RangePropertyDrawerVector3 : RangePropertyDrawerVector2
    {
        private const string ZCoordinatePath = "z";

        protected override void DrawPropertyFields(Rect rect, SerializedProperty property)
        {
            CreatePropertyFields(rect, property, GetRangePropertyPaths(), new[] { XCoordinatePath, YCoordinatePath, ZCoordinatePath });
        }
    }
}
