using UnityEditor;
using UnityEngine;

namespace RangePrimitive.Editor
{
    [CustomPropertyDrawer(typeof(Range<Vector2>))]
    [CustomPropertyDrawer(typeof(Range<Vector2Int>))]
    public class RangePropertyDrawerVector2 : RangePropertyDrawer
    {
        private const float SubLabelWidth = 12f;
        private const float SpacingBetweenLines = 2f;
        
        protected const string XCoordinatePath = "x";
        protected const string YCoordinatePath = "y";

        protected override int BaseRowAmount => 2;

        protected override void DrawPropertyFields(Rect rect, SerializedProperty property)
        {
            CreatePropertyFields(rect, property, GetRangePropertyPaths(), new[] { XCoordinatePath, YCoordinatePath });
        }

        protected void CreatePropertyFields(Rect rect, SerializedProperty property, string[] rowPaths, string[] columnPaths)
        {
            float propertyHeight = (rect.height - (rowPaths.Length - 1) - SpacingBetweenLines) / rowPaths.Length;
            rect.height = propertyHeight;

            foreach (string rowPath in rowPaths)
            {
                SerializedProperty p = property.FindPropertyRelative(rowPath);

                string[] paths = CreateCopyWithParentPath(columnPaths, p.name);
                CreateRowOfPropertyFields(rect, property, paths, SubLabelWidth, p.displayName);

                rect.y += rect.height + SpacingBetweenLines;
            }
        }

        private string[] CreateCopyWithParentPath(string[] paths, string parentPath)
        {
            string[] newPaths = new string[paths.Length];

            for (int i = 0; i < paths.Length; i++)
                newPaths[i] = $"{parentPath}.{paths[i]}";

            return newPaths;
        }
    }
}