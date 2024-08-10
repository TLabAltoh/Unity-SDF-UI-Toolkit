using System.Linq;
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
        [SerializeField, Range(0, 1)] private float m_width = 0.15f;

        [SerializeField] private Control[] m_controls;

        private Mask m_mask;

        public Control[] activeControls
        {
            get
            {
                if (m_controls == null)
                {
                    return new Control[0];
                }

                return m_controls.Where(c => c != null).ToArray();
            }
        }

        public float3[] activeControlPoints => activeControls.Select((c, i) => new float3(c.position)).ToArray();

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

        protected virtual void LateUpdate()
        {
            var update = false;
            foreach (var control in activeControls)
                update |= control.OnLateUpdate();

            if (update)
                OnUpdateDimensions();
        }

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
            // Add the control point of the spline to the vertex texcoord (1 ~ 3).
            // This UI mesh is intended to cover only the entire surface of the
            // spline shape (currently, no consideration of performance).

            vh.Clear();

            var controlPoints = activeControlPoints;

            var color32 = color;
            var width = m_width * m_minSize;

            if (controlPoints.Length == 2)
            {
                var p0 = controlPoints[0];
                var p1 = controlPoints[controlPoints.Length - 1];

                SDFUtils.GetQuadVertexData(rectTransform.rect.size, rectTransform.pivot, p0, p1, width, out var vertex0, out var vertex1, out var vertex2, out var vertex3);

                vh.AddVert(new Vector3(vertex0.position.x, vertex0.position.y), color32, new Vector4(vertex0.uv.x, vertex0.uv.y));
                vh.AddVert(new Vector3(vertex1.position.x, vertex1.position.y), color32, new Vector4(vertex1.uv.x, vertex1.uv.y));
                vh.AddVert(new Vector3(vertex2.position.x, vertex2.position.y), color32, new Vector4(vertex2.uv.x, vertex2.uv.y));
                vh.AddVert(new Vector3(vertex3.position.x, vertex3.position.y), color32, new Vector4(vertex3.uv.x, vertex3.uv.y));

                vh.AddTriangle(0, 2, 1);
                vh.AddTriangle(0, 3, 2);

                return;
            }

            if (controlPoints.Length > 2)
            {
                int vertexCount = 0;

                SDFUtils.GetForwardAndLeft(new float3(controlPoints[0]), new float3(controlPoints[1]), out var forward, out var left);

                for (int i = 0; i < controlPoints.Length - 2; i++)
                {
                    var p0 = i == 0 ? controlPoints[0] : (controlPoints[i] + controlPoints[i + 1]) * 0.5f;
                    var p1 = controlPoints[i + 1];
                    var p2 = i + 2 == controlPoints.Length - 1 ? controlPoints[controlPoints.Length - 1] : (controlPoints[i + 1] + controlPoints[i + 2]) * 0.5f;

                    if (math.abs(1.0 - math.abs(math.dot(math.normalize(p0 - p1), math.normalize(p1 - p2)))) < 1e-4)
                    {
                        SDFUtils.GetQuadVertexData(rectTransform.rect.size, new float2(), p0, p2, width, out var vertex0, out var vertex1, out var vertex2, out var vertex3);

                        vh.AddVert(vertex0.position, color32, new Vector4(vertex0.uv.x, vertex0.uv.y));
                        vh.AddVert(vertex1.position, color32, new Vector4(vertex1.uv.x, vertex1.uv.y));
                        vh.AddVert(vertex2.position, color32, new Vector4(vertex2.uv.x, vertex2.uv.y));
                        vh.AddVert(vertex3.position, color32, new Vector4(vertex3.uv.x, vertex3.uv.y));

                        vh.AddTriangle(vertexCount + 0, vertexCount + 2, vertexCount + 1);
                        vh.AddTriangle(vertexCount + 0, vertexCount + 3, vertexCount + 2);

                        vertexCount += 4;
                    }
                    else
                    {
                        SDFUtils.GetQuadraticBezierVertexData(rectTransform.rect.size, new float2(), p0, p1, p2, width,
                            out var vertex0, out var vertex1, out var vertex2, out var vertex3, out var vertex4, out var vertex5, out var vertex6, out var vertex7);

                        vh.AddVert(vertex0.position, color32, new Vector4(vertex0.uv.x, vertex0.uv.y));
                        vh.AddVert(vertex1.position, color32, new Vector4(vertex1.uv.x, vertex1.uv.y));
                        vh.AddVert(vertex2.position, color32, new Vector4(vertex2.uv.x, vertex2.uv.y));
                        vh.AddVert(vertex3.position, color32, new Vector4(vertex3.uv.x, vertex3.uv.y));
                        vh.AddVert(vertex4.position, color32, new Vector4(vertex4.uv.x, vertex4.uv.y));
                        vh.AddVert(vertex5.position, color32, new Vector4(vertex5.uv.x, vertex5.uv.y));
                        vh.AddVert(vertex6.position, color32, new Vector4(vertex6.uv.x, vertex6.uv.y));
                        vh.AddVert(vertex7.position, color32, new Vector4(vertex7.uv.x, vertex7.uv.y));

                        vh.AddTriangle(vertexCount + 0, vertexCount + 1, vertexCount + 3);
                        vh.AddTriangle(vertexCount + 0, vertexCount + 3, vertexCount + 2);
                        vh.AddTriangle(vertexCount + 2, vertexCount + 3, vertexCount + 5);
                        vh.AddTriangle(vertexCount + 2, vertexCount + 5, vertexCount + 4);
                        vh.AddTriangle(vertexCount + 4, vertexCount + 5, vertexCount + 7);
                        vh.AddTriangle(vertexCount + 4, vertexCount + 7, vertexCount + 6);

                        vertexCount += 8;
                    }
                }
            }
        }
    }
}
