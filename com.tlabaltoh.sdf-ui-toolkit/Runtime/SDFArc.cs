#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Unity.Mathematics;

namespace TLab.UI.SDF
{
	public class SDFArc : SDFCircleBased
	{
#if UNITY_EDITOR
		[MenuItem("GameObject/UI/SDFUI/SDFArc", false)]
		private static void Create(MenuCommand menuCommand)
		{
			Create<SDFArc>(menuCommand);
		}
#endif

		protected override string SHADER_NAME => $"Hidden/UI/SDF/Arc/{SHADER_TYPE}/Outline";

		[SerializeField, Min(0)] private float m_cornersRounding = 0;

		[SerializeField] private float m_startAngle = 0;
		[SerializeField, Range(0, 1)] protected float m_ratio = 0.45f;

		internal static readonly int PROP_WIDTH = Shader.PropertyToID("_Width");
		internal static readonly int PROP_CIRCLE_BORDER = Shader.PropertyToID("_CircleBorder");
		internal static readonly int PROP_ANGLE_OFFSET = Shader.PropertyToID("_AngleOffset");
		internal static readonly int PROP_CORNERS_ROUNDING = Shader.PropertyToID("_CornersRounding");

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

			float cornersRounding = math.max(0, m_cornersRounding);
			float width = minSize * (1 - m_ratio) * 0.5f;
			cornersRounding = math.min(cornersRounding, width);
			_materialRecord.SetFloat(PROP_RADIUSE, (minSize - width) * 0.5f);
			_materialRecord.SetFloat(PROP_WIDTH, width - cornersRounding);
			_materialRecord.SetFloat(PROP_CORNERS_ROUNDING, cornersRounding * 0.5f);
			_materialRecord.SetFloat(PROP_CIRCLE_BORDER, width * 0.5f);

			float fill = fillAmount * 180;
			float start = startAngle;
			float2 angleOffset = new();
			if (fillAmount < 0)
			{
				start += math.mad(-2, fill, 360);
				fill = -fill;
				_materialRecord.SetFloat(PROP_THETA, -m_fillAmount * math.PI);
			}

			float angleOffsetTheta = math.radians(start - fill);
			math.sincos(angleOffsetTheta, out angleOffset.x, out angleOffset.y);
			_materialRecord.SetVector(PROP_ANGLE_OFFSET, angleOffset);

			_materialRecord.SetFloat(PROP_EULER_Z, math.radians(eulerZ) + math.radians(startAngle) - (fillAmount * math.PI) - (math.PI * 0.5f));
		}
	}
}