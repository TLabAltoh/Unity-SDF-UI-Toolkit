using UnityEngine;
using UnityEditor;

namespace TLab.UI.SDF.Editor
{
    public static class EditorUtil
    {
        public static string THIS_NAME => "[EditorUtil] ";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aspect"></param>
        /// <returns></returns>
        public static Rect CreatePreviewArea(float aspect)
        {
            var margin = 0.8f;
            var xoffset = Screen.width * (1 - margin) * 0.25f;

            var area = GUILayoutUtility.GetRect(Screen.width * margin, Screen.width * margin * aspect, GUILayout.ExpandWidth(false));
            area.xMax += xoffset;
            area.x += xoffset;

            Handles.DrawSolidRectangleWithOutline(area, Color.black, Color.white);

            return area;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="area"></param>
        /// <returns></returns>
        public static bool CheckArea(Vector2 position, Rect area)
        {
            return (position.x >= area.xMin) && (position.x <= area.xMax) && (position.y >= area.yMin) && (position.y <= area.yMax);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rect"></param>
        /// <param name="rectTransformSize"></param>
        /// <returns></returns>
        public static Vector2 RectToRectTransform(Vector2 position, Rect rect, Vector2 rectTransformSize)
        {
            var x = (position.x - rect.xMin) / rect.width;
            var y = (position.y - rect.yMin) / rect.height;

            return new Vector2((x - 0.5f) * rectTransformSize.x, (y - 0.5f) * rectTransformSize.y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rect"></param>
        /// <param name="rectTransformSize"></param>
        /// <returns></returns>
        public static Vector2 RectTransformToRect(Vector2 position, Rect rect, Vector2 rectTransformSize)
        {
            var x = (position.x + rectTransformSize.x * 0.5f) / rectTransformSize.x;
            var y = (position.y + rectTransformSize.y * 0.5f) / rectTransformSize.y;

            return new Vector2((1f - x) * rect.xMin + x * rect.xMax, (1f - y) * rect.yMin + y * rect.yMax);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rect"></param>
        /// <param name="texSize"></param>
        /// <returns></returns>
        public static Vector2 TextureToRect(Vector2 position, Rect rect, Vector2Int texSize)
        {
            var center = GetCenter(rect);

            return center + new Vector2(position.x / texSize.x * rect.width, position.y / texSize.y * rect.height);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="positions"></param>
        /// <param name="rect"></param>
        /// <param name="texSize"></param>
        /// <returns></returns>
        public static Vector2[] TextureToRect(Vector2[] positions, Rect rect, Vector2Int texSize)
        {
            var result = new Vector2[positions.Length];

            var center = GetCenter(rect);

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = center + new Vector2(positions[i].x / texSize.x * rect.width, positions[i].y / texSize.y * rect.height);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="rect"></param>
        /// <param name="texSize"></param>
        /// <returns></returns>
        public static float TextureToRect(float value, Rect rect, Vector2Int texSize)
        {
            return value / texSize.x * rect.width;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="rect"></param>
        /// <param name="texSize"></param>
        /// <returns></returns>
        public static float[] TextureToRect(float[] values, Rect rect, Vector2Int texSize)
        {
            var result = new float[values.Length];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = TextureToRect(values[i], rect, texSize);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="area"></param>
        /// <param name="texSize"></param>
        /// <returns></returns>
        public static Vector2 RectToTexture(Vector2 position, Rect area, Vector2Int texSize)
        {
            var center = GetCenter(area);

            return new Vector2(((position.x - center.x) / area.width * texSize.x), ((position.y - center.y) / area.height * texSize.y));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        public static Vector2 GetCenter(Rect area)
        {
            return new Vector2(area.xMin + area.xMax, area.yMin + area.yMax) * 0.5f;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startPosition"></param>
        /// <param name="endPosition"></param>
        /// <param name="startTangent"></param>
        /// <param name="endTangent"></param>
        public static void DrawBezier(Vector3 startPosition, Vector3 endPosition, Vector3 startTangent, Vector3 endTangent)
        {
            Handles.DrawBezier(startPosition, endPosition, startTangent, endTangent, Handles.color, null, 2.0f);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="closed"></param>
        public static void DrawBezier(Vector3[] points, bool closed)
        {
            if (closed)
            {
                DrawBezier(points[points.Length - 2], points[1], points[points.Length - 1], points[0]);
            }

            for (int i = 3; i < points.Length; i += 3)
            {
                DrawBezier(points[i - 2], points[i + 1], points[i - 1], points[i]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="xSize"></param>
        /// <param name="ySize"></param>
        public static void DrawCube(Vector3 center, float xSize, float ySize)
        {
            Handles.DrawAAConvexPolygon(center + new Vector3(xSize, ySize, 0),
                center + new Vector3(xSize, -ySize, 0),
                center + new Vector3(-xSize, -ySize, 0),
                center + new Vector3(-xSize, ySize, 0));
        }
    }
}
