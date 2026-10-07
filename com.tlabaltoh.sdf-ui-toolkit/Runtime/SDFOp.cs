#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;

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

		protected override string SHADER_NAME => $"Hidden/UI/SDF/Op/{SHADER_TYPE}/Outline";

		internal static readonly int PROP_OPTEX = Shader.PropertyToID("_OpTex");
		internal static readonly int PROP_ELEMCOUNT = Shader.PropertyToID("_ElemCount");

		[SerializeField] private SdfElement[] m_elements;

		public SdfElement[] elements
        {
			get => m_elements;
			set
            {
				if (m_elements != value)
                {
					m_elements = value;

					SetAllDirty();
				}
			}
		}

		private Texture2D m_opTex = null;

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
			public Vector4 parameters;

			public Vector4 position;
			public float scale;
			public float rotation;
			public float onion;

			public SdfBoolOp boolOp;
			public float boolSmooth;

			public Color color;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 4)]
		private struct SdfOp
		{
			public SdfShape shape;
			public SdfBoolOp boolOp;
			public float onion;
			public float boolSmooth;
			public float4 invTransform;
			public float4 position;	// x, y + padd (4 byte)
			public float4 parameters;
			public float4 color;
		}

        protected override void OnDestroy()
        {
            base.OnDestroy();

			if (m_opTex != null)
			{
				DestroyOpTexture();
			}
		}

		private void DestroyOpTexture()
        {
			if (Application.isPlaying) Destroy(m_opTex);
			else DestroyImmediate(m_opTex);
			m_opTex = null;
		}

		[BurstCompile(CompileSynchronously = true)]
		private struct PackSdfOpBufferJob : IJob
		{
			[ReadOnly] public NativeArray<SdfOp> srcOps;
			[WriteOnly] public NativeArray<float4> outputTextureData;

			public void Execute()
			{
				unsafe
				{
					int count = srcOps.Length;
					for (int index = 0; index < count; index++)
					{
						SdfOp op = srcOps[index];
						float4* opPtr = (float4*)&op;

						int baseTexelIndex = index * (sizeof(SdfOp) / 16);
						for (int i = 0; i < (sizeof(SdfOp) / 16); i++)
						{
							outputTextureData[baseTexelIndex + i] = opPtr[i];
						}
					}
				}
			}
		}

		protected override void UpdateMaterialRecord()
		{
			base.UpdateMaterialRecord();

#if false
			SdfElement[] t_elements = new SdfElement[1];

			int index;
#endif

#if false
			// Circle
			index = 0;
			t_elements[index] = new SdfElement();
			t_elements[index].shape = SdfShape.Circle;
			/***
			* x: Radius
			* y: None
			* z: None
			* w: None
			*/
			t_elements[index].parameters = new Vector4(25f, 0f, 0f, 0f);
			t_elements[index].position = new Vector4(25f, -10f, 0f, 0f);
			t_elements[index].rotation = 0f;
			t_elements[index].scale = 1.0f;
			t_elements[index].onion = 5f;
			t_elements[index].boolOp = SdfBoolOp.Union;
			t_elements[index].boolSmooth = 1.0f;
			t_elements[index].color = Color.white;
#endif

#if false
			// Triangle
			index = 0;
			t_elements[index] = new SdfElement();
			t_elements[index].shape = SdfShape.Triangle;
			/***
			* x: Base of a triangle
			* y: Height of a triangle
			* z: Roundness
			* w: AnchorY
			*/
			t_elements[index].parameters = new Vector4(50f, 50f, 2.5f, 0f);
			t_elements[index].position = new Vector4(-15f, 10f, 0f, 0f);
			t_elements[index].rotation = -90f;
			t_elements[index].scale = 1.0f;
			t_elements[index].onion = 0f;
			t_elements[index].boolOp = SdfBoolOp.Union;
			t_elements[index].boolSmooth = 5f;
			t_elements[index].color = Color.green;
#endif

#if false
			// Quad
			index = 0;
			t_elements[index] = new SdfElement();
			t_elements[index].shape = SdfShape.Quad;
			/***
			* x: Top right corner radius
			* y: Bottom right corner radius
			* z: Top left corner radius
			* w: Bottom left corner radius
			* 
			* position.z: Width
			* position.w: Height
			*/
			t_elements[index].parameters = new Vector4(10f, 0f, 0f, 10f);
			// Since 'parameters' (float4) alone cannot store all the data required to draw the Quad,
			// the remaining parameters are exceptionally packed into position.z and position.w.
			t_elements[index].position = new Vector4(-5f, 5f, 20f, 20f);
			t_elements[index].rotation = -180f;
			t_elements[index].scale = 1.0f;
			t_elements[index].onion = 0f;
			t_elements[index].boolOp = SdfBoolOp.Union;
			t_elements[index].boolSmooth = 5f;
			t_elements[index].color = Color.blue;
#endif

#if false
			// Arc
			index = 0;
			t_elements[index] = new SdfElement();
			t_elements[index].shape = SdfShape.Arc;
			/***
			* x: Theta
			* y: Radius
			* z: Width
			* w: CircleBorder
			* 
			* position.z: CornerRounding
			*/
			float arcRadius = 25f;
			float arcWidth = 15f;
			float arcCornerRounding = 15f;
			t_elements[index].parameters = new Vector4((270f * Mathf.Deg2Rad * 0.5f), arcRadius, (arcWidth - arcCornerRounding), (arcWidth * 0.5f));
			// Since 'parameters' (float4) alone cannot store all the data required to draw the Quad,
			// the remaining parameters are exceptionally packed into position.z and position.w.
			t_elements[index].position = new Vector4(-5f, 5f, (arcCornerRounding * 0.5f), 0f);
			t_elements[index].rotation = 0f;
			t_elements[index].scale = 1.0f;
			t_elements[index].onion = 0f;
			t_elements[index].boolOp = SdfBoolOp.Union;
			t_elements[index].boolSmooth = 5f;
			t_elements[index].color = Color.maroon;
#endif

#if false
			// Parallelogram
			index = 0;
			t_elements[index] = new SdfElement();
			t_elements[index].shape = SdfShape.Parallelogram;
			/***
			* x: Width
			* y: Height
			* z: Slide
			* w: Roundness
			*/
			t_elements[index].parameters = new Vector4(40f, 40f, -5f, 5f);
			t_elements[index].position = new Vector4(-5f, 5f, 0f, 0f);
			t_elements[index].rotation = 0f;
			t_elements[index].scale = 1.0f;
			t_elements[index].onion = 0f;
			t_elements[index].boolOp = SdfBoolOp.Union;
			t_elements[index].boolSmooth = 5f;
			t_elements[index].color = Color.yellowNice;
#endif

			_materialRecord.SetInteger(PROP_ELEMCOUNT, m_elements.Length);

			if (m_elements.Length > 0)
            {
				unsafe
				{
					int opTexLength = m_elements.Length * (sizeof(SdfOp) / 16);

					if (m_opTex == null)
                    {
						m_opTex = new Texture2D((sizeof(SdfOp) / 16), m_elements.Length, TextureFormat.RGBAFloat, false, true);
						m_opTex.filterMode = FilterMode.Point;
						m_opTex.wrapMode = TextureWrapMode.Clamp;
					}
					else if ((m_opTex.width * m_opTex.height) < opTexLength)
					{
						DestroyOpTexture();
						m_opTex = new Texture2D((sizeof(SdfOp) / 16), m_elements.Length, TextureFormat.RGBAFloat, false, true);
						m_opTex.filterMode = FilterMode.Point;
						m_opTex.wrapMode = TextureWrapMode.Clamp;
					}
				}

				NativeArray<SdfOp> srcOps = new NativeArray<SdfOp>(m_elements.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);

				for (int i = 0; i < m_elements.Length; i++)
				{
					SdfElement elem = m_elements[i];
					if (elem == null) continue;

					SdfOp op = new SdfOp();
					op.shape = elem.shape;
					op.boolOp = elem.boolOp;
					op.onion = elem.onion;
					op.boolSmooth = elem.boolSmooth;

					float rad = math.radians(elem.rotation);
					float cos = math.cos(rad);
					float sin = math.sin(rad);
					float s = elem.scale != 0f ? 1f / elem.scale : 1f;

					op.invTransform = new float4(cos * s, sin * s, -sin * s, cos * s);
					op.color = new float4(elem.color.r, elem.color.g, elem.color.b, elem.color.a);
					if (elem.shape == SdfShape.Arc)
					{
						// Unpack inspector input values (x: Ratio, y: Radius, z: Width, w: CornerRounding)
						float arcRatio = elem.parameters.x;
						float arcRadius = elem.parameters.y;
						float arcWidth = elem.parameters.z;
						float arcCornerRounding = elem.parameters.w;

						op.parameters = new Vector4(
							arcRatio * Mathf.PI * 2.0f,         // x: Theta
							arcRadius,                          // y: Radius
							arcWidth - arcCornerRounding,       // z: Width
							arcWidth * 0.5f                     // w: CircleBorder
						);

						op.position = new Vector4(
							elem.position.x,                    // x: Original center X
							elem.position.y,                    // y: Original center Y
							arcCornerRounding * 0.5f,           // z: CornerRounding
							0f                                  // w: Fixed value
						);
					}
					else
					{
						// Assign default layout for shapes other than Arc
						op.position = (Vector4)elem.position;
						op.parameters = (Vector4)elem.parameters;
					}

					srcOps[i] = op;
				}

				NativeArray<float4> texRawData = m_opTex.GetRawTextureData<float4>();

				PackSdfOpBufferJob job = new PackSdfOpBufferJob
				{
					srcOps = srcOps,
					outputTextureData = texRawData
				};

				job.Run();

				m_opTex.Apply(false, false);
				srcOps.Dispose();

				_materialRecord.SetTexture(PROP_OPTEX, m_opTex);
			}
			else
            {
				if (m_opTex != null)
                {
					DestroyOpTexture();
                }
            }
		}
	}
}
