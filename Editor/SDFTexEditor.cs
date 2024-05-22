/***
* This code is adapted and modified from
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/ImageWithRoundedCorners.cs
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/Editor/ImageWithIndependentRoundedCornersInspector.cs
**/

using UnityEngine.UI;
using UnityEditor;

namespace TLab.UI.SDF.Editor
{
	[CustomEditor(typeof(SDFTex))]
	public class SDFTexEditor : UnityEditor.Editor
	{
		private SDFTex m_instance;

		private void OnEnable()
		{
			m_instance = (SDFTex)target;
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			serializedObject.TryDrawProperty("m_" + nameof(m_instance.radius), "Radius");
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.sdfTexture), "SDFTexture");

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
}
