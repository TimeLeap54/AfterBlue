using AfterBlue.Data;
using AfterBlue.Fishing;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AfterBlue.Journal
{
    [Serializable]
    public sealed class FishCollectionRecord
    {
        [SerializeField] private string fishId;
        [SerializeField] private string displayName;
        [SerializeField] private FishRarity rarity;
        [SerializeField] private int caughtCount;
        [SerializeField] private float largestSizeCm;

        public string FishId => fishId;
        public string DisplayName => displayName;
        public FishRarity Rarity => rarity;
        public int CaughtCount => caughtCount;
        public float LargestSizeCm => largestSizeCm;

        public FishCollectionRecord(string fishId, string displayName, FishRarity rarity, float sizeCm)
        {
            this.fishId = fishId;
            this.displayName = displayName;
            this.rarity = rarity;
            caughtCount = 1;
            largestSizeCm = sizeCm;
        }

        public void Register(float sizeCm)
        {
            caughtCount++;
            largestSizeCm = Mathf.Max(largestSizeCm, sizeCm);
        }
    }

    [Serializable]
    public sealed class FishCollectionSaveData
    {
        public List<FishCollectionRecord> records = new List<FishCollectionRecord>();
    }

    public sealed class FishCollectionLog : MonoBehaviour
    {
        private const string PlayerPrefsKey = "AfterBlue.FishCollectionLog.v1";

        [SerializeField] private FishData[] knownFish;
        [SerializeField] private FishCollectionSaveData saveData = new FishCollectionSaveData();
        [SerializeField] private bool loadOnAwake = true;

        public FishData[] KnownFish => knownFish ?? Array.Empty<FishData>();
        public IReadOnlyList<FishCollectionRecord> Records => saveData.records;

        private void Awake()
        {
            if (loadOnAwake)
            {
                Load();
            }
        }

        public void RegisterCatch(FishCatchResult result)
        {
            if (!result.HasResult || !result.Success || string.IsNullOrWhiteSpace(result.FishId))
            {
                return;
            }

            FishCollectionRecord record = saveData.records.Find(item => item.FishId == result.FishId);
            if (record == null)
            {
                saveData.records.Add(new FishCollectionRecord(result.FishId, result.FishName, result.Rarity, result.SizeCm));
            }
            else
            {
                record.Register(result.SizeCm);
            }

            Save();
            Debug.Log($"Fish registered: {result.FishName} / {result.SizeCm:0.0} cm");
        }

        public bool HasCaught(string fishId)
        {
            return saveData.records.Exists(item => item.FishId == fishId);
        }

        public FishCollectionRecord GetRecord(string fishId)
        {
            return saveData.records.Find(item => item.FishId == fishId);
        }

        public void Save()
        {
            PlayerPrefs.SetString(PlayerPrefsKey, JsonUtility.ToJson(saveData));
            PlayerPrefs.Save();
        }

        public void Load()
        {
            if (!PlayerPrefs.HasKey(PlayerPrefsKey))
            {
                saveData = new FishCollectionSaveData();
                return;
            }

            string json = PlayerPrefs.GetString(PlayerPrefsKey);
            saveData = JsonUtility.FromJson<FishCollectionSaveData>(json) ?? new FishCollectionSaveData();
        }

        public void Clear()
        {
            saveData = new FishCollectionSaveData();
            PlayerPrefs.DeleteKey(PlayerPrefsKey);
        }
    }
}
