using UnityEngine;
using UnityEditor;

namespace TLab.UI.SDF.Editor
{
    [System.Serializable]
    public class ShapePainter
    {
        protected Object m_texPainter;
        protected Rect m_area;
        protected Vector2Int m_size;
        protected Vector2Int m_texSize;

        public virtual void Update(Rect area, SDFTexPainter texPainter)
        {
            m_texPainter = texPainter;
            m_area = area;
            m_size = texPainter.size;
            m_texSize = texPainter.size * texPainter.texScale / 100;
        }

        public virtual void DrawPath() { }

        public virtual void Edit() { }

        /// <summary>
        /// This function is called as a layout event 
        /// after the current event has finished. Also, 
        /// the command name was ignored.
        /// </summary>
        public void Repaint()
        {
            var view = GUIView.current;

            var @event = EditorGUIUtility.CommandEvent("Repaint");
            @event.type = EventType.Used;

            GUIView.SendEvent(view, @event);
        }

        protected void GetNormal(Vector2 point0, Vector2 point1, Vector2 point2,
            out Vector2 tangent, out float scale)
        {
            var dir0 = (point1 - point0).normalized;
            var dir1 = (point2 - point1).normalized;

            tangent = ((dir0 + dir1) * 0.5f).normalized;

            tangent = new Vector2(-tangent.y, tangent.x);

            var deg = Vector2.Angle(tangent, -dir0);
            var sin = Mathf.Sin(deg * Mathf.Deg2Rad);
            scale = 1 / sin;
        }

        protected void GetNormal(Vector2 point0, Vector2 point1,
            out Vector2 tangent)
        {
            tangent = (point1 - point0).normalized;

            tangent = new Vector2(-tangent.y, tangent.x);
        }

        protected bool GetPathExpanded(out Vector2[] result, Vector2[] points, float offset, bool isClosed = false)
        {
            if (points.Length < 2)
            {
                result = new Vector2[0];
                return false;
            }

            result = new Vector2[points.Length];

            Vector2 tangent;
            float scale;

            if (isClosed)
            {
                GetNormal(points[points.Length - 1], points[0], points[1], out tangent, out scale);
                result[0] = points[0] + tangent * offset * scale;

                GetNormal(points[points.Length - 2], points[points.Length - 1], points[0], out tangent, out scale);
                result[result.Length - 1] = points[result.Length - 1] + tangent * offset * scale;
            }
            else
            {
                GetNormal(points[0], points[1], out tangent);
                result[0] = points[0] + tangent * offset;

                GetNormal(points[points.Length - 2], points[points.Length - 1], out tangent);
                result[result.Length - 1] = points[points.Length - 1] + tangent * offset;
            }

            if (points.Length > 2)
                for (int i = 2; i < points.Length; i++)
                {
                    GetNormal(points[i - 2], points[i - 1], points[i - 0], out tangent, out scale);
                    result[i - 1] = points[i - 1] + tangent * offset * scale;
                }

            return true;
        }

        protected Vector2[] GetPointsWithOffset(Vector2[] points, Vector2 offset)
        {
            var result = new Vector2[points.Length];
            for (int i = 0; i < points.Length; i++)
                result[i] = points[i] + offset;
            return result;
        }

        protected Vector3[] GetPointsWithOffset(Vector3[] points, Vector3 offset)
        {
            var result = new Vector3[points.Length];
            for (int i = 0; i < points.Length; i++)
                result[i] = points[i] + offset;
            return result;
        }

        protected Vector3[] Vector3ArrayFrom(Vector2[] points, Vector2 offset, bool isClosed)
        {
            var result = new Vector3[points.Length + (isClosed ? 1 : 0)];
            for (int i = 0; i < points.Length; i++)
                result[i] = points[i] + offset;
            if (isClosed)
                result[result.Length - 1] = result[0];
            return result;
        }

        protected Vector3[] Vector3ArrayFrom(Vector2[] points, bool isClosed) => Vector3ArrayFrom(points, Vector2.zero, isClosed);
    }
}
