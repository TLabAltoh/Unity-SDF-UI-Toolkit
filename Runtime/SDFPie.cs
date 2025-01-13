/***
* This code is adapted and modified from
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/ImageWithRoundedCorners.cs
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/Editor/ImageWithIndependentRoundedCornersInspector.cs
**/

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#endif

namespace TLab.UI.SDF
{
	public class SDFPie : SDFCircleBased
	{
#if UNITY_EDITOR
		[MenuItem("GameObject/UI/SDFUI/SDFPie", false)]
		private static void Create(MenuCommand menuCommand)
		{
			Create<SDFPie>(menuCommand);
		}
#endif

		protected override string SHADER_NAME => "Hidden/UI/SDF/Pie/Outline";

		private static readonly int PROP_ROUNDNESS = Shader.PropertyToID("_Roundness");

		[SerializeField, Range(0, 1)] private float m_roundness = 0.1f;

		protected override float m_extraMargin
		{
			get
			{
				return base.m_extraMargin + m_roundness * this.minSize * 0.5f;
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

		protected override void UpdateMaterialRecord()
		{
			base.UpdateMaterialRecord();

			var minSize = this.minSize;
			_materialRecord.SetFloat(PROP_ROUNDNESS, m_roundness * minSize * 0.5f);
			_materialRecord.SetFloat(PROP_THETA, Mathf.Abs(m_fillAmount) * Mathf.PI);
		}
	}
}