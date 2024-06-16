using System;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;

namespace TLab.UI.SDF.Editor
{
    public struct BezierN
    {
        public int splineS;
        public int splineE;
        public bool closed;
        public float thickness;

        public Draw draw;
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
        /// https://www.shadertoy.com/view/dls3Wr
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
        /// Source: https://www.shadertoy.com/view/wdBXRW
        /// </summary>
        /// <param name="p"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public float WindingSign(in Vector2 p, in Vector2 a, in Vector2 b)
        {
            Vector2 e = b - a;
            Vector2 w = p - a;

            var cond0 = p.y >= a.y;
            var cond1 = p.y < b.y;
            var cond2 = e.x * w.y > e.y * w.x;

            if ((cond0 && cond1 && cond2) || (!cond0 && !cond1 && !cond2))
            {
                return -1.0f;
            }
            else
            {
                return 1.0f;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="min"></param>
        /// <param name="maxDist"></param>
        /// <returns></returns>
        public byte Fill(float min, float maxDist)
        {
            var norm = Mathf.Clamp01(min / (2 * maxDist) + 0.5f);

            return (byte)(255f * (1f - norm));
            //return (byte)(255f * min / (2 * maxDist));
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

            var fMin = float.MaxValue;
            var wMin = float.MaxValue;
            var sMin = float.MaxValue;
            var fSign = 1.0f;
            var wSign = 1.0f;

            for (int i = 0; i < BEZIERS.Length; i++)
            {
                var bezier = BEZIERS[i];
                var splineS = bezier.splineS;
                var splineE = bezier.splineE;

                switch (bezier.draw)
                {
                    case Draw.FILL:
                        var tMin = float.MaxValue;
                        var tSign = 1.0f;
                        for (int j = splineS; j < splineE - 2; j += 2)
                        {
                            var v0 = SPLINES[j];
                            var v1 = SPLINES[j + 1];
                            var v2 = SPLINES[j + 2];

                            var sd = SdBezier(in texP, v0, v1, v2);

                            tMin = Mathf.Min(tMin, Mathf.Abs(sd));

                            if ((sd > 0.0f) == (Cross(v1 - v2, v1 - v0) < 0.0f))
                            {
                                tSign *= WindingSign(texP, v0, v1);
                                tSign *= WindingSign(texP, v1, v2);
                            }
                            else
                            {
                                tSign *= WindingSign(texP, v0, v2);
                            }
                        }

                        if (tMin * tSign < fMin * fSign)
                        {
                            fMin = tMin;
                            fSign = tSign;
                        }
                        break;
                    case Draw.WINDING:
                        for (int j = splineS; j < splineE - 2; j += 2)
                        {
                            var v0 = SPLINES[j];
                            var v1 = SPLINES[j + 1];
                            var v2 = SPLINES[j + 2];

                            var sd = SdBezier(in texP, v0, v1, v2);

                            wMin = Mathf.Min(wMin, Mathf.Abs(sd));

                            if ((sd > 0.0f) == (Cross(v1 - v2, v1 - v0) < 0.0f))
                            {
                                wSign *= WindingSign(texP, v0, v1);
                                wSign *= WindingSign(texP, v1, v2);
                            }
                            else
                            {
                                wSign *= WindingSign(texP, v0, v2);
                            }
                        }

                        break;
                    case Draw.STROKE:
                        for (int j = splineS; j < splineE - 2; j += 2)
                        {
                            var sd = SdBezier(in texP, SPLINES[j], SPLINES[j + 1], SPLINES[j + 2]);

                            sMin = Mathf.Min(sMin, Mathf.Abs(sd) - bezier.thickness);
                        }

                        break;
                }
            }

            var min = Mathf.Min(fMin * fSign, wMin * wSign, sMin);
            var dist = Fill(min, MAX_DIST);

            result[index] = result[index] < dist ? dist : result[index];
        }
    }
}
