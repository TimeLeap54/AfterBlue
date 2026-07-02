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

        private Vector3 currentVelocity;
        private Vector2 currentInput;

        public Vector3 Velocity => currentVelocity;

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

            GUI.Label(new Rect(12f, 12f, 360f, 24f), $"Boat Input: {currentInput}  Speed: {currentVelocity.magnitude:0.00}");
        }
    }
}
