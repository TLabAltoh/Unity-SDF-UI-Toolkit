using System.Collections.Generic;
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
        protected virtual string SHADER_TYPE { get; set; } = "Default";

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

        #region SHADER_PROPERTYS

        internal const string PREFIX_SHADOW = "Shadow";
        internal const string PREFIX_OUTLINE = "Outline";
        internal const string PREFIX_GRAPHIC = "Graphic";

        #region SHAPE

	internal static readonly int PROP_IS_WHITE_TEX_USED = Shader.PropertyToID("_IsWhiteTexUsed");
        internal static readonly int PROP_PADDING = Shader.PropertyToID("_Padding");
        internal static readonly int PROP_EULER_Z = Shader.PropertyToID("_EulerZ");
        internal static readonly int PROP_OUTERUV = Shader.PropertyToID("_OuterUV");
        internal static readonly int PROP_RECTSIZE = Shader.PropertyToID("_RectSize");
        internal static readonly int PROP_HALFSIZE = Shader.PropertyToID("_HalfSize");

        internal static readonly int PROP_ONION = Shader.PropertyToID("_Onion");
        internal static readonly int PROP_ONION_WIDTH = Shader.PropertyToID("_OnionWidth");

        #region SHADOW

        internal static readonly int PROP_SHADOW_BLUR = Shader.PropertyToID($"_{PREFIX_SHADOW}Blur");
        internal static readonly int PROP_SHADOW_WIDTH = Shader.PropertyToID($"_{PREFIX_SHADOW}Width");
        internal static readonly int PROP_SHADOW_DILATE = Shader.PropertyToID($"_{PREFIX_SHADOW}Dilate");
        internal static readonly int PROP_SHADOW_OFFSET = Shader.PropertyToID($"_{PREFIX_SHADOW}Offset");
        internal static readonly int PROP_SHADOW_BORDER = Shader.PropertyToID($"_{PREFIX_SHADOW}Border");
        internal static readonly int PROP_SHADOW_GAUSSIAN = Shader.PropertyToID($"_{PREFIX_SHADOW}Gaussian");

        #endregion SHADOW

        #region OUTLINE

        internal static readonly int PROP_OUTLINE_WIDTH = Shader.PropertyToID($"_{PREFIX_OUTLINE}Width");
        internal static readonly int PROP_OUTLINE_BORDER = Shader.PropertyToID($"_{PREFIX_OUTLINE}Border");
        internal static readonly int PROP_OUTLINE_INNER_BLUR = Shader.PropertyToID($"_{PREFIX_OUTLINE}InnerBlur");
        internal static readonly int PROP_OUTLINE_INNER_GAUSSIAN = Shader.PropertyToID($"_{PREFIX_OUTLINE}InnerGaussian");

        #endregion OUTLINE

        #region GRAPHIC

        internal static readonly int PROP_GRAPHIC_BORDER = Shader.PropertyToID($"_{PREFIX_GRAPHIC}Border");

        #endregion GRAPHIC

        #endregion SHAPE

        #region GRADATION

        internal const string PREFIX_GRADATION = "Gradation";

        #region GRAPHIC

        internal static readonly string PREFIX_GRAPHIC_GRADATION = $"{PREFIX_GRAPHIC}{PREFIX_GRADATION}";
        internal static readonly int PROP_GRAPHIC_GRADATION_COLOR = Shader.PropertyToID($"_{PREFIX_GRAPHIC_GRADATION}Color");
        internal static readonly int PROP_GRAPHIC_GRADATION_LAYER = Shader.PropertyToID($"_{PREFIX_GRAPHIC_GRADATION}Layer");
        internal static readonly int PROP_GRAPHIC_GRADATION_RANGE = Shader.PropertyToID($"_{PREFIX_GRAPHIC_GRADATION}Range");
        internal static readonly int PROP_GRAPHIC_GRADATION_ANGLE = Shader.PropertyToID($"_{PREFIX_GRAPHIC_GRADATION}Angle");
        internal static readonly int PROP_GRAPHIC_GRADATION_RADIUS = Shader.PropertyToID($"_{PREFIX_GRAPHIC_GRADATION}Radius");
        internal static readonly int PROP_GRAPHIC_GRADATION_SMOOTH = Shader.PropertyToID($"_{PREFIX_GRAPHIC_GRADATION}Smooth");
        internal static readonly int PROP_GRAPHIC_GRADATION_OFFSET = Shader.PropertyToID($"_{PREFIX_GRAPHIC_GRADATION}Offset");

        #endregion GRAPHIC

        #region OUTLINE

        internal static readonly string PREFIX_OUTLINE_GRADATION = $"{PREFIX_OUTLINE}{PREFIX_GRADATION}";
        internal static readonly int PROP_OUTLINE_GRADATION_COLOR = Shader.PropertyToID($"_{PREFIX_OUTLINE_GRADATION}Color");
        internal static readonly int PROP_OUTLINE_GRADATION_LAYER = Shader.PropertyToID($"_{PREFIX_OUTLINE_GRADATION}Layer");
        internal static readonly int PROP_OUTLINE_GRADATION_ANGLE = Shader.PropertyToID($"_{PREFIX_OUTLINE_GRADATION}Angle");
        internal static readonly int PROP_OUTLINE_GRADATION_RANGE = Shader.PropertyToID($"_{PREFIX_OUTLINE_GRADATION}Range");
        internal static readonly int PROP_OUTLINE_GRADATION_RADIUS = Shader.PropertyToID($"_{PREFIX_OUTLINE_GRADATION}Radius");
        internal static readonly int PROP_OUTLINE_GRADATION_SMOOTH = Shader.PropertyToID($"_{PREFIX_OUTLINE_GRADATION}Smooth");
        internal static readonly int PROP_OUTLINE_GRADATION_OFFSET = Shader.PropertyToID($"_{PREFIX_OUTLINE_GRADATION}Offset");

        #endregion OUTLINE

        #region SHADOW

        internal static readonly string PREFIX_SHADOW_GRADATION = $"{PREFIX_SHADOW}{PREFIX_GRADATION}";
        internal static readonly int PROP_SHADOW_GRADATION_COLOR = Shader.PropertyToID($"_{PREFIX_SHADOW_GRADATION}Color");
        internal static readonly int PROP_SHADOW_GRADATION_LAYER = Shader.PropertyToID($"_{PREFIX_SHADOW_GRADATION}Layer");
        internal static readonly int PROP_SHADOW_GRADATION_RANGE = Shader.PropertyToID($"_{PREFIX_SHADOW_GRADATION}Range");
        internal static readonly int PROP_SHADOW_GRADATION_ANGLE = Shader.PropertyToID($"_{PREFIX_SHADOW_GRADATION}Angle");
        internal static readonly int PROP_SHADOW_GRADATION_RADIUS = Shader.PropertyToID($"_{PREFIX_SHADOW_GRADATION}Radius");
        internal static readonly int PROP_SHADOW_GRADATION_SMOOTH = Shader.PropertyToID($"_{PREFIX_SHADOW_GRADATION}Smooth");
        internal static readonly int PROP_SHADOW_GRADATION_OFFSET = Shader.PropertyToID($"_{PREFIX_SHADOW_GRADATION}Offset");

        #endregion SHADOW

        #endregion GRADATION

        // Others
        internal static readonly int PROP_MAINTEX = Shader.PropertyToID("_MainTex");
        internal static readonly int PROP_SHADOW_COLOR = Shader.PropertyToID("_ShadowColor");
        internal static readonly int PROP_OUTLINE_TYPE = Shader.PropertyToID("_OutlineType");
        internal static readonly int PROP_OUTLINE_COLOR = Shader.PropertyToID("_OutlineColor");

        #region EFFECT

        private const string PREFIX_SHINY = "Shiny";
        private const string PREFIX_EFFECT = "Effect";
        private const string PREFIX_PATTERN = "Pattern";

        #region GRAPHIC

        internal static readonly string PREFIX_GRAPHIC_EFFECT = $"{PREFIX_GRAPHIC}{PREFIX_EFFECT}";
        internal static readonly int PROP_GRAPHIC_EFFECT_ANGLE = Shader.PropertyToID($"_{PREFIX_GRAPHIC_EFFECT}Angle");
        internal static readonly int PROP_GRAPHIC_EFFECT_COLOR = Shader.PropertyToID($"_{PREFIX_GRAPHIC_EFFECT}Color");
        internal static readonly int PROP_GRAPHIC_EFFECT_OFFSET = Shader.PropertyToID($"_{PREFIX_GRAPHIC_EFFECT}Offset");

        #region SHINY

        internal static readonly string PREFIX_GRAPHIC_EFFECT_SHINY = $"{PREFIX_GRAPHIC}{PREFIX_EFFECT}{PREFIX_SHINY}";
        internal static readonly int PROP_GRAPHIC_EFFECT_SHINY_BLUR = Shader.PropertyToID($"_{PREFIX_GRAPHIC_EFFECT_SHINY}Blur");
        internal static readonly int PROP_GRAPHIC_EFFECT_SHINY_WIDTH = Shader.PropertyToID($"_{PREFIX_GRAPHIC_EFFECT_SHINY}Width");

        #endregion SHINY

        #region PATTERN

        internal static readonly string PREFIX_GRAPHIC_EFFECT_PATTERN = $"{PREFIX_GRAPHIC}{PREFIX_EFFECT}{PREFIX_PATTERN}";
        internal static readonly int PROP_GRAPHIC_EFFECT_PATTERN_TEX = Shader.PropertyToID($"_{PREFIX_GRAPHIC_EFFECT_PATTERN}Tex");
        internal static readonly int PROP_GRAPHIC_EFFECT_PATTERN_ROW = Shader.PropertyToID($"_{PREFIX_GRAPHIC_EFFECT_PATTERN}Row");
        internal static readonly int PROP_GRAPHIC_EFFECT_PATTERN_SCALE = Shader.PropertyToID($"_{PREFIX_GRAPHIC_EFFECT_PATTERN}Scale");
        internal static readonly int PROP_GRAPHIC_EFFECT_PATTERN_SCROLL = Shader.PropertyToID($"_{PREFIX_GRAPHIC_EFFECT_PATTERN}Scroll");
        internal static readonly int PROP_GRAPHIC_EFFECT_PATTERN_PARAMS = Shader.PropertyToID($"_{PREFIX_GRAPHIC_EFFECT_PATTERN}Params");

        #endregion PATTERN

        #endregion GRAPHIC

        #region OUTLINE

        internal static readonly string PREFIX_OUTLINE_EFFECT = $"{PREFIX_OUTLINE}{PREFIX_EFFECT}";
        internal static readonly int PROP_OUTLINE_EFFECT_ANGLE = Shader.PropertyToID($"_{PREFIX_OUTLINE_EFFECT}Angle");
        internal static readonly int PROP_OUTLINE_EFFECT_COLOR = Shader.PropertyToID($"_{PREFIX_OUTLINE_EFFECT}Color");
        internal static readonly int PROP_OUTLINE_EFFECT_OFFSET = Shader.PropertyToID($"_{PREFIX_OUTLINE_EFFECT}Offset");

        #region SHINY

        internal static readonly string PREFIX_OUTLINE_EFFECT_SHINY = $"{PREFIX_OUTLINE}{PREFIX_EFFECT}{PREFIX_SHINY}";
        internal static readonly int PROP_OUTLINE_EFFECT_SHINY_BLUR = Shader.PropertyToID($"_{PREFIX_OUTLINE_EFFECT_SHINY}Blur");
        internal static readonly int PROP_OUTLINE_EFFECT_SHINY_WIDTH = Shader.PropertyToID($"_{PREFIX_OUTLINE_EFFECT_SHINY}Width");

        #endregion SHINY

        #region PATTERN

        internal static readonly string PREFIX_OUTLINE_EFFECT_PATTERN = $"{PREFIX_OUTLINE}{PREFIX_EFFECT}{PREFIX_PATTERN}";
        internal static readonly int PROP_OUTLINE_EFFECT_PATTERN_TEX = Shader.PropertyToID($"_{PREFIX_OUTLINE_EFFECT_PATTERN}Tex");
        internal static readonly int PROP_OUTLINE_EFFECT_PATTERN_ROW = Shader.PropertyToID($"_{PREFIX_OUTLINE_EFFECT_PATTERN}Row");
        internal static readonly int PROP_OUTLINE_EFFECT_PATTERN_SCALE = Shader.PropertyToID($"_{PREFIX_OUTLINE_EFFECT_PATTERN}Scale");
        internal static readonly int PROP_OUTLINE_EFFECT_PATTERN_SCROLL = Shader.PropertyToID($"_{PREFIX_OUTLINE_EFFECT_PATTERN}Scroll");
        internal static readonly int PROP_OUTLINE_EFFECT_PATTERN_PARAMS = Shader.PropertyToID($"_{PREFIX_OUTLINE_EFFECT_PATTERN}Params");

        #endregion PATTERN

        #endregion OUTLINE

        #endregion EFFECT

        #region RAINBOW

        internal const string PREFIX_RAINBOW = "Rainbow";

        internal static readonly string PREFIX_GRAPHIC_RAINBOW = $"{PREFIX_GRAPHIC}{PREFIX_RAINBOW}";
        internal static readonly string PREFIX_OUTLINE_RAINBOW = $"{PREFIX_OUTLINE}{PREFIX_RAINBOW}";
        internal static readonly string PREFIX_SHADOW_RAINBOW = $"{PREFIX_SHADOW}{PREFIX_RAINBOW}";

        internal static readonly int PROP_GRAPHIC_USE_RAINBOW = Shader.PropertyToID($"_{PREFIX_GRAPHIC}UseRainbow");
        internal static readonly int PROP_OUTLINE_USE_RAINBOW = Shader.PropertyToID($"_{PREFIX_OUTLINE}UseRainbow");
        internal static readonly int PROP_SHADOW_USE_RAINBOW = Shader.PropertyToID($"_{PREFIX_SHADOW}UseRainbow");

        internal static readonly int PROP_GRAPHIC_RAINBOW_VALUE = Shader.PropertyToID($"_{PREFIX_GRAPHIC_RAINBOW}Value");
        internal static readonly int PROP_GRAPHIC_RAINBOW_HUE_OFFSET = Shader.PropertyToID($"_{PREFIX_GRAPHIC_RAINBOW}HueOffset");
        internal static readonly int PROP_GRAPHIC_RAINBOW_SATURATION = Shader.PropertyToID($"_{PREFIX_GRAPHIC_RAINBOW}Saturation");

        #endregion RAINBOW

        #region LIQUID_GLASS

        internal const string PREFIX_LIQUID_GLASS = "LiquidGlass";

        internal static readonly int PROP_LIQUID_GLASS_OVERRIDE_MAINTEX = Shader.PropertyToID($"_{PREFIX_LIQUID_GLASS}OverrideMainTex");
        internal static readonly int PROP_LIQUID_GLASS_OFFSET = Shader.PropertyToID($"_{PREFIX_LIQUID_GLASS}Offset");
        internal static readonly int PROP_LIQUID_GLASS_INDEX = Shader.PropertyToID($"_{PREFIX_LIQUID_GLASS}Index");
        internal static readonly int PROP_LIQUID_GLASS_THICKNESS = Shader.PropertyToID($"_{PREFIX_LIQUID_GLASS}Thickness");
        internal static readonly int PROP_LIQUID_GLASS_BASE_HEIGHT = Shader.PropertyToID($"_{PREFIX_LIQUID_GLASS}BaseHeight");
        internal static readonly int PROP_LIQUID_GLASS_EDGE_REFLECT = Shader.PropertyToID($"_{PREFIX_LIQUID_GLASS}EdgeReflect");
        internal static readonly int PROP_LIQUID_GLASS_FLIP_BLUR_TEX_X = Shader.PropertyToID($"_{PREFIX_LIQUID_GLASS}FlipBlurTexX");
        internal static readonly int PROP_LIQUID_GLASS_FLIP_BLUR_TEX_Y = Shader.PropertyToID($"_{PREFIX_LIQUID_GLASS}FlipBlurTexY");
        public static readonly int PROP_LIQUID_GLASS_IS_POST_PROCESS_PASS = Shader.PropertyToID($"_{PREFIX_LIQUID_GLASS}IsPostProcessPass");

        #endregion LIQUID_GLASS

        #endregion SHADER_PROPERTYS

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

        public enum ProjectType
        {
            BIRP = 0,
            URP = 1,
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

        #region FIELD

        [SerializeField, LeftToggle] protected bool m_onion = false;
        [SerializeField, Min(0f)] protected float m_onionWidth = 10;

        [SerializeField] protected AntialiasingType m_antialiasing = AntialiasingType.Default;
        [SerializeField] protected bool m_useHDR = false;

        #region OUTLINE

        [SerializeField, LeftToggle] protected bool m_outline = true;
        [SerializeField, Min(0f)] protected float m_outlineWidth = 10;
        [SerializeField, Min(0f)] protected float m_outlineInnerSoftWidth = 0;
        [SerializeField, Range(0, 1)] protected float m_outlineInnerSoftness = 0.0f;
        [SerializeField] protected Color m_outlineColor = Color.cyan;
        [SerializeField] protected OutlineType m_outlineType = OutlineType.Inside;

        #region GRADATION
        [SerializeField] protected Color m_outlineGradationColor = Color.cyan;
        [SerializeField, Range(0, 2)] protected float m_outlineGradationAngle = 0;
        [SerializeField, Min(0)] protected float m_outlineGradationRadius = 0.5f;
        [SerializeField, Min(0)] protected float m_outlineGradationSmooth = 0.5f;
        [SerializeField, MinMaxRange(0, 1)] protected Vector2 m_outlineGradationRange = new Vector2(0.25f, 0.75f);
        [SerializeField] protected Vector2 m_outlineGradationOffset;
        [SerializeField] protected GradationShape m_outlineGradationShape = GradationShape.None;
        #endregion GRADATION

        #region EFFECT
        [SerializeField] protected EffectType m_outlineEffectType = EffectType.None;
        [SerializeField] protected Color m_outlineEffectColor = Color.white;
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
        [SerializeField] protected Color m_shadowColor = Color.black;
        [SerializeField] protected EffectType m_shadowEffectType = EffectType.None;

        #region GRADATION
        [SerializeField] protected Color m_shadowGradationColor = Color.black;
        [SerializeField, Range(0, 2)] protected float m_shadowGradationAngle = 0;
        [SerializeField, Min(0)] protected float m_shadowGradationRadius = 0.5f;
        [SerializeField, Min(0)] protected float m_shadowGradationSmooth = 0.5f;
        [SerializeField, MinMaxRange(0, 1)] protected Vector2 m_shadowGradationRange = new Vector2(0.25f, 0.75f);
        [SerializeField] protected Vector2 m_shadowGradationOffset;
        [SerializeField] protected GradationShape m_shadowGradationShape = GradationShape.None;
        #endregion GRADATION

        #endregion SHADOW

        #region GRAPHIC

        [SerializeField] protected Color m_fillColor = Color.white;
        [SerializeField] protected ActiveImageType m_activeImageType;
        [SerializeField] protected Sprite m_sprite;
        [SerializeField] protected Texture m_texture;
        [SerializeField] protected Rect m_uvRect = new Rect(0f, 0f, 1f, 1f);

        #region GRADATION
        [SerializeField] protected Color m_gradationColor = Color.white;
        [SerializeField, Range(0, 2)] protected float m_gradationAngle = 0;
        [SerializeField, Min(0)] protected float m_gradationRadius = 0.5f;
        [SerializeField, Min(0)] protected float m_gradationSmooth = 0.5f;
        [SerializeField, MinMaxRange(0, 1)] protected Vector2 m_gradationRange = new Vector2(0.25f, 0.75f);
        [SerializeField] protected Vector2 m_gradationOffset;
        [SerializeField] protected GradationShape m_gradationShape = GradationShape.None;
        #endregion GRADATION

        #region EFFECT
        [SerializeField] protected EffectType m_graphicEffectType = EffectType.None;
        [SerializeField] protected Color m_graphicEffectColor = Color.white;
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

        #region LIQUID_GLASS

        [SerializeField] protected bool m_liquidGlass = false;
        [SerializeField, Min(0)] protected float m_liquidGlassThickness = 10.0f;
        [SerializeField, Min(0)] protected float m_liquidGlassIndex = 1.5f;
        [SerializeField] protected bool m_liquidGlassOverrideMainTex = false;
        [SerializeField, Min(0)] protected float m_liquidGlassBlur;
        [SerializeField, Min(0)] protected float m_liquidGlassBlurOffset;
        // Added because the Y-axis of GrabTexture was inverted when setting the Camera target to RenderTexture in URP.
        [SerializeField] protected bool m_liquidGlassFlipBlurTexX = false;
        [SerializeField] protected bool m_liquidGlassFlipBlurTexY = false;
        [SerializeField, Min(1)] protected float m_liquidGlassEdgeReflect = 1;

        #endregion LIQUID_GLASS

        #endregion GRAPHIC

        #endregion FIELD

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
                var shadowWidth = m_shadow ? m_shadowWidth : 0;
                switch (m_outlineType)
                {
                    case OutlineType.Inside: return shadowWidth;
                    case OutlineType.Outside: return shadowWidth + (m_outline ? m_outlineWidth : 0);
                }
                return 0;
            }
        }

        #region PROPERTYS

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
                var tmp = Mathf.Clamp01(value);
                if (m_shadowGradationAngle != tmp)
                {
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
                var tmp = math.max(value, 0);
                if (m_shadowGradationRadius != tmp)
                {
                    m_shadowGradationRadius = tmp;

                    SetAllDirty();
                }
            }
        }

        public float shadowGradationSmooth
        {
            get => m_shadowGradationSmooth;
            set
            {
                var tmp = math.max(value, 0);
                if (m_shadowGradationSmooth != tmp)
                {
                    m_shadowGradationSmooth = tmp;

                    SetAllDirty();
                }
            }
        }

        public Vector2 shadowGradationRange
        {
            get => m_shadowGradationRange;
            set
            {
                var tmp = value;
                tmp.x = Mathf.Clamp01(tmp.x);
                tmp.y = Mathf.Clamp01(tmp.y);
                if (m_shadowGradationRange != tmp)
                {
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

        #endregion OUTLINE

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
                var tmp = Mathf.Clamp01(value);
                if (m_outlineGradationAngle != tmp)
                {
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
                var tmp = math.max(value, 0);
                if (m_outlineGradationRadius != tmp)
                {
                    m_outlineGradationRadius = tmp;

                    SetAllDirty();
                }
            }
        }

        public float outlineGradationSmooth
        {
            get => m_outlineGradationSmooth;
            set
            {
                var tmp = math.max(value, 0);
                if (m_outlineGradationSmooth != tmp)
                {
                    m_outlineGradationSmooth = tmp;

                    SetAllDirty();
                }
            }
        }

        public Vector2 outlineGradationRange
        {
            get => m_outlineGradationRange;
            set
            {
                var tmp = value;
                tmp.x = Mathf.Clamp01(tmp.x);
                tmp.y = Mathf.Clamp01(tmp.y);
                if (m_outlineGradationRange != tmp)
                {
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

        public bool useHDR
        {
            get => m_useHDR;
            set
            {
                if (m_useHDR != value)
                {
                    m_useHDR = value;

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
                var tmp = Mathf.Clamp01(value);
                if (m_gradationAngle != tmp)
                {
                    m_gradationAngle = tmp;

                    SetAllDirty();
                }
            }
        }

        public float gradationRadius
        {
            get => m_gradationRadius;
            set
            {
                var tmp = math.max(value, 0);
                if (m_gradationRadius != tmp)
                {
                    m_gradationRadius = tmp;

                    SetAllDirty();
                }
            }
        }

        public float gradationSmooth
        {
            get => m_gradationSmooth;
            set
            {
                var tmp = math.max(value, 0);
                if (m_gradationSmooth != tmp)
                {
                    m_gradationSmooth = tmp;

                    SetAllDirty();
                }
            }
        }

        public Vector2 gradationRange
        {
            get => m_gradationRange;
            set
            {
                var tmp = value;
                tmp.x = Mathf.Clamp01(tmp.x);
                tmp.y = Mathf.Clamp01(tmp.y);
                if (m_gradationRange != tmp)
                {
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
            set => base.material = value;
            get
            {
                if (m_Material != null)
                    return m_Material;
                return defaultMaterial;
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
            get => m_overrideSprite;
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
            get => m_overrideTexture;
            set
            {
                if (m_overrideTexture != value)
                {
                    m_overrideTexture = value;

                    SetAllDirty();
                }
            }
        }

        public Sprite activeSprite => m_overrideSprite != null ? m_overrideSprite : sprite;

        public Texture activeTexture => m_overrideTexture != null ? m_overrideTexture : texture;

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
                                    return _materialRecord.Texture;
                                return s_WhiteTexture;
                            }
                            return m_sprite.texture;
                        }
                    default: // ActiveImageType.Texture
                        {
                            if (m_texture == null)
                            {
                                if (_materialRecord != null && _materialRecord.Texture != null)
                                    return _materialRecord.Texture;
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

        #region RAINBOW

        public float rainbowSaturation
        {
            get => m_rainbowSaturation;
            set
            {
                var tmp = Mathf.Clamp01(value);
                if (m_rainbowSaturation != tmp)
                {
                    m_rainbowSaturation = tmp;

                    SetAllDirty();
                }
            }
        }

        public float rainbowValue
        {
            get => m_rainbowValue;
            set
            {
                var tmp = Mathf.Clamp01(value);
                if (m_rainbowValue != tmp)
                {
                    m_rainbowValue = tmp;

                    SetAllDirty();
                }
            }
        }

        public float rainbowHueOffset
        {
            get => m_rainbowHueOffset;
            set
            {
                var tmp = Mathf.Clamp01(value);
                if (m_rainbowHueOffset != tmp)
                {
                    m_rainbowHueOffset = tmp;

                    SetAllDirty();
                }
            }
        }

        #endregion RAINBOW

        #region LIQUID_GLASS

        public bool liquidGlass
        {
            get => m_liquidGlass;
            set
            {
                if (m_liquidGlass != value)
                {
                    m_liquidGlass = value;

                    SetAllDirty();
                }
            }
        }

        public float liquidGlassBlur
        {
            get => m_liquidGlassBlur;
            set
            {
                var tmp = Mathf.Max(value, 0);
                if (m_liquidGlassBlur != tmp)
                {
                    m_liquidGlassBlur = tmp;

                    SetAllDirty();
                }
            }
        }

        public bool liquidGlassFlipBlurTexX
        {
            get => m_liquidGlassFlipBlurTexX;
            set
            {
                if (m_liquidGlassFlipBlurTexX != value)
                {
                    m_liquidGlassFlipBlurTexX = value;

                    SetAllDirty();
                }
            }
        }

        public bool liquidGlassFlipBlurTexY
        {
            get => m_liquidGlassFlipBlurTexY;
            set
            {
                if (m_liquidGlassFlipBlurTexY != value)
                {
                    m_liquidGlassFlipBlurTexY = value;

                    SetAllDirty();
                }
            }
        }

        public float liquidGlassBlurOffset
        {
            get => m_liquidGlassBlurOffset;
            set
            {
                var tmp = Mathf.Max(value, 0);
                if (m_liquidGlassBlurOffset != tmp)
                {
                    m_liquidGlassBlurOffset = tmp;

                    SetAllDirty();
                }
            }
        }

        public float liquidGlassEdgeReflect
        {
            get => m_liquidGlassEdgeReflect;
            set
            {
                var tmp = Mathf.Max(value, 1);
                if (m_liquidGlassEdgeReflect != value)
                {
                    m_liquidGlassEdgeReflect = value;

                    SetAllDirty();
                }
            }
        }

        public float liquidGlassThickness
        {
            get => m_liquidGlassThickness;
            set
            {
                var tmp = Mathf.Clamp(value, 0, float.MaxValue);
                if (m_liquidGlassThickness != tmp)
                {
                    m_liquidGlassThickness = tmp;

                    SetAllDirty();
                }
            }
        }

        public float liquidGlassIndex
        {
            get => m_liquidGlassIndex;
            set
            {
                var tmp = Mathf.Clamp(value, 0, float.MaxValue);
                if (m_liquidGlassIndex != tmp)
                {
                    m_liquidGlassIndex = tmp;

                    SetAllDirty();
                }
            }
        }

        public bool liquidGlassOverrideMainTex
        {
            get => m_liquidGlassOverrideMainTex;
            set
            {
                if (m_liquidGlassOverrideMainTex != value)
                {
                    m_liquidGlassOverrideMainTex = value;

                    SetAllDirty();
                }
            }
        }

        #endregion LIQUID_GLASS

        #endregion PROPERTYS

        internal MaterialRecord MaterialRecord => (MaterialRecord)_materialRecord.Clone();
        private protected MaterialRecord _materialRecord { get; } = new();

        protected bool materialDirty;

        protected static readonly HashSet<SDFUI> m_blurTargets = new HashSet<SDFUI>();
        protected static readonly HashSet<Mesh> m_blurTargetMeshPool = new HashSet<Mesh>();

        public static IEnumerable<SDFUI> blurTargets => m_blurTargets;
        public static IEnumerable<Mesh> blurTargetMeshPool => m_blurTargetMeshPool;

        public static void ClearBlurTargetRegistry()
        {
            m_blurTargets.Clear();
            m_blurTargetMeshPool.Clear();
        }

        protected static void AddBlurTargetRegistry(SDFUI blurTarget)
        {
            if (m_blurTargets.Add(blurTarget))
                m_blurTargetMeshPool.Add(new Mesh());
        }

        protected static void RemoveBlurTargetRegistry(SDFUI blurTarget)
        {
            if (m_blurTargets.Remove(blurTarget))
                for (int i = 0; i < m_blurTargets.Count; i++)
                    m_blurTargetMeshPool.Add(new Mesh());
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

            ClearBlurTargetRegistry();

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

            ClearBlurTargetRegistry();

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
            m_useHDR = SDFUISettings.Instance.UseHDR;
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
            _materialRecord.ResetKeywords();

            var minSize = this.minSize;
            var maxSize = this.maxSize;

            var hminSize = minSize * .5f;
            var hmaxSize = maxSize * .5f;

            if (m_liquidGlass)
            {
                SHADER_TYPE = "LiquidGlass";
                AddBlurTargetRegistry(this);
                if (SDFUISettings.Instance.ProjectType == ProjectType.URP)
                    _materialRecord.SetFloat(PROP_LIQUID_GLASS_IS_POST_PROCESS_PASS, 0);
                else
                    _materialRecord.SetFloat(PROP_LIQUID_GLASS_IS_POST_PROCESS_PASS, 1);
                _materialRecord.SetFloat(PROP_LIQUID_GLASS_INDEX, m_liquidGlassIndex);
                _materialRecord.SetFloat(PROP_LIQUID_GLASS_THICKNESS, m_liquidGlassThickness);
                _materialRecord.SetFloat(PROP_LIQUID_GLASS_BASE_HEIGHT, m_liquidGlassThickness * 8.0f);
                _materialRecord.SetFloat(PROP_LIQUID_GLASS_EDGE_REFLECT, m_liquidGlassEdgeReflect);
                _materialRecord.SetFloat(PROP_LIQUID_GLASS_FLIP_BLUR_TEX_X, m_liquidGlassFlipBlurTexX ? 1.0f : 0.0f);
                _materialRecord.SetFloat(PROP_LIQUID_GLASS_FLIP_BLUR_TEX_Y, m_liquidGlassFlipBlurTexY ? 1.0f : 0.0f);
                _materialRecord.SetFloat(PROP_LIQUID_GLASS_OVERRIDE_MAINTEX, m_liquidGlassOverrideMainTex ? 1.0f : 0.0f);
            }
            else
            {
                SHADER_TYPE = "Default";
                RemoveBlurTargetRegistry(this);
            }

            _materialRecord.ShaderName = SHADER_NAME;

            _materialRecord.SetVector(PROP_RECTSIZE, new float4(((RectTransform)transform).rect.size, 0, 0));

            _materialRecord.TextureUV = new float4(uvRect.x, uvRect.y, uvRect.size.x, uvRect.size.y);
            _materialRecord.TextureColor = m_useHDR ? m_fillColor.gamma : m_fillColor;

	        Texture texture;
	        float isWhiteTexUsed;

            var activeImageType = m_activeImageType;
            switch (activeImageType)
            {
                case ActiveImageType.Sprite:
                    {
                        var activeSprite = this.activeSprite;
                        float4 outerUV;
                        if (activeSprite == null)
                        {
                            isWhiteTexUsed = 1.0f;
                            texture = s_WhiteTexture;
                            outerUV = defaultOuterUV;
                        }
                        else
                        {
                            isWhiteTexUsed = 0.0f;
                            texture = activeSprite.texture;
                            outerUV = DataUtility.GetOuterUV(activeSprite);
                        }
                        _materialRecord.Texture = texture;
                        _materialRecord.SetVector(PROP_OUTERUV, outerUV);
	                _materialRecord.SetFloat(PROP_IS_WHITE_TEX_USED, isWhiteTexUsed);
                    }
                    break;
                case ActiveImageType.Texture:
                    {
                        var activeTexture = this.activeTexture;
                        if (activeTexture == null) {
                        	isWhiteTexUsed = 1.0f;
                        	texture = s_WhiteTexture;
                        	
                        } else {
                        	isWhiteTexUsed = 0.0f;
                        	texture = activeTexture;
                        }
	                _materialRecord.Texture = texture;
                        _materialRecord.SetVector(PROP_OUTERUV, defaultOuterUV);
	                _materialRecord.SetFloat(PROP_IS_WHITE_TEX_USED, isWhiteTexUsed);
                    }
                    break;
            }

            {
                var gradationShape = m_gradationShape;
                var gradationColor = m_gradationColor;
                var gradationLayer = new float4(0, 0, 0, 0);
                switch (gradationShape)
                {
                    case GradationShape.None:
                        gradationColor = m_fillColor;
                        gradationLayer.x = 1;
                        break;
                    case GradationShape.Linear:
                        gradationLayer.y = 1;
                        break;
                    case GradationShape.Radial:
                        gradationLayer.z = 1;
                        break;
                    case GradationShape.Conical:
                        gradationLayer.w = 1;
                        break;
                }

                _materialRecord.SetColor(PROP_GRAPHIC_GRADATION_COLOR, m_useHDR ? gradationColor.gamma : gradationColor);
                _materialRecord.SetVector(PROP_GRAPHIC_GRADATION_LAYER, gradationLayer);

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

            {
                var enabled = 0;
                var width = 0f;
                if (m_onion)
                {
                    enabled = 1;
                    width = m_onionWidth;
                }
                _materialRecord.SetFloat(PROP_ONION, enabled);
                _materialRecord.SetFloat(PROP_ONION_WIDTH, width);
            }

            if (m_shadow)
            {
                _materialRecord.SetFloat(PROP_SHADOW_WIDTH, m_shadowWidth);
                _materialRecord.SetFloat(PROP_SHADOW_BLUR, m_shadowSoftness * (m_shadowWidth + m_shadowInnerSoftWidth));
                _materialRecord.SetFloat(PROP_SHADOW_DILATE, m_shadowDilate);
                _materialRecord.SetColor(PROP_SHADOW_COLOR, m_useHDR ? m_shadowColor.gamma : m_shadowColor);
                _materialRecord.SetFloat(PROP_SHADOW_GAUSSIAN, (m_shadowSoftness > 0) ? 1 : 0);

                _materialRecord.SetFloat(PROP_SHADOW_GRADATION_ANGLE, m_shadowGradationAngle);
                _materialRecord.SetFloat(PROP_SHADOW_GRADATION_SMOOTH, m_shadowGradationSmooth);
                _materialRecord.SetFloat(PROP_SHADOW_GRADATION_RADIUS, m_shadowGradationRadius);
                _materialRecord.SetVector(PROP_SHADOW_GRADATION_RANGE, m_shadowGradationRange);
                _materialRecord.SetVector(PROP_SHADOW_GRADATION_OFFSET, m_shadowGradationOffset);

                var gradationShape = m_shadowGradationShape;
                var gradationColor = m_shadowGradationColor;
                var gradationLayer = new float4(0, 0, 0, 0);
                switch (gradationShape)
                {
                    case GradationShape.None:
                        gradationColor = m_shadowColor;
                        gradationLayer.x = 1;
                        break;
                    case GradationShape.Linear:
                        gradationLayer.y = 1;
                        break;
                    case GradationShape.Radial:
                        gradationLayer.z = 1;
                        break;
                    case GradationShape.Conical:
                        gradationLayer.w = 1;
                        break;
                }
                _materialRecord.SetColor(PROP_SHADOW_GRADATION_COLOR, m_useHDR ? gradationColor.gamma : gradationColor);
                _materialRecord.SetVector(PROP_SHADOW_GRADATION_LAYER, gradationLayer);

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
            }
            _materialRecord.SetKeywordActive(KEYWORD_SHADOW, m_shadow);

            {
                var patternType = m_graphicEffectType;
                switch (patternType)
                {
                    case EffectType.None:
                        _materialRecord.DisableKeyword(KEYWORD_GRAPHIC_EFFECT_SHINY, KEYWORD_GRAPHIC_EFFECT_PATTERN);
                        break;
                    case EffectType.Shiny:
                        _materialRecord.SetKeywordActive(KEYWORD_GRAPHIC_EFFECT_PATTERN, false);
                        _materialRecord.SetKeywordActive(KEYWORD_GRAPHIC_EFFECT_SHINY, true);

                        _materialRecord.SetFloat(PROP_GRAPHIC_EFFECT_SHINY_WIDTH, Mathf.PI * m_graphicEffectShinyWidth);
                        _materialRecord.SetFloat(PROP_GRAPHIC_EFFECT_SHINY_BLUR, hminSize * m_graphicEffectShinyBlur);
                        _materialRecord.SetVector(PROP_GRAPHIC_EFFECT_OFFSET, m_graphicEffectOffset);

                        _materialRecord.SetFloat(PROP_GRAPHIC_EFFECT_ANGLE, Mathf.PI * m_graphicEffectAngle);
                        _materialRecord.SetColor(PROP_GRAPHIC_EFFECT_COLOR, m_useHDR ? m_graphicEffectColor.gamma : m_graphicEffectColor);
                        break;
                    case EffectType.Pattern:
                        _materialRecord.SetKeywordActive(KEYWORD_GRAPHIC_EFFECT_SHINY, false);
                        _materialRecord.SetKeywordActive(KEYWORD_GRAPHIC_EFFECT_PATTERN, true);

                        _materialRecord.SetTexture(PROP_GRAPHIC_EFFECT_PATTERN_TEX, m_graphicEffectPatternTexture);
                        _materialRecord.SetFloat(PROP_GRAPHIC_EFFECT_PATTERN_ROW, m_graphicEffectPatternRow);
                        _materialRecord.SetFloat(PROP_GRAPHIC_EFFECT_PATTERN_SCROLL, m_graphicEffectPatternScroll);
                        _materialRecord.SetVector(PROP_GRAPHIC_EFFECT_PATTERN_SCALE, m_graphicEffectPatternScale);
                        _materialRecord.SetVector(PROP_GRAPHIC_EFFECT_PATTERN_PARAMS, new float4(m_graphicEffectPatternParamsX, m_graphicEffectPatternParamsY, m_graphicEffectPatternParamsZ, m_graphicEffectPatternParamsW));

                        _materialRecord.SetFloat(PROP_GRAPHIC_EFFECT_ANGLE, Mathf.PI * m_graphicEffectAngle);
                        _materialRecord.SetColor(PROP_GRAPHIC_EFFECT_COLOR, m_useHDR ? m_graphicEffectColor.gamma : m_graphicEffectColor);
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

            if (m_outline && (m_outlineWidth > 0))
            {
                _materialRecord.SetFloat(PROP_OUTLINE_WIDTH, m_outlineWidth);
                _materialRecord.SetColor(PROP_OUTLINE_COLOR, m_useHDR ? m_outlineColor.gamma : m_outlineColor);
                _materialRecord.SetFloat(PROP_OUTLINE_INNER_BLUR, m_outlineInnerSoftness * m_outlineInnerSoftWidth);
                _materialRecord.SetFloat(PROP_OUTLINE_INNER_GAUSSIAN, (m_outlineInnerSoftness > 0) && (m_outlineInnerSoftWidth > 0) ? 1 : 0);

                _materialRecord.SetFloat(PROP_OUTLINE_GRADATION_ANGLE, m_outlineGradationAngle);
                _materialRecord.SetFloat(PROP_OUTLINE_GRADATION_SMOOTH, m_outlineGradationSmooth);
                _materialRecord.SetFloat(PROP_OUTLINE_GRADATION_RADIUS, m_outlineGradationRadius);
                _materialRecord.SetVector(PROP_OUTLINE_GRADATION_RANGE, m_outlineGradationRange);
                _materialRecord.SetVector(PROP_OUTLINE_GRADATION_OFFSET, m_outlineGradationOffset);

                var outlineType = m_outlineType;
                float shadowBorder, outlineBorder = 0, graphicBorder = 0;
                switch (outlineType)
                {
                    case OutlineType.Inside:
                        shadowBorder = m_shadowWidth - m_shadowDilate;
                        graphicBorder = -m_outlineWidth;
                        break;
                    default: // case OutlineType.Outside:
                        shadowBorder = (m_shadowWidth - m_shadowDilate) + m_outlineWidth;
                        outlineBorder = m_outlineWidth;
                        break;
                }
                _materialRecord.SetFloat(PROP_SHADOW_BORDER, shadowBorder);
                _materialRecord.SetFloat(PROP_OUTLINE_BORDER, outlineBorder);
                _materialRecord.SetFloat(PROP_GRAPHIC_BORDER, graphicBorder);

                var gradationShape = m_outlineGradationShape;
                var gradationColor = m_outlineGradationColor;
                var gradationLayer = new float4(0, 0, 0, 0);
                switch (gradationShape)
                {
                    case GradationShape.None:
                        gradationColor = m_outlineColor;
                        gradationLayer.x = 1;
                        break;
                    case GradationShape.Linear:
                        gradationLayer.y = 1;
                        break;
                    case GradationShape.Radial:
                        gradationLayer.z = 1;
                        break;
                    case GradationShape.Conical:
                        gradationLayer.w = 1;
                        break;
                }
                _materialRecord.SetColor(PROP_OUTLINE_GRADATION_COLOR, m_useHDR ? gradationColor.gamma : gradationColor);
                _materialRecord.SetVector(PROP_OUTLINE_GRADATION_LAYER, gradationLayer);

                var patternType = m_outlineEffectType;
                switch (patternType)
                {
                    case EffectType.None:
                        _materialRecord.DisableKeyword(KEYWORD_OUTLINE_EFFECT_SHINY, KEYWORD_OUTLINE_EFFECT_PATTERN);
                        break;
                    case EffectType.Shiny:
                        _materialRecord.SetKeywordActive(KEYWORD_OUTLINE_EFFECT_SHINY, true);
                        _materialRecord.SetKeywordActive(KEYWORD_OUTLINE_EFFECT_PATTERN, false);

                        _materialRecord.SetFloat(PROP_OUTLINE_EFFECT_SHINY_WIDTH, Mathf.PI * m_outlineEffectShinyWidth);
                        _materialRecord.SetFloat(PROP_OUTLINE_EFFECT_SHINY_BLUR, hminSize * m_outlineEffectShinyBlur);

                        _materialRecord.SetFloat(PROP_OUTLINE_EFFECT_ANGLE, Mathf.PI * m_outlineEffectAngle);
                        _materialRecord.SetColor(PROP_OUTLINE_EFFECT_COLOR, m_useHDR ? m_outlineEffectColor.gamma : m_outlineEffectColor);
                        _materialRecord.SetVector(PROP_OUTLINE_EFFECT_OFFSET, m_outlineEffectOffset);

                        // Set outline rainbow properties
                        bool outlineUseRainbowGradient = m_outlineEffectType == EffectType.Rainbow;
                        _materialRecord.SetFloat(PROP_OUTLINE_USE_RAINBOW, outlineUseRainbowGradient ? 1 : 0);
                        break;
                    case EffectType.Pattern:
                        _materialRecord.SetKeywordActive(KEYWORD_OUTLINE_EFFECT_SHINY, false);
                        _materialRecord.SetKeywordActive(KEYWORD_OUTLINE_EFFECT_PATTERN, true);

                        _materialRecord.SetTexture(PROP_OUTLINE_EFFECT_PATTERN_TEX, m_outlineEffectPatternTexture);
                        _materialRecord.SetFloat(PROP_OUTLINE_EFFECT_PATTERN_ROW, m_outlineEffectPatternRow);
                        _materialRecord.SetFloat(PROP_OUTLINE_EFFECT_PATTERN_SCROLL, m_outlineEffectPatternScroll);
                        _materialRecord.SetVector(PROP_OUTLINE_EFFECT_PATTERN_SCALE, m_outlineEffectPatternScale);
                        _materialRecord.SetVector(PROP_OUTLINE_EFFECT_PATTERN_PARAMS, new float4(m_outlineEffectPatternParamsX, m_outlineEffectPatternParamsY, m_outlineEffectPatternParamsZ, m_outlineEffectPatternParamsW));

                        _materialRecord.SetFloat(PROP_OUTLINE_EFFECT_ANGLE, Mathf.PI * m_outlineEffectAngle);
                        _materialRecord.SetColor(PROP_OUTLINE_EFFECT_COLOR, m_useHDR ? m_outlineEffectColor.gamma : m_outlineEffectColor);
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
                _materialRecord.SetColor(PROP_OUTLINE_COLOR, m_useHDR ? m_fillColor.gamma : m_fillColor);
                _materialRecord.SetColor(PROP_OUTLINE_GRADATION_COLOR, m_useHDR ? m_fillColor.gamma : m_fillColor);

                _materialRecord.SetFloat(PROP_SHADOW_BORDER, (m_shadowWidth - m_shadowDilate));
                _materialRecord.SetFloat(PROP_OUTLINE_BORDER, 0);
                _materialRecord.SetFloat(PROP_GRAPHIC_BORDER, 0);
            }

            var antialiasing = m_antialiasing is AntialiasingType.Default ? SDFUISettings.Instance.DefaultAA : m_antialiasing;
            _materialRecord.SetKeywordActive(KEYWORD_AA, antialiasing == AntialiasingType.ON);

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
    }
}
