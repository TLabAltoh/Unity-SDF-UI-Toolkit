using UnityEngine;
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;

namespace TLab.UI.SDF.Editor
{
    public struct BezierN
    {
        public int splineS;
        public int splineE;
        public bool isClosed;
        public float thickness;

        public Draw draw;
    }

    [BurstCompile]
    public struct SDFBezierJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<BezierN> beziers;
        [ReadOnly] public NativeArray<Vector2> splines;
        [ReadOnly] public Vector2Int size;
        [ReadOnly] public Vector2Int texSize;
        [ReadOnly] public float maxDist;

        public NativeArray<byte> result;

        private const float EPSILON = 1e-2f;

        [BurstCompile]
        private float dot2(in float2 v) => math.dot(v, v);

        [BurstCompile]
        private float cross(in float2 a, in float2 b) => a.x * b.y - a.y * b.x;

        [BurstCompile]
        private float udPoint(in float2 p, in float2 a) => math.length(p - a);

        [BurstCompile]
        private float udSegment(in float2 p, in float2 a, in float2 b)
        {
            var pa = p - a;
            var ba = b - a;
            var h = (float)math.clamp(math.dot(pa, ba) / math.dot(ba, ba), 0.0, 1.0);
            return math.length(pa - ba * h);
        }

        [BurstCompile]
        private float sdBezier(in float2 pos, in float2 A, in float2 B, in float2 C)
        {
            // https://www.shadertoy.com/view/dls3Wr

            var a = B - A;
            var b = A - 2.0f * B + C;
            var c = a * 2.0f;
            var d = A - pos;

            var kk = 1.0f / math.dot(b, b);
            var kx = kk * math.dot(a, b);
            var ky = kk * (2.0f * math.dot(a, a) + math.dot(d, b)) / 3.0f;
            var kz = kk * math.dot(d, a);

            float res, sgn;

            var p = ky - kx * kx;
            var q = kx * (2.0f * kx * kx - 3.0f * ky) + kz;
            var p3 = p * p * p;
            var q2 = q * q;
            var h = q2 + 4.0f * p3;

            if (h >= 0.0f)
            {   // 1 root
                h = math.sqrt(h);
                var x = (new float2(h, -h) - new float2(q, q)) / 2.0f;

                if (math.abs(math.abs(h / q) - 1.0f) < 0.0001f)
                {
                    var k = (1.0f - p3 / q2) * p3 / q;  // quadratic approx
                    x = new float2(k, -k - q);
                }

                var uv = new float2(math.sign(x.x) * math.pow(math.abs(x.x), 1.0f / 3.0f),
                    math.sign(x.y) * math.pow(math.abs(x.y), 1.0f / 3.0f));
                var t = math.clamp(uv.x + uv.y - kx, 0.0f, 1.0f);
                var z = d + (c + b * t) * t;
                res = dot2(z);
                sgn = cross(c + 2.0f * b * t, z);
            }
            else
            {   // 3 roots
                var z = math.sqrt(-p);
                var v = math.acos(q / (p * z * 2.0f)) / 3.0f;
                var m = math.cos(v);
                var n = math.sin(v) * 1.732050808f;
                var t = new float3(m + m, -n - m, n - m) * z - kx * new float3(1, 1, 1);
                t.x = math.clamp(t.x, 0.0f, 1.0f);
                t.y = math.clamp(t.y, 0.0f, 1.0f);
                t.z = math.clamp(t.z, 0.0f, 1.0f);
                var qx = d + (c + b * t.x) * t.x;
                var dx = dot2(qx);
                var sx = cross(c + 2.0f * b * t.x, qx);
                var qy = d + (c + b * t.y) * t.y;
                var dy = dot2(qy);
                var sy = cross(c + 2.0f * b * t.y, qy);
                if (dx < dy)
                {
                    res = dx;
                    sgn = sx;
                }
                else
                {
                    res = dy;
                    sgn = sy;
                }
            }
            return math.sqrt(res) * math.sign(sgn);
        }

        [BurstCompile]
        private float windingSign(in float2 p, in float2 a, in float2 b)
        {
            // https://www.shadertoy.com/view/wdBXRW

            float2 e = b - a, w = p - a;

            var cond0 = p.y >= a.y;
            var cond1 = p.y < b.y;
            var cond2 = e.x * w.y > e.y * w.x;

            if ((cond0 && cond1 && cond2) || (!cond0 && !cond1 && !cond2))
                return -1.0f;
            else
                return 1.0f;
        }

        [BurstCompile]
        private void swapMinSign(in float2 pos, in float2 A, in float2 B, in float2 C, ref float min, ref float sign)
        {
            var abEqual = A == B;
            var bcEqual = B == C;
            var acEqual = A == C;

            if (math.all(abEqual) && math.all(bcEqual))
                return;
            else if (math.all(abEqual) || math.all(acEqual))
            {
                var ud = udSegment(pos, B, C);
                min = math.min(min, ud);
                sign *= windingSign(pos, B, C);
                return;
            }
            else if (math.all(bcEqual) || (math.abs(cross(A - B, A - C)) <= EPSILON))
            // It is preferable to use large value as
            // epsilon because this culculates cross
            // product as a float value, a parallel
            // vector may return second decimal value
            // even if it is parallel.
            {
                var ud = udSegment(pos, A, C);
                min = math.min(min, ud);
                sign *= windingSign(pos, A, C);
                return;
            }

            var sd = sdBezier(pos, A, B, C);
            min = math.min(min, math.abs(sd));
            if ((sd > 0.0f) == (cross(B - C, B - A) < 0.0f))
            {
                sign *= windingSign(pos, A, B);
                sign *= windingSign(pos, B, C);
            }
            else
                sign *= windingSign(pos, A, C);
        }

        [BurstCompile]
        private void stroke(in float2 pos, in float2 A, in float2 B, in float2 C, in float thickness, ref float min)
        {
            var abEqual = A == B;
            var bcEqual = B == C;
            var acEqual = A == C;

            if (math.all(abEqual) && math.all(bcEqual))
            {
                var ud = udPoint(pos, A);
                min = math.min(min, ud - thickness);
                return;
            }
            else if (math.all(abEqual) || math.all(acEqual))
            {
                var ud = udSegment(pos, B, C);
                min = math.min(min, ud - thickness);
                return;
            }
            else if (math.all(bcEqual) || (math.abs(cross(A - B, A - C)) <= EPSILON))
            {
                var ud = udSegment(pos, A, C);
                min = math.min(min, ud - thickness);
                return;
            }

            var sd = sdBezier(pos, A, B, C);
            min = math.min(min, math.abs(sd) - thickness);
        }

        [BurstCompile]
        private byte cutout(float min, float maxDist) => (byte)(255f * (1f - math.clamp(min / (2 * maxDist) + 0.5f, 0, 1)));

        [BurstCompile]
        public void Execute(int index)
        {
            var idxY = texSize.y - index / texSize.x - 1;
            var idxX = index % texSize.x;

            var texX = idxX * (float)size.x / texSize.x;
            var texY = idxY * (float)size.y / texSize.y;

            var texP = new float2(texX, texY);

            var fMin = float.MaxValue;
            var wMin = float.MaxValue;
            var sMin = float.MaxValue;
            var fSign = 1.0f;
            var wSign = 1.0f;

            for (int i = 0; i < beziers.Length; i++)
            {
                var bezier = beziers[i];
                var splineS = bezier.splineS;
                var splineE = bezier.splineE;

                switch (bezier.draw)
                {
                    case Draw.FILL:
                        var tMin = float.MaxValue;
                        var tSign = 1.0f;
                        for (int j = splineS; j < splineE - 2; j += 2)
                            swapMinSign(texP, splines[j], splines[j + 1], splines[j + 2], ref tMin, ref tSign);
                        if (tMin * tSign < fMin * fSign)
                        {
                            fMin = tMin;
                            fSign = tSign;
                        }
                        break;
                    case Draw.WINDING:
                        for (int j = splineS; j < splineE - 2; j += 2)
                            swapMinSign(texP, splines[j], splines[j + 1], splines[j + 2], ref wMin, ref wSign);
                        break;
                    case Draw.STROKE:
                        for (int j = splineS; j < splineE - 2; j += 2)
                            stroke(texP, splines[j], splines[j + 1], splines[j + 2], bezier.thickness, ref sMin);
                        break;
                }
            }

            var min = math.min(fMin * fSign, math.min(wMin * wSign, sMin));
            var dist = cutout(min, maxDist);

            result[index] = result[index] < dist ? dist : result[index];
        }
    }
}
