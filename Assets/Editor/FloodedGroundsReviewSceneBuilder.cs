using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AfterBlue.EditorTools
{
    public static class FloodedGroundsReviewSceneBuilder
    {
        private const string ScenePath = "Assets/AfterBlue/Scenes/AssetLab/FloodedGrounds_Review.unity";
        private const string TriggerPath = "Temp/AfterBlueRunFloodedGroundsReview.once";
        private const string FloodedGroundsRoot = "Assets/Flooded_Grounds";
        private const string WaterPrefabPath = "Assets/IgniteCoders/Simple Water Shader/Prefabs/WaterBlock_50m.prefab";
        private const string WaterMaterialPath = "Assets/AfterBlue/Materials/Week7/AB_Water_Map01_Candidate.mat";
        private const string FallbackWaterMaterialPath = "Assets/AfterBlue/Materials/AssetLab/MAT_Review_WaterFallback.mat";
        private const string GroundMaterialPath = "Assets/AfterBlue/Materials/AssetLab/MAT_Review_Ground.mat";
        private const string LabelMaterialPath = "Assets/AfterBlue/Materials/AssetLab/MAT_Review_Label.mat";

        private static readonly ReviewAsset[] RoadAssets =
        {
            new("Pavement Corner", "Assets/Flooded_Grounds/Prefabs/Buildings/Structures1/Pavement1_Cor_A.prefab"),
            new("Pavement Side", "Assets/Flooded_Grounds/Prefabs/Buildings/Structures1/Pavement1_Side_A.prefab"),
            new("Pavement Mid", "Assets/Flooded_Grounds/Prefabs/Buildings/Structures1/Pavement2_Mid_A.prefab"),
            new("Pavement End", "Assets/Flooded_Grounds/Prefabs/Buildings/Structures1/Pavement2_End_A.prefab"),
            new("Roundabout", "Assets/Flooded_Grounds/Prefabs/Buildings/Structures1/Struct_Roundabout_A.prefab"),
            new("Wood Path", "Assets/Flooded_Grounds/Prefabs/Buildings/Structures1/Struct_WoodPath_A.prefab"),
        };

        private static readonly ReviewAsset[] BuildingAssets =
        {
            new("Cabin", "Assets/Flooded_Grounds/Prefabs/Buildings/Cabins/Cabin1.prefab"),
            new("Villa Base", "Assets/Flooded_Grounds/Prefabs/Buildings/Villa1/Villa1_Base_Mid_A.prefab"),
            new("Villa Roof", "Assets/Flooded_Grounds/Prefabs/Buildings/Villa1/Villa1_Roof_Mid_A.prefab"),
            new("Bridge", "Assets/Flooded_Grounds/Prefabs/Buildings/Bridge/BLD_Bridge_A.prefab"),
            new("Docking", "Assets/Flooded_Grounds/Prefabs/Buildings/Structures1/Struct_Docking_A.prefab"),
            new("Lighthouse", "Assets/Flooded_Grounds/Prefabs/Buildings/LightHouse/LightHouse_A.prefab"),
        };

        private static readonly ReviewAsset[] PropAssets =
        {
            new("Car", "Assets/Flooded_Grounds/Prefabs/Props/Prop_Car_A.prefab"),
            new("Damaged Car", "Assets/Flooded_Grounds/Prefabs/Props/Prop_Car1_DM.prefab"),
            new("Boat", "Assets/Flooded_Grounds/Prefabs/Props/Prop_Boat_A.prefab"),
            new("Ship", "Assets/Flooded_Grounds/Prefabs/Props/Prop_Ship_A.prefab"),
            new("Lamp A", "Assets/Flooded_Grounds/Prefabs/Props/Prop_Lamp_A.prefab"),
            new("Lamp C", "Assets/Flooded_Grounds/Prefabs/Props/Prop_Lamp_C.prefab"),
            new("Fence", "Assets/Flooded_Grounds/Prefabs/Buildings/Structures1/Struct_Fence1_Mid_A.prefab"),
            new("Billboard", "Assets/Flooded_Grounds/Prefabs/Buildings/Structures1/Struct_BillBoard_A.prefab"),
            new("Pole", "Assets/Flooded_Grounds/Prefabs/Buildings/Structures1/Struct_Pole_A.prefab"),
            new("Flood Wall", "Assets/Flooded_Grounds/Prefabs/Buildings/Structures1/Struct_FloodWall_A.prefab"),
        };

        private static readonly ReviewAsset[] NatureAssets =
        {
            new("Bush A", "Assets/Flooded_Grounds/Prefabs/Nature/Bushes/DecoBush_A.prefab"),
            new("Bush B", "Assets/Flooded_Grounds/Prefabs/Nature/Bushes/DecoBush_B.prefab"),
            new("Rock A", "Assets/Flooded_Grounds/Prefabs/Nature/Rocks/Rock_A.prefab"),
            new("Rock B", "Assets/Flooded_Grounds/Prefabs/Nature/Rocks/Rock_B.prefab"),
            new("Cobble A", "Assets/Flooded_Grounds/Prefabs/Nature/Rocks/CobbleRock_A.prefab"),
            new("Tree Bush", "Assets/Flooded_Grounds/Prefabs/Nature/Trees/TreeCreator_Bush_A.prefab"),
        };

        [InitializeOnLoadMethod]
        private static void RunWhenTriggered()
        {
            if (!File.Exists(TriggerPath))
            {
                return;
            }

            File.Delete(TriggerPath);
            EditorApplication.delayCall += CreateReviewScene;
        }

        [MenuItem("AfterBlue/Setup/Create Flooded Grounds Review Scene")]
        public static void CreateReviewScene()
        {
            if (!AssetDatabase.IsValidFolder(FloodedGroundsRoot))
            {
                EditorUtility.DisplayDialog("AfterBlue", "Flooded Grounds is not imported yet.", "OK");
                return;
            }

            EnsureFolder("Assets/AfterBlue");
            EnsureFolder("Assets/AfterBlue/Scenes");
            EnsureFolder("Assets/AfterBlue/Scenes/AssetLab");
            EnsureFolder("Assets/AfterBlue/Materials");
            EnsureFolder("Assets/AfterBlue/Materials/AssetLab");

            Material water = LoadOrCreateWaterMaterial();
            Material ground = CreateMaterial(GroundMaterialPath, new Color(0.10f, 0.14f, 0.14f, 1f), false);
            Material label = CreateMaterial(LabelMaterialPath, new Color(0.82f, 0.95f, 1f, 1f), false);

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "FloodedGrounds_Review";

            GameObject root = new("FloodedGrounds_Review");
            Transform system = CreateChild(root.transform, "00_SYSTEM");
            Transform waterRoot = CreateChild(root.transform, "01_WATER");
            Transform diorama = CreateChild(root.transform, "02_H2_TRAFFIC_INTERSECTION_DIORAMA");
            Transform catalog = CreateChild(root.transform, "03_ASSET_CATALOG");
            Transform notes = CreateChild(root.transform, "99_NOTES");

            CreateWaterStage(waterRoot, water);
            DestroyCollider(CreateBox(waterRoot, "Review_GroundPlane_180x120", new Vector3(0f, -1.05f, 0f), new Vector3(180f, 0.12f, 120f), Quaternion.identity, ground));

            CreateDiorama(diorama);
            CreateCatalogRow(catalog, "Roads And Pavement", RoadAssets, new Vector3(-72f, 0f, 42f), label);
            CreateCatalogRow(catalog, "Buildings And Structures", BuildingAssets, new Vector3(-72f, 0f, 24f), label);
            CreateCatalogRow(catalog, "Props And Remnants", PropAssets, new Vector3(-72f, 0f, 6f), label);
            CreateCatalogRow(catalog, "Nature And Ground Detail", NatureAssets, new Vector3(-72f, 0f, -12f), label);

            CreateLabel(notes, "Source demo scenes: Assets/Flooded_Grounds/Scenes/Scene_A.unity and PreAsembeld_Buildings.unity", new Vector3(-72f, 3f, -46f), label, 2.2f);
            CreateLabel(notes, "Local review only. Flooded_Grounds is ignored by git; convert selected materials before Map_01 use.", new Vector3(-72f, 1.2f, -46f), label, 2.0f);

            CreateCamera(system);
            CreateLighting(system);

            RenderSettings.ambientLight = new Color(0.50f, 0.62f, 0.64f, 1f);
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Linear;
            RenderSettings.fogColor = new Color(0.42f, 0.66f, 0.70f, 1f);
            RenderSettings.fogStartDistance = 80f;
            RenderSettings.fogEndDistance = 240f;

            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();
            Debug.Log($"Created Flooded Grounds review scene: {ScenePath}");
        }

        private static void CreateDiorama(Transform parent)
        {
            CreateLabel(parent, "Small flooded intersection composition", new Vector3(-28f, 3.2f, -24f), AssetDatabase.LoadAssetAtPath<Material>(LabelMaterialPath), 2.0f);

            InstantiateAsset(parent, RoadAssets[0].Path, "Diorama_Pavement_Corner_A", new Vector3(-8f, -0.75f, -2f), Quaternion.Euler(0f, 20f, 0f), Vector3.one * 2.1f);
            InstantiateAsset(parent, RoadAssets[2].Path, "Diorama_Pavement_Mid_A", new Vector3(6f, -0.78f, 4f), Quaternion.Euler(0f, 20f, 0f), Vector3.one * 2.2f);
            InstantiateAsset(parent, RoadAssets[2].Path, "Diorama_Pavement_Mid_B", new Vector3(-18f, -0.82f, 11f), Quaternion.Euler(0f, 95f, 0f), Vector3.one * 1.8f);
            InstantiateAsset(parent, "Assets/Flooded_Grounds/Prefabs/Buildings/Structures1/Struct_Pole_A.prefab", "Diorama_Pole_A", new Vector3(-3f, -0.10f, 10f), Quaternion.Euler(0f, -25f, 0f), Vector3.one * 1.7f);
            InstantiateAsset(parent, "Assets/Flooded_Grounds/Prefabs/Props/Prop_Lamp_C.prefab", "Diorama_Lamp_C", new Vector3(7f, -0.20f, 13f), Quaternion.Euler(0f, 45f, 0f), Vector3.one * 1.5f);
            InstantiateAsset(parent, "Assets/Flooded_Grounds/Prefabs/Props/Prop_Car_A.prefab", "Diorama_Submerged_Car_A", new Vector3(-21f, -0.72f, -7f), Quaternion.Euler(0f, -32f, 0f), Vector3.one * 1.7f);
            InstantiateAsset(parent, "Assets/Flooded_Grounds/Prefabs/Buildings/Structures1/Struct_Fence1_Mid_A.prefab", "Diorama_Broken_Fence_A", new Vector3(21f, -0.55f, -8f), Quaternion.Euler(0f, 18f, 0f), Vector3.one * 1.7f);
            InstantiateAsset(parent, "Assets/Flooded_Grounds/Prefabs/Buildings/Cabins/Cabin1.prefab", "Diorama_Background_Cabin_A", new Vector3(-33f, -0.35f, 24f), Quaternion.Euler(0f, 18f, 0f), Vector3.one * 1.7f);
            InstantiateAsset(parent, "Assets/Flooded_Grounds/Prefabs/Buildings/Villa1/Villa1_Roof_Mid_A.prefab", "Diorama_Partial_Roof_A", new Vector3(28f, -0.48f, 20f), Quaternion.Euler(0f, -18f, 0f), Vector3.one * 1.8f);
            InstantiateAsset(parent, "Assets/Flooded_Grounds/Prefabs/Nature/Bushes/DecoBush_A.prefab", "Diorama_Overgrowth_A", new Vector3(-8f, -0.40f, 16f), Quaternion.identity, Vector3.one * 1.8f);
            InstantiateAsset(parent, "Assets/Flooded_Grounds/Prefabs/Nature/Rocks/Rock_A.prefab", "Diorama_Rock_A", new Vector3(16f, -0.60f, 5f), Quaternion.Euler(0f, 12f, 0f), Vector3.one * 1.5f);
        }

        private static void CreateCatalogRow(Transform parent, string title, IReadOnlyList<ReviewAsset> assets, Vector3 origin, Material labelMaterial)
        {
            Transform row = CreateChild(parent, title.Replace(" ", "_"));
            CreateLabel(row, title, origin + new Vector3(0f, 3f, -3.5f), labelMaterial, 1.8f);

            for (int i = 0; i < assets.Count; i++)
            {
                ReviewAsset asset = assets[i];
                Vector3 position = origin + new Vector3(i * 14f, -0.45f, 0f);
                InstantiateAsset(row, asset.Path, asset.Label, position, Quaternion.Euler(0f, 25f, 0f), Vector3.one * 1.25f);
                CreateLabel(row, asset.Label, position + new Vector3(-4f, 1.7f, -4f), labelMaterial, 0.85f);
            }
        }

        private static void CreateWaterStage(Transform parent, Material water)
        {
            GameObject waterPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(WaterPrefabPath);
            if (waterPrefab != null)
            {
                GameObject waterObject = (GameObject)PrefabUtility.InstantiatePrefab(waterPrefab);
                waterObject.name = "Review_Water_IgniteCoders_180x120";
                waterObject.transform.SetParent(parent, false);
                waterObject.transform.localPosition = Vector3.zero;
                waterObject.transform.localScale = new Vector3(180f / 50f, 1f, 120f / 50f);
                foreach (Renderer renderer in waterObject.GetComponentsInChildren<Renderer>())
                {
                    renderer.sharedMaterial = water;
                }
                DestroyCollidersRecursively(waterObject);
                return;
            }

            DestroyCollider(CreateBox(parent, "Review_Water_Fallback_180x120", Vector3.zero, new Vector3(180f, 0.08f, 120f), Quaternion.identity, water));
        }

        private static bool InstantiateAsset(Transform parent, string path, string name, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null)
            {
                Debug.LogWarning($"Review asset missing: {path}");
                return false;
            }

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            instance.name = name;
            instance.transform.SetParent(parent, false);
            instance.transform.localPosition = position;
            instance.transform.localRotation = rotation;
            instance.transform.localScale = scale;
            DestroyCollidersRecursively(instance);
            return true;
        }

        private static void CreateCamera(Transform parent)
        {
            GameObject cameraObject = new("Main Camera");
            cameraObject.transform.SetParent(parent, false);
            cameraObject.transform.position = new Vector3(0f, 62f, -82f);
            cameraObject.transform.rotation = Quaternion.Euler(58f, 0f, 0f);
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.tag = "MainCamera";
            camera.fieldOfView = 48f;
        }

        private static void CreateLighting(Transform parent)
        {
            GameObject lightObject = new("Directional Light");
            lightObject.transform.SetParent(parent, false);
            Light light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 0.92f;
            light.color = new Color(0.83f, 0.96f, 1f, 1f);
            lightObject.transform.rotation = Quaternion.Euler(45f, -34f, 0f);
        }

        private static void CreateLabel(Transform parent, string text, Vector3 position, Material material, float size)
        {
            GameObject labelObject = new($"Label_{SanitizeName(text)}");
            labelObject.transform.SetParent(parent, false);
            labelObject.transform.localPosition = position;
            labelObject.transform.localRotation = Quaternion.Euler(65f, 0f, 0f);
            TextMesh textMesh = labelObject.AddComponent<TextMesh>();
            textMesh.text = text;
            textMesh.fontSize = 32;
            textMesh.characterSize = size * 0.12f;
            textMesh.anchor = TextAnchor.MiddleLeft;
            textMesh.alignment = TextAlignment.Left;
            Renderer renderer = labelObject.GetComponent<Renderer>();
            renderer.sharedMaterial = material;
        }

        private static Material LoadOrCreateWaterMaterial()
        {
            Material material = AssetDatabase.LoadAssetAtPath<Material>(WaterMaterialPath);
            if (material != null)
            {
                return material;
            }

            return CreateMaterial(FallbackWaterMaterialPath, new Color(0.12f, 0.70f, 0.78f, 0.42f), true);
        }

        private static Material CreateMaterial(string path, Color color, bool transparent)
        {
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                material = new Material(Shader.Find("Standard"));
                AssetDatabase.CreateAsset(material, path);
            }

            material.color = color;
            if (transparent)
            {
                material.SetFloat("_Mode", 3f);
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.EnableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;
            }

            EditorUtility.SetDirty(material);
            return material;
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

        private static Transform CreateChild(Transform parent, string name)
        {
            GameObject child = new(name);
            child.transform.SetParent(parent, false);
            return child.transform;
        }

        private static void DestroyCollidersRecursively(GameObject target)
        {
            foreach (Collider collider in target.GetComponentsInChildren<Collider>())
            {
                Object.DestroyImmediate(collider);
            }
        }

        private static void DestroyCollider(GameObject target)
        {
            Collider collider = target.GetComponent<Collider>();
            if (collider != null)
            {
                Object.DestroyImmediate(collider);
            }
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
            {
                return;
            }

            string parent = Path.GetDirectoryName(path)?.Replace('\\', '/');
            string name = Path.GetFileName(path);
            if (!string.IsNullOrEmpty(parent))
            {
                EnsureFolder(parent);
                AssetDatabase.CreateFolder(parent, name);
            }
        }

        private static string SanitizeName(string value)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                value = value.Replace(c, '_');
            }
            return value.Length > 48 ? value.Substring(0, 48) : value;
        }

        private readonly struct ReviewAsset
        {
            public ReviewAsset(string label, string path)
            {
                Label = label;
                Path = path;
            }

            public string Label { get; }
            public string Path { get; }
        }
    }
}
