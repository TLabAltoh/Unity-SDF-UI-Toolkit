using UnityEditor;

namespace TLab.UI.SDF.Editor
{
	[CustomEditor(typeof(SDFPie), true)]
	[CanEditMultipleObjects]
	public class SDFPieEditor : SDFUIEditor
	{
		private SDFPie m_instance;

		protected override void OnEnable()
		{
			base.OnEnable();

			m_instance = target as SDFPie;
		}

		protected override void DrawShapeProp()
		{
			base.DrawShapeProp();
			EditorGUI.indentLevel++;
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.fillAmount), "FillAmount");
			EditorGUI.indentLevel--;
		}
	}
}