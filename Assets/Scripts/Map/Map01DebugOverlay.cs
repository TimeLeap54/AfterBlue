using System.Text;
using UnityEngine;

namespace AfterBlue.Map
{
    [ExecuteAlways]
    public sealed class Map01DebugOverlay : MonoBehaviour
    {
        [SerializeField] private Map01Settings settings;
        [SerializeField] private Map01HabitatData[] habitats = System.Array.Empty<Map01HabitatData>();
        [SerializeField] private bool createRuntimeGreyboxProps = true;
        [SerializeField] private bool showGameplayMap = true;
        [SerializeField] private bool showHabitatRadius = true;
        [SerializeField] private bool showDepthColors;
        [SerializeField] private bool showCastValidity;
        [SerializeField] private bool showBobberWeights;
        [SerializeField] private bool showMovementPath;
        [SerializeField] private bool showLandmarkSightlines = true;

        private readonly StringBuilder builder = new StringBuilder(512);
        private const string RuntimePropsRootName = "Map01 Runtime Greybox Props";

        private void OnEnable()
        {
            EnsureRuntimeGreyboxProps();
        }

        private void Start()
        {
            EnsureRuntimeGreyboxProps();
        }

        private void Update()
        {
            if (SafeGetKeyDown(KeyCode.F1)) showGameplayMap = !showGameplayMap;
            if (SafeGetKeyDown(KeyCode.F2)) showHabitatRadius = !showHabitatRadius;
            if (SafeGetKeyDown(KeyCode.F3)) showDepthColors = !showDepthColors;
            if (SafeGetKeyDown(KeyCode.F4)) showCastValidity = !showCastValidity;
            if (SafeGetKeyDown(KeyCode.F5)) showBobberWeights = !showBobberWeights;
            if (SafeGetKeyDown(KeyCode.F6)) showMovementPath = !showMovementPath;
            if (SafeGetKeyDown(KeyCode.F7)) showLandmarkSightlines = !showLandmarkSightlines;
            if (SafeGetKeyDown(KeyCode.F8)) Debug.Log("Map_01 S2 debug test data reset requested.");
        }

        private void OnGUI()
        {
            builder.Length = 0;
            builder.AppendLine("S2 Map_01 Design Lock");
            builder.AppendLine($"F1 Gameplay Map: {Flag(showGameplayMap)}");
            builder.AppendLine($"F2 Habitat Radius: {Flag(showHabitatRadius)}");
            builder.AppendLine($"F3 Depth Colors: {Flag(showDepthColors)}");
            builder.AppendLine($"F4 Cast Validity: {Flag(showCastValidity)}");
            builder.AppendLine($"F5 Bobber Weights: {Flag(showBobberWeights)}");
            builder.AppendLine($"F6 Movement Path: {Flag(showMovementPath)}");
            builder.AppendLine($"F7 Sightlines: {Flag(showLandmarkSightlines)}");
            builder.AppendLine("F8 Reset test data");

            GUI.Box(new Rect(12f, 42f, 250f, 176f), builder.ToString());
        }

        private void OnDrawGizmos()
        {
            if (settings == null)
            {
                return;
            }

            if (showGameplayMap)
            {
                DrawPlayableBoundary();
                DrawPoint(ToWorld(settings.StartSupplyBuoy), Color.yellow, "Start / Supply");
                DrawPoint(ToWorld(settings.CentralSubmergedRoad), Color.white, "Central Road");
            }

            if (showHabitatRadius)
            {
                for (int i = 0; i < habitats.Length; i++)
                {
                    DrawHabitat(habitats[i]);
                }
            }

            if (showLandmarkSightlines)
            {
                DrawSightlineChain();
            }
        }

        private static string Flag(bool value)
        {
            return value ? "ON" : "OFF";
        }

        private static bool SafeGetKeyDown(KeyCode keyCode)
        {
            try
            {
                return Input.GetKeyDown(keyCode);
            }
            catch (System.InvalidOperationException)
            {
                return false;
            }
        }

        private void DrawPlayableBoundary()
        {
            Vector2 min = settings.PlayableMin;
            Vector2 max = settings.PlayableMax;
            float y = settings.WaterY + 0.05f;
            Vector3 a = ToWorld(new Vector3(min.x, y, min.y));
            Vector3 b = ToWorld(new Vector3(max.x, y, min.y));
            Vector3 c = ToWorld(new Vector3(max.x, y, max.y));
            Vector3 d = ToWorld(new Vector3(min.x, y, max.y));

            Gizmos.color = new Color(1f, 1f, 1f, 0.8f);
            Gizmos.DrawLine(a, b);
            Gizmos.DrawLine(b, c);
            Gizmos.DrawLine(c, d);
            Gizmos.DrawLine(d, a);
        }

        private void DrawHabitat(Map01HabitatData habitat)
        {
            if (habitat == null)
            {
                return;
            }

            Color color = habitat.DebugColor;
            Vector3 center = ToWorld(habitat.Center) + Vector3.up * 0.08f;
            Gizmos.color = new Color(color.r, color.g, color.b, 0.9f);
            Gizmos.DrawWireSphere(center, habitat.CoreRadius * settings.WorldScale);
            Gizmos.color = new Color(color.r, color.g, color.b, 0.35f);
            Gizmos.DrawWireSphere(center, habitat.BlendRadius * settings.WorldScale);
            DrawPoint(ToWorld(habitat.Center), color, habitat.DisplayName);
        }

        private void DrawSightlineChain()
        {
            Vector3[] points =
            {
                ToWorld(settings.StartSupplyBuoy),
                ToWorld(settings.ShallowResidentialH1),
                ToWorld(settings.CentralSubmergedRoad),
                ToWorld(settings.TrafficLightH2),
                ToWorld(settings.DeepDebrisH3),
                ToWorld(settings.ReturnSightline),
                ToWorld(settings.StartSupplyBuoy)
            };

            Gizmos.color = new Color(0.9f, 1f, 1f, 0.75f);
            for (int i = 0; i < points.Length - 1; i++)
            {
                Gizmos.DrawLine(points[i] + Vector3.up * 0.45f, points[i + 1] + Vector3.up * 0.45f);
            }
        }

        private Vector3 ToWorld(Vector3 designPosition)
        {
            return new Vector3(designPosition.x * settings.WorldScale, designPosition.y, designPosition.z * settings.WorldScale);
        }

        private void EnsureRuntimeGreyboxProps()
        {
            if (!createRuntimeGreyboxProps || settings == null || transform.Find(RuntimePropsRootName) != null)
            {
                return;
            }

            Transform root = new GameObject(RuntimePropsRootName).transform;
            root.SetParent(transform, false);

            Material buoy = RuntimeMaterial("Map01 Runtime Buoy", new Color(0.95f, 0.72f, 0.22f, 1f));
            Material supply = RuntimeMaterial("Map01 Runtime Supply", new Color(0.45f, 0.38f, 0.24f, 1f));
            Material marker = RuntimeMaterial("Map01 Runtime Marker", new Color(0.80f, 0.88f, 0.82f, 1f));
            Material debris = RuntimeMaterial("Map01 Runtime Debris", new Color(0.08f, 0.12f, 0.15f, 1f));
            Material road = RuntimeMaterial("Map01 Runtime Road", new Color(0.10f, 0.12f, 0.13f, 1f));

            Vector3 start = ToWorld(settings.StartSupplyBuoy);
            CreateCylinder(root, "Start Beacon Mast Proxy", start + Vector3.up * 5f, new Vector3(0.28f, 4.8f, 0.28f), Quaternion.identity, marker);
            CreateCylinder(root, "Start Beacon Light Proxy", start + Vector3.up * 8.2f, new Vector3(0.75f, 0.35f, 0.75f), Quaternion.identity, buoy);
            CreateBox(root, "Start Supply Platform Proxy", start + ScaleOffset(new Vector3(1.8f, 0.08f, -0.2f)), ScaleXZ(new Vector3(2.8f, 0.16f, 1.8f)), Quaternion.Euler(0f, -12f, 0f), supply);
            CreateBox(root, "Start Crate Stack Proxy A", start + ScaleOffset(new Vector3(1.25f, 0.42f, -0.1f)), ScaleXZ(new Vector3(0.6f, 0.5f, 0.55f)), Quaternion.Euler(0f, 9f, 0f), supply);
            CreateBox(root, "Start Crate Stack Proxy B", start + ScaleOffset(new Vector3(2f, 0.32f, 0.45f)), ScaleXZ(new Vector3(0.72f, 0.36f, 0.45f)), Quaternion.Euler(0f, -18f, 0f), supply);
            CreateBox(root, "Start Broken Dock Arm Proxy", start + ScaleOffset(new Vector3(-1.7f, 0.1f, 1.35f)), ScaleXZ(new Vector3(3.8f, 0.12f, 0.42f)), Quaternion.Euler(0f, 28f, 0f), debris);
            CreateCylinder(root, "Start Route Marker Proxy L", start + ScaleOffset(new Vector3(-2.4f, 0.55f, 3.1f)), new Vector3(0.35f, 1.7f, 0.35f), Quaternion.identity, marker);
            CreateCylinder(root, "Start Route Marker Proxy R", start + ScaleOffset(new Vector3(2.1f, 0.55f, 3.35f)), new Vector3(0.35f, 1.7f, 0.35f), Quaternion.identity, marker);

            CreateBox(root, "Midwater Low Roof Proxy A", ToWorld(new Vector3(-10f, 0.06f, -2f)), ScaleXZ(new Vector3(5.5f, 0.12f, 2.4f)), Quaternion.Euler(0f, 21f, 0f), debris);
            CreateBox(root, "Midwater Low Roof Proxy B", ToWorld(new Vector3(6f, 0.05f, -7f)), ScaleXZ(new Vector3(6f, 0.10f, 2f)), Quaternion.Euler(0f, -24f, 0f), debris);
            CreateBox(root, "Midwater Broken Asphalt Proxy", ToWorld(new Vector3(-2f, 0.035f, -11f)), ScaleXZ(new Vector3(9.5f, 0.07f, 1.6f)), Quaternion.Euler(0f, 17f, 0f), road);
            CreateCylinder(root, "Midwater Marker Proxy A", ToWorld(new Vector3(-6.5f, 0.8f, 9.5f)), new Vector3(0.25f, 2.4f, 0.25f), Quaternion.Euler(0f, 0f, -7f), marker);
            CreateCylinder(root, "Midwater Marker Proxy B", ToWorld(new Vector3(13.5f, 0.75f, -4f)), new Vector3(0.25f, 2.2f, 0.25f), Quaternion.Euler(0f, 0f, 9f), marker);
        }

        private Vector3 ScaleOffset(Vector3 designOffset)
        {
            return new Vector3(designOffset.x * settings.WorldScale, designOffset.y, designOffset.z * settings.WorldScale);
        }

        private Vector3 ScaleXZ(Vector3 designScale)
        {
            return new Vector3(designScale.x * settings.WorldScale, designScale.y, designScale.z * settings.WorldScale);
        }

        private static Material RuntimeMaterial(string name, Color color)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            Material material = new Material(shader)
            {
                name = name,
                color = color
            };

            if (material.HasProperty("_BaseColor"))
            {
                material.SetColor("_BaseColor", color);
            }

            return material;
        }

        private static void CreateBox(Transform parent, string name, Vector3 position, Vector3 scale, Quaternion rotation, Material material)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.name = name;
            obj.transform.SetParent(parent, false);
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.transform.localScale = scale;
            obj.GetComponent<Renderer>().sharedMaterial = material;
        }

        private static void CreateCylinder(Transform parent, string name, Vector3 position, Vector3 scale, Quaternion rotation, Material material)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            obj.name = name;
            obj.transform.SetParent(parent, false);
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.transform.localScale = scale;
            obj.GetComponent<Renderer>().sharedMaterial = material;
        }

        private static void DrawPoint(Vector3 position, Color color, string label)
        {
            Gizmos.color = color;
            Gizmos.DrawSphere(position + Vector3.up * 0.25f, 0.35f);
#if UNITY_EDITOR
            UnityEditor.Handles.Label(position + Vector3.up * 0.85f, label);
#endif
        }
    }
}
