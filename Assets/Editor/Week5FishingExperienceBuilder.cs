using AfterBlue.Fishing;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AfterBlue.EditorTools
{
    public static class Week5FishingExperienceBuilder
    {
        private const string ScenePath = "Assets/Scenes/FishingScene.unity";
        private const string PatternFolder = "Assets/Data/Fish";

        [MenuItem("AfterBlue/Setup/Apply Week 5 Fishing Experience")]
        public static void ApplyWeekFiveFishingExperience()
        {
            EnsureAssetFolder(PatternFolder);

            S1FishBehaviorPattern[] patterns = CreatePatterns();

            Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

            RemoveWeekFivePrototypeObjects();
            ApplyPatternsToWeekFourLoop(patterns);
            RestoreWeekFourFishingInput();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            Debug.Log("Applied AfterBlue week 5 fishing experience to the existing Week 4 fishing loop.");
        }

        private static void ApplyPatternsToWeekFourLoop(S1FishBehaviorPattern[] patterns)
        {
            GameObject gameManager = GameObject.Find("GameManager");
            if (gameManager == null)
            {
                gameManager = new GameObject("GameManager");
            }

            FishingStateMachine stateMachine = gameManager.GetComponent<FishingStateMachine>();
            if (stateMachine == null)
            {
                stateMachine = gameManager.AddComponent<FishingStateMachine>();
            }

            stateMachine.enabled = true;

            SerializedObject stateMachineObject = new SerializedObject(stateMachine);
            SerializedProperty useBehavioralFight = stateMachineObject.FindProperty("useBehavioralFight");
            if (useBehavioralFight != null)
            {
                useBehavioralFight.boolValue = true;
            }

            SerializedProperty showDebugFishIdentity = stateMachineObject.FindProperty("showDebugFishIdentity");
            if (showDebugFishIdentity != null)
            {
                showDebugFishIdentity.boolValue = false;
            }

            SetFloatIfPresent(stateMachineObject, "durabilityDamageRate", 0.68f);
            SetFloatIfPresent(stateMachineObject, "stressWarningThreshold", 0.55f);
            SetFloatIfPresent(stateMachineObject, "stressBreakThreshold", 0.72f);
            SetFloatIfPresent(stateMachineObject, "wrongInputDistancePenalty", 0.42f);

            SerializedProperty behaviorPatterns = stateMachineObject.FindProperty("behaviorPatterns");
            if (behaviorPatterns != null)
            {
                SetObjectArray(behaviorPatterns, patterns);
            }
            else
            {
                Debug.LogWarning("Week 5 behavior pattern field was not found. Let Unity finish compiling, then apply Week 5 again.");
            }

            stateMachineObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(stateMachine);
        }

        private static void RemoveWeekFivePrototypeObjects()
        {
            DestroyIfExists("Week 5 Fishing Experience");
            DestroyIfExists("Week 5 Habitat Markers");
        }

        private static void DestroyIfExists(string objectName)
        {
            GameObject existing = GameObject.Find(objectName);
            if (existing != null)
            {
                Object.DestroyImmediate(existing);
            }
        }

        private static void RestoreWeekFourFishingInput()
        {
            GameObject gameManager = GameObject.Find("GameManager");
            if (gameManager == null)
            {
                return;
            }

            FishingStateMachine stateMachine = gameManager.GetComponent<FishingStateMachine>();
            if (stateMachine != null)
            {
                stateMachine.enabled = true;
            }

            FishingDebugUI debugUI = gameManager.GetComponent<FishingDebugUI>();
            if (debugUI != null)
            {
                debugUI.enabled = true;
            }
        }

        private static S1FishBehaviorPattern[] CreatePatterns()
        {
            return new[]
            {
                CreatePattern("s1_blue_minnow_calm", "Blue Minnow", FishBehaviorArchetype.Calm, 12f, 16f, 8f, new[]
                {
                    Segment(FishBehaviorState.Approach, FishInputHint.Hold, 3.0f, 0.7f),
                    Segment(FishBehaviorState.Turn, FishInputHint.Hold, 3.0f, 0.6f),
                    Segment(FishBehaviorState.Pull, FishInputHint.Release, 2.4f, 1.0f),
                    Segment(FishBehaviorState.Turn, FishInputHint.Hold, 2.6f, 0.55f),
                }),
                CreatePattern("s1_signal_fin_dart", "Signal Fin", FishBehaviorArchetype.Dart, 18f, 24f, 9f, new[]
                {
                    Segment(FishBehaviorState.Approach, FishInputHint.Hold, 3.2f, 0.8f),
                    Segment(FishBehaviorState.Turn, FishInputHint.Hold, 2.8f, 0.6f),
                    Segment(FishBehaviorState.Pull, FishInputHint.Release, 2.0f, 1.55f),
                    Segment(FishBehaviorState.Struggle, FishInputHint.Mixed, 3.0f, 1.15f),
                    Segment(FishBehaviorState.Turn, FishInputHint.Hold, 3.0f, 0.65f),
                }),
                CreatePattern("s1_glass_mackerel_pulse", "Glass Mackerel", FishBehaviorArchetype.Pulse, 25f, 35f, 10f, new[]
                {
                    Segment(FishBehaviorState.Pull, FishInputHint.Release, 3.2f, 1.15f),
                    Segment(FishBehaviorState.Turn, FishInputHint.Hold, 3.2f, 0.65f),
                    Segment(FishBehaviorState.Pull, FishInputHint.Release, 3.2f, 1.25f),
                    Segment(FishBehaviorState.Turn, FishInputHint.Hold, 3.2f, 0.65f),
                    Segment(FishBehaviorState.Struggle, FishInputHint.Mixed, 4.0f, 1.05f),
                }),
                CreatePattern("s1_concrete_carp_heavy", "Concrete Carp", FishBehaviorArchetype.Heavy, 45f, 60f, 12f, new[]
                {
                    Segment(FishBehaviorState.Pull, FishInputHint.Release, 5.0f, 1.35f),
                    Segment(FishBehaviorState.Turn, FishInputHint.Hold, 2.4f, 0.55f),
                    Segment(FishBehaviorState.Pull, FishInputHint.Release, 5.4f, 1.45f),
                    Segment(FishBehaviorState.ObstacleRun, FishInputHint.Release, 4.0f, 1.25f),
                    Segment(FishBehaviorState.Struggle, FishInputHint.Mixed, 4.5f, 1.15f),
                    Segment(FishBehaviorState.Turn, FishInputHint.Hold, 3.0f, 0.55f),
                }),
            };
        }

        private static S1FishBehaviorPattern CreatePattern(string id, string displayName, FishBehaviorArchetype archetype, float minTime, float maxTime, float distance, FishBehaviorSegment[] segments)
        {
            string path = $"{PatternFolder}/{id}.asset";
            S1FishBehaviorPattern pattern = AssetDatabase.LoadAssetAtPath<S1FishBehaviorPattern>(path);
            if (pattern == null)
            {
                pattern = ScriptableObject.CreateInstance<S1FishBehaviorPattern>();
                AssetDatabase.CreateAsset(pattern, path);
            }

            SerializedObject serializedObject = new SerializedObject(pattern);
            serializedObject.FindProperty("patternId").stringValue = id;
            serializedObject.FindProperty("displayName").stringValue = displayName;
            serializedObject.FindProperty("archetype").enumValueIndex = (int)archetype;
            serializedObject.FindProperty("targetMinSeconds").floatValue = minTime;
            serializedObject.FindProperty("targetMaxSeconds").floatValue = maxTime;
            serializedObject.FindProperty("startingDistance").floatValue = distance;
            serializedObject.FindProperty("landingDistance").floatValue = 1.1f;
            serializedObject.FindProperty("escapeDistance").floatValue = distance + 6f;
            SetSegmentArray(serializedObject.FindProperty("segments"), segments);
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(pattern);
            return pattern;
        }

        private static FishBehaviorSegment Segment(FishBehaviorState state, FishInputHint input, float duration, float strength)
        {
            return new FishBehaviorSegment
            {
                state = state,
                expectedInput = input,
                duration = duration,
                pullStrength = strength,
                staminaDamage = state == FishBehaviorState.Turn || state == FishBehaviorState.Approach ? 1.25f : 0.85f,
                mistakeStress = state == FishBehaviorState.Pull || state == FishBehaviorState.ObstacleRun ? 0.32f : 0.22f
            };
        }

        private static void SetSegmentArray(SerializedProperty property, FishBehaviorSegment[] values)
        {
            property.arraySize = values.Length;
            for (int i = 0; i < values.Length; i++)
            {
                SerializedProperty element = property.GetArrayElementAtIndex(i);
                element.FindPropertyRelative("state").enumValueIndex = (int)values[i].state;
                element.FindPropertyRelative("expectedInput").enumValueIndex = (int)values[i].expectedInput;
                element.FindPropertyRelative("duration").floatValue = values[i].duration;
                element.FindPropertyRelative("pullStrength").floatValue = values[i].pullStrength;
                element.FindPropertyRelative("staminaDamage").floatValue = values[i].staminaDamage;
                element.FindPropertyRelative("mistakeStress").floatValue = values[i].mistakeStress;
            }
        }

        private static void SetObjectArray<T>(SerializedProperty property, T[] values)
            where T : Object
        {
            property.arraySize = values.Length;
            for (int i = 0; i < values.Length; i++)
            {
                property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
            }
        }

        private static void SetFloatIfPresent(SerializedObject serializedObject, string propertyName, float value)
        {
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            if (property != null)
            {
                property.floatValue = value;
            }
        }

        private static void EnsureAssetFolder(string folderPath)
        {
            string[] parts = folderPath.Split('/');
            string currentPath = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                string nextPath = $"{currentPath}/{parts[i]}";
                if (!AssetDatabase.IsValidFolder(nextPath))
                {
                    AssetDatabase.CreateFolder(currentPath, parts[i]);
                }

                currentPath = nextPath;
            }
        }
    }
}
