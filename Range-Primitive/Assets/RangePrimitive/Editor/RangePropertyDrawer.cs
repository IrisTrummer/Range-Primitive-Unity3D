using UnityEditor;
using UnityEngine;

namespace RangePrimitive.Editor
{
    [CustomPropertyDrawer(typeof(Range<int>))]
    [CustomPropertyDrawer(typeof(Range<float>))]
    public class RangePropertyDrawer : PropertyDrawer
    {
        private const float LabelWidth = 30f;
        private const float SpacingBetweenFields = 5f;

        private const string MinPath = "min";
        private const string MaxPath = "max";

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            Rect tooltipLabel = position;
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // support for tooltip attribute
            tooltipLabel.width -= position.width;
            EditorGUI.LabelField(tooltipLabel, new GUIContent("", property.tooltip));

            DrawPropertyFields(position, property);

            EditorGUI.EndProperty();
        }

        protected virtual void DrawPropertyFields(Rect rect, SerializedProperty property)
        {
            CreateRowOfPropertyFields(rect, property, GetRangePropertyPaths(), LabelWidth);
        }

        protected void CreateRowOfPropertyFields(Rect rect, SerializedProperty property, string[] propertyPaths, float propertyLabelWidth, string rowLabel = "")
        {
            float labelWidth = string.IsNullOrWhiteSpace(rowLabel) ? 0 : LabelWidth;
            float propertyWidth = (rect.width - labelWidth - (propertyPaths.Length - 1) * SpacingBetweenFields) / propertyPaths.Length;

            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUI.LabelField(rect, rowLabel);
            rect.x += labelWidth;

            EditorGUIUtility.labelWidth = propertyLabelWidth;
            Rect propertyRect = new Rect(rect.x, rect.y, propertyWidth, rect.height);

            foreach (string columnPath in propertyPaths)
            {
                var vectorProperty = property.FindPropertyRelative($"{columnPath}");

                EditorGUI.PropertyField(propertyRect, vectorProperty, new GUIContent(vectorProperty.displayName));
                propertyRect.x += propertyWidth + SpacingBetweenFields;
            }
        }
        
        protected string[] GetRangePropertyPaths()
        {
            return new[] { MinPath, MaxPath };
        }
    }
}