/***
* This code is adapted from
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/ImageWithRoundedCorners.cs
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/Editor/ImageWithIndependentRoundedCornersInspector.cs
**/

using UnityEngine;
using TLab.UI.SDF.Editor;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TLab.UI.SDF
{
	public class SDFQuad : SDFUI
	{
#if UNITY_EDITOR
		[MenuItem("GameObject/UI/SDFUI/SDFQuad", false)]
		private static void Create(MenuCommand menuCommand)
		{
			Create<SDFQuad>(menuCommand);
		}
#endif

		protected override string SHAPE_NAME => "Quad";

		[SerializeField, LeftToggle] private bool m_independent = false;

		[SerializeField, Min(0)] private float m_radius = 40;

		[SerializeField, Min(0)] private float m_radiusX = 40;
		[SerializeField, Min(0)] private float m_radiusY = 40;
		[SerializeField, Min(0)] private float m_radiusZ = 40;
		[SerializeField, Min(0)] private float m_radiusW = 40;

		public static readonly int PROP_RADIUSE = Shader.PropertyToID("_Radius");

		public bool independent
		{
			get => m_independent;
			set
			{
				if (m_independent != value)
				{
					m_independent = value;

					SetAllDirty();
				}
			}
		}

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

		public float radiusX
		{
			get => m_radiusX;
			set
			{
				if (m_radiusX != value)
				{
					m_radiusX = value;

					SetAllDirty();
				}
			}
		}

		public float radiusY
		{
			get => m_radiusY;
			set
			{
				if (m_radiusY != value)
				{
					m_radiusY = value;

					SetAllDirty();
				}
			}
		}

		public float radiusZ
		{
			get => m_radiusZ;
			set
			{
				if (m_radiusZ != value)
				{
					m_radiusZ = value;

					SetAllDirty();
				}
			}
		}

		public float radiusW
		{
			get => m_radiusW;
			set
			{
				if (m_radiusW != value)
				{
					m_radiusW = value;

					SetAllDirty();
				}
			}
		}

		protected override bool UpdateMaterialRecord(in string shapeName, bool simplification = false)
		{
			base.UpdateMaterialRecord(in shapeName);

			var halfRect = ((RectTransform)transform).rect.size * .5f;

			var corners = m_independent ? new Vector4(m_radiusX, m_radiusY, m_radiusZ, m_radiusW) : Vector4.one * m_radius;

			var shortest = halfRect.x < halfRect.y ? halfRect.x : halfRect.y;
			if (corners.x > shortest)
			{
				corners.x = shortest;
			}
			if (corners.y > shortest)
			{
				corners.y = shortest;
			}
			if (corners.z > shortest)
			{
				corners.z = shortest;
			}
			if (corners.w > shortest)
			{
				corners.w = shortest;
			}

			_materialRecord.SetVector(PROP_RADIUSE, corners);

			return true;
		}
	}
}