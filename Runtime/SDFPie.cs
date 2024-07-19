/***
* This code is adapted and modified from
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/ImageWithRoundedCorners.cs
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/Editor/ImageWithIndependentRoundedCornersInspector.cs
**/

using UnityEngine;

namespace TLab.UI.SDF
{
	public class SDFPie : SDFUI
	{
		protected override string OUTLINE_INSIDE => "UI/SDF/Pie/Outline/Inside";
		protected override string OUTLINE_OUTSIDE => "UI/SDF/Pie/Outline/Outside";

		[SerializeField, Min(0)] private float m_radius = 40;

		[Range(0, Mathf.PI), SerializeField]
		private float m_theta = Mathf.PI * 0.5f;

		public static readonly int PROP_RADIUSE = Shader.PropertyToID("_Radius");
		public static readonly int PROP_THETA = Shader.PropertyToID("_Theta");

		public float radius
		{
			get => m_radius;
			set
			{
				if (m_radius != value)
				{
					m_radius = value;

					SetAllDirty();
				}
			}
		}

		public float theta
		{
			get => m_theta;
			set
			{
				if (m_theta != value)
				{
					m_theta = value;

					SetAllDirty();
				}
			}
		}

		public override void SetMaterialDirty()
		{
			base.SetMaterialDirty();

			m_material.SetFloat(PROP_THETA, m_theta);
			m_material.SetFloat(PROP_RADIUSE, m_radius);
		}
	}
}