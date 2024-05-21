/***
* This code is adapted and modified from
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/ImageWithRoundedCorners.cs
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/Editor/ImageWithIndependentRoundedCornersInspector.cs
**/

using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace TLab.UI.SDF.Editor
{
	[CustomEditor(typeof(SDFTriangle))]
	public class SDFTriangleEditor : UnityEditor.Editor
	{
		private SDFTriangle m_instance;

		private Rect m_area;

		private int m_index = -1;

		private string[] m_corners = new string[] { "corner0", "corner1", "corner2" };

		private void OnEnable()
		{
			m_instance = target as SDFTriangle;
		}

		private void DrawInput()
		{
			var rectT = m_instance.GetComponent<RectTransform>();

			m_area = EditorUtil.CreatePreviewArea((float)rectT.rect.height / rectT.rect.width);

			var rectTSize = new Vector2(rectT.rect.width, rectT.rect.height);

			var rectTPosition = EditorUtil.RectToRectTransform(Event.current.mousePosition, m_area, rectTSize);

			if (Event.current.rawType == EventType.MouseDown && Event.current.button == 0)
			{
				if (EditorUtil.CheckArea(Event.current.mousePosition, m_area))
				{
					for (int i = 0; i < m_corners.Length; i++)
					{
						var prop = serializedObject.FindProperty(m_corners[i]);

						if (Vector2.Distance(rectTPosition, prop.vector2Value) < 10)
						{
							m_index = i;

							break;
						}
					}
				}
			}
			else if (Event.current.rawType == EventType.MouseDrag && Event.current.button == 0)
			{
				if (m_index != -1 && EditorUtil.CheckArea(Event.current.mousePosition, m_area))
				{
					var prop = serializedObject.FindProperty(m_corners[m_index]);

					prop.vector2Value = rectTPosition;
				}
			}
			else if (Event.current.rawType == EventType.MouseUp && Event.current.button == 0)
			{
				m_index = -1;
			}

			var corners = new Vector3[3];

			var radius = 5f;

			for (int i = 0; i < m_corners.Length; i++)
			{
				var prop = serializedObject.FindProperty(m_corners[i]);

				corners[i] = EditorUtil.RectTransformToRect(prop.vector2Value, m_area, rectTSize);
			}

			Handles.color = Color.white;

			Handles.DrawAAConvexPolygon(corners);

			Handles.color = Color.green;

			for (int i = 0; i < m_corners.Length; i++)
			{
				Handles.DrawSolidDisc(corners[i], Vector3.forward, radius);
			}
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			serializedObject.TryDrawProperty(nameof(m_instance.radius), "Radius");
			serializedObject.TryDrawProperty(nameof(m_instance.corner0), "Corner0");
			serializedObject.TryDrawProperty(nameof(m_instance.corner1), "Corner1");
			serializedObject.TryDrawProperty(nameof(m_instance.corner2), "Corner2");

			serializedObject.TryDrawProperty(nameof(m_instance.onion), "Onion");
			serializedObject.TryDrawProperty(nameof(m_instance.onionWidth), "OnionWidth");

			serializedObject.TryDrawProperty(nameof(m_instance.outline), "Outline");
			serializedObject.TryDrawProperty(nameof(m_instance.outlineWidth), "OutlineWidth");
			serializedObject.TryDrawProperty(nameof(m_instance.outlineColor), "OutlineColor");

			EditorGUILayout.Space();

			DrawInput();

			EditorGUILayout.Space();

			serializedObject.ApplyModifiedProperties();

			if (!m_instance.TryGetComponent<MaskableGraphic>(out var _))
			{
				EditorGUILayout.HelpBox("This m_instance requires an MaskableGraphic (Image or RawImage) component on the same gameobject", MessageType.Warning);
			}
		}
	}
}