using AfterBlue.Boat;
using AfterBlue.Core;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AfterBlue.EditorTools
{
    public static class CreateWeekZeroScene
    {
        private const string ScenePath = "Assets/Scenes/FishingScene.unity";
        private const string BoatModelPath = "Assets/Art/Exports/boat_small_v01.fbx";

        [MenuItem("AfterBlue/Setup/Create Week 0 Fishing Scene")]
        public static void CreateScene()
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "FishingScene";

            Material waterMaterial = CreateMaterial("Assets/Materials/PrototypeWater.mat", new Color(0.05f, 0.65f, 0.78f, 0.7f));
            Material boatMaterial = CreateMaterial("Assets/Materials/PrototypeBoat.mat", new Color(0.48f, 0.28f, 0.14f, 1f));
            Material deckMaterial = CreateMaterial("Assets/Materials/PrototypeDeck.mat", new Color(0.64f, 0.42f, 0.22f, 1f));

            GameObject water = GameObject.CreatePrimitive(PrimitiveType.Plane);
            water.name = "Prototype Water";
            water.transform.localScale = new Vector3(8f, 1f, 8f);
            water.GetComponent<Renderer>().sharedMaterial = waterMaterial;

            GameObject boat = new GameObject("PlayerBoat");
            boat.transform.position = new Vector3(0f, 0.18f, 0f);
            boat.AddComponent<BoatController>();

            GameObject hull = GameObject.CreatePrimitive(PrimitiveType.Cube);
            hull.name = "Prototype Hull";
            hull.transform.SetParent(boat.transform);
            hull.transform.localPosition = Vector3.zero;
            hull.transform.localScale = new Vector3(1.6f, 0.35f, 2.8f);
            hull.GetComponent<Renderer>().sharedMaterial = boatMaterial;

            GameObject bow = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bow.name = "Prototype Bow";
            bow.transform.SetParent(boat.transform);
            bow.transform.localPosition = new Vector3(0f, 0.08f, 1.35f);
            bow.transform.localRotation = Quaternion.Euler(0f, 45f, 0f);
            bow.transform.localScale = new Vector3(1.1f, 0.3f, 1.1f);
            bow.GetComponent<Renderer>().sharedMaterial = boatMaterial;

            GameObject deck = GameObject.CreatePrimitive(PrimitiveType.Cube);
            deck.name = "Prototype Deck";
            deck.transform.SetParent(boat.transform);
            deck.transform.localPosition = new Vector3(0f, 0.25f, -0.2f);
            deck.transform.localScale = new Vector3(1.2f, 0.12f, 1.2f);
            deck.GetComponent<Renderer>().sharedMaterial = deckMaterial;

            GameObject cameraObject = new GameObject("Main Camera");
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.tag = "MainCamera";
            camera.fieldOfView = 40f;
            FollowCamera followCamera = cameraObject.AddComponent<FollowCamera>();
            followCamera.SetTarget(boat.transform);
            cameraObject.transform.position = new Vector3(0f, 9f, -8f);
            cameraObject.transform.LookAt(boat.transform);

            GameObject lightObject = new GameObject("Directional Light");
            Light light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.2f;
            lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            GameObject gameManager = new GameObject("GameManager");
            gameManager.AddComponent<GameManager>();

            RenderSettings.ambientLight = new Color(0.32f, 0.48f, 0.55f);
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0.16f, 0.52f, 0.62f);
            RenderSettings.fogDensity = 0.01f;

            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(ScenePath, true)
            };

            Debug.Log($"Created AfterBlue week 0 scene at {ScenePath}");
        }

        [MenuItem("AfterBlue/Setup/Apply Week 1 Visual Pass")]
        public static void ApplyWeekOneVisualPass()
        {
            Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

            GameObject boat = GameObject.Find("PlayerBoat");
            if (boat == null)
            {
                boat = new GameObject("PlayerBoat");
                boat.transform.position = new Vector3(0f, 0.18f, 0f);
                boat.AddComponent<BoatController>();
            }

            BoatController boatController = boat.GetComponent<BoatController>();
            if (boatController == null)
            {
                boatController = boat.AddComponent<BoatController>();
            }

            SetSerializedValue(boatController, "moveSpeed", 5f);
            SetSerializedValue(boatController, "rotationSpeed", 8f);
            SetSerializedValue(boatController, "acceleration", 14f);
            SetSerializedValue(boatController, "deceleration", 10f);
            SetSerializedValue(boatController, "createPrototypeReferenceMarkers", false);

            ReplacePrototypeBoatModel(boat);
            ApplyCameraDefaults(boat.transform);
            ApplyWaterDefaults();
            CreateWeekOneScenery();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("Applied AfterBlue week 1 visual pass.");
        }

        private static Material CreateMaterial(string path, Color color)
        {
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                AssetDatabase.CreateAsset(material, path);
            }

            material.color = color;
            EditorUtility.SetDirty(material);
            return material;
        }

        private static void ReplacePrototypeBoatModel(GameObject boat)
        {
            string[] prototypeNames =
            {
                "Prototype Hull",
                "Prototype Bow",
                "Prototype Deck",
                "boat_small_v01"
            };

            foreach (Transform child in boat.transform.Cast<Transform>().ToArray())
            {
                if (prototypeNames.Any(name => child.name.StartsWith(name)))
                {
                    Object.DestroyImmediate(child.gameObject);
                }
            }

            AssetDatabase.ImportAsset(BoatModelPath, ImportAssetOptions.ForceUpdate);
            GameObject boatAsset = AssetDatabase.LoadAssetAtPath<GameObject>(BoatModelPath);
            if (boatAsset == null)
            {
                Debug.LogWarning($"Could not load boat model at {BoatModelPath}. Keep using the prototype cube boat until Unity imports the FBX.");
                return;
            }

            GameObject boatModel = (GameObject)PrefabUtility.InstantiatePrefab(boatAsset);
            boatModel.name = "boat_small_v01";
            boatModel.transform.SetParent(boat.transform, false);
            boatModel.transform.localPosition = new Vector3(0f, -0.05f, 0f);
            boatModel.transform.localRotation = Quaternion.identity;
            boatModel.transform.localScale = new Vector3(1.15f, 1.15f, 1.15f);
        }

        private static void ApplyCameraDefaults(Transform boat)
        {
            Camera camera = Camera.main;
            if (camera == null)
            {
                GameObject cameraObject = new GameObject("Main Camera");
                camera = cameraObject.AddComponent<Camera>();
                camera.tag = "MainCamera";
            }

            camera.fieldOfView = 40f;
            FollowCamera followCamera = camera.GetComponent<FollowCamera>();
            if (followCamera == null)
            {
                followCamera = camera.gameObject.AddComponent<FollowCamera>();
            }

            followCamera.SetTarget(boat);
            SetSerializedValue(followCamera, "offset", new Vector3(0f, 10f, -9f));
            SetSerializedValue(followCamera, "followSmoothTime", 0.25f);
            SetSerializedValue(followCamera, "lookAtHeight", 0.5f);
        }

        private static void ApplyWaterDefaults()
        {
            GameObject water = GameObject.Find("Prototype Water");
            if (water == null)
            {
                water = GameObject.CreatePrimitive(PrimitiveType.Plane);
                water.name = "Prototype Water";
                water.transform.localScale = new Vector3(8f, 1f, 8f);
            }

            Material waterMaterial = CreateMaterial("Assets/Materials/PrototypeWater.mat", new Color(0.02f, 0.48f, 0.62f, 0.78f));
            water.GetComponent<Renderer>().sharedMaterial = waterMaterial;
        }

        private static void CreateWeekOneScenery()
        {
            GameObject oldRoot = GameObject.Find("Week 1 Flooded Scenery");
            if (oldRoot != null)
            {
                Object.DestroyImmediate(oldRoot);
            }

            GameObject root = new GameObject("Week 1 Flooded Scenery");
            Material roofMaterial = CreateMaterial("Assets/Materials/PrototypeFloodedRoof.mat", new Color(0.08f, 0.16f, 0.18f, 1f));
            Material roadMaterial = CreateMaterial("Assets/Materials/PrototypeRoad.mat", new Color(0.10f, 0.11f, 0.12f, 1f));
            Material poleMaterial = CreateMaterial("Assets/Materials/PrototypeRustedPole.mat", new Color(0.18f, 0.14f, 0.10f, 1f));
            Material buoyMaterial = CreateMaterial("Assets/Materials/PrototypeBuoyRed.mat", new Color(0.88f, 0.16f, 0.10f, 1f));

            CreateBox(root.transform, "Flooded Roof A", new Vector3(-8f, 0.06f, 5f), new Vector3(2.8f, 0.16f, 1.6f), Quaternion.Euler(0f, 15f, 0f), roofMaterial);
            CreateBox(root.transform, "Flooded Roof B", new Vector3(4.5f, 0.06f, 7f), new Vector3(2.2f, 0.16f, 1.4f), Quaternion.Euler(0f, -12f, 0f), roofMaterial);
            CreateBox(root.transform, "Flooded Roof C", new Vector3(8f, 0.06f, -5f), new Vector3(3.2f, 0.16f, 1.8f), Quaternion.Euler(0f, 22f, 0f), roofMaterial);
            CreateBox(root.transform, "Road Segment A", new Vector3(-4f, 0.02f, -9f), new Vector3(5.5f, 0.08f, 1.2f), Quaternion.Euler(0f, -8f, 0f), roadMaterial);
            CreateBox(root.transform, "Road Segment B", new Vector3(3f, 0.02f, -10.5f), new Vector3(5.0f, 0.08f, 1.2f), Quaternion.Euler(0f, -8f, 0f), roadMaterial);
            CreateCylinder(root.transform, "Supply Buoy Placeholder", new Vector3(-9f, 0.35f, -1f), new Vector3(0.35f, 0.4f, 0.35f), Quaternion.identity, buoyMaterial);
            CreateCylinder(root.transform, "Rusted Utility Pole", new Vector3(8f, 0.9f, -2f), new Vector3(0.12f, 1.8f, 0.12f), Quaternion.Euler(0f, 0f, -18f), poleMaterial);
            CreateCylinder(root.transform, "Flooded Sign Post", new Vector3(-2f, 0.65f, -6.5f), new Vector3(0.08f, 0.9f, 0.08f), Quaternion.identity, poleMaterial);
            CreateBox(root.transform, "Flooded Sign Plate", new Vector3(-2f, 1.25f, -6.5f), new Vector3(0.9f, 0.35f, 0.08f), Quaternion.Euler(0f, 15f, 0f), roofMaterial);
        }

        private static void CreateBox(Transform parent, string name, Vector3 position, Vector3 scale, Quaternion rotation, Material material)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.name = name;
            obj.transform.SetParent(parent);
            obj.transform.SetPositionAndRotation(position, rotation);
            obj.transform.localScale = scale;
            obj.GetComponent<Renderer>().sharedMaterial = material;
        }

        private static void CreateCylinder(Transform parent, string name, Vector3 position, Vector3 scale, Quaternion rotation, Material material)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            obj.name = name;
            obj.transform.SetParent(parent);
            obj.transform.SetPositionAndRotation(position, rotation);
            obj.transform.localScale = scale;
            obj.GetComponent<Renderer>().sharedMaterial = material;
        }

        private static void SetSerializedValue(Object target, string propertyName, float value)
        {
            SerializedObject serializedObject = new SerializedObject(target);
            serializedObject.FindProperty(propertyName).floatValue = value;
            serializedObject.ApplyModifiedProperties();
        }

        private static void SetSerializedValue(Object target, string propertyName, Vector3 value)
        {
            SerializedObject serializedObject = new SerializedObject(target);
            serializedObject.FindProperty(propertyName).vector3Value = value;
            serializedObject.ApplyModifiedProperties();
        }

        private static void SetSerializedValue(Object target, string propertyName, bool value)
        {
            SerializedObject serializedObject = new SerializedObject(target);
            serializedObject.FindProperty(propertyName).boolValue = value;
            serializedObject.ApplyModifiedProperties();
        }
    }
}
