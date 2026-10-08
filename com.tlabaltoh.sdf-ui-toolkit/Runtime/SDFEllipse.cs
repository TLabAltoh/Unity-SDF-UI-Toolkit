using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TLab.UI.SDF
{
	public class SDFEllipse : SDFUI
	{
#if UNITY_EDITOR
		[MenuItem("GameObject/UI/SDFUI/SDFEllipse", false)]
		private static void Create(MenuCommand menuCommand)
		{
			Create<SDFEllipse>(menuCommand);
		}
#endif

		protected override string SHADER_NAME => $"Hidden/UI/SDF/Ellipse/{SHADER_TYPE}/Outline";
	}
}