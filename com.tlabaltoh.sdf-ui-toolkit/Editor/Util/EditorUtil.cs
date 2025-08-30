using UnityEngine;
using UnityEditor;

namespace TLab.UI.SDF.Editor
{
    public static class EditorUtil
    {
        public static string THIS_NAME => "[EditorUtil] ";

        public static Rect CreatePreviewArea(float aspect, Color faceColor, Color outlineColor)
        {
            var area = GUILayoutUtility.GetAspectRect(aspect);
            DrawQuadArea(area, faceColor, outlineColor);
            return area;
        }

        public static void DrawQuadArea(Rect area, Color faceColor, Color outlineColor) =>
            Handles.DrawSolidRectangleWithOutline(area, faceColor, outlineColor);

        public static bool IsInTheArea(Vector2 pos, Rect area) => (pos.x >= area.xMin) && (pos.x <= area.xMax) && (pos.y >= area.yMin) && (pos.y <= area.yMax);

        public static bool MouseIsInTheArea(Rect area) => IsInTheArea(Event.current.mousePosition, area);

        public static Vector2 RectToRectTransform(Vector2 pos, Rect rect, Vector2 rectTransformSize)
        {
            var x = (pos.x - rect.xMin) / rect.width;
            var y = (pos.y - rect.yMin) / rect.height;

            return new Vector2((x - 0.5f) * rectTransformSize.x, (y - 0.5f) * rectTransformSize.y);
        }

        public static Vector2 RectTransformToRect(Vector2 pos, Rect rect, Vector2 rectTransformSize)
        {
            var x = (pos.x + rectTransformSize.x * 0.5f) / rectTransformSize.x;
            var y = (pos.y + rectTransformSize.y * 0.5f) / rectTransformSize.y;

            return new Vector2((1f - x) * rect.xMin + x * rect.xMax, (1f - y) * rect.yMin + y * rect.yMax);
        }

        public static Vector2 ActualPosToRect(Vector2 pos, Rect rect, Vector2Int size, float areaZoom, Vector2Int areaPos) => GetCenterOfRect(rect) + (pos + areaPos) / ((Vector2)size * areaZoom) * rect.size;

        public static Vector2[] ActualPosToRect(Vector2[] pos, Rect rect, Vector2Int size, float areaZoom, Vector2Int areaPos)
        {
            var result = new Vector2[pos.Length];
            var center = GetCenterOfRect(rect);
            for (int i = 0; i < result.Length; i++)
                result[i] = center + (pos[i] + areaPos) / ((Vector2)size * areaZoom) * rect.size;
            return result;
        }

        public static Vector2 RectToActualVec(Vector2 vec, Rect area, Vector2Int size, float areaZoom) => vec / area.size * ((Vector2)size * areaZoom);

        public static Vector2 RectToActualPos(Vector2 pos, Rect area, Vector2Int size, float areaZoom, Vector2Int areaPos) => (pos - GetCenterOfRect(area)) / area.size * ((Vector2)size * areaZoom) - areaPos;

        public static Vector2 GetCenterOfRect(Rect area) => new Vector2(area.xMin + area.xMax, area.yMin + area.yMax) * 0.5f;

        public static void DrawBezier(Vector3[] points, bool isClosed, float width = 2.0f)
        {
            if (isClosed)
                Handles.DrawBezier(points[points.Length - 2], points[1], points[points.Length - 1], points[0], Handles.color, null, width);

            for (int i = 3; i < points.Length; i += 3)
                Handles.DrawBezier(points[i - 2], points[i + 1], points[i - 1], points[i], Handles.color, null, width);
        }

        public static void DrawCube(Vector3 center, float xSize, float ySize)
        {
            var c0 = new Vector3(+xSize, +ySize, 0);
            var c1 = new Vector3(+xSize, -ySize, 0);
            var c2 = new Vector3(-xSize, -ySize, 0);
            var c3 = new Vector3(-xSize, +ySize, 0);
            Handles.DrawAAConvexPolygon(center + c0, center + c1, center + c2, center + c3);
        }
    }
}
