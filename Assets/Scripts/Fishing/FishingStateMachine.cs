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

        private float biteTimer;
        private float biteWindowTimer;
        private FishData pendingFish;
        private FishCatchResult lastResult = FishCatchResult.None;

        public FishingState CurrentState => currentState;
        public float BiteTimer => biteTimer;
        public float BiteWindowTimer => biteWindowTimer;
        public FishCatchResult LastResult => lastResult;
        public FishData PendingFish => pendingFish;
        public FishData[] AvailableFish => availableFish;

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
                biteWindowTimer -= Time.deltaTime;
                if (biteWindowTimer <= 0f)
                {
                    FailCatch();
                }
            }
        }

        public void SetState(FishingState nextState)
        {
            if (currentState == nextState)
            {
                return;
            }

            Debug.Log($"Fishing state changed: {currentState} -> {nextState}");
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
                    CompleteCatch();
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
            biteTimer = 0f;
            biteWindowTimer = 0f;
            SetState(FishingState.Casting);

            castController.Cast(playerBoat, rodTip, OnBobberLanded);
            fishingLine.SetTargets(rodTip, castController.ActiveBobberTransform);
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
                FailCatch();
                return;
            }

            biteWindowTimer = pendingFish.BiteWindow;
            castController.ActiveBobber?.SetBite();
            SetState(FishingState.Bite);
        }

        private void CompleteCatch()
        {
            SetState(FishingState.Hooked);
            float size = Random.Range(pendingFish.MinSize, pendingFish.MaxSize);
            lastResult = FishCatchResult.Caught(pendingFish, size);
            collectionLog.RegisterCatch(lastResult);
            castController.ActiveBobber?.SetSuccess();
            SetState(FishingState.Result);
        }

        private void FailCatch()
        {
            lastResult = FishCatchResult.Missed;
            castController.ActiveBobber?.SetFailed();
            SetState(FishingState.Result);
        }

        private void ResetLoop()
        {
            castController.Retrieve();
            fishingLine.SetTargets(rodTip, null);
            pendingFish = default;
            biteTimer = 0f;
            biteWindowTimer = 0f;
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
    }
}
