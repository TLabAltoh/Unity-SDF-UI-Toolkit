using System.Linq;
using UnityEngine;
using TMPro;

namespace TLab.UI.SDF.Test
{
    public class BuildTest : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_log;

        public void AddQuad() => gameObject.AddComponent<SDFQuad>();

        public void MakeLog()
        {
            var quad = FindObjectOfType<SDFQuad>();
            if (quad != null)
            {
                var color = quad.fillColor;
                var keywords = quad.material.enabledKeywords.Select((e) => e.name);
                var antialiasing = keywords.Contains("SFD_UI_AA");
                m_log.text = $"Color: {color}, AA: {antialiasing}";
            }
        }
    }
}
