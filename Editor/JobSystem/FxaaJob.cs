using UnityEngine;
using Unity.Jobs;
using Unity.Collections;

namespace TLab.UI.SDF.Editor
{
    /// <summary>
    /// https://catlikecoding.com/unity/tutorials/custom-srp/fxaa/
    /// </summary>
    public struct FxaaJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<byte> SOURCE;
        [ReadOnly] public NativeArray<float> EDGE_STEP_SIZES;
        [ReadOnly] public Vector4 FXAA_CONFIG;
        [ReadOnly] public int WIDTH;
        [ReadOnly] public int HEIGHT;
        [ReadOnly] public float LAST_EDGE_STEP_GUESS;

        public NativeArray<byte> result;

        private Vector2 GetSourceTexelSize()
        {
            return new Vector2(1.0f / WIDTH, 1.0f / HEIGHT);
        }

        private byte GetSource(Vector2 uv)
        {
            var sx = uv.x * (WIDTH - 1);
            var sy = uv.y * (HEIGHT - 1);
            var x = (int)sx;
            var y = (int)sy;

            var rx = sx - x;
            var ry = sy - y;

            x = x > (WIDTH - 1) ? (WIDTH - 1) : x;
            y = y > (HEIGHT - 1) ? (HEIGHT - 1) : y;

            x = x < 0 ? 0 : x;
            y = y < 0 ? 0 : y;

            int offsetX = x == WIDTH - 1 ? 0 : 1;
            int offsetY = y == HEIGHT - 1 ? 0 : 1;

            var nw = SOURCE[y * WIDTH + x];
            var ne = SOURCE[y * WIDTH + (x + offsetX)];
            var sw = SOURCE[(y + offsetY) * WIDTH + x];
            var se = SOURCE[(y + offsetY) * WIDTH + (x + offsetX)];

            var rn = (byte)((1.0f - rx) * nw + rx * ne);
            var rs = (byte)((1.0f - rx) * sw + rx * se);

            return (byte)((1.0f - ry) * rn + ry * rs);
        }

        private float GetLuma(Vector2 uv, float uOffset = 0.0f, float vOffset = 0.0f)
        {
            uv += new Vector2(uOffset, vOffset) * GetSourceTexelSize();

            return GetSource(uv) / 255.0f;
        }

        private struct LumaNeighborhood
        {
            public float m, n, e, s, w, ne, se, sw, nw;
            public float highest, lowest, range;
        };

        private LumaNeighborhood GetLumaNeighborhood(Vector2 uv)
        {
            LumaNeighborhood luma;
            luma.m = GetLuma(uv);
            luma.n = GetLuma(uv, 0.0f, 1.0f);
            luma.e = GetLuma(uv, 1.0f, 0.0f);
            luma.s = GetLuma(uv, 0.0f, -1.0f);
            luma.w = GetLuma(uv, -1.0f, 0.0f);
            luma.ne = GetLuma(uv, 1.0f, 1.0f);
            luma.se = GetLuma(uv, 1.0f, -1.0f);
            luma.sw = GetLuma(uv, -1.0f, -1.0f);
            luma.nw = GetLuma(uv, -1.0f, 1.0f);

            luma.highest = Mathf.Max(Mathf.Max(Mathf.Max(Mathf.Max(luma.m, luma.n), luma.e), luma.s), luma.w);
            luma.lowest = Mathf.Min(Mathf.Min(Mathf.Min(Mathf.Min(luma.m, luma.n), luma.e), luma.s), luma.w);
            luma.range = luma.highest - luma.lowest;

            return luma;
        }

        private bool IsHorizontalEdge(LumaNeighborhood luma)
        {
            var horizontal = 2.0f * Mathf.Abs(luma.n + luma.s - 2.0f * luma.m) +
                Mathf.Abs(luma.ne + luma.se - 2.0f * luma.e) +
                Mathf.Abs(luma.nw + luma.sw - 2.0f * luma.w);
            var vertical = 2.0f * Mathf.Abs(luma.e + luma.w - 2.0f * luma.m) +
                Mathf.Abs(luma.ne + luma.nw - 2.0f * luma.n) +
                Mathf.Abs(luma.se + luma.sw - 2.0f * luma.s);
            return horizontal >= vertical;
        }

        private struct FXAAEdge
        {
            public bool isHorizontal;
            public float pixelStep;
            public float lumaGradient, otherLuma;
        };

        private FXAAEdge GetFXAAEdge(LumaNeighborhood luma)
        {
            FXAAEdge edge;
            edge.isHorizontal = IsHorizontalEdge(luma);
            float lumaP, lumaN;
            if (edge.isHorizontal)
            {
                edge.pixelStep = GetSourceTexelSize().y;
                lumaP = luma.n;
                lumaN = luma.s;
            }
            else
            {
                edge.pixelStep = GetSourceTexelSize().x;
                lumaP = luma.e;
                lumaN = luma.w;
            }
            var gradientP = Mathf.Abs(lumaP - luma.m);
            var gradientN = Mathf.Abs(lumaN - luma.m);

            if (gradientP < gradientN)
            {
                edge.pixelStep = -edge.pixelStep;
                edge.lumaGradient = gradientN;
                edge.otherLuma = lumaN;
            }
            else
            {
                edge.lumaGradient = gradientP;
                edge.otherLuma = lumaP;
            }

            return edge;
        }

        private bool CanSkipFXAA(LumaNeighborhood luma)
        {
            return luma.range < Mathf.Max(FXAA_CONFIG.x, FXAA_CONFIG.y * luma.highest);
        }

        private float GetSubpixelBlendFactor(LumaNeighborhood luma)
        {
            var filter = 2.0f * (luma.n + luma.e + luma.s + luma.w);
            filter += luma.ne + luma.nw + luma.se + luma.sw;
            filter *= 1.0f / 12.0f;
            filter = Mathf.Abs(filter - luma.m);
            filter = Mathf.Clamp01(filter / luma.range);
            filter = Mathf.SmoothStep(0, 1, filter);
            return filter * filter * FXAA_CONFIG.z;
        }

        private float GetEdgeBlendFactor(LumaNeighborhood luma, FXAAEdge edge, Vector2 uv)
        {
            var edgeUV = uv;
            var uvStep = Vector2.zero;
            if (edge.isHorizontal)
            {
                edgeUV.y += 0.5f * edge.pixelStep;
                uvStep.x = GetSourceTexelSize().x;
            }
            else
            {
                edgeUV.x += 0.5f * edge.pixelStep;
                uvStep.y = GetSourceTexelSize().y;
            }

            var edgeLuma = 0.5f * (luma.m + edge.otherLuma);
            var gradientThreshold = 0.25f * edge.lumaGradient;

            var uvP = edgeUV + uvStep;
            var lumaDeltaP = GetLuma(uvP) - edgeLuma;
            var atEndP = Mathf.Abs(lumaDeltaP) >= gradientThreshold;

            int i;
            for (i = 0; i < EDGE_STEP_SIZES.Length && !atEndP; i++)
            {
                uvP += uvStep * EDGE_STEP_SIZES[i];
                lumaDeltaP = GetLuma(uvP) - edgeLuma;
                atEndP = Mathf.Abs(lumaDeltaP) >= gradientThreshold;
            }
            if (!atEndP)
            {
                uvP += uvStep * LAST_EDGE_STEP_GUESS;
            }

            var uvN = edgeUV - uvStep;
            var lumaDeltaN = GetLuma(uvN) - edgeLuma;
            var atEndN = Mathf.Abs(lumaDeltaN) >= gradientThreshold;

            for (i = 0; i < EDGE_STEP_SIZES.Length && !atEndN; i++)
            {
                uvN -= uvStep * EDGE_STEP_SIZES[i];
                lumaDeltaN = GetLuma(uvN) - edgeLuma;
                atEndN = Mathf.Abs(lumaDeltaN) >= gradientThreshold;
            }
            if (!atEndN)
            {
                uvN -= uvStep * LAST_EDGE_STEP_GUESS;
            }

            float distanceToEndP, distanceToEndN;
            if (edge.isHorizontal)
            {
                distanceToEndP = uvP.x - uv.x;
                distanceToEndN = uv.x - uvN.x;
            }
            else
            {
                distanceToEndP = uvP.y - uv.y;
                distanceToEndN = uv.y - uvN.y;
            }

            float distanceToNearestEnd;
            bool deltaSign;
            if (distanceToEndP <= distanceToEndN)
            {
                distanceToNearestEnd = distanceToEndP;
                deltaSign = lumaDeltaP >= 0;
            }
            else
            {
                distanceToNearestEnd = distanceToEndN;
                deltaSign = lumaDeltaN >= 0;
            }

            if (deltaSign == (luma.m - edgeLuma >= 0))
            {
                return 0.0f;
            }
            else
            {
                return 0.5f - distanceToNearestEnd / (distanceToEndP + distanceToEndN);
            }
        }

        public void Execute(int index)
        {
            var x = index % WIDTH;
            var y = index / WIDTH;
            var uv = new Vector2(x / (float)(WIDTH - 1), y / (float)(HEIGHT - 1));

            var luma = GetLumaNeighborhood(uv);

            if (CanSkipFXAA(luma))
            {
                result[index] = GetSource(uv);
            }

            var edge = GetFXAAEdge(luma);
            var blendFactor = Mathf.Max(GetSubpixelBlendFactor(luma), GetEdgeBlendFactor(luma, edge, uv));
            var blendUV = uv;
            if (edge.isHorizontal)
            {
                blendUV.y += blendFactor * edge.pixelStep;
            }
            else
            {
                blendUV.x += blendFactor * edge.pixelStep;
            }

            result[index] = GetSource(blendUV);
        }
    }
}
