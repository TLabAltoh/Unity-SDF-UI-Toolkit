using UnityEngine;
using UnityEditor;

namespace TLab.UI.SDF.Editor
{
    [CustomPropertyDrawer(typeof(MinMaxRangeAttribute))]
    public class MinMaxRangeDrawer : PropertyDrawer
    {
        const float kPrefixPaddingRight = 2;
        const float kSpacing = 5;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);

            EditorGUI.BeginChangeCheck();

            var range = attribute as MinMaxRangeAttribute;
            float minValue = property.vector2Value.x;
            float maxValue = property.vector2Value.y;

            Rect labelPosition = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
            EditorGUI.LabelField(labelPosition, label);

            var lastIndentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            Rect sliderPosition = new Rect(
                position.x + EditorGUIUtility.labelWidth + kPrefixPaddingRight + EditorGUIUtility.fieldWidth + kSpacing,
                position.y,
                position.width - EditorGUIUtility.labelWidth - 2 * (EditorGUIUtility.fieldWidth + kSpacing) - kPrefixPaddingRight,
                position.height
            );
            EditorGUI.MinMaxSlider(sliderPosition, ref minValue, ref maxValue, range.min, range.max);

            Rect minPosition = new Rect(position.x + EditorGUIUtility.labelWidth + kPrefixPaddingRight, position.y, EditorGUIUtility.fieldWidth, position.height);
            minValue = EditorGUI.FloatField(minPosition, minValue);

            Rect maxPosition = new Rect(position.xMax - EditorGUIUtility.fieldWidth, position.y, EditorGUIUtility.fieldWidth, position.height);
            maxValue = EditorGUI.FloatField(maxPosition, maxValue);

            EditorGUI.indentLevel = lastIndentLevel;

            if (EditorGUI.EndChangeCheck())
                property.vector2Value = new Vector2(minValue, maxValue);

            EditorGUI.EndProperty();
        }
    }
}