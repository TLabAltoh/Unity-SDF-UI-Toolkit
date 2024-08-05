using UnityEditor;

namespace TLab.UI.SDF.Editor
{
	[CustomEditor(typeof(SDFRing), true)]
	[CanEditMultipleObjects]
	public class SDFRingEditor : SDFUIEditor
	{
		private SDFRing m_instance;

		protected override void OnEnable()
		{
			base.OnEnable();

			m_instance = target as SDFRing;
		}

		protected override void DrawShapeProp()
		{
			base.DrawShapeProp();
			EditorGUI.indentLevel++;
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.fillAmount), "FillAmount");
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.width), "Width");
			EditorGUI.indentLevel--;
		}
	}
}