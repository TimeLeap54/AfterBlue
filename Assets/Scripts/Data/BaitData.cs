using UnityEngine;

namespace AfterBlue.Data
{
    [CreateAssetMenu(menuName = "AfterBlue/Data/Bait Data", fileName = "BaitData")]
    public sealed class BaitData : ScriptableObject
    {
        [SerializeField] private string baitId;
        [SerializeField] private string displayName;
        [SerializeField] private int price = 5;
        [SerializeField] private float rarityBonus;
        [SerializeField] private string[] targetFishTags;

        public string BaitId => baitId;
        public string DisplayName => displayName;
        public int Price => price;
        public float RarityBonus => rarityBonus;
        public string[] TargetFishTags => targetFishTags;
    }
}
