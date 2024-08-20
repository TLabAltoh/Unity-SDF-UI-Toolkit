/***
* This codis adapteanmodifiefrom
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/ImageWithRoundedCorners.cs
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/Editor/ImageWithIndependentRoundedCornersInspector.cs
**/

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TLab.UI.SDF
{
	public class SDFRing : SDFUI
	{
#if UNITY_EDITOR
		[MenuItem("GameObject/UI/SDFUI/SDFRing", false)]
		private static void Create(MenuCommand menuCommand)
		{
			Create<SDFRing>(menuCommand);
		}
#endif

		protected override string SHADER_NAME => "Hidden/UI/SDF/Ring/Outline";

		[SerializeField, Min(0)] private float m_width = 10;

		[Range(0, 1), SerializeField]
		private float m_fillAmount = 0.5f;

		public static readonly int PROP_RADIUSE = Shader.PropertyToID("_Radius");
		public static readonly int PROP_THETA = Shader.PropertyToID("_Theta");
		public static readonly int PROP_WIDTH = Shader.PropertyToID("_Width");

		public float width
		{
			get => m_width;
			set
			{
				if (m_width != value)
				{
					m_width = value;

					SetAllDirty();
				}
			}
		}

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

			float width = m_width * 2.0f;   // Adjust width property to rect size
			_materialRecord.SetFloat(PROP_RADIUSE, minSize * 0.5f - width * 0.5f);
			_materialRecord.SetFloat(PROP_THETA, m_fillAmount * Mathf.PI);
			_materialRecord.SetFloat(PROP_WIDTH, width);
		}
	}
}