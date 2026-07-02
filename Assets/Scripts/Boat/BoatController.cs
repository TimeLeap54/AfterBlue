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

        private static Vector2 ReadMoveInput()
        {
            float horizontal = 0f;
            float vertical = 0f;

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                horizontal -= 1f;
            }

            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                horizontal += 1f;
            }

            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                vertical -= 1f;
            }

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                vertical += 1f;
            }

            if (Mathf.Approximately(horizontal, 0f))
            {
                horizontal = Input.GetAxisRaw("Horizontal");
            }

            if (Mathf.Approximately(vertical, 0f))
            {
                vertical = Input.GetAxisRaw("Vertical");
            }

            Vector2 input = new Vector2(horizontal, vertical);
            return input.sqrMagnitude > 1f ? input.normalized : input;
        }

        private void OnGUI()
        {
            if (!showDebugOverlay)
            {
                return;
            }

            GUI.Label(new Rect(12f, 12f, 520f, 24f), $"Boat Input: {currentInput}  Speed: {currentVelocity.magnitude:0.00}  Click Game view, then press WASD");
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
