using AfterBlue.Data;

namespace AfterBlue.Fishing
{
    public readonly struct FishCatchResult
    {
        public static readonly FishCatchResult None = new FishCatchResult(false, false, string.Empty, string.Empty, FishRarity.Common, 0f, 0);
        public static readonly FishCatchResult Missed = new FishCatchResult(true, false, string.Empty, string.Empty, FishRarity.Common, 0f, 0);

        public FishCatchResult(bool hasResult, bool success, string fishId, string fishName, FishRarity rarity, float sizeCm, int basePrice)
        {
            HasResult = hasResult;
            Success = success;
            FishId = fishId;
            FishName = fishName;
            Rarity = rarity;
            SizeCm = sizeCm;
            BasePrice = basePrice;
        }

        public bool HasResult { get; }
        public bool Success { get; }
        public string FishId { get; }
        public string FishName { get; }
        public FishRarity Rarity { get; }
        public float SizeCm { get; }
        public int BasePrice { get; }

        public static FishCatchResult Caught(FishData fishData, float sizeCm)
        {
            return new FishCatchResult(
                true,
                true,
                fishData.FishId,
                fishData.DisplayName,
                fishData.Rarity,
                sizeCm,
                fishData.BasePrice);
        }
    }
}
