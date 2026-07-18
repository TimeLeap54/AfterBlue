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
        private Material runtimeMaterial;

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.positionCount = 2;
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;
            lineRenderer.useWorldSpace = true;
            lineRenderer.enabled = false;

            Shader shader = Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Sprites/Default");
            if (shader == null)
            {
                return;
            }

            runtimeMaterial = new Material(shader)
            {
                color = new Color(0.9f, 0.86f, 0.72f, 1f)
            };
            lineRenderer.sharedMaterial = runtimeMaterial;
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

        public void SetStress(float stress)
        {
            if (runtimeMaterial == null)
            {
                return;
            }

            Color calm = new Color(0.9f, 0.86f, 0.72f, 1f);
            Color warning = new Color(1f, 0.62f, 0.18f, 1f);
            Color danger = new Color(1f, 0.18f, 0.12f, 1f);
            runtimeMaterial.color = stress < 0.72f
                ? Color.Lerp(calm, warning, Mathf.Clamp01(stress / 0.72f))
                : Color.Lerp(warning, danger, Mathf.Clamp01((stress - 0.72f) / 0.28f));
        }

        public void ResetAppearance()
        {
            SetStress(0f);
        }
    }
}
