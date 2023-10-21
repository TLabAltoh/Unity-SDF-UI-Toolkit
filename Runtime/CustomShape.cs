/***
* This code is adapted and modified from
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/ImageWithRoundedCorners.cs
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/Editor/ImageWithIndependentRoundedCornersInspector.cs
**/

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
	public class CustomShape : CustomRoundedCorners
	{
		private static readonly int PROP_SDFTEX = Shader.PropertyToID("_SDFTex");
		private static readonly int PROP_HALFSIZE = Shader.PropertyToID("_halfSize");
		private static readonly int PROP_RADIUSES = Shader.PropertyToID("_r");
		private static readonly int PROP_OUTLINECOLOR = Shader.PropertyToID("_outlineColor");
		private static readonly int PROP_OUTLINEWIDTH = Shader.PropertyToID("_outlineWidth");

		[SerializeField, Range(0.0f, 1.0f)] public float radius = 0.4f;
		[SerializeField] public Texture2D sdfTexture;

		protected override void OnValidate()
		{
			base.OnValidate();

			Validate("CustomShape");
			Refresh();
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			Validate("CustomShape");
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
			m_material.SetTexture(PROP_SDFTEX, sdfTexture);
			m_material.SetVector(PROP_HALFSIZE, halfRect);
			m_material.SetFloat(PROP_RADIUSES, radius);
			m_material.SetFloat(PROP_OUTLINEWIDTH, outlineWidth);
			m_material.SetColor(PROP_OUTLINECOLOR, outlineColor);
		}
	}
}
