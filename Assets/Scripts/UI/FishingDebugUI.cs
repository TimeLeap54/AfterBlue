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

            float height = stateMachine.CurrentState == FishingState.Fighting && stateMachine.ShowDebugFishIdentity ? 190f : 126f;
            if (stateMachine.CurrentState == FishingState.Fighting && !stateMachine.ShowDebugFishIdentity)
            {
                height = 168f;
            }
            Rect rect = new Rect(12f, 42f, 440f, height);
            GUI.Box(rect, GUIContent.none);

            GUILayout.BeginArea(new Rect(rect.x + 10f, rect.y + 8f, rect.width - 20f, rect.height - 16f));
            GUILayout.Label($"Fishing State: {stateMachine.CurrentState}");

            if (stateMachine.CurrentState == FishingState.Waiting)
            {
                GUILayout.Label($"Bite In: {stateMachine.BiteTimer:0.0}s");
            }
            else if (stateMachine.CurrentState == FishingState.Bite)
            {
                GUILayout.Label($"{stateMachine.BiteInstruction}   Window: {stateMachine.BiteWindowTimer:0.0}s");
                GUILayout.Label("Signal: Bite");
            }
            else if (stateMachine.CurrentState == FishingState.Fighting)
            {
                GUILayout.Label($"Signal: {stateMachine.FightSignal}   Action: {stateMachine.FightInstruction}");
                GUILayout.Label($"Distance: {stateMachine.FishDistance:0.0}  Catch <= {stateMachine.CatchDistance:0.0}  Escape >= {stateMachine.EscapeDistance:0.0}");
                GUILayout.Label($"Stress: {stateMachine.LineStress:0.00}  Durability: {stateMachine.LineDurability:0.00}  Break at 0");
                if (stateMachine.ShowDebugFishIdentity)
                {
                    string pattern = stateMachine.ActiveBehaviorPattern != null ? stateMachine.ActiveBehaviorPattern.DisplayName : "-";
                    string segment = stateMachine.CurrentBehaviorSegment != null ? stateMachine.CurrentBehaviorSegment.state.ToString() : "-";
                    GUILayout.Label($"Debug: {pattern} / {segment}");
                }
            }
            else if (stateMachine.CurrentState == FishingState.Landing)
            {
                GUILayout.Label($"Landing... Distance: {stateMachine.FishDistance:0.0}");
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
                GUILayout.Label($"Result: Missed! {stateMachine.MissReasonText}");
            }
            else
            {
                GUILayout.Label("Result: None");
            }

            GUILayout.EndArea();
        }
    }
}
