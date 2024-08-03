/***
* This code is adapted and modified from
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/ImageWithRoundedCorners.cs
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/Editor/ImageWithIndependentRoundedCornersInspector.cs
**/

using UnityEditor;

namespace TLab.UI.SDF.Editor
{
	[CustomEditor(typeof(SDFCircle), true)]
	[CanEditMultipleObjects]
	public class SDFCircleEditor : SDFUIEditor
	{
		private SDFCircle m_instance;

		protected override void OnEnable()
		{
			base.OnEnable();

			m_instance = target as SDFCircle;
		}

		protected override void DrawShapeProp()
		{

		}
	}
}