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

		[SerializeField, Min(0)] private float m_radius = 40;

		[SerializeField, Min(0)] private float m_height = 10;

		public static readonly int PROP_RADIUSE = Shader.PropertyToID("_Radius");
		public static readonly int PROP_HEIGHT = Shader.PropertyToID("_Height");

		public float radius
		{
			get => radius;
			set
			{
				if (m_radius != value)
				{
					m_radius = value;

					SetAllDirty();
				}
			}
		}

		public float height
		{
			get => m_height;
			set
			{
				if (m_height != value)
				{
					m_height = value;

					SetAllDirty();
				}
			}
		}

#if UNITY_EDITOR
		protected override void OnValidate()
		{
			Validate(SHAPE_NAME);

			base.OnValidate();
		}
#endif

		protected override void OnEnable()
		{
			DeleteOldMat();

			Validate(SHAPE_NAME);

			base.OnEnable();
		}

		public override void SetMaterialDirty()
		{
			base.SetMaterialDirty();

			m_material.SetFloat(PROP_HEIGHT, m_height);
			m_material.SetFloat(PROP_RADIUSE, m_radius);
		}
	}
}