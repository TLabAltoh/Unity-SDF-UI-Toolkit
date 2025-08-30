using UnityEngine;

namespace TLab.UI.SDF.Editor
{
    [System.Serializable]
    public class RasterizeOptions
    {
        [Min(0f)] public float maxDist = 50.0f;
    }
}
