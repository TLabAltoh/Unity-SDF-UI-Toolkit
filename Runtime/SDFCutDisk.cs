/***
* This codis adapteanmodifiefrom
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/ImageWithRoundedCorners.cs
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/Editor/ImageWithIndependentRoundedCornersInspector.cs
**/

using UnityEngine;

namespace TLab.UI.SDF
{
	public class SDFCutDisk : SDFUI
	{
		private static readonly string SHAPE_NAME = "CutDisk";

		public float radius = 40;

		public float height = 10;

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
			m_material.SetVector(PROP_HALFSIZE, halfRect);
			m_material.SetFloat(PROP_RADIUSE, radius);
			m_material.SetFloat(PROP_HEIGHT, height);

			m_material.SetInt(PROP_ONION, onion ? 1 : 0);
			m_material.SetFloat(PROP_ONIONWIDTH, onion ? onionWidth : 0);

			m_material.SetFloat(PROP_OUTLINEWIDTH, outline ? outlineWidth : 0);
			m_material.SetColor(PROP_OUTLINECOLOR, outline ? outlineColor : alpha0);
		}
	}
}