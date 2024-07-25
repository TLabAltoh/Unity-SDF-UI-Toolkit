using UnityEditor;

namespace TLab.UI.SDF.Editor
{
	[CustomEditor(typeof(SDFRing), true)]
	[CanEditMultipleObjects]
	public class SDFRingEditor : SDFUIEditor
	{
		private SDFRing m_instance;

		protected override void DrawShapeProp()
		{
			base.DrawShapeProp();
			EditorGUI.indentLevel++;
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.radius), "Radius");
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.theta), "Theta");
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.width), "Width");
			EditorGUI.indentLevel--;
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			m_instance = target as SDFRing;
		}
	}
}