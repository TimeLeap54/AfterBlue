# Fishing A/B/D Test

## Test Objective

Compare whether Hook Only, Continuous Tension, or Behavioral Tug should become AfterBlue's locked fishing model.

## Test Order

Rotate order per tester:

- Tester 1: A -> B -> D
- Tester 2: B -> D -> A
- Tester 3: D -> A -> B

## Task

Tell the tester only:

> Catch three fish.

## Observations

Record:

- Did the tester notice the bite window?
- Did the tester release during Pull?
- Did the tester hold during Turn?
- Did the tester watch fish shadow/water or the debug numbers?
- Did the tester understand why they failed?
- Did they voluntarily cast again?

## Pass Signals

- 4/5 testers understand hold/release by the second catch.
- 4/5 testers can explain Pull vs Turn.
- 3/5 testers want to try a different fish behavior.
- Repeated catches trend faster for the same fish.

## Current Build

Week 5 applies to `Assets/Scenes/FishingScene.unity` through `AfterBlue -> Setup -> Apply Week 5 Fishing Experience`.

## S1 Outcome

D: Behavioral Tug remains the active candidate for the next pass. A and B are retained as fallback design references, but the runtime implementation now tests D inside the existing Week 4 loop.

## Current Runtime Test

- Catch three fish.
- Confirm the player hooks after the bite instead of holding early.
- Confirm the player releases when stress is dangerous.
- Confirm the player can explain `Distance <= Catch`, `Distance >= Escape`, and `Durability <= 0`.
- Confirm fish identity remains unknown until the result.
