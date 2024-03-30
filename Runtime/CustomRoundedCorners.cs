using UnityEngine;
using UnityEngine.UI;

namespace TLab.UI.RoundedCorners
{
	public class CustomRoundedCorners : MonoBehaviour
	{
		[SerializeField] public float outlineWidth = 10;
		[SerializeField] public Color outlineColor = new Color(0.0f, 1.0f, 1.0f, 1.0f);

		protected Material m_material;

		[HideInInspector, SerializeField] protected MaskableGraphic m_image;

		protected virtual void Validate(string shape)
		{
			if (m_material == null)
			{
				m_material = new Material(Shader.Find("UI/RoundedCorners/" + shape));
			}

			if (m_image == null)
			{
				TryGetComponent(out m_image);
			}

			if (m_image != null)
			{
				m_image.material = m_material;
			}
		}

		protected virtual void OnRectTransformDimensionsChange()
		{
			if (enabled && m_material != null)
			{
				Refresh();
			}
		}

		protected virtual void OnValidate()
		{
		}

		protected virtual void OnEnable()
		{
			var other2 = GetComponent<CustomRoundedCorners>();
			if (other2 != null && other2 != this)
			{
				DestroyHelper.Destroy(other2);
			}
		}

		protected virtual void OnDestroy()
		{
			m_image.material = null;      // This makes so that when the component is removed, the UI m_material returns to null

			DestroyHelper.Destroy(m_material);
			m_image = null;
			m_material = null;
		}

		protected virtual void Refresh()
		{
		}
	}
}
