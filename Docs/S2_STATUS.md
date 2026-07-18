# S2 Status

S2 is now on the Map_01 large-map readability pass.

The current milestone is not final art placement. It is the Week 6 Map_01 test scene for map size, boat speed, landmark scale, and zone distance.

This corresponds to Week 6 Map_01 in `MAP_01_FULL_PIPELINE.md`: W6-A promotes the active candidate to 640 x 420m after rejecting 192 x 128m as still too compact.

## Current Builder

Use:

`AfterBlue > Setup > Apply Week 6 Map 01`

This creates:

`Assets/AfterBlue/Scenes/Map_01/Map_01_Week6.unity`

## Current Test

- P0 scale rules are recorded in `MAP_01_P0_SPEC.md`.
- W6-A large-map scale candidate is active.
- Gameplay area: 640 x 420m
- Water visual plane: 900 x 600m
- Unity scale: 1 unit = 1 meter
- Water height: Y 0
- Boat reference cube: 1.8m wide x 4.5m long
- Boat forward speed: 9.5m/s
- Boat reverse speed: 4.1m/s
- Node markers: S, A, M, B, C, D
- Route markers: S-A, S-D, A-M, A-B, M-B, M-D, B-C, C-D
- Measurement markers: 0m, 50m, 100m
- Week 6 replaces small yellow dots with large color-coded area fields.
- Week 6 adds Start/H1/M/H2/H3/D proxy clusters for camera readability.

## Not Locked

- Final map size is not locked yet.
- Final area art is not locked yet.
- 96 x 64m was rejected as too small-feeling in the first visual check.
- 112 x 72m was also rejected as too small-feeling after user manual scale testing.
- 192 x 128m was rejected as still too compact after the Week6 readability check.
- 640 x 420m is the current W6-A candidate, not final locked scale.
- 7.2m/s forward and 3.2m/s reverse were rejected as too slow-feeling for the large map.
- 9.5m/s forward and 4.1m/s reverse are the current W6-D movement candidates.
- Final Blender props and real water shader are deferred until after the readability test.
- H1/H2/H3 landmark art is deferred until the map size and movement metrics are measured.

## Required Measurements

- Current boat real size
- 100m straight travel time
- Average movement speed
- Comfortable U-turn diameter
- Stop distance
- Horizontal crossing time
- Vertical crossing time
- Empty outer loop time
- Keep 640 x 420 / expand / shrink decision

## Next

Open the Week 6 scene, play it with the current boat camera, and record whether H1/H2/H3/Start read as real destinations at 640 x 420m.
