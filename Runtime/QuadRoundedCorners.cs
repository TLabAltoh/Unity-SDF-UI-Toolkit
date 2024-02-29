/***
* This code is adapted and modified from
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/ImageWithRoundedCorners.cs
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/Editor/ImageWithIndependentRoundedCornersInspector.cs
**/

using UnityEngine;

namespace Nobi.UiRoundedCorners
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
	public class QuadRoundedCorners : CustomRoundedCorners
	{
		private static readonly int PROP_HALFSIZE = Shader.PropertyToID("_halfSize");
		private static readonly int PROP_RADIUSES = Shader.PropertyToID("_r");
		private static readonly int PROP_OUTLINECOLOR = Shader.PropertyToID("_outlineColor");
		private static readonly int PROP_OUTLINEWIDTH = Shader.PropertyToID("_outlineWidth");
		private static readonly string SHAPE_NAME = "Quad";

		[SerializeField] public float radiusX = 40;
		[SerializeField] public float radiusY = 40;
		[SerializeField] public float radiusZ = 40;
		[SerializeField] public float radiusW = 40;

		protected override void OnValidate()
		{
			base.OnValidate();

			Validate(SHAPE_NAME);
			Refresh();
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			Validate(SHAPE_NAME);
			Refresh();
		}

		protected override void OnRectTransformDimensionsChange()
		{
			base.OnRectTransformDimensionsChange();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
		}

		protected override void Refresh()
		{
			Vector2 halfRect = ((RectTransform)transform).rect.size * .5f;
			m_material.SetVector(PROP_RADIUSES, new Vector4(radiusX, radiusY, radiusZ, radiusW));
			m_material.SetVector(PROP_HALFSIZE, halfRect);
			m_material.SetFloat(PROP_OUTLINEWIDTH, outlineWidth);
			m_material.SetColor(PROP_OUTLINECOLOR, outlineColor);
		}
	}
}
