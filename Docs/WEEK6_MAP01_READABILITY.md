# Week 6 Map_01 Readability

Week 6 Readability exists because the 192 x 128m test map reads too empty with only small zone dots.

## Unity Menu

Use:

`AfterBlue > Setup > Apply Week 6 Map 01 Readability`

This creates:

`Assets/AfterBlue/Scenes/Map_01/Map_01_Week6_Readability.unity`

## Goal

Make the player camera read clear destinations on the large map before final art, real water, or asset production.

## Active Rules

- Keep gameplay area at 192 x 128m for this test.
- Keep boat speed at 7.2m/s forward and 3.2m/s reverse.
- Replace small yellow point markers with large color-coded habitat fields.
- Reuse Week5 visual language as proxy structure: roof, road, traffic light, debris, buoy, ripple cues.
- Do not treat this as final art.

## Zone Color Language

- Start / Supply: warm orange.
- H1 Shallow Residential: mint / light green.
- M Central Water: cyan / teal.
- H2 Traffic Light: teal-blue / violet.
- H3 Deep Debris: dark blue / violet.
- D Return Water: quiet blue.

## Added Readability Proxies

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

## Known Issues To Solve In Week 7

- The current proxy clusters still feel too compact.
- Too many structures sit on top of the water instead of being submerged.
- Transparent guide fields can block water readability.
- A water/depth pass is needed before further art placement decisions.

## Non-Goals

- No final water shader.
- No full Blender terrain.
- No imported free asset pass.
- No final collision pass.
- No final H1/H2/H3 art completion.
