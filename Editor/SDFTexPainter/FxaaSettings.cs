using UnityEngine;
using Unity.Jobs;
using Unity.Collections;

namespace TLab.UI.SDF.Editor
{
    [System.Serializable]
    public class FXAASettings
    {
        [Range(0.0f, 1.0f)]
        public float fixedThreshold = 0.0312f;

        [Range(0.0f, 1.0f)]
        public float relativeThreshold = 0.063f;

        [Range(0.0f, 1.0f)]
        public float subpixelBlending = 0.75f;

        public string THIS_NAME => "[" + GetType() + "] ";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rasterized"></param>
        /// <param name="rasterizedTex"></param>
        /// <param name="texSize"></param>
        /// <param name="channelSize"></param>
        public void Fxaa(in NativeArray<byte> rasterized, in Texture2D rasterizedTex,
            Vector2Int texSize, int channelSize)
        {
            Debug.Log(THIS_NAME + "Start Fxaa effect");

            var fxaaResult = new NativeArray<byte>(rasterized.Length, Allocator.TempJob);

            var edgeStepSizes = new NativeArray<float>(10, Allocator.TempJob);
            edgeStepSizes[0] = 1.0f;
            edgeStepSizes[1] = 1.0f;
            edgeStepSizes[2] = 1.0f;
            edgeStepSizes[3] = 1.0f;
            edgeStepSizes[4] = 1.5f;
            edgeStepSizes[5] = 2.0f;
            edgeStepSizes[6] = 2.0f;
            edgeStepSizes[7] = 2.0f;
            edgeStepSizes[8] = 2.0f;
            edgeStepSizes[9] = 4.0f;

            var fxaaJob = new FxaaJob
            {
                SOURCE = rasterized,
                EDGE_STEP_SIZES = edgeStepSizes,
                FXAA_CONFIG = new Vector4(fixedThreshold, relativeThreshold, subpixelBlending, 0.0f),
                LAST_EDGE_STEP_GUESS = 8.0f,
                WIDTH = texSize.x,
                HEIGHT = texSize.y,

                result = fxaaResult
            };

            var handle = fxaaJob.Schedule(fxaaResult.Length, 1);

            JobHandle.ScheduleBatchedJobs();

            handle.Complete();

            edgeStepSizes.Dispose();

            Debug.Log(THIS_NAME + "Finish Fxaa effect");

            var r8ToARGBResult = new NativeArray<byte>(fxaaResult.Length * channelSize, Allocator.TempJob);

            var copyR8ToARGB32 = new R8ToNChannel
            {
                SOURCE = fxaaResult,
                CHANNEL_SIZE = channelSize,
                result = r8ToARGBResult
            };

            handle = copyR8ToARGB32.Schedule(r8ToARGBResult.Length, 1);

            JobHandle.ScheduleBatchedJobs();

            handle.Complete();

            if (rasterizedTex != null)
            {
                rasterizedTex.LoadRawTextureData(r8ToARGBResult);
                rasterizedTex.Apply();
            }

            fxaaResult.Dispose();
            r8ToARGBResult.Dispose();
        }
    }
}
