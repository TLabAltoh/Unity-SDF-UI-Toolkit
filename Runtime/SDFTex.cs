/***
* This codis adapteanmodifiefrom
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/ImageWithRoundedCorners.cs
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/Editor/ImageWithIndependentRoundedCornersInspector.cs
**/

using UnityEngine;
using UnityEngine.UI;

namespace TLab.UI.SDF
{
	public class SDFTex : SDFUI
	{
		private static readonly int PROP_SDFTEX = Shader.PropertyToID("_SDFTex");
		private static readonly string SHAPE_NAME = "Tex";

		[SerializeField, Min(0)] private float m_radius = 40;
		[SerializeField] private Texture2D m_sdfTexture;

		public static readonly int PROP_RADIUSE = Shader.PropertyToID("_radius");

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

		public Texture2D sdfTexture
		{
			get => m_sdfTexture;
			set
			{
				if (m_sdfTexture != value)
				{
					m_sdfTexture = value;

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

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();

			var width = rectTransform.rect.width;
			var height = rectTransform.rect.height;

			var pivot = new Vector3(rectTransform.pivot.x * width, rectTransform.pivot.y * height, 0);

			var vertex = UIVertex.simpleVert;
			vertex.color = color;

			vertex.position = new Vector3(0, 0) - pivot;
			vertex.uv0 = new Vector2(0, 0);
			vh.AddVert(vertex);

			vertex.position = new Vector3(0, height) - pivot;
			vertex.uv0 = new Vector2(0, 1);
			vh.AddVert(vertex);

			vertex.position = new Vector3(width, height) - pivot;
			vertex.uv0 = new Vector2(1, 1);
			vh.AddVert(vertex);

			vertex.position = new Vector3(width, 0) - pivot;
			vertex.uv0 = new Vector2(1, 0);
			vh.AddVert(vertex);

			vh.AddTriangle(0, 1, 2);
			vh.AddTriangle(2, 3, 0);
		}

		protected override void Refresh()
		{
			material.SetVector(PROP_HALFSIZE, ((RectTransform)transform).rect.size * .5f);

			material.mainTexture = m_mainTexture;
			material.mainTextureScale = m_mainTextureScale;
			material.mainTextureOffset = m_mainTextureOffset;
			material.color = m_mainColor;

			if (m_onion)
			{
				material.SetInt(PROP_ONION, 1);
				material.SetFloat(PROP_ONIONWIDTH, m_onionWidth);
			}
			else
			{
				material.SetInt(PROP_ONION, 0);
				material.SetFloat(PROP_ONIONWIDTH, 0);
			}

			float shadowWidth = m_shadowWidth;

			if (m_shadow)
			{
				material.SetFloat(PROP_SHADOWWIDTH, shadowWidth);
				material.SetColor(PROP_SHADOWCOLOR, m_shadowColor);
			}
			else
			{
				shadowWidth = 0;
				material.SetFloat(PROP_SHADOWWIDTH, shadowWidth);
				material.SetColor(PROP_SHADOWCOLOR, alpha0);
			}

			material.SetFloat(PROP_SHADOWBLUR, m_shadowBlur);
			material.SetFloat(PROP_SHADOWPOWER, m_shadowPower);

			float outlineWidth = m_outlineWidth;

			if (m_outline)
			{
				material.SetFloat(PROP_OUTLINEWIDTH, outlineWidth);
				material.SetColor(PROP_OUTLINECOLOR, m_outlineColor);
			}
			else
			{
				outlineWidth = 0;
				material.SetFloat(PROP_OUTLINEWIDTH, outlineWidth);
				material.SetColor(PROP_OUTLINECOLOR, alpha0);
			}

			material.SetFloat(PROP_RADIUSE, m_radius);
			material.SetTexture(PROP_SDFTEX, m_sdfTexture);
		}
	}
}
