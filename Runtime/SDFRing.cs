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
	public class SDFRing : SDFCircleBasedArc
	{
#if UNITY_EDITOR
		[MenuItem("GameObject/UI/SDFUI/SDFRing", false)]
		private static void Create(MenuCommand menuCommand)
		{
			Create<SDFRing>(menuCommand);
		}
#endif

		protected override string SHADER_NAME => "Hidden/UI/SDF/Ring/Outline";

		protected override void UpdateMaterialRecord()
		{
			base.UpdateMaterialRecord();

			float width = m_width;
			_materialRecord.SetFloat(PROP_WIDTH, width);
		}
	}
}