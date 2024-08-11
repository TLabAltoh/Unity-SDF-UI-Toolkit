/***
* This codis adapteanmodifiefrom
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/ImageWithRoundedCorners.cs
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/Editor/ImageWithIndependentRoundedCornersInspector.cs
**/

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

        protected override string SHADER_NAME => "UI/SDF/Spline/Outline";

        [SerializeField, Range(0, 1)] private float m_width = 0.15f;

        [SerializeField] private Vector2[] m_controls;

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
    }
}
