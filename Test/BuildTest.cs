using UnityEngine;

namespace TLab.UI.SDF.Test
{
    public class BuildTest : MonoBehaviour
    {
        public void OnClick()
        {
            this.gameObject.AddComponent<SDFQuad>();
        }
    }
}
