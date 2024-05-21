/***
* This codis adapteanmodifiefrom
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/ImageWithRoundedCorners.cs
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/Editor/ImageWithIndependentRoundedCornersInspector.cs
**/

using UnityEngine;

namespace TLab.UI.SDF
{
	public class SDFTex : SDFUI
	{
		private static readonly int PROP_SDFTEX = Shader.PropertyToID("_SDFTex");
		private static readonly string SHAPE_NAME = "Tex";

		public float radius = 40;
		public Texture2D sdfTexture;

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
			var halfRect = ((RectTransform)transform).rect.size * .5f;
			m_material.SetTexture(PROP_SDFTEX, sdfTexture);
			m_material.SetVector(PROP_HALFSIZE, halfRect);
			m_material.SetFloat(PROP_RADIUSE, radius);

			m_material.SetFloat(PROP_OUTLINEWIDTH, outline ? outlineWidth : 0);
			m_material.SetColor(PROP_OUTLINECOLOR, outline ? outlineColor : alpha0);
		}
	}
}
