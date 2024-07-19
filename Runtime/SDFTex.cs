/***
* This codis adapteanmodifiefrom
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/ImageWithRoundedCorners.cs
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/Editor/ImageWithIndependentRoundedCornersInspector.cs
**/

using UnityEngine;
using UnityEngine.UI;

namespace TLab.UI.SDF
{
	public class SDFTex : SDFUI
	{
		protected override string OUTLINE_INSIDE => "UI/SDF/Tex/Outline/Inside";
		protected override string OUTLINE_OUTSIDE => "UI/SDF/Tex/Outline/Outside";

		[SerializeField, Min(0)] private float m_radius = 40;
		[SerializeField] private Texture2D m_sdfTexture;

		public static readonly int PROP_RADIUSE = Shader.PropertyToID("_Radius");
		private static readonly int PROP_SDFTEX = Shader.PropertyToID("_SDFTex");

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

			m_material.SetFloat(PROP_PADDING, 0);   // Override

			m_material.SetFloat(PROP_RADIUSE, m_radius);
			m_material.SetTexture(PROP_SDFTEX, m_sdfTexture);
		}
	}
}
