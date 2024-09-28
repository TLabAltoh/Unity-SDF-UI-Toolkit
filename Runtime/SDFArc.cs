/***
* This code is adapted from
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
	public class SDFArc : SDFPie
	{
#if UNITY_EDITOR
		[MenuItem("GameObject/UI/SDFUI/SDFArc", false)]
		private static void Create(MenuCommand menuCommand)
		{
			Create<SDFArc>(menuCommand);
		}
#endif

		protected override string SHAPE_NAME => "Arc";

		[SerializeField, Min(0)] private float m_cornersRounding = 0;

		[SerializeField, Range(0, 1)] protected float m_ratio = 10;

		public static readonly int PROP_WIDTH = Shader.PropertyToID("_Width");
		public static readonly int PROP_CIRCLE_BORDER = Shader.PropertyToID("_CircleBorder");
		public static readonly int PROP_CORNERS_ROUNDING = Shader.PropertyToID("_CornersRounding");

		public float ratio
		{
			get => m_ratio;
			set
			{
				if (m_ratio != value)
				{
					m_ratio = value;

					SetAllDirty();
				}
			}
		}
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

		protected override bool UpdateMaterialRecord(in string shapeName, bool simplification = false)
		{
			if (simplification && (m_ratio == 0) && (m_cornersRounding == 0))
			{
				base.UpdateMaterialRecord(base.SHAPE_NAME, simplification);
				return false;
			}

			base.UpdateMaterialRecord(shapeName);

			float cornersRounding = math.max(0, m_cornersRounding);
			float width = minSize * (1 - m_ratio) * 0.5f;
			cornersRounding = math.min(cornersRounding, width);
			_materialRecord.SetFloat(PROP_RADIUS, (minSize - width) * 0.5f);
			_materialRecord.SetFloat(PROP_WIDTH, width - cornersRounding);
			_materialRecord.SetFloat(PROP_CORNERS_ROUNDING, cornersRounding * 0.5f);
			_materialRecord.SetFloat(PROP_CIRCLE_BORDER, width * 0.5f);

			return true;
		}
	}
}