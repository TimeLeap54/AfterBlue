# S2 Status

S2 / Week6 is closed as the Map_01 spatial foundation candidate.

The milestone was not final art placement. It locked the Week 6 Map_01 candidate for map size, boat speed, landmark scale, and zone distance so the next pass can focus on water, submerged readability, and assets.

This corresponds to Week 6 Map_01 in `MAP_01_FULL_PIPELINE.md`: W6-A promotes the active candidate to 640 x 420m after rejecting 192 x 128m as still too compact.

## Current Builder

Use:

`AfterBlue > Setup > Apply Week 6 Map 01`

This creates:

`Assets/AfterBlue/Scenes/Map_01/Map_01_Week6.unity`

## Locked Candidate

- P0 scale rules are recorded in `MAP_01_P0_SPEC.md`.
- W6-A large-map scale candidate is active.
- Gameplay area: 640 x 420m
- Water visual plane: 900 x 600m
- Unity scale: 1 unit = 1 meter
- Water height: Y 0
- Boat reference cube: 1.8m wide x 4.5m long
- Boat forward speed: 9.5m/s
- Boat reverse speed: 4.1m/s
- Node markers: Start, H1, M, H2, H3, D
- Main route: Start-H1-M-H2-H3-D-Start
- Support routes: Start-M, H1-H2, M-D
- Measurement markers: 0m, 50m, 100m
- Week 6 replaces small yellow dots with large color-coded area fields.
- Week 6 adds Start/H1/M/H2/H3/D proxy clusters for camera readability.

## Deferred To Week7

- Final map size is accepted as the next-pass candidate, not as a final shipping lock.
- Final area art is not locked yet.
- 96 x 64m was rejected as too small-feeling in the first visual check.
- 112 x 72m was also rejected as too small-feeling after user manual scale testing.
- 192 x 128m was rejected as still too compact after the Week6 readability check.
- 640 x 420m is the Week6 locked candidate for the next pass.
- 7.2m/s forward and 3.2m/s reverse were rejected as too slow-feeling for the large map.
- 9.5m/s forward and 4.1m/s reverse are the Week6 movement candidates.
- W6-B/C replaces raw 6.0x scaled nodes with direct layout coordinates for a clearer travel loop.
- Final Blender props and real water shader are deferred to Week7+.
- H1/H2/H3 landmark art is deferred to Week7+.
- Underwater object height, visibility, and water occlusion are deferred to Week7.

## Week6 Closure Checks

- Manual large-map loop check: accepted qualitatively.
- User timing note: slow baseline felt close to a 5 minute loop.
- Movement decision: 9.5m/s forward feels better for the 640 x 420m candidate.
- Exact stopwatch table remains useful, but is not blocking Week6 closure.
- Keep / expand / shrink decision: keep 640 x 420m as the next-pass candidate.

## Next

Start Week7 from this scene and solve water/depth/submerged readability before committing to final H1/H2/H3 art assets.
