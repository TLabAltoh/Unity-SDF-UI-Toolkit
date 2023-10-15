/***
* This code is adapted and modified from
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/ImageWithRoundedCorners.cs
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/Editor/ImageWithIndependentRoundedCornersInspector.cs
**/

using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Nobi.UiRoundedCorners.Editor
{
#if UNITY_EDITOR
	[CustomEditor(typeof(QuadRoundedCorners))]
	public class Vector4Editor : UnityEditor.Editor
	{
		private QuadRoundedCorners script;

		private void OnEnable()
		{
			script = (QuadRoundedCorners)target;
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			SerializedProperty prop;

			prop = serializedObject.FindProperty("radiusX");
			EditorGUILayout.PropertyField(prop, new GUIContent("Top Right Corner"));

			prop = serializedObject.FindProperty("radiusY");
			EditorGUILayout.PropertyField(prop, new GUIContent("Bottom Right Corner"));

			prop = serializedObject.FindProperty("radiusZ");
			EditorGUILayout.PropertyField(prop, new GUIContent("Top Left Corner"));

			prop = serializedObject.FindProperty("radiusW");
			EditorGUILayout.PropertyField(prop, new GUIContent("Bottom Left Corner"));

			prop = serializedObject.FindProperty("outlineWidth");
			EditorGUILayout.PropertyField(prop, new GUIContent("OutlineWidth"));

			prop = serializedObject.FindProperty("outlineColor");
			EditorGUILayout.PropertyField(prop, new GUIContent("OutlineColor"));

			serializedObject.ApplyModifiedProperties();

			if (!script.TryGetComponent<MaskableGraphic>(out var _))
			{
				EditorGUILayout.HelpBox("This script requires an MaskableGraphic (Image or RawImage) component on the same gameobject", MessageType.Warning);
			}
		}
	}
#endif
}
