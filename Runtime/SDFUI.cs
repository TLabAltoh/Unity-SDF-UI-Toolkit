using TLab.UI.SDF.Registry;
using Unity.Burst;
using Unity.Mathematics;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Sprites;
using UnityEngine.Pool;

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
		protected virtual string SHADER_NAME => "";

		internal static readonly int PROP_HALFSIZE = Shader.PropertyToID("_HalfSize");
		internal static readonly int PROP_PADDING = Shader.PropertyToID("_Padding");
		internal static readonly int PROP_OUTERUV = Shader.PropertyToID("_OuterUV");
		internal static readonly int PROP_RECTSIZE = Shader.PropertyToID("_RectSize");
		internal static readonly int PROP_ANTIALIASING = Shader.PropertyToID("_Antialiasing");

		internal static readonly int PROP_ONION = Shader.PropertyToID("_Onion");
		internal static readonly int PROP_ONIONWIDTH = Shader.PropertyToID("_OnionWidth");

		internal static readonly int PROP_MAINTEX = Shader.PropertyToID("_MainTex");

		internal static readonly int PROP_SHADOWWIDTH = Shader.PropertyToID("_ShadowWidth");
		internal static readonly int PROP_SHADOWBLUR = Shader.PropertyToID("_ShadowBlur");
		internal static readonly int PROP_SHADOWPOWER = Shader.PropertyToID("_ShadowPower");
		internal static readonly int PROP_SHADOWCOLOR = Shader.PropertyToID("_ShadowColor");
		internal static readonly int PROP_SHADOWOFFSET = Shader.PropertyToID("_ShadowOffset");

		internal static readonly int PROP_OUTLINETYPE = Shader.PropertyToID("_OutlineType");
		internal static readonly int PROP_OUTLINECOLOR = Shader.PropertyToID("_OutlineColor");
		internal static readonly int PROP_OUTLINEWIDTH = Shader.PropertyToID("_OutlineWidth");

		public enum OutlineType
		{
			INSIDE,
			OUTSIDE
		};

		public enum ActiveImageType
		{
			SPRITE,
			TEXTURE
		};

		[SerializeField] protected bool m_onion = false;
		[SerializeField, Min(0f)] protected float m_onionWidth = 10;

		[SerializeField] protected bool m_antialiasing = true;

		[SerializeField] protected bool m_shadow = false;
		[SerializeField, Min(0f)] protected float m_shadowWidth = 10;
		[SerializeField, Min(0f)] protected float m_shadowBlur = 0f;
		[SerializeField, Min(0f)] protected float m_shadowPower = 1f;
		[SerializeField] protected Vector2 m_shadowOffset;
		[SerializeField] protected Color m_shadowColor = Color.black;

		[SerializeField] protected bool m_outline = true;
		[SerializeField, Min(0f)] protected float m_outlineWidth = 10;
		[SerializeField] protected Color m_outlineColor = new Color(0.0f, 1.0f, 1.0f, 1.0f);
		[SerializeField] protected OutlineType m_outlineType = OutlineType.INSIDE;

		[SerializeField] protected Color m_fillColor = Color.white;

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

		public bool antialiasing
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
					case ActiveImageType.SPRITE:
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
					default: // ActiveImageType.TEXTURE
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

				if (MaskEnabled())
                {
					m_mask.GetModifiedMaterial(material);
                }
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

			SDFUtils.CalculateVertexes(rectTransform.rect.size, rectTransform.pivot, m_extraMargin, shadowOffset, rectTransform.eulerAngles.z, m_antialiasing,
				out var vertex0, out var vertex1, out var vertex2, out var vertex3);

			var color32 = color;
			vh.AddVert(vertex0.position, color32, new Vector4(vertex0.uv.x, vertex0.uv.y));
			vh.AddVert(vertex1.position, color32, new Vector4(vertex1.uv.x, vertex1.uv.y));
			vh.AddVert(vertex2.position, color32, new Vector4(vertex2.uv.x, vertex2.uv.y));
			vh.AddVert(vertex3.position, color32, new Vector4(vertex3.uv.x, vertex3.uv.y));

			vh.AddTriangle(0, 1, 2);
			vh.AddTriangle(2, 3, 0);
		}

		public override Material materialForRendering
		{
			get
			{
				var currentMat = base.materialForRendering;

				_materialRecord.Populate(currentMat);

				return currentMat;
			}
		}

		public override void SetMaterialDirty()
		{
			base.SetMaterialDirty();

			materialDirty = true;

			_materialRecord.ShaderName = SHADER_NAME;

			_materialRecord.SetVector(PROP_RECTSIZE, new float4(((RectTransform)transform).rect.size, 0, 0));

			_materialRecord.TextureUV = new float4(uvRect.x, uvRect.y, uvRect.size.x, uvRect.size.y);
			_materialRecord.TextureColor = m_fillColor;

			switch (m_activeImageType)
			{
				case ActiveImageType.SPRITE:
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
				case ActiveImageType.TEXTURE:
					{
						var activeTexture = this.activeTexture;
						_materialRecord.Texture = (activeTexture == null) ? s_WhiteTexture : activeTexture;
						_materialRecord.SetVector(PROP_OUTERUV, defaultOuterUV);
					}
					break;
			}

			if (m_onion)
			{
				_materialRecord.SetInteger(PROP_ONION, 1);
				_materialRecord.SetFloat(PROP_ONIONWIDTH, m_onionWidth);
			}
			else
			{
				_materialRecord.SetInteger(PROP_ONION, 0);
				_materialRecord.SetFloat(PROP_ONIONWIDTH, 0);
			}

			float shadowWidth = m_shadowWidth;

			if (m_shadow)
			{
				_materialRecord.SetFloat(PROP_SHADOWWIDTH, shadowWidth);
				_materialRecord.SetColor(PROP_SHADOWCOLOR, m_shadowColor);
			}
			else
			{
				shadowWidth = 0;
				_materialRecord.SetFloat(PROP_SHADOWWIDTH, shadowWidth);
				_materialRecord.SetColor(PROP_SHADOWCOLOR, alpha0);
			}

			_materialRecord.SetFloat(PROP_SHADOWBLUR, m_shadowBlur);
			_materialRecord.SetFloat(PROP_SHADOWPOWER, m_shadowPower);
			SDFUtils.ShadowSizeOffset(rectTransform.rect.size, m_shadowOffset, rectTransform.eulerAngles.z, out float4 sizeOffset);
			_materialRecord.SetVector(PROP_SHADOWOFFSET, sizeOffset);

			_materialRecord.SetInteger(PROP_OUTLINETYPE, (int)m_outlineType);

			float outlineWidth = m_outlineWidth;
			if (m_outline)
			{
				_materialRecord.SetFloat(PROP_OUTLINEWIDTH, outlineWidth);
				_materialRecord.SetColor(PROP_OUTLINECOLOR, m_outlineColor);
			}
			else
			{
				outlineWidth = 0;
				_materialRecord.SetFloat(PROP_OUTLINEWIDTH, outlineWidth);
				_materialRecord.SetColor(PROP_OUTLINECOLOR, m_fillColor);
			}

			_materialRecord.SetInteger(PROP_ANTIALIASING, m_antialiasing ? 1 : 0);

			_materialRecord.SetFloat(PROP_PADDING, m_extraMargin);
		}
	}

	[BurstCompile]
	public static class SDFUtils
	{
		[BurstCompile]
		public static void CalculateVertexes(in float2 rectSize, in float2 rectPivot, in float margin, in float2 shadowOffset, in float rotation, in bool antialiasing,
			out VertexData vertex0, out VertexData vertex1, out VertexData vertex2, out VertexData vertex3)
		{
			float3 pivotPoint = new(rectSize * rectPivot, 0);
			float4 shadowExpand = float4.zero;

			RotateVector(shadowOffset, rotation, out float2 rotatedOffset);

			if (rotatedOffset.x < 0)
			{
				shadowExpand.x = rotatedOffset.x;
				shadowExpand.y = 0;
			}
			else
			{
				shadowExpand.x = 0;
				shadowExpand.y = rotatedOffset.x;
			}

			if (rotatedOffset.y < 0)
			{
				shadowExpand.z = rotatedOffset.y;
				shadowExpand.w = 0;
			}
			else
			{
				shadowExpand.z = 0;
				shadowExpand.w = rotatedOffset.y;
			}

			float4 expand = shadowExpand;

			if (antialiasing && rectSize.x > 0 && rectSize.y > 0)
			{
				expand += new float4(-1, 1, -1, 1);

			}

			float scaleX = math.mad(2, margin, rectSize.x);
			float scaleY = math.mad(2, margin, rectSize.y);
			float4 uvExpand = new(expand.x / scaleX, expand.y / scaleX, expand.z / scaleY, expand.w / scaleY);

			vertex0 = new VertexData();
			vertex0.position = new float3(expand.x - margin, expand.z - margin, 0) - pivotPoint;
			vertex0.uv = new float2(uvExpand.x, uvExpand.z);

			vertex1 = new VertexData();
			vertex1.position = new float3(expand.x - margin, expand.w + margin + rectSize.y, 0) - pivotPoint;
			vertex1.uv = new float2(uvExpand.x, 1 + uvExpand.w);

			vertex2 = new VertexData();
			vertex2.position = new float3(expand.y + margin + rectSize.x, expand.w + margin + rectSize.y, 0) - pivotPoint;
			vertex2.uv = new float2(1 + uvExpand.y, 1 + uvExpand.w);

			vertex3 = new VertexData();
			vertex3.position = new float3(expand.y + margin + rectSize.x, expand.z - margin, 0) - pivotPoint;
			vertex3.uv = new float2(1 + uvExpand.y, uvExpand.z);
		}

		[BurstCompile]
		public static void ShadowSizeOffset(in float2 rectSize, in float2 shadowOffset, in float rotation, out float4 sizeOffset)
		{
			RotateVector(shadowOffset, rotation, out float2 rotatedOffset);
			sizeOffset = new float4(rotatedOffset / rectSize, rotatedOffset.x, rotatedOffset.y);
		}

		[BurstCompile]
		public static void RotateVector(in float2 vector, in float rotation, out float2 rotated)
		{
			if (math.abs(rotation) < 0.0001f)
			{
				rotated = vector;
				return;
			}
			if (math.abs(vector.x) < 0.0001f && math.abs(vector.y) < 0.0001f)
			{
				rotated = vector;
				return;
			}

			math.sincos(math.radians(-rotation), out float sin, out float cos);
			float2x2 matrix = new(cos, -sin, sin, cos);
			rotated = math.mul(matrix, vector);
		}
	}

	public struct VertexData
	{
		public float3 position;
		public float2 uv;
	}
}