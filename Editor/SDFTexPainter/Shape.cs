using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

namespace TLab.UI.SDF.Editor
{
    public enum Draw
    {
        STROKE,
        FILL,
        WINDING
    };

    [Serializable]
    public class Circle
    {
        public Vector2 position;
        public float radius;
        public float thickness;

        public Draw draw;
    }

    [Serializable]
    public class Bezier
    {
        [Serializable]
        public class Handle
        {
            public Vector2 anchor;
            public Vector2 controlA;
            public Vector2 controlB;
        }

        public enum Control
        {
            A,
            B,
            NONE
        };

        public List<Handle> handles;
        public bool closed;
        public float thickness;

        public Draw draw;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public bool GetPoints(out Vector2[] points)
        {
            points = new Vector2[3 * handles.Count];

            if (handles.Count < 2)
            {
                return false;
            }

            for (int i = 0, j = 0; i < points.Length / 3; i++, j += 3)
            {
                points[j + 0] = handles[i].anchor + handles[i].controlB;
                points[j + 1] = handles[i].anchor;
                points[j + 2] = handles[i].anchor + handles[i].controlA;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="n"></param>
        /// <param name="closed"></param>
        /// <param name="points"></param>
        /// <returns></returns>
        public bool GetPoints(int n, bool closed, out Vector2[] points)
        {
            var cache = new List<Handle>();

            if (handles.Count < 2)
            {
                points = new Vector2[0];

                return false;
            }

            for (int i = 0; i < handles.Count - 1; i++)
            {
                var splits = Split(handles[i + 0], handles[i + 1], n);

                if (i == 0)
                {
                    cache.AddRange(splits);
                }
                else
                {
                    cache[cache.Count - 1].controlA = splits[0].controlA;

                    cache.AddRange(splits.Skip(1));
                }
            }

            if (closed)
            {
                var splits = Split(handles[handles.Count - 1], handles[0], n);

                cache[cache.Count - 1].controlA = splits[0].controlA;
                cache[0].controlB = splits[splits.Length - 1].controlB;

                cache.AddRange(splits.Skip(1));
            }

            points = new Vector2[3 * cache.Count];

            for (int i = 0, j = 0; i < points.Length / 3; i++, j += 3)
            {
                points[j + 0] = cache[i].anchor + cache[i].controlB;
                points[j + 1] = cache[i].anchor;
                points[j + 2] = cache[i].anchor + cache[i].controlA;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="h0"></param>
        /// <param name="h1"></param>
        /// <param name="c0"></param>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <param name="c3"></param>
        public static void GetCubic(Handle h0, Handle h1,
            out Vector2 c0, out Vector2 c1, out Vector2 c2, out Vector2 c3)
        {
            c0 = h0.anchor;
            c1 = c0 + h0.controlA;
            c3 = h1.anchor;
            c2 = c3 + h1.controlB;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        private static void GetCubicParams(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3,
            out Vector2 a, out Vector2 b, out Vector2 c, out Vector2 d)
        {
            c = (p1 - p0) * 3.0f;
            b = (p2 - p1) * 3.0f - c;
            d = p0;
            a = p3 - d - c - b;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        private static void GetCubicPoints(Vector2 a, Vector2 b, Vector2 c, Vector2 d,
            out Vector2 p0, out Vector2 p1, out Vector2 p2, out Vector2 p3)
        {
            p0 = d;
            p1 = (c / 3.0f) + d;
            p2 = (b + c) / 3.0f + p1;
            p3 = a + d + c + b;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="h0"></param>
        /// <param name="h1"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static Handle[] Split(Handle h0, Handle h1, int n)
        {
            var handles = new Handle[n + 2];

            for (int i = 0; i < handles.Length; i++)
            {
                handles[i] = new Handle();
            }

            handles[0].controlB = h0.controlB;
            handles[handles.Length - 1].controlA = h1.controlA;

            GetCubic(h0, h1, out var c0, out var c1, out var c2, out var c3);

            GetCubicParams(c0, c1, c2, c3,
                out var a, out var b, out var c, out var d);

            var dt_1 = 1f / (handles.Length - 1);
            var dt_2 = dt_1 * dt_1;
            var dt_3 = dt_1 * dt_2;

            for (int i = 0; i < handles.Length - 1; i++)
            {
                var t1_1 = i * dt_1;
                var t1_2 = t1_1 * t1_1;

                var p0 = a * dt_3;
                var p1 = (3 * a * t1_1 + b) * dt_2;
                var p2 = (2 * b * t1_1 + c + 3 * a * t1_2) * dt_1;
                var p3 = a * t1_1 * t1_2 + b * t1_2 + c * t1_1 + d;

                GetCubicPoints(p0, p1, p2, p3,
                    out var _1, out var _2, out var _3, out var _4);

                handles[i + 0].anchor = _1;
                handles[i + 0].controlA = _2 - _1;
                handles[i + 1].controlB = _3 - _4;
                handles[i + 1].anchor = _4;
            }

            return handles;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spline"></param>
        /// <param name="h0"></param>
        /// <param name="h1"></param>
        /// <param name="n"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        public static bool Cu2Qu(out Vector2[] spline, Handle h0, Handle h1, int n, float err)
        {
            var cache = new List<Vector2>();

            cache.Add(h0.anchor);

            for (int i = 1; i < (n + 1); i++)
            {
                var split = Split(h0, h1, i);

                var q0 = split[0].anchor;

                var pass = true;

                for (int j = 0; j < (split.Length - 1); j++)
                {
                    var split0 = split[j + 0];
                    var split1 = split[j + 1];

                    GetCubic(split0, split1,
                        out var c0, out var c2, out var c1, out var c3);

                    var t = (float)j / (split.Length - 2);

                    var qL = c0 + (c1 - c0) * 1.5f;
                    var qR = c3 + (c2 - c3) * 1.5f;

                    var q1 = qL + (qR - qL) * t;

                    if (j > 0)
                    {
                        cache.Add((cache[cache.Count - 1] + q1) * 0.5f);
                    }

                    cache.Add(q1);

                    if (Vector2.Distance((q0 + q1) * 0.5f, c0) > err)
                    {
                        pass = false;

                        cache.RemoveRange(1, cache.Count - 1);

                        break;
                    }

                    q0 = q1;
                }

                if (pass)
                {
                    break;
                }
            }

            cache.Add(h1.anchor);

            spline = cache.ToArray();

            return spline.Length > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spline"></param>
        /// <param name="n"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        public bool Cu2Qu(out Vector2[] spline, int n, float err)
        {
            if (handles.Count < 2)
            {
                spline = new Vector2[0];

                return false;
            }

            var cache = new List<Vector2>();

            if (Cu2Qu(out var tmp, handles[0], handles[1], n, err))
            {
                cache.AddRange(tmp);
            }

            for (int i = 1; i < (handles.Count - 1); i++)
            {
                var h0 = handles[i + 0];
                var h1 = handles[i + 1];

                if (Cu2Qu(out tmp, h0, h1, n, err))
                {
                    cache.AddRange(tmp.Skip(1));
                }
            }

            if (closed)
            {
                var h0 = handles[handles.Count - 1];
                var h1 = handles[0];

                if (Cu2Qu(out tmp, h0, h1, n, err))
                {
                    cache.AddRange(tmp.Skip(1));
                }
            }

            spline = cache.ToArray();

            return spline.Length > 0;
        }
    }
}
