# S2 Status

S2 is now on the Map_01 large-map readability pass.

The current milestone is not final art placement. It is the Week 6 Map_01 test scene for map size, boat speed, landmark scale, and zone distance.

This corresponds to Week 6 Map_01 in `MAP_01_FULL_PIPELINE.md`: the 192 x 128m candidate is active and the Week 6 scene adds large readable habitat fields plus Week5-style proxies.

## Current Builder

Use:

`AfterBlue > Setup > Apply Week 6 Map 01`

This creates:

`Assets/AfterBlue/Scenes/Map_01/Map_01_Week6.unity`

## Current Test

- P0 scale rules are recorded in `MAP_01_P0_SPEC.md`.
- P1 large-map speed candidate is active.
- Gameplay area: 192 x 128m
- Water visual blockout: 280 x 192m
- Unity scale: 1 unit = 1 meter
- Water height: Y 0
- Boat reference cube: 1.8m wide x 4.5m long
- Boat forward speed: 7.2m/s
- Boat reverse speed: 3.2m/s
- Node markers: S, A, M, B, C, D
- Route markers: S-A, S-D, A-M, A-B, M-B, M-D, B-C, C-D
- Measurement markers: 0m, 10m, 20m
- Week 6 replaces small yellow dots with large color-coded area fields.
- Week 6 adds Start/H1/M/H2/H3/D proxy clusters for camera readability.

## Not Locked

- Final map size is not locked yet.
- Final area art is not locked yet.
- 96 x 64m was rejected as too small-feeling in the first visual check.
- 112 x 72m was also rejected as too small-feeling after user manual scale testing.
- 192 x 128m is the current P1 test candidate, not final locked scale.
- Final Blender props and real water shader are deferred until after the readability test.
- H1/H2/H3 landmark art is deferred until the map size and movement metrics are measured.

## Required Measurements

- Current boat real size
- 20m straight travel time
- Average movement speed
- Comfortable U-turn diameter
- Stop distance
- Horizontal crossing time
- Vertical crossing time
- Empty outer loop time
- Keep 192 x 128 / expand / shrink decision

## Next

Open the Week 6 scene, play it with the current boat camera, and record whether H1/H2/H3/Start read as real destinations at 192 x 128m.
