using Unity.Jobs;
using Unity.Collections;

namespace TLab.UI.SDF.Editor
{
    public struct R8ToNChannel : IJobParallelFor
    {
        [ReadOnly] public NativeArray<byte> SOURCE;
        [ReadOnly] public int CHANNEL_SIZE;

        public NativeArray<byte> result;

        public void Execute(int index)
        {
            result[index] = SOURCE[index / CHANNEL_SIZE];
        }
    }
}
