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
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.fill), "Fill");
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.splines), "Splines");
			EditorGUI.indentLevel--;
		}

		protected void MoveHandleGUI(in SDFSpline.QuadraticBezier bezier, int i, int index)
		{
			EditorGUI.BeginChangeCheck();
			var controls = m_instance.GetControls(i, true);
			controls[index] = Handles.PositionHandle(controls[index], Quaternion.identity);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(m_instance, $"[{nameof(SDFSpline)}] Edit");
				m_instance.SetControls(i, controls, true);
			}
		}

		protected void MoveLastHandleGUI(in SDFSpline.QuadraticBezier bezier, int i)
		{
			EditorGUI.BeginChangeCheck();
			var controls = m_instance.GetControls(i, true);
			controls[controls.Length - 1] = Handles.PositionHandle(controls[controls.Length - 1], Quaternion.identity);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(m_instance, $"[{nameof(SDFSpline)}] Edit");
				m_instance.SetControls(i, controls, true);
			}
		}

		protected unsafe void MoveHandleGUI(in SDFSpline.QuadraticBezier bezier, int i, int startIndex, int offset)
		{
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
		}

		public void MoveHandleGUILoop()
		{
			var count = m_instance.splines.Length;

			for (var i = 0; i < count; i++)
			{
				var bezier = m_instance[i];
				if (!bezier.active || (bezier.controls.Length == 0))
					continue;

				switch (bezier.curveMode)
				{
					case SDFSpline.QuadraticBezier.CurveMode.Free:
					case SDFSpline.QuadraticBezier.CurveMode.Line:
						MoveHandleGUI(bezier, i, 0, 1);
						break;
					default:    // SDFSpline.QuadraticBezier.CurveMode.Auto
						MoveHandleGUI(bezier, i, 0);
						MoveHandleGUI(bezier, i, 1, 2);

						var num = bezier.controls.Length;
						if (num % 2 == 1)
							MoveLastHandleGUI(bezier, i);
						else if (num > 1)
							MoveHandleGUI(bezier, i, num - 2);
						break;
				}
			}
		}

		protected unsafe void OnSceneGUI() => MoveHandleGUILoop();
	}
}
