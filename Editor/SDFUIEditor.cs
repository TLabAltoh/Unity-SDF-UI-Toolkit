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

		protected static GUIStyle labelStyle => new(EditorStyles.boldLabel)
		{
			fontSize = 16,
			contentOffset = new Vector2(15, 0),
		};

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
				case SDFUI.ActiveImageType.Sprite:
					SetShowNativeSize(m_sprite.objectReferenceValue != null, instant);
					break;
				case SDFUI.ActiveImageType.Texture:
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
			EditorGUILayout.LabelField("Fill", labelStyle);
			EditorGUI.indentLevel++;
			serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.activeImageType), "ActiveImageType");
			switch (m_baseInstance.activeImageType)
			{
				case SDFUI.ActiveImageType.Sprite:
					EditorGUILayout.PropertyField(m_sprite);
					if (m_baseInstance.sprite)
					{
						EditorGUILayout.PropertyField(m_uvRect, m_uvRectContent);
					}
					break;
				case SDFUI.ActiveImageType.Texture:
					EditorGUILayout.PropertyField(m_texture);
					if (m_baseInstance.texture)
					{
						EditorGUILayout.PropertyField(m_uvRect, m_uvRectContent);
					}
					break;
			}
			serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.fillColor), "FillColor");

			serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.gradationShape), "Gradation Shape");
			EditorGUI.indentLevel++;
			bool drawGradationColor = m_baseInstance.gradationShape != SDFUI.GradationShape.None;
			if (drawGradationColor)
			{
				serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.gradationColor), "Color");
				switch (m_baseInstance.gradationShape)
				{
					case SDFUI.GradationShape.Linear:
						serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.gradationAngle), "Angle");
						serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.gradationSmooth), "Smooth");
						break;
					case SDFUI.GradationShape.Radial:
						serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.gradationRadius), "Radius");
						serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.gradationSmooth), "Smooth");
						break;
					case SDFUI.GradationShape.Conical:
						serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.gradationAngle), "Angle");
						serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.gradationRange), "Range");
						break;
				}
				serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.gradationOffset), "Offset");
			}
			EditorGUI.indentLevel--;

			serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.graphicEffectType), "Effect");
			EditorGUI.indentLevel++;
			var effectType = m_baseInstance.graphicEffectType;
			switch (effectType)
			{
				case SDFUI.EffectType.None:
					break;
				case SDFUI.EffectType.Shiny:
					serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.graphicEffectColor), "Color");
					serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.graphicEffectAngle), "Angle");
					serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.graphicEffectShinyWidth), "Width");
					serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.graphicEffectShinyBlur), "Blur");
					serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.graphicEffectOffset), "Offset");
					break;
				case SDFUI.EffectType.Pattern:
					serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.graphicEffectColor), "Color");
					serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.graphicEffectAngle), "Angle");

					serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.graphicEffectPatternTexture), "Texture");
					serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.graphicEffectPatternRow), "Row");

					serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.graphicEffectOffset), "Offset");
					serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.graphicEffectPatternScale), "Scale");
					serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.graphicEffectPatternScroll), "Scroll");

					EditorGUILayout.BeginHorizontal();
					{
						EditorGUILayout.LabelField("Params");
						EditorGUILayout.BeginVertical();
						{
							serializedObject.TryDrawPropertyNoLabel("m_" + nameof(m_baseInstance.graphicEffectPatternParamsX));
							serializedObject.TryDrawPropertyNoLabel("m_" + nameof(m_baseInstance.graphicEffectPatternParamsZ));
						}
						EditorGUILayout.EndVertical();
						EditorGUILayout.BeginVertical();
						{
							serializedObject.TryDrawPropertyNoLabel("m_" + nameof(m_baseInstance.graphicEffectPatternParamsY));
							serializedObject.TryDrawPropertyNoLabel("m_" + nameof(m_baseInstance.graphicEffectPatternParamsW));
						}
						EditorGUILayout.EndVertical();
					}
					EditorGUILayout.EndHorizontal();
					break;
				case SDFUI.EffectType.Rainbow:
					serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.rainbowSaturation), "Saturation");
					serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.rainbowValue), "Value");
					serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.rainbowHueOffset), "Hue Offset");
					EditorGUILayout.HelpBox("Alpha from the gradation color is applied to the rainbow.", MessageType.Info);
					break;
			}
			EditorGUI.indentLevel--;

			serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.liquidGlass), "Liquid Glass");
			EditorGUI.indentLevel++;
			bool drawLiquidGlass = m_baseInstance.liquidGlass;
			if (drawLiquidGlass)
			{
				serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.liquidGlassThickness), "Thickness");
				serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.liquidGlassIndex), "Index");
			}
			EditorGUI.indentLevel--;
			EditorGUI.indentLevel--;
		}

		protected virtual void DrawShapeProp()
		{
			EditorGUILayout.LabelField("Shape", labelStyle);
		}

		protected virtual void DrawOutlineProp()
		{
			serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.outline), "Outline");
			EditorGUI.indentLevel++;
			if (m_baseInstance.outline)
			{
				serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.outlineWidth), "Width");
				serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.outlineColor), "Color");

				serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.outlineGradationShape), "Gradation Shape");
				EditorGUI.indentLevel++;
				bool drawGradationColor = m_baseInstance.outlineGradationShape != SDFUI.GradationShape.None;
				if (drawGradationColor)
				{
					serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.outlineGradationColor), "Color");
					switch (m_baseInstance.outlineGradationShape)
					{
						case SDFUI.GradationShape.Linear:
							serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.outlineGradationAngle), "Angle");
							serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.outlineGradationSmooth), "Smooth");
							break;
						case SDFUI.GradationShape.Radial:
							serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.outlineGradationRadius), "Radius");
							serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.outlineGradationSmooth), "Smooth");
							break;
						case SDFUI.GradationShape.Conical:
							serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.outlineGradationAngle), "Angle");
							serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.outlineGradationRange), "Range");
							break;
					}
					serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.outlineGradationOffset), "Offset");
				}
				EditorGUI.indentLevel--;

				serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.outlineInnerSoftWidth), "InnerSoftWidth");
				serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.outlineInnerSoftness), "InnerSoftness");
				serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.outlineType), "Direction");

				serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.outlineEffectType), "Effect");

				EditorGUI.indentLevel++;
				var effectType = m_baseInstance.outlineEffectType;
				switch (effectType)
				{
					case SDFUI.EffectType.None:
						break;
					case SDFUI.EffectType.Shiny:
						serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.outlineEffectColor), "Color");
						serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.outlineEffectAngle), "Angle");
						serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.outlineEffectShinyWidth), "Width");
						serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.outlineEffectShinyBlur), "Blur");
						serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.outlineEffectOffset), "Offset");
						break;
					case SDFUI.EffectType.Pattern:
						serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.outlineEffectColor), "Color");
						serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.outlineEffectAngle), "Angle");

						serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.outlineEffectPatternTexture), "Texture");
						serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.outlineEffectPatternRow), "Row");

						serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.outlineEffectOffset), "Offset");
						serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.outlineEffectPatternScale), "Scale");
						serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.outlineEffectPatternScroll), "Scroll");

						EditorGUILayout.BeginHorizontal();
						{
							EditorGUILayout.LabelField("Params");
							EditorGUILayout.BeginVertical();
							{
								serializedObject.TryDrawPropertyNoLabel("m_" + nameof(m_baseInstance.outlineEffectPatternParamsX));
								serializedObject.TryDrawPropertyNoLabel("m_" + nameof(m_baseInstance.outlineEffectPatternParamsZ));
							}
							EditorGUILayout.EndVertical();
							EditorGUILayout.BeginVertical();
							{
								serializedObject.TryDrawPropertyNoLabel("m_" + nameof(m_baseInstance.outlineEffectPatternParamsY));
								serializedObject.TryDrawPropertyNoLabel("m_" + nameof(m_baseInstance.outlineEffectPatternParamsW));
							}
							EditorGUILayout.EndVertical();
						}
						EditorGUILayout.EndHorizontal();
						break;
					case SDFUI.EffectType.Rainbow:
						serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.rainbowSaturation), "Saturation");
						serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.rainbowValue), "Value");
						serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.rainbowHueOffset), "Hue Offset");
						EditorGUILayout.HelpBox("Alpha from the outline gradation color is applied to the rainbow.", MessageType.Info);
						break;
				}
				EditorGUI.indentLevel--;
			}
			EditorGUI.indentLevel--;
		}

		protected virtual void DrawOnionProp()
		{
			serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.onion), "Onion");
			EditorGUI.indentLevel++;
			if (m_baseInstance.onion)
			{
				serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.onionWidth), "Width");
			}
			EditorGUI.indentLevel--;
		}

		protected virtual void DrawShadowProp()
		{
			serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.shadow), "Shadow");
			EditorGUI.indentLevel++;
			if (m_baseInstance.shadow)
			{
				serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.shadowColor), "Color");

				serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.shadowGradationShape), "Gradation Shape");
				EditorGUI.indentLevel++;
				bool drawGradationColor = m_baseInstance.shadowGradationShape != SDFUI.GradationShape.None;
				if (drawGradationColor)
				{
					serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.shadowGradationColor), "Color");
					switch (m_baseInstance.shadowGradationShape)
					{
						case SDFUI.GradationShape.Linear:
							serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.shadowGradationAngle), "Angle");
							serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.shadowGradationSmooth), "Smooth");
							break;
						case SDFUI.GradationShape.Radial:
							serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.shadowGradationRadius), "Radius");
							serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.shadowGradationSmooth), "Smooth");
							break;
						case SDFUI.GradationShape.Conical:
							serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.shadowGradationAngle), "Angle");
							serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.shadowGradationRange), "Range");
							break;
					}
					serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.shadowGradationOffset), "Offset");
				}
				EditorGUI.indentLevel--;

				serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.shadowOffset), "Offset");
				serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.shadowWidth), "Width");
				serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.shadowInnerSoftWidth), "InnerSoftWidth");
				serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.shadowSoftness), "Softness");
				serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.shadowDilate), "Dilate");

                serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.shadowEffectType), "Effect");
                EditorGUI.indentLevel++;
                var shadowEffectType = m_baseInstance.shadowEffectType;
                switch (shadowEffectType)
                {
                    case SDFUI.EffectType.None:
                        break;
                    case SDFUI.EffectType.Rainbow:
                        serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.rainbowSaturation), "Saturation");
                        serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.rainbowValue), "Value");
                        serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.rainbowHueOffset), "Hue Offset");
                        EditorGUILayout.HelpBox("Alpha from the shadow gradation color is applied to the rainbow.", MessageType.Info);
                        break;
                    default:
	                    EditorGUILayout.LabelField("Effect not implemented for Shadow");
						break;
                }
                EditorGUI.indentLevel--;
			}
			EditorGUI.indentLevel--;
		}

		protected void DrawAntialiasingProp()
		{
			serializedObject.TryDrawProperty("m_" + nameof(m_baseInstance.antialiasing), "Antialiasing");
		}

		protected virtual void DrawOtherProp()
		{
			EditorGUILayout.LabelField("Others", labelStyle);
			EditorGUI.indentLevel++;
			var color = EditorGUILayout.ColorField(new GUIContent("Color"), m_Color.colorValue, false, true, true);
			m_Color.colorValue = color;
			RaycastControlsGUI();
			MaskableControlsGUI();
			SetShowNativeSize(false);
			NativeSizeButtonGUI();
			DrawAntialiasingProp();
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

			DrawOtherProp();

			serializedObject.ApplyModifiedProperties();
		}
	}
}
