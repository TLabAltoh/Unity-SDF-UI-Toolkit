using UnityEngine;
using UnityEditor;
using Unity.Collections;

namespace TLab.UI.SDF.Editor
{
    [CustomEditor(typeof(SDFTexPainter))]
    public class SDFTexPainterEditor : UnityEditor.Editor
    {
        private SDFTexPainter m_instance;

        private string THIS_NAME => "[" + GetType() + "] ";

        const int CHANNEL_SIZE = 4;

        const TextureFormat TEX_FORMAT = TextureFormat.ARGB32;

        private string[] m_shapeOptions = new string[]
        {
            Shape.CIRCLE.ToString(),
            Shape.BEZIER.ToString(),
            Shape.NONE.ToString()
        };

        private string[] m_previewModeOptions = new string[]
        {
            PreviewMode.PATH.ToString(),
            PreviewMode.SDF.ToString()
        };

        private Rect m_area;

        private void OnEnable()
        {
            m_instance = target as SDFTexPainter;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            serializedObject.TryDrawProperty(nameof(m_instance.texWidth), "Tex Width");
            serializedObject.TryDrawProperty(nameof(m_instance.texHeight), "Tex Height");
            serializedObject.TryDrawProperty(nameof(m_instance.sdfWidth), "SDF Width");
            serializedObject.TryDrawProperty(nameof(m_instance.sdfHeight), "SDF Height");

            EditorGUILayout.Space();

            serializedObject.TryDrawProperty(nameof(m_instance.sdfSettings), "SDF Settings");

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Preview");

            EditorGUILayout.Space();

            GetGUIRect();

            EditorGUILayout.Space();

            serializedObject.Call(() =>
            {
                PainterUpdate();

                PainterEdit();

                Preview();

                EditorUtility.SetDirty(m_instance);
            });

            EditorGUILayout.Space();

            serializedObject.TryDrawEnumProperty(nameof(m_instance.shape), "Shape", m_shapeOptions);
            serializedObject.TryDrawEnumProperty(nameof(m_instance.previewMode), "Preview Mode", m_previewModeOptions);

            EditorGUILayout.Space();

            PainterOption();

            EditorGUILayout.Space();

            serializedObject.TryDrawProperty(nameof(m_instance.savePath), "Save Path");

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Generate SDF"))
            {
                serializedObject.Call(() =>
                {
                    GenerateSDF();
                });
            }

            if (GUILayout.Button("Save"))
            {
                Save();
            }

            if (GUILayout.Button("Save as"))
            {
                SaveAs();
            }
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 
        /// </summary>
        private void Save()
        {
            if (serializedObject.TryGetStringValue(nameof(m_instance.savePath), out string savePath) &&
                serializedObject.TryGetObject(nameof(m_instance.sdfTex), out Texture2D sdfTex))
            {
                if (!AssetUtil.DirectoryExists(savePath))
                {
                    if (AssetUtil.SelectSavePath(savePath, out savePath))
                    {
                        AssetUtil.SaveTexture(savePath, ref sdfTex);
                    }
                }
                else
                {
                    AssetUtil.SaveTexture(savePath, ref sdfTex);
                }

                serializedObject.TrySetValue(nameof(m_instance.savePath), savePath);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void SaveAs()
        {
            if (serializedObject.TryGetStringValue(nameof(m_instance.savePath), out string savePath) &&
                serializedObject.TryGetObject(nameof(m_instance.sdfTex), out Texture2D sdfTex))
            {
                if (!AssetUtil.DirectoryExists(m_instance.savePath))
                {
                    if (AssetUtil.SelectSavePath(savePath, out savePath))
                    {
                        AssetUtil.SaveTexture(savePath, ref sdfTex);
                    }
                }

                serializedObject.TrySetValue(nameof(m_instance.savePath), savePath);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void GetGUIRect()
        {
            m_area = EditorUtil.CreatePreviewArea((float)m_instance.texHeight / m_instance.texWidth);
        }

        /// <summary>
        /// 
        /// </summary>
        private void PainterUpdate()
        {
            m_instance.circlePainter.Update(m_area, GetTextureSize(), GetSDFTextureSize());
            m_instance.bezierPainter.Update(m_area, GetTextureSize(), GetSDFTextureSize());
        }

        /// <summary>
        /// 
        /// </summary>
        private void PainterEdit()
        {
            switch (m_instance.shape)
            {
                case Shape.CIRCLE:
                    m_instance.circlePainter.Edit();
                    break;
                case Shape.BEZIER:
                    m_instance.bezierPainter.Edit();
                    break;
                case Shape.NONE:
                    break;
            }
        }

        private void PainterOption()
        {
            switch (m_instance.shape)
            {
                case Shape.CIRCLE:
                    serializedObject.TryDrawProperty(nameof(m_instance.circlePainter), "Circle Painter");
                    break;
                case Shape.BEZIER:
                    serializedObject.TryDrawProperty(nameof(m_instance.bezierPainter), "Bezier Painter");
                    break;
                case Shape.NONE:
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void GenerateSDF()
        {
            var texSize = GetTextureSize();
            var sdfTexSize = GetSDFTextureSize();

            var sdfTex = new Texture2D(sdfTexSize.x, sdfTexSize.y, TEX_FORMAT, false);

            var sdfBuffer = new NativeArray<byte>(sdfTexSize.x * sdfTexSize.y, Allocator.TempJob);

            var settings = m_instance.sdfSettings;

            m_instance.circlePainter.GenerateSDF(in sdfBuffer, sdfTexSize, texSize, settings);

            m_instance.bezierPainter.GenerateSDF(in sdfBuffer, sdfTexSize, texSize, settings);

            ConvertFormat.R8ToNChannel(in sdfBuffer, in sdfTex, CHANNEL_SIZE);

            m_instance.sdfTex = sdfTex;

            sdfBuffer.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Vector2Int GetTextureSize()
        {
            var texSize = new Vector2Int();

            if (serializedObject.TryGetIntValue(nameof(m_instance.texWidth), out int x))
            {
                texSize.x = x;
            }

            if (serializedObject.TryGetIntValue(nameof(m_instance.texHeight), out int y))
            {
                texSize.y = y;
            }

            return texSize;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Vector2Int GetSDFTextureSize()
        {
            var sdfTexSize = new Vector2Int();

            if (serializedObject.TryGetIntValue(nameof(m_instance.sdfWidth), out int x))
            {
                sdfTexSize.x = x;
            }

            if (serializedObject.TryGetIntValue(nameof(m_instance.sdfHeight), out int y))
            {
                sdfTexSize.y = y;
            }

            return sdfTexSize;
        }

        /// <summary>
        /// 
        /// </summary>
        private void Preview()
        {
            switch (m_instance.previewMode)
            {
                case PreviewMode.PATH:
                    m_instance.circlePainter.DrawPath();
                    m_instance.bezierPainter.DrawPath();
                    break;
                case PreviewMode.SDF:
                    if (m_instance.sdfTex)
                    {
                        EditorGUI.DrawPreviewTexture(m_area, m_instance.sdfTex);
                    }
                    break;
            }
        }
    }
}
