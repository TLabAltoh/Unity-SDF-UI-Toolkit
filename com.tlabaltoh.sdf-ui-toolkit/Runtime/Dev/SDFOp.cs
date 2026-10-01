#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine;

namespace TLab.UI.SDF
{
	public class SDFOp : SDFUI
	{
#if UNITY_EDITOR
		[MenuItem("GameObject/UI/SDFUI/SDFOp", false)]
		private static void Create(MenuCommand menuCommand)
		{
			Create<SDFOp>(menuCommand);
		}
#endif

		[SerializeField] private SdfElement[] m_elements;

		public SdfElement[] elements => m_elements;

		public enum SdfBoolOp
		{
			Union,
			Subtract,
			Intersect,
		}

		public enum SdfShape
		{
			Circle,
			Arc,
			Triangle,
			Quad,
			Parallelogram,
		}

		[System.Serializable]
		public class SdfElement
		{
			public SdfShape shape;
			public Vector2 position;
			[Min(0f)] public float scale;
			[Range(-1, 1)] public float rotation;
			public Color color;
			public SdfBoolOp boolOp;
			public bool onion = false;
			[Min(0f)] public float smooth;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 4)]
		private struct SdfOp
		{
			public SdfShape shape;
			public SdfBoolOp boolOp;
			public int onion;
			public float smooth;
			public float4 invTransform;
			public float4 position;	// x, y + padd (4 byte)
			public float4 parameters;
			public float4 color;
		}

		protected override void UpdateMaterialRecord()
		{
			base.UpdateMaterialRecord();
		}
	}
}
