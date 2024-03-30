using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace TLab.UI.RoundedCorners.Editor
{
	[CustomEditor(typeof(CustomShape))]
	public class CustomShapeEditor : UnityEditor.Editor
	{
		private CustomShape m_instance;

		private void OnEnable()
		{
			m_instance = (CustomShape)target;
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			SerializedProperty prop;

			prop = serializedObject.FindProperty("radius");
			EditorGUILayout.PropertyField(prop, new GUIContent("Radius"));

			prop = serializedObject.FindProperty("sdfTexture");
			EditorGUILayout.PropertyField(prop, new GUIContent("SDFTexture"));

			prop = serializedObject.FindProperty("outlineWidth");
			EditorGUILayout.PropertyField(prop, new GUIContent("OutlineWidth"));

			prop = serializedObject.FindProperty("outlineColor");
			EditorGUILayout.PropertyField(prop, new GUIContent("OutlineColor"));

			serializedObject.ApplyModifiedProperties();

			if (!m_instance.TryGetComponent<MaskableGraphic>(out var _))
			{
				EditorGUILayout.HelpBox("This m_instance requires an MaskableGraphic (Image or RawImage) component on the same gameobject", MessageType.Warning);
			}
		}
	}
}