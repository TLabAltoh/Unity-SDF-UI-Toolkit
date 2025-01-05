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
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.close), "Close");
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.fill), "Fill");
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.curveMode), "Curve Mode");
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.splines), "Splines");
			EditorGUI.indentLevel--;
		}

		protected void MoveFirstHandleGUI()
		{
			var count = m_instance.splines.Length;

			for (var i = 0; i < count; i++)
			{
				var segment = m_instance[i];
				if (!segment.active || (segment.controls.Length == 0))
					continue;

				EditorGUI.BeginChangeCheck();
				var controls = m_instance.GetControls(i, true);
				controls[0] = Handles.PositionHandle(controls[0], Quaternion.identity);
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(m_instance, $"[{nameof(SDFSpline)}] Edit");
					m_instance.SetControls(i, controls, true);
				}
			}
		}

		protected void MoveLastHandleGUI()
		{
			var count = m_instance.splines.Length;

			for (var i = 0; i < count; i++)
			{
				var segment = m_instance[i];
				if (!segment.active || (segment.controls.Length == 0))
					continue;

				EditorGUI.BeginChangeCheck();
				var controls = m_instance.GetControls(i, true);
				controls[controls.Length - 1] = Handles.PositionHandle(controls[controls.Length - 1], Quaternion.identity);
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(m_instance, $"[{nameof(SDFSpline)}] Edit");
					m_instance.SetControls(i, controls, true);
				}
			}
		}

		protected unsafe void MoveHandleGUI(int startIndex, int offset)
		{
			var count = m_instance.splines.Length;

			for (var i = 0; i < count; i++)
			{
				var segment = m_instance[i];
				if (!segment.active || (segment.controls.Length == 0))
					continue;

				EditorGUI.BeginChangeCheck();
				var controls = m_instance.GetControls(i, true);
				fixed (Vector2* start = controls)
					for (Vector2* current = (start + startIndex); current < (start + controls.Length); current += offset)
						*current = Handles.PositionHandle(*current, Quaternion.identity);
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(m_instance, $"[{nameof(SDFSpline)}] Edit");
					m_instance.SetControls(i, controls, true);
				}

				if (controls.Length % 2 == 1)
					MoveLastHandleGUI();
			}
		}

		protected unsafe void OnSceneGUI()
		{
			switch (m_instance.curveMode)
			{
				case SDFSpline.CurveMode.Free:
					MoveHandleGUI(0, 1);
					break;
				default:    // SDFSpline.CurveMode.Auto
					MoveFirstHandleGUI();
					MoveHandleGUI(1, 2);
					break;
			}
		}
	}
}
