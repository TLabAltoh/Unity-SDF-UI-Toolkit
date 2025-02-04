using UnityEditor;

namespace TLab.UI.SDF.Editor
{
    [CustomEditor(typeof(SDFApproxSquircle), true)]
    [CanEditMultipleObjects]
    public class SDFApproxSquircleEditor : SDFUIEditor
    {
        private SDFApproxSquircle m_instance;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_instance = target as SDFApproxSquircle;
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
