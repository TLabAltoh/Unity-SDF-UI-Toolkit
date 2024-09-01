/***
* This code is adapted from
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/ImageWithRoundedCorners.cs
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/Editor/ImageWithIndependentRoundedCornersInspector.cs
**/

using System.Runtime.InteropServices;
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
            Create<SDFTriangle>(menuCommand);
        }
#endif

        protected override string SHADER_NAME => "Hidden/UI/SDF/Spline/Outline";

        internal const string KEYWORD_SPLINE_FILL = SHADER_KEYWORD_PREFIX + "SPLINE_FILL";

        //internal const string KEYWORD_SPLINE_FONT_RENDERING = SHADER_KEYWORD_PREFIX + "SPLINE_FONT_RENDERING";

        internal static readonly int PROP_SPLINES = Shader.PropertyToID("_Splines");
        internal static readonly int PROP_SPLINES_NUM = Shader.PropertyToID("_SplinesNum");
        internal static readonly int PROP_LINES = Shader.PropertyToID("_Lines");
        internal static readonly int PROP_LINES_NUM = Shader.PropertyToID("_LinesNum");
        internal static readonly int PROP_WIDTH = Shader.PropertyToID("_Width");

        [SerializeField, Range(0, 1)] private float m_width = 0.15f;
        [SerializeField] private bool m_closed = false;
        [SerializeField] private bool m_fill = false;
        [SerializeField] private bool m_reverse = false;
        [SerializeField] private Vector2[] m_controls;

        private GraphicsBuffer m_bufferSpline;
        private GraphicsBuffer m_bufferLine;

        public enum RenderMode
        {
            DISTANCE,
            FONT
        }

        public int length
        {
            get
            {
                if (m_controls == null)
                    return 0;

                return m_controls.Length;
            }
        }

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

        public bool closed
        {
            get => m_closed;
            set
            {
                if (m_closed != value)
                {
                    m_closed = value;

                    SetAllDirty();
                }
            }
        }

        public bool reverse
        {
            get => m_reverse;
            set
            {
                if (m_reverse != value)
                {
                    m_reverse = value;

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

        public Vector2[] controls
        {
            get => m_controls;
            set
            {
                if (m_controls != value)
                {
                    m_controls = value;

                    SetAllDirty();
                }
            }
        }

        public Vector2 this[int i]
        {
            get => m_controls[i];
            set
            {
                if (m_controls[i] != value)
                {
                    m_controls[i] = value;

                    SetAllDirty();
                }
            }
        }

        public Vector2 GetControl(int index, bool isWorldSpace = false)
        {
            Vector2 corner = this[index];

            return isWorldSpace ? rectTransform.TransformPoint(new Vector2(corner.x, -corner.y) * minSize) : corner;
        }

        public void SetControl(int index, Vector2 corner, bool isWorldSpace = false)
        {
            if (isWorldSpace)
            {
                corner = rectTransform.InverseTransformPoint(corner) / minSize;
                corner = new Vector2(corner.x, -corner.y);
            }

            this[index] = corner;
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

        private void AllocateZeroBuffre(ref GraphicsBuffer buffer, int stride)
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

            var splines = new Vector2[0].Select((v) => v);
            var lines = new Vector2[0].Select((v) => v);

            if (m_controls.Length > 1)
            {
                var controls = m_controls.Select((v) => v * minSize);
                if (m_closed)
                    controls = controls.Append(controls.ElementAt(0));
                if (m_reverse)
                    controls = controls.Reverse();

                int pass = 0;
                for (int i = 0; i < (controls.Count() - 2); i += 2)
                {
                    var v0 = controls.ElementAt(i + 0);
                    var v1 = controls.ElementAt(i + 1);
                    var v2 = controls.ElementAt(i + 2);

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
                    else if (bcEqual)
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

                if (splines.Count() > 0)
                {
                    AllocateBuffer(ref m_bufferSpline, splines.Count(), stride);
                    m_bufferSpline.SetData(splines.ToArray());
                }
                else
                    AllocateZeroBuffre(ref m_bufferSpline, stride);

                if (lines.Count() > 0)
                {
                    AllocateBuffer(ref m_bufferLine, lines.Count(), stride);
                    m_bufferLine.SetData(lines.ToArray());
                }
                else
                    AllocateZeroBuffre(ref m_bufferLine, stride);
            }
            else
            {
                AllocateZeroBuffre(ref m_bufferSpline, stride);
                AllocateZeroBuffre(ref m_bufferLine, stride);
            }

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
