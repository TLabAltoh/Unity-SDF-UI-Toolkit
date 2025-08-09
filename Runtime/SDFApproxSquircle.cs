#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TLab.UI.SDF
{
    public class SDFApproxSquircle : SDFSquircleBased
    {
#if UNITY_EDITOR
        [MenuItem("GameObject/UI/SDFUI/SDFApproxSquircle", false)]
        private static void Create(MenuCommand menuCommand)
        {
            Create<SDFApproxSquircle>(menuCommand);
        }
#endif

        protected override string SHADER_NAME => $"Hidden/UI/SDF/ApproxSquircle/{SHADER_TYPE}/Outline";
    }
}