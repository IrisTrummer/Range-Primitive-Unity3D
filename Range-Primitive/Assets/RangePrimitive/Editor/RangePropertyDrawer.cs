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

        protected virtual int BaseRowAmount => 1;

        private static bool ShouldLineWrap => !EditorGUIUtility.wideMode;
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int rowAmount = BaseRowAmount + (ShouldLineWrap ? 1 : 0);
            return base.GetPropertyHeight(property, label) * rowAmount;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            Rect fieldRect = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // support for tooltip attribute
            Rect tooltipLabel = position;
            tooltipLabel.width -= fieldRect.width;
            EditorGUI.LabelField(tooltipLabel, new GUIContent("", property.tooltip));

            if (ShouldLineWrap)
                fieldRect = GetWrappedRect(position);

            DrawPropertyFields(fieldRect, property);

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

            foreach (string path in propertyPaths)
            {
                SerializedProperty p = property.FindPropertyRelative($"{path}");

                EditorGUI.PropertyField(propertyRect, p, new GUIContent(p.displayName));
                propertyRect.x += propertyWidth + SpacingBetweenFields;
            }
        }
        
        protected string[] GetRangePropertyPaths()
        {
            return new[] { MinPath, MaxPath };
        }
        
        private Rect GetWrappedRect(Rect startingRect)
        {
            EditorGUI.indentLevel++;

            Rect rect = EditorGUI.IndentedRect(startingRect);

            float lineHeight = rect.height / (BaseRowAmount + 1);
            rect.y += lineHeight;
            rect.height -= lineHeight;
            
            EditorGUI.indentLevel--;

            return rect;
        }
    }
}