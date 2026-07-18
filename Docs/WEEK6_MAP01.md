# Week 6 Map_01

Week 6 defines the current Map_01 playable layout pass.

## Unity Menu

Use:

`AfterBlue > Setup > Apply Week 6 Map 01`

This creates:

`Assets/AfterBlue/Scenes/Map_01/Map_01_Week6.unity`

## Goal

Make the player camera read clear destinations on the large map before final art, real water, or asset production.

## Active Rules

- W6-A active gameplay area: 640 x 420m.
- W6-A active water plane: 900 x 600m.
- Treat 192 x 128m as a rejected small-map reference frame, not the current candidate.
- W6-D active boat speed: 9.5m/s forward and 4.1m/s reverse.
- Treat 7.2m/s forward and 3.2m/s reverse as the rejected slow baseline for 640 x 420m.
- Replace small yellow point markers with large color-coded habitat fields.
- Reuse Week5 visual language as proxy structure: roof, road, traffic light, debris, buoy, ripple cues.
- Do not treat this as final art.

## W6-A Scale Segment

Goal: make Map_01 large enough that Start/H1/M/H2/H3/D read as separate travel destinations instead of compact points on one board.

- Active candidate: 640 x 420m gameplay area.
- Water support plane: 900 x 600m.
- W6-B/C uses direct designed coordinates instead of raw scaled planning coordinates.
- Rejected reference frame: 192 x 128m remains visible as a comparison guide.
- Measurement markers use a 100m line instead of the old 20m test line.

## W6-D Movement Segment

Goal: match boat speed to the 640 x 420m map without making the world feel small again.

- Active forward speed: 9.5m/s.
- Active reverse speed: 4.1m/s.
- Active acceleration: 14.0.
- Active deceleration: 11.0.
- Rejected slow baseline: 7.2m/s forward, 3.2m/s reverse.
- User loop check: about 5 minutes at the slower baseline, with 9.5m/s feeling better.

## W6-B/C Zone And Loop Segment

Goal: make Map_01 read as a playable travel loop, not a compact board of nearby points.

Active node coordinates:

- Start / Supply: (-270, -165)
- H1 Shallow Residential: (-230, 135)
- M Central Open Water: (-30, 20)
- H2 Traffic Light: (235, 120)
- H3 Deep Debris: (250, -155)
- D Return Water: (-90, -170)

Main loop:

`Start -> H1 -> M -> H2 -> H3 -> D -> Start`

Support routes:

- `Start -> M`
- `H1 -> H2`
- `M -> D`

## Zone Color Language

- Start / Supply: warm orange.
- H1 Shallow Residential: mint / light green.
- M Central Water: cyan / teal.
- H2 Traffic Light: teal-blue / violet.
- H3 Deep Debris: dark blue / violet.
- D Return Water: quiet blue.

## Added Proxies

- Start: supply buoy, platform, warm lantern, simple dock planks.
- H1: broad shallow zone, roof cluster, vegetation, light clutter.
- M: submerged road ribbon and open navigation water.
- H2: intersection slab, crosswalk stripes, tilted traffic light, utility poles, sign, broken road blockers.
- H3: dark depth field, large debris silhouette, metal frames, long vegetation.
- D: return-water cue with low roof and seaweed.
- Water: depth patches, ripple rings, simple flow arrows.

## Pass Questions

- From the boat camera, is there always at least one readable destination?
- Are H1, H2, and H3 visually distinct without relying on text labels?
- Does Start read as a start/supply point instead of a tiny marker?
- Does H2 already feel like the main identity landmark?
- Does H3 feel deeper and heavier without becoming unreadably dark?
- Does central water still leave enough open navigation space?

## Week 6 Lock Result

Week 6 is closed as the Map_01 spatial foundation candidate.

Locked for the next pass:

- Gameplay area: 640 x 420m.
- Water plane: 900 x 600m.
- Boat movement candidate: 9.5m/s forward, 4.1m/s reverse, acceleration 14.0, deceleration 11.0.
- Main loop: `Start -> H1 -> M -> H2 -> H3 -> D -> Start`.
- Support routes: `Start -> M`, `H1 -> H2`, `M -> D`.
- H1/H2/H3 are locked as role and location anchors only, not as final art.

Manual judgement:

- 96 x 64m, 112 x 72m, and 192 x 128m felt too compact.
- 7.2m/s felt too slow on the large map.
- 9.5m/s felt acceptable for the Week 6 candidate.
- W6-B/C layout is acceptable enough to move into water, submerged readability, and asset planning.

## Deferred To Week 7

- Water/depth/submerged readability.
- Height rules for submerged roofs, roads, debris, and landmark props.
- H1/H2/H3 asset sourcing from free assets and/or Blender MCP.
- Water transparency, water texture, guide-field visibility, and underwater occlusion.
- Detailed collision, final art density, and final camera polish.

## Non-Goals

- No final water shader.
- No full Blender terrain.
- No imported free asset pass.
- No final collision pass.
- No final H1/H2/H3 art completion.
