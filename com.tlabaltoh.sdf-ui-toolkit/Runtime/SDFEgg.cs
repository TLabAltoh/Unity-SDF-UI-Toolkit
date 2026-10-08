using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TLab.UI.SDF
{
	public class SDFEgg : SDFUI
	{
#if UNITY_EDITOR
		[MenuItem("GameObject/UI/SDFUI/SDFEgg", false)]
		private static void Create(MenuCommand menuCommand)
		{
			Create<SDFEgg>(menuCommand);
		}
#endif

		protected override string SHADER_NAME => $"Hidden/UI/SDF/Egg/{SHADER_TYPE}/Outline";

		[SerializeField, Range(0f, 1f)] private float m_radiusA = 0.50f;
		[SerializeField, Range(0f, 1f)] private float m_radiusB = 0.25f;
		[SerializeField, Range(0f, 1f)] private float m_bluge = 0.5f;

		internal static readonly int PROP_OFFSET_Y = Shader.PropertyToID("_OffsetY");
		internal static readonly int PROP_HEIGHT = Shader.PropertyToID("_Height");
		internal static readonly int PROP_RADIUS_A = Shader.PropertyToID("_RadiusA");
		internal static readonly int PROP_RADIUS_B = Shader.PropertyToID("_RadiusB");
		internal static readonly int PROP_BLUGE = Shader.PropertyToID("_Bluge");

		public float radiusA
		{
			get => m_radiusA;
			set
			{
				var tmp = Mathf.Clamp(value, 0, 1);
				if (m_radiusA != tmp)
				{
					m_radiusA = tmp;

					SetAllDirty();
				}
			}
		}

		public float radiusB
		{
			get => m_radiusB;
			set
			{
				var tmp = Mathf.Clamp(value, 0, 1);
				if (m_radiusB != tmp)
				{
					m_radiusB = tmp;

					SetAllDirty();
				}
			}
		}

		public float bluge
		{
			get => m_bluge;
			set
			{
				if (m_bluge != value)
				{
					m_bluge = value;

					SetAllDirty();
				}
			}
		}

		protected override void UpdateMaterialRecord()
		{
			base.UpdateMaterialRecord();

			var halfWidth = rectTransform.rect.size.x * 0.5f;
			var halfHeight = rectTransform.rect.size.y * 0.5f;
			var radiusA = halfWidth * m_radiusA;
			var radiusB = halfWidth * m_radiusB;
			var height = (rectTransform.rect.size.y - (radiusA + radiusB));

			_materialRecord.SetFloat(PROP_OFFSET_Y, halfHeight - radiusB);
			_materialRecord.SetFloat(PROP_HEIGHT, height);
			_materialRecord.SetFloat(PROP_RADIUS_A, radiusA);
			_materialRecord.SetFloat(PROP_RADIUS_B, radiusB);
			_materialRecord.SetFloat(PROP_BLUGE, m_bluge);
		}
	}
}