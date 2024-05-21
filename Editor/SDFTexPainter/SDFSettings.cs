using UnityEngine;

namespace TLab.UI.SDF.Editor
{
    [System.Serializable]
    public class SDFSettings
    {
        [Range(0.0f, 512.0f)]
        public float maxDist = 50.0f;
    }
}
