using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;
#endif

namespace TLab.UI.SDF.Editor
{
	public class SDFUIEditor : GraphicEditor
	{
		protected SerializedProperty m_Texture;
		protected SerializedProperty m_UVRect;
		protected GUIContent m_UVRectContent;
		protected SDFUI m_baseInstance;

		protected override void OnEnable()
		{
			base.OnEnable();

			m_baseInstance = target as SDFUI;

			// Note we have precedence for calling rectangle for just rect, even in the Inspector.
			// For example in the Camera component's Viewport Rect.
			// Hence sticking with Rect here to be consistent with corresponding property in the API.
			m_UVRectContent = EditorGUIUtility.TrTextContent("UV Rect");

			m_Texture = serializedObject.FindProperty("m_Texture");
			m_UVRect = serializedObject.FindProperty("m_UVRect");

			SetShowNativeSize(true);
		}

		protected void SetShowNativeSize(bool instant)
		{
			SetShowNativeSize(m_Texture.objectReferenceValue != null, instant);
		}

		protected static Rect Outer(RawImage rawImage)
		{
			var outer = rawImage.uvRect;
			outer.xMin *= rawImage.rectTransform.rect.width;
			outer.xMax *= rawImage.rectTransform.rect.width;
			outer.yMin *= rawImage.rectTransform.rect.height;
			outer.yMax *= rawImage.rectTransform.rect.height;
			return outer;
		}

		/// <summary>
		/// Allow the texture to be previewed.
		/// </summary>

		public override bool HasPreviewGUI()
		{
			var rawImage = target as RawImage;
			if (rawImage == null)
				return false;

			var outer = Outer(rawImage);
			return outer.width > 0 && outer.height > 0;
		}

		/// <summary>
		/// Draw the Image preview.
		/// </summary>

		public override void OnPreviewGUI(Rect rect, GUIStyle background)
		{
			var rawImage = target as RawImage;
			var tex = rawImage.mainTexture;

			if (tex == null)
				return;

			var outer = Outer(rawImage);
			SpriteDrawUtility.DrawSprite(tex, rect, outer, rawImage.uvRect, rawImage.canvasRenderer.GetColor());
		}

		/// <summary>
		/// Info String drawn at the bottom of the Preview
		/// </summary>

		public override string GetInfoString()
		{
			var rawImage = target as RawImage;

			// Image size Text
			var text = string.Format("RawImage Size: {0}x{1}",
				Mathf.RoundToInt(Mathf.Abs(rawImage.rectTransform.rect.width)),
				Mathf.RoundToInt(Mathf.Abs(rawImage.rectTransform.rect.height)));

			return text;
		}

		protected virtual void DrawProp()
		{
			GUIStyle style = new(EditorStyles.boldLabel)
			{
				fontSize = 16,
				contentOffset = new Vector2(15, 0),
			};
			EditorGUILayout.LabelField("Fill", style);
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(m_Texture);

			if (m_baseInstance.texture)
			{
				EditorGUILayout.PropertyField(m_UVRect, m_UVRectContent);
			}

			serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.fillColor), "FillColor");
			EditorGUI.indentLevel--;
		}

		protected virtual void DrawShapeProp()
		{
			GUIStyle style = new(EditorStyles.boldLabel)
			{
				fontSize = 16,
				contentOffset = new Vector2(15, 0),
			};
			EditorGUILayout.LabelField("Shape", style);
		}

		protected virtual void DrawOutlineProp()
		{
			serializedObject.TryDrawLeftToggle("m_" + nameof(m_baseInstance.outline), "Outline");
			EditorGUI.indentLevel++;
			if (m_baseInstance.outline)
			{
				serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.outlineWidth), "OutlineWidth");
				serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.outlineColor), "OutlineColor");
				serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.outlineType), "OutlineType");
			}
			EditorGUI.indentLevel--;
		}

		protected virtual void DrawOnionProp()
		{
			serializedObject.TryDrawLeftToggle("m_" + nameof(m_baseInstance.onion), "Onion");
			EditorGUI.indentLevel++;
			if (m_baseInstance.onion)
			{
				serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.onionWidth), "OnionWidth");
			}
			EditorGUI.indentLevel--;
		}

		protected virtual void DrawShadowProp()
		{
			serializedObject.TryDrawLeftToggle("m_" + nameof(m_baseInstance.shadow), "Shadow");
			EditorGUI.indentLevel++;
			if (m_baseInstance.shadow)
			{
				serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.shadowColor), "ShadowColor");
				serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.shadowOffset), "ShadowOffset");
				serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.shadowWidth), "ShadowWidth");
				serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.shadowBlur), "ShadowBlur");
				serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.shadowPower), "shadowPower");
			}
			EditorGUI.indentLevel--;
		}

		protected virtual void DrawOtherProp()
		{
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(m_Color);
			RaycastControlsGUI();
			MaskableControlsGUI();
			SetShowNativeSize(false);
			NativeSizeButtonGUI();
			EditorGUI.indentLevel--;

		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			DrawProp();

			DrawShapeProp();

			DrawOutlineProp();

			DrawOnionProp();

			DrawShadowProp();

			EditorGUILayout.Space();

			DrawOtherProp();

			serializedObject.ApplyModifiedProperties();
		}
	}
}
