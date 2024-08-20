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
	public class SDFCircle : SDFUI
	{
#if UNITY_EDITOR
		[MenuItem("GameObject/UI/SDFUI/SDFCircle", false)]
		private static void Create(MenuCommand menuCommand)
		{
			Create<SDFCircle>(menuCommand);
		}
#endif

		protected override string SHADER_NAME => "Hidden/UI/SDF/Circle/Outline";

		[Range(0, 1), SerializeField]
		public float m_min = 0;

		[Range(0, 1), SerializeField]
		public float m_max = 1;

		public static readonly int PROP_RADIUSE = Shader.PropertyToID("_Radius");

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

		protected override void UpdateMaterialRecord()
		{
			base.UpdateMaterialRecord();

			_materialRecord.SetFloat(PROP_RADIUSE, minSize * 0.5f);
		}
	}
}