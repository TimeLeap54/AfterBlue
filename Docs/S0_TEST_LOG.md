# S0 Baseline Test Log

## Environment

- Test Date: 2026-07-16
- Unity Version: 2022.3.62f3 (96770f904ca7)
- Render Pipeline: Universal Render Pipeline 14.0.12
- Main Scene: Assets/Scenes/FishingScene.unity
- Platform: Unity Editor Play Mode / Windows Standalone Development Build
- Resolution: Not locked yet
- Input Device: Keyboard and mouse
- Git Branch: codex/week4-fish-codex
- Git Commit: 3a1d5610b4ed923b65c0ab3d1a11044b340b9ee1
- Development Build: Builds/S0_Baseline/Windows/AfterBlue.exe
- Pre-S0 Backup: C:\Users\minil\GameMaking\AfterBlue_S0_Backup_20260716_192518
- Freeze Commit: Pending. Worktree had existing scene and asset changes before S0 documentation.

## Current State Snapshot

- Game executable from Unity: Pass, confirmed in Editor/Player smoke
- Boat movement: Pass, confirmed by manual S0 check
- Casting: Pass, confirmed by manual S0 check
- Bite occurrence: Pass, confirmed by manual S0 check
- Success: Pass, confirmed by manual S0 check
- Failure: Pass, confirmed by manual S0 check
- Collection registration: Implemented through `FishCollectionLog`
- Recasting: Pass after success and failure, confirmed by manual S0 check
- Save persistence: PlayerPrefs JSON; defensive load added during S0
- Console Error: 0 known after user manual validation and standalone smoke
- Console Warning: 0 unreviewed gameplay warnings reported

## S0 Automated Checks

| Check | Result | Notes |
|---|---|---|
| Unity script compilation | Pass | `Logs/codex-s0-compile-2.log`; no C# errors found |
| Windows development build | Pass | `Logs/codex-s0-dev-build-3.log`; `Build Finished, Result: Success.` |
| Standalone startup smoke | Pass | `Logs/codex-s0-player-smoke-2.log`; started and stopped after 15 seconds |
| Scene included in build settings | Pass | Build method explicitly includes `Assets/Scenes/FishingScene.unity` |

## 20-Run Manual Matrix

| Run | Environment | Start State | Input Condition | Expected Result | Actual Result | Pass/Fail | Bug ID |
|---:|---|---|---|---|---|---|---|
| 01 | Editor | Idle | Boat stopped, normal cast and hook | Success then recast possible | Confirmed by manual S0 check | Pass |  |
| 02 | Editor | Idle | Boat stopped, normal cast and hook | Reward granted once | Confirmed by manual S0 check | Pass |  |
| 03 | Editor | Idle | Boat stopped, normal cast and hook | Collection registered once | Confirmed by codex screenshot/manual S0 check | Pass |  |
| 04 | Editor | Idle | Catch same species again | Collection record updates without duplicate record | Counts increment in one row | Pass |  |
| 05 | Editor | Idle | Cast from another position | Loop completes normally | Confirmed by manual S0 check | Pass |  |
| 06 | Editor | Bite | No input during bite window | Miss then recast possible | Confirmed by manual S0 check | Pass |  |
| 07 | Editor | Waiting | Press action before bite | No reward before bite | Confirmed by manual S0 check | Pass |  |
| 08 | Editor | Bite | Press action immediately | Single success decision | Confirmed by manual S0 check | Pass |  |
| 09 | Editor | Bite | Press action near timeout | Only one result decision | Confirmed by manual S0 check | Pass |  |
| 10 | Editor | Hooked/Result | Repeated action input | Success and reward each occur once | Confirmed by manual S0 check | Pass |  |
| 11 | Editor | Casting | Move immediately after cast | Fishing state remains valid | Confirmed by manual S0 check | Pass |  |
| 12 | Editor | Waiting | Move while waiting | Bobber and line do not duplicate | Confirmed by manual S0 check | Pass |  |
| 13 | Editor | Bite | Move and hook | Inputs do not corrupt state | Confirmed by manual S0 check | Pass |  |
| 14 | Editor | Result | Move during result | Result can reset to Idle | Confirmed by manual S0 check | Pass |  |
| 15 | Editor | Any active state | Try recast while active | Recast is ignored until reset | Confirmed by manual S0 check | Pass |  |
| 16 | Editor | Idle | Fast recast repeated | No bobber or line accumulation | Confirmed by manual S0 check | Pass |  |
| 17 | Editor | Idle | Success and miss alternation | State resets after each cast | Confirmed by manual S0 check | Pass |  |
| 18 | Standalone | Idle | Catch then quit | Save data persists | Confirmed by manual S0 check | Pass |  |
| 19 | Standalone | New launch | Load existing data | Collection data is retained | Confirmed by manual S0 check | Pass |  |
| 20 | Standalone | Existing data | Catch new fish | Old and new data persist | Confirmed by manual S0 check | Pass |  |

## Per-Run Measurements

- Cast ID:
- Selected Fish ID:
- Result:
- Reward Count:
- Active Bobber Count:
- Active Line Count:
- Fishing State After Result:
- Recast Possible:
- Console Error:

## Known Warnings

None recorded from the normal standalone startup smoke.

Headless `-nographics` smoke was not used as a gate because URP shaders report unsupported-shader errors on Unity's Null graphics device. The normal Windows player smoke did not show those errors.
