using AfterBlue.Data;
using UnityEngine;

namespace AfterBlue.Fishing
{
    public static class FishRollTable
    {
        public static FishData Roll(FishData[] entries)
        {
            if (entries == null || entries.Length == 0)
            {
                return null;
            }

            float totalWeight = 0f;
            for (int i = 0; i < entries.Length; i++)
            {
                if (entries[i] != null)
                {
                    totalWeight += Mathf.Max(0.01f, entries[i].SpawnWeight);
                }
            }

            if (totalWeight <= 0f)
            {
                return entries[0];
            }

            float roll = Random.Range(0f, totalWeight);
            for (int i = 0; i < entries.Length; i++)
            {
                if (entries[i] == null)
                {
                    continue;
                }

                roll -= Mathf.Max(0.01f, entries[i].SpawnWeight);
                if (roll <= 0f)
                {
                    return entries[i];
                }
            }

            return entries[entries.Length - 1];
        }
    }
}
