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
		protected virtual string OUTLINE_INSIDE => "";
		protected virtual string OUTLINE_OUTSIDE => "";

		public static readonly int PROP_HALFSIZE = Shader.PropertyToID("_HalfSize");
		public static readonly int PROP_PADDING = Shader.PropertyToID("_Padding");

		public static readonly int PROP_ONION = Shader.PropertyToID("_Onion");
		public static readonly int PROP_ONIONWIDTH = Shader.PropertyToID("_OnionWidth");

		public static readonly int PROP_MAINTEX = Shader.PropertyToID("_MainTex");

		public static readonly int PROP_SHADOWWIDTH = Shader.PropertyToID("_ShadowWidth");
		public static readonly int PROP_SHADOWBLUR = Shader.PropertyToID("_ShadowBlur");
		public static readonly int PROP_SHADOWPOWER = Shader.PropertyToID("_ShadowPower");
		public static readonly int PROP_SHADOWCOLOR = Shader.PropertyToID("_ShadowColor");
		public static readonly int PROP_SHADOWOFFSET = Shader.PropertyToID("_ShadowOffset");

		public static readonly int PROP_OUTLINECOLOR = Shader.PropertyToID("_OutlineColor");
		public static readonly int PROP_OUTLINEWIDTH = Shader.PropertyToID("_OutlineWidth");

		public enum OutlineType
		{
			INSIDE,
			OUTSIDE
		}

		[Header("Onion Option")]

		[SerializeField] protected bool m_onion = false;

		[SerializeField, Min(0f)] protected float m_onionWidth = 10;

		[Header("Shadow Option")]

		[SerializeField] protected bool m_shadow = false;

		[SerializeField, Min(0f)] protected float m_shadowWidth = 10;

		[SerializeField, Min(0f)] protected float m_shadowBlur = 0f;

		[SerializeField, Min(0f)] protected float m_shadowPower = 1f;

		[SerializeField] protected Vector2 m_shadowOffset;

		[SerializeField] protected Color m_shadowColor = Color.black;

		[Header("Outline Option")]

		[SerializeField] protected bool m_outline = true;

		[SerializeField, Min(0f)] protected float m_outlineWidth = 10;

		[SerializeField] protected Color m_outlineColor = new Color(0.0f, 1.0f, 1.0f, 1.0f);

		[SerializeField] protected OutlineType m_outlineType = OutlineType.INSIDE;

		[Header("Main")]

		[SerializeField] protected Sprite m_sprite;

		[SerializeField] protected Vector2 m_mainTextureScale = Vector2.one;

		[SerializeField] protected Vector2 m_mainTextureOffset = Vector2.zero;

		[SerializeField] protected Color m_mainColor = Color.white;

		protected Material m_material;

		protected Material m_materialOutlineInside;

		protected Material m_materialOutlineOutside;

		protected Sprite m_overrideSprite;

		protected Mask m_mask;

		protected float m_extraMargin
		{
			get
			{
				switch (m_outlineType)
				{
					case OutlineType.INSIDE:
						return m_shadow ? m_shadowWidth : 0;
					case OutlineType.OUTSIDE:
						return Mathf.Max(m_outline ? m_outlineWidth : 0, m_shadow ? m_shadowWidth : 0);
				}

				return 0;
			}
		}

		public bool onion
		{
			get => m_onion;
			set
			{
				if (m_onion != value)
				{
					m_onion = value;

					SetAllDirty();
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

					SetAllDirty();
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

					SetAllDirty();
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

					SetAllDirty();
				}
			}
		}

		public Vector2 shadowOffset
		{
			get => m_shadowOffset;
			set
			{
				if (m_shadowOffset != value)
				{
					m_shadowOffset = value;

					SetAllDirty();
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

					SetAllDirty();
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

					SetAllDirty();
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

					SetAllDirty();
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

					SetAllDirty();
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

					SetAllDirty();
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

					SetAllDirty();
				}
			}
		}

		public OutlineType outlineType
		{
			get => m_outlineType;
			set
			{
				if (m_outlineType != value)
				{
					m_outlineType = value;

					CreateMaterial();

					SetAllDirty();
				}
			}
		}

		public Sprite sprite
		{
			get => m_sprite;
			set
			{
				if (m_sprite != null)
				{
					if (m_sprite != value)
					{
						m_SkipLayoutUpdate = m_sprite.rect.size.Equals(value ? value.rect.size : Vector2.zero);
						m_SkipMaterialUpdate = m_sprite.texture == (value ? value.texture : null);
						m_sprite = value;

						SetAllDirty();
					}
				}
				else if (value != null)
				{
					m_SkipLayoutUpdate = value.rect.size == Vector2.zero;
					m_SkipMaterialUpdate = value.texture == null;
					m_sprite = value;

					SetAllDirty();
				}
			}
		}

		public Sprite overrideSprite
		{
			get => activeSprite;
			set
			{
				if (m_overrideSprite != value)
				{
					m_overrideSprite = value;

					SetAllDirty();
				}
			}
		}

		private Sprite activeSprite
		{
			get
			{
				return m_overrideSprite != null ? m_overrideSprite : sprite;
			}
		}

		public override Texture mainTexture
		{
			get
			{
				if (activeSprite == null)
				{
					if (material != null && material.mainTexture != null)
					{
						return material.mainTexture;
					}
					return s_WhiteTexture;
				}

				return activeSprite.texture;
			}
		}

		public Vector2 mainTextureOffset
		{
			get => m_mainTextureOffset;
			set
			{
				if (m_mainTextureOffset != value)
				{
					m_mainTextureOffset = value;

					SetAllDirty();
				}
			}
		}

		public Vector2 mainTextureScale
		{
			get => m_mainTextureScale;
			set
			{
				if (m_mainTextureScale != value)
				{
					m_mainTextureScale = value;

					SetAllDirty();
				}
			}
		}

		public Color mainColor
		{
			get => (material != null) ? material.color : Color.white;
			set
			{
				if (m_mainColor != value)
				{
					m_mainColor = value;

					SetAllDirty();
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

		protected virtual void CreateMaterial()
		{
			switch (m_outlineType)
			{
				case OutlineType.INSIDE:
					if (m_materialOutlineInside == null)
					{
						m_materialOutlineInside = new Material(Shader.Find(OUTLINE_INSIDE));
					}
					m_material = m_materialOutlineInside;
					break;
				case OutlineType.OUTSIDE:
					if (m_materialOutlineOutside == null)
					{
						m_materialOutlineOutside = new Material(Shader.Find(OUTLINE_OUTSIDE));
					}
					m_material = m_materialOutlineOutside;
					break;
			}

			material = m_material;
		}

		/// <summary>
		/// This function must be called before calling the set material dirty function.
		/// </summary>
		protected virtual void Validate()
		{
			CreateMaterial();

			m_mask = GetComponent<Mask>();
		}

#if UNITY_EDITOR
		protected override void OnValidate()
		{
			Validate();

			base.OnValidate();
		}
#endif

		protected override void OnEnable()
		{
			DeleteOldMat();

			Validate();

			base.OnEnable();
		}

		protected virtual void OnUpdateDimentions()
		{
			if (enabled && m_material != null)
			{
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

		protected virtual void DeleteOldMat()
		{
			var others = GetComponent<SDFUI>();
			if (others != null && others != this)
			{
				DestroyHelper.Destroy(others);
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			if (m_materialOutlineInside)
			{
				DestroyHelper.Destroy(m_materialOutlineInside);
				m_materialOutlineInside = null;
			}

			if (m_materialOutlineOutside)
			{
				DestroyHelper.Destroy(m_materialOutlineOutside);
				m_materialOutlineOutside = null;
			}

			m_material = null;
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();

			var width = rectTransform.rect.width;
			var height = rectTransform.rect.height;

			var pivot = new Vector3(rectTransform.pivot.x * width, rectTransform.pivot.y * height, 0);

			var extraMargin = m_extraMargin;

			var shadowExpand = Vector4.zero;

			if (m_shadowOffset.x < 0)
			{
				shadowExpand.x = m_shadowOffset.x;
				shadowExpand.y = 0;
			}
			else
			{
				shadowExpand.y = m_shadowOffset.x;
				shadowExpand.x = 0;
			}

			if (m_shadowOffset.y < 0)
			{
				shadowExpand.z = m_shadowOffset.y;
				shadowExpand.w = 0;
			}
			else
			{
				shadowExpand.w = m_shadowOffset.y;
				shadowExpand.z = 0;
			}

			var uvScale = new Vector2(width / (2 * extraMargin + width), height / (2 * extraMargin + height));
			var uvExpand = new Vector4(shadowExpand.x / width, shadowExpand.y / width, shadowExpand.z / height, shadowExpand.w / height);
			uvExpand.x = uvExpand.x * uvScale.x;
			uvExpand.y = uvExpand.y * uvScale.x;
			uvExpand.z = uvExpand.z * uvScale.y;
			uvExpand.w = uvExpand.w * uvScale.y;

			var vertex = UIVertex.simpleVert;
			vertex.color = color;

			vertex.position = new Vector3(-extraMargin + shadowExpand.x, -extraMargin + shadowExpand.z) - pivot;
			vertex.uv0 = new Vector2(uvExpand.x, uvExpand.z);
			vh.AddVert(vertex);

			vertex.position = new Vector3(-extraMargin + shadowExpand.x, extraMargin + shadowExpand.w + height) - pivot;
			vertex.uv0 = new Vector2(uvExpand.x, 1 + uvExpand.w);
			vh.AddVert(vertex);

			vertex.position = new Vector3(extraMargin + shadowExpand.y + width, extraMargin + shadowExpand.w + height) - pivot;
			vertex.uv0 = new Vector2(1 + uvExpand.y, 1 + uvExpand.w);
			vh.AddVert(vertex);

			vertex.position = new Vector3(extraMargin + shadowExpand.y + width, -extraMargin + shadowExpand.z) - pivot;
			vertex.uv0 = new Vector2(1 + uvExpand.y, uvExpand.z);
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
			canvasRenderer.SetTexture((activeSprite == null) ? s_WhiteTexture : activeSprite.texture);
		}

		public override void SetMaterialDirty()
		{
			base.SetMaterialDirty();

			m_material.SetVector(PROP_HALFSIZE, ((RectTransform)transform).rect.size * .5f);

			m_material.mainTexture = (activeSprite == null) ? s_WhiteTexture : activeSprite.texture;
			m_material.mainTextureScale = m_mainTextureScale;
			m_material.mainTextureOffset = m_mainTextureOffset;
			m_material.color = m_mainColor;

			if (m_onion)
			{
				m_material.SetInt(PROP_ONION, 1);
				m_material.SetFloat(PROP_ONIONWIDTH, m_onionWidth);
			}
			else
			{
				m_material.SetInt(PROP_ONION, 0);
				m_material.SetFloat(PROP_ONIONWIDTH, 0);
			}

			float shadowWidth = m_shadowWidth;

			if (m_shadow)
			{
				m_material.SetFloat(PROP_SHADOWWIDTH, shadowWidth);
				m_material.SetColor(PROP_SHADOWCOLOR, m_shadowColor);
			}
			else
			{
				shadowWidth = 0;
				m_material.SetFloat(PROP_SHADOWWIDTH, shadowWidth);
				m_material.SetColor(PROP_SHADOWCOLOR, alpha0);
			}

			m_material.SetFloat(PROP_SHADOWBLUR, m_shadowBlur);
			m_material.SetFloat(PROP_SHADOWPOWER, m_shadowPower);
			m_material.SetVector(PROP_SHADOWOFFSET, new Vector4(m_shadowOffset.x / rectTransform.rect.width, m_shadowOffset.y / rectTransform.rect.height, m_shadowOffset.x, m_shadowOffset.y));

			float outlineWidth = m_outlineWidth;

			if (m_outline)
			{
				m_material.SetFloat(PROP_OUTLINEWIDTH, outlineWidth);
				m_material.SetColor(PROP_OUTLINECOLOR, m_outlineColor);
			}
			else
			{
				outlineWidth = 0;
				m_material.SetFloat(PROP_OUTLINEWIDTH, outlineWidth);
				m_material.SetColor(PROP_OUTLINECOLOR, alpha0);
			}

			m_material.SetFloat(PROP_PADDING, m_extraMargin);
		}
	}
}
