using UnityEngine;
using UnityEngine.UI;

namespace TLab.UI.SDF
{
	internal static class DestroyHelper
	{
		internal static void Destroy(Object @object)
		{
#if UNITY_EDITOR
			if (Application.isPlaying)
			{
				Object.Destroy(@object);
			}
			else
			{
				Object.DestroyImmediate(@object);
			}
#else
			Object.Destroy(@object);
#endif
		}
	}

	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(CanvasRenderer))]
	public class SDFUI : MaskableGraphic
	{
		public static readonly int PROP_HALFSIZE = Shader.PropertyToID("_HalfSize");
		public static readonly int PROP_PADDING = Shader.PropertyToID("_Padding");

		public static readonly int PROP_ONION = Shader.PropertyToID("_Onion");
		public static readonly int PROP_ONIONWIDTH = Shader.PropertyToID("_OnionWidth");

		public static readonly int PROP_MAINTEX = Shader.PropertyToID("_MainTex");

		public static readonly int PROP_SHADOWWIDTH = Shader.PropertyToID("_ShadowWidth");
		public static readonly int PROP_SHADOWBLUR = Shader.PropertyToID("_ShadowBlur");
		public static readonly int PROP_SHADOWPOWER = Shader.PropertyToID("_ShadowPower");
		public static readonly int PROP_SHADOWCOLOR = Shader.PropertyToID("_ShadowColor");

		public static readonly int PROP_OUTLINECOLOR = Shader.PropertyToID("_OutlineColor");
		public static readonly int PROP_OUTLINEWIDTH = Shader.PropertyToID("_OutlineWidth");

		[Header("Onion Option")]

		[SerializeField] protected bool m_onion = false;

		[SerializeField, Min(0f)] protected float m_onionWidth = 10;

		[Header("Shadow Option")]

		[SerializeField] protected bool m_shadow = false;

		[SerializeField, Min(0f)] protected float m_shadowWidth = 10;

		[SerializeField, Min(0f)] protected float m_shadowBlur = 0f;

		[SerializeField, Min(0f)] protected float m_shadowPower = 1f;

		[SerializeField] protected Color m_shadowColor = Color.black;

		[Header("Outline Option")]

		[SerializeField] protected bool m_outline = true;

		[SerializeField, Min(0f)] protected float m_outlineWidth = 10;

		[SerializeField] protected Color m_outlineColor = new Color(0.0f, 1.0f, 1.0f, 1.0f);

		[Header("Main")]

		[SerializeField] protected Texture m_mainTexture;

		[SerializeField] protected Vector2 m_mainTextureScale = Vector2.one;

		[SerializeField] protected Vector2 m_mainTextureOffset = Vector2.zero;

		[SerializeField] protected Color m_mainColor = Color.white;

		protected Mask m_mask;

		protected float m_extraMargin => Mathf.Max(m_outlineWidth, m_shadowWidth);

		public bool onion
		{
			get => m_onion;
			set
			{
				if (m_onion != value)
				{
					m_onion = value;

					Refresh();
				}
			}
		}

		public float onionWidth
		{
			get => m_onionWidth;
			set
			{
				if (m_onionWidth != value)
				{
					m_onionWidth = value;

					Refresh();
				}
			}
		}

		public bool shadow
		{
			get => m_shadow;
			set
			{
				if (m_shadow != value)
				{
					m_shadow = value;

					Refresh();
				}
			}
		}

		public float shadowWidth
		{
			get => m_shadowWidth;
			set
			{
				if (m_shadowWidth != value)
				{
					m_shadowWidth = value;

					Refresh();
				}
			}
		}

		public float shadowBlur
		{
			get => m_shadowBlur;
			set
			{
				if (m_shadowBlur != value)
				{
					m_shadowBlur = value;

					Refresh();
				}
			}
		}

		public float shadowPower
		{
			get => m_shadowPower;
			set
			{
				if (m_shadowPower != value)
				{
					m_shadowPower = value;

					Refresh();
				}
			}
		}

		public Color shadowColor
		{
			get => m_shadowColor;
			set
			{
				if (m_shadowColor != value)
				{
					m_shadowColor = value;

					Refresh();
				}
			}
		}

		public bool outline
		{
			get => m_outline;
			set
			{
				if (m_outline != value)
				{
					m_outline = value;

					Refresh();
				}
			}
		}

		public float outlineWidth
		{
			get => m_outlineWidth;
			set
			{
				if (m_outlineWidth != value)
				{
					m_outlineWidth = value;

					Refresh();
				}
			}
		}

		public Color outlineColor
		{
			get => m_outlineColor;
			set
			{
				if (m_outlineColor != value)
				{
					m_outlineColor = value;

					Refresh();
				}
			}
		}

		public new Texture mainTexture
		{
			get => (material != null) ? material.mainTexture : Texture2D.whiteTexture;
			set
			{
				m_mainTexture = value;

				if (material != null)
				{
					material.mainTexture = m_mainTexture;
				}
			}
		}

		public Vector2 mainTextureOffset
		{
			get => m_mainTextureOffset;
			set
			{
				m_mainTextureOffset = value;

				if (material != null)
				{
					material.mainTextureOffset = m_mainTextureOffset;
				}
			}
		}

		public Vector2 mainTextureScale
		{
			get => m_mainTextureScale;
			set
			{
				m_mainTextureScale = value;

				if (material != null)
				{
					material.mainTextureScale = m_mainTextureScale;
				}
			}
		}

		public Color mainColor
		{
			get => (material != null) ? material.color : Color.white;
			set
			{
				m_mainColor = value;

				if (material != null)
				{
					material.color = m_mainColor;
				}
			}
		}

		public override Material material
		{
			get
			{
				if (m_Material != null)
				{
					return m_Material;
				}

				return defaultMaterial;
			}

			set
			{
				base.material = value;
			}
		}

		protected readonly static Color alpha0 = new Color(0, 0, 0, 0);

		protected virtual void Validate(string shape)
		{
			if (material == null)
			{
				material = new Material(Shader.Find("UI/SDF/" + shape));
			}

			m_mask = GetComponent<Mask>();
		}

		protected virtual void OnUpdateDimentions()
		{
			if (enabled && material != null)
			{
				Refresh();

				if (m_mask != null)
				{
					var old = m_mask.enabled;

					m_mask.enabled = !old;

					m_mask.enabled = old;
				}
			}
		}

		public override void SetLayoutDirty()
		{
			base.SetLayoutDirty();

			OnUpdateDimentions();
		}

		protected override void OnRectTransformDimensionsChange()
		{
			base.OnRectTransformDimensionsChange();

			OnUpdateDimentions();
		}

		protected override void OnValidate()
		{
			base.OnValidate();
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			var other2 = GetComponent<SDFUI>();
			if (other2 != null && other2 != this)
			{
				DestroyHelper.Destroy(other2);
			}

			var image = GetComponent<Image>();
			if (image != null)
			{
				DestroyHelper.Destroy(image);
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			DestroyHelper.Destroy(material);
			material = null;
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();

			var width = rectTransform.rect.width;
			var height = rectTransform.rect.height;

			var pivot = new Vector3(rectTransform.pivot.x * width, rectTransform.pivot.y * height, 0);

			var vertex = UIVertex.simpleVert;
			vertex.color = color;

			vertex.position = new Vector3(-m_extraMargin, -m_extraMargin) - pivot;
			vertex.uv0 = new Vector2(0, 0);
			vh.AddVert(vertex);

			vertex.position = new Vector3(-m_extraMargin, height + m_extraMargin) - pivot;
			vertex.uv0 = new Vector2(0, 1);
			vh.AddVert(vertex);

			vertex.position = new Vector3(width + m_extraMargin, height + m_extraMargin) - pivot;
			vertex.uv0 = new Vector2(1, 1);
			vh.AddVert(vertex);

			vertex.position = new Vector3(width + m_extraMargin, -m_extraMargin) - pivot;
			vertex.uv0 = new Vector2(1, 0);
			vh.AddVert(vertex);

			vh.AddTriangle(0, 1, 2);
			vh.AddTriangle(2, 3, 0);
		}

		protected override void UpdateMaterial()
		{
			if (!IsActive())
				return;

			canvasRenderer.materialCount = 1;
			canvasRenderer.SetMaterial(materialForRendering, 0);
			canvasRenderer.SetTexture(mainTexture);
		}

		protected virtual void Refresh()
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

			material.SetFloat(PROP_PADDING, Mathf.Max(outlineWidth, shadowWidth));
		}
	}
}
