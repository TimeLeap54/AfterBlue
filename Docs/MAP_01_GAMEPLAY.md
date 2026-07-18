# Week 6 Map_01 Gameplay Test

S2 now starts from the final 0-1 production spec. The goal is not a pretty map. The goal is to verify map size and boat movement metrics before placing real assets.

## Current Test Hypothesis

- Gameplay area: 640 x 420m
- X range: -320 to 320
- Z range: -210 to 210
- Water visual plane: 900 x 600m
- Unity scale: 1 unit = 1 meter
- Water height: Y 0
- Test scene: `Assets/AfterBlue/Scenes/Map_01/Map_01_Week6.unity`
- Boat forward speed: 9.5m/s
- Boat reverse speed: 4.1m/s

## Do Now

- Verify current boat size against a 1.8 x 4.5m reference cube.
- Measure 100m straight travel time.
- Measure average speed.
- Measure comfortable U-turn diameter.
- Measure stop distance after releasing input.
- Measure horizontal and vertical crossing times.
- Measure empty outer loop time.
- Decide whether 640 x 420m should be kept, expanded again, or reduced.

## Do Not Do Yet

- Blender prop placement
- Free asset placement
- Real water shader
- Flooded house production
- Mountain/background production
- Detailed obstacle placement

## Week 6 Builder

Use:

`AfterBlue > Setup > Apply Week 6 Map 01`

This creates the exact 0-1 blockout scene specified by the latest Map_01 production document.

## Zone Nodes

- Start / Supply: (-270, -165)
- H1 Shallow Residential: (-230, 135)
- M Central Open Water: (-30, 20)
- H2 Traffic Light: (235, 120)
- H3 Deep Debris: (250, -155)
- D Return Water: (-90, -170)

## Route Loop

Main loop:

`Start -> H1 -> M -> H2 -> H3 -> D -> Start`

Support routes:

- `Start -> M`
- `H1 -> H2`
- `M -> D`

## Decision Rules

Keep 640 x 420m if:

- Horizontal crossing is 45-75 seconds at the W6-D speed candidate.
- Empty outer loop is 3-5 minutes at the W6-D speed candidate.
- Most adjacent zone travel is 20-50 seconds at the W6-D speed candidate.
- Whole map is not visible from center at once.
- Movement does not feel too long or too short.

Consider a larger size if:

- Horizontal crossing is under 40 seconds at the W6-D speed candidate.
- Empty outer loop is under 3 minutes at the W6-D speed candidate.
- Opposite landmarks read too easily.
- Zones feel too close together.

Consider a smaller size if:

- Horizontal crossing is over 80 seconds at the W6-D speed candidate.
- Empty outer loop is over 5 minutes at the W6-D speed candidate.
- Zone travel already feels dull.
- Empty water travel is too long.
