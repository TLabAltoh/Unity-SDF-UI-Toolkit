using UnityEditor;

namespace TLab.UI.SDF.Editor
{
    [CustomEditor(typeof(SDFSquircle), true)]
    [CanEditMultipleObjects]
    public class SDFSquircleEditor : SDFUIEditor
    {
        private SDFSquircle m_instance;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_instance = target as SDFSquircle;
        }


        protected override void DrawShapeProp()
        {
            base.DrawShapeProp();
            EditorGUI.indentLevel++;
            serializedObject.TryDrawProperty("m_" + nameof(m_instance.roundness), "Roundness");
            serializedObject.TryDrawProperty("m_" + nameof(m_instance.iteration), "Iteration");
            EditorGUI.indentLevel--;
        }
    }
}
