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

		[SerializeField, Range(0, 1)] private float m_radius = 0.1f;
		[SerializeField] private Vector2 m_corner0 = new Vector2(-0.45f, -0.45f);
		[SerializeField] private Vector2 m_corner1 = new Vector2(0.45f, -0.45f);
		[SerializeField] private Vector2 m_corner2 = new Vector2(0.0f, 0.45f);

		public static readonly int PROP_RADIUSE = Shader.PropertyToID("_Radius");
		private static readonly int PROP_CORNER0 = Shader.PropertyToID("_Corner0");
		private static readonly int PROP_CORNER1 = Shader.PropertyToID("_Corner1");
		private static readonly int PROP_CORNER2 = Shader.PropertyToID("_Corner2");

		private string THIS_NAME => "[" + this.GetType() + "] ";

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

		public Vector2 GetCorner(int index, bool isWorldSpace = false)
		{
			Vector2 corner;

			switch (index)
			{
				case 0:
					corner = corner0;
					break;
				case 1:
					corner = corner1;
					break;
				case 2:
					corner = corner2;
					break;
				default:
					Debug.LogError(THIS_NAME + $"An invalid index has been given: {index}");

					return Vector2.zero;
			}

			return isWorldSpace ? rectTransform.TransformPoint(new Vector2(corner.x, -corner.y) * minSize) : corner;
		}

		public void SetCorner(int index, Vector2 corner, bool isWorldSpace = false)
		{
			if (isWorldSpace)
			{
				corner = rectTransform.InverseTransformPoint(corner) / minSize;
				corner = new Vector2(corner.x, -corner.y);
			}

			switch (index)
			{
				case 0:
					corner0 = corner;
					return;
				case 1:
					corner1 = corner;
					return;
				case 2:
					corner2 = corner;
					return;
			}

			Debug.LogError(THIS_NAME + $"An invalid index has been given: {index}");
		}

		public override void SetMaterialDirty()
		{
			base.SetMaterialDirty();

			var minSize = this.minSize;
			_materialRecord.SetFloat(PROP_RADIUSE, m_radius * minSize);
			_materialRecord.SetVector(PROP_CORNER0, m_corner0 * minSize);
			_materialRecord.SetVector(PROP_CORNER1, m_corner1 * minSize);
			_materialRecord.SetVector(PROP_CORNER2, m_corner2 * minSize);
		}
	}
}