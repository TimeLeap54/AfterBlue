# AfterBlue Roadmap v1.0

This roadmap supersedes the old Week 0-4 task list. It follows the final game design v1.0 and treats Map_01 as the only launch map.

## Completed / Current Foundation

- [x] Unity 2022 URP project exists.
- [x] Git repository exists.
- [x] Week 04 baseline fishing scene exists.
- [x] S0 baseline stabilization docs exist.
- [x] S1 fishing behavior lock candidate exists.
- [x] Week 6 Map_01 builder exists.
- [x] Map_01 scale test is currently on the 640 x 420m W6-A candidate after rejecting 96 x 64m, 112 x 72m, and 192 x 128m as too small-feeling.

## S0 - Week 04 Baseline Stabilization

Goal: preserve a known stable build before expanding the game.

- [x] Verify boat movement, fishing, codex, and saved records.
- [x] Remove blocking console errors.
- [x] Preserve baseline docs.

## S1 - Fishing Experience Lock

Goal: make the one-button Hold/Release fishing loop understandable and repeatable.

- [x] Preserve Week4 fishing loop.
- [x] Add behavior-pattern data.
- [x] Hide fish identity before catch.
- [x] Tune distance, stress, integrity, landing, and failure flow.
- [x] Document fishing behavior grammar.

Remaining later polish:

- [ ] Add stronger fish visual telegraphs.
- [ ] Reduce reliance on debug UI.
- [ ] Log fight timing and failure reasons.

## S2 / Week6 - Map_01 Size And Readability Test

Goal: verify the launch-map size and boat movement metrics before art, props, water shader, or obstacle placement.

- [x] Add `AfterBlue > Setup > Apply Week 6 Map 01`.
- [x] Create scene path `Assets/AfterBlue/Scenes/Map_01/Map_01_Week6.unity`.
- [x] Create role-based hierarchy: system, water, guides, zone markers, route guides, obstacle blockout, boundary, landmarks, fishing zones, debug.
- [x] Record P0 scale rules: Unity 1 unit = 1m, Blender 1m = Unity 1 unit, Unity water height Y 0, Blender water height Z 0.
- [x] Add P0 spec marker object under `00_SYSTEM` in the Week 6 builder.
- [x] Create debug materials under `Assets/AfterBlue/Materials/Debug`.
- [x] Reject the first 96 x 64m view check as too small-feeling.
- [x] Create 900 x 600m water plane.
- [x] Create 640 x 420m gameplay guide.
- [x] Add four corner markers at X -320/320 and Z -210/210.
- [x] Add 0m/50m/100m speed markers.
- [x] Replace raw scaled S/A/M/B/C/D markers with direct W6-B/C layout coordinates.
- [x] Reject 7.2m/s forward and 3.2m/s reverse as too slow-feeling for the large-map test.
- [x] Set W6-D boat speed candidate to 9.5m/s forward and 4.1m/s reverse.
- [x] Add edge crossing markers for horizontal and vertical travel checks.
- [x] Add primary and support route guides between S/A/M/B/C/D.
- [x] Set main loop to Start-H1-M-H2-H3-D-Start.
- [x] Set support routes to Start-M, H1-H2, and M-D.
- [x] Replace small yellow zone dots with larger color-coded Start/H1/M/H2/H3/D fields in Week 6.
- [x] Add Week5-style proxy language to Week 6: supply buoy, roofs, submerged roads, tilted traffic light, utility poles, debris, vegetation, ripple rings.
- [ ] Regenerate and open `Map_01_Week6.unity` in Unity.
- [ ] Measure current boat real size against the 1.8 x 4.5m reference cube.
- [ ] Measure 100m straight travel time.
- [ ] Measure average movement speed.
- [ ] Measure comfortable U-turn diameter.
- [ ] Measure stop distance.
- [ ] Measure horizontal crossing time.
- [ ] Measure vertical crossing time.
- [ ] Measure empty outer loop time.
- [ ] Measure main loop time from Start through H1/M/H2/H3/D back to Start.
- [ ] Check whether Start, H1, H2, H3, M, and D read as large zones from the gameplay camera.
- [ ] Check whether H2 is the strongest landmark candidate.
- [ ] Check whether central water still feels open enough after proxy placement.
- [ ] Decide 640 x 420 keep / expand / shrink.

Pass criteria:

- Movement metrics are recorded as actual numbers.
- 640 x 420m is either kept or rejected for a concrete reason.
- No Blender props, free assets, real water shader, house production, background production, or detailed obstacles are added during this stage.

## S3 / Week7 - Map_01 Hero Layout

Goal: add representative hero silhouettes directly on Map_01, not in a detached art lab.

- [ ] Start supply buoy hero blockout.
- [ ] H1 representative flooded house/roof silhouette.
- [ ] H2 tilted traffic light and crosswalk hero layout.
- [ ] H3 large deep debris silhouette.
- [ ] Distant background silhouettes.
- [ ] Confirm no hero blockout blocks normal boating.

Pass criteria:

- H1/H2/H3 are readable from navigation camera.
- H2 works as the representative screenshot composition.
- H3 feels deep, not horror.
- Start remains visually simple.

## S4 / Week8 - Modular Kit Production

Goal: build the minimum modular asset kit needed to populate Map_01.

Priority:

1. Traffic light
2. Road straight / curve / intersection
3. Crosswalk
4. Utility pole
5. Roof and wall-top pieces
6. Weed and rock clusters
7. Pipes, metal frames, plank debris
8. Signs and small props

Pass criteria:

- At least 8-10 modular base assets exist.
- Variants can be made through scale, tilt, color, moss/rust decals, and damage variation.
- Blender -> Unity export loop is stable.

## S5 / Week9 - H2 Environment Vertical Slice

Goal: H2 should look like AfterBlue before the whole map is populated.

- [ ] H2 traffic light model or controlled proxy.
- [ ] H2 road/intersection/crosswalk composition.
- [ ] Boat, angler, bobber, fish shadow visible in representative camera.
- [ ] First water/material/lighting pass.
- [ ] Screenshot and 10-second GIF candidate.

Pass criteria:

- Without explanation, viewers can read flooded modern village, boat, fishing, calm/melancholic mood.

## S6 / Week10 - Fish Habitat And Generation

Goal: connect fishing results to bobber landing position, habitat, depth, bait, progress, local activity, and duplicate protection.

- [ ] Ambient fish visual pool.
- [ ] Habitat influence blending.
- [ ] Catch candidate selection.
- [ ] Fish shadow spawn from plausible approach points.
- [ ] Local activity decay.
- [ ] Duplicate protection.

Pass criteria:

- Fishing location and bait change results.
- Repeated fishing in one spot is allowed but gently discouraged.
- Habitat boundaries do not feel like hard invisible lines.

## S7 / Week11 - Core Session Loop Greybox

Goal: the full game loop works before final art pass.

- [ ] Depart from supply buoy.
- [ ] Fish and fill 6-slot storage.
- [ ] Return to buoy.
- [ ] Register aquarium entries.
- [ ] Sell duplicates.
- [ ] Buy bait.
- [ ] Unlock next habitat clue.
- [ ] Save/load session state.

Pass criteria:

- New fish registration, duplicate sale, bait purchase, and next-zone incentive all work.

## S8 / Week12 - Map_01 Population

Goal: populate the whole launch map while preserving navigation and fishing readability.

Order:

1. H2
2. H1
3. H3
4. Central water cleanup
5. Outer silhouettes

Pass criteria:

- Three habitats are visually distinct.
- Most water remains fishable.
- Repeated assets are not too obvious.
- Bobber and fish shadows are not buried under props.

## S9 / Week13 - Progression, Journal, Legendary Fish, Ending

Goal: make the 20-40 minute run complete.

- [ ] Four voyage journal entries.
- [ ] Special bait progression.
- [ ] Legendary fish condition.
- [ ] Legendary fight.
- [ ] Final aquarium registration.
- [ ] Return ending.

Pass criteria:

- A player can reach the ending without explicit external help.
- Progression does not become grind.

## S10 / Week14 - Balance, QA, Steam Release Candidate

Feature freeze:

- No new maps.
- No new core systems.
- No free camera.
- No full water shader rewrite.
- No full hero asset redesign.

Measure:

- First cast time.
- First catch time.
- First return time.
- First H2 visit.
- First H3 visit.
- Legendary encounter time.
- Ending time.
- Failure reasons.

Targets:

- First cast within 30 seconds.
- First catch within 90 seconds.
- H1 three species within 12 minutes.
- H2 mutated fish within 22 minutes.
- Deep bait within 30 minutes.
- Ending within 20-40 minutes.
