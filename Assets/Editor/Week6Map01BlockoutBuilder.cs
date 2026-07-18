using AfterBlue.Boat;
using AfterBlue.Core;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AfterBlue.EditorTools
{
    public static class Week6Map01BlockoutBuilder
    {
        private const string ScenePath = "Assets/AfterBlue/Scenes/Map_01/Map_01_Week6_Blockout.unity";
        private const string DebugMaterialFolder = "Assets/AfterBlue/Materials/Debug";
        private const string BoatModelPath = "Assets/Art/Exports/boat_small_v01.fbx";
        private const float GameplayWidth = 192f;
        private const float GameplayDepth = 128f;
        private const float WaterWidth = 280f;
        private const float WaterDepth = 192f;
        private const float NodeExpansion = 2.0f;

        [MenuItem("AfterBlue/Setup/Apply Week 6 Map 01 Blockout")]
        public static void ApplyWeek6Map01Blockout()
        {
            EnsureFolder("Assets/AfterBlue");
            EnsureFolder("Assets/AfterBlue/Scenes");
            EnsureFolder("Assets/AfterBlue/Scenes/Map_01");
            EnsureFolder("Assets/AfterBlue/Materials");
            EnsureFolder(DebugMaterialFolder);

            Material water = CreateDebugMaterial($"{DebugMaterialFolder}/MAT_Debug_Water.mat", new Color(0.04f, 0.72f, 0.78f, 0.48f), true);
            Material zone = CreateDebugMaterial($"{DebugMaterialFolder}/MAT_Debug_Zone.mat", new Color(1f, 0.82f, 0.12f, 0.65f), true);
            Material route = CreateDebugMaterial($"{DebugMaterialFolder}/MAT_Debug_Route.mat", new Color(0.20f, 0.55f, 1f, 0.7f), true);
            Material blocked = CreateDebugMaterial($"{DebugMaterialFolder}/MAT_Debug_Blocked.mat", new Color(1f, 0.16f, 0.12f, 0.68f), true);
            Material boundary = CreateDebugMaterial($"{DebugMaterialFolder}/MAT_Debug_Boundary.mat", new Color(0.72f, 0.22f, 1f, 0.9f), true);
            Material boatReference = CreateDebugMaterial($"{DebugMaterialFolder}/MAT_Debug_BoatReference.mat", new Color(1f, 1f, 1f, 0.22f), true);

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "Map_01_Week6_Blockout";

            GameObject root = new GameObject("Map_01_Week6_Blockout");
            Transform system = CreateChild(root.transform, "00_SYSTEM").transform;
            Transform waterRoot = CreateChild(root.transform, "01_WATER").transform;
            Transform mapGuides = CreateChild(root.transform, "02_MAP_GUIDES").transform;
            Transform zones = CreateChild(root.transform, "03_ZONE_MARKERS").transform;
            Transform routes = CreateChild(root.transform, "04_ROUTE_GUIDES").transform;
            CreateChild(root.transform, "05_OBSTACLE_BLOCKOUT");
            Transform boundaryRoot = CreateChild(root.transform, "06_BOUNDARY").transform;
            CreateChild(root.transform, "07_LANDMARKS");
            CreateChild(root.transform, "08_FISHING_ZONES");
            Transform debug = CreateChild(root.transform, "99_DEBUG").transform;

            CreateWaterBlockout(waterRoot, water);
            CreateGameplayAreaGuide(mapGuides, zone);
            CreateCornerMarkers(mapGuides, boundary);
            CreateBoundaryFrame(boundaryRoot, boundary);
            CreateSpeedMarkers(debug, route);
            CreateCrossingMarkers(debug, boundary);
            CreateZoneMarkers(zones, zone);
            CreateRouteGuides(routes, route);
            CreateBoatAndCamera(system, boatReference);
            CreateP0SpecNotes(system);
            CreateP1SpecNotes(system);
            CreateLighting(system);

            RenderSettings.ambientLight = new Color(0.62f, 0.70f, 0.72f, 1f);
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Linear;
            RenderSettings.fogColor = new Color(0.62f, 0.76f, 0.78f, 1f);
            RenderSettings.fogStartDistance = 60f;
            RenderSettings.fogEndDistance = 145f;

            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();
            Debug.Log($"Applied Week 6 Map_01 blockout. Scene: {ScenePath}");
        }

        private static void CreateWaterBlockout(Transform parent, Material material)
        {
            GameObject water = CreateBox(parent, "Water_Blockout", new Vector3(0f, -0.05f, 0f), new Vector3(WaterWidth, 0.10f, WaterDepth), Quaternion.identity, material);
            DestroyCollider(water);
        }

        private static void CreateGameplayAreaGuide(Transform parent, Material material)
        {
            GameObject area = CreateBox(parent, "GameplayArea_Debug_192x128", new Vector3(0f, -0.04f, 0f), new Vector3(GameplayWidth, 0.02f, GameplayDepth), Quaternion.identity, material);
            DestroyCollider(area);
        }

        private static void CreateCornerMarkers(Transform parent, Material material)
        {
            float halfWidth = GameplayWidth * 0.5f;
            float halfDepth = GameplayDepth * 0.5f;
            CreateBox(parent, "Corner_NW", new Vector3(-halfWidth, 2.5f, halfDepth), new Vector3(0.5f, 5f, 0.5f), Quaternion.identity, material);
            CreateBox(parent, "Corner_NE", new Vector3(halfWidth, 2.5f, halfDepth), new Vector3(0.5f, 5f, 0.5f), Quaternion.identity, material);
            CreateBox(parent, "Corner_SW", new Vector3(-halfWidth, 2.5f, -halfDepth), new Vector3(0.5f, 5f, 0.5f), Quaternion.identity, material);
            CreateBox(parent, "Corner_SE", new Vector3(halfWidth, 2.5f, -halfDepth), new Vector3(0.5f, 5f, 0.5f), Quaternion.identity, material);
        }

        private static void CreateBoundaryFrame(Transform parent, Material material)
        {
            float halfWidth = GameplayWidth * 0.5f;
            float halfDepth = GameplayDepth * 0.5f;
            GameObject north = CreateBox(parent, "Boundary_North_192x128", new Vector3(0f, 0.18f, halfDepth), new Vector3(GameplayWidth, 0.12f, 0.18f), Quaternion.identity, material);
            GameObject south = CreateBox(parent, "Boundary_South_192x128", new Vector3(0f, 0.18f, -halfDepth), new Vector3(GameplayWidth, 0.12f, 0.18f), Quaternion.identity, material);
            GameObject east = CreateBox(parent, "Boundary_East_192x128", new Vector3(halfWidth, 0.18f, 0f), new Vector3(0.18f, 0.12f, GameplayDepth), Quaternion.identity, material);
            GameObject west = CreateBox(parent, "Boundary_West_192x128", new Vector3(-halfWidth, 0.18f, 0f), new Vector3(0.18f, 0.12f, GameplayDepth), Quaternion.identity, material);
            DestroyCollider(north);
            DestroyCollider(south);
            DestroyCollider(east);
            DestroyCollider(west);
        }

        private static void CreateSpeedMarkers(Transform parent, Material material)
        {
            CreateBox(parent, "SpeedMarker_0m", new Vector3(0f, 1f, -10f), new Vector3(0.2f, 2f, 0.2f), Quaternion.identity, material);
            CreateBox(parent, "SpeedMarker_10m", new Vector3(0f, 1f, 0f), new Vector3(0.2f, 2f, 0.2f), Quaternion.identity, material);
            CreateBox(parent, "SpeedMarker_20m", new Vector3(0f, 1f, 10f), new Vector3(0.2f, 2f, 0.2f), Quaternion.identity, material);
        }

        private static void CreateCrossingMarkers(Transform parent, Material material)
        {
            float halfWidth = GameplayWidth * 0.5f;
            float halfDepth = GameplayDepth * 0.5f;
            CreateBox(parent, "CrossingMarker_WestEdge", new Vector3(-halfWidth, 1.25f, 0f), new Vector3(0.5f, 2.5f, 4f), Quaternion.identity, material);
            CreateBox(parent, "CrossingMarker_EastEdge", new Vector3(halfWidth, 1.25f, 0f), new Vector3(0.5f, 2.5f, 4f), Quaternion.identity, material);
            CreateBox(parent, "CrossingMarker_SouthEdge", new Vector3(0f, 1.25f, -halfDepth), new Vector3(4f, 2.5f, 0.5f), Quaternion.identity, material);
            CreateBox(parent, "CrossingMarker_NorthEdge", new Vector3(0f, 1.25f, halfDepth), new Vector3(4f, 2.5f, 0.5f), Quaternion.identity, material);
        }

        private static void CreateZoneMarkers(Transform parent, Material material)
        {
            CreateZone(parent, "ZONE_S_Start", ExpandNode(-38f, -20f), new Vector2(12f, 12f), material);
            CreateZone(parent, "ZONE_A_ShallowVillage", ExpandNode(-28f, 14f), new Vector2(18f, 16f), material);
            CreateZone(parent, "ZONE_M_CentralWater", ExpandNode(-6f, 2f), new Vector2(26f, 22f), material);
            CreateZone(parent, "ZONE_B_Intersection", ExpandNode(24f, 14f), new Vector2(18f, 16f), material);
            CreateZone(parent, "ZONE_C_DeepDebris", ExpandNode(30f, -18f), new Vector2(21f, 18f), material);
            CreateZone(parent, "ZONE_D_ReturnWater", ExpandNode(-8f, -20f), new Vector2(20f, 14f), material);
        }

        private static void CreateZone(Transform parent, string name, Vector3 position, Vector2 size, Material material)
        {
            GameObject zone = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            zone.name = name;
            zone.transform.SetParent(parent, false);
            zone.transform.localPosition = position;
            zone.transform.localScale = new Vector3(size.x, 0.05f, size.y);
            zone.GetComponent<Renderer>().sharedMaterial = material;
            DestroyCollider(zone);
        }

        private static void CreateRouteGuides(Transform parent, Material material)
        {
            Vector3 s = ExpandNode(-38f, -20f);
            Vector3 a = ExpandNode(-28f, 14f);
            Vector3 m = ExpandNode(-6f, 2f);
            Vector3 b = ExpandNode(24f, 14f);
            Vector3 c = ExpandNode(30f, -18f);
            Vector3 d = ExpandNode(-8f, -20f);

            CreateRoute(parent, "Route_S_to_A", s, a, material);
            CreateRoute(parent, "Route_S_to_D", s, d, material);
            CreateRoute(parent, "Route_A_to_M", a, m, material);
            CreateRoute(parent, "Route_A_to_B", a, b, material);
            CreateRoute(parent, "Route_M_to_B", m, b, material);
            CreateRoute(parent, "Route_M_to_D", m, d, material);
            CreateRoute(parent, "Route_B_to_C", b, c, material);
            CreateRoute(parent, "Route_C_to_D", c, d, material);
        }

        private static void CreateRoute(Transform parent, string name, Vector3 start, Vector3 end, Material material)
        {
            start.y = 0.05f;
            end.y = 0.05f;
            Vector3 midpoint = (start + end) * 0.5f;
            Vector3 direction = end - start;
            GameObject route = CreateBox(parent, name, midpoint, new Vector3(0.18f, 0.04f, direction.magnitude), Quaternion.LookRotation(direction.normalized, Vector3.up), material);
            DestroyCollider(route);
        }

        private static void CreateBoatAndCamera(Transform parent, Material referenceMaterial)
        {
            GameObject boat = new GameObject("PlayerBoat");
            boat.transform.SetParent(parent, false);
            boat.transform.localPosition = ExpandNode(-38f, -22.2f) + Vector3.up * 0.18f;
            boat.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

            GameObject model = AssetDatabase.LoadAssetAtPath<GameObject>(BoatModelPath);
            if (model != null)
            {
                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(model);
                instance.name = "Boat Model";
                instance.transform.SetParent(boat.transform, false);
                instance.transform.localPosition = Vector3.zero;
                instance.transform.localRotation = Quaternion.identity;
                instance.transform.localScale = Vector3.one;
            }
            else
            {
                Material boatMaterial = CreateDebugMaterial($"{DebugMaterialFolder}/MAT_Debug_BoatFallback.mat", new Color(0.42f, 0.30f, 0.18f, 1f), false);
                CreateBox(boat.transform, "Boat_Fallback_4p5m", Vector3.zero, new Vector3(1.8f, 0.6f, 4.5f), Quaternion.identity, boatMaterial);
            }

            GameObject reference = CreateBox(parent, "BoatSize_Reference", ExpandNode(-34f, -22.2f) + Vector3.up * 0.5f, new Vector3(1.8f, 1f, 4.5f), Quaternion.identity, referenceMaterial);
            DestroyCollider(reference);

            BoatController controller = boat.AddComponent<BoatController>();
            SerializedObject controllerObject = new SerializedObject(controller);
            controllerObject.FindProperty("moveSpeed").floatValue = 7.2f;
            controllerObject.FindProperty("reverseSpeed").floatValue = 3.2f;
            controllerObject.FindProperty("rotationSpeed").floatValue = 8f;
            controllerObject.FindProperty("steeringTurnSpeed").floatValue = 82f;
            controllerObject.FindProperty("acceleration").floatValue = 11.0f;
            controllerObject.FindProperty("deceleration").floatValue = 9.0f;
            controllerObject.FindProperty("movementSpace").enumValueIndex = (int)BoatMovementSpace.WorldAxes;
            controllerObject.FindProperty("controlStyle").enumValueIndex = (int)BoatControlStyle.HeadingSteer;
            controllerObject.FindProperty("rotateWhileReversing").boolValue = false;
            controllerObject.FindProperty("invertSteeringWhenReversing").boolValue = true;
            controllerObject.FindProperty("showDebugOverlay").boolValue = true;
            controllerObject.FindProperty("createPrototypeReferenceMarkers").boolValue = false;
            controllerObject.ApplyModifiedPropertiesWithoutUndo();

            GameObject cameraObject = new GameObject("Main Camera");
            cameraObject.transform.SetParent(parent, false);
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.tag = "MainCamera";
            camera.fieldOfView = 58f;

            FollowCamera follow = cameraObject.AddComponent<FollowCamera>();
            follow.SetTarget(boat.transform);
            SerializedObject followObject = new SerializedObject(follow);
            followObject.FindProperty("offset").vector3Value = new Vector3(0f, 9f, -15f);
            followObject.FindProperty("followSmoothTime").floatValue = 0.28f;
            followObject.FindProperty("lookTargetSmoothTime").floatValue = 0.16f;
            followObject.FindProperty("rotationSharpness").floatValue = 6f;
            followObject.FindProperty("lookAtHeight").floatValue = 0.55f;
            followObject.FindProperty("lookAheadDistance").floatValue = 14f;
            followObject.FindProperty("useTargetLocalOffset").boolValue = true;
            followObject.ApplyModifiedPropertiesWithoutUndo();

            cameraObject.transform.position = boat.transform.position + new Vector3(0f, 9f, -15f);
            cameraObject.transform.LookAt(boat.transform.position + Vector3.up * 0.55f + boat.transform.forward * 14f);
        }

        private static Vector3 ExpandNode(float x, float z)
        {
            return new Vector3(x * NodeExpansion, 0f, z * NodeExpansion);
        }

        private static void CreateLighting(Transform parent)
        {
            GameObject lightObject = new GameObject("Directional Light");
            lightObject.transform.SetParent(parent, false);
            Light light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 0.85f;
            light.color = new Color(0.86f, 0.96f, 1f, 1f);
            lightObject.transform.rotation = Quaternion.Euler(50f, -32f, 0f);
        }

        private static void CreateP0SpecNotes(Transform parent)
        {
            GameObject notes = new GameObject("P0_SPEC_Unity1m_Blender1m_WaterY0");
            notes.transform.SetParent(parent, false);
            notes.transform.localPosition = Vector3.zero;
        }

        private static void CreateP1SpecNotes(Transform parent)
        {
            GameObject notes = new GameObject("P1_TEST_192x128_Boat7p2ms");
            notes.transform.SetParent(parent, false);
            notes.transform.localPosition = new Vector3(0f, 0f, 2f);
        }

        private static GameObject CreateChild(Transform parent, string name)
        {
            GameObject child = new GameObject(name);
            child.transform.SetParent(parent, false);
            return child;
        }

        private static GameObject CreateBox(Transform parent, string name, Vector3 position, Vector3 scale, Quaternion rotation, Material material)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.name = name;
            obj.transform.SetParent(parent, false);
            obj.transform.localPosition = position;
            obj.transform.localRotation = rotation;
            obj.transform.localScale = scale;
            obj.GetComponent<Renderer>().sharedMaterial = material;
            return obj;
        }

        private static void DestroyCollider(GameObject obj)
        {
            Collider collider = obj.GetComponent<Collider>();
            if (collider != null)
            {
                Object.DestroyImmediate(collider);
            }
        }

        private static Material CreateDebugMaterial(string path, Color color, bool transparent)
        {
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
                material = new Material(shader);
                AssetDatabase.CreateAsset(material, path);
            }

            material.color = color;
            if (material.HasProperty("_BaseColor"))
            {
                material.SetColor("_BaseColor", color);
            }

            if (transparent)
            {
                if (material.HasProperty("_Surface"))
                {
                    material.SetFloat("_Surface", 1f);
                }

                if (material.HasProperty("_Blend"))
                {
                    material.SetFloat("_Blend", 0f);
                }

                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            }

            EditorUtility.SetDirty(material);
            return material;
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
            {
                return;
            }

            string parent = System.IO.Path.GetDirectoryName(path)?.Replace("\\", "/");
            string name = System.IO.Path.GetFileName(path);
            if (!string.IsNullOrEmpty(parent))
            {
                EnsureFolder(parent);
                AssetDatabase.CreateFolder(parent, name);
            }
        }
    }
}
