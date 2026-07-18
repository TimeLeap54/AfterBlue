# Map_01 Production Bible v0.1

Map_01 is the launch map. It must support a complete 20-40 minute loop with free boating, free fishing, three readable habitats, and a strong flooded modern village identity.

## Core Goal

When the player moves a small boat across clear teal water, they should see submerged roads, rooftops, crosswalks, traffic lights, fish shadows, and fishing spots that naturally invite exploration.

## Current Scale Decision

The original planning coordinates were 64 x 44. For the launch-only Map_01 direction, S2 now expands that layout by 3x:

- Design size: 192 x 132
- Playable area: X -90 to 90, Z -60 to 60
- Unity world scale: 4.0
- Approximate world read: 768 x 528

This keeps the original landmark relationship while giving the boat enough room to feel like it is exploring a real map instead of circling a small test board.

## Map Identity

- Flooded modern village
- Small wooden boat
- Free fishing and quiet exploration
- Clear teal water over roads, rooftops, crosswalks, poles, and debris
- Melancholic but peaceful mood
- Low-poly base shapes with painterly lighting and restrained material detail

Avoid:

- Dense open-world city
- High-rise urban map
- Realistic flood disaster scene
- Horror darkness
- Toy-like cute diorama
- Overcrowded prop display
- Fully modeled interiors, furniture, or vehicle details

## Habitat Structure

### Start / Supply Buoy

Role:

- Starting point
- Return point
- Shop/aquarium access later

Placement:

- Supply buoy
- Lantern
- Radio
- Small crates
- Sparse rocks or debris
- Wide open water around it

Rule: it must be visually simple, and H1 should be readable from the start within about 20 seconds of play.

### H1 Shallow Residential

Role:

- First fishing comfort zone
- Shallow water
- Common fish
- Brightest habitat

Required read:

- Rooftops
- Window frames
- Soft weed clusters
- Small fish shadows
- Clear water and safe navigation space

Rule: H1 should feel friendly and readable. Do not overfill it with debris.

### Central Submerged Road

Role:

- Movement connector
- Modern-civilization read
- Sightline guide to H2

Required read:

- Long road direction
- Lane/crosswalk fragments
- Minimal debris
- Open traversal water

Rule: this area must stay comparatively empty. It is navigation space first.

### H2 Traffic Light

Role:

- Hero composition
- Map identity screenshot
- Mid-depth fishing area
- Mutated/special fish later

Required read:

- Tilted traffic light
- Crosswalk
- Intersection road pieces
- Utility pole/sign support
- One or two warm accents
- Bobber and fish shadow readable beside the landmark

Rule: H2 has the highest density, but the boat must still have room to stop and fish.

### H3 Deep Debris

Role:

- Deepest habitat
- Large fish/legendary fish direction
- Final quiet tension

Required read:

- Large low debris silhouette
- Metal frames
- Pipes
- Vehicle silhouette only if cheap
- Long weeds
- Large fish shadow
- Bubbles/ripples

Rule: H3 should feel deep and weighty, not like a horror area.

## Asset Classification

### Hero Assets

Directly controlled and not repeated casually:

- Small wooden boat
- Seated angler
- Start supply buoy
- H2 tilted traffic light
- H1 representative flooded roof/house top
- H3 representative deep debris structure
- Legendary fish silhouette

### Modular Assets

Few base models, many variants:

- Road straight
- Road curve
- Intersection piece
- Crosswalk piece
- Lane paint fragments
- Curb pieces
- Utility pole
- Road signs
- Guardrail
- Roof A/B/broken
- Wall-top fragments
- Window frames
- Small vents/tanks
- Metal frames
- Wooden planks
- Pipes
- Crates
- Rocks
- Weed clusters

### Background / Filler Assets

Allowed to be external/AI-assisted if easy to clean:

- Distant building masses
- Distant roof clusters
- Far utility poles
- Distant submerged vegetation
- Low-detail debris silhouettes

## Production Rules

- Hero assets define the game identity.
- Modular assets fill the map through scale, tilt, rotation, depth, color, moss/rust decals, and cluster variation.
- Background assets support depth without competing with landmarks.
- Do not add new one-off props whenever the map feels empty.
- Empty water is intentional: 45-55% of the map should remain open navigation space.

## Density Targets

- Open navigation water: 45-55%
- Habitat mid-density areas: 20-25%
- Underwater structure and decor: 15-20%
- Hero landmark density: 5-8%
- Outer silhouettes: 5-10%

## Material Set

Use a small shared material library:

- `M_Wood_Painted`: boat, crates, small signs
- `M_Metal_Rusted`: traffic light, poles, metal debris
- `M_Concrete_Submerged`: concrete, walls, curbs
- `M_Asphalt_Flooded`: road surface
- `M_Roof_Faded`: roof pieces
- `M_Plants_Wet`: weeds and moss
- `M_Emission_Accent`: lanterns, traffic light residue, special fish traces

Textures should support large color and roughness reads. Rust, moss, and wear should mostly come from decals or material variation, not unique high-detail textures.

## Water Direction

Water is the core visual system:

- H1: bright cyan, highest clarity
- H2: balanced teal, road/fish/bobber all readable
- H3: darker blue-green, deeper but still playable

Water priority:

1. Bobber readable
2. Fish shadow readable
3. Submerged roads/roofs readable
4. Atmosphere/reflection afterward

## Lighting Direction

- Cool environment light
- Warm late-afternoon sunlight
- Very limited warm local accents
- H2 signal and Start lantern can carry warm guidance
- H3 stays readable and non-horror

## Camera Direction

Target camera:

- Mid-height angled quarter view
- Pitch around 45-55 degrees
- FOV around 38-45 as a later LookDev target
- Boat, current landmark, bobber, fish shadow, and submerged road should be readable in priority order

S2 camera remains temporary. Final camera is a later task.

## Blender MCP Role

The user decides:

- Asset category
- Silhouette approval
- Camera readability
- Hero vs modular vs background classification
- Final accept/reject

Blender MCP helps with:

- Variant generation
- Scale/rotation application
- Pivot cleanup
- Naming cleanup
- FBX export
- Tilt/damage/proportion variations
- Batch debris/filler generation

MCP is a production assistant, not the art director.

## External Asset Strategy

- Hero assets: direct control; external assets only as reference or base after heavy cleanup
- Modular assets: direct base models, MCP variants
- Background/filler assets: external or AI allowed if license and cleanup are safe

Likely source categories:

- Kenney / Quaternius: proxy or low-poly reference
- Poly Haven / ambientCG: HDRI and material reference
- Sketchfab: only carefully licensed CC0/CC BY background candidates

## Production Order

1. Map blueprint lock
2. Hero layout blockout
3. Modular kit production
4. Material and water setup
5. Lighting and LookDev
6. Zone population
7. Final map polish

## Immediate Priority

The next correct work is not a detached Week7 ArtLab. It is:

1. Regenerate Week6 Map_01 at the 3x launch-map scale.
2. Verify boat travel time and camera comfort.
3. Confirm Start -> H1 -> Central -> H2 -> H3 sightline flow.
4. Begin replacing greybox proxies with controlled Hero and Modular kits.
