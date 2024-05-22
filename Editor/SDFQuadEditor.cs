/***
* This code is adapted and modified from
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/ImageWithRoundedCorners.cs
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/Editor/ImageWithIndependentRoundedCornersInspector.cs
**/

using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TLab.UI.SDF.Editor
{
#if UNITY_EDITOR
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

			serializedObject.TryDrawProperty("m_" + nameof(m_instance.independent), "Independent Corner");

			if (m_instance.independent)
			{
				serializedObject.TryDrawProperty("m_" + nameof(m_instance.radiusX), "Top Right Corner");
				serializedObject.TryDrawProperty("m_" + nameof(m_instance.radiusY), "Bottom Right Corner");
				serializedObject.TryDrawProperty("m_" + nameof(m_instance.radiusZ), "Top Left Corner");
				serializedObject.TryDrawProperty("m_" + nameof(m_instance.radiusW), "Bottom Left Corner");
			}
			else
			{
				serializedObject.TryDrawProperty("m_" + nameof(m_instance.radius), "Corner");
			}

			serializedObject.TryDrawProperty("m_" + nameof(m_instance.onion), "Onion");
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.onionWidth), "OnionWidth");

			serializedObject.TryDrawProperty("m_" + nameof(m_instance.outline), "Outline");
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.outlineWidth), "OutlineWidth");
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.outlineColor), "OutlineColor");

			serializedObject.ApplyModifiedProperties();

			if (!m_instance.TryGetComponent<MaskableGraphic>(out var _))
			{
				EditorGUILayout.HelpBox("This m_instance requires an MaskableGraphic (Image or RawImage) component on the same gameobject", MessageType.Warning);
			}
		}
	}
#endif
}
