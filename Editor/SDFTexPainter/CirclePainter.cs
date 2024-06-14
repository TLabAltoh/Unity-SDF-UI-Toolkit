using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Unity.Jobs;
using Unity.Collections;

namespace TLab.UI.SDF.Editor
{
    [System.Serializable]
    public class CirclePainter : ShapePainter
    {
        private class Grabber
        {
            private bool m_isGrab;

            private Circle m_circle;

            private Vector2 m_grabPos;
            private Vector2 m_grabDist;

            public bool isGrab => m_isGrab;

            public Circle circle => m_circle;

            public Vector2 grabPos => m_grabPos;
            public Vector2 dist => m_grabDist;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="pos"></param>
            public void Update(Vector2 pos)
            {
                m_circle.position = pos + m_grabDist;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="grabPos"></param>
            /// <param name="circle"></param>
            public void Grab(Vector2 grabPos, Circle circle)
            {
                m_isGrab = true;

                m_circle = circle;

                m_grabPos = grabPos;
                m_grabDist = circle.position - grabPos;
            }

            /// <summary>
            /// 
            /// </summary>
            public void Release()
            {
                m_isGrab = false;
                m_circle = null;
            }
        }

        public enum EditMode
        {
            ADD,
            MOVE,
            RADIUS,
        }

        [Header("Brush Settings")]

        public float handleRadius = 10;
        public Color handleColor = Color.green;
        public Color selectColor = Color.magenta;
        public bool showHandle = false;


        [Header("Shape Settings")]

        public EditMode editMode;
        public float radius;
        public float thickness;
        public Draw draw;
        public List<Circle> circles = new List<Circle>();

        private Grabber m_grabber = new Grabber();

        private List<Circle> m_selects = new List<Circle>();

        private string THIS_NAME => "[" + GetType() + "] ";

        /// <summary>
        /// 
        /// </summary>
        public override void DrawPath()
        {
            base.DrawPath();

            var prevColor = Handles.color;

            var handles = new List<Vector3>();

            foreach (var circle in circles)
            {
                var anchor = EditorUtil.TextureToRect(circle.position, m_area, m_texSize);
                var radius = EditorUtil.TextureToRect(circle.radius, m_area, m_texSize);
                var thickness = EditorUtil.TextureToRect(circle.thickness, m_area, m_texSize);

                var normal = Vector3.forward;

                switch (circle.draw)
                {
                    case Draw.STROKE:
                        Handles.color = Color.cyan;
                        Handles.DrawWireDisc(anchor, normal, radius - thickness);
                        Handles.DrawWireDisc(anchor, normal, radius + thickness);
                        break;
                    case Draw.FILL:
                        Handles.color = Color.white;
                        Handles.DrawWireDisc(anchor, normal, radius + thickness);
                        break;
                }

                handles.Add(anchor);
            }

            if (showHandle)
            {
                var handleRadius = EditorUtil.TextureToRect(this.handleRadius, m_area, m_texSize);

                var normal = Vector3.forward;

                Handles.color = handleColor;

                foreach (var handle in handles)
                {
                    Handles.DrawSolidDisc(handle, normal, handleRadius);
                }

                Handles.color = selectColor;

                foreach (var select in m_selects)
                {
                    var position = EditorUtil.TextureToRect(select.position, m_area, m_texSize);

                    Handles.DrawWireDisc(position, normal, handleRadius + 1);
                }
            }

            Handles.color = prevColor;
        }

        /// <summary>
        /// 
        /// </summary>
        private void AddSegment()
        {
            if (Event.current.button == 0)
            {
                switch (Event.current.rawType)
                {
                    case EventType.MouseDown:
                        if (EditorUtil.CheckArea(Event.current.mousePosition, m_area))
                        {
                            var circle = new Circle
                            {
                                position = EditorUtil.RectToTexture(Event.current.mousePosition, m_area, m_texSize),
                                radius = radius,
                                thickness = thickness,

                                draw = draw,
                            };

                            circles.Add(circle);
                        }
                        break;
                }
            }
        }

        private bool GetMostClosed(Vector2 position, out Circle select)
        {
            var min = float.MaxValue;

            select = null;

            foreach (var circle in circles)
            {
                var dist = Vector2.Distance(position, circle.position);

                if (dist < handleRadius)
                {
                    if (dist < min)
                    {
                        min = dist;
                        select = circle;
                    }
                }
            }

            return select != null;
        }

        /// <summary>
        /// 
        /// </summary>
        private void DeleteSegment()
        {
            circles = circles.Where((c) => !m_selects.Contains(c)).ToList();

            m_selects.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        private void MoveSegment()
        {
            if (Event.current.button == 0)
            {
                switch (Event.current.rawType)
                {
                    case EventType.MouseDown:
                        if (EditorUtil.CheckArea(Event.current.mousePosition, m_area))
                        {
                            var grabPos = EditorUtil.RectToTexture(Event.current.mousePosition, m_area, m_texSize);

                            m_grabber.Release();

                            if (GetMostClosed(grabPos, out var circle))
                            {
                                m_grabber.Grab(grabPos, circle);
                            }
                        }
                        break;
                    case EventType.MouseDrag:
                        if (m_grabber.isGrab)
                        {
                            m_grabber.Update(EditorUtil.RectToTexture(Event.current.mousePosition, m_area, m_texSize));
                        }
                        break;
                    case EventType.MouseUp:
                        m_grabber.Release();
                        break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void RadisuSegment()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        private void SelectSegment()
        {
            if (Event.current.button == 0)
            {
                switch (Event.current.rawType)
                {
                    case EventType.MouseDown:
                        if (Event.current.shift)
                        {
                            if (EditorUtil.CheckArea(Event.current.mousePosition, m_area))
                            {
                                var input = EditorUtil.RectToTexture(Event.current.mousePosition, m_area, m_texSize);

                                if (GetMostClosed(input, out var index))
                                {
                                    if (!m_selects.Contains(index))
                                    {
                                        m_selects.Add(index);

                                        Repaint();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (m_selects.Count > 0)
                            {
                                m_selects.Clear();

                                Repaint();
                            }
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Edit()
        {
            base.Edit();

            switch (editMode)
            {
                case EditMode.ADD:
                    AddSegment();
                    break;
                case EditMode.MOVE:
                    MoveSegment();
                    break;
                case EditMode.RADIUS:
                    RadisuSegment();
                    break;
            }

            SelectSegment();

            if (Event.current.rawType == EventType.KeyDown)
            {
                switch (Event.current.keyCode)
                {
                    case KeyCode.Delete:
                        DeleteSegment();
                        break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sdfBuffer"></param>
        /// <param name="sdfTexSize"></param>
        /// <param name="texSize"></param>
        /// <param name="settings"></param>
        public void GenerateSDF(in NativeArray<byte> sdfBuffer,
            Vector2Int sdfTexSize, Vector2Int texSize, SDFSettings settings)
        {
            Debug.Log(THIS_NAME + "Start generate sdf form circle shape");

            var halfSize = 0.5f * new Vector2(texSize.x, texSize.y);

            var circlesN = new NativeArray<CircleN>(circles.Count, Allocator.TempJob);
            for (int i = 0; i < circlesN.Length; i++)
            {
                circlesN[i] = new CircleN
                {
                    center = circles[i].position + halfSize,
                    radius = circles[i].radius,
                    thickness = circles[i].thickness,

                    draw = circles[i].draw,
                    clockwise = circles[i].clockwise,
                };
            }

            var rasterizeCircle = new SDFCircleJob
            {
                CIRCLES = circlesN,
                TEX_WIDTH = texSize.x,
                TEX_HEIGHT = texSize.y,
                SDF_WIDTH = sdfTexSize.x,
                SDF_HEIGHT = sdfTexSize.y,
                MAX_DIST = settings.maxDist,
                result = sdfBuffer
            };

            var handle = rasterizeCircle.Schedule(sdfBuffer.Length, 1);

            JobHandle.ScheduleBatchedJobs();

            handle.Complete();

            circlesN.Dispose();

            Debug.Log(THIS_NAME + "Finish generate sdf from circle shape");
        }
    }
}
