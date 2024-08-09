using UnityEngine;
using UnityEngine.UI;
using Unity.Mathematics;

namespace TLab.UI.SDF.Experiment
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(CanvasRenderer))]
    public class SDFSpline : MaskableGraphic
    {
        [System.Serializable]
        public class SplineCorner
        {
            public Vector2 control = new Vector2(0.5f, 0.5f);
            [Range(0, 1)] public float radius = 0.1f;
        }

        [SerializeField] private Vector2 m_start = new Vector2(0.5f, 0);

        [SerializeField] private SplineCorner[] m_splineCorners;

        [SerializeField] private Vector2 m_end = new Vector2(0.5f, 1);

        [SerializeField, Range(0, 1)] private float m_width = 0.15f;

        private Mask m_mask;

        private float m_minSize => Mathf.Min(rectTransform.rect.size.x, rectTransform.rect.size.y);

        protected override void OnDidApplyAnimationProperties()
        {
            SetMaterialDirty();
            SetVerticesDirty();
            SetRaycastDirty();
        }

        /// <summary>
        /// This function must be called before calling the set material dirty function.
        /// </summary>
        protected virtual void Validate()
        {
            var canvasRenderer = GetComponent<CanvasRenderer>();
            canvasRenderer.cullTransparentMesh = false;

            m_mask = GetComponent<Mask>();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            Validate();

            base.OnValidate();
        }
#endif

        protected virtual void OnUpdateDimensions()
        {
            if (enabled)
            {
                SetVerticesDirty();
                SetMaterialDirty();
            }
        }

        public override void SetLayoutDirty()
        {
            base.SetLayoutDirty();

            OnUpdateDimensions();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();

            OnUpdateDimensions();
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            var color32 = color;

            var start = new float3(m_start * rectTransform.rect.size, 0);
            var end = new float3(m_end * rectTransform.rect.size, 0);
            var width = m_minSize * m_width;

            if ((m_splineCorners == null) || (m_splineCorners.Length == 0))
            {
                SDFUtils.GetQuadVertexData(rectTransform.rect.size, rectTransform.pivot, start, end, width, out var vertex0, out var vertex1, out var vertex2, out var vertex3);

                vh.AddVert(new Vector3(vertex0.position.x, vertex0.position.y), color32, new Vector4(vertex0.uv.x, vertex0.uv.y));
                vh.AddVert(new Vector3(vertex1.position.x, vertex1.position.y), color32, new Vector4(vertex1.uv.x, vertex1.uv.y));
                vh.AddVert(new Vector3(vertex2.position.x, vertex2.position.y), color32, new Vector4(vertex2.uv.x, vertex2.uv.y));
                vh.AddVert(new Vector3(vertex3.position.x, vertex3.position.y), color32, new Vector4(vertex3.uv.x, vertex3.uv.y));

                vh.AddTriangle(0, 2, 1);
                vh.AddTriangle(0, 3, 2);

                return;
            }

            if (m_splineCorners.Length > 0)
            {
                int vertexCount = 0, triangleCount = 0;

                float3 control, diff, dir;
                float radius, len, lim, margin = m_minSize * 0.001f;
            }
        }
    }
}
