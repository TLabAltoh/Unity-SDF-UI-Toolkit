using UnityEditor;
using UnityEngine;


namespace TLab.UI.SDF.Editor
{
	[CustomPropertyDrawer(typeof(LeftToggleAttribute))]
	public class LeftToggleDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			property.boolValue = EditorGUI.ToggleLeft(position, label, property.boolValue, SerializeUtil.style);
			EditorGUI.EndProperty();
		}
	}
}