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

		protected Vector2[] Edit()
		{
			var controls = new Vector2[m_instance.length];
			for (int i = 0; i < controls.Length; i++)
			{
				var oldPos = m_instance.GetControl(i, true);
				var newPos = Handles.PositionHandle(oldPos, Quaternion.identity);

				controls[i] = newPos;
			}
			return controls;
		}

		protected override void DrawShapeProp()
		{
			base.DrawShapeProp();
			EditorGUI.indentLevel++;
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.width), "Width");
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.closed), "Closed");
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.fill), "Fill");
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.reverse), "Reverse");
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.controls), "Controls");
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.renderMode), "RenderMode");
			EditorGUI.indentLevel--;
		}

		protected void OnSceneGUI()
		{
			EditorGUI.BeginChangeCheck();

			var controls = Edit();

			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(m_instance, $"[{nameof(SDFSpline)}] Edit");

				for (int i = 0; i < controls.Length; i++)
					m_instance.SetControl(i, controls[i], true);
			}
		}
	}
}
