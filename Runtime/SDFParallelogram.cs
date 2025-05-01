using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

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

		[SerializeField] private float m_slide = 0;

		public static readonly int PROP_SLIDE = Shader.PropertyToID("_Slide");

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

		protected override void UpdateMaterial()
		{
			base.UpdateMaterial();

			_materialRecord.SetFloat(PROP_SLIDE, (minSize * 0.5f) * m_slide);
		}
	}
}
