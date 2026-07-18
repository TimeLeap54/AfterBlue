using UnityEngine;

namespace AfterBlue.Core
{
    public sealed class FollowCamera : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new Vector3(0f, 10f, -9f);
        [SerializeField] private float followSmoothTime = 0.25f;
        [SerializeField] private float lookTargetSmoothTime = 0.16f;
        [SerializeField] private float rotationSharpness = 7f;
        [SerializeField] private float lookAtHeight = 0.5f;
        [SerializeField] private float lookAheadDistance;
        [SerializeField] private bool useTargetLocalOffset;
        [SerializeField] private bool lookAtTarget = true;

        private Vector3 followVelocity;
        private Vector3 lookTargetVelocity;
        private Vector3 smoothedLookTarget;
        private bool hasLookTarget;

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            Vector3 desiredPosition = useTargetLocalOffset ? target.TransformPoint(offset) : target.position + offset;
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref followVelocity, followSmoothTime);

            if (lookAtTarget)
            {
                Vector3 lookTarget = target.position + Vector3.up * lookAtHeight + target.forward * lookAheadDistance;
                if (!hasLookTarget)
                {
                    smoothedLookTarget = lookTarget;
                    hasLookTarget = true;
                }
                else
                {
                    smoothedLookTarget = Vector3.SmoothDamp(smoothedLookTarget, lookTarget, ref lookTargetVelocity, lookTargetSmoothTime);
                }

                Vector3 lookDirection = smoothedLookTarget - transform.position;
                if (lookDirection.sqrMagnitude > 0.001f)
                {
                    Quaternion desiredRotation = Quaternion.LookRotation(lookDirection.normalized, Vector3.up);
                    float rotationT = 1f - Mathf.Exp(-rotationSharpness * Time.deltaTime);
                    transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationT);
                }
            }
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }
    }
}
