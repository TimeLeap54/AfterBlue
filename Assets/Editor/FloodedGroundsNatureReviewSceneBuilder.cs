using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AfterBlue.EditorTools
{
    public static class FloodedGroundsNatureReviewSceneBuilder
    {
        private const string ScenePath = "Assets/AfterBlue/Scenes/AssetLab/FloodedGrounds_NatureMoss_Review.unity";
        private const string TriggerPath = "Temp/AfterBlueRunFloodedGroundsNatureReview.once";
        private const string FloodedGroundsRoot = "Assets/Flooded_Grounds";
        private const string ConvertedMaterialFolder = "Assets/AfterBlue/Materials/FloodedGroundsConverted";
        private const string EmbeddedNatureMaterialFolder = "Assets/AfterBlue/Materials/FloodedGroundsConverted/NatureEmbedded";
        private const string WaterPrefabPath = "Assets/IgniteCoders/Simple Water Shader/Prefabs/WaterBlock_50m.prefab";
        private const string WaterMaterialPath = "Assets/AfterBlue/Materials/Week7/AB_Water_Map01_Candidate.mat";
        private const string FallbackWaterMaterialPath = "Assets/AfterBlue/Materials/AssetLab/MAT_NatureReview_WaterFallback.mat";
        private const string GroundMaterialPath = "Assets/AfterBlue/Materials/AssetLab/MAT_NatureReview_Ground.mat";
        private const string LabelMaterialPath = "Assets/AfterBlue/Materials/AssetLab/MAT_NatureReview_Label.mat";
        private const string ShallowTintMaterialPath = "Assets/AfterBlue/Materials/AssetLab/MAT_NatureReview_ShallowTint.mat";
        private const string DeepTintMaterialPath = "Assets/AfterBlue/Materials/AssetLab/MAT_NatureReview_DeepTint.mat";

        private static readonly Dictionary<Material, Material> EmbeddedMaterialCache = new();

        private static readonly ReviewAsset[] GrassAssets =
        {
            new("Grass Small A", "Assets/Flooded_Grounds/Prefabs/Nature/Grass/Grass_Small_A.prefab", 1.8f),
            new("Grass Small B", "Assets/Flooded_Grounds/Prefabs/Nature/Grass/Grass_Small_B.prefab", 1.8f),
            new("Grass Small C", "Assets/Flooded_Grounds/Prefabs/Nature/Grass/Grass_Small_C.prefab", 1.8f),
            new("Grass Small D", "Assets/Flooded_Grounds/Prefabs/Nature/Grass/Grass_Small_D.prefab", 1.8f),
            new("Grass Med A", "Assets/Flooded_Grounds/Prefabs/Nature/Grass/Grass_Med_A.prefab", 1.7f),
            new("Grass Med B", "Assets/Flooded_Grounds/Prefabs/Nature/Grass/Grass_Med_B.prefab", 1.7f),
            new("Grass Med C", "Assets/Flooded_Grounds/Prefabs/Nature/Grass/Grass_Med_C.prefab", 1.7f),
            new("Grass Tall A", "Assets/Flooded_Grounds/Prefabs/Nature/Grass/Grass_Tall_A.prefab", 1.5f),
            new("Grass Tall B", "Assets/Flooded_Grounds/Prefabs/Nature/Grass/Grass_Tall_B.prefab", 1.5f),
            new("Grass Tall C", "Assets/Flooded_Grounds/Prefabs/Nature/Grass/Grass_Tall_C.prefab", 1.5f),
        };

        private static readonly ReviewAsset[] BushAssets =
        {
            new("Bush A", "Assets/Flooded_Grounds/Prefabs/Nature/Bushes/DecoBush_A.prefab", 1.45f),
            new("Bush B", "Assets/Flooded_Grounds/Prefabs/Nature/Bushes/DecoBush_B.prefab", 1.45f),
            new("Bush C", "Assets/Flooded_Grounds/Prefabs/Nature/Bushes/DecoBush_C.prefab", 1.45f),
            new("Bush D", "Assets/Flooded_Grounds/Prefabs/Nature/Bushes/DecoBush_D.prefab", 1.45f),
            new("Tree Bush", "Assets/Flooded_Grounds/Prefabs/Nature/Trees/TreeCreator_Bush_A.prefab", 1.15f),
            new("Crinkly A", "Assets/Flooded_Grounds/Prefabs/Nature/Trees/TreeCreator_Crinkly_A.prefab", 0.9f),
            new("Crinkly B", "Assets/Flooded_Grounds/Prefabs/Nature/Trees/TreeCreator_Crinkly_B.prefab", 0.9f),
        };

        private static readonly ReviewAsset[] TreeAssets =
        {
            new("Small Tree A", "Assets/Flooded_Grounds/Prefabs/Nature/Trees/TreeCreator_Small_A.prefab", 0.9f),
            new("Small Tree B", "Assets/Flooded_Grounds/Prefabs/Nature/Trees/TreeCreator_Small_B.prefab", 0.9f),
            new("Tall Tree A", "Assets/Flooded_Grounds/Prefabs/Nature/Trees/TreeCreator_Tall_A.prefab", 0.72f),
            new("Tall Tree B", "Assets/Flooded_Grounds/Prefabs/Nature/Trees/TreeCreator_Tall_B.prefab", 0.72f),
            new("Tall Tree C", "Assets/Flooded_Grounds/Prefabs/Nature/Trees/TreeCreator_Tall_C.prefab", 0.72f),
            new("Dead Tall C", "Assets/Flooded_Grounds/Prefabs/Nature/Trees/TreeCreator_Tall_C_Dead.prefab", 0.72f),
        };

        private static readonly ReviewAsset[] RockAssets =
        {
            new("Rock A", "Assets/Flooded_Grounds/Prefabs/Nature/Rocks/Rock_A.prefab", 1.35f),
            new("Rock B", "Assets/Flooded_Grounds/Prefabs/Nature/Rocks/Rock_B.prefab", 1.35f),
            new("Cobble A", "Assets/Flooded_Grounds/Prefabs/Nature/Rocks/CobbleRock_A.prefab", 1.25f),
            new("Cobble B", "Assets/Flooded_Grounds/Prefabs/Nature/Rocks/CobbleRock_B.prefab", 1.25f),
            new("Cobble C", "Assets/Flooded_Grounds/Prefabs/Nature/Rocks/CobbleRock_C.prefab", 1.25f),
            new("Cobble D", "Assets/Flooded_Grounds/Prefabs/Nature/Rocks/CobbleRock_D.prefab", 1.25f),
            new("Cobble E", "Assets/Flooded_Grounds/Prefabs/Nature/Rocks/CobbleRock_E.prefab", 1.25f),
            new("Cobble F", "Assets/Flooded_Grounds/Prefabs/Nature/Rocks/CobbleRock_F.prefab", 1.25f),
        };

        private static readonly ReviewAsset[] AtmosphereAssets =
        {
            new("Leaves A", "Assets/Flooded_Grounds/Prefabs/Atmospherics/ATM_Leaves_A.prefab", 1.0f),
            new("Leaves B", "Assets/Flooded_Grounds/Prefabs/Atmospherics/ATM_Leaves_B.prefab", 1.0f),
        };

        [InitializeOnLoadMethod]
        private static void RunWhenTriggered()
        {
            if (!File.Exists(TriggerPath))
            {
                return;
            }

            File.Delete(TriggerPath);
            EditorApplication.delayCall += CreateNatureReviewScene;
        }

        [MenuItem("AfterBlue/Setup/Create Flooded Grounds Nature Review Scene")]
        public static void CreateNatureReviewScene()
        {
            if (!AssetDatabase.IsValidFolder(FloodedGroundsRoot))
            {
                EditorUtility.DisplayDialog("AfterBlue", "Flooded Grounds is not imported yet.", "OK");
                return;
            }

            EnsureFolder("Assets/AfterBlue/Scenes/AssetLab");
            EnsureFolder("Assets/AfterBlue/Materials/AssetLab");
            EnsureFolder(EmbeddedNatureMaterialFolder);
            EmbeddedMaterialCache.Clear();

            Material water = LoadOrCreateWaterMaterial();
            Material ground = CreateMaterial(GroundMaterialPath, new Color(0.09f, 0.15f, 0.12f, 1f), false);
            Material label = CreateMaterial(LabelMaterialPath, new Color(0.84f, 0.98f, 0.88f, 1f), false);
            Material shallowTint = CreateMaterial(ShallowTintMaterialPath, new Color(0.54f, 0.95f, 0.76f, 0.22f), true);
            Material deepTint = CreateMaterial(DeepTintMaterialPath, new Color(0.14f, 0.23f, 0.42f, 0.30f), true);

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "FloodedGrounds_NatureMoss_Review";

            GameObject root = new("FloodedGrounds_NatureMoss_Review");
            Transform system = CreateChild(root.transform, "00_SYSTEM");
            Transform stage = CreateChild(root.transform, "01_WATER_AND_DEPTH_STAGE");
            Transform catalog = CreateChild(root.transform, "02_NATURE_ASSET_CATALOG");
            Transform placement = CreateChild(root.transform, "03_SUBMERGED_PLACEMENT_TESTS");
            Transform notes = CreateChild(root.transform, "99_NOTES");

            CreateWaterStage(stage, water);
            DestroyCollider(CreateBox(stage, "Ground_Underwater_160x110", new Vector3(0f, -1.25f, 0f), new Vector3(160f, 0.12f, 110f), Quaternion.identity, ground));
            DestroyCollider(CreateBox(stage, "Shallow_Readability_Tint", new Vector3(-44f, -0.95f, 0f), new Vector3(52f, 0.05f, 96f), Quaternion.identity, shallowTint));
            DestroyCollider(CreateBox(stage, "Deep_Readability_Tint", new Vector3(44f, -0.90f, 0f), new Vector3(52f, 0.05f, 96f), Quaternion.identity, deepTint));
            CreateLabel(stage, "Y=0 Water. Left: shallow overgrowth / right: deeper silhouette check", new Vector3(-76f, 2.8f, -52f), label, 1.3f);

            CreateCatalogRow(catalog, "Grass Variants", GrassAssets, new Vector3(-72f, 0f, 42f), label);
            CreateCatalogRow(catalog, "Bush And Small Tree Variants", BushAssets, new Vector3(-72f, 0f, 24f), label);
            CreateCatalogRow(catalog, "Tall Tree Variants", TreeAssets, new Vector3(-72f, 0f, 6f), label);
            CreateCatalogRow(catalog, "Rock And Cobble Variants", RockAssets, new Vector3(-72f, 0f, -12f), label);
            CreateCatalogRow(catalog, "Atmosphere Leaves", AtmosphereAssets, new Vector3(-72f, 0f, -30f), label);

            CreatePlacementTests(placement, label);

            CreateLabel(notes, "Use candidates for: submerged road seams, roof moss, shallow H1 greenery, H3 debris silhouettes.", new Vector3(-72f, 2.4f, -48f), label, 1.35f);
            CreateLabel(notes, "Flooded_Grounds remains local-only. Keep selected prefabs/material copies under AfterBlue before committing.", new Vector3(-72f, 0.9f, -48f), label, 1.15f);

            CreateCamera(system);
            CreateLighting(system);

            RenderSettings.ambientLight = new Color(0.46f, 0.58f, 0.54f, 1f);
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Linear;
            RenderSettings.fogColor = new Color(0.45f, 0.66f, 0.65f, 1f);
            RenderSettings.fogStartDistance = 90f;
            RenderSettings.fogEndDistance = 250f;

            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();
            Debug.Log($"Created Flooded Grounds nature review scene: {ScenePath}");
        }

        private static void CreatePlacementTests(Transform parent, Material label)
        {
            CreateLabel(parent, "Placement height tests", new Vector3(-46f, 2.8f, -2f), label, 1.4f);

            InstantiateAsset(parent, "Assets/Flooded_Grounds/Prefabs/Nature/Grass/Grass_Tall_A.prefab", "Test_RoadSeam_TallGrass_Submerged", new Vector3(-44f, -0.95f, 10f), Quaternion.Euler(0f, 12f, 0f), Vector3.one * 2.4f);
            InstantiateAsset(parent, "Assets/Flooded_Grounds/Prefabs/Nature/Grass/Grass_Med_A.prefab", "Test_RoadSeam_MedGrass_Submerged", new Vector3(-38f, -0.95f, 13f), Quaternion.Euler(0f, -22f, 0f), Vector3.one * 2.2f);
            InstantiateAsset(parent, "Assets/Flooded_Grounds/Prefabs/Nature/Bushes/DecoBush_A.prefab", "Test_Shallow_Bush_Partial", new Vector3(-28f, -0.68f, 4f), Quaternion.Euler(0f, 28f, 0f), Vector3.one * 2.0f);
            InstantiateAsset(parent, "Assets/Flooded_Grounds/Prefabs/Nature/Rocks/Rock_A.prefab", "Test_Rock_Silhouette", new Vector3(-19f, -0.86f, 9f), Quaternion.Euler(0f, -12f, 0f), Vector3.one * 2.0f);

            InstantiateAsset(parent, "Assets/Flooded_Grounds/Prefabs/Nature/Trees/TreeCreator_Small_A.prefab", "Test_RoofTree_Small", new Vector3(18f, -0.55f, 8f), Quaternion.Euler(0f, -18f, 0f), Vector3.one * 1.25f);
            InstantiateAsset(parent, "Assets/Flooded_Grounds/Prefabs/Nature/Trees/TreeCreator_Tall_C_Dead.prefab", "Test_H3_DeadTree_Silhouette", new Vector3(31f, -0.40f, 13f), Quaternion.Euler(0f, 18f, 0f), Vector3.one * 1.1f);
            InstantiateAsset(parent, "Assets/Flooded_Grounds/Prefabs/Nature/Bushes/DecoBush_D.prefab", "Test_H3_Bush_DeepTint", new Vector3(42f, -0.92f, 2f), Quaternion.Euler(0f, 50f, 0f), Vector3.one * 2.2f);
            InstantiateAsset(parent, "Assets/Flooded_Grounds/Prefabs/Nature/Rocks/CobbleRock_F.prefab", "Test_H3_Cobble_DeepTint", new Vector3(51f, -0.95f, 11f), Quaternion.Euler(0f, -5f, 0f), Vector3.one * 2.2f);

            CreateLabel(parent, "Road seams: mostly below water", new Vector3(-50f, 1.4f, 18f), label, 0.9f);
            CreateLabel(parent, "Roof / shallow edge: partial exposure", new Vector3(8f, 1.4f, 18f), label, 0.9f);
            CreateLabel(parent, "H3: dark silhouette, not full detail", new Vector3(34f, 1.4f, 18f), label, 0.9f);
        }

        private static void CreateCatalogRow(Transform parent, string title, IReadOnlyList<ReviewAsset> assets, Vector3 origin, Material labelMaterial)
        {
            Transform row = CreateChild(parent, title.Replace(" ", "_"));
            CreateLabel(row, title, origin + new Vector3(0f, 3f, -4.2f), labelMaterial, 1.2f);

            for (int i = 0; i < assets.Count; i++)
            {
                ReviewAsset asset = assets[i];
                Vector3 position = origin + new Vector3(i * 13f, -0.65f, 0f);
                InstantiateAsset(row, asset.Path, asset.Label, position, Quaternion.Euler(0f, 25f, 0f), Vector3.one * asset.Scale);
                CreateLabel(row, asset.Label, position + new Vector3(-3.8f, 1.8f, -4.2f), labelMaterial, 0.7f);
            }
        }

        private static bool InstantiateAsset(Transform parent, string path, string name, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null)
            {
                Debug.LogWarning($"Nature review asset missing: {path}");
                return false;
            }

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            instance.name = name;
            instance.transform.SetParent(parent, false);
            instance.transform.localPosition = position;
            instance.transform.localRotation = rotation;
            instance.transform.localScale = scale;
            ApplyConvertedMaterials(instance);
            DestroyCollidersRecursively(instance);
            return true;
        }

        private static void ApplyConvertedMaterials(GameObject target)
        {
            foreach (Renderer renderer in target.GetComponentsInChildren<Renderer>(true))
            {
                Material[] materials = renderer.sharedMaterials;
                bool changed = false;
                for (int i = 0; i < materials.Length; i++)
                {
                    Material converted = FindConvertedMaterial(materials[i]);
                    if (converted == null)
                    {
                        continue;
                    }

                    materials[i] = converted;
                    changed = true;
                }

                if (changed)
                {
                    renderer.sharedMaterials = materials;
                }
            }
        }

        private static Material FindConvertedMaterial(Material source)
        {
            if (source == null || !AssetDatabase.IsValidFolder(ConvertedMaterialFolder))
            {
                return null;
            }

            string safeName = SanitizeName(source.name);
            string[] guids = AssetDatabase.FindAssets($"{safeName} t:Material", new[] { ConvertedMaterialFolder });
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (Path.GetFileNameWithoutExtension(path).StartsWith(safeName + "_"))
                {
                    return AssetDatabase.LoadAssetAtPath<Material>(path);
                }
            }

            return CreateEmbeddedNatureMaterial(source);
        }

        private static Material CreateEmbeddedNatureMaterial(Material source)
        {
            if (EmbeddedMaterialCache.TryGetValue(source, out Material cached))
            {
                return cached;
            }

            Texture diffuse = GetFirstTexture(source, "_MainTex", "_BaseMap", "_BaseColorMap", "_DiffuseTex");
            if (diffuse == null && !source.name.Contains("Optimized"))
            {
                return null;
            }

            bool leafLike = source.name.ToLowerInvariant().Contains("leaf") ||
                            source.name.ToLowerInvariant().Contains("bush") ||
                            source.name.ToLowerInvariant().Contains("flower") ||
                            source.name.ToLowerInvariant().Contains("branch");

            string textureName = diffuse != null ? SanitizeName(diffuse.name) : "NoTexture";
            string materialName = SanitizeName($"{source.name}_{textureName}");
            string path = $"{EmbeddedNatureMaterialFolder}/{materialName}.mat";

            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
                material = new Material(shader)
                {
                    name = materialName
                };
                AssetDatabase.CreateAsset(material, path);
            }

            Color color = source.HasProperty("_Color") ? source.color : Color.white;
            color = leafLike
                ? Color.Lerp(color, new Color(0.47f, 0.68f, 0.38f, color.a), 0.35f)
                : Color.Lerp(color, new Color(0.38f, 0.30f, 0.22f, color.a), 0.20f);

            material.color = color;
            SetColorIfPresent(material, "_BaseColor", color);
            SetTextureIfPresent(material, "_MainTex", diffuse);
            SetTextureIfPresent(material, "_BaseMap", diffuse);

            if (leafLike)
            {
                SetFloatIfPresent(material, "_AlphaClip", 1f);
                SetFloatIfPresent(material, "_Cutoff", 0.33f);
                material.EnableKeyword("_ALPHATEST_ON");
                material.renderQueue = 2450;
            }
            else
            {
                material.renderQueue = 2000;
            }

            EditorUtility.SetDirty(material);
            EmbeddedMaterialCache[source] = material;
            return material;
        }

        private static Texture GetFirstTexture(Material source, params string[] propertyNames)
        {
            foreach (string propertyName in propertyNames)
            {
                if (source.HasProperty(propertyName))
                {
                    Texture texture = source.GetTexture(propertyName);
                    if (texture != null)
                    {
                        return texture;
                    }
                }
            }

            return source.mainTexture;
        }

        private static void SetColorIfPresent(Material material, string propertyName, Color color)
        {
            if (material.HasProperty(propertyName))
            {
                material.SetColor(propertyName, color);
            }
        }

        private static void SetTextureIfPresent(Material material, string propertyName, Texture texture)
        {
            if (texture != null && material.HasProperty(propertyName))
            {
                material.SetTexture(propertyName, texture);
            }
        }

        private static void SetFloatIfPresent(Material material, string propertyName, float value)
        {
            if (material.HasProperty(propertyName))
            {
                material.SetFloat(propertyName, value);
            }
        }

        private static void CreateWaterStage(Transform parent, Material water)
        {
            GameObject waterPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(WaterPrefabPath);
            if (waterPrefab != null)
            {
                GameObject waterObject = (GameObject)PrefabUtility.InstantiatePrefab(waterPrefab);
                waterObject.name = "NatureReview_Water_160x110";
                waterObject.transform.SetParent(parent, false);
                waterObject.transform.localPosition = Vector3.zero;
                waterObject.transform.localScale = new Vector3(160f / 50f, 1f, 110f / 50f);
                foreach (Renderer renderer in waterObject.GetComponentsInChildren<Renderer>())
                {
                    renderer.sharedMaterial = water;
                }

                DestroyCollidersRecursively(waterObject);
                return;
            }

            DestroyCollider(CreateBox(parent, "NatureReview_Water_Fallback_160x110", Vector3.zero, new Vector3(160f, 0.08f, 110f), Quaternion.identity, water));
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
                material.SetFloat("_Surface", 1f);
                material.SetFloat("_Blend", 0f);
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
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
            light.intensity = 1.05f;
            light.color = new Color(0.84f, 0.96f, 1f, 1f);
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
            public ReviewAsset(string label, string path, float scale)
            {
                Label = label;
                Path = path;
                Scale = scale;
            }

            public string Label { get; }
            public string Path { get; }
            public float Scale { get; }
        }
    }
}
