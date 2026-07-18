# Week 6 Map_01 Blockout

Source spec: `AfterBlue Map_01 제작 - 0~1단계 최종판.txt`

Week 6 Blockout is a measurement scene, not a production map.

## Unity Menu

Use:

`AfterBlue > Setup > Apply Week 6 Map 01 Blockout`

This creates:

`Assets/AfterBlue/Scenes/Map_01/Map_01_Week6_Blockout.unity`

## Purpose

- Keep Unity 1 unit = 1 meter.
- Keep Blender 1 meter = Unity 1 unit.
- Keep water surface at Unity Y 0.
- Test the 192 x 128m large-map candidate.
- Measure boat speed, stop distance, turn diameter, crossing time, and empty loop time.

## Current Test Size

- Gameplay area: 192 x 128m
- X range: -96 to 96
- Z range: -64 to 64
- Water blockout: 280 x 192m
- Boat forward speed: 7.2m/s
- Boat reverse speed: 3.2m/s
- Node coordinates are expanded by 2.0x from the original 96 x 64m layout.

## Zone Nodes

| Node | Position | Role |
| --- | --- | --- |
| S | (-76, -40) | Start / Supply |
| H1 | (-56, 28) | Shallow Residential |
| M | (-12, 4) | Central Water |
| H2 | (48, 28) | Traffic Light |
| H3 | (60, -36) | Deep Debris |
| D | (-16, -40) | Return Water |

## Required Hierarchy

- `Map_01_Week6_Blockout`
- `00_SYSTEM`
- `01_WATER`
- `02_MAP_GUIDES`
- `03_ZONE_MARKERS`
- `04_ROUTE_GUIDES`
- `05_OBSTACLE_BLOCKOUT`
- `06_BOUNDARY`
- `07_LANDMARKS`
- `08_FISHING_ZONES`
- `99_DEBUG`

## Measurement Targets

- Current boat real size
- 20m straight time
- Average movement speed
- Comfortable U-turn diameter
- Stop distance
- Horizontal crossing time
- Vertical crossing time
- Empty outer loop time
- Keep 192 x 128 / expand / shrink decision

## Non-Goals

- No final Blender prop placement.
- No free asset placement pass.
- No final water shader.
- No final flooded house production.
- No final background production.
- No final collision pass.
