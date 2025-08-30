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

        private const TextureFormat TEX_FORMAT = TextureFormat.ARGB32;

        private string[] m_previewModeOptions = new string[]
        {
            PreviewMode.Path.ToString(),
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

            serializedObject.TryDrawProperty(nameof(m_instance.size), "Size");
            serializedObject.TryDrawProperty(nameof(m_instance.texScale), "Tex Scale (%)");

            EditorGUILayout.Space();

            serializedObject.TryDrawProperty(nameof(m_instance.rasterizeOptions), "Rasterize Options");

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Preview");

            serializedObject.TryDrawProperty(nameof(m_instance.areaScale), "Zoom");
            serializedObject.TryDrawProperty(nameof(m_instance.areaPos), "Position");
            serializedObject.TryDrawProperty(nameof(m_instance.areaBorderCol), "Border Color");

            EditorGUILayout.Space();

            m_area = EditorUtil.CreatePreviewArea((float)m_instance.size.x / m_instance.size.y, Color.black, Color.white);

            EditorGUILayout.Space();

            serializedObject.Call(() =>
            {
                PainterUpdate();

                PainterEdit();

                Preview();
            });

            EditorGUILayout.Space();

            serializedObject.TryDrawEnumProperty(nameof(m_instance.previewMode), "Preview Mode", m_previewModeOptions);

            EditorGUILayout.Space();

            PainterOption();

            EditorGUILayout.Space();

            serializedObject.TryDrawProperty(nameof(m_instance.savePath), "Save Path");

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Gen SDF Texture"))
                serializedObject.Call(() =>
                {
                    GenSDFTexture();
                });
            if (GUILayout.Button("Save"))
                Save();
            if (GUILayout.Button("Save as"))
                SaveAs();
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }

        private void Save()
        {
            if (serializedObject.TryGetStringValue(nameof(m_instance.savePath), out string savePath) &&
                serializedObject.TryGetObject(nameof(m_instance.sdfTex), out Texture2D sdfTex))
            {
                if (AssetUtil.DirectoryExists(savePath))
                {
                    AssetUtil.SaveTexture(savePath, ref sdfTex);

                    serializedObject.Call(() =>
                    {
                        AssetUtil.SaveTexture(savePath, ref sdfTex);
                    });
                }
                else if (AssetUtil.SelectSavePath(savePath, out savePath))
                {
                    AssetUtil.SaveTexture(savePath, ref sdfTex);

                    serializedObject.Call(() =>
                    {
                        m_instance.savePath = savePath;
                        EditorUtility.SetDirty(m_instance);
                    });
                }
            }
        }

        private void SaveAs()
        {
            if (serializedObject.TryGetStringValue(nameof(m_instance.savePath), out string savePath) &&
                serializedObject.TryGetObject(nameof(m_instance.sdfTex), out Texture2D sdfTex))
            {
                if (AssetUtil.SelectSavePath(savePath, out savePath))
                {
                    AssetUtil.SaveTexture(savePath, ref sdfTex);

                    serializedObject.Call(() =>
                    {
                        m_instance.savePath = savePath;
                        EditorUtility.SetDirty(m_instance);
                    });
                }
            }
        }

        private void PainterUpdate() => m_instance.bezierPainter.Update(m_area, m_instance);

        private void PainterEdit() => m_instance.bezierPainter.Edit();

        private void PainterOption() => serializedObject.TryDrawProperty(nameof(m_instance.bezierPainter), "Bezier Painter");

        private void GenSDFTexture()
        {
            var size = m_instance.size;
            var texSize = size * m_instance.texScale / 100;
            var sdfTex = new Texture2D(texSize.x, texSize.y, TEX_FORMAT, false);
            var options = m_instance.rasterizeOptions;
            var pixelBuffer = new NativeArray<byte>(texSize.x * texSize.y, Allocator.TempJob);

            m_instance.bezierPainter.GenSDFTexture(in pixelBuffer, size, texSize, options);

            PixelFormatUtil.LoadR8AsARGB32(in pixelBuffer, in sdfTex);

            m_instance.sdfTex = sdfTex;

            pixelBuffer.Dispose();
        }

        private void Preview()
        {
            switch (m_instance.previewMode)
            {
                case PreviewMode.Path:
                    m_instance.bezierPainter.DrawPath();
                    break;
                case PreviewMode.SDF:
                    if (m_instance.sdfTex)
                        EditorGUI.DrawPreviewTexture(m_area, m_instance.sdfTex);
                    break;
            }
        }
    }
}
