using System;
using UnityEngine;

namespace AfterBlue.Fishing
{
    public enum S1FishingMode
    {
        HookOnly,
        ContinuousTension,
        BehavioralTug
    }

    public enum FishBehaviorArchetype
    {
        Calm,
        Dart,
        Heavy,
        Pulse
    }

    public enum FishBehaviorState
    {
        Approach,
        Pull,
        Turn,
        Struggle,
        ObstacleRun,
        Landing
    }

    public enum FishInputHint
    {
        Hold,
        Release,
        Mixed
    }

    [Serializable]
    public sealed class FishBehaviorSegment
    {
        public FishBehaviorState state = FishBehaviorState.Approach;
        public FishInputHint expectedInput = FishInputHint.Hold;
        public float duration = 3f;
        public float pullStrength = 1f;
        public float staminaDamage = 1f;
        public float mistakeStress = 0.22f;
    }

    [CreateAssetMenu(menuName = "AfterBlue/Fishing/S1 Fish Behavior Pattern", fileName = "S1FishBehaviorPattern")]
    public sealed class S1FishBehaviorPattern : ScriptableObject
    {
        [SerializeField] private string patternId = "blue_minnow_calm";
        [SerializeField] private string displayName = "Blue Minnow";
        [SerializeField] private FishBehaviorArchetype archetype = FishBehaviorArchetype.Calm;
        [SerializeField] private float targetMinSeconds = 12f;
        [SerializeField] private float targetMaxSeconds = 16f;
        [SerializeField] private float startingDistance = 8f;
        [SerializeField] private float landingDistance = 1.1f;
        [SerializeField] private float escapeDistance = 14f;
        [SerializeField] private FishBehaviorSegment[] segments = Array.Empty<FishBehaviorSegment>();

        public string PatternId => patternId;
        public string DisplayName => displayName;
        public FishBehaviorArchetype Archetype => archetype;
        public float TargetMinSeconds => targetMinSeconds;
        public float TargetMaxSeconds => targetMaxSeconds;
        public float StartingDistance => startingDistance;
        public float LandingDistance => landingDistance;
        public float EscapeDistance => escapeDistance;
        public FishBehaviorSegment[] Segments => segments;
    }
}
