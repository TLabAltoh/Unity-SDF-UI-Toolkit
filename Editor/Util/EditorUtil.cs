using UnityEngine;
using UnityEditor;

namespace Nobi.UiRoundedCorners.Editor
{
    public static class EditorUtil
    {
        public static string THIS_NAME => "[EditorUtil] ";

        public static Rect DrawPreviewArea(float width, float height)
        {
            var margin = 0.8f;
            var xoffset = Screen.width * (1 - margin) * 0.25f;

            var area = GUILayoutUtility.GetRect(Screen.width * margin, Screen.width * margin * height / width, GUILayout.ExpandWidth(false));
            area.xMax += xoffset;
            area.x += xoffset;

            Handles.DrawSolidRectangleWithOutline(area, Color.black, Color.white);

            return area;
        }

        public static bool InputAreaCheck(Rect rect, Vector2 input)
        {
            return (input.x >= rect.xMin) && (input.x <= rect.xMax) && (input.y >= rect.yMin) && (input.y <= rect.yMax);
        }

        public static Vector2 ScreenToRectTransformPosition(Rect rect, Vector2 screenPos, Vector2 rectTransformSize)
        {
            Vector2 halfRectTransformSize = new Vector2(rectTransformSize.x * 0.5f, rectTransformSize.y * 0.5f);

            float x = (screenPos.x - rect.xMin) / rect.width;
            float y = (screenPos.y - rect.yMin) / rect.height;

            return new Vector2(x * rectTransformSize.x - halfRectTransformSize.x, y * rectTransformSize.y - halfRectTransformSize.y);
        }

        public static Vector2 RectTransformPositionToScreen(Rect rect, Vector2 rectTransformPosition, Vector2 rectTransformSize)
        {
            Vector2 halfRectTransformSize = new Vector2(rectTransformSize.x * 0.5f, rectTransformSize.y * 0.5f);

            float x = (rectTransformPosition.x + halfRectTransformSize.x) / rectTransformSize.x;
            float y = (rectTransformPosition.y + halfRectTransformSize.y) / rectTransformSize.y;

            return new Vector2((1f - x) * rect.xMin + x * rect.xMax, (1f - y) * rect.yMin + y * rect.yMax);
        }

        public static Vector2 TexturePositionToRectPosition(Rect rect, Vector2Int textureSize, Vector2 texturePosition)
        {
            return new Vector2(texturePosition.x / textureSize.x * rect.width, texturePosition.y / textureSize.y * rect.height);
        }

        public static float TextureUnitToRectUnit(Rect rect, Vector2Int textureSize, float textureUnit)
        {
            return textureUnit / textureSize.x * rect.width;
        }
    }
}
