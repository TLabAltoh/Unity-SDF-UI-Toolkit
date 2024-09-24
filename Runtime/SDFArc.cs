/***
* This code is adapted from
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/ImageWithRoundedCorners.cs
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/Editor/ImageWithIndependentRoundedCornersInspector.cs
**/

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

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

		[SerializeField, Range(0, 1)]
		private float m_cornersRounding = 0;

		[SerializeField, Range(0, 1)]
		private float m_startAngle = 0;

		public static readonly int PROP_ANGLE_OFFSET = Shader.PropertyToID("_AngleOffset");
		public static readonly int PROP_CORNERS_ROUNDING = Shader.PropertyToID("_CornersRounding");

		public float cornersRounding
		{
			get => m_cornersRounding;
			set
			{
				if (m_cornersRounding != value)
				{
					m_cornersRounding = value;

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

		protected override void UpdateMaterialRecord()
		{
			base.UpdateMaterialRecord();

			var cornersRounding = Mathf.Max(0, m_width * m_cornersRounding);
			_materialRecord.SetFloat(PROP_WIDTH, m_width - cornersRounding);
			_materialRecord.SetFloat(PROP_CORNERS_ROUNDING, cornersRounding * 0.5f);

			var angleOffsetTheta = (m_fillAmount - startAngle * 2) * Mathf.PI;
			var angleOffset = new Vector2(Mathf.Cos(angleOffsetTheta), Mathf.Sin(angleOffsetTheta));
			_materialRecord.SetVector(PROP_ANGLE_OFFSET, angleOffset);
		}
	}
}