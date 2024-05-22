/***
* This codis adapteanmodifiefrom
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/ImageWithRoundedCorners.cs
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/Editor/ImageWithIndependentRoundedCornersInspector.cs
**/

using UnityEngine;

namespace TLab.UI.SDF
{
	public class SDFCutDisk : SDFUI
	{
		private static readonly string SHAPE_NAME = "CutDisk";

		[SerializeField] private float m_radius = 40;

		[SerializeField] private float m_height = 10;

		public float radius
		{
			get => radius;
			set
			{
				if (m_radius != value)
				{
					m_radius = value;

					Refresh();
				}
			}
		}

		public float height
		{
			get => m_height;
			set
			{
				if (m_height != value)
				{
					m_height = value;

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
			m_material.SetFloat(PROP_HEIGHT, m_height);

			m_material.SetInt(PROP_ONION, m_onion ? 1 : 0);
			m_material.SetFloat(PROP_ONIONWIDTH, m_onion ? m_onionWidth : 0);

			m_material.SetFloat(PROP_OUTLINEWIDTH, m_outline ? m_outlineWidth : 0);
			m_material.SetColor(PROP_OUTLINECOLOR, m_outline ? m_outlineColor : alpha0);
		}
	}
}