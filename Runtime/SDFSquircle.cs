using UnityEditor;
#if UNITY_EDITOR
using UnityEngine;
#endif

namespace TLab.UI.SDF
{
    public class SDFSquircle : SDFSquircleBased
    {
#if UNITY_EDITOR
        [MenuItem("GameObject/UI/SDFUI/SDFSquircle", false)]
        private static void Create(MenuCommand menuCommand)
        {
            Create<SDFSquircle>(menuCommand);
        }
#endif

        protected override string SHADER_NAME => "Hidden/UI/SDF/Squircle/Outline";

        [SerializeField, Min(1)] protected int m_iteration = 4;

        public static readonly int PROP_ITERATION = Shader.PropertyToID("_Iteration");

        public int iteration
        {
            get => m_iteration;
            set
            {
                if (m_iteration != value)
                {
                    m_iteration = value;

                    SetAllDirty();
                }
            }
        }

        protected override void UpdateMaterialRecord()
        {
            base.UpdateMaterialRecord();

            _materialRecord.SetInteger(PROP_ITERATION, m_iteration);
        }
    }
}
