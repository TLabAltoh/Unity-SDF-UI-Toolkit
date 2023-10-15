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
    [CustomEditor(typeof(CrossRoundedCorners))]
    public class CrossRoundedCornersEditor : UnityEditor.Editor
    {
		private CrossRoundedCorners script;

		private void OnEnable()
		{
			script = (CrossRoundedCorners)target;
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			SerializedProperty prop;

			prop = serializedObject.FindProperty("radius");
			EditorGUILayout.PropertyField(prop, new GUIContent("Radius"));

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
}
#endif