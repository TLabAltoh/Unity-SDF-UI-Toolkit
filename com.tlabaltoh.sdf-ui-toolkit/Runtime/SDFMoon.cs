using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TLab.UI.SDF
{
	public class SDFMoon : SDFUI
	{
#if UNITY_EDITOR
		[MenuItem("GameObject/UI/SDFUI/SDFMoon", false)]
		private static void Create(MenuCommand menuCommand)
		{
			Create<SDFMoon>(menuCommand);
		}
#endif

		protected override string SHADER_NAME => $"Hidden/UI/SDF/Moon/{SHADER_TYPE}/Outline";

		[SerializeField, Range(0f, 1f)] private float m_innerRadius = 0.5f;
		[SerializeField, Range(-1f, 1f)] private float m_slide = 0.5f;
		[SerializeField, Min(0f)] private float m_roundness = 0.0f;

		internal static readonly int PROP_RADIUS_A = Shader.PropertyToID("_RadiusA");
		internal static readonly int PROP_RADIUS_B = Shader.PropertyToID("_RadiusB");
		internal static readonly int PROP_SLIDE = Shader.PropertyToID("_Slide");
		internal static readonly int PROP_ROUNDNESS = Shader.PropertyToID("_Roundness");

		public float innerRadius
		{
			get => m_innerRadius;
			set
			{
				var tmp = Mathf.Clamp(value, 0, 1);
				if (m_innerRadius != tmp)
				{
					m_innerRadius = tmp;

					SetAllDirty();
				}
			}
		}

		public float slide
		{
			get => m_slide;
			set
			{
				var tmp = Mathf.Clamp(value, -1, 1);
				if (m_slide != tmp)
				{
					m_slide = tmp;

					SetAllDirty();
				}
			}
		}

		public float roundness
		{
			get => m_roundness;
			set
			{
				var tmp = Mathf.Max(value, 0);
				if (m_roundness != tmp)
				{
					m_roundness = tmp;

					SetAllDirty();
				}
			}
		}

		protected override void UpdateMaterialRecord()
		{
			base.UpdateMaterialRecord();

			var baseSize = minSize - (2 * m_roundness);
			var radius = baseSize * 0.5f;
			var innerRadius = radius * m_innerRadius;
			var slide = baseSize * m_slide;

			_materialRecord.SetFloat(PROP_RADIUS_A, radius);
			_materialRecord.SetFloat(PROP_RADIUS_B, innerRadius);
			_materialRecord.SetFloat(PROP_SLIDE, slide);
			_materialRecord.SetFloat(PROP_ROUNDNESS, m_roundness);
		}
	}
}