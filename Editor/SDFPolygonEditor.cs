/**
 * The creation of the polygon rounded corners was implemented using this thread as a reference
 * https://stackoverflow.com/questions/24771828/how-to-calculate-rounded-corners-for-a-polygon
 * https://zenn.dev/murnana/articles/unity-editor-imgui-popup
 * https://tori29.jp/blog/export_9
 * https://appleorbit.hatenablog.com/entry/2015/09/26/185352
 * https://docs.unity3d.com/ja/2021.2/ScriptReference/EditorGUILayout.PropertyField.html
 * https://docs.unity3d.com/ja/2022.1/Manual/class-TextureImporterOverride.html
 * */

#define USE_ARCTAN
#undef USE_ARCTAN

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.Jobs;
using Unity.Collections;
using CatlikeCoding.SDFToolkit;
using Nobi.UiRoundedCorners.RasterizeJob;

namespace TLab.UI.RoundedCorners.Editor
{
    [CustomEditor(typeof(SDFPolygon))]
    public class SDFPolygonEditor : UnityEditor.Editor
    {
        private SDFPolygon m_instance;

        const float DISC_RADIUS = 5.0f;

        const int CHANNEL_SIZE = 4;

        const TextureFormat TEX_FORMAT = TextureFormat.ARGB32;

        private string[] m_editModeOptions = new string[]
        {
            EditMode.CIRCLE.ToString(),
            EditMode.POLYGON.ToString(),
            EditMode.NONE.ToString()
        };

        private string[] m_previewModeOptions = new string[]
        {
            PreviewMode.VECTOR.ToString(),
            PreviewMode.RASTRIZED.ToString(),
            PreviewMode.SDF.ToString()
        };

        private Rect m_area;
        private Vector2 m_center;

        private void OnEnable()
        {
            m_instance = target as SDFPolygon;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            serializedObject.TryDrawProperty("width", "Vector Width");
            serializedObject.TryDrawProperty("height", "Vector Height");
            serializedObject.TryDrawProperty("sdfWidth", "SDF Width");
            serializedObject.TryDrawProperty("sdfHeight", "SDF Height");

            EditorGUILayout.Space();

            serializedObject.TryDrawProperty("rasterizeSettings", "Rasterize Settings");
            serializedObject.TryDrawProperty("fxaaSettings", "FXAA Settings");

            EditorGUILayout.Space();

            serializedObject.TryDrawProperty("circles", "Circles");
            serializedObject.TryDrawProperty("polygons", "Polygons");

            EditorGUILayout.Space();

            GetGUIRect();

            AddSegment();

            EditorGUILayout.Space();

            serializedObject.TryGetEnumValue("previewMode", out int index);

            switch ((PreviewMode)index)
            {
                case PreviewMode.VECTOR:
                    DrawMesh();
                    break;
                case PreviewMode.RASTRIZED:
                    if (serializedObject.TryGetObject("rasterizedTex", out Texture2D texture0))
                    {
                        EditorGUI.DrawPreviewTexture(m_area, texture0);
                    }
                    break;
                case PreviewMode.SDF:
                    if (serializedObject.TryGetObject("sdfTex", out Texture2D texture1))
                    {
                        EditorGUI.DrawPreviewTexture(m_area, texture1);
                    }
                    break;
            }
            EditorGUILayout.Space();

            // Vector Editor

            serializedObject.TryDrawEnumProperty("previewMode", "Preview Mode", m_previewModeOptions);
            serializedObject.TryDrawEnumProperty("editMode", "Edito Mode", m_editModeOptions);

            serializedObject.TryDrawProperty("showAnchors", "Show Anchors");
            serializedObject.TryDrawProperty("showSegments", "Show Segments");
            serializedObject.TryDrawProperty("radius", "Radius");

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Rasterize"))
            {
                Rasterize();
            }

            if (GUILayout.Button("GenerateSDF"))
            {
                GenerateSDF();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            serializedObject.TryDrawProperty("savePath", "Save Path");

            EditorGUILayout.Space();

            if (GUILayout.Button("Save"))
            {
                if (serializedObject.TryGetStringValue("savePath", out string savePath) && serializedObject.TryGetObject("sdfTex", out Texture2D sdfTex))
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

                    serializedObject.TrySetValue("savePath", savePath);
                }
            }

            if (GUILayout.Button("Save as"))
            {
                if (serializedObject.TryGetStringValue("savePath", out string savePath) && serializedObject.TryGetObject("sdfTex", out Texture2D sdfTex))
                {
                    if (!AssetUtil.DirectoryExists(m_instance.savePath))
                    {
                        if (AssetUtil.SelectSavePath(savePath, out savePath))
                        {
                            AssetUtil.SaveTexture(savePath, ref sdfTex);
                        }
                    }

                    serializedObject.TrySetValue("savePath", savePath);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void GetGUIRect()
        {
            m_area = EditorUtil.DrawPreviewArea(m_instance.width, m_instance.height);

            m_center = new Vector2(m_area.xMin + m_area.xMax, m_area.yMin + m_area.yMax) * 0.5f;
        }

        private Vector2 Transrate(Vector2 pos)
        {
            return new Vector2(((pos.x - m_center.x) / m_area.width * m_instance.width), ((pos.y - m_center.y) / m_area.height * m_instance.height));
        }

        private void AddSegment()
        {
            if (Event.current.rawType == EventType.MouseDown && Event.current.button == 0)
            {
                Vector2 input = Event.current.mousePosition;

                if (EditorUtil.InputAreaCheck(m_area, input))
                {
                    serializedObject.TryGetFloatValue("radius", out float radius);

                    switch (m_instance.editMode)
                    {
                        case EditMode.CIRCLE:
                            Circle circle = new Circle() { center = Transrate(input), radius = radius };
                            serializedObject.TryAddArrayElement("circles", circle);
                            break;
                        case EditMode.POLYGON:
                            Corner corner = new Corner() { position = Transrate(input), radius = radius };
                            if (serializedObject.TryGetValueList("polygons", out List<Polygon> polygons))
                            {
                                if (polygons.Count == 0)
                                {
                                    polygons.Add(new Polygon
                                    {
                                        offset = Vector2.zero,
                                        corners = new List<Corner>()
                                    });
                                }

                                polygons[polygons.Count - 1].corners.Add(corner);

                                serializedObject.TrySetValue("polygons", polygons);
                            }
                            break;
                        case EditMode.NONE:
                            break;
                    }
                }
            }
        }

        private void GenerateSDF()
        {
            Vector2Int textureSize = GetSDFTextureSize();

            serializedObject.TryGetObject("rasterizedTex", out Texture2D rasterizedTex);
            serializedObject.TryGetValue("rasterizeSettings", out RasterizeSettings rasterizeSettings);

            Texture2D sdfTex = new Texture2D(textureSize.x, textureSize.y, TEX_FORMAT, false);

            SDFTextureGenerator.Generate(rasterizedTex, sdfTex, rasterizeSettings.maxInside, rasterizeSettings.maxOutside, rasterizeSettings.postProcessDistance, RGBFillMode.Distance);

            sdfTex.Apply();

            serializedObject.TrySetValue("sdfTex", sdfTex);
        }

        private void Rasterize()
        {
            Vector2Int textureSize = GetTextureSize(), sdfTextureSize = GetSDFTextureSize();

            Texture2D rasterizedTex = new Texture2D(sdfTextureSize.x, sdfTextureSize.y, TEX_FORMAT, false);

            Vector2 offset, halfSize = new Vector2(textureSize.x * 0.5f, textureSize.y * 0.5f);

            NativeArray<byte> rasterized = new NativeArray<byte>(sdfTextureSize.x * sdfTextureSize.y, Allocator.TempJob);

            JobHandle handle;

            if (serializedObject.TryGetValueList("circles", out List<Circle> circles))
            {
                Debug.Log("Start circle rasterize");

                NativeArray<CircleN> circlesN = new NativeArray<CircleN>(circles.Count, Allocator.TempJob);
                for (int i = 0; i < circlesN.Length; i++)
                {
                    circlesN[i] = new CircleN
                    {
                        center = circles[i].center + halfSize,
                        radius = circles[i].radius
                    };
                }

                RasterizeCircleJob rasterizeCircle = new RasterizeCircleJob
                {
                    CIRCLES = circlesN,
                    X_RATIO = 1.0f / sdfTextureSize.x * textureSize.x,
                    Y_RATIO = 1.0f / sdfTextureSize.y * textureSize.y,
                    SDF_WIDTH = sdfTextureSize.x,
                    SDF_HEIGHT = sdfTextureSize.y,
                    result = rasterized
                };

                handle = rasterizeCircle.Schedule(rasterized.Length, 1);

                JobHandle.ScheduleBatchedJobs();

                handle.Complete();

                circlesN.Dispose();

                Debug.Log("Finish circle rasterize");
            }

            if (serializedObject.TryGetValueList("polygons", out List<Polygon> polygons))
            {
                Debug.Log("Start polygon rasterize");

                foreach (Polygon polygon in polygons)
                {
                    List<Vector3> points = new List<Vector3>();
                    List<Corner> corners = polygon.corners;
                    offset = polygon.offset + halfSize;
                    for (int i = 0; i < corners.Count; i++)
                    {
                        int j = (i + 1) % corners.Count;
                        int k = (i + 2) % corners.Count;
                        Corner corner0 = corners[i];
                        Corner corner1 = corners[j];
                        Corner corner2 = corners[k];

                        Vector2 p0 = corner0.position;
                        Vector2 p1 = corner1.position;
                        Vector2 p2 = corner2.position;
                        float radius = Mathf.Max(corner1.radius, 0.0f);

                        if (radius == 0.0f)
                        {
                            points.Add(p1 + offset);
                            continue;
                        }

                        Vector2 vec0To1 = p1 - p0;
                        Vector2 vec2To1 = p1 - p2;

                        float angle = Vector2.Angle(vec0To1, vec2To1) * Mathf.Deg2Rad * 0.5f;
                        float tan = Mathf.Abs(Mathf.Tan(angle));
                        float segment = radius / tan;

                        float pp0 = vec0To1.magnitude;
                        float pp2 = vec2To1.magnitude;

                        float min = Mathf.Min(pp0, pp2);
                        if (segment > min)
                        {
                            segment = min;
                            radius = segment * tan;
                        }

                        float po = Mathf.Sqrt(radius * radius + segment * segment);

                        Vector2 c0 = p1 - vec0To1 * segment / pp0;
                        Vector2 c2 = p1 - vec2To1 * segment / pp2;

                        Vector2 c = c0 + c2 - p1;
                        Vector2 d = p1 - c;
                        Vector2 o = p1 - d * po / d.magnitude;

                        float sweepAngle = Vector2.SignedAngle(c0 - o, c2 - o) * Mathf.Deg2Rad;

                        Vector2 from = c0 - o;

                        points.Add(c0 + offset);
                        float delta = Mathf.Sign(sweepAngle) * 0.1f;
                        for (float theta = 0.0f; Mathf.Abs(theta) < Mathf.Abs(sweepAngle); theta += delta)
                        {
                            points.Add(o + (Vector2)(Quaternion.Euler(0f, 0f, theta * Mathf.Rad2Deg) * from) + offset);
                        }
                        points.Add(c2 + offset);
                    }

                    NativeArray<Vector2> pointsN = new NativeArray<Vector2>(points.Count, Allocator.TempJob);
                    for (int i = 0; i < pointsN.Length; i++)
                    {
                        pointsN[i] = points[i];
                    }

                    RasterizePolygonJob rasterizePolygon = new RasterizePolygonJob
                    {
                        POLYGON = pointsN,
                        X_RATIO = 1.0f / sdfTextureSize.x * textureSize.x,
                        Y_RATIO = 1.0f / sdfTextureSize.y * textureSize.y,
                        SDF_WIDTH = sdfTextureSize.x,
                        SDF_HEIGHT = sdfTextureSize.y,
                        result = rasterized
                    };

                    handle = rasterizePolygon.Schedule(rasterized.Length, 1);

                    JobHandle.ScheduleBatchedJobs();

                    handle.Complete();

                    pointsN.Dispose();
                }

                Debug.Log("Finish polygon rasterize");
            }

            if (serializedObject.TryGetValue("fxaaSettings", out FXAASettings fxaaSettings))
            {
                Debug.Log("Start Fxaa effect");

                NativeArray<byte> aaData = new NativeArray<byte>(rasterized.Length, Allocator.TempJob);
                NativeArray<float> edgeStepSizes = new NativeArray<float>(10, Allocator.TempJob);
                edgeStepSizes[0] = 1.0f;
                edgeStepSizes[1] = 1.0f;
                edgeStepSizes[2] = 1.0f;
                edgeStepSizes[3] = 1.0f;
                edgeStepSizes[4] = 1.5f;
                edgeStepSizes[5] = 2.0f;
                edgeStepSizes[6] = 2.0f;
                edgeStepSizes[7] = 2.0f;
                edgeStepSizes[8] = 2.0f;
                edgeStepSizes[9] = 4.0f;

                FxaaEffect fxaaEffect = new FxaaEffect
                {
                    SOURCE = rasterized,
                    EDGE_STEP_SIZES = edgeStepSizes,
                    FXAA_CONFIG = new Vector4(fxaaSettings.fixedThreshold, fxaaSettings.relativeThreshold, fxaaSettings.subpixelBlending, 0.0f),
                    LAST_EDGE_STEP_GUESS = 8.0f,
                    WIDTH = sdfTextureSize.x,
                    HEIGHT = sdfTextureSize.y,

                    result = aaData
                };

                handle = fxaaEffect.Schedule(aaData.Length, 1);

                JobHandle.ScheduleBatchedJobs();

                handle.Complete();

                edgeStepSizes.Dispose();

                Debug.Log("Finish Fxaa effect");

                NativeArray<byte> rawData = new NativeArray<byte>(aaData.Length * CHANNEL_SIZE, Allocator.TempJob);

                CopyR8ToARGB32 copyR8ToARGB32 = new CopyR8ToARGB32
                {
                    SOURCE = aaData,
                    CHANNEL_SIZE = CHANNEL_SIZE,
                    result = rawData
                };

                handle = copyR8ToARGB32.Schedule(rawData.Length, 1);

                JobHandle.ScheduleBatchedJobs();

                handle.Complete();

                rasterizedTex.LoadRawTextureData(rawData);
                rasterizedTex.Apply();

                serializedObject.TrySetValue("rasterizedTex", rasterizedTex);

                aaData.Dispose();
                rawData.Dispose();
            }

            rasterized.Dispose();
        }

        private Vector2Int GetTextureSize()
        {
            Vector2Int textureSize = new Vector2Int();

            if (serializedObject.TryGetIntValue("width", out int x))
            {
                textureSize.x = x;
            }

            if (serializedObject.TryGetIntValue("height", out int y))
            {
                textureSize.y = y;
            }

            return textureSize;
        }

        private Vector2Int GetSDFTextureSize()
        {
            Vector2Int textureSize = new Vector2Int();

            if (serializedObject.TryGetIntValue("sdfWidth", out int x))
            {
                textureSize.x = x;
            }

            if (serializedObject.TryGetIntValue("sdfHeight", out int y))
            {
                textureSize.y = y;
            }

            return textureSize;
        }

        private void DrawMesh()
        {
            List<Vector2> anchors = new List<Vector2>();

            Vector2Int textureSize = GetTextureSize();

            serializedObject.TryGetBoolValue("showSegments", out bool showSegments);
            serializedObject.TryGetBoolValue("showAnchors", out bool showAnchors);

            if (serializedObject.TryGetValueList("circles", out List<Circle> circles))
            {
                foreach (Circle circle in circles)
                {
                    Vector2 anchor = EditorUtil.TexturePositionToRectPosition(m_area, textureSize, circle.center) + m_center;
                    Handles.DrawSolidDisc(anchor, Vector3.forward, EditorUtil.TextureUnitToRectUnit(m_area, textureSize, circle.radius));
                    anchors.Add(anchor);
                }
            }

            if (serializedObject.TryGetValueList("polygons", out List<Polygon> polygons))
            {
                foreach (Polygon polygon in polygons)
                {
                    List<Vector3> points = new List<Vector3>();

                    // Drawing in editor window
                    List<Corner> corners = polygon.corners;
                    Vector2 offset = EditorUtil.TexturePositionToRectPosition(m_area, textureSize, polygon.offset) + m_center;
                    for (int i = 0; i < corners.Count; i++)
                    {
                        int j = (i + 1) % corners.Count;
                        int k = (i + 2) % corners.Count;
                        Corner corner0 = corners[i];
                        Corner corner1 = corners[j];
                        Corner corner2 = corners[k];

                        Vector2 p0 = EditorUtil.TexturePositionToRectPosition(m_area, textureSize, corner0.position);
                        Vector2 p1 = EditorUtil.TexturePositionToRectPosition(m_area, textureSize, corner1.position);
                        Vector2 p2 = EditorUtil.TexturePositionToRectPosition(m_area, textureSize, corner2.position);
                        float radius = Mathf.Max(EditorUtil.TextureUnitToRectUnit(m_area, textureSize, corner1.radius), 0.0f);

                        if (radius == 0.0f)
                        {
                            points.Add(p1 + offset);
                            anchors.Add(p1 + offset);

                            continue;
                        }

                        Vector2 vec0To1 = p1 - p0;
                        Vector2 vec2To1 = p1 - p2;
#if USE_ARCTAN
                        float angle = (Mathf.Atan2(vec0To1.y, vec0To1.x) - Mathf.Atan2(vec2To1.y, vec2To1.x)) * 0.5f;
#else
                        float angle = Vector2.Angle(vec0To1, vec2To1) * Mathf.Deg2Rad * 0.5f;
#endif
                        float tan = Mathf.Abs(Mathf.Tan(angle));
                        float segment = radius / tan;

                        float pp0 = vec0To1.magnitude;
                        float pp2 = vec2To1.magnitude;

                        float min = Mathf.Min(pp0, pp2);
                        if (segment > min)
                        {
                            segment = min;
                            radius = segment * tan;
                        }

                        float po = Mathf.Sqrt(radius * radius + segment * segment);

                        Vector2 c0 = p1 - vec0To1 * segment / pp0;
                        Vector2 c2 = p1 - vec2To1 * segment / pp2;

                        Vector2 c = c0 + c2 - p1;
                        Vector2 d = p1 - c;
                        Vector2 o = p1 - d * po / d.magnitude;

#if USE_ARCTAN
                        float angle0 = Mathf.Atan2(c0.y - o.y, c0.x - o.x);
                        float angle1 = Mathf.Atan2(c2.y - o.y, c2.x - o.x);
                        float sweepAngle = angle1 - angle0;
#else
                        float sweepAngle = Vector2.SignedAngle(c0 - o, c2 - o) * Mathf.Deg2Rad;
#endif
                        Vector2 from = c0 - o;
#if USE_ARCTAN
                        if (sweepAngle < 0)
                        {
                            from = c2 - o;
                            sweepAngle = -sweepAngle;
                        }

                        if (sweepAngle > Mathf.PI)
                        {
                            sweepAngle = Mathf.PI - sweepAngle;
                        }
#endif
                        points.Add(c0 + offset);
                        float delta = Mathf.Sign(sweepAngle) * 0.1f;
                        for (float theta = 0.0f; Mathf.Abs(theta) < Mathf.Abs(sweepAngle); theta += delta)
                        {
                            if (showSegments)
                            {
                                Handles.color = Color.green;
                                Handles.DrawSolidDisc(o + (Vector2)(Quaternion.Euler(0f, 0f, theta * Mathf.Rad2Deg) * from) + offset, -Vector3.forward, DISC_RADIUS * 0.5f);
                                Handles.color = Color.white;
                            }
                            points.Add(o + (Vector2)(Quaternion.Euler(0f, 0f, theta * Mathf.Rad2Deg) * from) + offset);
                        }
                        points.Add(c2 + offset);
                        if (showSegments)
                        {
                            Handles.color = Color.red;
                            Handles.DrawSolidDisc(c0 + offset, -Vector3.forward, DISC_RADIUS);
                            Handles.color = Color.yellow;
                            Handles.DrawSolidDisc(c2 + offset, -Vector3.forward, DISC_RADIUS);
                            Handles.color = Color.white;
                        }

                        anchors.Add(p1 + offset);
                    }
                    Handles.DrawAAConvexPolygon(points.ToArray());
                }
            }

            if (showAnchors)
            {
                Handles.color = Color.blue;
                foreach (Vector2 anchor in anchors)
                {
                    Handles.DrawSolidDisc(anchor, -Vector3.forward, DISC_RADIUS);
                }
                Handles.color = Color.white;
            }
        }
    }
}
