using UnityEditor;

namespace TLab.UI.SDF.Editor
{
	[CustomEditor(typeof(SDFMoon), true)]
	[CanEditMultipleObjects]
	public class SDFMoonEditor : SDFUIEditor
	{
		private SDFMoon m_instance;

		protected override void OnEnable()
		{
			base.OnEnable();

			m_instance = target as SDFMoon;
		}

		protected override void DrawShapeProp()
		{
			base.DrawShapeProp();
			EditorGUI.indentLevel++;
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.innerRadius), "Inner Radius");
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.slide), "Slide");
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.roundness), "Roundness");
			EditorGUI.indentLevel--;
		}
	}
}