/***
* This code is adapted and modified from
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/ImageWithRoundedCorners.cs
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/Editor/ImageWithIndependentRoundedCornersInspector.cs
**/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

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

		private const float PI = Mathf.PI;

		[SerializeField, Range(0.0f, 1.0f)] public float radius = 0.4f;
		[SerializeField] public Vector2 corner0 = new Vector2(0.0f, 0.5f);
		[SerializeField] public Vector2 corner1 = new Vector2(0.5f, -0.3535534f);
		[SerializeField] public Vector2 corner2 = new Vector2(-0.5f, -0.3535534f);

		protected override void OnValidate()
		{
			base.OnValidate();

			corner0.x = Mathf.Clamp(corner0.x, -0.5f, 0.5f);
			corner0.y = Mathf.Clamp(corner0.y, -0.5f, 0.5f);
			corner1.x = Mathf.Clamp(corner1.x, -0.5f, 0.5f);
			corner1.y = Mathf.Clamp(corner1.y, -0.5f, 0.5f);
			corner2.x = Mathf.Clamp(corner2.x, -0.5f, 0.5f);
			corner2.y = Mathf.Clamp(corner2.y, -0.5f, 0.5f);

			Validate("Triangle");
			Refresh();
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			Validate("Triangle");
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
			float baseEdge = Mathf.Min(halfRect.x, halfRect.y);
			List<Vector4> corners = new List<Vector4>();
			corners.Add(corner0 * baseEdge);
			corners.Add(corner1 * baseEdge);
			corners.Add(corner2 * baseEdge);
			m_material.SetVector(PROP_HALFSIZE, halfRect);
			m_material.SetFloat(PROP_RADIUSES, radius * baseEdge);
			m_material.SetVectorArray(PROP_CORNERS, corners);
			m_material.SetFloat(PROP_OUTLINEWIDTH, outlineWidth * baseEdge);
			m_material.SetColor(PROP_OUTLINECOLOR, outlineColor);
		}
	}
}