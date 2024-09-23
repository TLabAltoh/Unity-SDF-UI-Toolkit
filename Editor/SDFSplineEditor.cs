using UnityEngine;
using UnityEditor;

namespace TLab.UI.SDF.Editor
{
	[CustomEditor(typeof(SDFSpline), true)]
	[CanEditMultipleObjects]
	public class SDFSplineEditor : SDFUIEditor
	{
		private SDFSpline m_instance;

		protected override void OnEnable()
		{
			base.OnEnable();

			m_instance = target as SDFSpline;
		}

		protected override void DrawShapeProp()
		{
			base.DrawShapeProp();
			EditorGUI.indentLevel++;
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.width), "Width");
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.isClosed), "Is Closed");
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.fill), "Fill");
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.splines), "Splines");
			EditorGUI.indentLevel--;
		}

		protected unsafe void OnSceneGUI()
		{
			for (var i = 0; i < m_instance.splines.Length; i++)
			{
				EditorGUI.BeginChangeCheck();
				var controls = m_instance.GetControls(i, true);
				fixed (Vector2* start = controls)
					for (Vector2* current = start; current < (start + controls.Length); current++)
						*current = Handles.PositionHandle(*current, Quaternion.identity);
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(m_instance, $"[{nameof(SDFSpline)}] Edit");
					m_instance.SetControls(i, controls, true);
				}
			}
		}
	}
}
