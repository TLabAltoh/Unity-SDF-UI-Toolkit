/***
* This code is adapted from
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/ImageWithRoundedCorners.cs
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/Editor/ImageWithIndependentRoundedCornersInspector.cs
**/

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TLab.UI.SDF
{
	public class SDFCircle : SDFUI
	{
#if UNITY_EDITOR
		[MenuItem("GameObject/UI/SDFUI/SDFCircle", false)]
		private static void Create(MenuCommand menuCommand)
		{
			Create<SDFCircle>(menuCommand);
		}
#endif

		protected override string SHAPE_NAME => "Circle";

		public static readonly int PROP_RADIUS = Shader.PropertyToID("_Radius");

		protected override bool UpdateMaterialRecord(in string shapeName, bool simplification = false)
		{
			base.UpdateMaterialRecord(shapeName);

			_materialRecord.SetFloat(PROP_RADIUS, minSize * 0.5f);

			return true;
		}
	}
}