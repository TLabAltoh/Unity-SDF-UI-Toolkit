using UnityEngine;
using UnityEditor;

namespace TLab.UI.SDF.Editor
{
	[CustomEditor(typeof(SDFTriangle), true)]
	[CanEditMultipleObjects]
	public class SDFTriangleEditor : SDFUIEditor
	{
		private SDFTriangle m_instance;

		private Rect m_area;

		private int m_index = -1;

		private string[] m_corners = new string[] { "m_corner0", "m_corner1", "m_corner2" };

		protected override void OnEnable()
		{
			base.OnEnable();

			m_instance = target as SDFTriangle;
		}

		private void Draw()
		{
			var rectT = m_instance.gameObject.transform as RectTransform;

			var rectTSize = new Vector2(rectT.rect.width, rectT.rect.height);

			m_area = EditorUtil.CreatePreviewArea((float)rectT.rect.height / rectT.rect.width);

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

		private void Edit()
		{
			var rectT = m_instance.gameObject.transform as RectTransform;

			var rectTSize = new Vector2(rectT.rect.width, rectT.rect.height);

			var rectTPosition = EditorUtil.RectToRectTransform(Event.current.mousePosition, m_area, rectTSize);

			if (Event.current.button == 0)
			{
				switch (Event.current.rawType)
				{
					case EventType.MouseDown:
						{
							if (EditorUtil.CheckArea(Event.current.mousePosition, m_area))
							{
								for (int i = 0; i < m_corners.Length; i++)
								{
									var corner = Vector2.zero;

									switch (i)
									{
										case 0:
											corner = m_instance.corner0;
											break;
										case 1:
											corner = m_instance.corner1;
											break;
										case 2:
											corner = m_instance.corner2;
											break;
									}

									if (Vector2.Distance(rectTPosition, corner) < 10)
									{
										m_index = i;

										break;
									}
								}
							}
						}
						break;
					case EventType.MouseDrag:
						{
							if (m_index != -1 && EditorUtil.CheckArea(Event.current.mousePosition, m_area))
							{
								switch (m_index)
								{
									case 0:
										m_instance.corner0 = rectTPosition;
										break;
									case 1:
										m_instance.corner1 = rectTPosition;
										break;
									case 2:
										m_instance.corner2 = rectTPosition;
										break;
								}
							}
						}
						break;
					case EventType.MouseUp:
						{
							m_index = -1;
						}
						break;
				}
			}
		}

		protected override void DrawCustomProp()
		{
			EditorGUI.indentLevel++;
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.radius), "Radius");
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.corner0), "Corner0");
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.corner1), "Corner1");
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.corner2), "Corner2");
			EditorGUI.indentLevel--;
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			DrawProp();

			DrawCustomProp();

			DrawOnionProp();

			DrawOutlineProp();

			DrawShadowProp();

			EditorGUILayout.Space();

			Draw();

			serializedObject.Call(() =>
			{
				Edit();

				EditorUtility.SetDirty(m_instance);
			});

			EditorGUILayout.Space();

			serializedObject.ApplyModifiedProperties();
		}
	}
}