using UnityEngine;
using Unity.Jobs;
using Unity.Collections;

namespace TLab.UI.SDF.Editor
{
    public static class ConvertFormat
    {
        public static string THIS_NAME => "[convertformat] ";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rasterized"></param>
        /// <param name="rasterizedTex"></param>
        /// <param name="channelSize"></param>
        public static void R8ToNChannel(in NativeArray<byte> rasterized, in Texture2D rasterizedTex, int channelSize)
        {
            var nChannelBuffer = new NativeArray<byte>(rasterized.Length * channelSize, Allocator.TempJob);

            var copyR8ToARGB32 = new R8ToNChannel
            {
                SOURCE = rasterized,
                CHANNEL_SIZE = channelSize,
                result = nChannelBuffer
            };

            var handle = copyR8ToARGB32.Schedule(nChannelBuffer.Length, 1);

            JobHandle.ScheduleBatchedJobs();

            handle.Complete();

            if (rasterizedTex != null)
            {
                rasterizedTex.LoadRawTextureData(nChannelBuffer);
                rasterizedTex.Apply();
            }

            nChannelBuffer.Dispose();
        }
    }
}
