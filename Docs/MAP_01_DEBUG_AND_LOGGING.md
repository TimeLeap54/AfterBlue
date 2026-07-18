# MAP_01 Debug And Logging

## Debug Toggles

- F1: Gameplay map
- F2: Habitat core/blend radius
- F3: Depth colors
- F4: Castable/non-castable
- F5: Bobber position fish weights
- F6: Movement path record
- F7: Landmark sightlines
- F8: Reset test data

Day1 implements the toggle shell and scene gizmo overlay. Full CSV logging is a later S2 step.

## Target CSV Files

- `PlaytestSession.csv`
- `BoatPath.csv`
- `FishingEvents.csv`
- `HabitatVisit.csv`

## Required Event Shape

- Timed samples every 0.5 to 1 second: time, boat position, speed, direction, nearest habitat, collision
- Fishing events: cast position, cast success/fail, selected fish, catch success/fail
- Habitat events: first habitat entry, start exit, start reentry
