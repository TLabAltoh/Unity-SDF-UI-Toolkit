using UnityEditor;

namespace TLab.UI.SDF.Editor
{
	[CustomEditor(typeof(SDFParallelogram), true)]
	[CanEditMultipleObjects]
	public class SDFParallelogramEditor : SDFUIEditor
	{
		private SDFParallelogram m_instance;

		protected override void OnEnable()
		{
			base.OnEnable();

			m_instance = target as SDFParallelogram;
		}

		protected override void DrawShapeProp()
		{
			base.DrawShapeProp();
			EditorGUI.indentLevel++;
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.slide), "Slide");
			EditorGUI.indentLevel--;
		}
	}
}