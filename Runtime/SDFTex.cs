/***
* This codis adapteanmodifiefrom
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/ImageWithRoundedCorners.cs
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/Editor/ImageWithIndependentRoundedCornersInspector.cs
**/

using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TLab.UI.SDF
{
	public class SDFTex : SDFUI
	{
#if UNITY_EDITOR
		[MenuItem("GameObject/UI/SDFUI/SDFTex", false)]
		private static void Create(MenuCommand menuCommand)
		{
			Create<SDFTex>(menuCommand);
		}
#endif

		protected override string SHADER_NAME => "UI/SDF/Tex/Outline";

		[SerializeField, Min(0)] private float m_radius = 40;
		[SerializeField, Min(0)] private float m_maxDist = 50;
		[SerializeField] private Texture2D m_sdfTexture;

		private static readonly int PROP_RADIUSE = Shader.PropertyToID("_Radius");
		private static readonly int PROP_SDFTEX = Shader.PropertyToID("_SDFTex");
		private static readonly int PROP_MAXDIST = Shader.PropertyToID("_MaxDist");

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

		public float maxDist
		{
			get => m_maxDist;
			set
			{
				if (m_maxDist != value)
				{
					m_maxDist = value;

					SetAllDirty();
				}
			}
		}

		public Texture2D sdfTexture
		{
			get => m_sdfTexture;
			set
			{
				if (m_sdfTexture != value)
				{
					m_sdfTexture = value;

					SetAllDirty();
				}
			}
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();

			var width = rectTransform.rect.width;
			var height = rectTransform.rect.height;

			var pivot = new Vector3(rectTransform.pivot.x * width, rectTransform.pivot.y * height, 0);

			var vertex = UIVertex.simpleVert;
			vertex.color = color;

			vertex.position = new Vector3(0, 0) - pivot;
			vertex.uv0 = new Vector2(0, 0);
			vh.AddVert(vertex);

			vertex.position = new Vector3(0, height) - pivot;
			vertex.uv0 = new Vector2(0, 1);
			vh.AddVert(vertex);

			vertex.position = new Vector3(width, height) - pivot;
			vertex.uv0 = new Vector2(1, 1);
			vh.AddVert(vertex);

			vertex.position = new Vector3(width, 0) - pivot;
			vertex.uv0 = new Vector2(1, 0);
			vh.AddVert(vertex);

			vh.AddTriangle(0, 1, 2);
			vh.AddTriangle(2, 3, 0);
		}

		public override void SetMaterialDirty()
		{
			base.SetMaterialDirty();

			_materialRecord.SetFloat(PROP_PADDING, 0);   // Override
			_materialRecord.SetFloat(PROP_RADIUSE, m_radius);
			_materialRecord.SetTexture(PROP_SDFTEX, m_sdfTexture);
			_materialRecord.SetFloat(PROP_MAXDIST, m_maxDist);
		}
	}
}
