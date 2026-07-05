using AfterBlue.Data;
using AfterBlue.Journal;
using UnityEngine;

namespace AfterBlue.UI
{
    public sealed class FishCodexDebugUI : MonoBehaviour
    {
        [SerializeField] private FishCollectionLog collectionLog;
        [SerializeField] private bool visible;

        private void Awake()
        {
            if (collectionLog == null)
            {
                collectionLog = GetComponent<FishCollectionLog>();
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.J))
            {
                visible = !visible;
            }

            if (visible && Input.GetKeyDown(KeyCode.F9))
            {
                collectionLog?.Clear();
            }
        }

        private void OnGUI()
        {
            if (!visible || collectionLog == null)
            {
                return;
            }

            Rect rect = new Rect(Screen.width - 390f, 42f, 370f, 420f);
            GUI.Box(rect, "Fish Codex");

            GUILayout.BeginArea(new Rect(rect.x + 12f, rect.y + 28f, rect.width - 24f, rect.height - 40f));
            GUILayout.Label("Tab / J: Close    F9: Clear test records");
            GUILayout.Space(8f);

            FishData[] fish = collectionLog.KnownFish;
            for (int i = 0; i < fish.Length; i++)
            {
                if (fish[i] == null)
                {
                    continue;
                }

                FishCollectionRecord record = collectionLog.GetRecord(fish[i].FishId);
                if (record == null)
                {
                    GUILayout.Label($"???        {fish[i].Rarity}");
                    continue;
                }

                GUILayout.Label($"{record.DisplayName}  x{record.CaughtCount}  {record.LargestSizeCm:0.0} cm  {record.Rarity}");
            }

            GUILayout.EndArea();
        }
    }
}
