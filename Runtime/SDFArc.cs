/***
* This code is adapted from
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/ImageWithRoundedCorners.cs
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/Editor/ImageWithIndependentRoundedCornersInspector.cs
**/

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TLab.UI.SDF
{
	public class SDFArc : SDFCircleBasedArc
	{
#if UNITY_EDITOR
		[MenuItem("GameObject/UI/SDFUI/SDFArc", false)]
		private static void Create(MenuCommand menuCommand)
		{
			Create<SDFArc>(menuCommand);
		}
#endif

		protected override string SHADER_NAME => "Hidden/UI/SDF/Arc/Outline";
	}
}