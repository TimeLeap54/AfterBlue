# S0 Bug Log

## S0-BUG-001

### Title
Fish collection save load was not defensive against empty or malformed JSON.

### Priority
P0

### Environment
- Scene: Assets/Scenes/FishingScene.unity
- Build: Editor / Standalone
- Commit: 3a1d5610b4ed923b65c0ab3d1a11044b340b9ee1 plus S0 local changes

### Reproduction Steps
1. Store an empty or malformed value in `AfterBlue.FishCollectionLog.v1`.
2. Launch the scene.
3. Trigger collection load.

### Expected
The game starts with empty collection data and remains playable.

### Actual
The previous implementation did not guarantee `saveData.records` would be initialized after malformed or partial data.

### Reproduction Rate
Static risk found during S0 code inspection.

### Evidence
`Assets/Scripts/Journal/FishCollectionLog.cs`

### Root Cause
`JsonUtility.FromJson` result and nested list were not fully normalized.

### Fix
Added empty-string handling, try/catch around load, and list normalization.

### Regression Test
- Save data missing:
- Save data empty:
- Save data malformed:

## S0-BUG-002

### Title
Fish codex debug input could throw when legacy input is unavailable.

### Priority
P1

### Environment
- Scene: Assets/Scenes/FishingScene.unity
- Build: Editor / Standalone
- Commit: 3a1d5610b4ed923b65c0ab3d1a11044b340b9ee1 plus S0 local changes

### Reproduction Steps
1. Run the project with an input configuration where legacy `Input.GetKeyDown` throws.
2. Let `FishCodexDebugUI.Update` execute.

### Expected
Debug UI input is ignored without throwing.

### Actual
The previous implementation called `Input.GetKeyDown` directly.

### Reproduction Rate
Static risk found during S0 code inspection.

### Evidence
`Assets/Scripts/UI/FishCodexDebugUI.cs`

### Root Cause
The UI did not use the same defensive input wrapper used by other gameplay scripts.

### Fix
Added `SafeGetKeyDown`.

### Regression Test
- Editor Play Mode with current input config:
- Standalone Development Build:

## S0-BUG-003

### Title
Runtime material creation could throw if a shader lookup returned null.

### Priority
P1

### Environment
- Scene: Assets/Scenes/FishingScene.unity
- Build: Windows Development Build
- Commit: 3a1d5610b4ed923b65c0ab3d1a11044b340b9ee1 plus S0 local changes

### Reproduction Steps
1. Run the standalone player in a context where runtime `Shader.Find` cannot resolve a shader.
2. Let `FishingLine.Awake` execute.

### Expected
Missing runtime shader lookup does not throw an exception.

### Actual
`new Material(null)` caused `ArgumentNullException`.

### Reproduction Rate
1/1 in headless player smoke.

### Evidence
`Logs/codex-s0-player-smoke.log`

### Root Cause
Runtime material creation assumed `Shader.Find` always returns a shader included in the player.

### Fix
Guarded runtime material creation in `FishingLine`, `Bobber`, `WaterRipple`, and prototype reference marker creation.

### Regression Test
- Windows player smoke without `-nographics`: Pass
- Manual cast and bobber/ripple visual check:
