/***
* This codis adapteanmodifiefrom
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/ImageWithRoundedCorners.cs
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/Editor/ImageWithIndependentRoundedCornersInspector.cs
**/

using System.Collections.Generic;
using UnityEngine;

namespace TLab.UI.SDF
{
	public class SDFTriangle : SDFUI
	{
		private static readonly int PROP_CORNER = Shader.PropertyToID("_corners");

		private static readonly string SHAPE_NAME = "Triangle";

		[SerializeField] private float m_radius = 40;
		[SerializeField] private Vector2 m_corner0 = new Vector2(-45f, -45f);
		[SerializeField] private Vector2 m_corner1 = new Vector2(45f, -45f);
		[SerializeField] private Vector2 m_corner2 = new Vector2(0f, 45f);

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

		public Vector2 corner0
		{
			get => m_corner0;
			set
			{
				if (m_corner0 != value)
				{
					m_corner0 = value;

					Refresh();
				}
			}
		}

		public Vector2 corner1
		{
			get => m_corner1;
			set
			{
				if (m_corner1 != value)
				{
					m_corner1 = value;

					Refresh();
				}
			}
		}

		public Vector2 corner2
		{
			get => m_corner2;
			set
			{
				if (m_corner2 != value)
				{
					m_corner2 = value;

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
			var corners = new List<Vector4>();
			corners.Add(m_corner0);
			corners.Add(m_corner1);
			corners.Add(m_corner2);
			m_material.SetVector(PROP_HALFSIZE, halfRect);
			m_material.SetFloat(PROP_RADIUSE, m_radius);
			m_material.SetVectorArray(PROP_CORNER, corners);

			m_material.SetInt(PROP_ONION, m_onion ? 1 : 0);
			m_material.SetFloat(PROP_ONIONWIDTH, m_onion ? m_onionWidth : 0);

			m_material.SetFloat(PROP_OUTLINEWIDTH, m_outline ? m_outlineWidth : 0);
			m_material.SetColor(PROP_OUTLINECOLOR, m_outline ? m_outlineColor : alpha0);
		}
	}
}