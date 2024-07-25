/***
* This code is adapted and modified from
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/ImageWithRoundedCorners.cs
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/Editor/ImageWithIndependentRoundedCornersInspector.cs
**/

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TLab.UI.SDF.Editor
{
	//#if UNITY_EDITOR
	[CustomEditor(typeof(SDFQuad))]
	public class SDFQuadEditor : UnityEditor.Editor
	{
		private SDFQuad m_instance;

		private void OnEnable()
		{
			m_instance = (SDFQuad)target;
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			GUIStyle style = new(EditorStyles.boldLabel)
			{
				fontSize = 16,
				contentOffset = new Vector2(18, 0),
			};
			EditorGUILayout.LabelField("Fill", style);
			EditorGUI.indentLevel++;
			{
				serializedObject.TryDrawProperty("m_" + nameof(m_instance.mainColor), "Color");
				serializedObject.TryDrawProperty("m_" + nameof(m_instance.sprite), "Sprite");
				if (m_instance.sprite != null)
				{
					serializedObject.TryDrawProperty("m_" + nameof(m_instance.mainTextureScale), "Scale");
					serializedObject.TryDrawProperty("m_" + nameof(m_instance.mainTextureOffset), "Offset");
				}
			}
			EditorGUI.indentLevel--;
			serializedObject.TryDrawLeftToggle("m_" + nameof(m_instance.independent), "Independent Corner");
			EditorGUI.indentLevel++;
			if (m_instance.independent)
			{
				EditorGUILayout.BeginHorizontal();
				{
					EditorGUILayout.LabelField("Corners");
					EditorGUILayout.BeginVertical();
					{
						serializedObject.TryDrawPropertyNoLabel("m_" + nameof(m_instance.radiusZ));
						serializedObject.TryDrawPropertyNoLabel("m_" + nameof(m_instance.radiusY));
					}
					EditorGUILayout.EndVertical();
					EditorGUILayout.BeginVertical();
					{
						serializedObject.TryDrawPropertyNoLabel("m_" + nameof(m_instance.radiusX));
						serializedObject.TryDrawPropertyNoLabel("m_" + nameof(m_instance.radiusW));
					}
					EditorGUILayout.EndVertical();
				}
				EditorGUILayout.EndHorizontal();
			}
			else
			{
				serializedObject.TryDrawProperty("m_" + nameof(m_instance.radius), "Corner");
			}
			EditorGUI.indentLevel--;

			serializedObject.TryDrawLeftToggle("m_" + nameof(m_instance.outline), "Outline");
			EditorGUI.indentLevel++;
			if (m_instance.outline)
			{
				serializedObject.TryDrawProperty("m_" + nameof(m_instance.outlineWidth), "OutlineWidth");
				serializedObject.TryDrawProperty("m_" + nameof(m_instance.outlineColor), "OutlineColor");
				serializedObject.TryDrawProperty("m_" + nameof(m_instance.outlineType), "OutlineType");
			}
			EditorGUI.indentLevel--;

			serializedObject.TryDrawLeftToggle("m_" + nameof(m_instance.onion), "Onion");
			EditorGUI.indentLevel++;
			if (m_instance.onion)
			{
				serializedObject.TryDrawProperty("m_" + nameof(m_instance.onionWidth), "OnionWidth");
			}
			EditorGUI.indentLevel--;

			serializedObject.TryDrawLeftToggle("m_" + nameof(m_instance.shadow), "Shadow");
			EditorGUI.indentLevel++;
			if (m_instance.shadow)
			{
				serializedObject.TryDrawProperty("m_" + nameof(m_instance.shadowColor), "ShadowColor");
				serializedObject.TryDrawProperty("m_" + nameof(m_instance.shadowOffset), "ShadowOffset");
				serializedObject.TryDrawProperty("m_" + nameof(m_instance.shadowWidth), "ShadowWidth");
				serializedObject.TryDrawProperty("m_" + nameof(m_instance.shadowBlur), "ShadowBlur");
				serializedObject.TryDrawProperty("m_" + nameof(m_instance.shadowPower), "shadowPower");
			}
			EditorGUI.indentLevel--;

			serializedObject.ApplyModifiedProperties();
		}
	}
	//#endif
}
