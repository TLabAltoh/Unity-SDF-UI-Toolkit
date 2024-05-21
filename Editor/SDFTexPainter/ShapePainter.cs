using UnityEngine;
using UnityEditor;

namespace TLab.UI.SDF.Editor
{
    [System.Serializable]
    public class ShapePainter
    {
        protected Rect m_area;
        protected Vector2Int m_texSize;
        protected Vector2Int m_sdfTexSize;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="area"></param>
        /// <param name="texSize"></param>
        /// <param name="sdfTexSize"></param>
        public virtual void Update(Rect area, Vector2Int texSize, Vector2Int sdfTexSize)
        {
            m_area = area;
            m_texSize = texSize;
            m_sdfTexSize = sdfTexSize;
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void DrawPath()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Edit()
        {

        }

        /// <summary>
        /// This function is called as a layout event 
        /// after the current event has finished. Also, 
        /// the command name was ignored.
        /// </summary>
        public void Repaint()
        {
            var view = GUIView.Current;

            var @event = EditorGUIUtility.CommandEvent("Repaint");
            @event.type = EventType.Used;

            GUIView.SendEvent(view, @event);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point0"></param>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="tangent"></param>
        /// <param name="scale"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point0"></param>
        /// <param name="point1"></param>
        /// <param name="tangent"></param>
        protected void GetNormal(Vector2 point0, Vector2 point1,
            out Vector2 tangent)
        {
            tangent = (point1 - point0).normalized;

            tangent = new Vector2(-tangent.y, tangent.x);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="points"></param>
        /// <param name="offset"></param>
        /// <param name="closed"></param>
        /// <returns></returns>
        protected bool GetPathOffseted(out Vector2[] result, Vector2[] points, float offset, bool closed = false)
        {
            result = new Vector2[points.Length];

            if (points.Length < 2)
            {
                return false;
            }

            Vector2 tangent;
            float scale;

            if (closed)
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
            {
                for (int i = 2; i < points.Length; i++)
                {
                    GetNormal(points[i - 2], points[i - 1], points[i - 0], out tangent, out scale);
                    result[i - 1] = points[i - 1] + tangent * offset * scale;
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        protected Vector2[] GetOffseted(Vector2[] points, Vector2 offset)
        {
            var result = new Vector2[points.Length];

            for (int i = 0; i < points.Length; i++)
            {
                result[i] = points[i] + offset;
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        protected Vector3[] GetOffseted(Vector3[] points, Vector3 offset)
        {
            var result = new Vector3[points.Length];

            for (int i = 0; i < points.Length; i++)
            {
                result[i] = points[i] + offset;
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="offset"></param>
        /// <param name="closed"></param>
        /// <returns></returns>
        protected Vector3[] Vector3ArrayFrom(Vector2[] points, Vector2 offset, bool closed)
        {
            var result = new Vector3[points.Length + (closed ? 1 : 0)];

            for (int i = 0; i < points.Length; i++)
            {
                result[i] = points[i] + offset;
            }

            if (closed)
            {
                result[result.Length - 1] = result[0];
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="closed"></param>
        /// <returns></returns>
        protected Vector3[] Vector3ArrayFrom(Vector2[] points, bool closed)
        {
            return Vector3ArrayFrom(points, Vector2.zero, closed);
        }
    }
}
