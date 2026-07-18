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
            FightHold,
            FightRelease,
            FightDanger,
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
            Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            if (shader != null)
            {
                bobber.runtimeMaterial = new Material(shader)
                {
                    color = new Color(0.88f, 0.1f, 0.08f, 1f)
                };
                bobber.bobberRenderer.sharedMaterial = bobber.runtimeMaterial;
            }

            return bobber;
        }

        private void Awake()
        {
            if (bobberRenderer == null)
            {
                bobberRenderer = GetComponentInChildren<Renderer>();
            }

            if (runtimeMaterial == null && bobberRenderer != null && bobberRenderer.sharedMaterial != null)
            {
                runtimeMaterial = new Material(bobberRenderer.sharedMaterial);
                bobberRenderer.sharedMaterial = runtimeMaterial;
            }
        }

        private void Update()
        {
            float speed = visualState == BobberVisualState.Bite ? biteBobSpeed : waitingBobSpeed;
            float amplitude = visualState == BobberVisualState.Bite ? bobAmplitude * 2.2f : bobAmplitude;
            if (visualState == BobberVisualState.FightRelease)
            {
                speed = biteBobSpeed * 0.72f;
                amplitude = bobAmplitude * 1.8f;
            }
            else if (visualState == BobberVisualState.FightDanger)
            {
                speed = biteBobSpeed;
                amplitude = bobAmplitude * 2.4f;
            }

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

        public void MoveWaterPosition(Vector3 position)
        {
            basePosition = position;
            transform.position = position;
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

        public void SetFightCue(FishBehaviorState state, float stress)
        {
            if (stress >= 0.72f)
            {
                visualState = BobberVisualState.FightDanger;
                SetColor(new Color(1f, 0.18f, 0.12f, 1f));
                return;
            }

            if (state == FishBehaviorState.Pull || state == FishBehaviorState.ObstacleRun)
            {
                visualState = BobberVisualState.FightRelease;
                SetColor(new Color(1f, 0.52f, 0.12f, 1f));
            }
            else
            {
                visualState = BobberVisualState.FightHold;
                SetColor(new Color(0.25f, 0.78f, 0.92f, 1f));
            }
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
