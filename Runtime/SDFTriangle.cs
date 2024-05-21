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

		public float radius = 40;
		public Vector2 corner0 = new Vector2(-45f, -45f);
		public Vector2 corner1 = new Vector2(45f, -45f);
		public Vector2 corner2 = new Vector2(0f, 45f);

		protected override void OnValidate()
		{
			base.OnValidate();

			Validate(SHAPE_NAME);
			Refresh();
		}

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
			corners.Add(corner0);
			corners.Add(corner1);
			corners.Add(corner2);
			m_material.SetVector(PROP_HALFSIZE, halfRect);
			m_material.SetFloat(PROP_RADIUSE, radius);
			m_material.SetVectorArray(PROP_CORNER, corners);

			m_material.SetInt(PROP_ONION, onion ? 1 : 0);
			m_material.SetFloat(PROP_ONIONWIDTH, onion ? onionWidth : 0);

			m_material.SetFloat(PROP_OUTLINEWIDTH, outline ? outlineWidth : 0);
			m_material.SetColor(PROP_OUTLINECOLOR, outline ? outlineColor : alpha0);
		}
	}
}