using UnityEngine;

namespace AfterBlue.Boat
{
    public sealed class BoatController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private float rotationSpeed = 7f;
        [SerializeField] private float acceleration = 10f;
        [SerializeField] private float deceleration = 8f;
        [SerializeField] private bool showDebugOverlay = true;
        [SerializeField] private bool createPrototypeReferenceMarkers = true;

        private Vector3 currentVelocity;
        private Vector2 currentInput;
        private bool eventLeftHeld;
        private bool eventRightHeld;
        private bool eventDownHeld;
        private bool eventUpHeld;
        private string lastKeyEvent = "None";

        public Vector3 Velocity => currentVelocity;

        private void Start()
        {
            if (createPrototypeReferenceMarkers)
            {
                CreateReferenceMarkers();
            }
        }

        private void Update()
        {
            currentInput = ReadMoveInput();
            Vector3 desiredDirection = new Vector3(currentInput.x, 0f, currentInput.y);

            if (desiredDirection.sqrMagnitude > 1f)
            {
                desiredDirection.Normalize();
            }

            Vector3 targetVelocity = desiredDirection * moveSpeed;
            float rate = desiredDirection.sqrMagnitude > 0f ? acceleration : deceleration;
            currentVelocity = Vector3.MoveTowards(currentVelocity, targetVelocity, rate * Time.deltaTime);

            transform.position += currentVelocity * Time.deltaTime;

            if (currentVelocity.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(currentVelocity.normalized, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        private Vector2 ReadMoveInput()
        {
            float horizontal = 0f;
            float vertical = 0f;

            if (SafeGetKey(KeyCode.A) || SafeGetKey(KeyCode.LeftArrow))
            {
                horizontal -= 1f;
            }

            if (SafeGetKey(KeyCode.D) || SafeGetKey(KeyCode.RightArrow))
            {
                horizontal += 1f;
            }

            if (SafeGetKey(KeyCode.S) || SafeGetKey(KeyCode.DownArrow))
            {
                vertical -= 1f;
            }

            if (SafeGetKey(KeyCode.W) || SafeGetKey(KeyCode.UpArrow))
            {
                vertical += 1f;
            }

            if (eventLeftHeld)
            {
                horizontal -= 1f;
            }

            if (eventRightHeld)
            {
                horizontal += 1f;
            }

            if (eventDownHeld)
            {
                vertical -= 1f;
            }

            if (eventUpHeld)
            {
                vertical += 1f;
            }

            if (Mathf.Approximately(horizontal, 0f))
            {
                horizontal = SafeGetAxisRaw("Horizontal");
            }

            if (Mathf.Approximately(vertical, 0f))
            {
                vertical = SafeGetAxisRaw("Vertical");
            }

            Vector2 input = new Vector2(horizontal, vertical);
            return input.sqrMagnitude > 1f ? input.normalized : input;
        }

        private void OnGUI()
        {
            TrackGuiKeyEvents(Event.current);

            if (!showDebugOverlay)
            {
                return;
            }

            GUI.Label(new Rect(12f, 12f, 760f, 24f), $"Boat Input: {currentInput}  Speed: {currentVelocity.magnitude:0.00}  Last Key: {lastKeyEvent}  Click Game view, then press WASD");
        }

        private void TrackGuiKeyEvents(Event guiEvent)
        {
            if (guiEvent == null || !guiEvent.isKey)
            {
                return;
            }

            bool isPressed = guiEvent.type == EventType.KeyDown;
            bool isReleased = guiEvent.type == EventType.KeyUp;

            if (!isPressed && !isReleased)
            {
                return;
            }

            lastKeyEvent = $"{guiEvent.type} {guiEvent.keyCode}";

            switch (guiEvent.keyCode)
            {
                case KeyCode.A:
                case KeyCode.LeftArrow:
                    eventLeftHeld = isPressed;
                    break;
                case KeyCode.D:
                case KeyCode.RightArrow:
                    eventRightHeld = isPressed;
                    break;
                case KeyCode.S:
                case KeyCode.DownArrow:
                    eventDownHeld = isPressed;
                    break;
                case KeyCode.W:
                case KeyCode.UpArrow:
                    eventUpHeld = isPressed;
                    break;
            }

            if (isReleased)
            {
                switch (guiEvent.keyCode)
                {
                    case KeyCode.A:
                    case KeyCode.LeftArrow:
                        eventLeftHeld = false;
                        break;
                    case KeyCode.D:
                    case KeyCode.RightArrow:
                        eventRightHeld = false;
                        break;
                    case KeyCode.S:
                    case KeyCode.DownArrow:
                        eventDownHeld = false;
                        break;
                    case KeyCode.W:
                    case KeyCode.UpArrow:
                        eventUpHeld = false;
                        break;
                }
            }
        }

        private static bool SafeGetKey(KeyCode keyCode)
        {
            try
            {
                return Input.GetKey(keyCode);
            }
            catch (System.InvalidOperationException)
            {
                return false;
            }
        }

        private static float SafeGetAxisRaw(string axisName)
        {
            try
            {
                return Input.GetAxisRaw(axisName);
            }
            catch (System.InvalidOperationException)
            {
                return 0f;
            }
        }

        private static void CreateReferenceMarkers()
        {
            if (GameObject.Find("Prototype Reference Markers") != null)
            {
                return;
            }

            GameObject root = new GameObject("Prototype Reference Markers");
            Material markerMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"))
            {
                color = new Color(0.08f, 0.18f, 0.22f, 1f)
            };

            Material buoyMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"))
            {
                color = new Color(0.9f, 0.18f, 0.12f, 1f)
            };

            for (int x = -3; x <= 3; x++)
            {
                for (int z = -3; z <= 3; z++)
                {
                    if (x == 0 && z == 0)
                    {
                        continue;
                    }

                    Vector3 position = new Vector3(x * 4f, 0.08f, z * 4f);
                    GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    marker.name = $"Flooded Roof Marker {x},{z}";
                    marker.transform.SetParent(root.transform);
                    marker.transform.position = position;
                    marker.transform.localScale = new Vector3(1.2f, 0.15f, 1.2f);
                    marker.GetComponent<Renderer>().sharedMaterial = markerMaterial;
                }
            }

            for (int i = 0; i < 4; i++)
            {
                float angle = i * 90f * Mathf.Deg2Rad;
                Vector3 position = new Vector3(Mathf.Cos(angle) * 7f, 0.35f, Mathf.Sin(angle) * 7f);
                GameObject buoy = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                buoy.name = $"Prototype Buoy {i + 1}";
                buoy.transform.SetParent(root.transform);
                buoy.transform.position = position;
                buoy.transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);
                buoy.GetComponent<Renderer>().sharedMaterial = buoyMaterial;
            }
        }
    }
}
