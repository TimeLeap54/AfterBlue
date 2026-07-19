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

Status: implementation complete, visual review pending after Unity menu apply.

- Imported candidate: `Assets/IgniteCoders/Simple Water Shader`.
- Week7 menu uses `WaterBlock_50m.prefab` when the asset is available.
- Week7 menu clones `Water_mat_03.mat` into `Assets/AfterBlue/Materials/Week7/AB_Water_Map01_Candidate.mat`.
- Week7 menu applies `AB_Water_Map01_Candidate.mat` to the large 900 x 600m water surface.
- Week7 debug guide materials are written under `Assets/AfterBlue/Materials/Week7` so Week6 debug materials stay stable.
- Week7 guide fields, route lines, and boundary lines use reduced alpha so water reads first.
- Fallback behavior: if the imported water prefab or material is missing, the builder falls back to the old simple water plane.
- Shallow/medium/deep are currently expressed with very faint depth overlays plus submerged shadows, not separate water shaders.

### W7-C Submerged Height Rules

Status: builder rules complete, visual review pending after Unity menu apply.

- Underwater: roads, some roofs, small debris, car bodies.
- Partially exposed: poles, traffic light, signs, large debris, some roof edges.
- Above water: supply buoy, gameplay anchors, readable landmark tops.
- H1 roof proxies are lowered so only roof tops/edges should read.
- M and H2 road/crosswalk proxies are lowered below the water surface.
- H3 debris is mostly submerged, with only large silhouettes and some planks reading near the surface.
- Start supply buoy and H2 traffic light remain clearly above water as navigation anchors.

### W7-D H2 Representative Slice

- Build one traffic-light intersection slice directly on Map_01.
- Include water color, submerged road, crosswalk, tilted signal, poles, wires, and a few visible debris silhouettes.
- Keep normal boat navigation open.

### W7-E Asset Sourcing Table

- Decide which assets are reused proxies.
- Decide which assets are Blender MCP candidates.
- Decide which assets should come from free asset sources.
- Defer anything that does not improve water/submerged readability.
