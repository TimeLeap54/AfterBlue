using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace AfterBlue.EditorTools
{
    public static class FloodedGroundsMaterialConversion
    {
        // Converts the locally imported Flooded Grounds package into AfterBlue review-only URP materials.
        private const string SourceRoot = "Assets/Flooded_Grounds";
        private const string ConvertedMaterialFolder = "Assets/AfterBlue/Materials/FloodedGroundsConverted";
        private const string SceneFolder = "Assets/AfterBlue/Scenes/AssetLab";
        private const string SourceSceneA = "Assets/AfterBlue/Scenes/AssetLab/FloodedGrounds_SceneA_Review.unity";
        private const string SourcePreassembled = "Assets/AfterBlue/Scenes/AssetLab/FloodedGrounds_PreAssembledBuildings_Review.unity";
        private const string ConvertedSceneA = "Assets/AfterBlue/Scenes/AssetLab/FloodedGrounds_SceneA_Review_Converted.unity";
        private const string ConvertedPreassembled = "Assets/AfterBlue/Scenes/AssetLab/FloodedGrounds_PreAssembledBuildings_Review_Converted.unity";
        private const string TriggerPath = "Temp/AfterBlueRunFloodedGroundsConversion.once";

        [InitializeOnLoadMethod]
        private static void RunWhenTriggered()
        {
            if (!File.Exists(TriggerPath))
            {
                return;
            }

            File.Delete(TriggerPath);
            EditorApplication.delayCall += ConvertMaterialsAndReviewScenes;
        }

        [MenuItem("AfterBlue/Setup/Convert Flooded Grounds Materials")]
        public static void ConvertMaterialsAndReviewScenes()
        {
            if (!AssetDatabase.IsValidFolder(SourceRoot))
            {
                EditorUtility.DisplayDialog("AfterBlue", "Flooded Grounds is not imported yet.", "OK");
                return;
            }

            EnsureFolder("Assets/AfterBlue");
            EnsureFolder("Assets/AfterBlue/Materials");
            EnsureFolder(ConvertedMaterialFolder);
            EnsureFolder("Assets/AfterBlue/Scenes");
            EnsureFolder(SceneFolder);

            Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            Dictionary<Material, Material> materialMap = ConvertAllMaterials(shader);

            int sceneAReplacements = ConvertScene(SourceSceneA, ConvertedSceneA, materialMap);
            int preassembledReplacements = ConvertScene(SourcePreassembled, ConvertedPreassembled, materialMap);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log(
                $"Converted Flooded Grounds materials. Materials: {materialMap.Count}, " +
                $"SceneA replacements: {sceneAReplacements}, Preassembled replacements: {preassembledReplacements}");
        }

        private static Dictionary<Material, Material> ConvertAllMaterials(Shader shader)
        {
            Dictionary<Material, Material> materialMap = new();
            string[] guids = AssetDatabase.FindAssets("t:Material", new[] { SourceRoot });
            foreach (string guid in guids)
            {
                string sourcePath = AssetDatabase.GUIDToAssetPath(guid);
                Material source = AssetDatabase.LoadAssetAtPath<Material>(sourcePath);
                if (source == null)
                {
                    continue;
                }

                string convertedPath = $"{ConvertedMaterialFolder}/{SanitizeName(Path.GetFileNameWithoutExtension(sourcePath))}_{guid.Substring(0, 8)}.mat";
                Material converted = AssetDatabase.LoadAssetAtPath<Material>(convertedPath);
                if (converted == null)
                {
                    converted = new Material(shader);
                    AssetDatabase.CreateAsset(converted, convertedPath);
                }
                else
                {
                    converted.shader = shader;
                }

                converted.name = Path.GetFileNameWithoutExtension(convertedPath);
                CopyReadableProperties(source, converted);
                EditorUtility.SetDirty(converted);
                materialMap[source] = converted;
            }

            return materialMap;
        }

        private static int ConvertScene(string sourceScenePath, string convertedScenePath, IReadOnlyDictionary<Material, Material> materialMap)
        {
            if (!File.Exists(sourceScenePath))
            {
                Debug.LogWarning($"Review source scene was not found: {sourceScenePath}");
                return 0;
            }

            UnityEngine.SceneManagement.Scene scene = EditorSceneManager.OpenScene(sourceScenePath, OpenSceneMode.Single);
            int replacements = 0;
            foreach (Renderer renderer in Object.FindObjectsOfType<Renderer>())
            {
                Material[] materials = renderer.sharedMaterials;
                bool changed = false;
                for (int i = 0; i < materials.Length; i++)
                {
                    Material material = materials[i];
                    if (material != null && materialMap.TryGetValue(material, out Material converted))
                    {
                        materials[i] = converted;
                        replacements++;
                        changed = true;
                    }
                }

                if (changed)
                {
                    renderer.sharedMaterials = materials;
                    EditorUtility.SetDirty(renderer);
                }
            }

            EditorSceneManager.SaveScene(scene, convertedScenePath);
            return replacements;
        }

        private static void CopyReadableProperties(Material source, Material target)
        {
            Color color = FindColor(source, new[] { "_BaseColor", "_Color", "_Tint", "_MainColor" }, new Color(0.72f, 0.72f, 0.68f, 1f));
            if (target.HasProperty("_BaseColor"))
            {
                target.SetColor("_BaseColor", color);
            }
            if (target.HasProperty("_Color"))
            {
                target.SetColor("_Color", color);
            }

            Texture baseMap = FindTexture(source, new[] { "_BaseMap", "_MainTex", "_Albedo", "_Diffuse", "_ColorMap" }, false);
            if (baseMap != null)
            {
                SetTextureIfPresent(target, "_BaseMap", baseMap);
                SetTextureIfPresent(target, "_MainTex", baseMap);
            }

            Texture normalMap = FindTexture(source, new[] { "_BumpMap", "_NormalMap", "_Normal", "_Bump" }, true);
            if (normalMap != null)
            {
                SetTextureIfPresent(target, "_BumpMap", normalMap);
                target.EnableKeyword("_NORMALMAP");
            }

            SetFloatIfPresent(target, "_Metallic", FindFloat(source, new[] { "_Metallic" }, 0f));
            SetFloatIfPresent(target, "_Smoothness", FindFloat(source, new[] { "_Smoothness", "_Glossiness" }, 0.35f));
            SetFloatIfPresent(target, "_Surface", color.a < 0.98f ? 1f : 0f);
            SetFloatIfPresent(target, "_AlphaClip", 0f);
            target.renderQueue = color.a < 0.98f ? 3000 : -1;
        }

        private static Texture FindTexture(Material material, IReadOnlyList<string> preferredNames, bool normalLike)
        {
            foreach (string property in preferredNames)
            {
                if (material.HasProperty(property))
                {
                    Texture texture = material.GetTexture(property);
                    if (texture != null)
                    {
                        return texture;
                    }
                }
            }

            if (material.shader == null)
            {
                return null;
            }

            int propertyCount = ShaderUtil.GetPropertyCount(material.shader);
            for (int i = 0; i < propertyCount; i++)
            {
                if (ShaderUtil.GetPropertyType(material.shader, i) != ShaderUtil.ShaderPropertyType.TexEnv)
                {
                    continue;
                }

                string name = ShaderUtil.GetPropertyName(material.shader, i);
                string lower = name.ToLowerInvariant();
                if (normalLike != (lower.Contains("normal") || lower.Contains("bump")))
                {
                    continue;
                }

                Texture texture = material.GetTexture(name);
                if (texture != null)
                {
                    return texture;
                }
            }

            return null;
        }

        private static Color FindColor(Material material, IReadOnlyList<string> preferredNames, Color fallback)
        {
            foreach (string property in preferredNames)
            {
                if (material.HasProperty(property))
                {
                    return material.GetColor(property);
                }
            }

            if (material.shader == null)
            {
                return fallback;
            }

            int propertyCount = ShaderUtil.GetPropertyCount(material.shader);
            for (int i = 0; i < propertyCount; i++)
            {
                if (ShaderUtil.GetPropertyType(material.shader, i) == ShaderUtil.ShaderPropertyType.Color)
                {
                    return material.GetColor(ShaderUtil.GetPropertyName(material.shader, i));
                }
            }

            return fallback;
        }

        private static float FindFloat(Material material, IReadOnlyList<string> preferredNames, float fallback)
        {
            foreach (string property in preferredNames)
            {
                if (material.HasProperty(property))
                {
                    return material.GetFloat(property);
                }
            }

            return fallback;
        }

        private static void SetTextureIfPresent(Material material, string property, Texture texture)
        {
            if (material.HasProperty(property))
            {
                material.SetTexture(property, texture);
            }
        }

        private static void SetFloatIfPresent(Material material, string property, float value)
        {
            if (material.HasProperty(property))
            {
                material.SetFloat(property, value);
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

            return value.Replace(' ', '_');
        }
    }
}
