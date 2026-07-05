using UnityEngine;

namespace AfterBlue.Data
{
    [CreateAssetMenu(menuName = "AfterBlue/Data/Rod Data", fileName = "RodData")]
    public sealed class RodData : ScriptableObject
    {
        [SerializeField] private string rodId;
        [SerializeField] private string displayName;
        [SerializeField] private int price = 20;
        [SerializeField] private float reelPower = 1f;
        [SerializeField] private float lineStability = 1f;
        [SerializeField] private float catchWindowBonus;

        public string RodId => rodId;
        public string DisplayName => displayName;
        public int Price => price;
        public float ReelPower => reelPower;
        public float LineStability => lineStability;
        public float CatchWindowBonus => catchWindowBonus;
    }
}
