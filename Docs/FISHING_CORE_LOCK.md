# Fishing Core Lock

## Status

S1 first-pass lock complete.

## Current Candidate

D: Behavioral Tug, integrated into the existing Week 4 fishing loop.

## Current Setup Entry

`AfterBlue -> Setup -> Apply Week 5 Fishing Experience`

## Locked Items

- Keep the Week 4 cast, bobber, line, ripple, result, and codex flow.
- Hide fish identity and rarity until the result.
- Use one-button Hold/Release during the post-hook fight.
- Catch by reducing distance to the catch threshold or exhausting fish stamina.
- Fail by missing the bite, holding before the bite without releasing, line break, obstacle escape, or distance escape.
- Treat Release as a safe recovery action whenever line stress is in the danger range.

## Pending Decisions

- Whether fish behavior can be read without debug text after stronger visual/audio cues.
- Whether target catch times match S1 ranges after tuning.
- Which archetype set ships into production.
- Whether equipment, bait, or fish size should affect stress/durability in S2 or later.

## Do Not Expand Yet

- More fish species
- Equipment economy
- Final map
- Final fish art
- Sound and VFX polish
