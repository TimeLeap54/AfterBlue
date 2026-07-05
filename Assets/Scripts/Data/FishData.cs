using UnityEngine;

namespace AfterBlue.Data
{
    public enum FishRarity
    {
        Common,
        Uncommon,
        Rare,
        Mutated,
        Legendary
    }

    [CreateAssetMenu(menuName = "AfterBlue/Data/Fish Data", fileName = "FishData")]
    public sealed class FishData : ScriptableObject
    {
        [SerializeField] private string fishId;
        [SerializeField] private string displayName;
        [SerializeField] private FishRarity rarity;
        [SerializeField] private int basePrice = 10;
        [SerializeField] private float minSize = 10f;
        [SerializeField] private float maxSize = 30f;
        [SerializeField] private string preferredBaitId;
        [SerializeField] private string[] locationIds;
        [SerializeField] private float catchDifficulty = 1f;
        [SerializeField] private float spawnWeight = 10f;
        [SerializeField] private float biteWindow = 1.2f;
        [TextArea]
        [SerializeField] private string journalText;
        [SerializeField] private GameObject modelPrefab;
        [SerializeField] private Sprite iconSprite;

        public string FishId => fishId;
        public string DisplayName => displayName;
        public FishRarity Rarity => rarity;
        public int BasePrice => basePrice;
        public float MinSize => minSize;
        public float MaxSize => maxSize;
        public string PreferredBaitId => preferredBaitId;
        public string[] LocationIds => locationIds;
        public float CatchDifficulty => catchDifficulty;
        public float SpawnWeight => spawnWeight;
        public float BiteWindow => biteWindow;
        public string JournalText => journalText;
        public GameObject ModelPrefab => modelPrefab;
        public Sprite IconSprite => iconSprite;
    }
}
