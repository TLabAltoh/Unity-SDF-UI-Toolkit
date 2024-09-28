/***
* This code is adapted and modified from
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/ImageWithRoundedCorners.cs
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/Editor/ImageWithIndependentRoundedCornersInspector.cs
**/

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Unity.Mathematics;

namespace TLab.UI.SDF
{
	public class SDFPie : SDFCircle
	{
#if UNITY_EDITOR
		[MenuItem("GameObject/UI/SDFUI/SDFPie", false)]
		private static void Create(MenuCommand menuCommand)
		{
			Create<SDFPie>(menuCommand);
		}
#endif

		protected override string SHAPE_NAME => "Pie";

		[Range(-1, 1), SerializeField]
		protected float m_fillAmount = 0.5f;

		[SerializeField]
		private float m_startAngle = 0;

		public static readonly int PROP_THETA = Shader.PropertyToID("_Theta");
		public static readonly int PROP_ANGLE_OFFSET = Shader.PropertyToID("_AngleOffset");

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

		public float startAngle
		{
			get => m_startAngle;
			set
			{
				if (m_startAngle != value)
				{
					m_startAngle = value;

					SetAllDirty();
				}
			}
		}

		protected override bool UpdateMaterialRecord(in string shapeName, bool simplification = false)
		{
			if (simplification && math.abs(m_fillAmount) == 1)
			{
				base.UpdateMaterialRecord(base.SHAPE_NAME, simplification);
				return false;
			}

			base.UpdateMaterialRecord(shapeName);

			float fill = m_fillAmount * 180;
			float start = m_startAngle;
			float2 angleOffset = new();
			if (m_fillAmount < 0)
			{
				start += math.mad(-2, fill, 360);
				fill = -fill;
				_materialRecord.SetFloat(PROP_THETA, -m_fillAmount * Mathf.PI);
			}
			else
				_materialRecord.SetFloat(PROP_THETA, m_fillAmount * Mathf.PI);

			float angleOffsetTheta = math.radians(start - fill);
			math.sincos(angleOffsetTheta, out angleOffset.x, out angleOffset.y);
			_materialRecord.SetVector(PROP_ANGLE_OFFSET, angleOffset);

			return true;
		}
	}
}