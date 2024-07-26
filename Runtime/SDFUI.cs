using Unity.Burst;
using Unity.Mathematics;
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
		public static readonly int PROP_ROTATION = Shader.PropertyToID("_Rotation");
		public static readonly int PROP_OUTERUV = Shader.PropertyToID("_OuterUV");

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
		};

		public enum ActiveImageType
		{
			SPRITE,
			TEXTURE
		};

		[SerializeField] protected bool m_onion = false;
		[SerializeField, Min(0f)] protected float m_onionWidth = 10;

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

		protected Material m_material;
		protected Material m_materialOutlineInside;
		protected Material m_materialOutlineOutside;

		protected Sprite m_overrideSprite;
		protected Texture m_overrideTexture;

		protected Mask m_mask;

		protected readonly static Vector4 defaultOuterUV = new Vector4(0, 0, 1, 1);

		protected readonly static Color alpha0 = new Color(0, 0, 0, 0);

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
								if (material != null && material.mainTexture != null)
								{
									return material.mainTexture;
								}
								return s_WhiteTexture;
							}
							return m_sprite.texture;
						}
					default: // ActiveImageType.TEXTURE
						{
							if (m_texture == null)
							{
								if (material != null && material.mainTexture != null)
								{
									return material.mainTexture;
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

			if (material != m_material)
			{
				material = m_material;
			}
		}

		/// <summary>
		/// This function must be called before calling the set material dirty function.
		/// </summary>
		protected virtual void Validate()
		{
			CreateMaterial();

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
			DeleteOldMat();

			Validate();

			base.OnEnable();
		}

		protected virtual void ForceUpdateMask()
		{
			SetVerticesDirty();

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

			ForceUpdateMask();
		}

		protected override void OnRectTransformDimensionsChange()
		{
			base.OnRectTransformDimensionsChange();

			ForceUpdateMask();
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

			float2 shadowOffset = shadow ? m_shadowOffset : float2.zero;
			float rotation = rectTransform.rotation.eulerAngles.z * Mathf.Deg2Rad;

			float2[] uvAtlas = new float2[4]
			{
				new float2(0, 0),
				new float2(0, 1),
				new float2(1, 1),
				new float2(1, 0)
			};

			switch (activeImageType)
			{
				case ActiveImageType.SPRITE:
					var activeSprite = this.activeSprite;
					if (activeSprite != null && activeSprite.uv.Length == 4)
					{
						//uvAtlas[0] = activeSprite.uv[0];
						//uvAtlas[1] = activeSprite.uv[1];
						//uvAtlas[2] = activeSprite.uv[2];
						//uvAtlas[3] = activeSprite.uv[3];
					}
					break;
				case ActiveImageType.TEXTURE:
					break;
			}

			SDFUtils.CalculateVertexes(rectTransform.rect.size, rectTransform.pivot, m_extraMargin, shadowOffset, rotation,
				uvAtlas[0], uvAtlas[1], uvAtlas[2], uvAtlas[3],
				out var vertex0, out var vertex1, out var vertex2, out var vertex3);

			var color32 = color;
			vh.AddVert(vertex0.position, color32, new Vector4(vertex0.uv.x, vertex0.uv.y));
			vh.AddVert(vertex1.position, color32, new Vector4(vertex1.uv.x, vertex1.uv.y));
			vh.AddVert(vertex2.position, color32, new Vector4(vertex2.uv.x, vertex2.uv.y));
			vh.AddVert(vertex3.position, color32, new Vector4(vertex3.uv.x, vertex3.uv.y));

			vh.AddTriangle(0, 1, 2);
			vh.AddTriangle(2, 3, 0);
		}

		protected override void UpdateMaterial()
		{
			if (!IsActive())
				return;

			canvasRenderer.materialCount = 1;
			canvasRenderer.SetMaterial(materialForRendering, 0);
			switch (m_activeImageType)
			{
				case ActiveImageType.SPRITE:
					canvasRenderer.SetTexture((activeSprite == null) ? s_WhiteTexture : activeSprite.texture);
					break;
				case ActiveImageType.TEXTURE:
					canvasRenderer.SetTexture((activeTexture == null) ? s_WhiteTexture : activeTexture);
					break;
			}
		}

		public virtual bool IsMaterialActive()
		{
			return IsActive() && (material == m_material);
		}

#if UNITY_EDITOR
		protected virtual void ConfirmMaterialExist()
		{
			CreateMaterial();
		}
#endif

		private float m_prebEulerZ = 0f;

		private void Update()
		{
			var eulerZ = rectTransform.rotation.eulerAngles.z;
			if (eulerZ != m_prebEulerZ)
			{
				m_prebEulerZ = eulerZ;

				ForceUpdateMask();
			}
		}

		public override void SetMaterialDirty()
		{
			base.SetMaterialDirty();

			if (!IsMaterialActive())
			{
				return;
			}

			m_material.SetVector(PROP_HALFSIZE, ((RectTransform)transform).rect.size * .5f);

			switch (m_activeImageType)
			{
				case ActiveImageType.SPRITE:
					{
						var activeSprite = this.activeSprite;
						if (activeSprite == null)
						{
							m_material.mainTexture = s_WhiteTexture;
							m_material.SetVector(PROP_OUTERUV, defaultOuterUV);
						}
						else
						{
							m_material.mainTexture = activeSprite.texture;
							m_material.SetVector(PROP_OUTERUV, UnityEngine.Sprites.DataUtility.GetOuterUV(activeSprite));
						}
					}
					break;
				case ActiveImageType.TEXTURE:
					{
						var activeTexture = this.activeTexture;
						m_material.SetVector(PROP_OUTERUV, defaultOuterUV);
						m_material.mainTexture = (activeTexture == null) ? s_WhiteTexture : activeTexture;
					}
					break;
			}

			m_material.mainTextureScale = new Vector2(uvRect.size.x, uvRect.size.y);
			m_material.mainTextureOffset = new Vector2(uvRect.x, uvRect.y);
			m_material.color = m_fillColor;

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

			var rotation = rectTransform.rotation.eulerAngles.z / 360f;
			m_material.SetFloat(PROP_ROTATION, rotation);
		}
	}

	[BurstCompile]
	public static class SDFUtils
	{
		[BurstCompile]
		public static void VectorTransformation(out float2 result, in float2 p, in float rotation)
		{
			float sine = math.sin(rotation), cosine = math.cos(rotation);
			result = new float2(cosine * p.x + sine * p.y, cosine * p.y - sine * p.x);
		}

		[BurstCompile]
		public static void VectorTransformation(out float2 result, in float2 p, in float rotation, in float2 scale)
		{
			float sine = math.sin(rotation), cosine = math.cos(rotation);
			result = scale * new float2(cosine * p.x + sine * p.y, cosine * p.y - sine * p.x);
		}

		[BurstCompile]
		public static void CalculateVertexes(in float2 rectSize, in float2 rectPivot, in float margin,
			in float2 shadowOffset, in float rotation,
			in float2 uvAtlas0, in float2 uvAtlas1, in float2 uvAtlas2, in float2 uvAtlas3,
			out VertexData vertex0, out VertexData vertex1, out VertexData vertex2, out VertexData vertex3)
		{
			float3 pivotPoint = new(rectSize * rectPivot, 0);
			float4 shadowExpand = float4.zero;

			VectorTransformation(out var shadowOffsetRotated, shadowOffset, rotation);

			if (shadowOffsetRotated.x < 0)
			{
				shadowExpand.x = shadowOffsetRotated.x;
				shadowExpand.y = 0;
			}
			else
			{
				shadowExpand.x = 0;
				shadowExpand.y = shadowOffsetRotated.x;
			}

			if (shadowOffsetRotated.y < 0)
			{
				shadowExpand.z = shadowOffsetRotated.y;
				shadowExpand.w = 0;
			}
			else
			{
				shadowExpand.z = 0;
				shadowExpand.w = shadowOffsetRotated.y;
			}

			float scaleX = math.mad(2, margin, rectSize.x);
			float scaleY = math.mad(2, margin, rectSize.y);
			float4 uvExpand = new(shadowExpand.x / scaleX, shadowExpand.y / scaleX, shadowExpand.z / scaleY, shadowExpand.w / scaleY);

			vertex0 = new VertexData();
			vertex0.position = new float3(shadowExpand.x - margin, shadowExpand.z - margin, 0) - pivotPoint;
			vertex0.uv = new float2(uvExpand.x, uvExpand.z);
			//VectorTransformation(out vertex0.uv, vertex0.uv, 0, uvAtlas0);

			vertex1 = new VertexData();
			vertex1.position = new float3(shadowExpand.x - margin, shadowExpand.w + margin + rectSize.y, 0) - pivotPoint;
			vertex1.uv = new float2(uvExpand.x, uvExpand.w + 1);
			//VectorTransformation(out vertex1.uv, vertex1.uv, 0, uvAtlas1);

			vertex2 = new VertexData();
			vertex2.position = new float3(shadowExpand.y + margin + rectSize.x, shadowExpand.w + margin + rectSize.y, 0) - pivotPoint;
			vertex2.uv = new float2(1 + uvExpand.y, 1 + uvExpand.w);
			//VectorTransformation(out vertex2.uv, vertex2.uv, 0, uvAtlas2);

			vertex3 = new VertexData();
			vertex3.position = new float3(shadowExpand.y + margin + rectSize.x, shadowExpand.z - margin, 0) - pivotPoint;
			vertex3.uv = new float2(1 + uvExpand.y, uvExpand.z);
			//VectorTransformation(out vertex3.uv, vertex3.uv, 0, uvAtlas3);
		}
	}

	public struct VertexData
	{
		public float3 position;
		public float2 uv;
	}
}