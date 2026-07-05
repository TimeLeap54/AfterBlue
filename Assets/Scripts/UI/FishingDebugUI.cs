using AfterBlue.Fishing;
using UnityEngine;

namespace AfterBlue.Fishing
{
    public sealed class FishingDebugUI : MonoBehaviour
    {
        [SerializeField] private FishingStateMachine stateMachine;

        public void SetStateMachine(FishingStateMachine nextStateMachine)
        {
            stateMachine = nextStateMachine;
        }

        private void OnGUI()
        {
            if (stateMachine == null)
            {
                return;
            }

            Rect rect = new Rect(12f, 42f, 440f, 126f);
            GUI.Box(rect, GUIContent.none);

            GUILayout.BeginArea(new Rect(rect.x + 10f, rect.y + 8f, rect.width - 20f, rect.height - 16f));
            GUILayout.Label($"Fishing State: {stateMachine.CurrentState}");

            if (stateMachine.CurrentState == FishingState.Waiting)
            {
                GUILayout.Label($"Bite In: {stateMachine.BiteTimer:0.0}s");
            }
            else if (stateMachine.CurrentState == FishingState.Bite)
            {
                GUILayout.Label($"Press Space! Window: {stateMachine.BiteWindowTimer:0.0}s");
                string rarity = stateMachine.PendingFish != null ? stateMachine.PendingFish.Rarity.ToString() : "Unknown";
                GUILayout.Label($"Tension: {rarity}");
            }
            else
            {
                GUILayout.Label("Controls: Space / Left Click");
            }

            FishCatchResult result = stateMachine.LastResult;
            if (result.HasResult && result.Success)
            {
                GUILayout.Label($"Caught: {result.FishName} / {result.Rarity} / {result.SizeCm:0.0} cm");
            }
            else if (result.HasResult)
            {
                GUILayout.Label("Result: Missed!");
            }
            else
            {
                GUILayout.Label("Result: None");
            }

            GUILayout.EndArea();
        }
    }
}
