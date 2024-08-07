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
	public class SDFTriangle : SDFUI
	{
#if UNITY_EDITOR
		[MenuItem("GameObject/UI/SDFUI/SDFTriangle", false)]
		private static void Create(MenuCommand menuCommand)
		{
			Create<SDFTriangle>(menuCommand);
		}
#endif

		protected override string SHADER_NAME => "UI/SDF/Triangle/Outline";

		private const float BASE_SIZE = 100f;

		[SerializeField, Min(0)] private float m_radius = 40;
		[SerializeField] private Vector2 m_corner0 = new Vector2(-45f, -45f);
		[SerializeField] private Vector2 m_corner1 = new Vector2(45f, -45f);
		[SerializeField] private Vector2 m_corner2 = new Vector2(0f, 45f);

		public static readonly int PROP_RADIUSE = Shader.PropertyToID("_Radius");
		private static readonly int PROP_CORNER0 = Shader.PropertyToID("_Corner0");
		private static readonly int PROP_CORNER1 = Shader.PropertyToID("_Corner1");
		private static readonly int PROP_CORNER2 = Shader.PropertyToID("_Corner2");

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

		public float scale => m_minSize / BASE_SIZE;

		public Vector2 corner0
		{
			get => m_corner0;
			set
			{
				if (m_corner0 != value)
				{
					m_corner0 = value;

					SetAllDirty();
				}
			}
		}

		public Vector2 corner1
		{
			get => m_corner1;
			set
			{
				if (m_corner1 != value)
				{
					m_corner1 = value;

					SetAllDirty();
				}
			}
		}

		public Vector2 corner2
		{
			get => m_corner2;
			set
			{
				if (m_corner2 != value)
				{
					m_corner2 = value;

					SetAllDirty();
				}
			}
		}

		public override void SetMaterialDirty()
		{
			base.SetMaterialDirty();

			var scale = m_minSize / BASE_SIZE;
			_materialRecord.SetFloat(PROP_RADIUSE, m_radius * scale);
			_materialRecord.SetVector(PROP_CORNER0, m_corner0 * scale);
			_materialRecord.SetVector(PROP_CORNER1, m_corner1 * scale);
			_materialRecord.SetVector(PROP_CORNER2, m_corner2 * scale);
		}
	}
}