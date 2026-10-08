using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TLab.UI.SDF
{
	public class SDFVesica : SDFUI
	{
#if UNITY_EDITOR
		[MenuItem("GameObject/UI/SDFUI/SDFVesica", false)]
		private static void Create(MenuCommand menuCommand)
		{
			Create<SDFVesica>(menuCommand);
		}
#endif

		protected override string SHADER_NAME => $"Hidden/UI/SDF/Vesica/{SHADER_TYPE}/Outline";

		[SerializeField, Min(0f)] private float m_roundness = 0.0f;

		internal static readonly int PROP_ROUNDNESS = Shader.PropertyToID("_Roundness");

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

			_materialRecord.SetFloat(PROP_ROUNDNESS, m_roundness);
		}
	}
}