using UnityEngine;

namespace AfterBlue.Environment
{
    public sealed class FishShadow : MonoBehaviour
    {
        [SerializeField] private Vector3 swimCenter;
        [SerializeField] private float swimRadius = 2.5f;
        [SerializeField] private float moveSpeed = 0.45f;
        [SerializeField] private float turnSpeed = 1.1f;
        [SerializeField] private float height = 0.055f;
        [SerializeField] private float phase;

        private Vector3 direction;

        private void Awake()
        {
            swimCenter = transform.position;
            phase = Random.Range(0f, 100f);
            direction = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f) * Vector3.forward;
        }

        private void Update()
        {
            float wave = Mathf.Sin(Time.time * turnSpeed + phase);
            direction = Quaternion.Euler(0f, wave * 55f * Time.deltaTime, 0f) * direction;

            Vector3 nextPosition = transform.position + direction.normalized * (moveSpeed * Time.deltaTime);
            Vector3 offset = nextPosition - swimCenter;
            offset.y = 0f;

            if (offset.magnitude > swimRadius)
            {
                direction = Vector3.RotateTowards(direction, -offset.normalized, turnSpeed * Time.deltaTime, 0f);
                nextPosition = transform.position + direction.normalized * (moveSpeed * Time.deltaTime);
            }

            nextPosition.y = height;
            transform.position = nextPosition;

            if (direction.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
            }
        }
    }
}
