using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Nobi.UiRoundedCorners
{
    public class CustomRoundedCorners : MonoBehaviour
    {
		[SerializeField, Range(0.0f, 1.0f)] public float outlineWidth = 0.1f;
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
			var other0 = GetComponent<ImageWithRoundedCorners>();
			if (other0 != null)
			{
				DestroyHelper.Destroy(other0);
			}

			var other1 = GetComponent<ImageWithIndependentRoundedCorners>();
			if (other1 != null)
			{
				DestroyHelper.Destroy(other1);
			}

			var other2 = GetComponent<CustomRoundedCorners>();
			if (other2 != null && other2 != this)
			{
				DestroyHelper.Destroy(other2);
			}
		}

		protected virtual void OnDestroy()
		{
			m_image.material = null;      //This makes so that when the component is removed, the UI m_material returns to null

			DestroyHelper.Destroy(m_material);
			m_image = null;
			m_material = null;
		}

		protected virtual void Refresh()
		{
		}
	}
}
