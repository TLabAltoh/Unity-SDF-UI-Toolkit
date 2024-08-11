/***
* This code is adapted and modified from
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/ImageWithRoundedCorners.cs
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/Editor/ImageWithIndependentRoundedCornersInspector.cs
**/

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TLab.UI.SDF
{
	public class SDFPie : SDFUI
	{
#if UNITY_EDITOR
		[MenuItem("GameObject/UI/SDFUI/SDFPie", false)]
		private static void Create(MenuCommand menuCommand)
		{
			Create<SDFPie>(menuCommand);
		}
#endif

		protected override string SHADER_NAME => "UI/SDF/Pie/Outline";

		[Range(0, 1), SerializeField]
		private float m_fillAmount = 0.5f;

		public static readonly int PROP_RADIUSE = Shader.PropertyToID("_Radius");
		public static readonly int PROP_THETA = Shader.PropertyToID("_Theta");

		public float fillAmount
		{
			get => m_fillAmount;
			set
			{
				if (m_fillAmount != value)
				{
					m_fillAmount = value;

					SetAllDirty();
				}
			}
		}

		public override void SetMaterialDirty()
		{
			base.SetMaterialDirty();

			_materialRecord.SetFloat(PROP_THETA, m_fillAmount * Mathf.PI);
			_materialRecord.SetFloat(PROP_RADIUSE, minSize * 0.5f);
		}
	}
}