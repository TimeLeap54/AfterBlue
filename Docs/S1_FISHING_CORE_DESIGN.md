# S1 Fishing Core Design

## Goal

Lock the fishing experience around reading fish behavior under clear water, not around watching a large central tension gauge.

## S1 Result

S1 first-pass lock keeps D: Behavioral Tug as the active candidate. The implementation is not a final content/balance lock; it is the current core loop foundation for later visual feedback, tuning, and fish variation work.

## Candidate Modes

- A: Hook Only
- B: Continuous Tension
- D: Behavioral Tug

The current implementation starts with D as the primary candidate inside the existing Week 4 fishing loop. A and B remain design references for testing discussion, not separate runtime modes.

## Core Input

- Hold Space / left mouse: reel in, reduce fish distance, increase line stress.
- Release: give line, reduce line stress, allow some fish distance loss.

## S1 Prototype Flow

Idle -> Casting -> Waiting -> Bite -> Hooked -> Fighting -> Landing -> Result -> Idle

## Behavior States

- Approach: hold to bring the fish closer.
- Pull: release during a strong run.
- Turn: hold during weak direction changes.
- Struggle: mixed read-and-react interval.
- ObstacleRun: release and wait for the direction change.

## Prototype Fish

| Fish | Archetype | Target Time | Purpose |
|---|---|---:|---|
| Blue Minnow | Calm | 12-16s | Basic hold/release learning |
| Signal Fin | Dart | 18-24s | Short strong runs |
| Glass Mackerel | Pulse | 25-35s | Pattern learning |
| Concrete Carp | Heavy | 45-60s | Long pull and recovery windows |

## Week 5 Controls

- Space / left mouse: cast, hook, hold/release during the fight, or reset from result.
- The existing Week 4 bobber, line, ripple, debug UI, and codex flow remain active.
- Week 5 behavior patterns are selected automatically from the hooked fish difficulty.
- Fish identity and rarity stay hidden until the result. During the fight, the player sees movement signals such as `Strong run`, `Weak turn`, or `Running toward debris`.
- Line color is the stress read: pale is safe, orange is warning, red is near line break.

## Setup Flow

Use the same menu chain as the earlier weekly work:

`AfterBlue -> Setup -> Apply Week 5 Fishing Experience`

This augments the existing `FishingScene`. It does not create a separate default scene, and it keeps the Week 4 fishing loop, bobber, line, ripple, and debug tools active. Week 5 behavior patterns are wired into the existing `FishingStateMachine`; after a successful bite hook, the same bobber and line enter a short behavioral fight instead of replacing the scene with a separate prototype controller.

## Decision Rule

Keep D only if players can describe Pull vs Turn and change input without relying on a central gauge. If they cannot, collapse toward B.

## S1 Decision

D remains the working model after the first implementation pass. The loop is understandable enough to continue, but S2 should focus on non-debug readability: stronger line motion, fish shadow movement, water disturbance, and audio cues.

## Current Failure Rules

- Missed the bite: the player does not hook before the bite window ends.
- Held before the bite: the player was already holding when the bite started and never released before the window ended.
- Line broke: line stress stays high during incorrect input until durability reaches zero.
- Fish got too far: distance reaches the escape threshold.
- Fish reached debris: an obstacle run reaches the escape threshold.

## Current Catch Rules

- Catch only begins when distance reaches the active pattern's catch distance or fish stamina is exhausted.
- Behavior segments loop until catch or escape; reaching the end of a pattern no longer auto-catches the fish.
- Wrong Hold/Release now increases distance and stress enough to affect the result.
- Durability mainly takes damage when the player keeps holding under high stress.
- When stress reaches the break threshold, Release is always treated as a safe recovery action even if the current fish segment normally expects Hold.
