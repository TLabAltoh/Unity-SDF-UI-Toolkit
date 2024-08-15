#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Rendering;

namespace TLab.UI.SDF
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public class RenderPipelineUtil
    {
        public enum RenderPipelineType
        {
            BuiltIn,
            URP,
        }

        private static RenderPipelineType m_current = RenderPipelineType.BuiltIn;

        private static RenderPipelineAsset m_pilelineAssetCache = null;

        public static RenderPipelineType current
        {
            get
            {
                UpdateCurrent();
                return m_current;
            }
        }

        public static string shaderSuffix => current.ToString();

        private static bool IsInherited(Object obj, string target, string limmit = "")
        {
            var type = obj.GetType();

            if (type.Name == target)
                return true;

            while ((type.Name != "Object") && (type.Name != limmit))
            {
                if (type.Name == target)
                    return true;

                type = type.BaseType;
            }

            return false;
        }

        private static void UpdateCurrent()
        {
            if (m_pilelineAssetCache == GraphicsSettings.currentRenderPipeline)
            {
                return;
            }

            m_pilelineAssetCache = GraphicsSettings.currentRenderPipeline;

            if (m_pilelineAssetCache != null)
            {
                if (IsInherited(m_pilelineAssetCache, "UniversalRenderPipelineAsset", "RenderPipelineAsset"))
                    m_current = RenderPipelineType.URP;
            }
            else
            {
                m_current = RenderPipelineType.BuiltIn;
            }
        }
    }
}
