using UnityEditor;

namespace TLab.UI.SDF.Editor
{
	[CustomEditor(typeof(SDFArc), true)]
	[CanEditMultipleObjects]
	public class SDFArcEditor : SDFUIEditor
	{
		private SDFArc m_instance;

		protected override void OnEnable()
		{
			base.OnEnable();

			m_instance = target as SDFArc;
		}

		protected override void DrawShapeProp()
		{
			base.DrawShapeProp();
			EditorGUI.indentLevel++;
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.fillAmount), "FillAmount");
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.ratio), "Ratio");
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.startAngle), "StartAngle");
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.cornersRounding), "CornersRounding");
			EditorGUI.indentLevel--;
		}
	}
}