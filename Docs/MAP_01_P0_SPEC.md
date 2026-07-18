# Map_01 P0 Spec

P0 locks the working rules before map size and art decisions.

## Scale Rules

- Unity 1 unit = 1 meter.
- Blender 1 meter = Unity 1 unit.
- Unity water height = Y 0.
- Blender water height = Z 0.
- Temporary boat reference size = 1.8m width x 4.5m length.

## Active Scene

`Assets/AfterBlue/Scenes/Map_01/Map_01_Week6.unity`

## Required Hierarchy

- `Map_01_Week6`
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

## Builder Support

`Assets/Editor/Week6Map01Builder.cs` now creates:

- the full required hierarchy
- `Water_Blockout` centered at Y -0.05 with top surface at Y 0
- gameplay guides centered around water height Y 0
- `BoatSize_Reference` at 1.8 x 1.0 x 4.5m
- `P0_SPEC_Unity1m_Blender1m_WaterY0` under `00_SYSTEM`

## Current Verification

- Builder is updated for P0.
- Docs are updated for P0.
- Existing scene file may need menu regeneration if Unity has not refreshed the builder yet.

Use:

`AfterBlue > Setup > Apply Week 6 Map 01`
