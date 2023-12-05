using UnityEditor;
using UnityEngine;

namespace RangePrimitive.Editor
{
    [CustomPropertyDrawer(typeof(Range<int>))]
    [CustomPropertyDrawer(typeof(Range<float>))]
    public class RangePropertyDrawer : PropertyDrawer
    {
        private const float SpacingBetweenFields = 10f;
        private const float LabelWidth = 30f;
        private const int LabelHideWidth = 130;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            Rect tooltipLabel = position;
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            
            // support for tooltip attribute
            tooltipLabel.width -= position.width;
            EditorGUI.LabelField(tooltipLabel, new GUIContent("", property.tooltip));
            
            // hide the "min" and "max" labels when the inspector window gets too small
            // so that there is more space to display the values
            bool hideLabels = position.width < LabelHideWidth;
            position.width = (position.width - SpacingBetweenFields) / 2f;
            EditorGUIUtility.labelWidth = LabelWidth;

            CreatePropertyField("min");
            
            position.x += position.width + SpacingBetweenFields;
            
            CreatePropertyField("max");

            EditorGUI.EndProperty();

            void CreatePropertyField(string relativePath)
            {
                SerializedProperty serializedProperty = property.FindPropertyRelative(relativePath);
                EditorGUI.PropertyField(position, serializedProperty, hideLabels ? GUIContent.none : new GUIContent(serializedProperty.displayName));
            }
        }
    }
}
