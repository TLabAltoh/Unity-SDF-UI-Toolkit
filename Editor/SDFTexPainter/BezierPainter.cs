#define SHOW_SPLINE
#undef SHOW_SPLINE

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Unity.Jobs;
using Unity.Collections;

namespace TLab.UI.SDF.Editor
{
    [System.Serializable]
    public class BezierPainter : ShapePainter
    {
        private class Selector
        {
            public enum EditMode
            {
                None,
                Position,
                Rotation,
                Scale,
            }

            private HashSet<(Bezier, Bezier.Handle)> m_selects = new HashSet<(Bezier, Bezier.Handle)>();

            private EditMode m_editMode = EditMode.None;

            private Vector2 m_initActualPos;
            private Vector2 m_initPos;
            private Vector2 m_initMousePos;
            private Vector2 m_initDir;

            private Rect m_area;
            private Vector2Int m_size;

            public Vector2 initMousePos => m_initMousePos;

            public Vector2 initPos => m_initPos;

            public EditMode editMode => m_editMode;

            public void BeginEdit(EditMode editMode, Rect area, Vector2Int size)
            {
                foreach (var element in m_selects)
                    element.Item2.MakeCache();

                m_editMode = editMode;
                m_area = area;
                m_size = size;
                m_initActualPos = m_selects.Average((v) => v.Item2.cache.anchor);
                m_initPos = EditorUtil.ActualPosToRect(m_initActualPos, area, size);
                m_initMousePos = Event.current.mousePosition;
                m_initDir = (m_initMousePos - m_initPos).normalized;
            }

            public void EditScale()
            {
                var @base = Mathf.Max(m_area.size.x, m_area.size.y);
                var diff = Event.current.mousePosition - m_initMousePos;
                var scale = (diff.magnitude * Vector2.Dot(m_initDir, diff.normalized) + @base) / @base;
                foreach (var element in m_selects)
                {
                    element.Item2.anchor = m_initActualPos + (element.Item2.cache.anchor - m_initActualPos) * scale;
                    element.Item2.controlA = element.Item2.cache.controlA * scale;
                    element.Item2.controlB = element.Item2.cache.controlB * scale;
                }
            }

            public void EditRotation()
            {
                var diff = Event.current.mousePosition - m_initPos;
                var angle = Vector2.SignedAngle(diff.normalized, m_initDir);
                foreach (var element in m_selects)
                {
                    MathUtils.RotateVector(element.Item2.cache.anchor - m_initActualPos, angle, out var rotated);
                    MathUtils.RotateVector(element.Item2.cache.controlA, angle, out var newControlA);
                    MathUtils.RotateVector(element.Item2.cache.controlB, angle, out var newControlB);
                    element.Item2.anchor = (Vector2)rotated + m_initActualPos;
                    element.Item2.controlA = newControlA;
                    element.Item2.controlB = newControlB;
                }
            }

            public void EditPosition()
            {
                var offset = EditorUtil.RectToActualVec(Event.current.mousePosition - m_initMousePos, m_area, m_size);
                foreach (var element in m_selects)
                    element.Item2.anchor = element.Item2.cache.anchor + offset;
            }

            public void EndEdit()
            {
                foreach (var element in m_selects)
                    element.Item2.MakeCache();
                m_editMode = EditMode.None;
            }

            public void CancleEdit()
            {
                foreach (var element in m_selects)
                    element.Item2.Revert();
                m_editMode = EditMode.None;
            }

            public void Add(Bezier bezier, Bezier.Handle handle) => m_selects.Add((bezier, handle));

            public void Add(Bezier bezier)
            {
                foreach (var handle in bezier.handles)
                    m_selects.Add((bezier, handle));
            }

            public int Count() => m_selects.Count();

            public void Clear() => m_selects.Clear();

            public IEnumerable<T> Select<T>(System.Func<(Bezier, Bezier.Handle), T> func) => m_selects.Select(func);

            public void Foreach(System.Action<(Bezier, Bezier.Handle)> func)
            {
                foreach (var element in m_selects)
                    func.Invoke(element);
            }

            public bool GetAvaragePosition(out Vector2 avarage)
            {
                if (m_selects.Count == 0)
                {
                    avarage = Vector2.zero;
                    return false;
                }
                avarage = m_selects.Average((v) => v.Item2.anchor);
                return true;
            }
        }

        private class Grabber
        {
            private bool m_isGrab;

            private Bezier m_bezier;
            private Bezier.Handle m_handle;
            private Bezier.Control m_control = Bezier.Control.None;

            private Vector2 m_grabPos;
            private Vector2 m_grabDist;

            public bool isGrab => m_isGrab;

            public Bezier bezier => m_bezier;
            public Bezier.Handle handle => m_handle;
            public Bezier.Control control => m_control;

            public Vector2 grabPos => m_grabPos;
            public Vector2 dist => m_grabDist;

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
                    case Bezier.Control.Anchor:
                        m_handle.anchor = pos + m_grabDist;
                        break;
                }
            }

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
                    case Bezier.Control.Anchor:
                        m_grabDist = handle.anchor - grabPos;
                        break;
                }
            }

            public void Release()
            {
                m_isGrab = false;
                m_bezier = null;
                m_handle = null;
                m_control = Bezier.Control.None;
            }
        }

        [System.Serializable]
        public class PrimitiveSettings
        {
            public Primitive.PrimitiveType primitiveType;
            [Min(3)] public int numPoints = 3;
            [Min(0)] public int scale = 25;
            [Min(0)] public bool fitCanvasX = true;

            public float GetSize(Rect area) => (fitCanvasX ? area.size.x : area.size.y) * scale / 100;
        }

        public enum EditMode
        {
            PRIMITIVE,
            ADD,
            MOVE,
            NONE,
        };

        [Header("Brush Settings")]
        [Min(0f)] public float handleRadius = 10;
        public Color anchorColor = Color.green;
        public Color bezierColor = Color.green;
        public Color controlColor = Color.yellow;
        public Color selectColor = Color.magenta;
        public bool showHandle = false;
        [Min(0f)] public int split = 0;

        [Header("Cu2Qu Settings")]
        [Min(0f)] public int maxNum = 25;
        [Min(0f)] public float maxErr = 25f;
        private Grabber m_grabber = new Grabber();

        [Header("Shape Settings")]
        public EditMode editMode;
        public PrimitiveSettings primitiveSettings;
        [Min(0f)] public float thickness;
        public bool isClosed;
        public Draw draw;
        public List<Bezier> beziers = new List<Bezier>();

        private Selector m_selector = new Selector();

        public string THIS_NAME => "[" + GetType() + "] ";

        public override void DrawPath()
        {
            base.DrawPath();

            var prevColor = Handles.color;

            var handles = new List<Vector3>();

            foreach (var bezier in beziers)
            {
                if (bezier.handles.Count == 0)
                    continue;

                var isClosed = bezier.isClosed;

                Vector2[] points;
                if (split > 0)
                    bezier.GetCubicAsArray(split, isClosed, out points);
                else
                    bezier.GetCubicAsArray(out points);

                points = EditorUtil.ActualPosToRect(points, m_area, m_size);

                Handles.color = bezierColor;

                EditorUtil.DrawBezier(Vector3ArrayFrom(points, false), isClosed);

                switch (bezier.draw)
                {
                    case Draw.STROKE:
                        Handles.color = Color.cyan;
                        break;
                    case Draw.FILL:
                        Handles.color = Color.white;
                        break;
                }
                handles.AddRange(Vector3ArrayFrom(points, false));
            }

            switch (m_selector.editMode)
            {
                case Selector.EditMode.Position:
                    Handles.color = Color.cyan;
                    Handles.DrawDottedLine(Event.current.mousePosition, m_selector.initMousePos, 2.0f);
                    break;
                case Selector.EditMode.Scale:
                case Selector.EditMode.Rotation:
                    Handles.color = Color.cyan;
                    Handles.DrawDottedLine(Event.current.mousePosition, m_selector.initPos, 2.0f);
                    break;
            }

            if (showHandle)
            {
                var handleRadius = this.handleRadius;
                var normal = Vector3.forward;

                Handles.color = Color.white;

                for (int i = 0; i < handles.Count; i += 3)
                {
                    Handles.DrawPolyLine(handles[i + 0], handles[i + 1]);
                    Handles.DrawPolyLine(handles[i + 2], handles[i + 1]);
                }

                Handles.color = anchorColor;

                for (int i = 1; i < handles.Count; i += 3)
                    Handles.DrawSolidDisc(handles[i], normal, handleRadius);

                Handles.color = controlColor;

                for (int i = 0; i < handles.Count; i += 3)
                {
                    EditorUtil.DrawCube(handles[i + 0], handleRadius, handleRadius);
                    EditorUtil.DrawCube(handles[i + 2], handleRadius, handleRadius);
                }

                Handles.color = selectColor;

                m_selector.Foreach((element) =>
                {
                    var bezier = element.Item1;
                    var anchor = element.Item2;
                    var position = EditorUtil.ActualPosToRect(anchor.anchor, m_area, m_size);
                    Handles.DrawWireDisc(position, normal, handleRadius + 1);
                });
            }

#if SHOW_SPLINE
            for (int i = 0; i < beziers.Count; i++)
            {
                if (beziers[i].handles.Count == 0)
                    continue;

                var isClosed = beziers[i].isClosed;

                Handles.color = bezierColor;

                if (beziers[i].Cu2Qu(out var spline, maxNum, maxErr))
                {
                    spline = EditorUtil.ActualPosToRect(spline, m_area, m_size);
                    Handles.DrawPolyLine(Vector3ArrayFrom(spline, isClosed));
                }
            }
#endif
            Handles.color = prevColor;
        }

        public void AddPrimitive()
        {
            if ((Event.current.button != 0) || (Event.current.rawType != EventType.MouseUp) || !EditorUtil.MouseIsInTheArea(m_area))
                return;
            Undo.RecordObject(m_texPainter, $"[{THIS_NAME}] Add Primitive");
            var handles = new List<Bezier.Handle>();
            var numPoints = primitiveSettings.numPoints;
            var size = primitiveSettings.GetSize(m_area);
            var offset = EditorUtil.RectToActualPos(Event.current.mousePosition, m_area, m_size);
            switch (primitiveSettings.primitiveType)
            {
                case Primitive.PrimitiveType.Circle:
                    handles.AddRange(Primitive.Circle(numPoints, size, offset));
                    break;
                case Primitive.PrimitiveType.Polygon:
                    handles.AddRange(Primitive.Box(numPoints, size, offset));
                    break;
            }
            beziers.Add(new Bezier
            {
                handles = handles,
                isClosed = isClosed,
                thickness = thickness,
                draw = draw,
            });
        }

        public void AddSegment()
        {
            if ((Event.current.button != 0) || (Event.current.rawType != EventType.MouseUp) || !EditorUtil.MouseIsInTheArea(m_area))
                return;
            Undo.RecordObject(m_texPainter, $"[{THIS_NAME}] Add Segment");
            if (beziers.Count == 0)
                beziers.Add(new Bezier
                {
                    handles = new List<Bezier.Handle>(),
                    isClosed = isClosed,
                    thickness = thickness,
                    draw = draw,
                });
            beziers[beziers.Count - 1].handles.Add(new Bezier.Handle
            {
                anchor = EditorUtil.RectToActualPos(Event.current.mousePosition, m_area, m_size),
                controlA = Vector2.zero,
                controlB = Vector2.zero,
            });
        }

        private bool GetControl(Vector2 position, out Bezier bezier, out Bezier.Handle handle, out Bezier.Control control)
        {
            var min = float.MaxValue;

            bezier = null;
            handle = null;
            control = Bezier.Control.None;

            for (int i = 0; i < beziers.Count; i++)
                for (int j = 0; j < beziers[i].handles.Count; j++)
                {
                    var distA = Vector2.Distance(position, beziers[i].handles[j].controlA + beziers[i].handles[j].anchor);
                    if ((distA < handleRadius) && (distA < min))
                    {
                        min = distA;
                        bezier = beziers[i];
                        handle = beziers[i].handles[j];
                        control = Bezier.Control.A;
                    }
                    var distB = Vector2.Distance(position, beziers[i].handles[j].controlB + beziers[i].handles[j].anchor);
                    if ((distB < handleRadius) && (distB < min))
                    {
                        min = distB;
                        bezier = beziers[i];
                        handle = beziers[i].handles[j];
                        control = Bezier.Control.B;
                    }
                }
            return (bezier != null) && (handle != null) && (control != Bezier.Control.None);
        }

        private bool GetAnchor(Vector2 position, out Bezier bezier, out Bezier.Handle handle)
        {
            var min = float.MaxValue;
            bezier = null;
            handle = null;

            for (int i = 0; i < beziers.Count; i++)
                for (int j = 0; j < beziers[i].handles.Count; j++)
                {
                    var dist = Vector2.Distance(position, beziers[i].handles[j].anchor);
                    if ((dist < handleRadius) && (dist < min))
                    {
                        min = dist;
                        bezier = beziers[i];
                        handle = beziers[i].handles[j];
                    }
                }
            return (bezier != null) && (handle != null);
        }

        private void EditSelectedSegment()
        {
            if ((m_selector.Count() > 0) && (Event.current.rawType == EventType.KeyUp) && EditorUtil.MouseIsInTheArea(m_area))
            {
                switch (Event.current.keyCode)
                {
                    case KeyCode.S:
                        if (m_selector.editMode != Selector.EditMode.Scale)
                        {
                            m_selector.BeginEdit(Selector.EditMode.Scale, m_area, m_size);
                            Repaint();
                        }
                        break;
                    case KeyCode.R:
                        if (m_selector.editMode != Selector.EditMode.Rotation)
                        {
                            m_selector.BeginEdit(Selector.EditMode.Rotation, m_area, m_size);
                            Repaint();
                        }
                        break;
                    case KeyCode.G:
                        if (m_selector.editMode != Selector.EditMode.Position)
                        {
                            m_selector.BeginEdit(Selector.EditMode.Position, m_area, m_size);
                            Repaint();
                        }
                        break;
                }
            }

            switch (m_selector.editMode)
            {
                case Selector.EditMode.Scale:
                    Undo.RecordObject(m_texPainter, $"[{THIS_NAME}] Edit scale of selected segments");
                    m_selector.EditScale();
                    Repaint();
                    break;
                case Selector.EditMode.Rotation:
                    Undo.RecordObject(m_texPainter, $"[{THIS_NAME}] Edit rotation of selected segments");
                    m_selector.EditRotation();
                    Repaint();
                    break;
                case Selector.EditMode.Position:
                    Undo.RecordObject(m_texPainter, $"[{THIS_NAME}] Edit position of selected segments");
                    m_selector.EditPosition();
                    Repaint();
                    break;
            }

            if ((m_selector.editMode != Selector.EditMode.None) && (Event.current.rawType == EventType.MouseUp) && (Event.current.button == 0))
                m_selector.EndEdit();
        }

        private void DeleteSegment()
        {
            if ((Event.current.keyCode == KeyCode.Delete) && (Event.current.rawType == EventType.KeyUp))
            {
                Undo.RecordObject(m_texPainter, $"[{THIS_NAME}] Delete Segment");
                m_selector.Foreach((element) =>
                {
                    var bezier = element.Item1;
                    var handle = element.Item2;
                    bezier.handles.Remove(handle);
                });
                m_selector.Clear();
            }
        }

        private void MoveSegment()
        {
            if (Event.current.button == 0)
                switch (Event.current.rawType)
                {
                    case EventType.MouseDown:
                        var grabPos = EditorUtil.RectToActualPos(Event.current.mousePosition, m_area, m_size);
                        m_grabber.Release();
                        if (Event.current.control && GetControl(grabPos, out var bezier, out var handle, out var control))
                            m_grabber.Grab(grabPos, bezier, handle, control);
                        else if (GetAnchor(grabPos, out bezier, out handle))
                            m_grabber.Grab(grabPos, bezier, handle, Bezier.Control.Anchor);
                        break;
                    case EventType.MouseDrag:
                        if (m_grabber.isGrab)
                        {
                            Undo.RecordObject(m_texPainter, $"[{THIS_NAME}] Move Segment");
                            m_grabber.Update(EditorUtil.RectToActualPos(Event.current.mousePosition, m_area, m_size));
                        }
                        break;
                    case EventType.MouseUp:
                        m_grabber.Release();
                        break;
                }
        }

        private void SelectSegment()
        {
            if (Event.current.rawType == EventType.MouseDown)
                switch (Event.current.button)
                {
                    case 0:
                        if (Event.current.shift)
                        {
                            var input = EditorUtil.RectToActualPos(Event.current.mousePosition, m_area, m_size);
                            if (GetAnchor(input, out var bezier, out var handle))
                            {
                                if (Event.current.control)
                                    m_selector.Add(bezier);
                                else
                                    m_selector.Add(bezier, handle);
                                Repaint();
                            }
                        }
                        break;
                    case 1:
                        if (m_selector.Count() > 0)
                        {
                            if (m_selector.editMode != Selector.EditMode.None)
                                m_selector.CancleEdit();
                            else
                                m_selector.Clear();
                            Repaint();
                        }
                        break;
                }
        }

        public override void Edit()
        {
            base.Edit();

            switch (editMode)
            {
                case EditMode.PRIMITIVE:
                    AddPrimitive();
                    break;
                case EditMode.ADD:
                    AddSegment();
                    break;
                case EditMode.MOVE:
                    MoveSegment();
                    break;
            }

            EditSelectedSegment();
            SelectSegment();    // SelectSegment is preferred to be called after calling Undo to discard previous changes.
            DeleteSegment();
        }

        public void GenSDFTexture(in NativeArray<byte> pixelBuffer, Vector2Int size, Vector2Int texSize, SDFSettings settings)
        {
            Debug.Log(THIS_NAME + "Start generate sdf from bezier shape");

            var halfSize = 0.5f * new Vector2(size.x, size.y);

            var beziers = this.beziers as IEnumerable<Bezier>;
            beziers = beziers.Where((v) => v.handles.Count > 1);    // Delete invalid bezier

            var beziersN = new NativeArray<BezierN>(beziers.Count(), Allocator.TempJob);
            var splines = new List<Vector2[]>();
            var splinesNum = 0;
            for (int i = 0; i < beziersN.Length; i++)
                if (beziers.ElementAt(i).Cu2Qu(out var spline, maxNum, maxErr))
                {
                    splines.Add(GetPointsWithOffset(spline, halfSize));
                    splinesNum += spline.Length;
                }

            var splinesN = new NativeArray<Vector2>(splinesNum, Allocator.TempJob);
            var splinesOffset = 0;
            if (splines.Count > 0)
                for (int i = 0; i < splines.Count; i++)
                {
                    beziersN[i] = new BezierN
                    {
                        splineS = splinesOffset,
                        splineE = splinesOffset + splines[i].Length,
                        isClosed = beziers.ElementAt(i).isClosed,
                        thickness = beziers.ElementAt(i).thickness,
                        draw = beziers.ElementAt(i).draw,
                    };

                    for (int j = 0; j < splines[i].Length; j++)
                        splinesN[splinesOffset + j] = splines[i][j];

                    splinesOffset += splines[i].Length;
                }

            var rasterizeCircle = new SDFBezierJob
            {
                beziers = beziersN,
                splines = splinesN,
                size = size,
                texSize = texSize,
                maxDist = settings.maxDist,
                result = pixelBuffer
            };

            var handle = rasterizeCircle.Schedule(pixelBuffer.Length, 1);

            JobHandle.ScheduleBatchedJobs();

            handle.Complete();

            splinesN.Dispose();
            beziersN.Dispose();

            Debug.Log(THIS_NAME + "Finish generate sdf from bezier shape");
        }
    }
}
