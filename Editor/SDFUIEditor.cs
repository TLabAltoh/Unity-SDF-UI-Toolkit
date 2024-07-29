using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;
#endif

namespace TLab.UI.SDF.Editor
{
	public class SDFUIEditor : GraphicEditor
	{
		protected SerializedProperty m_texture;
		protected SerializedProperty m_sprite;
		protected SerializedProperty m_uvRect;
		protected GUIContent m_uvRectContent;
		protected SDFUI m_baseInstance;

		protected override void OnEnable()
		{
			base.OnEnable();

			m_baseInstance = target as SDFUI;

			// Note we have precedence for calling rectangle for just rect, even in the Inspector.
			// For example in the Camera component's Viewport Rect.
			// Hence sticking with Rect here to be consistent with corresponding property in the API.
			m_uvRectContent = EditorGUIUtility.TrTextContent("UV Rect");

			m_texture = serializedObject.FindProperty("m_texture");
			m_sprite = serializedObject.FindProperty("m_sprite");
			m_uvRect = serializedObject.FindProperty("m_uvRect");

			SetShowNativeSize(true);
		}

		protected void SetShowNativeSize(bool instant)
		{
			switch (m_baseInstance.activeImageType)
			{
				case SDFUI.ActiveImageType.SPRITE:
					SetShowNativeSize(m_sprite.objectReferenceValue != null, instant);
					break;
				case SDFUI.ActiveImageType.TEXTURE:
					SetShowNativeSize(m_texture.objectReferenceValue != null, instant);
					break;
			}
		}

		private static Rect Outer(SDFUI sdfUI)
		{
			Rect outer = sdfUI.uvRect;
			outer.xMin *= sdfUI.rectTransform.rect.width;
			outer.xMax *= sdfUI.rectTransform.rect.width;
			outer.yMin *= sdfUI.rectTransform.rect.height;
			outer.yMax *= sdfUI.rectTransform.rect.height;
			return outer;
		}

		/// <summary>
		/// Allow the texture to be previewed.
		/// </summary>

		public override bool HasPreviewGUI()
		{
			SDFUI sdfUI = target as SDFUI;
			if (sdfUI == null)
				return false;

			var outer = Outer(sdfUI);
			return outer.width > 0 && outer.height > 0;
		}

		/// <summary>
		/// Draw the Image preview.
		/// </summary>

		public override void OnPreviewGUI(Rect rect, GUIStyle background)
		{
			SDFUI sdfUI = target as SDFUI;
			Texture tex = sdfUI.mainTexture;

			if (tex == null)
				return;

			var outer = Outer(sdfUI);
			SpriteDrawUtility.DrawSprite(tex, rect, outer, sdfUI.uvRect, sdfUI.canvasRenderer.GetColor());
		}

		/// <summary>
		/// Info String drawn at the bottom of the Preview
		/// </summary>

		public override string GetInfoString()
		{
			SDFUI sdfUI = target as SDFUI;

			// Image size Text
			string text = string.Format("SDFUI Size: {0}x{1}",
				Mathf.RoundToInt(Mathf.Abs(sdfUI.rectTransform.rect.width)),
				Mathf.RoundToInt(Mathf.Abs(sdfUI.rectTransform.rect.height)));

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

			serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.activeImageType), "ActiveImageType");

			switch (m_baseInstance.activeImageType)
			{
				case SDFUI.ActiveImageType.SPRITE:
					EditorGUILayout.PropertyField(m_sprite);
					if (m_baseInstance.sprite)
					{
						EditorGUILayout.PropertyField(m_uvRect, m_uvRectContent);
					}
					break;
				case SDFUI.ActiveImageType.TEXTURE:
					EditorGUILayout.PropertyField(m_texture);
					if (m_baseInstance.texture)
					{
						EditorGUILayout.PropertyField(m_uvRect, m_uvRectContent);
					}
					break;
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
