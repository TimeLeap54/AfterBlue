using UnityEngine;

namespace AfterBlue.Map
{
    [CreateAssetMenu(menuName = "AfterBlue/Map/Map 01 Habitat")]
    public sealed class Map01HabitatData : ScriptableObject
    {
        [SerializeField] private Map01HabitatId habitatId = Map01HabitatId.None;
        [SerializeField] private string displayName = "Habitat";
        [SerializeField] private Vector3 center;
        [SerializeField] private float coreRadius = 6f;
        [SerializeField] private float blendRadius = 14f;
        [SerializeField] private Map01DepthBand depthBand = Map01DepthBand.Mid;
        [SerializeField] private Color debugColor = Color.cyan;
        [SerializeField] private string[] proxyFishIds = System.Array.Empty<string>();
        [SerializeField] private string[] behaviorPatternIds = System.Array.Empty<string>();

        public Map01HabitatId HabitatId => habitatId;
        public string DisplayName => displayName;
        public Vector3 Center => center;
        public float CoreRadius => coreRadius;
        public float BlendRadius => blendRadius;
        public Map01DepthBand DepthBand => depthBand;
        public Color DebugColor => debugColor;
        public string[] ProxyFishIds => proxyFishIds;
        public string[] BehaviorPatternIds => behaviorPatternIds;
    }
}
