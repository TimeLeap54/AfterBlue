using UnityEngine;

namespace AfterBlue.Fishing
{
    public readonly struct FishRollEntry
    {
        public FishRollEntry(string displayName, string rarity, float weight, float biteWindow, float minSize, float maxSize)
        {
            DisplayName = displayName;
            Rarity = rarity;
            Weight = weight;
            BiteWindow = biteWindow;
            MinSize = minSize;
            MaxSize = maxSize;
        }

        public string DisplayName { get; }
        public string Rarity { get; }
        public float Weight { get; }
        public float BiteWindow { get; }
        public float MinSize { get; }
        public float MaxSize { get; }
    }

    public static class FishRollTable
    {
        private static readonly FishRollEntry[] Entries =
        {
            new FishRollEntry("Rust Minnow", "Common", 70f, 1.3f, 8f, 16f),
            new FishRollEntry("Glass Scale", "Uncommon", 25f, 1.1f, 14f, 24f),
            new FishRollEntry("Blue Wraith", "Rare", 5f, 0.8f, 22f, 38f)
        };

        public static FishRollEntry Roll()
        {
            float totalWeight = 0f;
            for (int i = 0; i < Entries.Length; i++)
            {
                totalWeight += Entries[i].Weight;
            }

            float roll = Random.Range(0f, totalWeight);
            for (int i = 0; i < Entries.Length; i++)
            {
                roll -= Entries[i].Weight;
                if (roll <= 0f)
                {
                    return Entries[i];
                }
            }

            return Entries[Entries.Length - 1];
        }
    }
}
