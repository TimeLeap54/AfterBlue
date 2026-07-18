using UnityEngine;

namespace AfterBlue.Map
{
    public enum Map01DepthBand
    {
        Shallow,
        Mid,
        Deep
    }

    public enum Map01HabitatId
    {
        None,
        H1ShallowResidential,
        CentralRoad,
        H2TrafficLight,
        H3DeepDebris
    }

    [CreateAssetMenu(menuName = "AfterBlue/Map/Map 01 Settings")]
    public sealed class Map01Settings : ScriptableObject
    {
        [Header("World")]
        [SerializeField] private Vector2 mapSize = new Vector2(192f, 132f);
        [SerializeField] private float worldScale = 4f;
        [SerializeField] private Vector2 playableMin = new Vector2(-90f, -60f);
        [SerializeField] private Vector2 playableMax = new Vector2(90f, 60f);
        [SerializeField] private float waterY = 0f;

        [Header("Landmarks")]
        [SerializeField] private Vector3 startSupplyBuoy = new Vector3(-72f, 0f, -42f);
        [SerializeField] private Vector3 shallowResidentialH1 = new Vector3(-54f, 0f, 30f);
        [SerializeField] private Vector3 centralSubmergedRoad = new Vector3(0f, 0f, 6f);
        [SerializeField] private Vector3 trafficLightH2 = new Vector3(51f, 0f, 18f);
        [SerializeField] private Vector3 deepDebrisH3 = new Vector3(33f, 0f, -39f);
        [SerializeField] private Vector3 returnSightline = new Vector3(-30f, 0f, -48f);

        [Header("Navigation Targets")]
        [SerializeField] private float channelWidth = 24f;
        [SerializeField] private float minimumPassageWidth = 14f;
        [SerializeField] private float mapEdgeBuffer = 8f;
        [SerializeField] private float boatCollisionRadius = 0.9f;
        [SerializeField] private float targetLongCrossingSeconds = 70f;
        [SerializeField] private float targetCircuitSeconds = 160f;

        public Vector2 MapSize => mapSize;
        public float WorldScale => worldScale;
        public Vector2 WorldMapSize => mapSize * worldScale;
        public Vector2 PlayableMin => playableMin;
        public Vector2 PlayableMax => playableMax;
        public float WaterY => waterY;
        public Vector3 StartSupplyBuoy => startSupplyBuoy;
        public Vector3 ShallowResidentialH1 => shallowResidentialH1;
        public Vector3 CentralSubmergedRoad => centralSubmergedRoad;
        public Vector3 TrafficLightH2 => trafficLightH2;
        public Vector3 DeepDebrisH3 => deepDebrisH3;
        public Vector3 ReturnSightline => returnSightline;
        public float ChannelWidth => channelWidth;
        public float MinimumPassageWidth => minimumPassageWidth;
        public float MapEdgeBuffer => mapEdgeBuffer;
        public float BoatCollisionRadius => boatCollisionRadius;
        public float TargetLongCrossingSeconds => targetLongCrossingSeconds;
        public float TargetCircuitSeconds => targetCircuitSeconds;
    }
}
