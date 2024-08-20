/***
* This codis adapteanmodifiefrom
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

        internal static readonly int PROP_CONTROLS = Shader.PropertyToID("_Controls");
        internal static readonly int PROP_WIDTH = Shader.PropertyToID("_Width");
        internal static readonly int PROP_NUM = Shader.PropertyToID("_Num");

        [SerializeField, Range(0, 1)] private float m_width = 0.15f;
        [SerializeField] private bool m_closed = false;
        [SerializeField] private Vector2[] m_controls;

        private GraphicsBuffer m_buffer;

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

        private void ReleaseBuffer()
        {
            if (m_buffer != null)
            {
                m_buffer.Release();
                m_buffer.Dispose();
            }
            m_buffer = null;
        }

        private void AllocateBuffer(int count, int stride)
        {
            if (m_buffer == null || m_buffer.count != count)
            {
                ReleaseBuffer();
                m_buffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, count, stride);
            }
        }

        private void UpdateBuffer()
        {
            var minSize = this.minSize;
            var stride = Marshal.SizeOf(Vector2.zero);
            if (m_controls.Length > 1)
            {
                var controls = m_controls.Select((v) => v * minSize);
                if (m_closed)
                {
                    if (controls.Count() % 2 == 0)
                        controls = controls.Append(controls.ElementAt(0));
                    else
                        controls = controls.Take(controls.Count() - 1).Append(controls.ElementAt(0));
                }
                AllocateBuffer(controls.Count(), stride);
                m_buffer.SetData(controls.ToArray());
            }
            else
            {
                AllocateBuffer(1, stride);  // Because GraphicsBuffer doesn't allow count zero
                m_buffer.SetData(new Vector2[1]);
            }
        }

        protected override void OnDisable()
        {
            ReleaseBuffer();
            base.OnDisable();
        }

        protected override void OnDestroy()
        {
            ReleaseBuffer();
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
            _materialRecord.SetBuffer(PROP_CONTROLS, m_buffer);
            _materialRecord.SetInteger(PROP_NUM, m_buffer.count);
        }
    }
}
