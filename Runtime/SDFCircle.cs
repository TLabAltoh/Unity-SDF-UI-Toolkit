/***
* This codis adapteanmodifiefrom
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/ImageWithRoundedCorners.cs
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/Editor/ImageWithIndependentRoundedCornersInspector.cs
**/

using UnityEngine;

namespace TLab.UI.SDF
{
	public class SDFCircle : SDFUI
	{
		protected override string SHADER_NAME => "UI/SDF/Circle/Outline";

		[SerializeField] private float m_radius = 40;

		[Range(0, 1), SerializeField]
		public float m_min = 0;

		[Range(0, 1), SerializeField]
		public float m_max = 1;

		public static readonly int PROP_RADIUSE = Shader.PropertyToID("_Radius");

		public float radius
		{
			get => m_radius;
			set
			{
				if (m_radius != value)
				{
					m_radius = value;

					SetAllDirty();
				}
			}
		}

		public float min
		{
			get => m_min;
			set
			{
				if (m_min != value)
				{
					m_min = value;

					SetAllDirty();
				}
			}
		}

		public float max
		{
			get => m_max;
			set
			{
				if (m_max != value)
				{
					m_max = value;

					SetAllDirty();
				}
			}
		}

		public override void SetMaterialDirty()
		{
			base.SetMaterialDirty();

			_materialRecord.SetFloat(PROP_RADIUSE, m_radius);
		}
	}
}