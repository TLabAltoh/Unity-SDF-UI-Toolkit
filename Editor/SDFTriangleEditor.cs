using UnityEngine;
using UnityEditor;

namespace TLab.UI.SDF.Editor
{
	[CustomEditor(typeof(SDFTriangle), true)]
	[CanEditMultipleObjects]
	public class SDFTriangleEditor : SDFUIEditor
	{
		private SDFTriangle m_instance;

		protected override void OnEnable()
		{
			base.OnEnable();

			m_instance = target as SDFTriangle;
		}

		protected Vector2[] Edit()
		{
			var corners = new Vector2[3];
			for (int i = 0; i < 3; i++)
			{
				var oldPos = m_instance.GetCorner(i, true);
				var newPos = Handles.PositionHandle(oldPos, Quaternion.identity);

				corners[i] = newPos;
			}
			return corners;
		}

		protected override void DrawShapeProp()
		{
			base.DrawShapeProp();
			EditorGUI.indentLevel++;
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.radius), "Radius");
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.corner0), "Corner0");
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.corner1), "Corner1");
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.corner2), "Corner2");
			EditorGUI.indentLevel--;
		}

		protected void OnSceneGUI()
		{
			EditorGUI.BeginChangeCheck();

			var corners = Edit();

			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(m_instance, $"[{nameof(SDFTriangle)}] Edit");

				for (int i = 0; i < corners.Length; i++)
					m_instance.SetCorner(i, corners[i], true);
			}
		}
	}
}