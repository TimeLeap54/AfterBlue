# MAP_01 Ecology Lock

S2 ecology uses bobber landing position as the future selection source. Boat position must not become the final fish roll source.

## Habitat Start Values

- H1 Shallow Residential: core 18, blend 42, shallow
- H2 Traffic Light: core 21, blend 45, mid
- H3 Deep Debris: core 24, blend 51, deep
- Central water: common fish and practice area, no rare/strong reward focus

## Proxy Fish

- `common_fish`: all water, central higher, S1 calm pattern
- `h1_fish`: shallow/H1, S1 calm or dart
- `h2_fish`: mid/traffic/road, S1 dart or pulse
- `h3_fish`: deep/H3, S1 heavy

## Weight Formula Target

Final S2 roll should follow:

`base fish weight * habitat suitability * depth suitability * landmark proximity * bait * progress`

For S2, bait and progress stay fixed at 1.0.
