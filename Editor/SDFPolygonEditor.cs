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
#define DEBUG
//#undef DEBUG

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Unity.Jobs;
using Unity.Collections;
using CatlikeCoding.SDFToolkit;

namespace Nobi.UiRoundedCorners.Editor
{
#if UNITY_EDITOR
    [CustomEditor(typeof(SDFPolygon))]
    public class SDFPolygonEditor : UnityEditor.Editor
    {
        private SDFPolygon instance;
#if DEBUG
        const float DISC_RADIUS = 5.0f;
#endif
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

        public override void OnInspectorGUI()
        {
            if (instance == null)
            {
                instance = target as SDFPolygon;
            }

            serializedObject.Update();

            SerializedProperty prop;
            
            prop = serializedObject.FindProperty("width");
            EditorGUILayout.PropertyField(prop, new GUIContent("Vector Width"));

            prop = serializedObject.FindProperty("height");
            EditorGUILayout.PropertyField(prop, new GUIContent("Vector Height"));

            prop = serializedObject.FindProperty("sdfWidth");
            EditorGUILayout.PropertyField(prop, new GUIContent("SDF Width"));

            prop = serializedObject.FindProperty("sdfHeight");
            EditorGUILayout.PropertyField(prop, new GUIContent("SDF Height"));

            EditorGUILayout.Space();

            prop = serializedObject.FindProperty("maxInside");
            EditorGUILayout.PropertyField(prop, new GUIContent("Max Inside"));

            prop = serializedObject.FindProperty("maxOutside");
            EditorGUILayout.PropertyField(prop, new GUIContent("Max Outside"));

            prop = serializedObject.FindProperty("postProcessDistance");
            EditorGUILayout.PropertyField(prop, new GUIContent("Post Process Distance"));

            EditorGUILayout.Space();

            prop = serializedObject.FindProperty("circles");
            EditorGUILayout.PropertyField(prop, new GUIContent("Circles"));

            prop = serializedObject.FindProperty("polygons");
            EditorGUILayout.PropertyField(prop, new GUIContent("Polygons"));

            EditorGUILayout.Space();
            GetGUIRect();

            AddSegment();            

            EditorGUILayout.Space();
            switch (instance.previewMode)
            {
                case PreviewMode.VECTOR:
                    DrawMesh();
                    break;
                case PreviewMode.RASTRIZED:
                    if (instance.rasterizedTex)
                    {
                        EditorGUI.DrawPreviewTexture(m_area, instance.rasterizedTex);
                    }
                    break;
                case PreviewMode.SDF:
                    if (instance.sdfTex)
                    {
                        EditorGUI.DrawPreviewTexture(m_area, instance.sdfTex);
                    }
                    break;
            }            
            EditorGUILayout.Space();

            instance.previewMode = (PreviewMode)EditorGUILayout.Popup("Preview Mode", (int)instance.previewMode, m_previewModeOptions);
            instance.editMode = (EditMode)EditorGUILayout.Popup("Edito Mode", (int)instance.editMode, m_editModeOptions);

            prop = serializedObject.FindProperty("showAnchors");
            EditorGUILayout.PropertyField(prop, new GUIContent("Show Anchors"));

            prop = serializedObject.FindProperty("showSegments");
            EditorGUILayout.PropertyField(prop, new GUIContent("Show Segments"));

            prop = serializedObject.FindProperty("radius");
            EditorGUILayout.PropertyField(prop, new GUIContent("Radius"));

            EditorGUILayout.BeginHorizontal();
            if(GUILayout.Button("Rasterize"))
            {
                Rasterize();
            }

            if (GUILayout.Button("GenerateSDF"))
            {
                if (instance.rasterizedTex)
                {
                    GenerateSDF();
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            prop = serializedObject.FindProperty("savePath");
            EditorGUILayout.PropertyField(prop, new GUIContent("Save Path"));

            EditorGUILayout.Space();

            if (GUILayout.Button("Save"))
            {
                if(instance.savePath == "" || !instance.sdfTex || !DirectoryExists(instance.savePath))
                {
                    if (SelectSavePath())
                    {
                        SaveTexture();
                    }
                }
                else
                {
                    SaveTexture();
                }
            }

            if(GUILayout.Button("Save as"))
            {
                if (SelectSavePath())
                {
                    SaveTexture();
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private struct RasterizePolygonJob : IJobParallelFor
        {
            [ReadOnly] public NativeArray<Vector2> polygon;
            [ReadOnly] public float xRatio;
            [ReadOnly] public float yRatio;
            [ReadOnly] public int sdfWidth;
            [ReadOnly] public int sdfHeight;

            public NativeArray<byte> result;

            public void Execute(int index)
            {
                int idxY = sdfHeight - index / sdfWidth - 1;
                int idxX = index % sdfWidth;

                float texX = idxX * xRatio;
                float texY = idxY * yRatio;

                float sum = 0;

                for(int i = 0; i < polygon.Length; i++)
                {
                    Vector2 point0 = polygon[(i + 0) % polygon.Length];
                    Vector2 point1 = polygon[(i + 1) % polygon.Length];
                    float dx0 = point0.x - texX;
                    float dy0 = point0.y - texY;
                    float dx1 = point1.x - texX;
                    float dy1 = point1.y - texY;

                    float dot = (dx0 * dx1 + dy0 * dy1) / (Mathf.Sqrt(dx0 * dx0 + dy0 * dy0) * Mathf.Sqrt(dx1 * dx1 + dy1 * dy1));
                    dot = Mathf.Clamp(dot, -1.0f, 1.0f);
                    float theta = Mathf.Acos(dot);
                    theta *= Mathf.Sign(dx0 * dy1 - dy0 * dx1);
                    sum += theta;
                }

                if(Mathf.Abs(sum) > Mathf.PI)
                {
                    result[index] = 255;
                }
            }
        }

        private struct RasterizeCircleJob: IJobParallelFor
        {
            [ReadOnly] public NativeArray<Circle> circles;
            [ReadOnly] public float xRatio;
            [ReadOnly] public float yRatio;
            [ReadOnly] public int sdfWidth;
            [ReadOnly] public int sdfHeight;

            public NativeArray<byte> result;

            public void Execute(int index)
            {
                int idxY = sdfHeight - index / sdfWidth - 1;
                int idxX = index % sdfWidth;

                float texX = idxX * xRatio;
                float texY = idxY * yRatio;

                for (int i = 0; i < circles.Length; i++)
                {
                    Circle circle = circles[i];
                    float dx = circle.center.x - texX;
                    float dy = circle.center.y - texY;

                    if(dx * dx + dy * dy < circle.radius * circle.radius)
                    {
                        result[index] = 255;
                    }
                }
            }
        }

        private struct CopyR8ToARGB32 : IJobParallelFor
        {
            [ReadOnly] public int channelSize;
            [ReadOnly] public NativeArray<byte> source;
            public NativeArray<byte> result;

            public void Execute(int index)
            {
                result[index] = source[index / channelSize];
            }
        }

        private void GetGUIRect()
        {
            float margin = 0.8f;
            float width = Screen.width * margin;
            float height = width * instance.height / instance.width;
            float xoffset = Screen.width * (1 - margin) * 0.25f;

            m_area = GUILayoutUtility.GetRect(width, height, GUILayout.ExpandWidth(false));
            m_area.xMax += xoffset;
            m_area.xMin += xoffset;

            m_center = new Vector2(m_area.xMin + m_area.xMax, m_area.yMin + m_area.yMax) * 0.5f;

            // Draw bounds
            Handles.DrawLine(new Vector2(m_area.xMax + 1, m_area.yMin - 1), new Vector2(m_area.xMax + 1, m_area.yMax + 1));
            Handles.DrawLine(new Vector2(m_area.xMin - 1, m_area.yMax + 1), new Vector2(m_area.xMax + 1, m_area.yMax + 1));
            Handles.DrawLine(new Vector2(m_area.xMin - 1, m_area.yMax + 1), new Vector2(m_area.xMin - 1, m_area.yMin - 1));
            Handles.DrawLine(new Vector2(m_area.xMax + 1, m_area.yMin - 1), new Vector2(m_area.xMin - 1, m_area.yMin - 1));
        }

        private Vector2 Transrate(Vector2 pos)
        {
            return new Vector2(((pos.x - m_center.x) / m_area.width * instance.width), ((pos.y - m_center.y) / m_area.height * instance.height));
        }

        private void Undo()
        {
            UnityEditor.Undo.RecordObject(instance, "Add Segment");
        }

        private void AddSegment()
        {
            if (Event.current.rawType == EventType.MouseDown && Event.current.button == 0)
            {
                Vector2 input = Event.current.mousePosition;

                if (Mathf.Abs(input.x - m_center.x) < m_area.width * 0.5f && Mathf.Abs(input.y - m_center.y) < m_area.height * 0.5f)
                {
                    switch (instance.editMode)
                    {
                        case EditMode.CIRCLE:
                            Undo();
                            instance.AddCircle(new Circle() { center = Transrate(input), radius = instance.radius });
                            break;
                        case EditMode.POLYGON:
                            Undo();
                            instance.AddCorner(new Corner() { position = Transrate(input), radius = instance.radius });
                            break;
                        case EditMode.NONE:
                            break;
                    }
                }

                EditorUtility.SetDirty(instance);
            }
        }

        private Vector2 TexToRect(Vector2 coord)
        {
            return new Vector2(coord.x / instance.width * m_area.width, coord.y / instance.height * m_area.height);
        }

        private float TexToRect(float size)
        {
            return size / instance.width * m_area.width;
        }

        private string GetFileName(string path, bool ignore)
        {
            string fileName = Path.GetFileName(instance.savePath);
            if (ignore)
            {
                string[] split = fileName.Split('.');
                if (split.Length > 1 && !split[split.Length - 1].Contains('\\') && !split[split.Length - 1].Contains('/'))
                {
                    return split[split.Length - 2];
                }
            }
            return fileName;
        }

        public void SaveTexture()
        {
            string savePath = instance.savePath;
            if (Path.GetExtension(instance.savePath) != ".asset")
            {
                savePath += ".asset";
            }

            Texture2D asset = AssetDatabase.LoadAssetAtPath<Texture2D>(savePath);

            instance.sdfTex.name = GetFileName(savePath, true);

            if (asset != null)
            {
                asset.name = instance.sdfTex.name;
                EditorUtility.CopySerialized(instance.sdfTex, asset);
            }
            else
            {
                AssetDatabase.CreateAsset(instance.sdfTex, savePath);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.SetDirty(instance);
        }

        private string GetDiskPath(string assetPath)
        {
            string assetDir = assetPath.Substring(0, assetPath.Length - 1 - Path.GetFileName(assetPath).Length);
            return Directory.GetCurrentDirectory() + "\\" + assetDir.Replace("/", "\\");
        }

        private bool DirectoryExists(string assetPath)
        {
            if (assetPath.Length < "Assets".Length - 1)
            {
                return false;
            }

            return Directory.Exists(GetDiskPath(assetPath));
        }

        public bool SelectSavePath()
        {
            string initialPath = instance.savePath != null && DirectoryExists(instance.savePath) ? instance.savePath : "Assets";
            string fullPath = EditorUtility.SaveFilePanel("Save Path", initialPath, "", "asset");
            if(fullPath == "")
            {
                return false;
            }
            string savePath = fullPath.Remove(0, Directory.GetCurrentDirectory().Length + 1);
            instance.savePath = savePath;
            return true;
        }

        private void GenerateSDF()
        {
            instance.sdfTex = new Texture2D(instance.sdfWidth, instance.sdfHeight, TEX_FORMAT, false);

            SDFTextureGenerator.Generate(instance.rasterizedTex, instance.sdfTex, instance.maxInside, instance.maxOutside, instance.postProcessDistance, RGBFillMode.Distance);

            instance.sdfTex.Apply();

            EditorUtility.SetDirty(instance);
        }

        private void Rasterize()
        {
            instance.rasterizedTex = new Texture2D(instance.sdfWidth, instance.sdfHeight, TEX_FORMAT, false);

            Vector2 halfSize = new Vector2(instance.width * 0.5f, instance.height * 0.5f);
            Vector2 offset = Vector2.zero;

            NativeArray<byte> result = new NativeArray<byte>(instance.sdfHeight * instance.sdfWidth, Allocator.TempJob);
            JobHandle handle;

            NativeArray<Circle> circlesN = new NativeArray<Circle>(instance.circles.Count, Allocator.TempJob);
            for (int i = 0; i < circlesN.Length; i++)
            {
                circlesN[i] = new Circle {
                    center = instance.circles[i].center + halfSize,
                    radius = instance.circles[i].radius
                };
            }

            RasterizeCircleJob rasterizeCircle = new RasterizeCircleJob
            {
                circles = circlesN,
                xRatio = 1.0f / instance.sdfWidth * instance.width,
                yRatio = 1.0f / instance.sdfHeight * instance.height,
                sdfWidth = instance.sdfWidth,
                sdfHeight = instance.sdfHeight,
                result = result
            };

            handle = rasterizeCircle.Schedule(result.Length, 1);

            JobHandle.ScheduleBatchedJobs();

            handle.Complete();

            circlesN.Dispose();

            Debug.Log("Finish circle rasterize");

            foreach (Polygon polygon in instance.polygons)
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
                    polygon = pointsN,
                    xRatio = 1.0f / instance.sdfWidth * instance.width,
                    yRatio = 1.0f / instance.sdfHeight * instance.height,
                    sdfWidth = instance.sdfWidth,
                    sdfHeight = instance.sdfHeight,
                    result = result
                };

                handle = rasterizePolygon.Schedule(result.Length, 1);

                JobHandle.ScheduleBatchedJobs();

                handle.Complete();

                pointsN.Dispose();
            }

            Debug.Log("Finish polygon rasterize");

            NativeArray<byte> rawData = new NativeArray<byte>(result.Length * CHANNEL_SIZE, Allocator.TempJob);

            CopyR8ToARGB32 copyR8ToARGB32 = new CopyR8ToARGB32
            {
                channelSize = CHANNEL_SIZE,
                source = result,
                result = rawData
            };

            handle = copyR8ToARGB32.Schedule(rawData.Length, 1);

            JobHandle.ScheduleBatchedJobs();

            handle.Complete();

            instance.rasterizedTex.LoadRawTextureData(rawData);
            instance.rasterizedTex.Apply();

            result.Dispose();
            rawData.Dispose();

            EditorUtility.SetDirty(instance);
        }

        private void DrawMesh()
        {
            Handles.color = Color.black;
            Handles.DrawAAConvexPolygon(
                new Vector2(m_area.xMax, m_area.yMin), new Vector2(m_area.xMax, m_area.yMax),
                new Vector2(m_area.xMin, m_area.yMax), new Vector2(m_area.xMax, m_area.yMax),
                new Vector2(m_area.xMin, m_area.yMax), new Vector2(m_area.xMin, m_area.yMin),
                new Vector2(m_area.xMax, m_area.yMin), new Vector2(m_area.xMin, m_area.yMin));
            Handles.color = Color.white;

            List<Vector2> anchors = new List<Vector2>();

            if (instance.circles != null)
            {
                // Drawing in editor window
                foreach (Circle circle in instance.circles)
                {
                    Vector2 anchor = TexToRect(circle.center) + m_center;
                    Handles.DrawSolidDisc(anchor, Vector3.forward, TexToRect(circle.radius));
                    anchors.Add(anchor);
                }
            }

            if (instance.polygons != null)
            {
                foreach (Polygon polygon in instance.polygons)
                {
                    List<Vector3> points = new List<Vector3>();

                    // Drawing in editor window
                    List<Corner> corners = polygon.corners;
                    Vector2 offset = TexToRect(polygon.offset) + m_center;
                    for (int i = 0; i < corners.Count; i++)
                    {
                        int j = (i + 1) % corners.Count;
                        int k = (i + 2) % corners.Count;
                        Corner corner0 = corners[i];
                        Corner corner1 = corners[j];
                        Corner corner2 = corners[k];

                        Vector2 p0 = TexToRect(corner0.position);
                        Vector2 p1 = TexToRect(corner1.position);
                        Vector2 p2 = TexToRect(corner2.position);
                        float radius = Mathf.Max(TexToRect(corner1.radius), 0.0f);

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
                        for(float theta = 0.0f; Mathf.Abs(theta) < Mathf.Abs(sweepAngle); theta += delta)
                        {
                            if (instance.showSegments)
                            {
                                Handles.color = Color.green;
                                Handles.DrawSolidDisc(o + (Vector2)(Quaternion.Euler(0f, 0f, theta * Mathf.Rad2Deg) * from) + offset, -Vector3.forward, DISC_RADIUS * 0.5f);
                                Handles.color = Color.white;
                            }
                            points.Add(o + (Vector2)(Quaternion.Euler(0f, 0f, theta * Mathf.Rad2Deg) * from) + offset);
                        }
                        points.Add(c2 + offset);

                        if (instance.showSegments)
                        {
                            Handles.color = Color.red;
                            Handles.DrawSolidDisc(c0 + offset, -Vector3.forward, DISC_RADIUS);
                            Handles.color = Color.yellow;
                            Handles.DrawSolidDisc(c2 + offset, -Vector3.forward, DISC_RADIUS);
                            Handles.color = Color.white;
                        }

                        anchors.Add(p1 + offset);
                        // Handles.DrawSolidArc(o + offset, Vector3.forward, from.normalized, sweepAngle * Mathf.Rad2Deg, radius);
                    }
                    Handles.DrawAAConvexPolygon(points.ToArray());
                }
            }

            if (instance.showAnchors)
            {
                Handles.color = Color.blue;
                foreach(Vector2 anchor in anchors)
                {
                    Handles.DrawSolidDisc(anchor, -Vector3.forward, DISC_RADIUS);
                }
                Handles.color = Color.white;
            }
        }
    }
#endif
}