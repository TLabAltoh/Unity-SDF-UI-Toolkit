using TLab.UI.SDF.Registry;
using TLab.UI.SDF.Editor;
using Unity.Mathematics;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Sprites;

namespace TLab.UI.SDF
{
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(CanvasRenderer))]
	public abstract class SDFUI : MaskableGraphic
	{
		protected virtual string SHADER_NAME => "";

		#region SHADER_KEYWORD

		internal const string SHADER_KEYWORD_PREFIX = "SDF_UI_";

		internal const string KEYWORD_AA = SHADER_KEYWORD_PREFIX + "AA";

		internal const string KEYWORD_SHADOW = SHADER_KEYWORD_PREFIX + "SHADOW";

		internal const string KEYWORD_OUTLINE_PATTERN_TEX = SHADER_KEYWORD_PREFIX + "OUTLINE_PATTERN_TEX";
		internal const string KEYWORD_GRAPHIC_PATTERN_TEX = SHADER_KEYWORD_PREFIX + "GRAPHIC_PATTERN_TEX";

		internal const string KEYWORD_OUTLINE_PATTERN_SHINY = SHADER_KEYWORD_PREFIX + "OUTLINE_PATTERN_SHINY";
		internal const string KEYWORD_GRAPHIC_PATTERN_SHINY = SHADER_KEYWORD_PREFIX + "GRAPHIC_PATTERN_SHINY";

		#endregion SHADER_KEYWORD

		#region SHADER_PROP

		internal static readonly int PROP_HALFSIZE = Shader.PropertyToID("_HalfSize");
		internal static readonly int PROP_PADDING = Shader.PropertyToID("_Padding");
		internal static readonly int PROP_OUTERUV = Shader.PropertyToID("_OuterUV");
		internal static readonly int PROP_RECTSIZE = Shader.PropertyToID("_RectSize");

		internal static readonly int PROP_ONION = Shader.PropertyToID("_Onion");
		internal static readonly int PROP_ONION_WIDTH = Shader.PropertyToID("_OnionWidth");

		internal static readonly int PROP_MAINTEX = Shader.PropertyToID("_MainTex");

		internal static readonly int PROP_SHADOW_BORDER = Shader.PropertyToID("_ShadowBorder");
		internal static readonly int PROP_OUTLINE_BORDER = Shader.PropertyToID("_OutlineBorder");
		internal static readonly int PROP_GRAPHIC_BORDER = Shader.PropertyToID("_GraphicBorder");

		internal static readonly int PROP_SHADOW_WIDTH = Shader.PropertyToID("_ShadowWidth");
		internal static readonly int PROP_SHADOW_BLUR = Shader.PropertyToID("_ShadowBlur");
		internal static readonly int PROP_SHADOW_DILATE = Shader.PropertyToID("_ShadowDilate");
		internal static readonly int PROP_SHADOW_COLOR = Shader.PropertyToID("_ShadowColor");
		internal static readonly int PROP_SHADOW_OFFSET = Shader.PropertyToID("_ShadowOffset");
		internal static readonly int PROP_SHADOW_GAUSSIAN = Shader.PropertyToID("_ShadowGaussian");

		internal static readonly int PROP_OUTLINE_TYPE = Shader.PropertyToID("_OutlineType");
		internal static readonly int PROP_OUTLINE_COLOR = Shader.PropertyToID("_OutlineColor");
		internal static readonly int PROP_OUTLINE_WIDTH = Shader.PropertyToID("_OutlineWidth");
		internal static readonly int PROP_OUTLINE_INNER_BLUR = Shader.PropertyToID("_OutlineInnerBlur");
		internal static readonly int PROP_OUTLINE_INNER_GAUSSIAN = Shader.PropertyToID("_OutlineInnerGaussian");

		internal static readonly int PROP_GRAPHIC_SHINY_WIDTH = Shader.PropertyToID("_GraphicShinyWidth");
		internal static readonly int PROP_GRAPHIC_SHINY_ANGLE = Shader.PropertyToID("_GraphicShinyAngle");
		internal static readonly int PROP_GRAPHIC_SHINY_BLUR = Shader.PropertyToID("_GraphicShinyBlur");

		internal static readonly int PROP_OUTLINE_SHINY_WIDTH = Shader.PropertyToID("_OutlineShinyWidth");
		internal static readonly int PROP_OUTLINE_SHINY_ANGLE = Shader.PropertyToID("_OutlineShinyAngle");
		internal static readonly int PROP_OUTLINE_SHINY_BLUR = Shader.PropertyToID("_OutlineShinyBlur");

		internal static readonly int PROP_OUTLINE_PATTERN_TEXTURE = Shader.PropertyToID("_OutlinePatternTexture");
		internal static readonly int PROP_OUTLINE_PATTERN_TEXTURE_ROW = Shader.PropertyToID("_OutlinePatternTextureRow");
		internal static readonly int PROP_OUTLINE_PATTERN_TEXTURE_SCALE = Shader.PropertyToID("_OutlinePatternTextureScale");

		internal static readonly int PROP_OUTLINE_PATTERN_COLOR = Shader.PropertyToID("_OutlinePatternColor");
		internal static readonly int PROP_GRAPHIC_PATTERN_COLOR = Shader.PropertyToID("_GraphicPatternColor");
		internal static readonly int PROP_OUTLINE_PATTERN_OFFSET = Shader.PropertyToID("_OutlinePatternOffset");
		internal static readonly int PROP_GRAPHIC_PATTERN_OFFSET = Shader.PropertyToID("_GraphicPatternOffset");

		#endregion SHADER_PROP

		public enum OutlineType
		{
			Inside = 0,
			Outside = 1,
		};

		public enum AntialiasingType
		{
			Default = -1,
			OFF = 0,
			ON = 1,
		}

		public enum ActiveImageType
		{
			Sprite = 0,
			Texture = 1,
		};

		public enum PatternType
		{
			None = 0,
			Shiny = 1,
			Texture = 2,
		};

		[SerializeField, LeftToggle] protected bool m_onion = false;
		[SerializeField, Min(0f)] protected float m_onionWidth = 10;

		[SerializeField] protected AntialiasingType m_antialiasing = AntialiasingType.Default;

		[SerializeField, LeftToggle] protected bool m_outline = true;
		[SerializeField, Min(0f)] protected float m_outlineWidth = 10;
		[SerializeField, Min(0f)] protected float m_outlineInnerSoftWidth = 0;
		[SerializeField, Range(0, 1)] protected float m_outlineInnerSoftness = 0.0f;
		[SerializeField, ColorUsage(true, true)] protected Color m_outlineColor = Color.cyan;
		[SerializeField] protected OutlineType m_outlineType = OutlineType.Inside;

		[SerializeField, LeftToggle] protected bool m_shadow = false;
		[SerializeField, Min(0f)] protected float m_shadowWidth = 10;
		[SerializeField, Min(0f)] protected float m_shadowInnerSoftWidth = 0;
		[SerializeField, Range(0, 1)] protected float m_shadowSoftness = 0.0f;
		[SerializeField, Min(0f)] protected float m_shadowDilate = 0;
		[SerializeField] protected Vector2 m_shadowOffset;
		[SerializeField, ColorUsage(true, true)] protected Color m_shadowColor = Color.black;

		[SerializeField] protected PatternType m_graphicPatternType = PatternType.None;
		[SerializeField] protected Color m_graphicPatternColor = Color.white;
		[SerializeField, Range(0, 1)] protected float m_graphicShinyWidth = 0.25f;
		[SerializeField, Range(0, 1)] protected float m_graphicShinyAngle = 0.0f;
		[SerializeField, Range(0, 1)] protected float m_graphicShinyBlur = 0.0f;
		[SerializeField] protected Vector2 m_graphicPatternOffset;

		[SerializeField] protected PatternType m_outlinePatternType = PatternType.None;
		[SerializeField, ColorUsage(true, true)] protected Color m_outlinePatternColor = Color.white;
		[SerializeField, Range(0, 1)] protected float m_outlineShinyWidth = 0.25f;
		[SerializeField, Range(0, 1)] protected float m_outlineShinyAngle = 0.0f;
		[SerializeField, Range(0, 1)] protected float m_outlineShinyBlur = 0.0f;
		[SerializeField] protected Texture m_outlinePatternTexture;
		[SerializeField] protected Vector2 m_outlinePatternTextureScale;
		[SerializeField, Min(0)] protected int m_outlinePatternTextureRow = 5;
		[SerializeField] protected Vector2 m_outlinePatternOffset;

		[SerializeField, ColorUsage(true)] protected Color m_fillColor = Color.white;

		[SerializeField] protected ActiveImageType m_activeImageType;
		[SerializeField] protected Sprite m_sprite;
		[SerializeField] protected Texture m_texture;
		[SerializeField] protected Rect m_uvRect = new Rect(0f, 0f, 1f, 1f);

		protected SDFUI()
		{
			useLegacyMeshGeneration = false;
		}

		protected Sprite m_overrideSprite;
		protected Texture m_overrideTexture;

		protected Mask m_mask;

		protected readonly static Vector4 defaultOuterUV = new Vector4(0, 0, 1, 1);

		protected readonly static Color alpha0 = new Color(0, 0, 0, 0);

		protected float eulerZ = float.NaN;

		protected virtual float m_extraMargin
		{
			get
			{
				switch (m_outlineType)
				{
					case OutlineType.Inside:
						return (m_shadow ? m_shadowWidth : 0);
					case OutlineType.Outside:
						return (m_shadow ? m_shadowWidth : 0) + (m_outline ? m_outlineWidth : 0);
				}

				return 0;
			}
		}

		public float minSize => Mathf.Min(rectTransform.rect.size.x, rectTransform.rect.size.y);

		public float maxSize => Mathf.Max(rectTransform.rect.size.x, rectTransform.rect.size.y);

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

		public AntialiasingType antialiasing
		{
			get => m_antialiasing;
			set
			{
				if (m_antialiasing != value)
				{
					m_antialiasing = value;

					SetAllDirty();
				}
			}
		}

		#region SHADOW
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

		public float shadowInnerSoftWidth
		{
			get => m_shadowInnerSoftWidth;
			set
			{
				if (m_shadowInnerSoftWidth != value)
				{
					m_shadowInnerSoftWidth = value;

					SetAllDirty();
				}
			}
		}

		public float shadowSoftness
		{
			get => m_shadowSoftness;
			set
			{
				if (m_shadowSoftness != value)
				{
					m_shadowSoftness = value;

					SetAllDirty();
				}
			}
		}

		public float shadowDilate
		{
			get => m_shadowDilate;
			set
			{
				if (m_shadowDilate != value)
				{
					m_shadowDilate = value;

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

		#endregion SHADOW

		#region OUTLINE

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

		public float outlineInnerSoftWidth
		{
			get => m_outlineInnerSoftWidth;
			set
			{
				if (m_outlineInnerSoftWidth != value)
				{
					m_outlineInnerSoftWidth = value;

					SetAllDirty();
				}
			}
		}

		public float outlineInnerSoftness
		{
			get => m_outlineInnerSoftness;
			set
			{
				if (m_outlineInnerSoftness != value)
				{
					m_outlineInnerSoftness = value;

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

					SetAllDirty();
				}
			}
		}

		#endregion

		#region GRAPHIC_PATTERN

		public PatternType graphicPatternType
		{
			get => m_graphicPatternType;
			set
			{
				if (m_graphicPatternType != value)
				{
					m_graphicPatternType = value;

					SetAllDirty();
				}
			}
		}

		public Color graphicPatternColor
		{
			get => m_graphicPatternColor;
			set
			{
				if (m_graphicPatternColor != value)
				{
					m_graphicPatternColor = value;

					SetAllDirty();
				}
			}
		}

		public Vector2 graphicPatternOffset
		{
			get => m_graphicPatternOffset;
			set
			{
				if (m_graphicPatternOffset != value)
				{
					m_graphicPatternOffset = value;

					SetAllDirty();
				}
			}

		}

		public float graphicShinyWidth
		{
			get => m_graphicShinyWidth;
			set
			{
				if (m_graphicShinyWidth != value)
				{
					m_graphicShinyWidth = value;

					SetAllDirty();
				}
			}
		}

		public float graphicShinyAngle
		{
			get => m_graphicShinyAngle;
			set
			{
				if (m_graphicShinyAngle != value)
				{
					m_graphicShinyAngle = value;

					SetAllDirty();
				}
			}
		}

		public float graphicShinyBlur
		{
			get => m_graphicShinyBlur;
			set
			{
				if (m_graphicShinyBlur != value)
				{
					m_graphicShinyBlur = value;

					SetAllDirty();
				}
			}
		}

		#endregion GRAPHIC_PATTERN

		#region OUTILNE_PATTERN

		public PatternType outlinePatternType
		{
			get => m_outlinePatternType;
			set
			{
				if (m_outlinePatternType != value)
				{
					m_outlinePatternType = value;

					SetAllDirty();
				}
			}
		}

		public Color outlinePatternColor
		{
			get => m_outlinePatternColor;
			set
			{
				if (m_outlinePatternColor != value)
				{
					m_outlinePatternColor = value;

					SetAllDirty();
				}
			}
		}

		public Vector2 outlinePatternOffset
		{
			get => m_outlinePatternOffset;
			set
			{
				if (m_outlinePatternOffset != value)
				{
					m_outlinePatternOffset = value;

					SetAllDirty();
				}
			}

		}

		public float outlineShinyWidth
		{
			get => m_outlineShinyWidth;
			set
			{
				if (m_outlineShinyWidth != value)
				{
					m_outlineShinyWidth = value;

					SetAllDirty();
				}
			}
		}

		public float outlineShinyAngle
		{
			get => m_outlineShinyAngle;
			set
			{
				if (m_outlineShinyAngle != value)
				{
					m_outlineShinyAngle = value;

					SetAllDirty();
				}
			}
		}

		public float outlineShinyBlur
		{
			get => m_outlineShinyBlur;
			set
			{
				if (m_outlineShinyBlur != value)
				{
					m_outlineShinyBlur = value;

					SetAllDirty();
				}
			}
		}

		public Texture outlinePatternTexture
		{
			get => m_outlinePatternTexture;
			set
			{
				if (m_outlinePatternTexture != value)
				{
					m_outlinePatternTexture = value;

					SetAllDirty();
				}
			}
		}

		public Vector2 outlinePatternTextureScale
		{
			get => m_outlinePatternTextureScale;
			set
			{
				var tmp = value;
				tmp.x = Mathf.Clamp(tmp.x, 0, 2);
				tmp.y = Mathf.Clamp(tmp.y, 0, 2);

				if (m_outlinePatternTextureScale != tmp)
				{
					m_outlinePatternTextureScale = tmp;

					SetAllDirty();
				}
			}
		}

		public int outlinePatternTextureRow
		{
			get => m_outlinePatternTextureRow;
			set
			{
				var tmp = Mathf.Max(0, value);

				if (m_outlinePatternTextureRow != tmp)
				{
					m_outlinePatternTextureRow = tmp;

					SetAllDirty();
				}
			}
		}

		#endregion OUTILNE_PATTERN

		public virtual Color fillColor
		{
			get => m_fillColor;
			set
			{
				if (m_fillColor != value)
				{
					m_fillColor = value;

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

		public override Material materialForRendering
		{
			get
			{
				var currentMat = base.materialForRendering;
				if (currentMat != material && currentMat.shader.name.StartsWith("Hidden/UI/SDF/"))
					_materialRecord.Populate(currentMat);
				return currentMat;
			}
		}

		public ActiveImageType activeImageType
		{
			get => m_activeImageType;
			set
			{
				if (m_activeImageType != value)
				{
					m_activeImageType = value;

					SetVerticesDirty();
					SetMaterialDirty();
				}
			}
		}

		public Sprite overrideSprite
		{
			get => overrideSprite;
			set
			{
				if (m_overrideSprite != value)
				{
					m_overrideSprite = value;

					SetAllDirty();
				}
			}
		}

		public Texture overrideTexture
		{
			get => activeTexture;
			set
			{
				if (m_overrideTexture != value)
				{
					m_overrideTexture = value;

					SetAllDirty();
				}
			}
		}

		public Sprite activeSprite
		{
			get
			{
				return m_overrideSprite != null ? m_overrideSprite : sprite;
			}
		}

		public Texture activeTexture
		{
			get
			{
				return m_overrideTexture != null ? m_overrideTexture : texture;
			}
		}

		public override Texture mainTexture
		{
			get
			{
				switch (m_activeImageType)
				{
					case ActiveImageType.Sprite:
						{
							if (m_sprite == null)
							{
								if (_materialRecord != null && _materialRecord.Texture != null)
								{
									return _materialRecord.Texture;
								}
								return s_WhiteTexture;
							}
							return m_sprite.texture;
						}
					default: // ActiveImageType.Texture
						{
							if (m_texture == null)
							{
								if (_materialRecord != null && _materialRecord.Texture != null)
								{
									return _materialRecord.Texture;
								}
								return s_WhiteTexture;
							}
							return m_texture;
						}
				}

			}
		}

		internal MaterialRecord MaterialRecord => (MaterialRecord)_materialRecord.Clone();
		private protected MaterialRecord _materialRecord { get; } = new();

		protected bool materialDirty;

		public Sprite sprite
		{
			get => m_sprite;
			set
			{
				if (m_sprite != value)
				{
					m_sprite = value;
					SetVerticesDirty();
					SetMaterialDirty();
				}
			}
		}

		public Texture texture
		{
			get => m_texture;
			set
			{
				if (m_texture != value)
				{
					m_texture = value;
					SetVerticesDirty();
					SetMaterialDirty();
				}
			}
		}

		public Rect uvRect
		{
			get => m_uvRect;
			set
			{
				if (m_uvRect != value)
				{
					m_uvRect = value;
					SetVerticesDirty();
				}
			}
		}

#if UNITY_EDITOR
		protected static TSDFUI Create<TSDFUI>(MenuCommand menuCommand) where TSDFUI : SDFUI
		{
			GameObject gameObject = new();
			gameObject.name = typeof(TSDFUI).Name;
			TSDFUI sdfUI = gameObject.AddComponent<TSDFUI>();
			GameObjectUtility.SetParentAndAlign(gameObject, menuCommand.context as GameObject);
			Undo.RegisterCreatedObjectUndo(gameObject, "Create " + gameObject.name);
			Selection.activeObject = gameObject;
			return sdfUI;
		}
#endif

		public override void SetNativeSize()
		{
			Texture tex = mainTexture;
			if (tex != null)
			{
				int w = Mathf.RoundToInt(tex.width * uvRect.width);
				int h = Mathf.RoundToInt(tex.height * uvRect.height);
				rectTransform.anchorMax = rectTransform.anchorMin;
				rectTransform.sizeDelta = new Vector2(w, h);
			}
		}

		protected override void OnDidApplyAnimationProperties()
		{
			SetMaterialDirty();
			SetVerticesDirty();
			SetRaycastDirty();
		}

		/// <summary>
		/// This function must be called before calling the set material dirty function.
		/// </summary>
		protected virtual void Validate()
		{
			var canvasRenderer = GetComponent<CanvasRenderer>();
			canvasRenderer.cullTransparentMesh = false;

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
			Validate();

#if UNITY_EDITOR
			if (EditorApplication.isPlaying)
#endif
				SDFUIGraphicsRegistry.AddToRegistry(this);

			materialDirty = true;

			base.OnEnable();
		}

		protected override void OnDisable()
		{
#if UNITY_EDITOR
			if (EditorApplication.isPlaying)
#endif
				SDFUIGraphicsRegistry.RemoveFromRegistry(this);

			MaterialRegistry.StopUsingMaterial(this);

			base.OnDisable();
		}

#if UNITY_EDITOR
		private void LateUpdate()
		{
			if (!EditorApplication.isPlaying)
				OnLateUpdate();
		}

		protected override void Reset()
		{
			m_antialiasing = AntialiasingType.Default;
			m_outline = SDFUISettings.Instance.UseOutline;
			m_outlineWidth = SDFUISettings.Instance.OutlineWidth;
			m_outlineColor = SDFUISettings.Instance.OutlineColor;
			m_outlineType = SDFUISettings.Instance.OutlineType;
			m_fillColor = SDFUISettings.Instance.FillColor;
			m_shadow = SDFUISettings.Instance.UseShadow;
			m_shadowColor = SDFUISettings.Instance.ShadowColor;
			m_shadowOffset = SDFUISettings.Instance.ShadowOffset;
			base.Reset();
		}
#endif

		public virtual bool MaskEnabled()
		{
			return (m_mask != null && m_mask.MaskEnabled());
		}

		internal virtual void OnLateUpdate()
		{
			if (shadow && !Mathf.Approximately(eulerZ, rectTransform.eulerAngles.z))
			{
				eulerZ = rectTransform.eulerAngles.z;

				OnUpdateDimensions();
			}

			if (materialDirty)
			{
				materialDirty = false;
				MaterialRegistry.UpdateMaterial(this);
			}
		}

		protected virtual void OnUpdateDimensions()
		{
			if (enabled)
			{
				SetVerticesDirty();
				SetMaterialDirty();
			}
		}

		public override void SetLayoutDirty()
		{
			base.SetLayoutDirty();

			OnUpdateDimensions();
		}

		protected override void OnRectTransformDimensionsChange()
		{
			base.OnRectTransformDimensionsChange();

			OnUpdateDimensions();
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();

			float2 shadowOffset = shadow ? m_shadowOffset : float2.zero;

			AntialiasingType antialiasing = m_antialiasing is AntialiasingType.Default ? SDFUISettings.Instance.DefaultAA : m_antialiasing;

			MeshUtils.CalculateVertexes(rectTransform.rect.size, rectTransform.pivot, m_extraMargin, shadowOffset, rectTransform.eulerAngles.z, antialiasing,
				out var vertex0, out var vertex1, out var vertex2, out var vertex3);

			var color32 = color;
			vh.AddVert(vertex0.position, color32, new Vector4(vertex0.uv.x, vertex0.uv.y));
			vh.AddVert(vertex1.position, color32, new Vector4(vertex1.uv.x, vertex1.uv.y));
			vh.AddVert(vertex2.position, color32, new Vector4(vertex2.uv.x, vertex2.uv.y));
			vh.AddVert(vertex3.position, color32, new Vector4(vertex3.uv.x, vertex3.uv.y));

			vh.AddTriangle(0, 1, 2);
			vh.AddTriangle(2, 3, 0);
		}

		public virtual Color GetAlpha0(Color color)
		{
			return new Color(color.r, color.g, color.b, 0.0f);
		}

		protected virtual void UpdateMaterialRecord()
		{
			var minSize = this.minSize;
			var maxSize = this.maxSize;

			var hminSize = minSize * .5f;
			var hmaxSize = maxSize * .5f;

			_materialRecord.ShaderName = SHADER_NAME;

			_materialRecord.SetVector(PROP_RECTSIZE, new float4(((RectTransform)transform).rect.size, 0, 0));

			_materialRecord.TextureUV = new float4(uvRect.x, uvRect.y, uvRect.size.x, uvRect.size.y);
			_materialRecord.TextureColor = m_fillColor;

			var activeImageType = m_activeImageType;
			switch (activeImageType)
			{
				case ActiveImageType.Sprite:
					{
						var activeSprite = this.activeSprite;
						if (activeSprite == null)
						{
							_materialRecord.Texture = s_WhiteTexture;
							_materialRecord.SetVector(PROP_OUTERUV, defaultOuterUV);
						}
						else
						{
							_materialRecord.Texture = activeSprite.texture;
							_materialRecord.SetVector(PROP_OUTERUV, DataUtility.GetOuterUV(activeSprite));
						}
					}
					break;
				case ActiveImageType.Texture:
					{
						var activeTexture = this.activeTexture;
						_materialRecord.Texture = (activeTexture == null) ? s_WhiteTexture : activeTexture;
						_materialRecord.SetVector(PROP_OUTERUV, defaultOuterUV);
					}
					break;
			}

			if (m_onion)
			{
				_materialRecord.SetFloat(PROP_ONION, 1);
				_materialRecord.SetFloat(PROP_ONION_WIDTH, m_onionWidth);
			}
			else
			{
				_materialRecord.SetFloat(PROP_ONION, 0);
				_materialRecord.SetFloat(PROP_ONION_WIDTH, 0);
			}

			if (m_shadow)
			{
				_materialRecord.SetFloat(PROP_SHADOW_WIDTH, m_shadowWidth);
				_materialRecord.SetFloat(PROP_SHADOW_BLUR, m_shadowSoftness * (m_shadowWidth + m_shadowInnerSoftWidth));
				_materialRecord.SetFloat(PROP_SHADOW_DILATE, m_shadowDilate);
				_materialRecord.SetColor(PROP_SHADOW_COLOR, m_shadowColor);
				_materialRecord.SetFloat(PROP_SHADOW_GAUSSIAN, (m_shadowSoftness > 0) ? 1 : 0);

				MeshUtils.ShadowSizeOffset(rectTransform.rect.size, m_shadowOffset, rectTransform.eulerAngles.z, out float4 sizeOffset);
				_materialRecord.SetVector(PROP_SHADOW_OFFSET, sizeOffset);

				_materialRecord.EnableKeyword(KEYWORD_SHADOW);
			}
			else
			{
				_materialRecord.DisableKeyword(KEYWORD_SHADOW);
			}

			{
				var patternType = m_graphicPatternType;
				switch (patternType)
				{
					case PatternType.None:
						_materialRecord.DisableKeyword(KEYWORD_GRAPHIC_PATTERN_SHINY, KEYWORD_GRAPHIC_PATTERN_TEX);
						break;
					case PatternType.Shiny:
						_materialRecord.EnableKeyword(KEYWORD_GRAPHIC_PATTERN_SHINY);

						_materialRecord.SetFloat(PROP_GRAPHIC_SHINY_ANGLE, Mathf.PI * m_graphicShinyAngle);
						_materialRecord.SetFloat(PROP_GRAPHIC_SHINY_WIDTH, Mathf.PI * m_graphicShinyWidth);
						_materialRecord.SetFloat(PROP_GRAPHIC_SHINY_BLUR, hminSize * m_graphicShinyBlur);

						_materialRecord.SetColor(PROP_GRAPHIC_PATTERN_COLOR, m_graphicPatternColor);
						_materialRecord.SetVector(PROP_GRAPHIC_PATTERN_OFFSET, m_graphicPatternOffset);
						break;
					case PatternType.Texture:
						_materialRecord.EnableKeyword(KEYWORD_GRAPHIC_PATTERN_TEX);
						break;
				}
			}

			if (m_outline && m_outlineWidth > 0)
			{
				_materialRecord.SetFloat(PROP_OUTLINE_WIDTH, m_outlineWidth);
				_materialRecord.SetColor(PROP_OUTLINE_COLOR, m_outlineColor);
				_materialRecord.SetFloat(PROP_OUTLINE_INNER_BLUR, m_outlineInnerSoftness * m_outlineInnerSoftWidth);
				_materialRecord.SetFloat(PROP_OUTLINE_INNER_GAUSSIAN, (m_outlineInnerSoftness > 0) && (m_outlineInnerSoftWidth > 0) ? 1 : 0);

				var outlineType = m_outlineType;
				switch (outlineType)
				{
					case OutlineType.Inside:
						_materialRecord.SetFloat(PROP_SHADOW_BORDER, (m_shadowWidth - m_shadowDilate));
						_materialRecord.SetFloat(PROP_OUTLINE_BORDER, 0);
						_materialRecord.SetFloat(PROP_GRAPHIC_BORDER, -m_outlineWidth);
						break;
					case OutlineType.Outside:
						_materialRecord.SetFloat(PROP_SHADOW_BORDER, (m_shadowWidth - m_shadowDilate) + m_outlineWidth);
						_materialRecord.SetFloat(PROP_OUTLINE_BORDER, m_outlineWidth);
						_materialRecord.SetFloat(PROP_GRAPHIC_BORDER, 0);
						break;
				}

				var patternType = m_outlinePatternType;
				switch (patternType)
				{
					case PatternType.None:
						_materialRecord.DisableKeyword(KEYWORD_OUTLINE_PATTERN_SHINY, KEYWORD_OUTLINE_PATTERN_TEX);
						break;
					case PatternType.Shiny:
						_materialRecord.EnableKeyword(KEYWORD_OUTLINE_PATTERN_SHINY);
						_materialRecord.DisableKeyword(KEYWORD_OUTLINE_PATTERN_TEX);

						_materialRecord.SetFloat(PROP_OUTLINE_SHINY_ANGLE, Mathf.PI * m_outlineShinyAngle);
						_materialRecord.SetFloat(PROP_OUTLINE_SHINY_WIDTH, Mathf.PI * m_outlineShinyWidth);
						_materialRecord.SetFloat(PROP_OUTLINE_SHINY_BLUR, hminSize * m_outlineShinyBlur);

						_materialRecord.SetColor(PROP_OUTLINE_PATTERN_COLOR, m_outlinePatternColor);
						_materialRecord.SetVector(PROP_OUTLINE_PATTERN_OFFSET, m_outlinePatternOffset);
						break;
					case PatternType.Texture:
						_materialRecord.DisableKeyword(KEYWORD_OUTLINE_PATTERN_SHINY);
						_materialRecord.EnableKeyword(KEYWORD_OUTLINE_PATTERN_TEX);

						_materialRecord.SetTexture(PROP_OUTLINE_PATTERN_TEXTURE, m_outlinePatternTexture);
						_materialRecord.SetFloat(PROP_OUTLINE_PATTERN_TEXTURE_ROW, m_outlinePatternTextureRow);
						_materialRecord.SetVector(PROP_OUTLINE_PATTERN_TEXTURE_SCALE, m_outlinePatternTextureScale);
						break;
				}
			}
			else
			{
				_materialRecord.SetFloat(PROP_OUTLINE_WIDTH, 0);
				_materialRecord.SetColor(PROP_OUTLINE_COLOR, m_fillColor);

				_materialRecord.SetFloat(PROP_SHADOW_BORDER, (m_shadowWidth - m_shadowDilate));
				_materialRecord.SetFloat(PROP_OUTLINE_BORDER, 0);
				_materialRecord.SetFloat(PROP_GRAPHIC_BORDER, 0);
			}

			AntialiasingType antialiasing = m_antialiasing is AntialiasingType.Default ? SDFUISettings.Instance.DefaultAA : m_antialiasing;

			switch (antialiasing)
			{
				case AntialiasingType.OFF:
					_materialRecord.DisableKeyword(KEYWORD_AA);
					break;
				case AntialiasingType.ON:
					_materialRecord.EnableKeyword(KEYWORD_AA);
					break;
			}

			_materialRecord.SetFloat(PROP_PADDING, m_extraMargin);
		}

		public override void SetMaterialDirty()
		{
			base.SetMaterialDirty();

			if (!IsActive())
				return;

			materialDirty = true;

			UpdateMaterialRecord();
		}
	}
}