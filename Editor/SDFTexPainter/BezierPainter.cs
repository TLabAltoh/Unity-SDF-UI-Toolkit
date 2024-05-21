using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.Jobs;
using Unity.Collections;

namespace TLab.UI.SDF.Editor
{
    [System.Serializable]
    public class BezierPainter : ShapePainter
    {
        private class Grabber
        {
            private bool m_isGrab;

            private Bezier m_bezier;
            private Bezier.Handle m_handle;
            private Bezier.Control m_control = Bezier.Control.NONE;

            private Vector2 m_grabPos;
            private Vector2 m_grabDist;

            public bool isGrab => m_isGrab;

            public Bezier bezier => m_bezier;
            public Bezier.Handle handle => m_handle;
            public Bezier.Control control => m_control;

            public Vector2 grabPos => m_grabPos;
            public Vector2 dist => m_grabDist;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="pos"></param>
            public void Update(Vector2 pos)
            {
                switch (m_control)
                {
                    case Bezier.Control.A:
                        m_handle.controlA = (pos + m_grabDist) - handle.anchor;
                        break;
                    case Bezier.Control.B:
                        m_handle.controlB = (pos + m_grabDist) - handle.anchor;
                        break;
                    case Bezier.Control.NONE:
                        m_handle.anchor = pos + m_grabDist;
                        break;
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="grabPos"></param>
            /// <param name="bezier"></param>
            /// <param name="handle"></param>
            /// <param name="control"></param>
            public void Grab(Vector2 grabPos, Bezier bezier, Bezier.Handle handle, Bezier.Control control)
            {
                m_isGrab = true;

                m_bezier = bezier;
                m_handle = handle;
                m_control = control;

                m_grabPos = grabPos;

                switch (m_control)
                {
                    case Bezier.Control.A:
                        m_grabDist = (handle.anchor + handle.controlA) - grabPos;
                        break;
                    case Bezier.Control.B:
                        m_grabDist = (handle.anchor + handle.controlB) - grabPos;
                        break;
                    case Bezier.Control.NONE:
                        m_grabDist = handle.anchor - grabPos;
                        break;
                }
            }

            /// <summary>
            /// 
            /// </summary>
            public void Release()
            {
                m_isGrab = false;
                m_bezier = null;
                m_handle = null;
                m_control = Bezier.Control.NONE;
            }
        }

        public enum EditMode
        {
            ADD,
            MOVE
        }

        [Header("Brush Settings")]

        public float handleRadius = 10;
        public Color anchorColor = Color.green;
        public Color bezierColor = Color.green;
        public Color controlColor = Color.yellow;
        public Color selectColor = Color.magenta;
        public bool showHandle = false;
        public bool showSpline = false;
        public uint split = 0;

        [Header("Cu2Qu Settings")]

        public uint maxNum = 5;
        public float maxErr = 200f;

        private Grabber m_grabber = new Grabber();

        [Header("Shape Settings")]

        public EditMode editMode;
        public float thickness;
        public bool closed;
        public Draw draw;
        public List<Bezier> beziers = new List<Bezier>();

        private List<(Bezier, Bezier.Handle)> m_selects = new List<(Bezier, Bezier.Handle)>();

        public string THIS_NAME => "[" + GetType() + "] ";

        /// <summary>
        /// 
        /// </summary>
        public override void DrawPath()
        {
            base.DrawPath();

            var prevColor = Handles.color;

            var handles = new List<Vector3>();

            foreach (var bezier in beziers)
            {
                if (bezier.handles.Count == 0)
                {
                    continue;
                }

                var closed = bezier.closed;

                Vector2[] points;

                if (split > 0)
                {
                    bezier.GetPoints((int)split, closed, out points);
                }
                else
                {
                    bezier.GetPoints(out points);
                }

                points = EditorUtil.TextureToRect(points, m_area, m_texSize);

                Handles.color = bezierColor;

                EditorUtil.DrawBezier(Vector3ArrayFrom(points, false), closed);

                switch (bezier.draw)
                {
                    case Draw.STROKE:
                        Handles.color = Color.cyan;
                        break;
                    case Draw.FILL:
                        Handles.color = Color.white;
                        break;
                }

                Handles.DrawPolyLine(Vector3ArrayFrom(points, closed));

                handles.AddRange(Vector3ArrayFrom(points, false));
            }

            if (showHandle)
            {
                var handleRadius = EditorUtil.TextureToRect(this.handleRadius, m_area, m_texSize);

                var normal = Vector3.forward;

                Handles.color = anchorColor;

                for (int i = 1; i < handles.Count; i += 3)
                {
                    Handles.DrawSolidDisc(handles[i], normal, handleRadius);
                }

                Handles.color = controlColor;

                for (int i = 0; i < handles.Count; i += 3)
                {
                    EditorUtil.DrawCube(handles[i + 0], handleRadius, handleRadius);
                    EditorUtil.DrawCube(handles[i + 2], handleRadius, handleRadius);
                }

                Handles.color = selectColor;

                foreach (var select in m_selects)
                {
                    var bezier = select.Item1;

                    var anchor = select.Item2;

                    var position = EditorUtil.TextureToRect(anchor.anchor, m_area, m_texSize);

                    Handles.DrawWireDisc(position, normal, handleRadius + 1);
                }
            }

            if (showSpline)
            {
                for (int i = 0; i < beziers.Count; i++)
                {
                    if (beziers[i].handles.Count == 0)
                    {
                        continue;
                    }

                    var closed = beziers[i].closed;

                    Handles.color = bezierColor;

                    if (beziers[i].Cu2Qu(out var spline, (int)maxNum, maxErr))
                    {
                        spline = EditorUtil.TextureToRect(spline, m_area, m_texSize);

                        Handles.DrawPolyLine(Vector3ArrayFrom(spline, closed));

                        foreach (var point in spline)
                        {
                            Handles.DrawWireDisc(point, Vector3.forward, handleRadius + 1);
                        }
                    }
                }
            }

            Handles.color = prevColor;
        }

        /// <summary>
        /// 
        /// </summary>
        public void AddSegment()
        {
            if (Event.current.button == 0)
            {
                switch (Event.current.rawType)
                {
                    case EventType.MouseDown:
                        if (EditorUtil.CheckArea(Event.current.mousePosition, m_area))
                        {
                            if (beziers.Count == 0)
                            {
                                beziers.Add(new Bezier
                                {
                                    handles = new List<Bezier.Handle>(),
                                    closed = closed,
                                    thickness = thickness,

                                    draw = draw,
                                });
                            }

                            beziers[beziers.Count - 1].handles.Add(new Bezier.Handle
                            {
                                anchor = EditorUtil.RectToTexture(Event.current.mousePosition, m_area, m_texSize),
                                controlA = Vector2.zero,
                                controlB = Vector2.zero,
                            });
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="bezier"></param>
        /// <param name="handle"></param>
        /// <param name="control"></param>
        /// <returns></returns>
        private bool GetMostClosed(Vector2 position, out Bezier bezier, out Bezier.Handle handle, out Bezier.Control control)
        {
            var min = float.MaxValue;

            bezier = null;
            handle = null;
            control = Bezier.Control.NONE;

            for (int i = 0; i < beziers.Count; i++)
            {
                for (int j = 0; j < beziers[i].handles.Count; j++)
                {
                    var distA = Vector2.Distance(position, beziers[i].handles[j].controlA + beziers[i].handles[j].anchor);

                    if (distA < handleRadius)
                    {
                        if (distA < min)
                        {
                            min = distA;
                            bezier = beziers[i];
                            handle = beziers[i].handles[j];
                            control = Bezier.Control.A;
                        }
                    }

                    var distB = Vector2.Distance(position, beziers[i].handles[j].controlB + beziers[i].handles[j].anchor);

                    if (distB < handleRadius)
                    {
                        if (distB < min)
                        {
                            min = distB;
                            bezier = beziers[i];
                            handle = beziers[i].handles[j];
                            control = Bezier.Control.B;
                        }
                    }
                }
            }

            return (bezier != null) && (handle != null) && (control != Bezier.Control.NONE);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="bezier"></param>
        /// <param name="handle"></param>
        /// <returns></returns>
        private bool GetMostClosed(Vector2 position, out Bezier bezier, out Bezier.Handle handle)
        {
            var min = float.MaxValue;

            bezier = null;
            handle = null;

            for (int i = 0; i < beziers.Count; i++)
            {
                for (int j = 0; j < beziers[i].handles.Count; j++)
                {
                    var dist = Vector2.Distance(position, beziers[i].handles[j].anchor);

                    if (dist < handleRadius)
                    {
                        if (dist < min)
                        {
                            min = dist;
                            bezier = beziers[i];
                            handle = beziers[i].handles[j];
                        }
                    }
                }
            }

            return (bezier != null) && (handle != null);
        }

        /// <summary>
        /// 
        /// </summary>
        private void DeleteSegment()
        {
            foreach (var select in m_selects)
            {
                var bezier = select.Item1;

                var handle = select.Item2;

                bezier.handles.Remove(handle);
            }

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

                            if (Event.current.control)
                            {
                                if (GetMostClosed(grabPos, out var bezier, out var handle, out var control))
                                {
                                    m_grabber.Grab(grabPos, bezier, handle, control);
                                }
                            }
                            else
                            {
                                if (GetMostClosed(grabPos, out var bezier, out var handle))
                                {
                                    m_grabber.Grab(grabPos, bezier, handle, Bezier.Control.NONE);
                                }
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

                                if (GetMostClosed(input, out var bezier, out var handle))
                                {
                                    if (!m_selects.Contains((bezier, handle)))
                                    {
                                        m_selects.Add((bezier, handle));

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
            Debug.Log(THIS_NAME + "Start generate sdf from bezier shape");

            var halfSize = 0.5f * new Vector2(texSize.x, texSize.y);

            var beziersN = new NativeArray<BezierN>(beziers.Count, Allocator.TempJob);
            var splines = new List<Vector2[]>();
            var splinesNum = 0;
            for (int i = 0; i < beziersN.Length; i++)
            {
                if (beziers[i].Cu2Qu(out var spline, (int)maxNum, maxErr))
                {
                    splines.Add(GetOffseted(spline, halfSize));

                    splinesNum += spline.Length;
                }
            }

            var splinesN = new NativeArray<Vector2>(splinesNum, Allocator.TempJob);

            var splinesOffset = 0;

            if (splines.Count > 0)
            {
                for (int i = 0; i < splines.Count; i++)
                {
                    beziersN[i] = new BezierN
                    {
                        splineS = splinesOffset,
                        splineE = splinesOffset + splines[i].Length,
                        closed = beziers[i].closed,
                        thickness = beziers[i].thickness,

                        draw = beziers[i].draw,
                    };

                    for (int j = 0; j < splines[i].Length; j++)
                    {
                        splinesN[j] = splines[i][j];
                    }

                    splinesOffset += splines[i].Length;
                }
            }

            var rasterizeCircle = new SDFBezierJob
            {
                BEZIERS = beziersN,
                SPLINES = splinesN,
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

            splinesN.Dispose();
            beziersN.Dispose();

            Debug.Log(THIS_NAME + "Finish generate sdf from bezier shape");
        }
    }
}
