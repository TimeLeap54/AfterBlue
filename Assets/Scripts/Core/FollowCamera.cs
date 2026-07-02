using UnityEngine;

namespace AfterBlue.Core
{
    public sealed class FollowCamera : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new Vector3(0f, 10f, -9f);
        [SerializeField] private float followSmoothTime = 0.25f;
        [SerializeField] private float lookAtHeight = 0.5f;
        [SerializeField] private bool lookAtTarget = true;

        private Vector3 followVelocity;

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            Vector3 desiredPosition = target.position + offset;
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref followVelocity, followSmoothTime);

            if (lookAtTarget)
            {
                transform.LookAt(target.position + Vector3.up * lookAtHeight);
            }
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }
    }
}
