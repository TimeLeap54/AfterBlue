# AfterBlue S0 Baseline Report

## 1. Current Core Loop

Game start -> boat movement -> cast -> bobber lands -> waiting for bite -> Bite input or timeout -> caught or missed result -> collection registration on success -> save -> reset to Idle -> recast.

## 2. Stable Functions

- Fishing loop is implemented in `FishingStateMachine`.
- Bobber lifecycle is owned by `FishingCastController`.
- Fishing line display is centralized in `FishingLine`.
- Collection records are stored by `FishCollectionLog`.
- Fish data and collection data are configured by Week 4 editor setup code.

## 3. Known Functional Limitations

- `Hooked` is currently a very short transition state.
- Reset from `Result` currently requires another action input.
- Collection persistence uses PlayerPrefs JSON rather than an explicit save file.
- Full 20-run manual matrix passed by user manual validation.
- A Windows Development Build exists and starts without filtered Error/Exception logs in a 15-second smoke run.
- Final freeze commit and tag are pending because the working tree contained unrelated pre-existing changes.

## 4. Known Experience Problems

- Hooked state ends almost immediately.
- There is no reeling process.
- Waiting feedback is minimal.
- Fish-specific resistance differences are not implemented.

## 5. Technical Debt Deferred

- Split debug UI and test controls more cleanly from production UI.
- Replace `PlayerPrefs` persistence with an explicit save file when save architecture starts.
- Add an automated Play Mode smoke test for success, miss, and recast.

## 6. S1 Inputs

- Current cast time: 0.45s from `FishingCastController.castDuration`
- Current bite wait time: 2.0s to 5.0s from `FishingStateMachine`
- Current Bite Window: FishData-specific, Week 4 data ranges approximately 0.7s to 1.35s
- Current result display time: Manual reset from `Result`
- Current total fishing time: Pending precise measurement for S1 tuning

## 7. Final Status

- P0: 0 known
- P1: 0 known
- P2: 0 blocking
- Console Errors: 0 known in manual validation and normal standalone startup smoke
- Test Runs Passed: 20/20 manual plus automated compile/build/startup smoke
- Standalone Build Passed: Yes, startup smoke only
- S0 Gate: Pass, commit/tag still pending
