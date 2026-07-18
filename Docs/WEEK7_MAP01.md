# Week 7 Map_01

Week 7 starts from the closed Week6 spatial foundation.

## Unity Menu

Use:

`AfterBlue > Setup > Apply Week 7 Water And Asset Pass`

This creates:

`Assets/AfterBlue/Scenes/Map_01/Map_01_Week7.unity`

## Goal

Make the Week6 map read as a flooded world before full asset production.

Week7 should not replace the Week6 layout. It should preserve the 640 x 420m map, the 9.5m/s boat candidate, and the Start/H1/M/H2/H3/D route structure.

## W7-A Scene And Branch Setup

Status: complete.

- Branch: `week07`.
- Base scene preserved: `Assets/AfterBlue/Scenes/Map_01/Map_01_Week6.unity`.
- Week7 scene created: `Assets/AfterBlue/Scenes/Map_01/Map_01_Week7.unity`.
- Week7 menu added under `AfterBlue > Setup`.
- Week7 scene currently uses the Week6 spatial foundation as its starting point.

## Next Segments

### W7-B Water Readability

- Reduce debug field dominance.
- Define shallow, medium, and deep water material targets.
- Make submerged silhouettes visible without looking like objects are sitting on top of the water.

### W7-C Submerged Height Rules

- Underwater: roads, some roofs, small debris, car bodies.
- Partially exposed: poles, traffic light, signs, large debris, some roof edges.
- Above water: supply buoy, gameplay anchors, readable landmark tops.

### W7-D H2 Representative Slice

- Build one traffic-light intersection slice directly on Map_01.
- Include water color, submerged road, crosswalk, tilted signal, poles, wires, and a few visible debris silhouettes.
- Keep normal boat navigation open.

### W7-E Asset Sourcing Table

- Decide which assets are reused proxies.
- Decide which assets are Blender MCP candidates.
- Decide which assets should come from free asset sources.
- Defer anything that does not improve water/submerged readability.
