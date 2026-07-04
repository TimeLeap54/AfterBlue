using AfterBlue.Boat;
using AfterBlue.Core;
using AfterBlue.Environment;
using AfterBlue.Fishing;
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
        private const string WaterSoftPatchTexturePath = "Assets/Textures/Water/water_soft_patches_v01.png";
        private const string WaterGlintTexturePath = "Assets/Textures/Water/water_surface_glints_v01.png";
        private const string WaterWaveBandTexturePath = "Assets/Textures/Water/water_wave_bands_v01.png";

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

        [MenuItem("AfterBlue/Setup/Apply Week 2 Fishing Loop")]
        public static void ApplyWeekTwoFishingLoop()
        {
            Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

            GameObject gameManager = GameObject.Find("GameManager");
            if (gameManager == null)
            {
                gameManager = new GameObject("GameManager");
                gameManager.AddComponent<GameManager>();
            }

            if (gameManager.GetComponent<FishingCastController>() == null)
            {
                gameManager.AddComponent<FishingCastController>();
            }

            if (gameManager.GetComponent<FishingLine>() == null)
            {
                gameManager.AddComponent<FishingLine>();
            }

            if (gameManager.GetComponent<FishingDebugUI>() == null)
            {
                gameManager.AddComponent<FishingDebugUI>();
            }

            if (gameManager.GetComponent<FishingStateMachine>() == null)
            {
                gameManager.AddComponent<FishingStateMachine>();
            }

            GameObject boat = GameObject.Find("PlayerBoat");
            if (boat != null && boat.transform.Find("RodTip") == null)
            {
                GameObject rodTip = new GameObject("RodTip");
                rodTip.transform.SetParent(boat.transform, false);
                rodTip.transform.localPosition = new Vector3(0.42f, 0.72f, 0.62f);
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("Applied AfterBlue week 2 fishing loop components.");
        }

        [MenuItem("AfterBlue/Setup/Apply Week 3 Flooded Village")]
        public static void ApplyWeekThreeFloodedVillage()
        {
            Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

            ApplyWeekTwoFishingLoop();
            ApplyWeekThreePalette();
            CreateWaterDetailPlanes();
            ApplyWeekThreeCameraAndLight();
            CreateFloodedVillageSet();
            RemoveFishShadowSet();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("Applied AfterBlue week 3 Flooded Village visual pass.");
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

        private static void ConfigureTransparent(Material material)
        {
            material.SetFloat("_Surface", 1f);
            material.SetFloat("_Blend", 0f);
            material.SetFloat("_AlphaClip", 0f);
            material.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetFloat("_ZWrite", 0f);
            material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            material.DisableKeyword("_ALPHATEST_ON");
            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            EditorUtility.SetDirty(material);
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

        private static void ApplyWeekThreePalette()
        {
            Material waterMaterial = CreateMaterial("Assets/Materials/PrototypeWater.mat", new Color(0.28f, 0.86f, 0.9f, 0.72f));
            ConfigureTransparent(waterMaterial);
            waterMaterial.SetColor("_EmissionColor", new Color(0.045f, 0.18f, 0.2f, 1f));
            waterMaterial.EnableKeyword("_EMISSION");

            CreateMaterial("Assets/Materials/UnderwaterRuin.mat", new Color(0.12f, 0.19f, 0.21f, 1f));
            CreateMaterial("Assets/Materials/UnderwaterConcrete.mat", new Color(0.25f, 0.31f, 0.32f, 1f));
            CreateMaterial("Assets/Materials/AsphaltFlooded.mat", new Color(0.145f, 0.173f, 0.184f, 1f));
            CreateMaterial("Assets/Materials/RustedMetal.mat", new Color(0.478f, 0.294f, 0.208f, 1f));
            CreateMaterial("Assets/Materials/MossAlgae.mat", new Color(0.31f, 0.435f, 0.259f, 1f));
            CreateMaterial("Assets/Materials/Ripple.mat", new Color(0.78f, 0.96f, 1f, 0.72f));
            CreateWaterDetailMaterial("Assets/Materials/WaterNoiseOverlay.mat", WaterSoftPatchTexturePath, new Color(0.7f, 0.96f, 1f, 0.6f), 3100, new Vector2(1.15f, 1.15f));
            CreateWaterDetailMaterial("Assets/Materials/WaterWaveBandsOverlay.mat", WaterWaveBandTexturePath, new Color(0.82f, 1f, 1f, 0.72f), 3110, new Vector2(1.05f, 1.05f));
            CreateWaterDetailMaterial("Assets/Materials/WaterLineOverlay.mat", WaterGlintTexturePath, new Color(0.86f, 0.99f, 1f, 0.58f), 3120, new Vector2(1.35f, 1.35f));

            GameObject water = GameObject.Find("Prototype Water");
            if (water == null)
            {
                water = GameObject.CreatePrimitive(PrimitiveType.Plane);
                water.name = "Prototype Water";
            }

            water.transform.localScale = new Vector3(12f, 1f, 12f);
            water.transform.position = Vector3.zero;
            water.GetComponent<Renderer>().sharedMaterial = waterMaterial;

            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogColor = new Color(0.13f, 0.42f, 0.48f, 1f);
            RenderSettings.fogDensity = 0.018f;
            RenderSettings.ambientLight = new Color(0.22f, 0.36f, 0.39f, 1f);
        }

        private static Material CreateWaterDetailMaterial(string path, string texturePath, Color color, int renderQueue, Vector2 textureScale)
        {
            Material material = CreateMaterial(path, color);
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
            if (texture != null)
            {
                material.mainTexture = texture;
                material.mainTextureScale = textureScale;
            }

            ConfigureTransparent(material);
            material.renderQueue = renderQueue;
            material.SetFloat("_Smoothness", 0f);
            EditorUtility.SetDirty(material);
            return material;
        }

        private static void CreateWaterDetailPlanes()
        {
            GameObject oldRoot = GameObject.Find("Week 3 Water Detail");
            if (oldRoot != null)
            {
                Object.DestroyImmediate(oldRoot);
            }

            GameObject root = new GameObject("Week 3 Water Detail");
            Material patchMaterial = CreateWaterDetailMaterial("Assets/Materials/WaterNoiseOverlay.mat", WaterSoftPatchTexturePath, new Color(0.7f, 0.96f, 1f, 0.6f), 3100, new Vector2(1.15f, 1.15f));
            Material waveBandMaterial = CreateWaterDetailMaterial("Assets/Materials/WaterWaveBandsOverlay.mat", WaterWaveBandTexturePath, new Color(0.82f, 1f, 1f, 0.72f), 3110, new Vector2(1.05f, 1.05f));
            Material glintMaterial = CreateWaterDetailMaterial("Assets/Materials/WaterLineOverlay.mat", WaterGlintTexturePath, new Color(0.86f, 0.99f, 1f, 0.58f), 3120, new Vector2(1.35f, 1.35f));

            CreateWaterOverlay(root.transform, "Soft Cyan Water Patches", 0.055f, 12.05f, patchMaterial, new Vector2(0.006f, 0.003f), 0.055f, 0.18f);
            CreateWaterOverlay(root.transform, "Broad Moving Wave Bands", 0.068f, 12.02f, waveBandMaterial, new Vector2(-0.018f, 0.006f), 0.12f, 0.42f);
            CreateWaterOverlay(root.transform, "Subtle Surface Glints", 0.075f, 12.0f, glintMaterial, new Vector2(-0.011f, 0.007f), 0.06f, 0.32f);
        }

        private static void CreateWaterOverlay(Transform parent, string name, float height, float scale, Material material, Vector2 scrollSpeed, float alphaPulse, float pulseSpeed)
        {
            GameObject overlay = GameObject.CreatePrimitive(PrimitiveType.Plane);
            overlay.name = name;
            overlay.transform.SetParent(parent, false);
            overlay.transform.position = new Vector3(0f, height, 0f);
            overlay.transform.localScale = new Vector3(scale, 1f, scale);
            overlay.GetComponent<Renderer>().sharedMaterial = material;

            WaterSurfaceController controller = overlay.AddComponent<WaterSurfaceController>();
            SerializedObject serializedController = new SerializedObject(controller);
            serializedController.FindProperty("scrollSpeed").vector2Value = scrollSpeed;
            serializedController.FindProperty("alphaPulse").floatValue = alphaPulse;
            serializedController.FindProperty("pulseSpeed").floatValue = pulseSpeed;
            serializedController.ApplyModifiedProperties();
        }

        private static void ApplyWeekThreeCameraAndLight()
        {
            GameObject boat = GameObject.Find("PlayerBoat");
            if (boat != null)
            {
                ApplyCameraDefaults(boat.transform);
            }

            Camera camera = Camera.main;
            if (camera != null)
            {
                camera.fieldOfView = 39f;
                FollowCamera followCamera = camera.GetComponent<FollowCamera>();
                if (followCamera != null)
                {
                    SetSerializedValue(followCamera, "offset", new Vector3(0f, 10.5f, -8.25f));
                    SetSerializedValue(followCamera, "followSmoothTime", 0.22f);
                    SetSerializedValue(followCamera, "lookAtHeight", 0.35f);
                }
            }

            Light directional = Object.FindObjectsOfType<Light>().FirstOrDefault(light => light.type == LightType.Directional);
            if (directional != null)
            {
                directional.intensity = 0.95f;
                directional.color = new Color(0.78f, 0.94f, 1f, 1f);
                directional.transform.rotation = Quaternion.Euler(48f, -34f, 0f);
            }
        }

        private static void CreateFloodedVillageSet()
        {
            GameObject oldRoot = GameObject.Find("Week 3 Flooded Village");
            if (oldRoot != null)
            {
                Object.DestroyImmediate(oldRoot);
            }

            GameObject root = new GameObject("Week 3 Flooded Village");
            root.AddComponent<FloodedVillageScenery>();

            Material ruin = CreateMaterial("Assets/Materials/UnderwaterRuin.mat", new Color(0.12f, 0.19f, 0.21f, 1f));
            Material concrete = CreateMaterial("Assets/Materials/UnderwaterConcrete.mat", new Color(0.25f, 0.31f, 0.32f, 1f));
            Material asphalt = CreateMaterial("Assets/Materials/AsphaltFlooded.mat", new Color(0.145f, 0.173f, 0.184f, 1f));
            Material rust = CreateMaterial("Assets/Materials/RustedMetal.mat", new Color(0.478f, 0.294f, 0.208f, 1f));
            Material moss = CreateMaterial("Assets/Materials/MossAlgae.mat", new Color(0.31f, 0.435f, 0.259f, 1f));
            Material bobberRed = CreateMaterial("Assets/Materials/PrototypeBuoyRed.mat", new Color(0.91f, 0.35f, 0.35f, 1f));

            CreateSubmergedRoof(root.transform, "Submerged House Roof A", new Vector3(-4.7f, 0.08f, 5.0f), Quaternion.Euler(0f, 12f, 0f), new Vector3(3.5f, 0.18f, 2.35f), ruin, concrete, moss);
            CreateSubmergedRoof(root.transform, "Submerged House Roof B", new Vector3(4.0f, 0.06f, 5.8f), Quaternion.Euler(0f, -18f, 0f), new Vector3(3.0f, 0.16f, 2.0f), ruin, concrete, moss);
            CreateSubmergedRoof(root.transform, "Submerged House Roof C", new Vector3(7.2f, 0.04f, -2.8f), Quaternion.Euler(0f, 22f, 0f), new Vector3(3.4f, 0.16f, 2.2f), ruin, concrete, moss);

            CreateRoadSegment(root.transform, "Flooded Asphalt Road A", new Vector3(-3.2f, 0.025f, -5.8f), Quaternion.Euler(0f, -7f, 0f), new Vector3(7.2f, 0.05f, 1.65f), asphalt, concrete);
            CreateRoadSegment(root.transform, "Flooded Asphalt Road B", new Vector3(3.8f, 0.02f, -7.0f), Quaternion.Euler(0f, -7f, 0f), new Vector3(6.0f, 0.05f, 1.65f), asphalt, concrete);

            CreateUtilityPole(root.transform, "Leaning Utility Pole", new Vector3(-6.6f, 0.78f, -1.5f), Quaternion.Euler(0f, 0f, -17f), rust, moss);
            CreateFloodedTrafficLight(root.transform, "Flooded Traffic Light", new Vector3(2.6f, 0.68f, -4.1f), Quaternion.Euler(0f, 18f, -7f), rust, bobberRed);
            CreateWindowWall(root.transform, "Broken Window Wall A", new Vector3(-7.4f, 0.26f, 2.6f), Quaternion.Euler(0f, -24f, 0f), concrete, moss);
            CreateWindowWall(root.transform, "Broken Window Wall B", new Vector3(6.0f, 0.22f, 2.2f), Quaternion.Euler(0f, 31f, 0f), concrete, moss);

            CreateBox(root.transform, "Algae Patch A", new Vector3(-5.6f, 0.11f, 4.1f), new Vector3(1.3f, 0.03f, 0.55f), Quaternion.Euler(0f, 22f, 0f), moss);
            CreateBox(root.transform, "Algae Patch B", new Vector3(3.4f, 0.1f, 4.9f), new Vector3(1.0f, 0.03f, 0.45f), Quaternion.Euler(0f, -18f, 0f), moss);
            CreateBox(root.transform, "Algae Patch C", new Vector3(-1.0f, 0.08f, -4.8f), new Vector3(1.8f, 0.03f, 0.35f), Quaternion.Euler(0f, -7f, 0f), moss);
            CreateCylinder(root.transform, "Submerged Rock A", new Vector3(-2.0f, 0.08f, 6.9f), new Vector3(0.55f, 0.08f, 0.38f), Quaternion.Euler(0f, 0f, 90f), concrete);
            CreateCylinder(root.transform, "Submerged Rock B", new Vector3(5.6f, 0.07f, -5.6f), new Vector3(0.5f, 0.07f, 0.32f), Quaternion.Euler(0f, 0f, 90f), concrete);
        }

        private static void RemoveFishShadowSet()
        {
            GameObject oldRoot = GameObject.Find("Week 3 Fish Shadows");
            if (oldRoot != null)
            {
                Object.DestroyImmediate(oldRoot);
            }
        }

        private static void CreateSubmergedRoof(Transform parent, string name, Vector3 position, Quaternion rotation, Vector3 scale, Material roofMaterial, Material concreteMaterial, Material mossMaterial)
        {
            GameObject root = new GameObject(name);
            root.transform.SetParent(parent);
            root.transform.SetPositionAndRotation(position, rotation);

            CreateBox(root.transform, "Wet Roof Slab", Vector3.zero, scale, Quaternion.identity, roofMaterial);
            CreateBox(root.transform, "Concrete Rim Front", new Vector3(0f, 0.08f, -scale.z * 0.48f), new Vector3(scale.x * 1.05f, 0.08f, 0.08f), Quaternion.identity, concreteMaterial);
            CreateBox(root.transform, "Concrete Rim Back", new Vector3(0f, 0.08f, scale.z * 0.48f), new Vector3(scale.x * 1.05f, 0.08f, 0.08f), Quaternion.identity, concreteMaterial);
            CreateBox(root.transform, "Broken Roof Hole", new Vector3(-scale.x * 0.23f, 0.11f, scale.z * 0.05f), new Vector3(scale.x * 0.28f, 0.035f, scale.z * 0.32f), Quaternion.Euler(0f, 8f, 0f), concreteMaterial);
            CreateBox(root.transform, "Skylight Frame", new Vector3(scale.x * 0.22f, 0.13f, -scale.z * 0.14f), new Vector3(scale.x * 0.22f, 0.04f, scale.z * 0.2f), Quaternion.identity, concreteMaterial);
            CreateBox(root.transform, "Moss Streak", new Vector3(-scale.x * 0.08f, 0.15f, -scale.z * 0.28f), new Vector3(scale.x * 0.34f, 0.025f, scale.z * 0.07f), Quaternion.Euler(0f, -10f, 0f), mossMaterial);
        }

        private static void CreateRoadSegment(Transform parent, string name, Vector3 position, Quaternion rotation, Vector3 scale, Material asphaltMaterial, Material concreteMaterial)
        {
            GameObject root = new GameObject(name);
            root.transform.SetParent(parent);
            root.transform.SetPositionAndRotation(position, rotation);

            CreateBox(root.transform, "Dark Asphalt", Vector3.zero, scale, Quaternion.identity, asphaltMaterial);
            CreateBox(root.transform, "Lane Mark A", new Vector3(-scale.x * 0.16f, 0.035f, 0f), new Vector3(scale.x * 0.22f, 0.018f, 0.035f), Quaternion.identity, concreteMaterial);
            CreateBox(root.transform, "Lane Mark B", new Vector3(scale.x * 0.18f, 0.035f, 0f), new Vector3(scale.x * 0.22f, 0.018f, 0.035f), Quaternion.identity, concreteMaterial);
            CreateBox(root.transform, "Broken Road Edge", new Vector3(scale.x * 0.41f, 0.045f, -scale.z * 0.34f), new Vector3(scale.x * 0.18f, 0.025f, scale.z * 0.24f), Quaternion.Euler(0f, 17f, 0f), concreteMaterial);
        }

        private static void CreateUtilityPole(Transform parent, string name, Vector3 position, Quaternion rotation, Material rustMaterial, Material mossMaterial)
        {
            GameObject root = new GameObject(name);
            root.transform.SetParent(parent);
            root.transform.SetPositionAndRotation(position, rotation);

            CreateCylinder(root.transform, "Rusted Pole", Vector3.zero, new Vector3(0.12f, 1.45f, 0.12f), Quaternion.identity, rustMaterial);
            CreateBox(root.transform, "Broken Cross Arm", new Vector3(0f, 1.15f, 0f), new Vector3(1.35f, 0.09f, 0.09f), Quaternion.Euler(0f, 0f, 8f), rustMaterial);
            CreateCylinder(root.transform, "Street Lamp Arm", new Vector3(-0.55f, 0.78f, 0f), new Vector3(0.035f, 0.75f, 0.035f), Quaternion.Euler(0f, 0f, 78f), rustMaterial);
            CreateCylinder(root.transform, "Lamp Head", new Vector3(-1.02f, 0.64f, 0f), new Vector3(0.12f, 0.08f, 0.12f), Quaternion.Euler(90f, 0f, 0f), rustMaterial);
            CreateBox(root.transform, "Transformer Box", new Vector3(0.26f, 0.55f, 0f), new Vector3(0.26f, 0.46f, 0.22f), Quaternion.identity, rustMaterial);
            CreateBox(root.transform, "Algae Waterline", new Vector3(0f, -0.72f, 0f), new Vector3(0.18f, 0.28f, 0.18f), Quaternion.identity, mossMaterial);
        }

        private static void CreateFloodedTrafficLight(Transform parent, string name, Vector3 position, Quaternion rotation, Material rustMaterial, Material accentMaterial)
        {
            GameObject root = new GameObject(name);
            root.transform.SetParent(parent);
            root.transform.SetPositionAndRotation(position, rotation);

            CreateCylinder(root.transform, "Bent Signal Pole", new Vector3(0f, -0.2f, 0f), new Vector3(0.07f, 0.9f, 0.07f), Quaternion.identity, rustMaterial);
            CreateBox(root.transform, "Signal Housing", new Vector3(0f, 0.62f, 0f), new Vector3(0.34f, 0.58f, 0.16f), Quaternion.identity, rustMaterial);
            CreateCylinder(root.transform, "Dim Red Lens", new Vector3(0f, 0.79f, -0.085f), new Vector3(0.07f, 0.016f, 0.07f), Quaternion.Euler(90f, 0f, 0f), accentMaterial);
            CreateCylinder(root.transform, "Dead Middle Lens", new Vector3(0f, 0.62f, -0.085f), new Vector3(0.06f, 0.014f, 0.06f), Quaternion.Euler(90f, 0f, 0f), rustMaterial);
            CreateCylinder(root.transform, "Dead Lower Lens", new Vector3(0f, 0.45f, -0.085f), new Vector3(0.06f, 0.014f, 0.06f), Quaternion.Euler(90f, 0f, 0f), rustMaterial);
        }

        private static void CreateWindowWall(Transform parent, string name, Vector3 position, Quaternion rotation, Material concreteMaterial, Material mossMaterial)
        {
            GameObject root = new GameObject(name);
            root.transform.SetParent(parent);
            root.transform.SetPositionAndRotation(position, rotation);

            CreateBox(root.transform, "Wall Left", new Vector3(-0.45f, 0f, 0f), new Vector3(0.18f, 0.8f, 0.12f), Quaternion.identity, concreteMaterial);
            CreateBox(root.transform, "Wall Right", new Vector3(0.45f, 0f, 0f), new Vector3(0.18f, 0.72f, 0.12f), Quaternion.identity, concreteMaterial);
            CreateBox(root.transform, "Wall Top", new Vector3(0f, 0.36f, 0f), new Vector3(1.08f, 0.14f, 0.12f), Quaternion.identity, concreteMaterial);
            CreateBox(root.transform, "Window Sill", new Vector3(0f, -0.34f, 0f), new Vector3(1.0f, 0.1f, 0.12f), Quaternion.identity, concreteMaterial);
            CreateBox(root.transform, "Moss Edge", new Vector3(0.1f, -0.42f, -0.01f), new Vector3(0.75f, 0.05f, 0.13f), Quaternion.identity, mossMaterial);
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
            obj.transform.SetParent(parent, false);
            obj.transform.localPosition = position;
            obj.transform.localRotation = rotation;
            obj.transform.localScale = scale;
            obj.GetComponent<Renderer>().sharedMaterial = material;
        }

        private static void CreateCylinder(Transform parent, string name, Vector3 position, Vector3 scale, Quaternion rotation, Material material)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            obj.name = name;
            obj.transform.SetParent(parent, false);
            obj.transform.localPosition = position;
            obj.transform.localRotation = rotation;
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
