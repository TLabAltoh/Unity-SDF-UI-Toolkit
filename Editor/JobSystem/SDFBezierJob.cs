using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using System;

namespace TLab.UI.SDF.Editor
{
    public struct BezierN
    {
        public int splineS;
        public int splineE;
        public bool closed;
        public float thickness;

        public Draw draw;
        public Clockwise clockwise;
    }

    public struct SDFBezierJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<BezierN> BEZIERS;
        [ReadOnly] public NativeArray<Vector2> SPLINES;
        [ReadOnly] public int TEX_WIDTH;
        [ReadOnly] public int SDF_WIDTH;
        [ReadOnly] public int TEX_HEIGHT;
        [ReadOnly] public int SDF_HEIGHT;
        [ReadOnly] public float MAX_DIST;

        public NativeArray<byte> result;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public float Dot2(in Vector2 v)
        {
            return Vector2.Dot(v, v);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public float Cross(in Vector2 a, in Vector2 b)
        {
            return a.x * b.y - a.y * b.x;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <returns></returns>
        public float SdBezier(in Vector2 pos, in Vector2 A, in Vector2 B, in Vector2 C)
        {
            var a = B - A;
            var b = A - 2.0f * B + C;
            var c = a * 2.0f;
            var d = A - pos;

            var kk = 1.0f / Vector2.Dot(b, b);
            var kx = kk * Vector2.Dot(a, b);
            var ky = kk * (2.0f * Vector2.Dot(a, a) + Vector2.Dot(d, b)) / 3.0f;
            var kz = kk * Vector2.Dot(d, a);

            var res = 0.0f;
            var sgn = 0.0f;

            var p = ky - kx * kx;
            var q = kx * (2.0f * kx * kx - 3.0f * ky) + kz;
            var p3 = p * p * p;
            var q2 = q * q;
            var h = q2 + 4.0f * p3;

            if (h >= 0.0f)
            {   // 1 root
                h = Mathf.Sqrt(h);
                var x = (new Vector2(h, -h) - new Vector2(q, q)) / 2.0f;

                if (Mathf.Abs(Mathf.Abs(h / q) - 1.0f) < 0.0001f)
                {
                    var k = (1.0f - p3 / q2) * p3 / q;  // quadratic approx
                    x = new Vector2(k, -k - q);
                }

                var uv = new Vector2(Mathf.Sign(x.x) * Mathf.Pow(Mathf.Abs(x.x), 1.0f / 3.0f),
                    Mathf.Sign(x.y) * Mathf.Pow(Mathf.Abs(x.y), 1.0f / 3.0f));
                var t = Mathf.Clamp(uv.x + uv.y - kx, 0.0f, 1.0f);
                var z = d + (c + b * t) * t;
                res = Dot2(z);
                sgn = Cross(c + 2.0f * b * t, z);
            }
            else
            {   // 3 roots
                var z = Mathf.Sqrt(-p);
                var v = Mathf.Acos(q / (p * z * 2.0f)) / 3.0f;
                var m = Mathf.Cos(v);
                var n = Mathf.Sin(v) * 1.732050808f;
                var t = new Vector3(m + m, -n - m, n - m) * z - kx * Vector3.one;
                t.x = Mathf.Clamp(t.x, 0.0f, 1.0f);
                t.y = Mathf.Clamp(t.y, 0.0f, 1.0f);
                t.z = Mathf.Clamp(t.z, 0.0f, 1.0f);
                var qx = d + (c + b * t.x) * t.x;
                var dx = Dot2(qx);
                var sx = Cross(c + 2.0f * b * t.x, qx);
                var qy = d + (c + b * t.y) * t.y;
                var dy = Dot2(qy);
                var sy = Cross(c + 2.0f * b * t.y, qy);
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

            return Mathf.Sqrt(res) * Mathf.Sign(sgn);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public float Swap(float a, float b)
        {
            return a < b ? a : b;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public float AbsSwap(float a, float b)
        {
            return Mathf.Abs(a) < Mathf.Abs(b) ? a : b;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="min"></param>
        /// <param name="maxDist"></param>
        /// <returns></returns>
        public byte Fill(float min, float maxDist)
        {
            var norm = min / (2 * maxDist) + 0.5f;

            return (byte)(255f * (1f - Mathf.Clamp01(norm)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void Execute(int index)
        {
            var idxY = SDF_HEIGHT - index / SDF_WIDTH - 1;
            var idxX = index % SDF_WIDTH;

            var texX = idxX * (float)TEX_WIDTH / SDF_WIDTH;
            var texY = idxY * (float)TEX_HEIGHT / SDF_HEIGHT;

            var texP = new Vector2(texX, texY);

            var min = float.MaxValue;

            for (int i = 0; i < BEZIERS.Length - 1; i++)
            {
                var bezier = BEZIERS[i];
                var splineS = bezier.splineS;
                var splineE = bezier.splineE;

                switch (bezier.draw)
                {
                    case Draw.FILL:
                        for (int j = splineS; j < splineE - 2; j += 2)
                        {
                            var tmp = SdBezier(in texP, SPLINES[j], SPLINES[j + 1], SPLINES[j + 2]);

                            min = AbsSwap(min, tmp);
                        }

                        if (bezier.closed)
                        {
                            var tmp = SdBezier(in texP, SPLINES[splineE - 2], SPLINES[splineE - 1], SPLINES[splineS]);

                            min = AbsSwap(min, tmp);
                        }

                        break;
                    case Draw.STROKE:
                        for (int j = splineS; j < splineE - 2; j += 2)
                        {
                            var tmp = SdBezier(in texP, SPLINES[j], SPLINES[j + 1], SPLINES[j + 2]);

                            tmp = Mathf.Abs(tmp) - bezier.thickness;

                            min = Swap(min, tmp);
                        }

                        if (bezier.closed)
                        {
                            var tmp = SdBezier(in texP, SPLINES[splineE - 2], SPLINES[splineE - 1], SPLINES[splineS]);

                            tmp = Mathf.Abs(tmp) - bezier.thickness;

                            min = Swap(min, tmp);
                        }

                        break;
                }
            }

            var dist = Fill(min, MAX_DIST);

            result[index] = result[index] < dist ? dist : result[index];
        }
    }
}
