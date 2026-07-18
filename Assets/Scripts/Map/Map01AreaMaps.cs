using UnityEngine;

namespace AfterBlue.Map
{
    [System.Serializable]
    public struct Map01DepthZone
    {
        public string label;
        public Map01DepthBand depthBand;
        public Vector3 center;
        public Vector2 size;
        public float rotationY;
        public Color debugColor;
    }

    [System.Serializable]
    public struct Map01CastZone
    {
        public string label;
        public Vector3 center;
        public Vector2 size;
        public float rotationY;
        public bool castable;
    }

    [CreateAssetMenu(menuName = "AfterBlue/Map/Map 01 Depth Map")]
    public sealed class Map01DepthMap : ScriptableObject
    {
        [SerializeField] private Map01DepthZone[] zones = System.Array.Empty<Map01DepthZone>();

        public Map01DepthZone[] Zones => zones;
    }

    [CreateAssetMenu(menuName = "AfterBlue/Map/Map 01 Cast Validity Map")]
    public sealed class Map01CastValidityMap : ScriptableObject
    {
        [SerializeField] private float targetCastableWaterRatio = 0.8f;
        [SerializeField] private Map01CastZone[] zones = System.Array.Empty<Map01CastZone>();

        public float TargetCastableWaterRatio => targetCastableWaterRatio;
        public Map01CastZone[] Zones => zones;
    }
}
