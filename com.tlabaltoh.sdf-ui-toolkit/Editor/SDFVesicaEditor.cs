using UnityEditor;

namespace TLab.UI.SDF.Editor
{
	[CustomEditor(typeof(SDFVesica), true)]
	[CanEditMultipleObjects]
	public class SDFVesicaEditor : SDFUIEditor
	{
		private SDFVesica m_instance;

		protected override void OnEnable()
		{
			base.OnEnable();

			m_instance = target as SDFVesica;
		}

		protected override void DrawShapeProp()
		{
			base.DrawShapeProp();
			EditorGUI.indentLevel++;
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.roundness), "Roundness");
			EditorGUI.indentLevel--;
		}
	}
}