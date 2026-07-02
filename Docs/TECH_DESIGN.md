# AfterBlue Technical Design

## Engine

Unity 2022.3 LTS with Universal Render Pipeline.

## Architecture Goal

Keep gameplay small, modular, and data-driven so Codex can safely work on one feature at a time.

## Core Systems

- BoatMovementSystem
- FishingSystem
- FishSpawnSystem
- CatchResultSystem
- InventorySystem
- ShopSystem
- JournalSystem
- ProgressionSystem
- SaveSystem
- UISystem
- AudioSystem

## First Prototype Scope

- Boat movement on the XZ plane
- Smooth follow camera
- Placeholder water plane
- Placeholder boat object
- Basic fishing state shell
- ScriptableObject definitions for fish and locations

## Implementation Constraints

- No open-world streaming
- No land movement
- No free camera rotation
- No advanced water physics
- Keep fishing state explicit and easy to debug

