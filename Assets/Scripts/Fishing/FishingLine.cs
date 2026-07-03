using UnityEngine;

namespace AfterBlue.Fishing
{
    [RequireComponent(typeof(LineRenderer))]
    public sealed class FishingLine : MonoBehaviour
    {
        [SerializeField] private Transform rodTip;
        [SerializeField] private Transform bobber;
        [SerializeField] private float lineWidth = 0.018f;

        private LineRenderer lineRenderer;

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.positionCount = 2;
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;
            lineRenderer.useWorldSpace = true;
            lineRenderer.enabled = false;

            Material material = new Material(Shader.Find("Universal Render Pipeline/Unlit"))
            {
                color = new Color(0.9f, 0.86f, 0.72f, 1f)
            };
            lineRenderer.sharedMaterial = material;
        }

        private void LateUpdate()
        {
            bool hasTargets = rodTip != null && bobber != null;
            lineRenderer.enabled = hasTargets;

            if (!hasTargets)
            {
                return;
            }

            lineRenderer.SetPosition(0, rodTip.position);
            lineRenderer.SetPosition(1, bobber.position);
        }

        public void SetTargets(Transform nextRodTip, Transform nextBobber)
        {
            rodTip = nextRodTip;
            bobber = nextBobber;
        }
    }
}
