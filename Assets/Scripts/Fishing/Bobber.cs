using AfterBlue.Environment;
using UnityEngine;

namespace AfterBlue.Fishing
{
    public sealed class Bobber : MonoBehaviour
    {
        [SerializeField] private Renderer bobberRenderer;
        [SerializeField] private float bobAmplitude = 0.06f;
        [SerializeField] private float waitingBobSpeed = 2.4f;
        [SerializeField] private float biteBobSpeed = 13f;

        private Vector3 basePosition;
        private Material runtimeMaterial;
        private BobberVisualState visualState;

        private enum BobberVisualState
        {
            Waiting,
            Bite,
            Success,
            Failed
        }

        public static Bobber CreateDefault(string name)
        {
            GameObject root = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            root.name = name;
            root.transform.localScale = new Vector3(0.16f, 0.26f, 0.16f);

            Bobber bobber = root.AddComponent<Bobber>();
            bobber.bobberRenderer = root.GetComponent<Renderer>();
            bobber.runtimeMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"))
            {
                color = new Color(0.88f, 0.1f, 0.08f, 1f)
            };
            bobber.bobberRenderer.sharedMaterial = bobber.runtimeMaterial;

            return bobber;
        }

        private void Awake()
        {
            if (bobberRenderer == null)
            {
                bobberRenderer = GetComponentInChildren<Renderer>();
            }

            if (runtimeMaterial == null && bobberRenderer != null)
            {
                runtimeMaterial = new Material(bobberRenderer.sharedMaterial);
                bobberRenderer.sharedMaterial = runtimeMaterial;
            }
        }

        private void Update()
        {
            float speed = visualState == BobberVisualState.Bite ? biteBobSpeed : waitingBobSpeed;
            float amplitude = visualState == BobberVisualState.Bite ? bobAmplitude * 2.2f : bobAmplitude;
            Vector3 position = basePosition;
            position.y += Mathf.Sin(Time.time * speed) * amplitude;
            transform.position = position;
        }

        public void SetWaterPosition(Vector3 position)
        {
            basePosition = position;
            transform.position = position;
            WaterRipple.Spawn(position + Vector3.up * 0.012f, 0.12f, 0.9f, 0.9f);
        }

        public void SetWaiting()
        {
            visualState = BobberVisualState.Waiting;
            SetColor(new Color(0.88f, 0.1f, 0.08f, 1f));
        }

        public void SetBite()
        {
            visualState = BobberVisualState.Bite;
            SetColor(new Color(1f, 0.82f, 0.08f, 1f));
            WaterRipple.Spawn(basePosition + Vector3.up * 0.012f, 0.1f, 0.62f, 0.55f);
        }

        public void SetSuccess()
        {
            visualState = BobberVisualState.Success;
            SetColor(new Color(0.1f, 0.85f, 0.28f, 1f));
            WaterRipple.Spawn(basePosition + Vector3.up * 0.012f, 0.16f, 1.1f, 0.75f);
        }

        public void SetFailed()
        {
            visualState = BobberVisualState.Failed;
            SetColor(new Color(0.45f, 0.45f, 0.45f, 1f));
            basePosition += Vector3.down * 0.12f;
            WaterRipple.Spawn(basePosition + Vector3.up * 0.14f, 0.08f, 0.5f, 0.65f);
        }

        private void SetColor(Color color)
        {
            if (runtimeMaterial != null)
            {
                runtimeMaterial.color = color;
            }
        }
    }
}
