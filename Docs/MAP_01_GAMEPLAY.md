# Week 6 Map_01 Gameplay Test

S2 now starts from the final 0-1 production spec. The goal is not a pretty map. The goal is to verify map size and boat movement metrics before placing real assets.

## Current Test Hypothesis

- Gameplay area: 192 x 128m
- X range: -96 to 96
- Z range: -64 to 64
- Water visual blockout: 280 x 192m
- Unity scale: 1 unit = 1 meter
- Water height: Y 0
- Test scene: `Assets/AfterBlue/Scenes/Map_01/Map_01_Week6_Blockout.unity`
- Boat forward speed: 7.2m/s
- Boat reverse speed: 3.2m/s

## Do Now

- Verify current boat size against a 1.8 x 4.5m reference cube.
- Measure 20m straight travel time.
- Measure average speed.
- Measure comfortable U-turn diameter.
- Measure stop distance after releasing input.
- Measure horizontal and vertical crossing times.
- Measure empty outer loop time.
- Decide whether 192 x 128m should be kept, expanded again, or reduced.

## Do Not Do Yet

- Blender prop placement
- Free asset placement
- Real water shader
- Flooded house production
- Mountain/background production
- Detailed obstacle placement

## Week 6 Builder

Use:

`AfterBlue > Setup > Apply Week 6 Map 01 Blockout`

This creates the exact 0-1 blockout scene specified by the latest Map_01 production document.

## Zone Nodes

- S Start / Supply: (-76, -40)
- A Shallow Village: (-56, 28)
- M Central Water: (-12, 4)
- B Intersection: (48, 28)
- C Deep Debris: (60, -36)
- D Return Water: (-16, -40)

## Decision Rules

Keep 192 x 128m if:

- Horizontal crossing is 25-38 seconds.
- Empty outer loop is 75-110 seconds.
- Most zone travel is 8-20 seconds.
- Whole map is not visible from center at once.
- Movement does not feel too long or too short.

Consider a larger size if:

- Horizontal crossing is under 23 seconds.
- Empty outer loop is under 70 seconds.
- Opposite landmarks read too easily.
- Zones feel too close together.

Consider a smaller size if:

- Horizontal crossing is over 40 seconds.
- Empty outer loop is over 120 seconds.
- Zone travel already feels dull.
- Empty water travel is too long.
