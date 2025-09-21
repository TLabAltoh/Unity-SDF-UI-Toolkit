using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace TLab.UI.SDF
{
    public class LiquidGlassRendererFeature : ScriptableRendererFeature
    {
        private LiquidGlassRenderPass m_liquidGlassRenderPass = null;
        [SerializeField] private RenderPassEvent m_renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;

        public override void Create()
        {
            if (m_liquidGlassRenderPass == null)
                m_liquidGlassRenderPass = new LiquidGlassRenderPass(m_renderPassEvent);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(m_liquidGlassRenderPass);
        }

#if !UNITY_6000_0_OR_NEWER || URP_COMPATIBILITY_MODE
        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
        {
            m_liquidGlassRenderPass.SetRenderTarget(renderer.cameraColorTargetHandle);
        }
#endif

        protected override void Dispose(bool disposing)
        {
            if ((m_liquidGlassRenderPass == null) || (m_liquidGlassRenderPass.disposed))
            {
                return;
            }

            if (m_liquidGlassRenderPass.TryDispose(out var material))
            {
                if (Application.isPlaying)
                {
                    Destroy(material);
                }
                else
                {
                    DestroyImmediate(material);
                }
            }
        }
    }
}
