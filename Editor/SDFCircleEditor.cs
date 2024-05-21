/***
* This code is adapted and modified from
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/ImageWithRoundedCorners.cs
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/Editor/ImageWithIndependentRoundedCornersInspector.cs
**/

using UnityEngine.UI;
using UnityEditor;

namespace TLab.UI.SDF.Editor
{
	[CustomEditor(typeof(SDFCircle))]
	public class SDFCircleEditor : UnityEditor.Editor
	{
		private SDFCircle m_instance;

		private void OnEnable()
		{
			m_instance = target as SDFCircle;
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			serializedObject.TryDrawProperty(nameof(m_instance.radius), "Radius");

			serializedObject.TryDrawProperty(nameof(m_instance.onion), "Onion");
			serializedObject.TryDrawProperty(nameof(m_instance.onionWidth), "OnionWidth");

			serializedObject.TryDrawProperty(nameof(m_instance.outline), "Outline");
			serializedObject.TryDrawProperty(nameof(m_instance.outlineWidth), "OutlineWidth");
			serializedObject.TryDrawProperty(nameof(m_instance.outlineColor), "OutlineColor");

			serializedObject.ApplyModifiedProperties();

			if (!m_instance.TryGetComponent<MaskableGraphic>(out var _))
			{
				EditorGUILayout.HelpBox("This m_instance requires an MaskableGraphic (Image or RawImage) component on the same gameobject", MessageType.Warning);
			}
		}
	}
}