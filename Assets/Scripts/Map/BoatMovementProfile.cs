using UnityEngine;

namespace AfterBlue.Map
{
    [CreateAssetMenu(menuName = "AfterBlue/Map/Boat Movement Profile")]
    public sealed class BoatMovementProfile : ScriptableObject
    {
        [SerializeField] private float maxForwardSpeed = 3f;
        [SerializeField] private float maxReverseSpeed = 1.6f;
        [SerializeField] private float acceleration = 5.2f;
        [SerializeField] private float deceleration = 7f;
        [SerializeField] private float turnSpeedDegrees = 95f;
        [SerializeField] private float stopTimeGoal = 2f;

        public float MaxForwardSpeed => maxForwardSpeed;
        public float MaxReverseSpeed => maxReverseSpeed;
        public float Acceleration => acceleration;
        public float Deceleration => deceleration;
        public float TurnSpeedDegrees => turnSpeedDegrees;
        public float StopTimeGoal => stopTimeGoal;
    }
}
