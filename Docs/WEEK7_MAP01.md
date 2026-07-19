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

### W7-D Submerged Height Rules

Status: Height Rule v2 implemented in builder, visual review pending after Unity menu apply.

- Underwater: roads, some roofs, small debris, car bodies.
- Partially exposed: poles, traffic light, signs, large debris, some roof edges.
- Above water: supply buoy, gameplay anchors, readable landmark tops.
- Water level: Y 0.00.
- Shallow underwater floor: Y -0.55.
- Medium underwater floor: Y -0.80.
- Deep underwater floor: Y -1.35.
- Underwater roads: Y -0.55.
- Underwater crosswalks / visible road fragments: Y -0.48.
- Underwater small debris: Y -0.35.
- Partially exposed roofs: Y -0.22.
- Partially exposed debris: Y -0.18.
- Surface planks / low markers: Y +0.03.
- Readable poles/signs: Y +1.15.
- Readable traffic light anchor: Y +1.70.
- H1 roof proxies are lowered so only roof tops/edges should read.
- M and H2 road/crosswalk proxies are lowered below the water surface.
- H3 debris is mostly submerged, with only large silhouettes and some planks reading near the surface.
- Start supply buoy and H2 traffic light remain clearly above water as navigation anchors.

### W7-E Underwater Floor And H2 Representative Slice

Status: builder implementation complete, visual review pending after Unity menu apply.

- Create a replaceable `W7_UnderwaterFloor_ReplaceWithFloodedGrounds` group under `05_OBSTACLE_BLOCKOUT`.
- Add large underwater floor plates for Start, H1, M, H2, H3, and D so the map no longer reads as empty water over guide plates.
- Lower roads and crosswalks farther below water so they read as submerged city structure, not floating surface props.
- Enlarge central road and H2 intersection proxies so the route reads at the 640 x 420m map scale.
- Keep these W7-E floor plates collider-free, so normal boat navigation remains open.
- Treat these plates as temporary slots. Replace them with Flooded Grounds road/ground prefabs once that package is actually imported under `Assets`.

### W7-F Asset Sourcing Table

Status: first pass drafted.

| Need | Current Week7 source | Replacement direction |
| --- | --- | --- |
| Water surface | `IgniteCoders/Simple Water Shader` | Keep as first candidate until performance or visual issues appear. |
| Underwater ground | W7-E primitive floor plates | Replace with Flooded Grounds terrain/ground prefabs after import. |
| Submerged asphalt/crosswalk | Enlarged Week5-style primitives | Replace with Flooded Grounds road/intersection pieces if scale and license fit. |
| Roof silhouettes | Existing `flooded_roof_modern_v01.fbx` plus proxies | Keep, then add variants through Blender MCP or a small modular kit. |
| Traffic light landmark | Existing tilted proxy | Build or source a hero asset for H2 later. |
| Utility poles | Existing `rusted_utility_pole_v01.fbx` | Keep and add wire readability pass later. |
| Vegetation/debris | Primitive proxies | Replace selectively; do not fill the whole map yet. |
| Fish silhouettes | Deferred | Add after water readability is accepted. |

`Flooded Grounds` is not currently visible in the project `Assets` tree. It may still need to be imported through Unity Package Manager or moved from the local download/cache location.
