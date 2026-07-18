using UnityEngine;

namespace AfterBlue.Map
{
    [System.Serializable]
    public struct FishHabitatSuitability
    {
        public string fishId;
        [Range(0f, 2f)] public float central;
        [Range(0f, 2f)] public float h1;
        [Range(0f, 2f)] public float h2;
        [Range(0f, 2f)] public float h3;
        [Range(0f, 2f)] public float shallow;
        [Range(0f, 2f)] public float mid;
        [Range(0f, 2f)] public float deep;
    }

    [CreateAssetMenu(menuName = "AfterBlue/Map/Fish Habitat Data")]
    public sealed class FishHabitatData : ScriptableObject
    {
        [SerializeField] private FishHabitatSuitability[] suitability = System.Array.Empty<FishHabitatSuitability>();

        public FishHabitatSuitability[] Suitability => suitability;
    }
}
