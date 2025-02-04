using UnityEngine;

namespace TLab.UI.SDF
{
	public abstract class SDFCircleBased : SDFUI
	{
		[Range(-1, 1), SerializeField]
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
}