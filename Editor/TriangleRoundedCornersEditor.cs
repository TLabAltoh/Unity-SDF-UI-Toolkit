/***
* This code is adapted and modified from
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/ImageWithRoundedCorners.cs
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/Editor/ImageWithIndependentRoundedCornersInspector.cs
**/

using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace Nobi.UiRoundedCorners.Editor
{
	[CustomEditor(typeof(TriangleRoundedCorners))]
	public class TriangleRoundedCornersEditor : UnityEditor.Editor
	{
		private TriangleRoundedCorners m_instance;

		private Rect m_area;

		private int m_index = -1;

		private string[] m_corners = new string[] { "corner0", "corner1", "corner2" };

		private void OnEnable()
		{
			m_instance = target as TriangleRoundedCorners;
		}

		private void DrawInput()
		{
			RectTransform rectTransform = m_instance.GetComponent<RectTransform>();

			m_area = EditorUtil.DrawPreviewArea(rectTransform.rect.width, rectTransform.rect.height);

			Vector2 input = Event.current.mousePosition;

			Vector2 rectTransformSize = new Vector2(rectTransform.rect.width, rectTransform.rect.height);

			Vector2 rectTransformPosition = EditorUtil.ScreenToRectTransformPosition(m_area, input, rectTransformSize);

			if (Event.current.rawType == EventType.MouseDown && Event.current.button == 0)
			{
				if (EditorUtil.InputAreaCheck(m_area, input))
				{
					for (int i = 0; i < m_corners.Length; i++)
					{
						SerializedProperty prop = serializedObject.FindProperty(m_corners[i]);

						if (Vector2.Distance(rectTransformPosition, prop.vector2Value) < 10)
						{
							m_index = i;

							break;
						}
					}
				}
			}
			else if (Event.current.rawType == EventType.MouseDrag && Event.current.button == 0)
			{
				if (m_index != -1 && EditorUtil.InputAreaCheck(m_area, input))
				{
					SerializedProperty prop = serializedObject.FindProperty(m_corners[m_index]);

					prop.vector2Value = rectTransformPosition;
				}
			}
			else if (Event.current.rawType == EventType.MouseUp && Event.current.button == 0)
			{
				m_index = -1;
			}

			Vector3[] corners = new Vector3[3];

			float radius = 5f;

			for (int i = 0; i < m_corners.Length; i++)
			{
				SerializedProperty prop = serializedObject.FindProperty(m_corners[i]);

				corners[i] = EditorUtil.RectTransformPositionToScreen(m_area, prop.vector2Value, rectTransformSize);
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

			serializedObject.TryDrawProperty("radius", "Radius");
			serializedObject.TryDrawProperty("corner0", "Corner0");
			serializedObject.TryDrawProperty("corner1", "Corner1");
			serializedObject.TryDrawProperty("corner2", "Corner2");
			serializedObject.TryDrawProperty("outlineWidth", "OutlineWidth");
			serializedObject.TryDrawProperty("outlineColor", "OutlineColor");

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