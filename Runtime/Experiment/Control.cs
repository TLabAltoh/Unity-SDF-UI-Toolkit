using UnityEngine;

namespace TLab.UI.SDF.Experiment
{
    public class Control : MonoBehaviour
    {
        // Game object as control for easy editing in Animator

        [SerializeField] private Color m_color = Color.green;

        private Vector3 m_prevPosition;

        public Vector3 position => transform.localPosition;

        public RectTransform rectTransform => (RectTransform)this.transform;

        public float radius => Mathf.Min(rectTransform.rect.size.x, rectTransform.rect.size.y) * 0.5f;

        public bool OnLateUpdate()
        {
            if (transform.localPosition != m_prevPosition)
            {
                m_prevPosition = transform.localPosition;

                return true;
            }

            return false;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = m_color;
            Gizmos.DrawSphere(transform.position, radius);
        }
#endif
    }
}