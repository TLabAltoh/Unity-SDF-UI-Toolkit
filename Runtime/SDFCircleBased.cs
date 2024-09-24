/***
* This code is adapted and modified from
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/ImageWithRoundedCorners.cs
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/Editor/ImageWithIndependentRoundedCornersInspector.cs
**/

using UnityEngine;
#if UNITY_EDITOR
#endif

namespace TLab.UI.SDF
{
	public abstract class SDFCircleBased : SDFUI
	{
		[Range(0, 1), SerializeField]
		protected float m_fillAmount = 0.5f;

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

		protected override void UpdateMaterialRecord()
		{
			base.UpdateMaterialRecord();

			_materialRecord.SetFloat(PROP_THETA, m_fillAmount * Mathf.PI);
			_materialRecord.SetFloat(PROP_RADIUSE, minSize * 0.5f);
		}
	}

	public abstract class SDFCircleBasedArc : SDFCircleBased
	{
		[SerializeField, Min(0)] protected float m_width = 10;

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

		protected override void UpdateMaterialRecord()
		{
			base.UpdateMaterialRecord();

			float width = m_width * 0.5f;
			_materialRecord.SetFloat(PROP_RADIUSE, minSize * 0.5f - width);
			_materialRecord.SetFloat(PROP_WIDTH, width);
		}
	}
}