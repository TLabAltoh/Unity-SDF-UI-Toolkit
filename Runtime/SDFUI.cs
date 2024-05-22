using UnityEngine;
using UnityEngine.UI;

namespace TLab.UI.SDF
{
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(MaskableGraphic))]
	public class SDFUI : MonoBehaviour
	{
		public static readonly int PROP_HALFSIZE = Shader.PropertyToID("_halfSize");
		public static readonly int PROP_RADIUSE = Shader.PropertyToID("_radius");
		public static readonly int PROP_THETA = Shader.PropertyToID("_theta");
		public static readonly int PROP_WIDTH = Shader.PropertyToID("_width");
		public static readonly int PROP_HEIGHT = Shader.PropertyToID("_height");
		public static readonly int PROP_ONION = Shader.PropertyToID("_onion");
		public static readonly int PROP_ONIONWIDTH = Shader.PropertyToID("_onionWidth");
		public static readonly int PROP_OUTLINECOLOR = Shader.PropertyToID("_outlineColor");
		public static readonly int PROP_OUTLINEWIDTH = Shader.PropertyToID("_outlineWidth");

		[SerializeField] protected bool m_onion = false;

		[SerializeField] protected float m_onionWidth = 10;

		[SerializeField] protected bool m_outline = true;

		[SerializeField] protected float m_outlineWidth = 10;

		[SerializeField] protected Color m_outlineColor = new Color(0.0f, 1.0f, 1.0f, 1.0f);

		protected Material m_material;

		protected Mask m_mask;

		public Material material => m_material;

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

		public Color mainColor
		{
			get => (m_image != null) ? Color.white : m_image.color;
			set
			{
				if ((m_image != null) && (m_image.color != value))
				{
					m_image.color = value;
				}
			}
		}

		protected readonly static Color alpha0 = new Color(0, 0, 0, 0);

		[HideInInspector, SerializeField] protected MaskableGraphic m_image;

		protected virtual void Validate(string shape)
		{
			if (m_material == null)
			{
				m_material = new Material(Shader.Find("UI/SDF/" + shape));
			}

			TryGetComponent(out m_image);

			m_image.material = m_material;

			m_mask = GetComponent<Mask>();
		}

		protected virtual void OnRectTransformDimensionsChange()
		{
			if (enabled && m_material != null)
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

		protected virtual void OnValidate()
		{
		}

		protected virtual void OnEnable()
		{
			var other2 = GetComponent<SDFUI>();
			if (other2 != null && other2 != this)
			{
				DestroyHelper.Destroy(other2);
			}
		}

		protected virtual void OnDestroy()
		{
			m_image.material = null;    // This makes so that when thcomponent is removed, thUI m_materiareturns to null

			DestroyHelper.Destroy(m_material);
			m_image = null;
			m_material = null;
		}

		protected virtual void Refresh()
		{
		}
	}
}
