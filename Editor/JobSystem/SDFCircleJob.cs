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
        /// <param name="min"></param>
        /// <param name="maxDist"></param>
        /// <returns></returns>
        public byte Fill(float min, float maxDist)
        {
            // -MAX_DIST ~ +MAX_DIST
            // 0 ~ 1

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
            var wSign = 1.0f;

            for (int i = 0; i < CIRCLES.Length; i++)
            {
                var circle = CIRCLES[i];

                switch (circle.draw)
                {
                    case Draw.FILL:
                        var sd = Vector2.Distance(circle.center, texP);

                        sd = sd - circle.radius;

                        fMin = Mathf.Abs(fMin) < Mathf.Abs(sd) ? fMin : sd;
                        break;
                    case Draw.WINDING:
                        sd = Vector2.Distance(circle.center, texP);

                        sd = sd - circle.radius;

                        wMin = Mathf.Min(Mathf.Abs(wMin), Mathf.Abs(sd));

                        if (sd < 0)
                        {
                            wSign = -wSign;
                        }
                        break;
                    case Draw.STROKE:
                        sd = Vector2.Distance(circle.center, texP);

                        sd = sd - circle.radius;

                        sd = Mathf.Abs(sd) - circle.thickness;

                        sMin = Mathf.Abs(sMin) < Mathf.Abs(sd) ? sMin : sd;
                        break;
                }
            }

            var dist = Fill(Mathf.Min(fMin, wSign * wMin, sMin), MAX_DIST);

            result[index] = result[index] < dist ? dist : result[index];
        }
    }
}
