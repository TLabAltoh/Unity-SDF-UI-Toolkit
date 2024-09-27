/***
* This code is adapted and modified from
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/ImageWithRoundedCorners.cs
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/Editor/ImageWithIndependentRoundedCornersInspector.cs
**/

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TLab.UI.SDF.Editor
{
	[CustomEditor(typeof(SDFQuad), true)]
	[CanEditMultipleObjects]
	public class SDFQuadEditor : SDFUIEditor
	{
		private SDFQuad m_sdfQued;

		protected override void OnEnable()
		{
			base.OnEnable();

			m_sdfQued = target as SDFQuad;
		}

		protected override void DrawShapeProp()
		{
			serializedObject.TryDrawProperty("m_" + nameof(m_sdfQued.independent), "Independent Corner");
			EditorGUI.indentLevel++;
			if (m_sdfQued.independent)
			{
				EditorGUILayout.BeginHorizontal();
				{
					EditorGUILayout.LabelField("Corners");
					EditorGUILayout.BeginVertical();
					{
						serializedObject.TryDrawPropertyNoLabel("m_" + nameof(m_sdfQued.radiusZ));
						serializedObject.TryDrawPropertyNoLabel("m_" + nameof(m_sdfQued.radiusW));
					}
					EditorGUILayout.EndVertical();
					EditorGUILayout.BeginVertical();
					{
						serializedObject.TryDrawPropertyNoLabel("m_" + nameof(m_sdfQued.radiusX));
						serializedObject.TryDrawPropertyNoLabel("m_" + nameof(m_sdfQued.radiusY));
					}
					EditorGUILayout.EndVertical();
				}
				EditorGUILayout.EndHorizontal();
			}
			else
			{
				serializedObject.TryDrawProperty("m_" + nameof(m_sdfQued.radius), "Corner");
			}
			EditorGUI.indentLevel--;
		}
	}
}
