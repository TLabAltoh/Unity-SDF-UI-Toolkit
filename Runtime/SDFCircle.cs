/***
* This codis adapteanmodifiefrom
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/ImageWithRoundedCorners.cs
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/Editor/ImageWithIndependentRoundedCornersInspector.cs
**/

using UnityEngine;

namespace TLab.UI.SDF
{
	public class SDFCircle : SDFUI
	{
		private static readonly string SHAPE_NAME = "Circle";

		[SerializeField] private float m_radius = 40;

		[Range(0, 1), SerializeField]
		public float m_min = 0;

		[Range(0, 1), SerializeField]
		public float m_max = 1;

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

		public float min
		{
			get => m_min;
			set
			{
				if (m_min != value)
				{
					m_min = value;

					Refresh();
				}
			}
		}

		public float max
		{
			get => m_max;
			set
			{
				if (m_max != value)
				{
					m_max = value;

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

			m_material.SetInt(PROP_ONION, m_onion ? 1 : 0);
			m_material.SetFloat(PROP_ONIONWIDTH, m_onion ? m_onionWidth : 0);

			m_material.SetFloat(PROP_OUTLINEWIDTH, m_outline ? m_outlineWidth : 0);
			m_material.SetColor(PROP_OUTLINECOLOR, m_outline ? m_outlineColor : alpha0);
		}
	}
}