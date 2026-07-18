using AfterBlue.Boat;
using AfterBlue.Core;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AfterBlue.EditorTools
{
    public static class Week6Map01Builder
    {
        private const string ScenePath = "Assets/AfterBlue/Scenes/Map_01/Map_01_Week6.unity";
        private const string DebugMaterialFolder = "Assets/AfterBlue/Materials/Debug";
        private const string BoatModelPath = "Assets/Art/Exports/boat_small_v01.fbx";
        private const string RoofModelPath = "Assets/Art/Exports/flooded_roof_modern_v01.fbx";
        private const string UtilityPoleModelPath = "Assets/Art/Exports/rusted_utility_pole_v01.fbx";

        private const float GameplayWidth = 192f;
        private const float GameplayDepth = 128f;
        private const float WaterWidth = 280f;
        private const float WaterDepth = 192f;
        private const float NodeExpansion = 2.0f;

        private static readonly Vector3 Start = ExpandNode(-38f, -20f);
        private static readonly Vector3 H1 = ExpandNode(-28f, 14f);
        private static readonly Vector3 M = ExpandNode(-6f, 2f);
        private static readonly Vector3 H2 = ExpandNode(24f, 14f);
        private static readonly Vector3 H3 = ExpandNode(30f, -18f);
        private static readonly Vector3 D = ExpandNode(-8f, -20f);

        [MenuItem("AfterBlue/Setup/Apply Week 6 Map 01")]
        public static void ApplyWeek6Map01()
        {
            EnsureFolder("Assets/AfterBlue");
            EnsureFolder("Assets/AfterBlue/Scenes");
            EnsureFolder("Assets/AfterBlue/Scenes/Map_01");
            EnsureFolder("Assets/AfterBlue/Materials");
            EnsureFolder(DebugMaterialFolder);

            Material water = CreateMaterial($"{DebugMaterialFolder}/MAT_S2_Water_Base.mat", new Color(0.04f, 0.55f, 0.62f, 0.68f), true);
            Material shallow = CreateMaterial($"{DebugMaterialFolder}/MAT_S2_Depth_Shallow_H1.mat", new Color(0.35f, 0.96f, 0.72f, 0.36f), true);
            Material medium = CreateMaterial($"{DebugMaterialFolder}/MAT_S2_Depth_Medium.mat", new Color(0.05f, 0.70f, 0.76f, 0.32f), true);
            Material h2Zone = CreateMaterial($"{DebugMaterialFolder}/MAT_S2_Zone_H2_TealViolet.mat", new Color(0.15f, 0.52f, 0.96f, 0.34f), true);
            Material h3Zone = CreateMaterial($"{DebugMaterialFolder}/MAT_S2_Zone_H3_DeepViolet.mat", new Color(0.22f, 0.16f, 0.58f, 0.46f), true);
            Material startZone = CreateMaterial($"{DebugMaterialFolder}/MAT_S2_Zone_Start_Warm.mat", new Color(1f, 0.56f, 0.12f, 0.48f), true);
            Material returnZone = CreateMaterial($"{DebugMaterialFolder}/MAT_S2_Zone_Return.mat", new Color(0.23f, 0.68f, 0.92f, 0.28f), true);
            Material route = CreateMaterial($"{DebugMaterialFolder}/MAT_S2_Route_Main.mat", new Color(0.57f, 0.94f, 1f, 0.64f), true);
            Material boundary = CreateMaterial($"{DebugMaterialFolder}/MAT_S2_Boundary.mat", new Color(0.72f, 0.94f, 1f, 0.82f), true);
            Material road = CreateMaterial($"{DebugMaterialFolder}/MAT_S2_AsphaltProxy.mat", new Color(0.08f, 0.13f, 0.16f, 1f), false);
            Material concrete = CreateMaterial($"{DebugMaterialFolder}/MAT_S2_ConcreteProxy.mat", new Color(0.42f, 0.54f, 0.56f, 1f), false);
            Material roof = CreateMaterial($"{DebugMaterialFolder}/MAT_S2_RoofProxy.mat", new Color(0.11f, 0.24f, 0.27f, 1f), false);
            Material wood = CreateMaterial($"{DebugMaterialFolder}/MAT_S2_WoodProxy.mat", new Color(0.38f, 0.27f, 0.18f, 1f), false);
            Material rust = CreateMaterial($"{DebugMaterialFolder}/MAT_S2_RustMetalProxy.mat", new Color(0.56f, 0.25f, 0.12f, 1f), false);
            Material vegetation = CreateMaterial($"{DebugMaterialFolder}/MAT_S2_VegetationProxy.mat", new Color(0.20f, 0.43f, 0.22f, 1f), false);
            Material warmLight = CreateMaterial($"{DebugMaterialFolder}/MAT_S2_WarmLightProxy.mat", new Color(1f, 0.72f, 0.30f, 1f), false);
            Material ripple = CreateMaterial($"{DebugMaterialFolder}/MAT_S2_RippleLines.mat", new Color(0.74f, 1f, 1f, 0.55f), true);
            Material shadow = CreateMaterial($"{DebugMaterialFolder}/MAT_S2_SubmergedShadow.mat", new Color(0.02f, 0.07f, 0.09f, 0.52f), true);

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "Map_01_Week6";

            GameObject root = new GameObject("Map_01_Week6");
            Transform system = CreateChild(root.transform, "00_SYSTEM");
            Transform waterRoot = CreateChild(root.transform, "01_WATER");
            Transform mapGuides = CreateChild(root.transform, "02_MAP_GUIDES");
            Transform zones = CreateChild(root.transform, "03_ZONE_MARKERS");
            Transform routes = CreateChild(root.transform, "04_ROUTE_GUIDES");
            Transform obstacles = CreateChild(root.transform, "05_OBSTACLE_BLOCKOUT");
            Transform boundaryRoot = CreateChild(root.transform, "06_BOUNDARY");
            Transform landmarks = CreateChild(root.transform, "07_LANDMARKS");
            Transform fishingZones = CreateChild(root.transform, "08_FISHING_ZONES");
            Transform debug = CreateChild(root.transform, "99_DEBUG");

            CreateWaterAndDepthPatches(waterRoot, water, shallow, medium, h2Zone, h3Zone, startZone, returnZone);
            CreateGameplayAreaGuide(mapGuides, boundary);
            CreateBoundaryFrame(boundaryRoot, boundary);
            CreateRouteGuides(routes, route);
            CreateLargeZoneFields(zones, startZone, shallow, medium, h2Zone, h3Zone, returnZone);
            CreateFishingZoneFields(fishingZones, shallow, medium, h2Zone, h3Zone, returnZone);
            CreateWeek5StyleProxies(landmarks, obstacles, road, concrete, roof, wood, rust, vegetation, warmLight, shadow);
            CreateWaterLines(waterRoot, ripple);
            CreateSpeedAndCrossingMarkers(debug, route, boundary);
            CreateBoatAndCamera(system, warmLight);
            CreateNotes(system);
            CreateLighting(system);

            RenderSettings.ambientLight = new Color(0.55f, 0.68f, 0.70f, 1f);
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Linear;
            RenderSettings.fogColor = new Color(0.42f, 0.65f, 0.68f, 1f);
            RenderSettings.fogStartDistance = 70f;
            RenderSettings.fogEndDistance = 175f;

            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();
            Debug.Log($"Applied Week 6 Map_01. Scene: {ScenePath}");
        }

        private static void CreateWaterAndDepthPatches(Transform parent, Material water, Material shallow, Material medium, Material h2, Material h3, Material start, Material ret)
        {
            GameObject baseWater = CreateBox(parent, "Water_Base_280x192", new Vector3(0f, -0.05f, 0f), new Vector3(WaterWidth, 0.10f, WaterDepth), Quaternion.identity, water);
            DestroyCollider(baseWater);

            CreateFlatEllipse(parent, "Depth_Start_Warm_Shelf", Start, new Vector2(28f, 24f), start);
            CreateFlatEllipse(parent, "Depth_H1_Shallow_Mint_Field", H1, new Vector2(58f, 46f), shallow);
            CreateFlatEllipse(parent, "Depth_M_Central_Open_Water", M, new Vector2(68f, 46f), medium);
            CreateFlatEllipse(parent, "Depth_H2_Traffic_Light_Field", H2, new Vector2(56f, 44f), h2);
            CreateFlatEllipse(parent, "Depth_H3_Deep_Debris_Field", H3, new Vector2(66f, 54f), h3);
            CreateFlatEllipse(parent, "Depth_D_Return_Water_Field", D, new Vector2(42f, 34f), ret);
        }

        private static void CreateGameplayAreaGuide(Transform parent, Material material)
        {
            GameObject guide = CreateBox(parent, "GameplayArea_192x128", new Vector3(0f, -0.035f, 0f), new Vector3(GameplayWidth, 0.02f, GameplayDepth), Quaternion.identity, material);
            DestroyCollider(guide);
        }

        private static void CreateBoundaryFrame(Transform parent, Material material)
        {
            float halfWidth = GameplayWidth * 0.5f;
            float halfDepth = GameplayDepth * 0.5f;
            DestroyCollider(CreateBox(parent, "Boundary_North_192x128", new Vector3(0f, 0.18f, halfDepth), new Vector3(GameplayWidth, 0.12f, 0.18f), Quaternion.identity, material));
            DestroyCollider(CreateBox(parent, "Boundary_South_192x128", new Vector3(0f, 0.18f, -halfDepth), new Vector3(GameplayWidth, 0.12f, 0.18f), Quaternion.identity, material));
            DestroyCollider(CreateBox(parent, "Boundary_East_192x128", new Vector3(halfWidth, 0.18f, 0f), new Vector3(0.18f, 0.12f, GameplayDepth), Quaternion.identity, material));
            DestroyCollider(CreateBox(parent, "Boundary_West_192x128", new Vector3(-halfWidth, 0.18f, 0f), new Vector3(0.18f, 0.12f, GameplayDepth), Quaternion.identity, material));
        }

        private static void CreateRouteGuides(Transform parent, Material material)
        {
            CreateRoute(parent, "Route_S_to_A", Start, H1, material);
            CreateRoute(parent, "Route_S_to_D", Start, D, material);
            CreateRoute(parent, "Route_A_to_M", H1, M, material);
            CreateRoute(parent, "Route_A_to_B", H1, H2, material);
            CreateRoute(parent, "Route_M_to_B", M, H2, material);
            CreateRoute(parent, "Route_M_to_D", M, D, material);
            CreateRoute(parent, "Route_B_to_C", H2, H3, material);
            CreateRoute(parent, "Route_C_to_D", H3, D, material);
        }

        private static void CreateLargeZoneFields(Transform parent, Material start, Material h1, Material mid, Material h2, Material h3, Material ret)
        {
            CreateZoneCluster(parent, "ZONE_S_Start_Supply_Large", Start, new Vector2(18f, 16f), start);
            CreateZoneCluster(parent, "ZONE_H1_ShallowResidential_Large", H1, new Vector2(46f, 34f), h1);
            CreateZoneCluster(parent, "ZONE_M_CentralWater_Large", M, new Vector2(52f, 34f), mid);
            CreateZoneCluster(parent, "ZONE_H2_TrafficLight_Large", H2, new Vector2(44f, 34f), h2);
            CreateZoneCluster(parent, "ZONE_H3_DeepDebris_Large", H3, new Vector2(52f, 42f), h3);
            CreateZoneCluster(parent, "ZONE_D_ReturnWater_Large", D, new Vector2(34f, 26f), ret);
        }

        private static void CreateFishingZoneFields(Transform parent, Material h1, Material mid, Material h2, Material h3, Material ret)
        {
            CreateFlatEllipse(parent, "FZ_H1_ShallowVillage_Readable", H1 + new Vector3(4f, 0f, -3f), new Vector2(38f, 26f), h1);
            CreateFlatEllipse(parent, "FZ_M_CentralFreeWater_Readable", M + new Vector3(6f, 0f, -6f), new Vector2(42f, 28f), mid);
            CreateFlatEllipse(parent, "FZ_H2_Intersection_Readable", H2 + new Vector3(-3f, 0f, -4f), new Vector2(36f, 28f), h2);
            CreateFlatEllipse(parent, "FZ_H3_DeepDebris_Readable", H3 + new Vector3(-4f, 0f, 2f), new Vector2(42f, 34f), h3);
            CreateFlatEllipse(parent, "FZ_D_ReturnWater_Readable", D + new Vector3(5f, 0f, 2f), new Vector2(30f, 22f), ret);
        }

        private static void CreateWeek5StyleProxies(Transform landmarks, Transform obstacles, Material road, Material concrete, Material roof, Material wood, Material rust, Material vegetation, Material warmLight, Material shadow)
        {
            CreateStartSupplyCluster(landmarks, wood, rust, warmLight);
            CreateH1ResidentialCluster(landmarks, obstacles, roof, concrete, vegetation, shadow);
            CreateCentralRoadCluster(landmarks, obstacles, road, concrete, shadow);
            CreateH2TrafficLightCluster(landmarks, obstacles, road, concrete, rust, warmLight, shadow);
            CreateH3DeepDebrisCluster(landmarks, obstacles, rust, wood, vegetation, shadow);
            CreateReturnWaterCluster(landmarks, obstacles, roof, vegetation, shadow);
        }

        private static void CreateStartSupplyCluster(Transform parent, Material wood, Material rust, Material warmLight)
        {
            Transform root = CreateChild(parent, "Start_Supply_Buoy");
            root.localPosition = Start;

            CreateCylinder(root, "Supply_Buoy_Outer", new Vector3(0f, 0.15f, 0f), new Vector3(3.4f, 0.25f, 3.4f), Quaternion.identity, rust);
            CreateBox(root, "Supply_Platform", new Vector3(0f, 0.55f, 0f), new Vector3(4.4f, 0.32f, 3.6f), Quaternion.identity, wood);
            CreateCylinder(root, "Supply_Lantern_Tower", new Vector3(0f, 3.0f, 0f), new Vector3(0.35f, 2.6f, 0.35f), Quaternion.identity, rust);
            CreateCylinder(root, "Warm_Lantern", new Vector3(0f, 4.55f, 0f), new Vector3(0.55f, 0.35f, 0.55f), Quaternion.identity, warmLight);
            CreateBox(root, "Dock_Plank_A", new Vector3(6.4f, 0.08f, 0.4f), new Vector3(8.5f, 0.18f, 1.4f), Quaternion.Euler(0f, 8f, 0f), wood);
            CreateBox(root, "Dock_Plank_B", new Vector3(7.0f, 0.1f, -1.2f), new Vector3(7.2f, 0.18f, 1.0f), Quaternion.Euler(0f, -5f, 0f), wood);
        }

        private static void CreateH1ResidentialCluster(Transform landmarks, Transform obstacles, Material roof, Material concrete, Material vegetation, Material shadow)
        {
            Transform root = CreateChild(landmarks, "H1_ShallowResidential");
            root.localPosition = H1;

            Vector3[] roofPositions =
            {
                new Vector3(-15f, 0.12f, 7f),
                new Vector3(-4f, 0.10f, 10f),
                new Vector3(9f, 0.10f, 5f),
                new Vector3(-11f, 0.10f, -6f),
                new Vector3(4f, 0.10f, -8f),
                new Vector3(17f, 0.09f, -3f)
            };

            for (int i = 0; i < roofPositions.Length; i++)
            {
                bool usedAsset = InstantiateAsset(root, RoofModelPath, $"H1_Roof_Asset_{i + 1}", roofPositions[i], Quaternion.Euler(0f, i * 23f - 32f, 0f), Vector3.one * (0.95f + i % 3 * 0.08f));
                if (!usedAsset)
                {
                    CreateRoofProxy(root, $"H1_Roof_Proxy_{i + 1}", roofPositions[i], Quaternion.Euler(0f, i * 23f - 32f, 0f), new Vector3(7.5f + i % 2 * 2f, 0.22f, 4.5f), roof, concrete, vegetation);
                }
            }

            CreatePlantCluster(root, "H1_Seaweed_Cluster_A", new Vector3(-19f, 0.32f, -2f), 6, vegetation);
            CreatePlantCluster(root, "H1_Seaweed_Cluster_B", new Vector3(15f, 0.32f, 9f), 5, vegetation);
            CreateFlatEllipse(root, "H1_Submerged_Roof_Shadows", new Vector3(2f, -0.01f, 0f), new Vector2(44f, 28f), shadow);

            Transform block = CreateChild(obstacles, "H1_Light_Obstacle");
            block.localPosition = H1;
            CreateBox(block, "H1_Passable_Narrow_Clutter_A", new Vector3(-20f, 0.25f, 2f), new Vector3(6f, 0.4f, 3f), Quaternion.Euler(0f, 12f, 0f), concrete);
            CreateBox(block, "H1_Passable_Narrow_Clutter_B", new Vector3(18f, 0.25f, -7f), new Vector3(5f, 0.4f, 2.5f), Quaternion.Euler(0f, -22f, 0f), concrete);
        }

        private static void CreateCentralRoadCluster(Transform landmarks, Transform obstacles, Material road, Material concrete, Material shadow)
        {
            Transform root = CreateChild(landmarks, "M_Central_SubmergedRoad");
            root.localPosition = Vector3.zero;

            CreateRoadSegment(root, "Road_S_to_M_A", Mid(Start, M, 0.28f) + Vector3.up * 0.02f, Start, M, 26f, road, concrete);
            CreateRoadSegment(root, "Road_S_to_M_B", Mid(Start, M, 0.64f) + Vector3.up * 0.02f, Start, M, 25f, road, concrete);
            CreateRoadSegment(root, "Road_M_to_H2_A", Mid(M, H2, 0.35f) + Vector3.up * 0.02f, M, H2, 30f, road, concrete);
            CreateRoadSegment(root, "Road_M_to_H2_B", Mid(M, H2, 0.72f) + Vector3.up * 0.02f, M, H2, 24f, road, concrete);
            CreateFlatEllipse(root, "M_Road_Submerged_Shadow", M + new Vector3(4f, 0f, -4f), new Vector2(52f, 24f), shadow);

            Transform block = CreateChild(obstacles, "M_Central_Road_OpenPassages");
            CreateBox(block, "M_Road_Fragment_Visible_Not_Blocking_A", M + new Vector3(-18f, 0.18f, -10f), new Vector3(10f, 0.32f, 3f), Quaternion.Euler(0f, -18f, 0f), concrete);
            CreateBox(block, "M_Road_Fragment_Visible_Not_Blocking_B", M + new Vector3(22f, 0.18f, 10f), new Vector3(9f, 0.32f, 2.6f), Quaternion.Euler(0f, 16f, 0f), concrete);
        }

        private static void CreateH2TrafficLightCluster(Transform landmarks, Transform obstacles, Material road, Material concrete, Material rust, Material warmLight, Material shadow)
        {
            Transform root = CreateChild(landmarks, "H2_TrafficLight");
            root.localPosition = H2;

            CreateBox(root, "H2_Intersection_Main_Road", new Vector3(0f, 0.03f, 0f), new Vector3(42f, 0.10f, 9f), Quaternion.Euler(0f, -10f, 0f), road);
            CreateBox(root, "H2_Intersection_Cross_Road", new Vector3(1f, 0.035f, 0f), new Vector3(9f, 0.10f, 34f), Quaternion.Euler(0f, -10f, 0f), road);
            for (int i = -2; i <= 2; i++)
            {
                CreateBox(root, $"H2_Crosswalk_Stripe_{i + 3}", new Vector3(i * 2.2f, 0.12f, -6.4f), new Vector3(1.1f, 0.04f, 7f), Quaternion.Euler(0f, -10f, 0f), concrete);
            }

            CreateTrafficLightProxy(root, "H2_Tilted_TrafficLight_Hero", new Vector3(2.5f, 1.7f, 0.5f), Quaternion.Euler(0f, 18f, -18f), rust, warmLight);
            InstantiateAsset(root, UtilityPoleModelPath, "H2_Rusted_UtilityPole_Asset_A", new Vector3(-13f, 1.2f, 8f), Quaternion.Euler(0f, -24f, -9f), Vector3.one * 1.15f);
            InstantiateAsset(root, UtilityPoleModelPath, "H2_Rusted_UtilityPole_Asset_B", new Vector3(16f, 1.2f, -10f), Quaternion.Euler(0f, 28f, 12f), Vector3.one);
            CreateBox(root, "H2_Road_Sign_Proxy", new Vector3(12f, 1.15f, 6f), new Vector3(2.8f, 1.0f, 0.12f), Quaternion.Euler(0f, -24f, 0f), rust);
            CreateFlatEllipse(root, "H2_Deepening_Water_Shadow", new Vector3(2f, -0.01f, -2f), new Vector2(46f, 32f), shadow);

            Transform block = CreateChild(obstacles, "H2_Obstacle");
            block.localPosition = H2;
            CreateBox(block, "H2_Broken_Road_Blocker_North", new Vector3(-18f, 0.25f, 10f), new Vector3(8f, 0.5f, 4f), Quaternion.Euler(0f, 18f, 0f), concrete);
            CreateBox(block, "H2_Broken_Road_Blocker_South", new Vector3(16f, 0.25f, -12f), new Vector3(9f, 0.5f, 4.5f), Quaternion.Euler(0f, -14f, 0f), concrete);
        }

        private static void CreateH3DeepDebrisCluster(Transform landmarks, Transform obstacles, Material rust, Material wood, Material vegetation, Material shadow)
        {
            Transform root = CreateChild(landmarks, "H3_DeepDebris");
            root.localPosition = H3;

            CreateFlatEllipse(root, "H3_Dark_Depth_Shadow_Core", new Vector3(0f, -0.02f, 0f), new Vector2(58f, 42f), shadow);
            for (int i = 0; i < 8; i++)
            {
                float angle = i * 46f;
                Vector3 position = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * (10f + i % 3 * 5f), 0.35f, Mathf.Sin(angle * Mathf.Deg2Rad) * (8f + i % 2 * 6f));
                CreateBox(root, $"H3_Metal_Frame_Debris_{i + 1}", position, new Vector3(9f + i % 2 * 4f, 0.28f, 0.55f), Quaternion.Euler(0f, angle, 0f), rust);
                CreateBox(root, $"H3_Wood_Plank_Debris_{i + 1}", position + new Vector3(1.2f, 0.2f, -1.3f), new Vector3(5f, 0.20f, 0.45f), Quaternion.Euler(0f, angle + 28f, 0f), wood);
            }

            CreatePlantCluster(root, "H3_Long_Seaweed_Cluster_A", new Vector3(-12f, 0.5f, 11f), 7, vegetation);
            CreatePlantCluster(root, "H3_Long_Seaweed_Cluster_B", new Vector3(17f, 0.5f, -9f), 8, vegetation);

            Transform block = CreateChild(obstacles, "H3_Obstacle");
            block.localPosition = H3;
            CreateBox(block, "H3_Hero_Wreck_Silhouette", new Vector3(3f, 0.28f, -1f), new Vector3(24f, 0.45f, 8f), Quaternion.Euler(0f, -22f, 0f), rust);
            CreateBox(block, "H3_Not_Passable_Debris_Core", new Vector3(-8f, 0.38f, 9f), new Vector3(12f, 0.55f, 5f), Quaternion.Euler(0f, 35f, 0f), rust);
        }

        private static void CreateReturnWaterCluster(Transform landmarks, Transform obstacles, Material roof, Material vegetation, Material shadow)
        {
            Transform root = CreateChild(landmarks, "D_ReturnWater");
            root.localPosition = D;

            CreateFlatEllipse(root, "D_Return_Water_Shadow", Vector3.zero, new Vector2(32f, 22f), shadow);
            CreateRoofProxy(root, "D_Low_Roof_Guide_A", new Vector3(-7f, 0.1f, 5f), Quaternion.Euler(0f, 30f, 0f), new Vector3(7f, 0.18f, 3.8f), roof, roof, vegetation);
            CreatePlantCluster(root, "D_Seaweed_Return_Cue", new Vector3(8f, 0.32f, -5f), 6, vegetation);

            Transform block = CreateChild(obstacles, "D_Light_Obstacle");
            block.localPosition = D;
            CreateBox(block, "D_Return_Debris_Not_Blocking", new Vector3(10f, 0.22f, 7f), new Vector3(7f, 0.35f, 3f), Quaternion.Euler(0f, -12f, 0f), roof);
        }

        private static void CreateWaterLines(Transform parent, Material material)
        {
            CreateRing(parent, "Ripple_Start_Supply", Start + new Vector3(2.5f, 0.04f, 2f), 5f, material);
            CreateRing(parent, "Ripple_H1_Fishing_Water", H1 + new Vector3(8f, 0.04f, -5f), 7f, material);
            CreateRing(parent, "Ripple_M_Open_Water", M + new Vector3(5f, 0.04f, 2f), 9f, material);
            CreateRing(parent, "Ripple_H2_Bobber", H2 + new Vector3(-5f, 0.04f, -4f), 6f, material);
            CreateRing(parent, "Ripple_H3_Deep_Water", H3 + new Vector3(-7f, 0.04f, 4f), 8f, material);

            CreateFlowArrow(parent, "Flow_Start_to_H1", Mid(Start, H1, 0.52f), Start, H1, material);
            CreateFlowArrow(parent, "Flow_M_to_H2", Mid(M, H2, 0.52f), M, H2, material);
            CreateFlowArrow(parent, "Flow_H2_to_H3", Mid(H2, H3, 0.50f), H2, H3, material);
        }

        private static void CreateSpeedAndCrossingMarkers(Transform parent, Material route, Material boundary)
        {
            DestroyCollider(CreateBox(parent, "SpeedMarker_0m", new Vector3(0f, 1f, -10f), new Vector3(0.2f, 2f, 0.2f), Quaternion.identity, route));
            DestroyCollider(CreateBox(parent, "SpeedMarker_10m", new Vector3(0f, 1f, 0f), new Vector3(0.2f, 2f, 0.2f), Quaternion.identity, route));
            DestroyCollider(CreateBox(parent, "SpeedMarker_20m", new Vector3(0f, 1f, 10f), new Vector3(0.2f, 2f, 0.2f), Quaternion.identity, route));

            float halfWidth = GameplayWidth * 0.5f;
            float halfDepth = GameplayDepth * 0.5f;
            DestroyCollider(CreateBox(parent, "CrossingMarker_WestEdge", new Vector3(-halfWidth, 1.25f, 0f), new Vector3(0.5f, 2.5f, 4f), Quaternion.identity, boundary));
            DestroyCollider(CreateBox(parent, "CrossingMarker_EastEdge", new Vector3(halfWidth, 1.25f, 0f), new Vector3(0.5f, 2.5f, 4f), Quaternion.identity, boundary));
            DestroyCollider(CreateBox(parent, "CrossingMarker_SouthEdge", new Vector3(0f, 1.25f, -halfDepth), new Vector3(4f, 2.5f, 0.5f), Quaternion.identity, boundary));
            DestroyCollider(CreateBox(parent, "CrossingMarker_NorthEdge", new Vector3(0f, 1.25f, halfDepth), new Vector3(4f, 2.5f, 0.5f), Quaternion.identity, boundary));
        }

        private static void CreateBoatAndCamera(Transform parent, Material fallbackMaterial)
        {
            GameObject boat = new GameObject("PlayerBoat");
            boat.transform.SetParent(parent, false);
            boat.transform.localPosition = Start + new Vector3(0f, 0.18f, -4.4f);
            boat.transform.localRotation = Quaternion.Euler(0f, 22f, 0f);

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
                CreateBox(boat.transform, "Boat_Fallback_4p5m", Vector3.zero, new Vector3(1.8f, 0.6f, 4.5f), Quaternion.identity, fallbackMaterial);
            }

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
            camera.fieldOfView = 54f;

            FollowCamera follow = cameraObject.AddComponent<FollowCamera>();
            follow.SetTarget(boat.transform);
            SerializedObject followObject = new SerializedObject(follow);
            followObject.FindProperty("offset").vector3Value = new Vector3(0f, 9f, -15f);
            followObject.FindProperty("followSmoothTime").floatValue = 0.28f;
            followObject.FindProperty("lookTargetSmoothTime").floatValue = 0.16f;
            followObject.FindProperty("rotationSharpness").floatValue = 6f;
            followObject.FindProperty("lookAtHeight").floatValue = 0.55f;
            followObject.FindProperty("lookAheadDistance").floatValue = 15f;
            followObject.FindProperty("useTargetLocalOffset").boolValue = true;
            followObject.ApplyModifiedPropertiesWithoutUndo();

            cameraObject.transform.position = boat.transform.position + boat.transform.TransformDirection(new Vector3(0f, 9f, -15f));
            cameraObject.transform.LookAt(boat.transform.position + Vector3.up * 0.55f + boat.transform.forward * 15f);
        }

        private static void CreateLighting(Transform parent)
        {
            GameObject lightObject = new GameObject("Directional Light");
            lightObject.transform.SetParent(parent, false);
            Light light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 0.82f;
            light.color = new Color(0.84f, 0.96f, 1f, 1f);
            lightObject.transform.rotation = Quaternion.Euler(45f, -35f, 0f);
        }

        private static void CreateNotes(Transform parent)
        {
            GameObject notes = new GameObject("WEEK6_GOAL_MapSize_Movement_ZoneRead");
            notes.transform.SetParent(parent, false);
            notes.transform.localPosition = Vector3.zero;

            GameObject spec = new GameObject("P1_TEST_192x128_Boat7p2ms_ZoneScale");
            spec.transform.SetParent(parent, false);
            spec.transform.localPosition = new Vector3(0f, 0f, 2f);
        }

        private static void CreateZoneCluster(Transform parent, string name, Vector3 center, Vector2 size, Material material)
        {
            Transform root = CreateChild(parent, name);
            root.localPosition = center;
            CreateFlatEllipse(root, $"{name}_Main", Vector3.zero, size, material);
            CreateFlatEllipse(root, $"{name}_Offset_A", new Vector3(size.x * 0.18f, 0f, size.y * 0.12f), size * 0.56f, material);
            CreateFlatEllipse(root, $"{name}_Offset_B", new Vector3(-size.x * 0.18f, 0f, -size.y * 0.10f), size * 0.48f, material);
        }

        private static void CreateRoadSegment(Transform parent, string name, Vector3 position, Vector3 start, Vector3 end, float length, Material asphalt, Material concrete)
        {
            Vector3 direction = (end - start).normalized;
            Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
            CreateBox(parent, name, position, new Vector3(8.8f, 0.10f, length), rotation, asphalt);
            CreateBox(parent, $"{name}_LaneMark_Left", position + rotation * new Vector3(-1.5f, 0.08f, 0f), new Vector3(0.18f, 0.035f, length * 0.75f), rotation, concrete);
            CreateBox(parent, $"{name}_LaneMark_Right", position + rotation * new Vector3(1.5f, 0.08f, 0f), new Vector3(0.18f, 0.035f, length * 0.72f), rotation, concrete);
        }

        private static void CreateRoofProxy(Transform parent, string name, Vector3 position, Quaternion rotation, Vector3 scale, Material roof, Material concrete, Material vegetation)
        {
            Transform root = CreateChild(parent, name);
            root.localPosition = position;
            root.localRotation = rotation;
            CreateBox(root, "Wet_Roof_Slab", Vector3.zero, scale, Quaternion.identity, roof);
            CreateBox(root, "Broken_Roof_Hole", new Vector3(-scale.x * 0.18f, 0.12f, scale.z * 0.04f), new Vector3(scale.x * 0.24f, 0.05f, scale.z * 0.32f), Quaternion.Euler(0f, 12f, 0f), concrete);
            CreateBox(root, "Moss_Ridge", new Vector3(scale.x * 0.18f, 0.16f, -scale.z * 0.22f), new Vector3(scale.x * 0.34f, 0.06f, scale.z * 0.10f), Quaternion.identity, vegetation);
        }

        private static void CreateTrafficLightProxy(Transform parent, string name, Vector3 position, Quaternion rotation, Material rust, Material warmLight)
        {
            Transform root = CreateChild(parent, name);
            root.localPosition = position;
            root.localRotation = rotation;
            CreateCylinder(root, "Bent_Signal_Pole", new Vector3(0f, 0.1f, 0f), new Vector3(0.12f, 3.2f, 0.12f), Quaternion.identity, rust);
            CreateBox(root, "Signal_Housing", new Vector3(0f, 3.0f, 0f), new Vector3(0.72f, 1.35f, 0.28f), Quaternion.identity, rust);
            CreateCylinder(root, "Signal_Red", new Vector3(0f, 3.38f, -0.18f), new Vector3(0.22f, 0.05f, 0.22f), Quaternion.Euler(90f, 0f, 0f), warmLight);
            CreateCylinder(root, "Signal_Amber", new Vector3(0f, 3.00f, -0.18f), new Vector3(0.18f, 0.05f, 0.18f), Quaternion.Euler(90f, 0f, 0f), rust);
            CreateCylinder(root, "Signal_Green", new Vector3(0f, 2.62f, -0.18f), new Vector3(0.18f, 0.05f, 0.18f), Quaternion.Euler(90f, 0f, 0f), rust);
        }

        private static void CreatePlantCluster(Transform parent, string name, Vector3 position, int count, Material material)
        {
            Transform root = CreateChild(parent, name);
            root.localPosition = position;
            for (int i = 0; i < count; i++)
            {
                float angle = i * 137.5f;
                float radius = 0.8f + (i % 4) * 0.45f;
                Vector3 local = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * radius, 0f, Mathf.Sin(angle * Mathf.Deg2Rad) * radius);
                CreateBox(root, $"Plant_Blade_{i + 1}", local + Vector3.up * (0.25f + i % 3 * 0.08f), new Vector3(0.16f, 0.7f + i % 3 * 0.25f, 0.16f), Quaternion.Euler(0f, angle, 12f - i % 5 * 5f), material);
            }
        }

        private static void CreateRoute(Transform parent, string name, Vector3 start, Vector3 end, Material material)
        {
            Vector3 midpoint = (start + end) * 0.5f + Vector3.up * 0.06f;
            Vector3 direction = end - start;
            GameObject route = CreateBox(parent, name, midpoint, new Vector3(0.32f, 0.04f, direction.magnitude), Quaternion.LookRotation(direction.normalized, Vector3.up), material);
            DestroyCollider(route);
        }

        private static void CreateFlatEllipse(Transform parent, string name, Vector3 position, Vector2 diameter, Material material)
        {
            GameObject ellipse = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            ellipse.name = name;
            ellipse.transform.SetParent(parent, false);
            ellipse.transform.localPosition = position + new Vector3(0f, 0.012f, 0f);
            ellipse.transform.localScale = new Vector3(diameter.x * 0.5f, 0.018f, diameter.y * 0.5f);
            ellipse.GetComponent<Renderer>().sharedMaterial = material;
            DestroyCollider(ellipse);
        }

        private static void CreateRing(Transform parent, string name, Vector3 center, float radius, Material material)
        {
            GameObject ring = new GameObject(name);
            ring.transform.SetParent(parent, false);
            ring.transform.localPosition = Vector3.zero;
            LineRenderer line = ring.AddComponent<LineRenderer>();
            line.sharedMaterial = material;
            line.useWorldSpace = false;
            line.loop = true;
            line.widthMultiplier = 0.18f;
            line.positionCount = 48;
            for (int i = 0; i < line.positionCount; i++)
            {
                float angle = i / (float)line.positionCount * Mathf.PI * 2f;
                line.SetPosition(i, center + new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius));
            }
        }

        private static void CreateFlowArrow(Transform parent, string name, Vector3 position, Vector3 start, Vector3 end, Material material)
        {
            Vector3 direction = (end - start).normalized;
            Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
            CreateBox(parent, $"{name}_Stem", position + Vector3.up * 0.08f, new Vector3(0.26f, 0.04f, 7.0f), rotation, material);
            CreateBox(parent, $"{name}_Head_L", position + rotation * new Vector3(-1.1f, 0.1f, 3.2f), new Vector3(0.24f, 0.04f, 2.8f), rotation * Quaternion.Euler(0f, -32f, 0f), material);
            CreateBox(parent, $"{name}_Head_R", position + rotation * new Vector3(1.1f, 0.1f, 3.2f), new Vector3(0.24f, 0.04f, 2.8f), rotation * Quaternion.Euler(0f, 32f, 0f), material);
        }

        private static bool InstantiateAsset(Transform parent, string path, string name, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (asset == null)
            {
                return false;
            }

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(asset);
            instance.name = name;
            instance.transform.SetParent(parent, false);
            instance.transform.localPosition = position;
            instance.transform.localRotation = rotation;
            instance.transform.localScale = scale;
            return true;
        }

        private static GameObject CreateBox(Transform parent, string name, Vector3 position, Vector3 scale, Quaternion rotation, Material material)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.name = name;
            obj.transform.SetParent(parent, false);
            obj.transform.localPosition = position;
            obj.transform.localScale = scale;
            obj.transform.localRotation = rotation;
            obj.GetComponent<Renderer>().sharedMaterial = material;
            return obj;
        }

        private static GameObject CreateCylinder(Transform parent, string name, Vector3 position, Vector3 scale, Quaternion rotation, Material material)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            obj.name = name;
            obj.transform.SetParent(parent, false);
            obj.transform.localPosition = position;
            obj.transform.localScale = scale;
            obj.transform.localRotation = rotation;
            obj.GetComponent<Renderer>().sharedMaterial = material;
            return obj;
        }

        private static Transform CreateChild(Transform parent, string name)
        {
            GameObject child = new GameObject(name);
            child.transform.SetParent(parent, false);
            return child.transform;
        }

        private static void DestroyCollider(GameObject obj)
        {
            Collider collider = obj.GetComponent<Collider>();
            if (collider != null)
            {
                Object.DestroyImmediate(collider);
            }
        }

        private static Material CreateMaterial(string path, Color color, bool transparent)
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
            else
            {
                if (material.HasProperty("_Surface"))
                {
                    material.SetFloat("_Surface", 0f);
                }

                material.SetInt("_ZWrite", 1);
                material.renderQueue = -1;
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

        private static Vector3 ExpandNode(float x, float z)
        {
            return new Vector3(x * NodeExpansion, 0f, z * NodeExpansion);
        }

        private static Vector3 Mid(Vector3 start, Vector3 end, float t)
        {
            return Vector3.Lerp(start, end, t);
        }
    }
}
