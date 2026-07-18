using UnityEngine;

namespace AfterBlue.Map
{
    [System.Serializable]
    public struct ProxyFishEntry
    {
        public string fishId;
        public string displayName;
        public float baseWeight;
        public Map01HabitatId primaryHabitat;
        public Map01DepthBand depthBand;
        public string behaviorPatternId;
    }

    [CreateAssetMenu(menuName = "AfterBlue/Map/Proxy Fish Pool")]
    public sealed class ProxyFishPool : ScriptableObject
    {
        [SerializeField] private ProxyFishEntry[] entries = System.Array.Empty<ProxyFishEntry>();

        public ProxyFishEntry[] Entries => entries;
    }
}
