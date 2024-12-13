/***
* This code is adapted from
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/ImageWithRoundedCorners.cs
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/Editor/ImageWithIndependentRoundedCornersInspector.cs
* 
* NOTE: SDFSpline is not supported in WebGL platform because SDFSpline uses StructuredBuffer and WebGL doesn't support it.
**/

using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TLab.UI.SDF
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(CanvasRenderer))]
    public class SDFSpline : SDFUI
    {
#if UNITY_EDITOR
        [MenuItem("GameObject/UI/SDFUI/SDFSpline", false)]
        private static void Create(MenuCommand menuCommand)
        {
            Create<SDFSpline>(menuCommand);
        }
#endif

        protected override string SHADER_NAME => "Hidden/UI/SDF/Spline/Outline";

        internal const string KEYWORD_SPLINE_FILL = SHADER_KEYWORD_PREFIX + "SPLINE_FILL";

        internal static readonly int PROP_SPLINES = Shader.PropertyToID("_Splines");
        internal static readonly int PROP_SPLINES_NUM = Shader.PropertyToID("_SplinesNum");
        internal static readonly int PROP_LINES = Shader.PropertyToID("_Lines");
        internal static readonly int PROP_LINES_NUM = Shader.PropertyToID("_LinesNum");
        internal static readonly int PROP_WIDTH = Shader.PropertyToID("_Width");

        [System.Serializable]
        public class QuadraticBezier
        {
            public Vector2[] controls;
        }

        [SerializeField, Range(0, 1)] private float m_width = 0.15f;
        [SerializeField] private bool m_fill = false;
        [SerializeField] private bool m_close = false;
        [SerializeField] private QuadraticBezier[] m_splines;

        private GraphicsBuffer m_bufferSpline;
        private GraphicsBuffer m_bufferLine;

        private const float EPSILON = 1e-2f;

        public float width
        {
            get => m_width;
            set
            {
                if (m_width != value)
                {
                    m_width = value;

                    SetAllDirty();
                }
            }
        }

        public bool close
        {
            get => m_close;
            set
            {
                if (m_close != value)
                {
                    m_close = value;

                    SetAllDirty();
                }
            }
        }

        public bool fill
        {
            get => m_fill;
            set
            {
                if (m_fill != value)
                {
                    m_fill = value;

                    SetAllDirty();
                }
            }
        }

        public QuadraticBezier[] splines
        {
            get => m_splines;
            set
            {
                if (m_splines != value)
                {
                    m_splines = value;

                    SetAllDirty();
                }
            }
        }

        public Vector2 this[int i, int j]
        {
            get => m_splines[i].controls[j];
            set
            {
                if (m_splines[i].controls[j] != value)
                {
                    m_splines[i].controls[j] = value;

                    SetAllDirty();
                }
            }
        }

        public QuadraticBezier this[int i]
        {
            get => m_splines[i];
            set
            {
                if (m_splines[i] != value)
                {
                    m_splines[i] = value;

                    SetAllDirty();
                }
            }
        }

        public unsafe Vector2[] GetControls(int i, bool isWorldSpace = false)
        {
            var controls = this[i].controls.Clone() as Vector2[];
            var minSize = this.minSize;
            if (isWorldSpace)
                fixed (Vector2* start = controls)
                    for (var current = start; current < (start + controls.Length); current++)
                        *current = transform.TransformPoint(new Vector2((*current).x, -(*current).y) * minSize);
            return controls;
        }

        public Vector2 GetControl(int i, int j, bool isWorldSpace = false)
        {
            var control = this[i, j];
            return isWorldSpace ? transform.TransformPoint(new Vector2(control.x, -control.y) * minSize) : control;
        }

        public unsafe void SetControls(int i, Vector2[] controls, bool isWorldSpace = false)
        {
            controls = controls.Clone() as Vector2[];
            var minSize = this.minSize;
            if (isWorldSpace)
                fixed (Vector2* start = controls)
                    for (var current = start; current < (start + controls.Length); current++)
                    {
                        *current = transform.InverseTransformPoint(*current) / minSize;
                        *current = new Vector2((*current).x, -(*current).y);
                    }
            this[i].controls = controls;
            SetAllDirty();
        }

        public void SetControl(int i, int j, Vector2 control, bool isWorldSpace = false)
        {
            if (isWorldSpace)
            {
                control = transform.InverseTransformPoint(control) / minSize;
                control = new Vector2(control.x, -control.y);
            }
            this[i, j] = control;
        }

        private void ReleaseBuffer(ref GraphicsBuffer buffer)
        {
            if (buffer != null)
            {
                buffer.Release();
                buffer.Dispose();
            }
            buffer = null;
        }

        private void AllocateBuffer(ref GraphicsBuffer buffer, int count, int stride)
        {
            if (buffer == null || buffer.count != count)
            {
                ReleaseBuffer(ref buffer);
                buffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, count, stride);
            }
        }

        private void AllocateZeroBuffer(ref GraphicsBuffer buffer, int stride)
        {
            if (buffer == null || buffer.count != 1)
            {
                ReleaseBuffer(ref buffer);
                buffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 1, stride);   // Because GraphicsBuffer doesn't allow count zero
            }
        }

        private void UpdateBuffer()
        {
            var minSize = this.minSize;
            var stride = Marshal.SizeOf(Vector2.zero);

            var splines = new Vector2[0] as IEnumerable<Vector2>;
            var lines = new Vector2[0] as IEnumerable<Vector2>;

            for (var i = 0; i < m_splines.Length; i++)
            {
                var controls = m_splines[i].controls.Select((v) => v * minSize);
                if (m_close)
                    controls = controls.Append(controls.ElementAt(0));

                if (controls.Count() > 1)
                {
                    var pass = 0;
                    for (var j = 0; j < (controls.Count() - 2); j += 2)
                    {
                        var v0 = controls.ElementAt(j + 0);
                        var v1 = controls.ElementAt(j + 1);
                        var v2 = controls.ElementAt(j + 2);

                        var abEqual = v0 == v1;
                        var bcEqual = v1 == v2;
                        var acEqual = v0 == v2;

                        if (abEqual && bcEqual)
                        {
                            // ignore
                        }
                        else if (abEqual || acEqual)
                        {
                            lines = lines.Append(v1);
                            lines = lines.Append(v2);
                        }
                        else if (bcEqual || (Mathf.Abs(MathUtils.Cross(v0 - v1, v1 - v2)) <= EPSILON))
                        {
                            lines = lines.Append(v0);
                            lines = lines.Append(v2);
                        }
                        else
                        {
                            splines = splines.Append(v0);
                            splines = splines.Append(v1);
                            splines = splines.Append(v2);
                        }

                        pass += 2;
                    }

                    if (pass <= (controls.Count() - 2))
                        lines = lines.Concat(controls.TakeLast(2));
                }
            }

            if (splines.Count() > 0)
            {
                AllocateBuffer(ref m_bufferSpline, splines.Count(), stride);
                m_bufferSpline.SetData(splines.ToArray());
            }
            else
                AllocateZeroBuffer(ref m_bufferSpline, stride);

            if (lines.Count() > 0)
            {
                AllocateBuffer(ref m_bufferLine, lines.Count(), stride);
                m_bufferLine.SetData(lines.ToArray());
            }
            else
                AllocateZeroBuffer(ref m_bufferLine, stride);

            _materialRecord.SetBuffer(PROP_SPLINES, m_bufferSpline);
            _materialRecord.SetBuffer(PROP_LINES, m_bufferLine);
            _materialRecord.SetInteger(PROP_SPLINES_NUM, splines.Count());
            _materialRecord.SetInteger(PROP_LINES_NUM, lines.Count());
        }

        protected override void OnDisable()
        {
            ReleaseBuffer(ref m_bufferSpline);
            ReleaseBuffer(ref m_bufferLine);
            base.OnDisable();
        }

        protected override void OnDestroy()
        {
            ReleaseBuffer(ref m_bufferSpline);
            ReleaseBuffer(ref m_bufferLine);
            base.OnDestroy();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void UpdateMaterialRecord()
        {
            base.UpdateMaterialRecord();

            var minSize = this.minSize;

            _materialRecord.SetFloat(PROP_WIDTH, m_width * minSize * 0.5f);

            UpdateBuffer();

            if (m_fill)
                _materialRecord.EnableKeyword(KEYWORD_SPLINE_FILL);
            else
                _materialRecord.DisableKeyword(KEYWORD_SPLINE_FILL);
        }
    }
}
