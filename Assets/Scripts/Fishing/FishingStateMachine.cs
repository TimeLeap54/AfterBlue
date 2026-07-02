using UnityEngine;

namespace AfterBlue.Fishing
{
    public enum FishingState
    {
        Idle,
        Casted,
        Waiting,
        Bite,
        Hooked,
        Result
    }

    public sealed class FishingStateMachine : MonoBehaviour
    {
        [SerializeField] private FishingState currentState = FishingState.Idle;

        public FishingState CurrentState => currentState;

        public void SetState(FishingState nextState)
        {
            if (currentState == nextState)
            {
                return;
            }

            Debug.Log($"Fishing state changed: {currentState} -> {nextState}");
            currentState = nextState;
        }
    }
}

