using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace TLab.UI.SDF
{
    public class LiquidGlassRenderPass : ScriptableRenderPass
    {
        private const string NAME = nameof(LiquidGlassRenderPass);

        private RenderTargetIdentifier m_currentTarget = default;

        private Material m_blurMaterial;
        private Shader m_blurShader;
        private float[] m_blurWeights = new float[10];

        private int m_blurredTempID1 = 0;
        private int m_blurredTempID2 = 0;
        private int m_screenCopyID = 0;
        private int m_blurWeightsID = 0;
        private int m_blurOffsetsID = 0;
        private int m_grabBlurTextureID = 0;

        public LiquidGlassRenderPass()
        {
            renderPassEvent = RenderPassEvent.AfterRendering;

            m_blurShader = Shader.Find("Hidden/UI/SDF/URP/Blur");
            m_blurMaterial = new Material(m_blurShader);

            m_blurredTempID1 = Shader.PropertyToID("_BlurTemp1");
            m_blurredTempID2 = Shader.PropertyToID("_BlurTemp2");
            m_screenCopyID = Shader.PropertyToID("_ScreenCopyTexture");
            m_blurWeightsID = Shader.PropertyToID("_Weights");
            m_blurOffsetsID = Shader.PropertyToID("_Offsets");
            m_grabBlurTextureID = Shader.PropertyToID("_GrabTexture");
        }

        public void SetRenderTarget(RenderTargetIdentifier target) => m_currentTarget = target;

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
                        var matrix = Matrix4x4.TRS(worldPoint, Quaternion.identity, rectTransform.lossyScale);

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
    }
}
