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
	public class SDFUI : RawImage
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

		protected Material m_material;

		protected Material m_materialOutlineInside;

		protected Material m_materialOutlineOutside;

		protected Texture m_overrideTexture;

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

		private Texture activeTexture
		{
			get
			{
				return m_overrideTexture != null ? m_overrideTexture : texture;
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

			float2 shadowOffset = shadow ? m_shadowOffset : float2.zero;

			SDFUtils.CalculateVertexes(rectTransform.rect.size, rectTransform.pivot, m_extraMargin, shadowOffset,
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
			canvasRenderer.SetTexture((activeTexture == null) ? s_WhiteTexture : activeTexture);
		}

		public override void SetMaterialDirty()
		{
			base.SetMaterialDirty();

			m_material.SetVector(PROP_HALFSIZE, ((RectTransform)transform).rect.size * .5f);

			m_material.mainTexture = (activeTexture == null) ? s_WhiteTexture : activeTexture;
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
		}
	}

	[BurstCompile]
	public static class SDFUtils
	{
		[BurstCompile]
		public static void CalculateVertexes(in float2 rectSize, in float2 rectPivot, in float margin, in float2 shadowOffset,
			out VertexData vertex0, out VertexData vertex1, out VertexData vertex2, out VertexData vertex3)
		{
			float3 pivotPoint = new(rectSize * rectPivot, 0);
			float4 shadowExpand = float4.zero;

			if (shadowOffset.x < 0)
			{
				shadowExpand.x = shadowOffset.x;
				shadowExpand.y = 0;
			}
			else
			{
				shadowExpand.x = 0;
				shadowExpand.y = shadowOffset.x;
			}

			if (shadowOffset.y < 0)
			{
				shadowExpand.z = shadowOffset.y;
				shadowExpand.w = 0;
			}
			else
			{
				shadowExpand.z = 0;
				shadowExpand.w = shadowOffset.y;
			}

			float scaleX = math.mad(2, margin, rectSize.x);
			float scaleY = math.mad(2, margin, rectSize.y);
			float4 uvExpand = new(shadowExpand.x / scaleX, shadowExpand.y / scaleX, shadowExpand.z / scaleY, shadowExpand.w / scaleY);

			vertex0 = new VertexData();
			vertex0.position = new float3(shadowExpand.x - margin, shadowExpand.z - margin, 0) - pivotPoint;
			vertex0.uv = new float2(uvExpand.x, uvExpand.z);

			vertex1 = new VertexData();
			vertex1.position = new float3(shadowExpand.x - margin, shadowExpand.w + margin + rectSize.y, 0) - pivotPoint;
			vertex1.uv = new float2(uvExpand.x, uvExpand.w + 1);

			vertex2 = new VertexData();
			vertex2.position = new float3(shadowExpand.y + margin + rectSize.x, shadowExpand.w + margin + rectSize.y, 0) - pivotPoint;
			vertex2.uv = new float2(1 + uvExpand.y, 1 + uvExpand.w);

			vertex3 = new VertexData();
			vertex3.position = new float3(shadowExpand.y + margin + rectSize.x, shadowExpand.z - margin, 0) - pivotPoint;
			vertex3.uv = new float2(1 + uvExpand.y, uvExpand.z);
		}
	}

	public struct VertexData
	{
		public float3 position;
		public float2 uv;
	}
}
