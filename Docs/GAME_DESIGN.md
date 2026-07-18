# AfterBlue Game Design v1.0

AfterBlue is a short 3D fishing collection game about drifting across a flooded modern village in a small boat, reading fish behavior and environmental clues through clear water, and completing an aquarium and voyage journal.

## Product Target

- Platform: Steam
- Runtime: 20-40 minutes
- Map: Map_01 only
- Engine: Unity 2022 URP
- Style: stylized low-poly with painterly lighting
- Camera: fixed angled mid-height quarter view
- Core activity: free boating, free fishing, fish collection, journal discovery, return ending

## One-Line Pitch

After the world has sunk, a small angler drifts above a flooded village and records the fish and landscapes that adapted to the water.

## Success Criteria

| Goal | Criteria |
| --- | --- |
| Identity | A 10-second clip clearly reads as fishing in a flooded modern village. |
| Completion | A new player can reach the ending in 20-40 minutes. |
| Reusable learning | Design, level, Blender, Unity art, AI workflow, and release lessons remain reusable. |

## Release Scope

| Category | Scope |
| --- | ---: |
| Maps | 1 |
| Core habitats | 3 |
| Fish species | 8 |
| Base fish body types | 4 |
| Baits | basic + 2 special |
| Rods | 1 base rod, optional single support upgrade |
| Catch storage | 6 slots |
| Aquarium | 8 species records |
| Voyage journal entries | 4 |
| Legendary fish | 1 |
| Ending | 1 |

## Forbidden Scope

- Map_02 or Map_03
- Land movement
- Free character walking
- Free-rotation camera
- Open-world city
- NPC dialogue or quest log
- Combat or survival stats
- Weather and seasons
- Direct underwater exploration
- Aquarium decoration
- Fish breeding or feeding
- Complex boat customization
- Multi-stage rod upgrades
- Multiple currencies
- Daily missions
- Realistic line collision physics
- Photorealistic AAA flooded city

## Design Philosophy

AfterBlue is not a tension-gauge management game. It is a game about reading fish and environmental behavior through transparent water, then changing movement and input to discover new life.

## Core Pillars

### Quiet Navigation

Boat movement should feel like reading the world, not commuting through empty space.

- Landmarks are visible without a minimap.
- Central water remains open and calm.
- Underwater roads and fish shadows can be observed while moving.
- Boat control is stable and not overly slippery.

### Readable Fishing

Fishing success and failure must be visually understandable.

- Fish approach is visible.
- Behavior telegraphs exist.
- Pull and recovery phases look different.
- Failure does not feel random.
- The player should not stare only at UI.

### Melancholic Discovery

The world explains itself through environment and fish.

- Small schooling fish around shallow residential roofs.
- Light-adapted fish near traffic signals.
- Rust/debris-adapted fish near H3.
- The mood is quiet recovery, not horror or disaster.

## Core Loop

Moment loop:

1. Notice an environmental or fish clue.
2. Stop the boat.
3. Cast.
4. Watch the shadow approach.
5. Hook.
6. Read fish behavior.
7. Hold or release.
8. Land the fish or fail.
9. Register result.

Session loop:

1. Depart from the supply buoy.
2. Explore a habitat.
3. Fish 2-4 times.
4. Fill storage.
5. Return.
6. Register aquarium entries.
7. Sell duplicates.
8. Buy bait.
9. Follow a new clue.

Full loop:

1. Learn H1.
2. Discover H2 mutation behavior.
3. Unlock deeper fishing.
4. Enter H3.
5. Catch the legendary fish.
6. Complete the final journal entry.
7. Return for the ending.

## Fish Behavior

The main input is one button:

- Hold: reel in, reduce distance, add line stress.
- Release: give slack, reduce stress, allow small distance gain, avoid rush damage.

Behavior grammar must stay consistent:

| Fish behavior meaning | Player response |
| --- | --- |
| Approach / Turn / Recovery | Hold |
| Pull / Dart / Obstacle Run | Release |
| Telegraph | Observe and prepare |

Species differ by telegraph time, behavior order, pull duration, recovery length, transition speed, repeated rushes, and late-pattern changes.

## Fish List

| ID | Working name | Rarity | Body type | Habitat | Behavior | Bait |
| --- | --- | --- | --- | --- | --- | --- |
| F01 | Silver Minnow | Common | Small | H1 / Central | Calm | Basic |
| F02 | Blueback Mackerel | Common | Medium | H1 / Central | Dart | Basic |
| F03 | Glassbelly Puffer | Mid | Round | H1 / H2 | Heavy | Basic |
| F04 | Red Sail Bream | Mid | Medium | H2 | Dart | Basic / Spark |
| F05 | Signal Scale | Rare | Small | H2 | Pulse | Spark |
| F06 | Lantern-Eye Fish | Rare | Medium | H2 | Dart + Pulse | Spark |
| F07 | Rustfin Eel | Rare | Eel | H3 | Heavy + Pulse | Deep |
| F08 | Afterglow Fish | Legendary | Large/Eel | H3 | Unique | Deep |

## Map_01

Map_01 is the entire launch map.

- Start / Supply Buoy
- H1 Shallow Residential
- Central Submerged Road
- H2 Traffic Light
- H3 Deep Debris
- Return sightline

Current production direction expands the original 64 x 44 plan to a 192 x 132 launch-map layout. See `Docs/MAP_01_PRODUCTION_BIBLE.md`.

## Production Rule

AfterBlue should not build many systems and attach a pretty map later. The flooded village space, water, light, and ecology must become the foundation that every system supports.
