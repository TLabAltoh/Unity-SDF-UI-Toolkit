#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace TLab.UI.SDF
{
	public class SDFParallelogram : SDFUI
	{
#if UNITY_EDITOR
		[MenuItem("GameObject/UI/SDFUI/SDFParallelogram", false)]
		private static void Create(MenuCommand menuCommand)
		{
			Create<SDFParallelogram>(menuCommand);
		}
#endif

		protected override string SHADER_NAME => "Hidden/UI/SDF/Parallelogram/Outline";

		[SerializeField, Range(-1, 1)] private float m_slide = 0;
		[SerializeField, Range(0, 1)] private float m_roundness = 0.1f;

		internal static readonly int PROP_ROUNDNESS = Shader.PropertyToID("_Roundness");
		internal static readonly int PROP_SLIDE = Shader.PropertyToID("_Slide");

		public float slide
		{
			get => m_slide;
			set
			{
				if (m_slide != value)
				{
					m_slide = value;

					SetAllDirty();
				}
			}
		}

		public float roundness
		{
			get => m_roundness;
			set
			{
				if (m_roundness != value)
				{
					m_roundness = value;

					SetAllDirty();
				}
			}
		}

		protected override void UpdateMaterial()
		{
			base.UpdateMaterial();

			var halfHorizontalSize = rectTransform.sizeDelta.x * 0.5f;
			var slide = halfHorizontalSize * m_slide;
			var round = halfHorizontalSize * m_roundness;
			var space = halfHorizontalSize - Mathf.Abs(slide);
			_materialRecord.SetFloat(PROP_SLIDE, slide);
			_materialRecord.SetFloat(PROP_ROUNDNESS, Mathf.Abs(round) < space ? round : Mathf.Sign(round) * Mathf.Abs(space));
		}
	}
}
