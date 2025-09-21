using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
#if UNITY_6000_0_OR_NEWER && !URP_COMPATIBILITY_MODE
using UnityEngine.Rendering.RenderGraphModule;
#endif

namespace TLab.UI.SDF
{
    public class LiquidGlassRenderPass : ScriptableRenderPass
    {
        private const string NAME = nameof(LiquidGlassRenderPass);

#if !UNITY_6000_0_OR_NEWER || URP_COMPATIBILITY_MODE
        private RenderTargetIdentifier m_currentTarget = default;
#endif

        private Material m_blurMaterial;
        private Shader m_blurShader;
        private float[] m_blurWeights = new float[10];

        private int m_blurredTempID1 = 0;
        private int m_blurredTempID2 = 0;
        private int m_screenCopyID = 0;
        private int m_blurWeightsID = 0;
        private int m_blurOffsetsID = 0;
        private int m_grabBlurTextureID = 0;

        private bool m_disposed = false;
        public bool disposed => m_disposed;
        public Material blurMaterial => m_blurMaterial;

        public bool TryDispose(out Material material)
        {
            material = null;
            if (m_disposed)
            {
                return false;
            }
            material = m_blurMaterial;
            m_disposed = true;
            return true;
        }

        public LiquidGlassRenderPass()
        {
            m_blurShader = Shader.Find("Hidden/UI/SDF/URP/Blur");
            m_blurMaterial = new Material(m_blurShader);

            m_blurredTempID1 = Shader.PropertyToID("_BlurTemp1");
            m_blurredTempID2 = Shader.PropertyToID("_BlurTemp2");
            m_screenCopyID = Shader.PropertyToID("_ScreenCopyTexture");
            m_blurWeightsID = Shader.PropertyToID("_Weights");
            m_blurOffsetsID = Shader.PropertyToID("_Offsets");
            m_grabBlurTextureID = Shader.PropertyToID("_GrabTexture");
        }

#if !UNITY_6000_0_OR_NEWER || URP_COMPATIBILITY_MODE
        public void SetRenderTarget(RenderTargetIdentifier target) => m_currentTarget = target;
#endif

        public void UpdateWeights(float blur)
        {
            float total = 0;
            float d = blur * blur * 0.001f;

            for (int i = 0; i < m_blurWeights.Length; i++)
            {
                float r = 1.0f + 2.0f * i;
                float w = Mathf.Exp(-0.5f * (r * r) / d);
                m_blurWeights[i] = w;
                if (i > 0) w *= 2.0f;

                total += w;
            }

            for (int i = 0; i < m_blurWeights.Length; i++)
                m_blurWeights[i] /= total;
        }

        private static Mesh[] m_meshPool = new Mesh[0];
        public static Mesh MeshCopy(in Mesh src, ref Mesh dst)
        {
            dst.vertices = src.vertices;
            dst.uv = src.uv;
            dst.uv2 = src.uv2;
            dst.uv3 = src.uv3;
            dst.uv4 = src.uv4;
            dst.triangles = src.triangles;

            dst.bindposes = src.bindposes;
            dst.boneWeights = src.boneWeights;
            dst.bounds = src.bounds;
            dst.colors = src.colors;
            dst.colors32 = src.colors32;
            dst.normals = src.normals;
            dst.subMeshCount = src.subMeshCount;
            dst.tangents = src.tangents;

            return dst;
        }

#if !UNITY_6000_0_OR_NEWER || URP_COMPATIBILITY_MODE
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.isPreviewCamera)
                return;

            var targets = SDFUI.blurTargets;
            var targetsCount = targets.Count();
            if (targetsCount == 0)
                return;
            var meshTargetPool = SDFUI.blurTargetMeshPool;

            ref var cam = ref renderingData.cameraData.camera;
            if (renderingData.cameraData.isSceneViewCamera)
                return;

            var cmd = CommandBufferPool.Get(NAME);

            using (new ProfilingScope(cmd, new ProfilingSampler(NAME)))
            {
                var targetDescriptor = renderingData.cameraData.cameraTargetDescriptor;
                cmd.GetTemporaryRT(m_screenCopyID, targetDescriptor, FilterMode.Bilinear);
                cmd.GetTemporaryRT(m_blurredTempID1, targetDescriptor, FilterMode.Bilinear);
                cmd.GetTemporaryRT(m_blurredTempID2, targetDescriptor, FilterMode.Bilinear);

                int pass;
                for (int i = 0; i < targetsCount; i++)
                {
                    var target = targets.ElementAt(i);

                    if (!target.IsDestroyed() && target.transform is RectTransform rectTransform)
                    {
                        var screenPos = RectTransformUtility.WorldToScreenPoint(cam, rectTransform.position);
                        RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, screenPos, cam, out var worldPoint);
                        var matrix = Matrix4x4.TRS(worldPoint, rectTransform.rotation, rectTransform.lossyScale);

                        var mesh = target.canvasRenderer.GetMesh();
                        if (mesh == null)
                            continue;

                        // [Internal Unity bug?] Only the Transform's Rotation property is not applied to the Mesh obtained from the CanvasRenderer.

                        var meshOld = mesh;
                        mesh = meshTargetPool.ElementAt(i);
                        MeshCopy(in meshOld, ref mesh);

                        var material = target.materialForRendering;

                        material.SetFloat(SDFUI.PROP_LIQUID_GLASS_IS_POST_PROCESS_PASS, 1);

                        pass = target.material.FindPass("Shadow");
                        cmd.DrawMesh(mesh, matrix, material, 0, pass);

                        cmd.Blit(m_currentTarget, m_screenCopyID);

                        Vector2Int scaledPixelSize = new Vector2Int(cam.scaledPixelWidth, cam.scaledPixelHeight);
                        float x = target.liquidGlassBlurOffset / scaledPixelSize.x;
                        float y = target.liquidGlassBlurOffset / scaledPixelSize.y;

                        if (target.liquidGlassBlur > 0)
                        {
                            UpdateWeights(target.liquidGlassBlur);
                            cmd.SetGlobalFloatArray(m_blurWeightsID, m_blurWeights);

                            cmd.SetGlobalVector(m_blurOffsetsID, new Vector4(x, 0, 0, 0));
                            cmd.Blit(m_screenCopyID, m_blurredTempID1, m_blurMaterial);

                            cmd.SetGlobalVector(m_blurOffsetsID, new Vector4(0, y, 0, 0));
                            cmd.Blit(m_blurredTempID1, m_blurredTempID2, m_blurMaterial);
                        }
                        else
                            cmd.Blit(m_screenCopyID, m_blurredTempID2);

                        cmd.SetGlobalTexture(m_grabBlurTextureID, m_blurredTempID2);

                        pass = target.material.FindPass("ShapeOutline");
                        cmd.SetRenderTarget(m_currentTarget);
                        cmd.DrawMesh(mesh, matrix, material, 0, pass);
                    }
                }

                cmd.ReleaseTemporaryRT(m_screenCopyID);
                cmd.ReleaseTemporaryRT(m_blurredTempID1);
                cmd.ReleaseTemporaryRT(m_blurredTempID2);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
#endif

#if UNITY_6000_0_OR_NEWER && !URP_COMPATIBILITY_MODE
        private class PassData
        {
            public UniversalCameraData cameraData;
            public TextureHandle activeColorTexture;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            // https://docs.unity3d.com/6000.0/Documentation/Manual/urp/renderer-features/create-custom-renderer-feature.html
            // https://docs.unity3d.com/6000.2/Documentation/Manual/urp/render-graph-draw-objects-in-a-pass.html

            renderGraph.nativeRenderPassesEnabled = false;

            var resourceData = frameData.Get<UniversalResourceData>();
            var cameraData = frameData.Get<UniversalCameraData>();
            if (resourceData.isActiveTargetBackBuffer || cameraData.isPreviewCamera)
                return;

            using (var renderPassBuilder = renderGraph.AddRenderPass<PassData>(NAME, out var renderPassData))
            {
                renderPassData.cameraData = cameraData;
                renderPassData.activeColorTexture = resourceData.activeColorTexture;
                renderPassBuilder.SetRenderFunc((PassData data, RenderGraphContext context) =>
                    ExecutePass(data, context));
            }
        }

        private void ExecutePass(PassData passData, RenderGraphContext context)
        {
            using (new ProfilingScope(context.cmd, new ProfilingSampler(NAME)))
            {
                var targets = SDFUI.blurTargets;
                var targetsCount = targets.Count();
                if (targetsCount == 0)
                    return;
                var meshTargetPool = SDFUI.blurTargetMeshPool;

                ref var cam = ref passData.cameraData.camera;
                if (passData.cameraData.isSceneViewCamera)
                    return;

                var targetDescriptor = passData.cameraData.cameraTargetDescriptor;
                context.cmd.GetTemporaryRT(m_screenCopyID, targetDescriptor, FilterMode.Bilinear);
                context.cmd.GetTemporaryRT(m_blurredTempID1, targetDescriptor, FilterMode.Bilinear);
                context.cmd.GetTemporaryRT(m_blurredTempID2, targetDescriptor, FilterMode.Bilinear);

                int pass;
                for (int i = 0; i < targetsCount; i++)
                {
                    var target = targets.ElementAt(i);

                    if (!target.IsDestroyed() && target.transform is RectTransform rectTransform)
                    {
                        var screenPos = RectTransformUtility.WorldToScreenPoint(cam, rectTransform.position);
                        RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, screenPos, cam, out var worldPoint);
                        var matrix = Matrix4x4.TRS(worldPoint, rectTransform.rotation, rectTransform.lossyScale);

                        var mesh = target.canvasRenderer.GetMesh();
                        if (mesh == null)
                            continue;

                        // [Internal Unity bug?] Only the Transform's Rotation property is not applied to the Mesh obtained from the CanvasRenderer.

                        var meshOld = mesh;
                        mesh = meshTargetPool.ElementAt(i);
                        MeshCopy(in meshOld, ref mesh);

                        var material = target.materialForRendering;

                        material.SetFloat(SDFUI.PROP_LIQUID_GLASS_IS_POST_PROCESS_PASS, 1);

                        pass = target.material.FindPass("Shadow");
                        context.cmd.DrawMesh(mesh, matrix, material, 0, pass);

                        context.cmd.Blit(passData.activeColorTexture, m_screenCopyID);

                        Vector2Int scaledPixelSize = new Vector2Int(cam.scaledPixelWidth, cam.scaledPixelHeight);
                        float x = target.liquidGlassBlurOffset / scaledPixelSize.x;
                        float y = target.liquidGlassBlurOffset / scaledPixelSize.y;

                        if (target.liquidGlassBlur > 0)
                        {
                            UpdateWeights(target.liquidGlassBlur);
                            context.cmd.SetGlobalFloatArray(m_blurWeightsID, m_blurWeights);

                            context.cmd.SetGlobalVector(m_blurOffsetsID, new Vector4(x, 0, 0, 0));
                            context.cmd.Blit(m_screenCopyID, m_blurredTempID1, m_blurMaterial);

                            context.cmd.SetGlobalVector(m_blurOffsetsID, new Vector4(0, y, 0, 0));
                            context.cmd.Blit(m_blurredTempID1, m_blurredTempID2, m_blurMaterial);
                        }
                        else
                            context.cmd.Blit(m_screenCopyID, m_blurredTempID2);

                        context.cmd.SetGlobalTexture(m_grabBlurTextureID, m_blurredTempID2);

                        pass = target.material.FindPass("ShapeOutline");
                        context.cmd.SetRenderTarget(passData.activeColorTexture);
                        context.cmd.DrawMesh(mesh, matrix, material, 0, pass);
                    }
                }

                context.cmd.ReleaseTemporaryRT(m_screenCopyID);
                context.cmd.ReleaseTemporaryRT(m_blurredTempID1);
                context.cmd.ReleaseTemporaryRT(m_blurredTempID2);
            }
        }
#endif
    }
}
