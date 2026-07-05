# AfterBlue Data Schema

Unity ScriptableObjects are the default data format.

## FishData

- FishId
- DisplayName
- Rarity
- BasePrice
- MinSize
- MaxSize
- PreferredBait
- LocationIds
- CatchDifficulty
- SpawnWeight
- BiteWindow
- JournalText
- ModelPrefab
- IconSprite

## BaitData

- BaitId
- DisplayName
- Price
- RarityBonus
- TargetFishTags

## RodData

- RodId
- DisplayName
- Price
- ReelPower
- LineStability
- CatchWindowBonus

## LocationData

- LocationId
- DisplayName
- Description
- SceneName
- BackgroundPrefab
- AvailableFishIds
- RequiredProgress
- AmbientColor
- MusicTrack
- JournalEntries

## SaveData

- Money
- UnlockedLocations
- OwnedRods
- OwnedBaits
- CaughtFishRecords
- UnlockedJournalEntries
- CurrentRod
- PlayTime

## Week 4 Prototype Save

The current Week 4 fish codex prototype uses `PlayerPrefs` with JSON under
`AfterBlue.FishCollectionLog.v1`. Move this into the final SaveSystem when
shop, money, and progression data are implemented.
