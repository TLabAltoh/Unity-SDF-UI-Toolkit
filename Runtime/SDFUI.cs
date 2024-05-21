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

		public bool onion = false;

		public float onionWidth = 10;

		public bool outline = true;

		public float outlineWidth = 10;

		public Color outlineColor = new Color(0.0f, 1.0f, 1.0f, 1.0f);

		protected Material m_material;

		protected Mask m_mask;

		public Material material => m_material;

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
