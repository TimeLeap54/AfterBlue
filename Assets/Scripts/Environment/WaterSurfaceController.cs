using UnityEngine;

namespace AfterBlue.Environment
{
    [RequireComponent(typeof(Renderer))]
    public sealed class WaterSurfaceController : MonoBehaviour
    {
        [SerializeField] private Vector2 scrollSpeed = new Vector2(0.015f, 0.006f);
        [SerializeField] private float alphaPulse = 0.035f;
        [SerializeField] private float pulseSpeed = 0.35f;

        private Renderer surfaceRenderer;
        private Material runtimeMaterial;
        private Color baseColor;

        private void Awake()
        {
            surfaceRenderer = GetComponent<Renderer>();
            runtimeMaterial = surfaceRenderer.material;
            baseColor = runtimeMaterial.color;
        }

        private void Update()
        {
            Vector2 offset = runtimeMaterial.mainTextureOffset;
            offset += scrollSpeed * Time.deltaTime;
            runtimeMaterial.mainTextureOffset = offset;

            float pulse = Mathf.Sin(Time.time * pulseSpeed) * alphaPulse;
            Color color = baseColor;
            color.a = Mathf.Clamp01(baseColor.a + pulse);
            runtimeMaterial.color = color;
        }
    }
}
