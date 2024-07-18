/***
* This codis adapteanmodifiefrom
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/ImageWithRoundedCorners.cs
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/Editor/ImageWithIndependentRoundedCornersInspector.cs
**/

using UnityEngine;

namespace TLab.UI.SDF
{
	public class SDFRing : SDFUI
	{
		private static readonly string SHAPE_NAME = "Ring";

		[SerializeField, Min(0)] private float m_radius = 40;
		[SerializeField, Min(0)] private float m_width = 10;

		[Range(0, Mathf.PI), SerializeField]
		private float m_theta = Mathf.PI * 0.5f;

		public static readonly int PROP_RADIUSE = Shader.PropertyToID("_Radius");
		public static readonly int PROP_THETA = Shader.PropertyToID("_Theta");
		public static readonly int PROP_WIDTH = Shader.PropertyToID("_Width");

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

		public float width
		{
			get => m_width;
			set
			{
				if (m_width != value)
				{
					m_width = value;

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

#if UNITY_EDITOR
		protected override void OnValidate()
		{
			Validate(SHAPE_NAME);

			base.OnValidate();
		}
#endif

		protected override void OnEnable()
		{
			DeleteOldMat();

			Validate(SHAPE_NAME);

			base.OnEnable();
		}

		public override void SetMaterialDirty()
		{
			base.SetMaterialDirty();

			material.SetFloat(PROP_RADIUSE, m_radius);
			material.SetFloat(PROP_THETA, m_theta);
			material.SetFloat(PROP_WIDTH, m_width);
		}
	}
}