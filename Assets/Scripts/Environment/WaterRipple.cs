using UnityEngine;

namespace AfterBlue.Environment
{
    public sealed class WaterRipple : MonoBehaviour
    {
        private const int SegmentCount = 48;

        [SerializeField] private float duration = 0.85f;
        [SerializeField] private float startRadius = 0.15f;
        [SerializeField] private float endRadius = 0.85f;
        [SerializeField] private Color color = new Color(0.78f, 0.96f, 1f, 0.72f);
        [SerializeField] private float lineWidth = 0.024f;

        private LineRenderer lineRenderer;
        private Material runtimeMaterial;
        private float age;

        public static WaterRipple Spawn(Vector3 position, float startRadius = 0.15f, float endRadius = 0.85f, float duration = 0.85f)
        {
            GameObject rippleObject = new GameObject("Water Ripple");
            rippleObject.transform.position = position;

            WaterRipple ripple = rippleObject.AddComponent<WaterRipple>();
            ripple.startRadius = startRadius;
            ripple.endRadius = endRadius;
            ripple.duration = duration;
            return ripple;
        }

        private void Awake()
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.useWorldSpace = false;
            lineRenderer.loop = true;
            lineRenderer.positionCount = SegmentCount;
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;
            lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            lineRenderer.receiveShadows = false;

            Shader shader = Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Sprites/Default");
            if (shader == null)
            {
                return;
            }

            runtimeMaterial = new Material(shader)
            {
                color = color
            };
            lineRenderer.sharedMaterial = runtimeMaterial;
        }

        private void Update()
        {
            age += Time.deltaTime;
            float t = Mathf.Clamp01(age / duration);
            float radius = Mathf.Lerp(startRadius, endRadius, t);
            float alpha = Mathf.Lerp(color.a, 0f, t);

            if (runtimeMaterial != null)
            {
                runtimeMaterial.color = new Color(color.r, color.g, color.b, alpha);
            }
            lineRenderer.startWidth = Mathf.Lerp(lineWidth, lineWidth * 0.35f, t);
            lineRenderer.endWidth = lineRenderer.startWidth;

            for (int i = 0; i < SegmentCount; i++)
            {
                float angle = (i / (float)SegmentCount) * Mathf.PI * 2f;
                Vector3 point = new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
                lineRenderer.SetPosition(i, point);
            }

            if (age >= duration)
            {
                Destroy(gameObject);
            }
        }
    }
}
