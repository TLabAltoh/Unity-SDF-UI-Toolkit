/**
 * https://catlikecoding.com/unity/tutorials/custom-srp/fxaa/
 */

using Unity.Jobs;
using UnityEngine;
using Unity.Collections;

namespace Nobi.UiRoundedCorners.RasterizeJob
{
#if UNITY_EDITOR
    public struct RasterizePolygonJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<Vector2> POLYGON;
        [ReadOnly] public float X_RATIO;
        [ReadOnly] public float Y_RATIO;
        [ReadOnly] public int SDF_WIDTH;
        [ReadOnly] public int SDF_HEIGHT;

        public NativeArray<byte> result;

        public void Execute(int index)
        {
            int idxY = SDF_HEIGHT - index / SDF_WIDTH - 1;
            int idxX = index % SDF_WIDTH;

            float texX = idxX * X_RATIO;
            float texY = idxY * Y_RATIO;

            float sum = 0;

            for (int i = 0; i < POLYGON.Length; i++)
            {
                Vector2 point0 = POLYGON[(i + 0) % POLYGON.Length];
                Vector2 point1 = POLYGON[(i + 1) % POLYGON.Length];
                float dx0 = point0.x - texX;
                float dy0 = point0.y - texY;
                float dx1 = point1.x - texX;
                float dy1 = point1.y - texY;

                float dot = (dx0 * dx1 + dy0 * dy1) / (Mathf.Sqrt(dx0 * dx0 + dy0 * dy0) * Mathf.Sqrt(dx1 * dx1 + dy1 * dy1));
                dot = Mathf.Clamp(dot, -1.0f, 1.0f);
                float theta = Mathf.Acos(dot);
                theta *= Mathf.Sign(dx0 * dy1 - dy0 * dx1);
                sum += theta;
            }

            if (Mathf.Abs(sum) > Mathf.PI)
            {
                result[index] = 255;
            }
        }
    }

    public struct RasterizeCircleJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<Circle> CIRCLES;
        [ReadOnly] public float X_RATIO;
        [ReadOnly] public float Y_RATIO;
        [ReadOnly] public int SDF_WIDTH;
        [ReadOnly] public int SDF_HEIGHT;

        public NativeArray<byte> result;

        public void Execute(int index)
        {
            int idxY = SDF_HEIGHT - index / SDF_WIDTH - 1;
            int idxX = index % SDF_WIDTH;

            float texX = idxX * X_RATIO;
            float texY = idxY * Y_RATIO;

            for (int i = 0; i < CIRCLES.Length; i++)
            {
                Circle circle = CIRCLES[i];
                float dx = circle.center.x - texX;
                float dy = circle.center.y - texY;

                if (dx * dx + dy * dy < circle.radius * circle.radius)
                {
                    result[index] = 255;
                }
            }
        }
    }

    /// <summary>
    /// Vector4 FXAA_CONFIG = new Vector4(0.0833f, 0.063f, 0.75f, 0.0f);
    /// float LAST_EDGE_STEP_GUESS = 8.0f;
    /// float[] EDGE_STEP_SIZES = { 1.0f, 1.0f, 1.0f, 1.0f, 1.5f, 2.0f, 2.0f, 2.0f, 2.0f, 4.0f };
    /// </summary>
    public struct FxaaEffect : IJobParallelFor
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
            float sx = uv.x * (WIDTH - 1);
            float sy = uv.y * (HEIGHT - 1);
            int x = (int)sx;
            int y = (int)sy;

            float rx = sx - x;
            float ry = sy - y;

            x = x > (WIDTH - 1) ? (WIDTH - 1) : x;
            y = y > (HEIGHT - 1) ? (HEIGHT - 1) : y;

            x = x < 0 ? 0 : x;
            y = y < 0 ? 0 : y;

            int offsetX = x == WIDTH - 1 ? 0 : 1;
            int offsetY = y == HEIGHT - 1 ? 0 : 1;

            byte nw = SOURCE[y * WIDTH + x];
            byte ne = SOURCE[y * WIDTH + (x + offsetX)];
            byte sw = SOURCE[(y + offsetY) * WIDTH + x];
            byte se = SOURCE[(y + offsetY) * WIDTH + (x + offsetX)];

            byte rn = (byte)((1.0f - rx) * nw + rx * ne);
            byte rs = (byte)((1.0f - rx) * sw + rx * se);

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
            float horizontal = 2.0f * Mathf.Abs(luma.n + luma.s - 2.0f * luma.m) +
                Mathf.Abs(luma.ne + luma.se - 2.0f * luma.e) +
                Mathf.Abs(luma.nw + luma.sw - 2.0f * luma.w);
            float vertical = 2.0f * Mathf.Abs(luma.e + luma.w - 2.0f * luma.m) +
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
            float gradientP = Mathf.Abs(lumaP - luma.m);
            float gradientN = Mathf.Abs(lumaN - luma.m);

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
            float filter = 2.0f * (luma.n + luma.e + luma.s + luma.w);
            filter += luma.ne + luma.nw + luma.se + luma.sw;
            filter *= 1.0f / 12.0f;
            filter = Mathf.Abs(filter - luma.m);
            filter = Mathf.Clamp01(filter / luma.range);
            filter = Mathf.SmoothStep(0, 1, filter);
            return filter * filter * FXAA_CONFIG.z;
        }

        private float GetEdgeBlendFactor(LumaNeighborhood luma, FXAAEdge edge, Vector2 uv)
        {
            Vector2 edgeUV = uv;
            Vector2 uvStep = Vector2.zero;
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

            float edgeLuma = 0.5f * (luma.m + edge.otherLuma);
            float gradientThreshold = 0.25f * edge.lumaGradient;

            Vector2 uvP = edgeUV + uvStep;
            float lumaDeltaP = GetLuma(uvP) - edgeLuma;
            bool atEndP = Mathf.Abs(lumaDeltaP) >= gradientThreshold;

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

            Vector2 uvN = edgeUV - uvStep;
            float lumaDeltaN = GetLuma(uvN) - edgeLuma;
            bool atEndN = Mathf.Abs(lumaDeltaN) >= gradientThreshold;

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
            int x = index % WIDTH;
            int y = index / WIDTH;
            Vector2 uv = new Vector2(x / (float)(WIDTH - 1), y / (float)(HEIGHT - 1));

            LumaNeighborhood luma = GetLumaNeighborhood(uv);

            if (CanSkipFXAA(luma))
            {
                result[index] = GetSource(uv);
            }

            FXAAEdge edge = GetFXAAEdge(luma);
            float blendFactor = Mathf.Max(GetSubpixelBlendFactor(luma), GetEdgeBlendFactor(luma, edge, uv));
            //float blendFactor = GetEdgeBlendFactor(luma, edge, uv);
            //float blendFactor = GetSubpixelBlendFactor(luma);
            Vector2 blendUV = uv;
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

    public struct CopyR8ToARGB32 : IJobParallelFor
    {
        [ReadOnly] public NativeArray<byte> SOURCE;
        [ReadOnly] public int CHANNEL_SIZE;

        public NativeArray<byte> result;

        public void Execute(int index)
        {
            result[index] = SOURCE[index / CHANNEL_SIZE];
        }
    }
#endif
}