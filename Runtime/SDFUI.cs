using TLab.UI.SDF.Registry;
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

		internal const string KEYWORD_OUTLINE_EFFECT_SHINY = SHADER_KEYWORD_PREFIX + "OUTLINE_EFFECT_SHINY";
		internal const string KEYWORD_GRAPHIC_EFFECT_SHINY = SHADER_KEYWORD_PREFIX + "GRAPHIC_EFFECT_SHINY";

		internal const string KEYWORD_OUTLINE_EFFECT_PATTERN = SHADER_KEYWORD_PREFIX + "OUTLINE_EFFECT_PATTERN";
		internal const string KEYWORD_GRAPHIC_EFFECT_PATTERN = SHADER_KEYWORD_PREFIX + "GRAPHIC_EFFECT_PATTERN";

		#endregion SHADER_KEYWORD

		#region SHADER_PROP

		internal static readonly int PROP_HALFSIZE = Shader.PropertyToID("_HalfSize");
		internal static readonly int PROP_PADDING = Shader.PropertyToID("_Padding");
		internal static readonly int PROP_EULER_Z = Shader.PropertyToID("_EulerZ");
		internal static readonly int PROP_OUTERUV = Shader.PropertyToID("_OuterUV");
		internal static readonly int PROP_RECTSIZE = Shader.PropertyToID("_RectSize");

		internal static readonly int PROP_ONION = Shader.PropertyToID("_Onion");
		internal static readonly int PROP_ONION_WIDTH = Shader.PropertyToID("_OnionWidth");

		internal static readonly int PROP_MAINTEX = Shader.PropertyToID("_MainTex");

		internal static readonly int PROP_SHADOW_BORDER = Shader.PropertyToID("_ShadowBorder");
		internal static readonly int PROP_OUTLINE_BORDER = Shader.PropertyToID("_OutlineBorder");
		internal static readonly int PROP_GRAPHIC_BORDER = Shader.PropertyToID("_GraphicBorder");

		internal static readonly int PROP_GRAPHIC_GRADATION_COLOR = Shader.PropertyToID("_GraphicGradationColor");
		internal static readonly int PROP_GRAPHIC_GRADATION_LAYER = Shader.PropertyToID("_GraphicGradationLayer");
		internal static readonly int PROP_GRAPHIC_GRADATION_RANGE = Shader.PropertyToID("_GraphicGradationRange");
		internal static readonly int PROP_GRAPHIC_GRADATION_ANGLE = Shader.PropertyToID("_GraphicGradationAngle");
		internal static readonly int PROP_GRAPHIC_GRADATION_RADIUS = Shader.PropertyToID("_GraphicGradationRadius");
		internal static readonly int PROP_GRAPHIC_GRADATION_SMOOTH = Shader.PropertyToID("_GraphicGradationSmooth");
		internal static readonly int PROP_GRAPHIC_GRADATION_OFFSET = Shader.PropertyToID("_GraphicGradationOffset");

		internal static readonly int PROP_GRAPHIC_USE_RAINBOW = Shader.PropertyToID("_GraphicUseRainbow");
		internal static readonly int PROP_GRAPHIC_RAINBOW_SATURATION = Shader.PropertyToID("_GraphicRainbowSaturation");
		internal static readonly int PROP_GRAPHIC_RAINBOW_VALUE = Shader.PropertyToID("_GraphicRainbowValue");

		internal static readonly int PROP_SHADOW_WIDTH = Shader.PropertyToID("_ShadowWidth");
		internal static readonly int PROP_SHADOW_BLUR = Shader.PropertyToID("_ShadowBlur");
		internal static readonly int PROP_SHADOW_DILATE = Shader.PropertyToID("_ShadowDilate");
		internal static readonly int PROP_SHADOW_COLOR = Shader.PropertyToID("_ShadowColor");
		internal static readonly int PROP_SHADOW_OFFSET = Shader.PropertyToID("_ShadowOffset");
		internal static readonly int PROP_SHADOW_GAUSSIAN = Shader.PropertyToID("_ShadowGaussian");
		internal static readonly int PROP_SHADOW_GRADATION_COLOR = Shader.PropertyToID("_ShadowGradationColor");
		internal static readonly int PROP_SHADOW_GRADATION_LAYER = Shader.PropertyToID("_ShadowGradationLayer");
		internal static readonly int PROP_SHADOW_GRADATION_RANGE = Shader.PropertyToID("_ShadowGradationRange");
		internal static readonly int PROP_SHADOW_GRADATION_ANGLE = Shader.PropertyToID("_ShadowGradationAngle");
		internal static readonly int PROP_SHADOW_GRADATION_RADIUS = Shader.PropertyToID("_ShadowGradationRadius");
		internal static readonly int PROP_SHADOW_GRADATION_SMOOTH = Shader.PropertyToID("_ShadowGradationSmooth");
		internal static readonly int PROP_SHADOW_GRADATION_OFFSET = Shader.PropertyToID("_ShadowGradationOffset");

		internal static readonly int PROP_OUTLINE_TYPE = Shader.PropertyToID("_OutlineType");
		internal static readonly int PROP_OUTLINE_WIDTH = Shader.PropertyToID("_OutlineWidth");
		internal static readonly int PROP_OUTLINE_COLOR = Shader.PropertyToID("_OutlineColor");
		internal static readonly int PROP_OUTLINE_INNER_BLUR = Shader.PropertyToID("_OutlineInnerBlur");
		internal static readonly int PROP_OUTLINE_INNER_GAUSSIAN = Shader.PropertyToID("_OutlineInnerGaussian");
		internal static readonly int PROP_OUTLINE_GRADATION_COLOR = Shader.PropertyToID("_OutlineGradationColor");
		internal static readonly int PROP_OUTLINE_GRADATION_LAYER = Shader.PropertyToID("_OutlineGradationLayer");
		internal static readonly int PROP_OUTLINE_GRADATION_ANGLE = Shader.PropertyToID("_OutlineGradationAngle");
		internal static readonly int PROP_OUTLINE_GRADATION_RANGE = Shader.PropertyToID("_OutlineGradationRange");
		internal static readonly int PROP_OUTLINE_GRADATION_RADIUS = Shader.PropertyToID("_OutlineGradationRadius");
		internal static readonly int PROP_OUTLINE_GRADATION_SMOOTH = Shader.PropertyToID("_OutlineGradationSmooth");
		internal static readonly int PROP_OUTLINE_GRADATION_OFFSET = Shader.PropertyToID("_OutlineGradationOffset");

		internal static readonly int PROP_GRAPHIC_EFFECT_SHINY_WIDTH = Shader.PropertyToID("_GraphicEffectShinyWidth");
		internal static readonly int PROP_GRAPHIC_EFFECT_SHINY_BLUR = Shader.PropertyToID("_GraphicEffectShinyBlur");

		internal static readonly int PROP_OUTLINE_EFFECT_SHINY_WIDTH = Shader.PropertyToID("_OutlineEffectShinyWidth");
		internal static readonly int PROP_OUTLINE_EFFECT_SHINY_BLUR = Shader.PropertyToID("_OutlineEffectShinyBlur");

		internal static readonly int PROP_OUTLINE_EFFECT_PATTERN_TEX = Shader.PropertyToID("_OutlineEffectPatternTex");
		internal static readonly int PROP_OUTLINE_EFFECT_PATTERN_ROW = Shader.PropertyToID("_OutlineEffectPatternRow");
		internal static readonly int PROP_OUTLINE_EFFECT_PATTERN_SCALE = Shader.PropertyToID("_OutlineEffectPatternScale");
		internal static readonly int PROP_OUTLINE_EFFECT_PATTERN_SCROLL = Shader.PropertyToID("_OutlineEffectPatternScroll");
		internal static readonly int PROP_OUTLINE_EFFECT_PATTERN_PARAMS = Shader.PropertyToID("_OutlineEffectPatternParams");

		internal static readonly int PROP_GRAPHIC_EFFECT_PATTERN_TEX = Shader.PropertyToID("_GraphicEffectPatternTex");
		internal static readonly int PROP_GRAPHIC_EFFECT_PATTERN_ROW = Shader.PropertyToID("_GraphicEffectPatternRow");
		internal static readonly int PROP_GRAPHIC_EFFECT_PATTERN_SCALE = Shader.PropertyToID("_GraphicEffectPatternScale");
		internal static readonly int PROP_GRAPHIC_EFFECT_PATTERN_SCROLL = Shader.PropertyToID("_GraphicEffectPatternScroll");
		internal static readonly int PROP_GRAPHIC_EFFECT_PATTERN_PARAMS = Shader.PropertyToID("_GraphicEffectPatternParams");

		internal static readonly int PROP_GRAPHIC_EFFECT_ANGLE = Shader.PropertyToID("_GraphicEffectAngle");
		internal static readonly int PROP_GRAPHIC_EFFECT_COLOR = Shader.PropertyToID("_GraphicEffectColor");
		internal static readonly int PROP_OUTLINE_EFFECT_OFFSET = Shader.PropertyToID("_OutlineEffectOffset");

		internal static readonly int PROP_OUTLINE_EFFECT_ANGLE = Shader.PropertyToID("_OutlineEffectAngle");
		internal static readonly int PROP_OUTLINE_EFFECT_COLOR = Shader.PropertyToID("_OutlineEffectColor");
		internal static readonly int PROP_GRAPHIC_EFFECT_OFFSET = Shader.PropertyToID("_GraphicEffectOffset");

		internal static readonly int PROP_GRAPHIC_RAINBOW_HUE_OFFSET = Shader.PropertyToID("_GraphicRainbowHueOffset");

		// Add a new shader property ID for the outline rainbow
		internal static readonly int PROP_OUTLINE_USE_RAINBOW = Shader.PropertyToID("_OutlineUseRainbow");

		// Add a new shader property ID for the shadow rainbow
		internal static readonly int PROP_SHADOW_USE_RAINBOW = Shader.PropertyToID("_ShadowUseRainbow");

		#endregion SHADER_PROP

		#region ENUM

		public enum GradationShape
		{
			None,
			Linear,
			Radial,
			Conical,
		}

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

		public enum EffectType
		{
			None = 0,
			Shiny = 1,
			Pattern = 2,
			Rainbow = 3,
		};

		#endregion ENUM

		[SerializeField, LeftToggle] protected bool m_onion = false;
		[SerializeField, Min(0f)] protected float m_onionWidth = 10;

		[SerializeField] protected AntialiasingType m_antialiasing = AntialiasingType.Default;

		#region OUTLINE

		[SerializeField, LeftToggle] protected bool m_outline = true;
		[SerializeField, Min(0f)] protected float m_outlineWidth = 10;
		[SerializeField, Min(0f)] protected float m_outlineInnerSoftWidth = 0;
		[SerializeField, Range(0, 1)] protected float m_outlineInnerSoftness = 0.0f;
		[SerializeField, ColorUsage(true, true)] protected Color m_outlineColor = Color.cyan;
		[SerializeField] protected OutlineType m_outlineType = OutlineType.Inside;

		#region GRADATION
		[SerializeField, ColorUsage(true, true)] protected Color m_outlineGradationColor = Color.cyan;
		[SerializeField, Range(0, 2)] protected float m_outlineGradationAngle = 0;
		[SerializeField, Min(0)] protected float m_outlineGradationRadius = 0.5f;
		[SerializeField, Min(0)] protected float m_outlineGradationSmooth = 0.5f;
		[SerializeField, MinMaxRange(0, 1)] protected Vector2 m_outlineGradationRange = new Vector2(0.25f, 0.75f);
		[SerializeField] protected Vector2 m_outlineGradationOffset;
		[SerializeField] protected GradationShape m_outlineGradationShape = GradationShape.None;
		#endregion GRADATION

		#region EFFECT
		[SerializeField] protected EffectType m_outlineEffectType = EffectType.None;
		[SerializeField, ColorUsage(true, true)] protected Color m_outlineEffectColor = Color.white;
		[SerializeField] protected Vector2 m_outlineEffectOffset;
		[SerializeField, Range(-1, 1)] protected float m_outlineEffectAngle = 0.0f;
		[SerializeField, Range(0, 1)] protected float m_outlineEffectShinyBlur = 0.0f;
		[SerializeField, Range(0, 1)] protected float m_outlineEffectShinyWidth = 0.25f;
		[SerializeField] protected Texture m_outlineEffectPatternTexture;
		[SerializeField, Min(0)] protected int m_outlineEffectPatternRow = 5;
		[SerializeField] protected float m_outlineEffectPatternScroll = 0;
		[SerializeField, Min(0)] protected float m_outlineEffectPatternParamsX = 1;
		[SerializeField, Min(0)] protected float m_outlineEffectPatternParamsY = 1;
		[SerializeField, Min(0)] protected float m_outlineEffectPatternParamsZ = 1;
		[SerializeField, Min(0)] protected float m_outlineEffectPatternParamsW = 1;
		[SerializeField, Min(0)] protected Vector2 m_outlineEffectPatternScale = Vector2.one;
		#endregion EFFECT

		#endregion OUTLINE

		#region SHADOW

		[SerializeField, LeftToggle] protected bool m_shadow = false;
		[SerializeField, Min(0f)] protected float m_shadowWidth = 10;
		[SerializeField, Min(0f)] protected float m_shadowInnerSoftWidth = 0;
		[SerializeField, Range(0, 1)] protected float m_shadowSoftness = 0.0f;
		[SerializeField, Min(0f)] protected float m_shadowDilate = 0;
		[SerializeField] protected Vector2 m_shadowOffset;
		[SerializeField, ColorUsage(true, true)] protected Color m_shadowColor = Color.black;
		[SerializeField] protected EffectType m_shadowEffectType = EffectType.None;

		#region GRADATION
		[SerializeField, ColorUsage(true, true)] protected Color m_shadowGradationColor = Color.black;
		[SerializeField, Range(0, 2)] protected float m_shadowGradationAngle = 0;
		[SerializeField, Min(0)] protected float m_shadowGradationRadius = 0.5f;
		[SerializeField, Min(0)] protected float m_shadowGradationSmooth = 0.5f;
		[SerializeField, MinMaxRange(0, 1)] protected Vector2 m_shadowGradationRange = new Vector2(0.25f, 0.75f);
		[SerializeField] protected Vector2 m_shadowGradationOffset;
		[SerializeField] protected GradationShape m_shadowGradationShape = GradationShape.None;
		#endregion GRADATION

		#endregion SHADOW

		#region GRAPHIC

		[SerializeField, ColorUsage(true)] protected Color m_fillColor = Color.white;
		[SerializeField] protected ActiveImageType m_activeImageType;
		[SerializeField] protected Sprite m_sprite;
		[SerializeField] protected Texture m_texture;
		[SerializeField] protected Rect m_uvRect = new Rect(0f, 0f, 1f, 1f);

		#region GRADATION
		[SerializeField, ColorUsage(true)] protected Color m_gradationColor = Color.white;
		[SerializeField, Range(0, 2)] protected float m_gradationAngle = 0;
		[SerializeField, Min(0)] protected float m_gradationRadius = 0.5f;
		[SerializeField, Min(0)] protected float m_gradationSmooth = 0.5f;
		[SerializeField, MinMaxRange(0, 1)] protected Vector2 m_gradationRange = new Vector2(0.25f, 0.75f);
		[SerializeField] protected Vector2 m_gradationOffset;
		[SerializeField] protected GradationShape m_gradationShape = GradationShape.None;
		#endregion GRADATION

		#region EFFECT
		[SerializeField] protected EffectType m_graphicEffectType = EffectType.None;
		[SerializeField, ColorUsage(true, true)] protected Color m_graphicEffectColor = Color.white;
		[SerializeField] protected Vector2 m_graphicEffectOffset;
		[SerializeField, Range(0, 1)] protected float m_graphicEffectAngle = 0.0f;
		[SerializeField, Range(0, 1)] protected float m_graphicEffectShinyBlur = 0.0f;
		[SerializeField, Range(0, 1)] protected float m_graphicEffectShinyWidth = 0.25f;
		[SerializeField] protected Texture m_graphicEffectPatternTexture;
		[SerializeField, Min(0)] protected int m_graphicEffectPatternRow = 5;
		[SerializeField] protected float m_graphicEffectPatternScroll = 0;
		[SerializeField, Min(0)] protected float m_graphicEffectPatternParamsX = 1;
		[SerializeField, Min(0)] protected float m_graphicEffectPatternParamsY = 1;
		[SerializeField, Min(0)] protected float m_graphicEffectPatternParamsZ = 1;
		[SerializeField, Min(0)] protected float m_graphicEffectPatternParamsW = 1;
		[SerializeField, Min(0)] protected Vector2 m_graphicEffectPatternScale = Vector2.one;
		// Rainbow effect properties
		[SerializeField, Range(0, 1)] protected float m_rainbowSaturation = 1.0f;
		[SerializeField, Range(0, 1)] protected float m_rainbowValue = 1.0f;
		[SerializeField, Range(0, 1)] protected float m_rainbowHueOffset = 0.0f;
		#endregion EFFECT

		#endregion GRAPHIC

		#region RAINBOW
		[SerializeField, LeftToggle] protected bool m_useRainbow = false;
		[SerializeField, LeftToggle] protected bool m_outlineUseRainbow = false;
		#endregion RAINBOW

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

		#region ONION

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

		#endregion ONION

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

		public EffectType shadowEffectType
		{
			get => m_shadowEffectType;
			set
			{
				if (m_shadowEffectType != value)
				{
					m_shadowEffectType = value;
					
					SetAllDirty();
				}
			}
		}

		#region GRADATION
		public Color shadowGradationColor
		{
			get => m_shadowGradationColor;
			set
			{
				if (m_shadowGradationColor != value)
				{
					m_shadowGradationColor = value;

					SetAllDirty();
				}
			}
		}

		public GradationShape shadowGradationShape
		{
			get => m_shadowGradationShape;
			set
			{
				if (m_shadowGradationShape != value)
				{
					m_shadowGradationShape = value;

					SetAllDirty();
				}
			}
		}

		public float shadowGradationAngle
		{
			get => m_shadowGradationAngle;
			set
			{
				if (m_shadowGradationAngle != value)
				{
					var tmp = Mathf.Clamp01(value);
					m_shadowGradationAngle = tmp;

					SetAllDirty();
				}
			}
		}

		public float shadowGradationRadius
		{
			get => m_shadowGradationRadius;
			set
			{
				if (m_shadowGradationRadius != value)
				{
					m_shadowGradationRadius = math.max(value, 0);

					SetAllDirty();
				}
			}
		}

		public float shadowGradationSmooth
		{
			get => m_shadowGradationSmooth;
			set
			{
				if (m_shadowGradationSmooth != value)
				{
					m_shadowGradationSmooth = math.max(value, 0);

					SetAllDirty();
				}
			}
		}

		public Vector2 shadowGradationRange
		{
			get => m_shadowGradationRange;
			set
			{
				if (m_shadowGradationRange != value)
				{
					var tmp = value;
					tmp.x = Mathf.Clamp01(tmp.x);
					tmp.y = Mathf.Clamp01(tmp.y);
					m_shadowGradationRange = tmp;

					SetAllDirty();
				}
			}
		}

		public Vector2 shadowGradationOffset
		{
			get => m_shadowGradationOffset;
			set
			{
				if (m_shadowGradationOffset != value)
				{
					m_shadowGradationOffset = value;

					SetAllDirty();
				}
			}
		}
		#endregion GRADATION

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

		#region GRADATION
		public Color outlineGradationColor
		{
			get => m_outlineGradationColor;
			set
			{
				if (m_outlineGradationColor != value)
				{
					m_outlineGradationColor = value;

					SetAllDirty();
				}
			}
		}

		public GradationShape outlineGradationShape
		{
			get => m_outlineGradationShape;
			set
			{
				if (m_outlineGradationShape != value)
				{
					m_outlineGradationShape = value;

					SetAllDirty();
				}
			}
		}

		public float outlineGradationAngle
		{
			get => m_outlineGradationAngle;
			set
			{
				if (m_outlineGradationAngle != value)
				{
					var tmp = Mathf.Clamp01(value);
					m_outlineGradationAngle = tmp;

					SetAllDirty();
				}
			}
		}

		public float outlineGradationRadius
		{
			get => m_outlineGradationRadius;
			set
			{
				if (m_outlineGradationRadius != value)
				{
					m_outlineGradationRadius = math.max(value, 0);

					SetAllDirty();
				}
			}
		}

		public float outlineGradationSmooth
		{
			get => m_outlineGradationSmooth;
			set
			{
				if (m_outlineGradationSmooth != value)
				{
					m_outlineGradationSmooth = math.max(value, 0);

					SetAllDirty();
				}
			}
		}

		public Vector2 outlineGradationRange
		{
			get => m_outlineGradationRange;
			set
			{
				if (m_outlineGradationRange != value)
				{
					var tmp = value;
					tmp.x = Mathf.Clamp01(tmp.x);
					tmp.y = Mathf.Clamp01(tmp.y);
					m_outlineGradationRange = tmp;

					SetAllDirty();
				}
			}
		}

		public Vector2 outlineGradationOffset
		{
			get => m_outlineGradationOffset;
			set
			{
				if (m_outlineGradationOffset != value)
				{
					m_outlineGradationOffset = value;

					SetAllDirty();
				}
			}
		}
		#endregion GRADATION

		#region EFFECT

		public EffectType outlineEffectType
		{
			get => m_outlineEffectType;
			set
			{
				if (m_outlineEffectType != value)
				{
					m_outlineEffectType = value;

					SetAllDirty();
				}
			}
		}

		public Color outlineEffectColor
		{
			get => m_outlineEffectColor;
			set
			{
				if (m_outlineEffectColor != value)
				{
					m_outlineEffectColor = value;

					SetAllDirty();
				}
			}
		}

		public Vector2 outlineEffectOffset
		{
			get => m_outlineEffectOffset;
			set
			{
				var tmp = value;
				tmp.x = Mathf.Clamp(tmp.x, 0, 2);
				tmp.y = Mathf.Clamp(tmp.y, 0, 2);

				if (m_outlineEffectOffset != tmp)
				{
					m_outlineEffectOffset = tmp;

					SetAllDirty();
				}
			}

		}

		public float outlineEffectShinyWidth
		{
			get => m_outlineEffectShinyWidth;
			set
			{
				if (m_outlineEffectShinyWidth != value)
				{
					m_outlineEffectShinyWidth = value;

					SetAllDirty();
				}
			}
		}

		public float outlineEffectAngle
		{
			get => m_outlineEffectAngle;
			set
			{
				var tmp = Mathf.Clamp(value, -1, 1);

				if (m_outlineEffectAngle != tmp)
				{
					m_outlineEffectAngle = tmp;

					SetAllDirty();
				}
			}
		}

		public float outlineEffectShinyBlur
		{
			get => m_outlineEffectShinyBlur;
			set
			{
				if (m_outlineEffectShinyBlur != value)
				{
					m_outlineEffectShinyBlur = value;

					SetAllDirty();
				}
			}
		}

		public Texture outlineEffectPatternTexture
		{
			get => m_outlineEffectPatternTexture;
			set
			{
				if (m_outlineEffectPatternTexture != value)
				{
					m_outlineEffectPatternTexture = value;

					SetAllDirty();
				}
			}
		}

		public Vector2 outlineEffectPatternScale
		{
			get => m_outlineEffectPatternScale;
			set
			{
				var tmp = value;
				tmp.x = Mathf.Max(tmp.x, 0);
				tmp.y = Mathf.Max(tmp.y, 0);

				if (m_outlineEffectPatternScale != tmp)
				{
					m_outlineEffectPatternScale = tmp;

					SetAllDirty();
				}
			}
		}

		public float outlineEffectPatternScroll
		{
			get => m_outlineEffectPatternScroll;
			set
			{
				if (m_outlineEffectPatternScroll != value)
				{
					m_outlineEffectPatternScroll = value;

					SetAllDirty();
				}
			}
		}

		public float outlineEffectPatternParamsX
		{
			get => m_outlineEffectPatternParamsX;
			set
			{
				var tmp = Mathf.Max(value, 0);

				if (m_outlineEffectPatternParamsX != tmp)
				{
					m_outlineEffectPatternParamsX = tmp;

					SetAllDirty();
				}
			}
		}

		public float outlineEffectPatternParamsY
		{
			get => m_outlineEffectPatternParamsY;
			set
			{
				var tmp = Mathf.Max(value, 0);

				if (m_outlineEffectPatternParamsY != tmp)
				{
					m_outlineEffectPatternParamsY = tmp;

					SetAllDirty();
				}
			}
		}

		public float outlineEffectPatternParamsZ
		{
			get => m_outlineEffectPatternParamsZ;
			set
			{
				var tmp = Mathf.Max(value, 0);

				if (m_outlineEffectPatternParamsZ != tmp)
				{
					m_outlineEffectPatternParamsZ = tmp;

					SetAllDirty();
				}
			}
		}

		public float outlineEffectPatternParamsW
		{
			get => m_outlineEffectPatternParamsW;
			set
			{
				var tmp = Mathf.Max(value, 0);

				if (m_outlineEffectPatternParamsW != tmp)
				{
					m_outlineEffectPatternParamsW = tmp;

					SetAllDirty();
				}
			}
		}

		public int outlineEffectPatternRow
		{
			get => m_outlineEffectPatternRow;
			set
			{
				var tmp = Mathf.Max(0, value);

				if (m_outlineEffectPatternRow != tmp)
				{
					m_outlineEffectPatternRow = tmp;

					SetAllDirty();
				}
			}
		}

		#endregion EFFECT

		#endregion OUTLINE

		#region GRAPHIC

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

		#region GRADATION
		public Color gradationColor
		{
			get => m_gradationColor;
			set
			{
				if (m_gradationColor != value)
				{
					m_gradationColor = value;

					SetAllDirty();
				}
			}
		}

		public GradationShape gradationShape
		{
			get => m_gradationShape;
			set
			{
				if (m_gradationShape != value)
				{
					m_gradationShape = value;

					SetAllDirty();
				}
			}
		}

		public float gradationAngle
		{
			get => m_gradationAngle;
			set
			{
				if (m_gradationAngle != value)
				{
					var tmp = Mathf.Clamp01(value);
					m_gradationAngle = tmp;

					SetAllDirty();
				}
			}
		}

		public float gradationRadius
		{
			get => m_gradationRadius; set
			{
				if (m_gradationRadius != value)
				{
					m_gradationRadius = math.max(value, 0);

					SetAllDirty();
				}
			}
		}

		public float gradationSmooth
		{
			get => m_gradationSmooth;
			set
			{
				if (m_gradationSmooth != value)
				{
					m_gradationSmooth = math.max(value, 0);

					SetAllDirty();
				}
			}
		}

		public Vector2 gradationRange
		{
			get => m_gradationRange;
			set
			{
				if (m_gradationRange != value)
				{
					var tmp = value;
					tmp.x = Mathf.Clamp01(tmp.x);
					tmp.y = Mathf.Clamp01(tmp.y);
					m_gradationRange = tmp;

					SetAllDirty();
				}
			}
		}

		public Vector2 gradationOffset
		{
			get => m_gradationOffset;
			set
			{
				if (m_gradationOffset != value)
				{
					m_gradationOffset = value;

					SetAllDirty();
				}
			}
		}
		#endregion GRADATION

		#region EFFECT
		public EffectType graphicEffectType
		{
			get => m_graphicEffectType;
			set
			{
				if (m_graphicEffectType != value)
				{
					m_graphicEffectType = value;

					SetAllDirty();
				}
			}
		}

		public Color graphicEffectColor
		{
			get => m_graphicEffectColor;
			set
			{
				if (m_graphicEffectColor != value)
				{
					m_graphicEffectColor = value;

					SetAllDirty();
				}
			}
		}

		public Vector2 graphicEffectOffset
		{
			get => m_graphicEffectOffset;
			set
			{
				var tmp = value;
				tmp.x = Mathf.Clamp(tmp.x, 0, 1);
				tmp.y = Mathf.Clamp(tmp.y, 0, 1);

				if (m_graphicEffectOffset != tmp)
				{
					m_graphicEffectOffset = tmp;

					SetAllDirty();
				}
			}

		}

		public float graphicEffectShinyWidth
		{
			get => m_graphicEffectShinyWidth;
			set
			{
				if (m_graphicEffectShinyWidth != value)
				{
					m_graphicEffectShinyWidth = value;

					SetAllDirty();
				}
			}
		}

		public float graphicEffectAngle
		{
			get => m_graphicEffectAngle;
			set
			{
				if (m_graphicEffectAngle != value)
				{
					m_graphicEffectAngle = value;

					SetAllDirty();
				}
			}
		}

		public float graphicEffectShinyBlur
		{
			get => m_graphicEffectShinyBlur;
			set
			{
				if (m_graphicEffectShinyBlur != value)
				{
					m_graphicEffectShinyBlur = value;

					SetAllDirty();
				}
			}
		}

		public Texture graphicEffectPatternTexture
		{
			get => m_graphicEffectPatternTexture;
			set
			{
				if (m_graphicEffectPatternTexture != value)
				{
					m_graphicEffectPatternTexture = value;

					SetAllDirty();
				}
			}
		}

		public Vector2 graphicEffectPatternScale
		{
			get => m_graphicEffectPatternScale;
			set
			{
				var tmp = value;
				tmp.x = Mathf.Max(tmp.x, 0);
				tmp.y = Mathf.Max(tmp.y, 0);

				if (m_graphicEffectPatternScale != tmp)
				{
					m_graphicEffectPatternScale = tmp;

					SetAllDirty();
				}
			}
		}

		public float graphicEffectPatternScroll
		{
			get => m_graphicEffectPatternScroll;
			set
			{
				if (m_graphicEffectPatternScroll != value)
				{
					m_graphicEffectPatternScroll = value;

					SetAllDirty();
				}
			}
		}

		public float graphicEffectPatternParamsX
		{
			get => m_graphicEffectPatternParamsX;
			set
			{
				var tmp = Mathf.Max(value, 0);

				if (m_graphicEffectPatternParamsX != tmp)
				{
					m_graphicEffectPatternParamsX = tmp;

					SetAllDirty();
				}
			}
		}

		public float graphicEffectPatternParamsY
		{
			get => m_graphicEffectPatternParamsY;
			set
			{
				var tmp = Mathf.Max(value, 0);

				if (m_graphicEffectPatternParamsY != tmp)
				{
					m_graphicEffectPatternParamsY = tmp;

					SetAllDirty();
				}
			}
		}

		public float graphicEffectPatternParamsZ
		{
			get => m_graphicEffectPatternParamsZ;
			set
			{
				var tmp = Mathf.Max(value, 0);

				if (m_graphicEffectPatternParamsZ != tmp)
				{
					m_graphicEffectPatternParamsZ = tmp;

					SetAllDirty();
				}
			}
		}

		public float graphicEffectPatternParamsW
		{
			get => m_graphicEffectPatternParamsW;
			set
			{
				var tmp = Mathf.Max(value, 0);

				if (m_graphicEffectPatternParamsW != tmp)
				{
					m_graphicEffectPatternParamsW = tmp;

					SetAllDirty();
				}
			}
		}

		public int graphicEffectPatternRow
		{
			get => m_graphicEffectPatternRow;
			set
			{
				var tmp = Mathf.Max(0, value);

				if (m_graphicEffectPatternRow != tmp)
				{
					m_graphicEffectPatternRow = tmp;

					SetAllDirty();
				}
			}
		}
		#endregion EFFECT

		#endregion GRAPHIC

		#region MATERIAL

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

		#endregion MATERIAL

		#region IMAGE

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

		#endregion IMAGE

		internal MaterialRecord MaterialRecord => (MaterialRecord)_materialRecord.Clone();
		private protected MaterialRecord _materialRecord { get; } = new();

		protected bool materialDirty;

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
			if (!Mathf.Approximately(eulerZ, rectTransform.eulerAngles.z))
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

			{
				var gradationShape = m_gradationShape;
				switch (gradationShape)
				{
					case GradationShape.None:
						_materialRecord.SetColor(PROP_GRAPHIC_GRADATION_COLOR, m_fillColor);
						_materialRecord.SetVector(PROP_GRAPHIC_GRADATION_LAYER, new float4(1, 0, 0, 0));
						break;
					case GradationShape.Linear:
						_materialRecord.SetColor(PROP_GRAPHIC_GRADATION_COLOR, m_gradationColor);
						_materialRecord.SetVector(PROP_GRAPHIC_GRADATION_LAYER, new float4(0, 1, 0, 0));
						break;
					case GradationShape.Radial:
						_materialRecord.SetColor(PROP_GRAPHIC_GRADATION_COLOR, m_gradationColor);
						_materialRecord.SetVector(PROP_GRAPHIC_GRADATION_LAYER, new float4(0, 0, 1, 0));
						break;
					case GradationShape.Conical:
						_materialRecord.SetColor(PROP_GRAPHIC_GRADATION_COLOR, m_gradationColor);
						_materialRecord.SetVector(PROP_GRAPHIC_GRADATION_LAYER, new float4(0, 0, 0, 1));
						break;
				}

				_materialRecord.SetFloat(PROP_GRAPHIC_GRADATION_ANGLE, m_gradationAngle);
				_materialRecord.SetFloat(PROP_GRAPHIC_GRADATION_SMOOTH, m_gradationSmooth);
				_materialRecord.SetFloat(PROP_GRAPHIC_GRADATION_RADIUS, m_gradationRadius);
				_materialRecord.SetVector(PROP_GRAPHIC_GRADATION_RANGE, m_gradationRange);
				_materialRecord.SetVector(PROP_GRAPHIC_GRADATION_OFFSET, m_gradationOffset);
				
				// Set rainbow gradient properties - only apply when effect type is Rainbow
				bool useRainbowGradient = m_graphicEffectType == EffectType.Rainbow;
				_materialRecord.SetFloat(PROP_GRAPHIC_USE_RAINBOW, useRainbowGradient ? 1 : 0);
				_materialRecord.SetFloat(PROP_GRAPHIC_RAINBOW_SATURATION, m_rainbowSaturation);
				_materialRecord.SetFloat(PROP_GRAPHIC_RAINBOW_VALUE, m_rainbowValue);
				_materialRecord.SetFloat(PROP_GRAPHIC_RAINBOW_HUE_OFFSET, m_rainbowHueOffset);
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

				_materialRecord.SetFloat(PROP_SHADOW_GRADATION_ANGLE, m_shadowGradationAngle);
				_materialRecord.SetFloat(PROP_SHADOW_GRADATION_SMOOTH, m_shadowGradationSmooth);
				_materialRecord.SetFloat(PROP_SHADOW_GRADATION_RADIUS, m_shadowGradationRadius);
				_materialRecord.SetVector(PROP_SHADOW_GRADATION_RANGE, m_shadowGradationRange);
				_materialRecord.SetVector(PROP_SHADOW_GRADATION_OFFSET, m_shadowGradationOffset);

				var gradationShape = m_shadowGradationShape;
				switch (gradationShape)
				{
					case GradationShape.None:
						_materialRecord.SetColor(PROP_SHADOW_GRADATION_COLOR, m_shadowColor);
						_materialRecord.SetVector(PROP_SHADOW_GRADATION_LAYER, new float4(1, 0, 0, 0));
						break;
					case GradationShape.Linear:
						_materialRecord.SetColor(PROP_SHADOW_GRADATION_COLOR, m_shadowGradationColor);
						_materialRecord.SetVector(PROP_SHADOW_GRADATION_LAYER, new float4(0, 1, 0, 0));
						break;
					case GradationShape.Radial:
						_materialRecord.SetColor(PROP_SHADOW_GRADATION_COLOR, m_shadowGradationColor);
						_materialRecord.SetVector(PROP_SHADOW_GRADATION_LAYER, new float4(0, 0, 1, 0));
						break;
					case GradationShape.Conical:
						_materialRecord.SetColor(PROP_SHADOW_GRADATION_COLOR, m_shadowGradationColor);
						_materialRecord.SetVector(PROP_SHADOW_GRADATION_LAYER, new float4(0, 0, 0, 1));
						break;
				}

				MeshUtils.ShadowSizeOffset(rectTransform.rect.size, m_shadowOffset, rectTransform.eulerAngles.z, out float4 sizeOffset);
				_materialRecord.SetVector(PROP_SHADOW_OFFSET, sizeOffset);

				// Set shadow rainbow properties
				bool shadowUseRainbowGradient = m_shadowEffectType == EffectType.Rainbow;
				_materialRecord.SetFloat(PROP_SHADOW_USE_RAINBOW, shadowUseRainbowGradient ? 1 : 0);
				if (shadowUseRainbowGradient)
				{
					_materialRecord.SetFloat(PROP_GRAPHIC_RAINBOW_SATURATION, m_rainbowSaturation);
					_materialRecord.SetFloat(PROP_GRAPHIC_RAINBOW_VALUE, m_rainbowValue);
					_materialRecord.SetFloat(PROP_GRAPHIC_RAINBOW_HUE_OFFSET, m_rainbowHueOffset);
				}

				_materialRecord.EnableKeyword(KEYWORD_SHADOW);
			}
			else
				_materialRecord.DisableKeyword(KEYWORD_SHADOW);

			{
				var patternType = m_graphicEffectType;
				switch (patternType)
				{
					case EffectType.None:
						_materialRecord.DisableKeyword(KEYWORD_GRAPHIC_EFFECT_SHINY, KEYWORD_GRAPHIC_EFFECT_PATTERN);
						break;
					case EffectType.Shiny:
						_materialRecord.DisableKeyword(KEYWORD_GRAPHIC_EFFECT_PATTERN);
						_materialRecord.EnableKeyword(KEYWORD_GRAPHIC_EFFECT_SHINY);

						_materialRecord.SetFloat(PROP_GRAPHIC_EFFECT_SHINY_WIDTH, Mathf.PI * m_graphicEffectShinyWidth);
						_materialRecord.SetFloat(PROP_GRAPHIC_EFFECT_SHINY_BLUR, hminSize * m_graphicEffectShinyBlur);
						_materialRecord.SetVector(PROP_GRAPHIC_EFFECT_OFFSET, m_graphicEffectOffset);

						_materialRecord.SetFloat(PROP_GRAPHIC_EFFECT_ANGLE, Mathf.PI * m_graphicEffectAngle);
						_materialRecord.SetColor(PROP_GRAPHIC_EFFECT_COLOR, m_graphicEffectColor);
						break;
					case EffectType.Pattern:
						_materialRecord.DisableKeyword(KEYWORD_GRAPHIC_EFFECT_SHINY);
						_materialRecord.EnableKeyword(KEYWORD_GRAPHIC_EFFECT_PATTERN);

						_materialRecord.SetTexture(PROP_GRAPHIC_EFFECT_PATTERN_TEX, m_graphicEffectPatternTexture);
						_materialRecord.SetFloat(PROP_GRAPHIC_EFFECT_PATTERN_ROW, m_graphicEffectPatternRow);
						_materialRecord.SetFloat(PROP_GRAPHIC_EFFECT_PATTERN_SCROLL, m_graphicEffectPatternScroll);
						_materialRecord.SetVector(PROP_GRAPHIC_EFFECT_PATTERN_SCALE, m_graphicEffectPatternScale);
						_materialRecord.SetVector(PROP_GRAPHIC_EFFECT_PATTERN_PARAMS, new float4(m_graphicEffectPatternParamsX, m_graphicEffectPatternParamsY, m_graphicEffectPatternParamsZ, m_graphicEffectPatternParamsW));

						_materialRecord.SetFloat(PROP_GRAPHIC_EFFECT_ANGLE, Mathf.PI * m_graphicEffectAngle);
						_materialRecord.SetColor(PROP_GRAPHIC_EFFECT_COLOR, m_graphicEffectColor);
						_materialRecord.SetVector(PROP_GRAPHIC_EFFECT_OFFSET, m_graphicEffectOffset);
						break;
					case EffectType.Rainbow:
						_materialRecord.DisableKeyword(KEYWORD_GRAPHIC_EFFECT_SHINY, KEYWORD_GRAPHIC_EFFECT_PATTERN);
						
						// Set rainbow gradient properties
						_materialRecord.SetFloat(PROP_GRAPHIC_USE_RAINBOW, 1);
						_materialRecord.SetFloat(PROP_GRAPHIC_RAINBOW_SATURATION, m_rainbowSaturation);
						_materialRecord.SetFloat(PROP_GRAPHIC_RAINBOW_VALUE, m_rainbowValue);
						_materialRecord.SetFloat(PROP_GRAPHIC_RAINBOW_HUE_OFFSET, m_rainbowHueOffset);
						break;
				}
			}

			if (m_outline && m_outlineWidth > 0)
			{
				_materialRecord.SetFloat(PROP_OUTLINE_WIDTH, m_outlineWidth);
				_materialRecord.SetColor(PROP_OUTLINE_COLOR, m_outlineColor);
				_materialRecord.SetFloat(PROP_OUTLINE_INNER_BLUR, m_outlineInnerSoftness * m_outlineInnerSoftWidth);
				_materialRecord.SetFloat(PROP_OUTLINE_INNER_GAUSSIAN, (m_outlineInnerSoftness > 0) && (m_outlineInnerSoftWidth > 0) ? 1 : 0);

				_materialRecord.SetFloat(PROP_OUTLINE_GRADATION_ANGLE, m_outlineGradationAngle);
				_materialRecord.SetFloat(PROP_OUTLINE_GRADATION_SMOOTH, m_outlineGradationSmooth);
				_materialRecord.SetFloat(PROP_OUTLINE_GRADATION_RADIUS, m_outlineGradationRadius);
				_materialRecord.SetVector(PROP_OUTLINE_GRADATION_RANGE, m_outlineGradationRange);
				_materialRecord.SetVector(PROP_OUTLINE_GRADATION_OFFSET, m_outlineGradationOffset);

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

				var gradationShape = m_outlineGradationShape;
				switch (gradationShape)
				{
					case GradationShape.None:
						_materialRecord.SetColor(PROP_OUTLINE_GRADATION_COLOR, m_outlineColor);
						_materialRecord.SetVector(PROP_OUTLINE_GRADATION_LAYER, new float4(1, 0, 0, 0));
						break;
					case GradationShape.Linear:
						_materialRecord.SetColor(PROP_OUTLINE_GRADATION_COLOR, m_outlineGradationColor);
						_materialRecord.SetVector(PROP_OUTLINE_GRADATION_LAYER, new float4(0, 1, 0, 0));
						break;
					case GradationShape.Radial:
						_materialRecord.SetColor(PROP_OUTLINE_GRADATION_COLOR, m_outlineGradationColor);
						_materialRecord.SetVector(PROP_OUTLINE_GRADATION_LAYER, new float4(0, 0, 1, 0));
						break;
					case GradationShape.Conical:
						_materialRecord.SetColor(PROP_OUTLINE_GRADATION_COLOR, m_outlineGradationColor);
						_materialRecord.SetVector(PROP_OUTLINE_GRADATION_LAYER, new float4(0, 0, 0, 1));
						break;
				}

				var patternType = m_outlineEffectType;
				switch (patternType)
				{
					case EffectType.None:
						_materialRecord.DisableKeyword(KEYWORD_OUTLINE_EFFECT_SHINY, KEYWORD_OUTLINE_EFFECT_PATTERN);
						break;
					case EffectType.Shiny:
						_materialRecord.EnableKeyword(KEYWORD_OUTLINE_EFFECT_SHINY);
						_materialRecord.DisableKeyword(KEYWORD_OUTLINE_EFFECT_PATTERN);

						_materialRecord.SetFloat(PROP_OUTLINE_EFFECT_SHINY_WIDTH, Mathf.PI * m_outlineEffectShinyWidth);
						_materialRecord.SetFloat(PROP_OUTLINE_EFFECT_SHINY_BLUR, hminSize * m_outlineEffectShinyBlur);

						_materialRecord.SetFloat(PROP_OUTLINE_EFFECT_ANGLE, Mathf.PI * m_outlineEffectAngle);
						_materialRecord.SetColor(PROP_OUTLINE_EFFECT_COLOR, m_outlineEffectColor);
						_materialRecord.SetVector(PROP_OUTLINE_EFFECT_OFFSET, m_outlineEffectOffset);

						// Set outline rainbow properties
						bool outlineUseRainbowGradient = m_outlineEffectType == EffectType.Rainbow;
						_materialRecord.SetFloat(PROP_OUTLINE_USE_RAINBOW, outlineUseRainbowGradient ? 1 : 0);
						break;
					case EffectType.Pattern:
						_materialRecord.DisableKeyword(KEYWORD_OUTLINE_EFFECT_SHINY);
						_materialRecord.EnableKeyword(KEYWORD_OUTLINE_EFFECT_PATTERN);

						_materialRecord.SetTexture(PROP_OUTLINE_EFFECT_PATTERN_TEX, m_outlineEffectPatternTexture);
						_materialRecord.SetFloat(PROP_OUTLINE_EFFECT_PATTERN_ROW, m_outlineEffectPatternRow);
						_materialRecord.SetFloat(PROP_OUTLINE_EFFECT_PATTERN_SCROLL, m_outlineEffectPatternScroll);
						_materialRecord.SetVector(PROP_OUTLINE_EFFECT_PATTERN_SCALE, m_outlineEffectPatternScale);
						_materialRecord.SetVector(PROP_OUTLINE_EFFECT_PATTERN_PARAMS, new float4(m_outlineEffectPatternParamsX, m_outlineEffectPatternParamsY, m_outlineEffectPatternParamsZ, m_outlineEffectPatternParamsW));

						_materialRecord.SetFloat(PROP_OUTLINE_EFFECT_ANGLE, Mathf.PI * m_outlineEffectAngle);
						_materialRecord.SetColor(PROP_OUTLINE_EFFECT_COLOR, m_outlineEffectColor);
						_materialRecord.SetVector(PROP_OUTLINE_EFFECT_OFFSET, m_outlineEffectOffset);
						break;
					case EffectType.Rainbow:
						_materialRecord.DisableKeyword(KEYWORD_OUTLINE_EFFECT_SHINY, KEYWORD_OUTLINE_EFFECT_PATTERN);
						
						// Set outline rainbow properties
						_materialRecord.SetFloat(PROP_OUTLINE_USE_RAINBOW, 1);
						_materialRecord.SetFloat(PROP_GRAPHIC_RAINBOW_SATURATION, m_rainbowSaturation);
						_materialRecord.SetFloat(PROP_GRAPHIC_RAINBOW_VALUE, m_rainbowValue);
						_materialRecord.SetFloat(PROP_GRAPHIC_RAINBOW_HUE_OFFSET, m_rainbowHueOffset);
						break;
				}
			}
			else
			{
				_materialRecord.SetFloat(PROP_OUTLINE_WIDTH, 0);
				_materialRecord.SetColor(PROP_OUTLINE_COLOR, m_fillColor);
				_materialRecord.SetColor(PROP_OUTLINE_GRADATION_COLOR, m_fillColor);

				_materialRecord.SetFloat(PROP_SHADOW_BORDER, (m_shadowWidth - m_shadowDilate));
				_materialRecord.SetFloat(PROP_OUTLINE_BORDER, 0);
				_materialRecord.SetFloat(PROP_GRAPHIC_BORDER, 0);
			}

			var antialiasing = m_antialiasing is AntialiasingType.Default ? SDFUISettings.Instance.DefaultAA : m_antialiasing;
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
			_materialRecord.SetFloat(PROP_EULER_Z, eulerZ * Mathf.Deg2Rad);
		}

		public override void SetMaterialDirty()
		{
			base.SetMaterialDirty();

			if (!IsActive())
				return;

			materialDirty = true;

			UpdateMaterialRecord();
		}

		public float rainbowSaturation
		{
			get => m_rainbowSaturation;
			set
			{
				if (m_rainbowSaturation != value)
				{
					m_rainbowSaturation = Mathf.Clamp01(value);
					SetAllDirty();
				}
			}
		}

		public float rainbowValue
		{
			get => m_rainbowValue;
			set
			{
				if (m_rainbowValue != value)
				{
					m_rainbowValue = Mathf.Clamp01(value);
					SetAllDirty();
				}
			}
		}

		public float rainbowHueOffset
		{
			get => m_rainbowHueOffset;
			set
			{
				if (m_rainbowHueOffset != value)
				{
					m_rainbowHueOffset = Mathf.Clamp01(value);
					SetAllDirty();
				}
			}
		}

		// Add the property accessor for outline rainbow
		public bool outlineUseRainbow
		{
			get => m_outlineUseRainbow;
			set
			{
				if (m_outlineUseRainbow != value)
				{
					m_outlineUseRainbow = value;
					
					SetAllDirty();
				}
			}
		}
	}
}