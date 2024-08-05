/***
* This codis adapteanmodifiefrom
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/ImageWithRoundedCorners.cs
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/Editor/ImageWithIndependentRoundedCornersInspector.cs
**/

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TLab.UI.SDF
{
	public class SDFRing : SDFUI
	{
#if UNITY_EDITOR
		[MenuItem("GameObject/UI/SDFUI/SDFRing", false)]
		private static void Create(MenuCommand menuCommand)
		{
			Create<SDFRing>(menuCommand);
		}
#endif

		protected override string SHADER_NAME => "UI/SDF/Ring/Outline";

		[SerializeField, Min(0)] private float m_width = 10;

		[Range(0, Mathf.PI), SerializeField]
		private float m_theta = Mathf.PI * 0.5f;

		public static readonly int PROP_RADIUSE = Shader.PropertyToID("_Radius");
		public static readonly int PROP_THETA = Shader.PropertyToID("_Theta");
		public static readonly int PROP_WIDTH = Shader.PropertyToID("_Width");

		public float width
		{
			get => m_width;
			set
			{
				if (m_width != value)
				{
					m_width = value;

					SetAllDirty();
				}
			}
		}

		public float theta
		{
			get => m_theta;
			set
			{
				if (m_theta != value)
				{
					m_theta = value;

					SetAllDirty();
				}
			}
		}

		public override void SetMaterialDirty()
		{
			base.SetMaterialDirty();

			material.SetFloat(PROP_RADIUSE, m_minSize * 0.5f - m_width * 0.5f);
			material.SetFloat(PROP_THETA, m_theta);
			material.SetFloat(PROP_WIDTH, m_width);
		}
	}
}