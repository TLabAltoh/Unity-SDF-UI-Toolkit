/***
* This code is adapted and modified from
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/ImageWithRoundedCorners.cs
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/Editor/ImageWithIndependentRoundedCornersInspector.cs
**/

using UnityEngine.UI;
using UnityEditor;

namespace TLab.UI.SDF.Editor
{
	[CustomEditor(typeof(SDFArc))]
	public class SDFArcEditor : UnityEditor.Editor
	{
		private SDFArc m_instance;

		private void OnEnable()
		{
			m_instance = target as SDFArc;
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			serializedObject.TryDrawProperty("m_" + nameof(m_instance.radius), "Radius");
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.theta), "Theta");
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.width), "Width");

			serializedObject.TryDrawProperty("m_" + nameof(m_instance.onion), "Onion");
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.onionWidth), "OnionWidth");

			serializedObject.TryDrawProperty("m_" + nameof(m_instance.shadow), "Shadow");
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.shadowWidth), "ShadowWidth");
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.shadowBlur), "ShadowBlur");
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.shadowPower), "shadowPower");
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.shadowColor), "ShadowColor");

			serializedObject.TryDrawProperty("m_" + nameof(m_instance.outline), "Outline");
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.outlineWidth), "OutlineWidth");
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.outlineColor), "OutlineColor");

			serializedObject.TryDrawProperty("m_" + nameof(m_instance.sprite), "Frame");
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.mainTextureScale), "Scale");
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.mainTextureOffset), "Offset");
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.mainColor), "Color");

			serializedObject.ApplyModifiedProperties();

			if (!m_instance.TryGetComponent<MaskableGraphic>(out var _))
			{
				EditorGUILayout.HelpBox("This m_instance requires an MaskableGraphic (Image or RawImage) component on the same gameobject", MessageType.Warning);
			}
		}
	}
}