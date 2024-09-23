using UnityEngine;
using Unity.Jobs;
using Unity.Collections;

namespace TLab.UI.SDF.Editor
{
    public static class PixelFormatUtil
    {
        public static string THIS_NAME => "[PixelFormatUtil] ";

        public static void LoadR8AsARGB32(in NativeArray<byte> R8Buffer, in Texture2D dstText)
        {
            var R8xNBuffer = new NativeArray<byte>(R8Buffer.Length * 4, Allocator.TempJob);

            var R8ToARGB32Job = new R8ToR8xNJob
            {
                R8Buffer = R8Buffer,
                channelSize = 4,
                R8xNBuffer = R8xNBuffer
            };

            var handle = R8ToARGB32Job.Schedule(R8xNBuffer.Length, 1);

            JobHandle.ScheduleBatchedJobs();

            handle.Complete();

            if (dstText != null)
            {
                dstText.LoadRawTextureData(R8xNBuffer);
                dstText.Apply();
            }

            R8xNBuffer.Dispose();
        }

        public struct R8ToR8xNJob : IJobParallelFor
        {
            [ReadOnly] public NativeArray<byte> R8Buffer;
            [ReadOnly] public int channelSize;

            public NativeArray<byte> R8xNBuffer;

            public void Execute(int index) => R8xNBuffer[index] = R8Buffer[index / channelSize];
        }
    }
}
