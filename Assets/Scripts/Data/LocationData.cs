using UnityEngine;

namespace AfterBlue.Data
{
    [CreateAssetMenu(menuName = "AfterBlue/Data/Location Data", fileName = "LocationData")]
    public sealed class LocationData : ScriptableObject
    {
        [SerializeField] private string locationId;
        [SerializeField] private string displayName;
        [TextArea]
        [SerializeField] private string description;
        [SerializeField] private string sceneName;
        [SerializeField] private GameObject backgroundPrefab;
        [SerializeField] private string[] availableFishIds;
        [SerializeField] private int requiredProgress;
        [SerializeField] private Color ambientColor = new Color(0.2f, 0.7f, 0.8f);
        [SerializeField] private AudioClip musicTrack;
        [SerializeField] private string[] journalEntryIds;

        public string LocationId => locationId;
        public string DisplayName => displayName;
        public string Description => description;
        public string SceneName => sceneName;
        public GameObject BackgroundPrefab => backgroundPrefab;
        public string[] AvailableFishIds => availableFishIds;
        public int RequiredProgress => requiredProgress;
        public Color AmbientColor => ambientColor;
        public AudioClip MusicTrack => musicTrack;
        public string[] JournalEntryIds => journalEntryIds;
    }
}

