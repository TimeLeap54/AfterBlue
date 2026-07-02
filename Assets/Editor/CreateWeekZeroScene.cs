using AfterBlue.Boat;
using AfterBlue.Core;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AfterBlue.EditorTools
{
    public static class CreateWeekZeroScene
    {
        private const string ScenePath = "Assets/Scenes/FishingScene.unity";

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
    }
}

