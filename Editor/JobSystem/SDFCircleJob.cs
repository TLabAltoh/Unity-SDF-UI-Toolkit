using Unity.Jobs;
using UnityEngine;
using Unity.Collections;

namespace TLab.UI.SDF.Editor
{
    public struct CircleN
    {
        public Vector2 center;
        public float radius;
        public float thickness;

        public Draw draw;
    }

    public struct SDFCircleJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<CircleN> CIRCLES;
        [ReadOnly] public int TEX_WIDTH;
        [ReadOnly] public int SDF_WIDTH;
        [ReadOnly] public int TEX_HEIGHT;
        [ReadOnly] public int SDF_HEIGHT;
        [ReadOnly] public float MAX_DIST;

        public NativeArray<byte> result;

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
        public float USwap(float a, float b)
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
            // -MAX_DIST ~ +MAX_DIST
            // 0 ~ 1

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

            for (int i = 0; i < CIRCLES.Length; i++)
            {
                var circle = CIRCLES[i];

                switch (circle.draw)
                {
                    case Draw.FILL:
                        var tmp = Vector2.Distance(circle.center, texP);

                        tmp = tmp - circle.radius;

                        min = USwap(min, tmp);

                        var dist = Fill(min, MAX_DIST);

                        result[index] = result[index] < dist ? dist : result[index];
                        break;
                    case Draw.STROKE:
                        tmp = Vector2.Distance(circle.center, texP);

                        tmp = tmp - circle.radius;

                        tmp = Mathf.Abs(tmp) - circle.thickness;

                        min = USwap(min, tmp);

                        dist = Fill(min, MAX_DIST);

                        result[index] = result[index] < dist ? dist : result[index];
                        break;
                }
            }
        }
    }
}
