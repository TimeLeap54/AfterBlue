using UnityEngine;

namespace AfterBlue.Environment
{
    public sealed class FloodedVillageScenery : MonoBehaviour
    {
        [SerializeField] private Color waterSurface = new Color(0.224f, 0.784f, 0.847f, 0.78f);
        [SerializeField] private Color fogColor = new Color(0.13f, 0.42f, 0.48f, 1f);
        [SerializeField] private float fogDensity = 0.018f;

        private void OnValidate()
        {
            ApplyRenderSettings();
        }

        private void Start()
        {
            ApplyRenderSettings();
        }

        private void ApplyRenderSettings()
        {
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogColor = fogColor;
            RenderSettings.fogDensity = fogDensity;
            RenderSettings.ambientLight = new Color(0.22f, 0.36f, 0.39f, 1f);
        }
    }
}
