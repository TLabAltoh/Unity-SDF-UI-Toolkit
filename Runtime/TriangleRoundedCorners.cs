/***
* This code is adapted and modified from
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/ImageWithRoundedCorners.cs
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/Editor/ImageWithIndependentRoundedCornersInspector.cs
**/

using System.Collections.Generic;
using UnityEngine;

namespace Nobi.UiRoundedCorners
{
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(RectTransform))]
	public class TriangleRoundedCorners : CustomRoundedCorners
	{
		private static readonly int PROP_HALFSIZE = Shader.PropertyToID("_halfSize");
		private static readonly int PROP_RADIUSES = Shader.PropertyToID("_r");
		private static readonly int PROP_CORNERS = Shader.PropertyToID("_corners");
		private static readonly int PROP_OUTLINECOLOR = Shader.PropertyToID("_outlineColor");
		private static readonly int PROP_OUTLINEWIDTH = Shader.PropertyToID("_outlineWidth");

		private static readonly string SHAPE_NAME = "Triangle";

		[SerializeField] public float radius = 40;
		[SerializeField] public Vector2 corner0 = new Vector2(-45f, -45f);
		[SerializeField] public Vector2 corner1 = new Vector2(45f, -45f);
		[SerializeField] public Vector2 corner2 = new Vector2(0f, 45f);

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
			Vector2 halfRect = ((RectTransform)transform).rect.size * .5f;
			List<Vector4> corners = new List<Vector4>();
			corners.Add(corner0);
			corners.Add(corner1);
			corners.Add(corner2);
			m_material.SetVector(PROP_HALFSIZE, halfRect);
			m_material.SetFloat(PROP_RADIUSES, radius);
			m_material.SetVectorArray(PROP_CORNERS, corners);
			m_material.SetFloat(PROP_OUTLINEWIDTH, outlineWidth);
			m_material.SetColor(PROP_OUTLINECOLOR, outlineColor);
		}
	}
}