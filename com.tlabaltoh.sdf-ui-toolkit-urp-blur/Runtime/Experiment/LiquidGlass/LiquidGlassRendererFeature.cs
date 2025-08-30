using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace TLab.UI.SDF
{
    public class LiquidGlassRendererFeature : ScriptableRendererFeature
    {
        private LiquidGlassRenderPass m_liquidGlassRenderPass = null;

        public override void Create()
        {
            if (m_liquidGlassRenderPass == null)
                m_liquidGlassRenderPass = new LiquidGlassRenderPass();
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(m_liquidGlassRenderPass);
        }

        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
        {
            m_liquidGlassRenderPass.SetRenderTarget(renderer.cameraColorTargetHandle);
        }
    }
}
