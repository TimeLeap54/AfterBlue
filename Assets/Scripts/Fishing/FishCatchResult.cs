namespace AfterBlue.Fishing
{
    public readonly struct FishCatchResult
    {
        public static readonly FishCatchResult None = new FishCatchResult(false, false, string.Empty, string.Empty, 0f);
        public static readonly FishCatchResult Missed = new FishCatchResult(true, false, string.Empty, string.Empty, 0f);

        public FishCatchResult(bool hasResult, bool success, string fishName, string rarity, float sizeCm)
        {
            HasResult = hasResult;
            Success = success;
            FishName = fishName;
            Rarity = rarity;
            SizeCm = sizeCm;
        }

        public bool HasResult { get; }
        public bool Success { get; }
        public string FishName { get; }
        public string Rarity { get; }
        public float SizeCm { get; }

        public static FishCatchResult Caught(string fishName, string rarity, float sizeCm)
        {
            return new FishCatchResult(true, true, fishName, rarity, sizeCm);
        }
    }
}
