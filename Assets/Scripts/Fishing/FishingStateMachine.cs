using AfterBlue.Data;
using AfterBlue.Journal;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AfterBlue.Fishing
{
    public enum FishingState
    {
        Idle,
        Casting,
        Waiting,
        Bite,
        Hooked,
        Fighting,
        Landing,
        Result
    }

    public sealed class FishingStateMachine : MonoBehaviour
    {
        [SerializeField] private FishingState currentState = FishingState.Idle;
        [SerializeField] private Transform playerBoat;
        [SerializeField] private Transform rodTip;
        [SerializeField] private FishingCastController castController;
        [SerializeField] private FishingLine fishingLine;
        [SerializeField] private FishingDebugUI debugUI;
        [SerializeField] private FishCollectionLog collectionLog;
        [SerializeField] private FishData[] availableFish;
        [SerializeField] private float minBiteDelay = 2f;
        [SerializeField] private float maxBiteDelay = 5f;
        [SerializeField] private KeyCode actionKey = KeyCode.Space;
        [SerializeField] private bool useBehavioralFight = true;
        [SerializeField] private S1FishBehaviorPattern[] behaviorPatterns;
        [SerializeField] private float holdReelRate = 0.95f;
        [SerializeField] private float releaseDriftRate = 0.28f;
        [SerializeField] private float stressGainRate = 0.34f;
        [SerializeField] private float stressRecoveryRate = 0.45f;
        [SerializeField] private float durabilityDamageRate = 0.68f;
        [SerializeField] private float stressWarningThreshold = 0.55f;
        [SerializeField] private float stressBreakThreshold = 0.72f;
        [SerializeField] private float wrongInputDistancePenalty = 0.42f;
        [SerializeField] private float landingDuration = 1.1f;
        [SerializeField] private bool showDebugFishIdentity;

        private float biteTimer;
        private float biteWindowTimer;
        private float fightTimer;
        private float landingTimer;
        private float segmentTimer;
        private float fishDistance;
        private float lineStress;
        private float lineDurability = 1f;
        private float fishStamina = 1f;
        private Vector3 fightStartPosition;
        private Vector3 fightDirection = Vector3.forward;
        private FishData pendingFish;
        private S1FishBehaviorPattern activePattern;
        private int segmentIndex;
        private bool biteStartedWhileHeld;
        private bool biteReleasedAfterStart;
        private string missReason = "Timeout";
        private FishCatchResult lastResult = FishCatchResult.None;
        private int castId;

        public FishingState CurrentState => currentState;
        public float BiteTimer => biteTimer;
        public float BiteWindowTimer => biteWindowTimer;
        public FishCatchResult LastResult => lastResult;
        public FishData PendingFish => pendingFish;
        public FishData[] AvailableFish => availableFish;
        public S1FishBehaviorPattern ActiveBehaviorPattern => activePattern;
        public FishBehaviorSegment CurrentBehaviorSegment => GetCurrentBehaviorSegment();
        public float FightTimer => fightTimer;
        public float FishDistance => fishDistance;
        public float LineStress => lineStress;
        public float LineDurability => lineDurability;
        public float FishStamina => fishStamina;
        public string MissReason => missReason;
        public bool ShowDebugFishIdentity => showDebugFishIdentity;
        public string FightSignal => GetFightSignal();
        public string FightInstruction => GetFightInstruction();
        public string MissReasonText => GetMissReasonText();
        public float CatchDistance => activePattern != null ? activePattern.LandingDistance : 0f;
        public float EscapeDistance => activePattern != null ? activePattern.EscapeDistance : 0f;
        public bool BiteRequiresRelease => currentState == FishingState.Bite && biteStartedWhileHeld && !biteReleasedAfterStart;
        public string BiteInstruction => BiteRequiresRelease ? "Release first, then press" : "Press to hook";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void BootstrapFishingSystem()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            if (!activeScene.isLoaded || activeScene.name != "FishingScene")
            {
                return;
            }

            GameObject gameManager = GameObject.Find("GameManager");
            if (gameManager == null)
            {
                gameManager = new GameObject("GameManager");
            }

            if (gameManager.GetComponent<FishingStateMachine>() == null)
            {
                gameManager.AddComponent<FishingStateMachine>();
            }
        }

        private void Awake()
        {
            ResolveDependencies();
        }

        private void Update()
        {
            if (WasActionPressed())
            {
                HandleActionPressed();
            }

            if (currentState == FishingState.Waiting)
            {
                biteTimer -= Time.deltaTime;
                if (biteTimer <= 0f)
                {
                    BeginBite();
                }
            }
            else if (currentState == FishingState.Bite)
            {
                if (biteStartedWhileHeld && !biteReleasedAfterStart && !IsActionHeld())
                {
                    biteReleasedAfterStart = true;
                }

                biteWindowTimer -= Time.deltaTime;
                if (biteWindowTimer <= 0f)
                {
                    FailCatch(BiteRequiresRelease ? "HeldTooEarly" : "Timeout");
                }
            }
            else if (currentState == FishingState.Fighting)
            {
                TickFight(IsActionHeld());
            }
            else if (currentState == FishingState.Landing)
            {
                TickLanding();
            }
        }

        public void SetState(FishingState nextState)
        {
            if (currentState == nextState)
            {
                return;
            }

            string prefix = castId > 0 ? $"[Cast {castId:000}] " : string.Empty;
            Debug.Log($"{prefix}{currentState} -> {nextState}");
            currentState = nextState;
        }

        private void ResolveDependencies()
        {
            if (playerBoat == null)
            {
                GameObject boatObject = GameObject.Find("PlayerBoat");
                if (boatObject != null)
                {
                    playerBoat = boatObject.transform;
                }
            }

            if (castController == null)
            {
                castController = GetComponent<FishingCastController>();
                if (castController == null)
                {
                    castController = gameObject.AddComponent<FishingCastController>();
                }
            }

            if (fishingLine == null)
            {
                fishingLine = GetComponent<FishingLine>();
                if (fishingLine == null)
                {
                    fishingLine = gameObject.AddComponent<FishingLine>();
                }
            }

            if (debugUI == null)
            {
                debugUI = GetComponent<FishingDebugUI>();
                if (debugUI == null)
                {
                    debugUI = gameObject.AddComponent<FishingDebugUI>();
                }
            }

            if (collectionLog == null)
            {
                collectionLog = GetComponent<FishCollectionLog>();
                if (collectionLog == null)
                {
                    collectionLog = gameObject.AddComponent<FishCollectionLog>();
                }
            }

            if ((availableFish == null || availableFish.Length == 0) && collectionLog.KnownFish.Length > 0)
            {
                availableFish = collectionLog.KnownFish;
            }

            if (playerBoat != null && rodTip == null)
            {
                rodTip = castController.EnsureRodTip(playerBoat);
            }

            fishingLine.SetTargets(rodTip, null);
            fishingLine.ResetAppearance();
            debugUI.SetStateMachine(this);
        }

        private void HandleActionPressed()
        {
            switch (currentState)
            {
                case FishingState.Idle:
                    BeginCast();
                    break;
                case FishingState.Bite:
                    if (BiteRequiresRelease)
                    {
                        break;
                    }

                    HookFish();
                    break;
                case FishingState.Result:
                    ResetLoop();
                    break;
            }
        }

        private void BeginCast()
        {
            ResolveDependencies();

            if (playerBoat == null || rodTip == null)
            {
                Debug.LogWarning("Fishing loop needs PlayerBoat and RodTip before casting.");
                return;
            }

            if (availableFish == null || availableFish.Length == 0)
            {
                Debug.LogWarning("Fishing loop needs FishData entries before casting. Run AfterBlue > Setup > Apply Week 4 Collection Data.");
                return;
            }

            lastResult = FishCatchResult.None;
            pendingFish = default;
            activePattern = null;
            biteTimer = 0f;
            biteWindowTimer = 0f;
            fightTimer = 0f;
            landingTimer = 0f;
            segmentTimer = 0f;
            fishDistance = 0f;
            lineStress = 0f;
            lineDurability = 1f;
            fishStamina = 1f;
            segmentIndex = 0;
            biteStartedWhileHeld = false;
            biteReleasedAfterStart = false;
            missReason = string.Empty;
            castId++;
            Debug.Log($"[Cast {castId:000}] Cast Started");
            SetState(FishingState.Casting);

            castController.Cast(playerBoat, rodTip, OnBobberLanded);
            fishingLine.SetTargets(rodTip, castController.ActiveBobberTransform);
            fishingLine.ResetAppearance();
            Debug.Log($"[Cast {castId:000}] Bobber Created");
        }

        private void OnBobberLanded(Bobber bobber)
        {
            if (currentState != FishingState.Casting)
            {
                return;
            }

            bobber.SetWaiting();
            biteTimer = Random.Range(minBiteDelay, maxBiteDelay);
            SetState(FishingState.Waiting);
        }

        private void BeginBite()
        {
            pendingFish = FishRollTable.Roll(availableFish);
            if (pendingFish == null)
            {
                Debug.LogWarning("Fish roll failed because no valid FishData entries were available.");
                FailCatch("RollFailed");
                return;
            }

            biteWindowTimer = pendingFish.BiteWindow;
            biteStartedWhileHeld = IsActionHeld();
            biteReleasedAfterStart = !biteStartedWhileHeld;
            Debug.Log($"[Cast {castId:000}] Fish Selected: {pendingFish.FishId}");
            Debug.Log($"[Cast {castId:000}] Bite Window Started");
            castController.ActiveBobber?.SetBite();
            SetState(FishingState.Bite);
        }

        private void HookFish()
        {
            SetState(FishingState.Hooked);

            activePattern = SelectBehaviorPattern(pendingFish);
            if (useBehavioralFight && activePattern != null && activePattern.Segments.Length > 0)
            {
                BeginFight();
                return;
            }

            CompleteCatch();
        }

        private void BeginFight()
        {
            fightTimer = 0f;
            landingTimer = 0f;
            segmentTimer = 0f;
            segmentIndex = 0;
            fishDistance = activePattern.StartingDistance;
            CacheFightDirection();
            lineStress = 0.15f;
            lineDurability = 1f;
            fishStamina = 1f;
            Debug.Log($"[Cast {castId:000}] Behavioral Fight Started: {activePattern.DisplayName} / {activePattern.Archetype}");
            SetState(FishingState.Fighting);
        }

        private void TickFight(bool held)
        {
            FishBehaviorSegment segment = GetCurrentBehaviorSegment();
            if (segment == null)
            {
                BeginLanding();
                return;
            }

            fightTimer += Time.deltaTime;
            segmentTimer += Time.deltaTime;

            bool stressDanger = lineStress >= stressBreakThreshold;
            bool correct = IsCorrectInput(segment.expectedInput, held);
            bool safeRelease = stressDanger && !held;
            bool effectiveCorrect = correct || safeRelease;
            if (held)
            {
                float reelMultiplier = effectiveCorrect ? 1f : 0.25f;
                fishDistance -= holdReelRate * segment.staminaDamage * reelMultiplier * Time.deltaTime;
                fishStamina = Mathf.Max(0f, fishStamina - (effectiveCorrect ? 0.055f : 0.015f) * segment.staminaDamage * Time.deltaTime);
                lineStress += stressGainRate * segment.pullStrength * (effectiveCorrect ? 0.85f : 1.9f) * Time.deltaTime;
            }
            else
            {
                fishDistance += releaseDriftRate * segment.pullStrength * (effectiveCorrect ? 0.75f : 1.15f) * Time.deltaTime;
                lineStress -= stressRecoveryRate * (safeRelease ? 2.1f : effectiveCorrect ? 1.4f : 0.75f) * Time.deltaTime;
            }

            if (!effectiveCorrect)
            {
                fishDistance += wrongInputDistancePenalty * segment.pullStrength * (held ? 1f : 0.35f) * Time.deltaTime;
                lineStress += segment.mistakeStress * (held ? 1.8f : 0.35f) * Time.deltaTime;
            }

            if (segment.state == FishBehaviorState.Pull || segment.state == FishBehaviorState.ObstacleRun)
            {
                fishDistance += segment.pullStrength * 0.38f * Time.deltaTime;
            }

            lineStress = Mathf.Clamp01(lineStress);
            fishDistance = Mathf.Clamp(fishDistance, 0f, activePattern.EscapeDistance + 2f);
            ApplyDurabilityDamage(effectiveCorrect, held);
            UpdateFightBobberPosition();
            UpdateFightVisuals(segment);

            if (lineDurability <= 0f)
            {
                FailCatch("LineBreak");
                return;
            }

            if (fishDistance >= activePattern.EscapeDistance)
            {
                FailCatch(segment.state == FishBehaviorState.ObstacleRun ? "ObstacleEscape" : "SlackEscape");
                return;
            }

            if (fishDistance <= activePattern.LandingDistance || fishStamina <= 0f)
            {
                BeginLanding();
                return;
            }

            if (segmentTimer >= segment.duration)
            {
                segmentTimer = 0f;
                segmentIndex = (segmentIndex + 1) % activePattern.Segments.Length;
            }
        }

        private void BeginLanding()
        {
            landingTimer = 0f;
            castController.ActiveBobber?.SetSuccess();
            SetState(FishingState.Landing);
        }

        private void TickLanding()
        {
            landingTimer += Time.deltaTime;
            fishDistance = Mathf.MoveTowards(fishDistance, 0.35f, Time.deltaTime * 2.8f);
            lineStress = Mathf.MoveTowards(lineStress, 0.08f, Time.deltaTime);
            UpdateFightBobberPosition();
            fishingLine.SetStress(lineStress);

            if (landingTimer >= landingDuration)
            {
                CompleteCatch();
            }
        }

        private void CompleteCatch()
        {
            float size = Random.Range(pendingFish.MinSize, pendingFish.MaxSize);
            lastResult = FishCatchResult.Caught(pendingFish, size);
            collectionLog.RegisterCatch(lastResult);
            Debug.Log($"[Cast {castId:000}] Reward Granted: {lastResult.FishId}");
            castController.ActiveBobber?.SetSuccess();
            SetState(FishingState.Result);
        }

        private void FailCatch(string reason)
        {
            missReason = reason;
            lastResult = FishCatchResult.Missed;
            castController.ActiveBobber?.SetFailed();
            Debug.Log($"[Cast {castId:000}] Miss Reason: {reason}");
            SetState(FishingState.Result);
        }

        private void ResetLoop()
        {
            castController.Retrieve();
            Debug.Log($"[Cast {castId:000}] Bobber Removed");
            fishingLine.SetTargets(rodTip, null);
            fishingLine.ResetAppearance();
            Debug.Log($"[Cast {castId:000}] Fishing Line Cleared");
            pendingFish = default;
            activePattern = null;
            biteTimer = 0f;
            biteWindowTimer = 0f;
            fightTimer = 0f;
            landingTimer = 0f;
            segmentTimer = 0f;
            fishDistance = 0f;
            lineStress = 0f;
            lineDurability = 1f;
            fishStamina = 1f;
            segmentIndex = 0;
            biteStartedWhileHeld = false;
            biteReleasedAfterStart = false;
            SetState(FishingState.Idle);
        }

        private bool WasActionPressed()
        {
            try
            {
                return Input.GetKeyDown(actionKey) || Input.GetMouseButtonDown(0);
            }
            catch (System.InvalidOperationException)
            {
                return false;
            }
        }

        private bool IsActionHeld()
        {
            try
            {
                return Input.GetKey(actionKey) || Input.GetMouseButton(0);
            }
            catch (System.InvalidOperationException)
            {
                return false;
            }
        }

        private S1FishBehaviorPattern SelectBehaviorPattern(FishData fishData)
        {
            if (behaviorPatterns == null || behaviorPatterns.Length == 0)
            {
                return null;
            }

            FishBehaviorArchetype preferredArchetype = FishBehaviorArchetype.Calm;
            if (fishData != null)
            {
                if (fishData.CatchDifficulty >= 1.8f)
                {
                    preferredArchetype = FishBehaviorArchetype.Heavy;
                }
                else if (fishData.CatchDifficulty >= 1.35f)
                {
                    preferredArchetype = FishBehaviorArchetype.Pulse;
                }
                else if (fishData.CatchDifficulty >= 1.1f)
                {
                    preferredArchetype = FishBehaviorArchetype.Dart;
                }
            }

            for (int i = 0; i < behaviorPatterns.Length; i++)
            {
                if (behaviorPatterns[i] != null && behaviorPatterns[i].Archetype == preferredArchetype)
                {
                    return behaviorPatterns[i];
                }
            }

            for (int i = 0; i < behaviorPatterns.Length; i++)
            {
                if (behaviorPatterns[i] != null)
                {
                    return behaviorPatterns[i];
                }
            }

            return null;
        }

        private void CacheFightDirection()
        {
            Bobber bobber = castController.ActiveBobber;
            fightStartPosition = bobber != null ? bobber.transform.position : transform.position;
            fightDirection = fightStartPosition - (rodTip != null ? rodTip.position : transform.position);
            fightDirection.y = 0f;
            if (fightDirection.sqrMagnitude < 0.001f)
            {
                fightDirection = playerBoat != null ? playerBoat.forward : Vector3.forward;
                fightDirection.y = 0f;
            }

            fightDirection.Normalize();
        }

        private void UpdateFightBobberPosition()
        {
            if (activePattern == null || castController.ActiveBobber == null)
            {
                return;
            }

            float pullOffset = (activePattern.StartingDistance - fishDistance) * 0.18f;
            Vector3 position = fightStartPosition - fightDirection * pullOffset;
            position.y = fightStartPosition.y;
            castController.ActiveBobber.MoveWaterPosition(position);
        }

        private void UpdateFightVisuals(FishBehaviorSegment segment)
        {
            fishingLine.SetStress(lineStress);
            castController.ActiveBobber?.SetFightCue(segment.state, lineStress);
        }

        private void ApplyDurabilityDamage(bool correct, bool held)
        {
            if (lineStress <= stressWarningThreshold)
            {
                return;
            }

            float warningRange = Mathf.Max(0.01f, 1f - stressWarningThreshold);
            float warningPressure = (lineStress - stressWarningThreshold) / warningRange;
            if (!correct && held)
            {
                lineDurability -= durabilityDamageRate * warningPressure * Time.deltaTime;
            }
            else if (!correct)
            {
                lineDurability -= durabilityDamageRate * 0.18f * warningPressure * Time.deltaTime;
            }

            if (held && lineStress > stressBreakThreshold)
            {
                float breakRange = Mathf.Max(0.01f, 1f - stressBreakThreshold);
                float breakPressure = (lineStress - stressBreakThreshold) / breakRange;
                lineDurability -= durabilityDamageRate * 0.35f * breakPressure * Time.deltaTime;
            }

            lineDurability = Mathf.Clamp01(lineDurability);
        }

        private FishBehaviorSegment GetCurrentBehaviorSegment()
        {
            if (activePattern == null || activePattern.Segments.Length == 0)
            {
                return null;
            }

            return activePattern.Segments[Mathf.Clamp(segmentIndex, 0, activePattern.Segments.Length - 1)];
        }

        private static bool IsCorrectInput(FishInputHint expectedInput, bool held)
        {
            return expectedInput == FishInputHint.Mixed
                || (expectedInput == FishInputHint.Hold && held)
                || (expectedInput == FishInputHint.Release && !held);
        }

        private string GetFightSignal()
        {
            FishBehaviorSegment segment = GetCurrentBehaviorSegment();
            if (segment == null)
            {
                return "Fish is close";
            }

            if (lineStress >= stressBreakThreshold)
            {
                return "Line under heavy stress";
            }

            switch (segment.state)
            {
                case FishBehaviorState.Approach:
                    return "Fish is coming in";
                case FishBehaviorState.Pull:
                    return "Strong run";
                case FishBehaviorState.Turn:
                    return "Weak turn";
                case FishBehaviorState.Struggle:
                    return "Erratic movement";
                case FishBehaviorState.ObstacleRun:
                    return "Running toward debris";
                default:
                    return "Fish is moving";
            }
        }

        private string GetFightInstruction()
        {
            FishBehaviorSegment segment = GetCurrentBehaviorSegment();
            if (segment == null)
            {
                return "Hold steady";
            }

            if (lineStress >= stressBreakThreshold)
            {
                return "Release";
            }

            switch (segment.expectedInput)
            {
                case FishInputHint.Hold:
                    return "Hold";
                case FishInputHint.Release:
                    return "Release";
                default:
                    return lineStress > 0.45f ? "Release" : "Hold";
            }
        }

        private string GetMissReasonText()
        {
            switch (missReason)
            {
                case "Timeout":
                    return "Missed the bite";
                case "HeldTooEarly":
                    return "Held before the bite";
                case "LineBreak":
                    return "Line broke";
                case "ObstacleEscape":
                    return "Fish reached debris";
                case "SlackEscape":
                    return "Fish got too far";
                case "RollFailed":
                    return "No fish data";
                default:
                    return string.IsNullOrWhiteSpace(missReason) ? "Missed" : missReason;
            }
        }
    }
}
