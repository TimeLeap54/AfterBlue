using UnityEngine;

namespace AfterBlue.Boat
{
    public sealed class BoatController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float rotationSpeed = 8f;
        [SerializeField] private float acceleration = 14f;
        [SerializeField] private float deceleration = 10f;
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

            GUI.Label(new Rect(12f, 12f, 520f, 24f), $"Boat Input: {currentInput}  Speed: {currentVelocity.magnitude:0.00}  Last Key: {lastKeyEvent}");
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
            Material roofMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"))
            {
                color = new Color(0.08f, 0.18f, 0.22f, 1f)
            };

            Material roadMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"))
            {
                color = new Color(0.12f, 0.13f, 0.14f, 1f)
            };

            Material poleMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"))
            {
                color = new Color(0.18f, 0.16f, 0.13f, 1f)
            };

            Material buoyMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"))
            {
                color = new Color(0.9f, 0.18f, 0.12f, 1f)
            };

            Vector3[] roofPositions =
            {
                new Vector3(-9f, 0.08f, 6f),
                new Vector3(-4f, 0.08f, 10f),
                new Vector3(5f, 0.08f, 8f),
                new Vector3(10f, 0.08f, 2f),
                new Vector3(-7f, 0.08f, -5f),
                new Vector3(6f, 0.08f, -6f)
            };

            for (int i = 0; i < roofPositions.Length; i++)
            {
                GameObject roof = GameObject.CreatePrimitive(PrimitiveType.Cube);
                roof.name = $"Flooded Roof {i + 1}";
                roof.transform.SetParent(root.transform);
                roof.transform.position = roofPositions[i];
                roof.transform.rotation = Quaternion.Euler(0f, i * 17f, 0f);
                roof.transform.localScale = new Vector3(1.8f + i % 2, 0.16f, 1.4f);
                roof.GetComponent<Renderer>().sharedMaterial = roofMaterial;
            }

            for (int i = 0; i < 3; i++)
            {
                GameObject road = GameObject.CreatePrimitive(PrimitiveType.Cube);
                road.name = $"Submerged Road Segment {i + 1}";
                road.transform.SetParent(root.transform);
                road.transform.position = new Vector3(-6f + i * 6f, 0.03f, -10f + i * 1.5f);
                road.transform.rotation = Quaternion.Euler(0f, -10f, 0f);
                road.transform.localScale = new Vector3(4f, 0.06f, 1.4f);
                road.GetComponent<Renderer>().sharedMaterial = roadMaterial;
            }

            for (int i = 0; i < 2; i++)
            {
                Vector3 position = i == 0 ? new Vector3(-10f, 0.35f, -1f) : new Vector3(9f, 0.35f, 5f);
                GameObject buoy = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                buoy.name = $"Prototype Buoy {i + 1}";
                buoy.transform.SetParent(root.transform);
                buoy.transform.position = position;
                buoy.transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);
                buoy.GetComponent<Renderer>().sharedMaterial = buoyMaterial;
            }

            GameObject pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pole.name = "Leaning Utility Pole";
            pole.transform.SetParent(root.transform);
            pole.transform.position = new Vector3(8f, 0.9f, -3f);
            pole.transform.rotation = Quaternion.Euler(0f, 0f, -18f);
            pole.transform.localScale = new Vector3(0.12f, 1.8f, 0.12f);
            pole.GetComponent<Renderer>().sharedMaterial = poleMaterial;

            GameObject signPost = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            signPost.name = "Flooded Sign Post";
            signPost.transform.SetParent(root.transform);
            signPost.transform.position = new Vector3(-3f, 0.65f, -7f);
            signPost.transform.localScale = new Vector3(0.08f, 0.9f, 0.08f);
            signPost.GetComponent<Renderer>().sharedMaterial = poleMaterial;

            GameObject sign = GameObject.CreatePrimitive(PrimitiveType.Cube);
            sign.name = "Flooded Sign Plate";
            sign.transform.SetParent(signPost.transform);
            sign.transform.localPosition = new Vector3(0f, 0.55f, 0f);
            sign.transform.localScale = new Vector3(3f, 0.45f, 0.08f);
            sign.GetComponent<Renderer>().sharedMaterial = roofMaterial;
        }
    }
}
