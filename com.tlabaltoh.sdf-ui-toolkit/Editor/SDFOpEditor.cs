using UnityEditor;

namespace TLab.UI.SDF.Editor
{
    [CustomEditor(typeof(SDFOp), true)]
    [CanEditMultipleObjects]
    public class SDFOpEditor : SDFUIEditor
    {
		private SDFOp m_instance;

		protected override void OnEnable()
		{
			base.OnEnable();

			m_instance = target as SDFOp;
		}

		protected override void DrawShapeProp()
		{
			base.DrawShapeProp();
			EditorGUI.indentLevel++;
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.elements), "Elements");
			EditorGUI.indentLevel--;
		}
	}
}
