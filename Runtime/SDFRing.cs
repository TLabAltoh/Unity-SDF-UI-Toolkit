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

		[SerializeField] private float m_radius = 40;

		[SerializeField] private float m_width = 10;

		[Range(0, Mathf.PI), SerializeField]
		private float m_theta = Mathf.PI * 0.5f;

		public float radius
		{
			get => m_radius;
			set
			{
				if (m_radius != value)
				{
					m_radius = value;

					Refresh();
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

					Refresh();
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

					Refresh();
				}
			}
		}

#if UNITY_EDITOR
		protected override void OnValidate()
		{
			base.OnValidate();

			Validate(SHAPE_NAME);
			Refresh();
		}
#endif

		protected override void OnEnable()
		{
			base.OnEnable();

			Validate(SHAPE_NAME);
			Refresh();
		}

		protected override void OnRectTransformDimensionsChange()
		{
			base.OnRectTransformDimensionsChange();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
		}

		protected override void Refresh()
		{
			var halfRect = ((RectTransform)transform).rect.size * .5f;
			m_material.SetVector(PROP_HALFSIZE, halfRect);
			m_material.SetFloat(PROP_RADIUSE, m_radius);
			m_material.SetFloat(PROP_THETA, m_theta);
			m_material.SetFloat(PROP_WIDTH, m_width);

			m_material.SetInt(PROP_ONION, m_onion ? 1 : 0);
			m_material.SetFloat(PROP_ONIONWIDTH, m_onion ? m_onionWidth : 0);

			m_material.SetFloat(PROP_OUTLINEWIDTH, m_outline ? m_outlineWidth : 0);
			m_material.SetColor(PROP_OUTLINECOLOR, m_outline ? m_outlineColor : alpha0);
		}
	}
}