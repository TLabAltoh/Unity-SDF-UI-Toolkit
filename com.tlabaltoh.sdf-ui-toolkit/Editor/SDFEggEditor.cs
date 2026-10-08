using UnityEditor;

namespace TLab.UI.SDF.Editor
{
	[CustomEditor(typeof(SDFEgg), true)]
	[CanEditMultipleObjects]
	public class SDFEggEditor : SDFUIEditor
	{
		private SDFEgg m_instance;

		protected override void OnEnable()
		{
			base.OnEnable();

			m_instance = target as SDFEgg;
		}

		protected override void DrawShapeProp()
		{
			base.DrawShapeProp();
			EditorGUI.indentLevel++;
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.radiusA), "Radius A");
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.radiusB), "Radius B");
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.bluge), "Bluge");
			EditorGUI.indentLevel--;
		}
	}
}