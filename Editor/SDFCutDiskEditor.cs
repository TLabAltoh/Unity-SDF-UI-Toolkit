using UnityEditor;

namespace TLab.UI.SDF.Editor
{
	[CustomEditor(typeof(SDFCutDisk), true)]
	[CanEditMultipleObjects]
	public class SDFCutDiskEditor : SDFUIEditor
	{
		private SDFCutDisk m_instance;

		protected override void OnEnable()
		{
			base.OnEnable();

			m_instance = target as SDFCutDisk;
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