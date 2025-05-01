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

		protected override string SHADER_NAME => "Hidden/UI/SDF/Triangle/Outline";

		[SerializeField, Range(0, 1)] private float m_roundness = 0.1f;
		[SerializeField] private Vector2 m_corner0 = new Vector2(-0.45f, -0.45f);
		[SerializeField] private Vector2 m_corner1 = new Vector2(0.45f, -0.45f);
		[SerializeField] private Vector2 m_corner2 = new Vector2(0.0f, 0.45f);

		internal static readonly int PROP_ROUNDNESS = Shader.PropertyToID("_Roundness");
		internal static readonly int PROP_CORNER0 = Shader.PropertyToID("_Corner0");
		internal static readonly int PROP_CORNER1 = Shader.PropertyToID("_Corner1");
		internal static readonly int PROP_CORNER2 = Shader.PropertyToID("_Corner2");

		private string THIS_NAME => "[" + this.GetType() + "] ";

		public float roundness
		{
			get => m_roundness;
			set
			{
				if (m_roundness != value)
				{
					m_roundness = value;

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

		public Vector2[] corners => new Vector2[] { m_corner0, m_corner1, m_corner2 };

		public Vector2 this[int i]
		{
			get => corners[i];
			set
			{
				switch (i)
				{
					case 0:
						corner0 = value;
						break;
					case 1:
						corner1 = value;
						break;
					case 2:
						corner2 = value;
						break;
				}
			}
		}

		public Vector2 GetCorner(int index, bool isWorldSpace = false)
		{
			Vector2 corner = this[index];

			return isWorldSpace ? rectTransform.TransformPoint(new Vector2(corner.x, -corner.y) * minSize) : corner;
		}

		public void SetCorner(int index, Vector2 corner, bool isWorldSpace = false)
		{
			if (isWorldSpace)
			{
				corner = rectTransform.InverseTransformPoint(corner) / minSize;
				corner = new Vector2(corner.x, -corner.y);
			}

			this[index] = corner;
		}

		protected override void UpdateMaterialRecord()
		{
			base.UpdateMaterialRecord();

			var minSize = this.minSize;
			_materialRecord.SetFloat(PROP_ROUNDNESS, m_roundness * minSize);
			_materialRecord.SetVector(PROP_CORNER0, m_corner0 * minSize);
			_materialRecord.SetVector(PROP_CORNER1, m_corner1 * minSize);
			_materialRecord.SetVector(PROP_CORNER2, m_corner2 * minSize);
		}
	}
}