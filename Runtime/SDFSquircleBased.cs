using UnityEngine;

namespace TLab.UI.SDF
{
    public class SDFSquircleBased : SDFUI
    {
        [SerializeField, Min(1)] protected float m_roundness = 2.4f;

        public static readonly int PROP_MIN_SIZE = Shader.PropertyToID("_MinSize");
        public static readonly int PROP_ROUNDNESS = Shader.PropertyToID("_Roundness");

        public float roundness
        {
            get => m_roundness;
            set
            {
                if (m_roundness != value)
                {
                    m_roundness = value;

                    SetAllDirty();
                }
            }
        }

        protected override void UpdateMaterialRecord()
        {
            base.UpdateMaterialRecord();

            var minSize = this.minSize;
            _materialRecord.SetFloat(PROP_MIN_SIZE, minSize);
            _materialRecord.SetFloat(PROP_ROUNDNESS, m_roundness);
        }
    }
}
