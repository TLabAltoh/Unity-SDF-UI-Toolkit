using System.Linq;
using UnityEngine;
using TMPro;

namespace TLab.UI.SDF.Test
{
    public class BuildTest : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_log;

        public void AddQuad()
        {
            this.gameObject.AddComponent<SDFQuad>();
        }

        public void MakeLog()
        {
            var quad = FindObjectOfType<SDFQuad>();
            if (quad != null)
            {
                var color = quad.fillColor;
                var shadow = quad.material.enabledKeywords.Select((e) => e.name).Contains("SDF_UI_AA_FASTER");
                m_log.text = color + ", " + shadow;
            }
        }
    }
}
