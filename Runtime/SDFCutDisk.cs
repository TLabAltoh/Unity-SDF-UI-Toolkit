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
	public class SDFCutDisk : SDFUI
	{
#if UNITY_EDITOR
		[MenuItem("GameObject/UI/SDFUI/SDFCutDisk", false)]
		private static void Create(MenuCommand menuCommand)
		{
			Create<SDFCutDisk>(menuCommand);
		}
#endif

		protected override string SHADER_NAME => "UI/SDF/CutDisk/Outline";

		[SerializeField, Range(0f, 1f)] private float m_height = 0.5f;

		public static readonly int PROP_RADIUSE = Shader.PropertyToID("_Radius");
		public static readonly int PROP_HEIGHT = Shader.PropertyToID("_Height");

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

		public override void SetMaterialDirty()
		{
			base.SetMaterialDirty();

			var radius = m_minSize * 0.5f;
			_materialRecord.SetFloat(PROP_HEIGHT, -radius * (1f - m_height) + radius * m_height);
			_materialRecord.SetFloat(PROP_RADIUSE, radius);
		}
	}
}