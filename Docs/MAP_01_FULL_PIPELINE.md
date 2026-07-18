# Map_01 Full Production Pipeline

Source spec: `AfterBlue Map_01 제작 전체 파이프라인.txt`

## Core Principle

Validate the playable space in Unity first, produce only the necessary assets in Blender, then assemble and direct the scene back in Unity.

Do not build the entire map as one Blender mesh.

## Production Order

1. Game rules
2. Boat control
3. Map size
4. Spatial structure
5. Grey blockout
6. Playtest
7. Asset production
8. Spot B lookdev
9. Full map expansion
10. Water, lighting, and outer scenery
11. Optimization and QA

## Phase List

| Phase | Name | Intent |
| --- | --- | --- |
| P0 | Project Scale Setup | Lock Unity/Blender meter rules and scene structure. |
| P1 | Boat Movement Metrics | Measure speed, stop distance, and turn diameter before judging map size. |
| P2 | Map Size Comparison | Compare 80 x 56m, 96 x 64m, and 112 x 72m candidates. |
| P3 | Spatial Graph | Lock S/A/M/B/C/D nodes and 8 route connections. |
| P4 | Grey Blockout | Use primitives and debug materials only. |
| P5 | Obstacle and Boundary Design | Add obstacle clusters and believable outer boundary logic. |
| P6 | S2 Playtest and Structure Lock | Test routes, collisions, loops, and camera readability. |
| P7 | Asset Production Table | Translate every blockout object into direct, free, MCP-derived, Unity effect, or delete. |
| P8 | Blender Core Assets | Make first versions of road, buoy, platform, pole, traffic light, and roof/house parts. |
| P9 | Free Asset Selection and MCP Cleanup | Import a small curated set only and normalize scale/material/origin. |
| P10 | Shared Style and Materials | Lock common material language before full map art. |
| P11 | Spot B Vertical Slice | Finish the traffic-light intersection first. |
| P12 | Underwater Terrain and Water | Build depth layers and Unity water behavior. |
| P13 | Full Area Art Expansion | Expand Spot B rules to A, M, C, D, Start, and boundaries. |
| P14 | Outer Scenery | Add near collision boundary, mid background, and distant silhouettes. |
| P15 | Fishing Zones and Systems | Connect habitats, trigger volumes, fish tables, shadows, and water color adjustments. |
| P16 | Colliders, Optimization, QA | Final movement, visibility, art consistency, and performance checks. |

## Current Position

Current work is Week 6 Map_01. W6-A promotes the active candidate to 640 x 420m, with 192 x 128m kept only as a rejected comparison frame.

Active scene:

`Assets/AfterBlue/Scenes/Map_01/Map_01_Week6.unity`

Active menu:

`AfterBlue > Setup > Apply Week 6 Map 01`

Current test candidate:

- Gameplay area: 640 x 420m
- Water plane: 900 x 600m
- Nodes: Start, H1, M, H2, H3, D placed with direct W6-B/C layout coordinates
- Boat speed: 9.5m/s forward, 4.1m/s reverse
- Main route: Start-H1-M-H2-H3-D-Start
- Support routes: Start-M, H1-H2, M-D

## Current Non-Goals

Do not add these until Week 6 readability and movement measurements are recorded:

- Final Blender prop placement
- Free asset placement pass
- Real water shader
- Final flooded house production
- Mountain/background production
- Final collision and obstacle pass

## Reference Image Role

The current reference images guide Week 6 readability and later P11-P14 direction:

- bright shallow water near Start/H1
- darker deep region toward C
- visible flooded road/traffic/pole language
- sparse central navigable water
- distant flooded settlement and hills extending the world

Week 6 implements these only as readable proxy shapes, not final art.

## S2 Lock Requirements

Before moving to serious asset production:

- Map size is chosen.
- Boat speed is chosen.
- Channel width is chosen.
- Six major zones are confirmed.
- Eight route connections are confirmed.
- Outer boundary method is confirmed.
- There are no large unused water areas.
- There are no invisible-wall-first collisions.
