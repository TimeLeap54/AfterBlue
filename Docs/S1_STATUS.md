# S1 Status

## Status

Complete as a first-pass fishing core lock candidate.

## Applied Scope

- Existing Week 4 fishing loop is preserved.
- `AfterBlue -> Setup -> Apply Week 5 Fishing Experience` wires S1 behavior patterns into `FishingScene`.
- Post-hook fight uses one-button Hold/Release.
- Fish identity and rarity are hidden until the result.
- Result and codex flow remain on the Week 4 path.

## Runtime Flow

Idle -> Casting -> Waiting -> Bite -> Hooked -> Fighting -> Landing -> Result -> Idle

## Current Success Rules

- `Distance <= Catch` enters landing.
- Fish stamina exhausted enters landing.
- Landing grants the fish result and registers the catch.

## Current Failure Rules

- Bite timeout.
- Holding before bite without releasing.
- `Distance >= Escape`.
- `Durability <= 0`.
- Obstacle escape during obstacle run.

## Locked For Now

- D: Behavioral Tug remains the active candidate.
- Release is always a safe recovery action when stress reaches the danger threshold.
- Behavior segments loop until catch or escape; pattern completion does not auto-catch.

## Deferred

- Stronger non-debug visual reads for Pull, Turn, Struggle, and ObstacleRun.
- Audio feedback.
- Fish-specific balancing.
- Equipment, bait, and size effects.
- Final UI polish.
