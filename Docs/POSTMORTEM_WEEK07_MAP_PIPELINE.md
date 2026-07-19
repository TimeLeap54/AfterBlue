# AfterBlue Week01-07 Postmortem

Date: 2026-07-20
Branch: `week07`

This document records why the current AfterBlue prototype is being archived instead of pushed further in the same direction.

## Summary

AfterBlue did not fail because the fishing loop was impossible. The main failure point was the map and environment production pipeline.

The project reached a working fishing prototype, but Map_01 required a much clearer art direction, underwater terrain plan, material pipeline, and 3D asset strategy than the project had at the time. AI-assisted implementation increased speed, but it also made it easy to keep adding plausible-looking pieces before the design foundation was locked.

## What Worked

- Week01-05 produced a usable prototype rhythm.
- Boat movement, fishing input, bite states, catch results, species data, and codex-style records were testable.
- The fishing system reached a point where tuning could continue.
- Week06 improved the map scale and boat speed. A 640 x 420m map and 9.5m/s boat speed felt closer to the intended play space than the earlier compact map.

## Main Failure Points

### 1. Art Direction Was Not Locked

The project moved between several visual targets:

- Low-poly prototype style.
- Semi-realistic reference images.
- Free Asset Store packs.
- Blender-made simple props.
- AI reference images with much higher environmental quality than the prototype could support.

Because the art target was not strict, every new asset shifted the visual direction. The work started following available assets instead of the original game intention.

Needed before further production:

- A narrow art bible.
- Clear allowed and disallowed asset styles.
- A decision on low-poly, stylized realism, or asset-pack-driven style.
- A material palette for water, wet wood, asphalt, concrete, moss, rust, and underwater silhouettes.

### 2. Map Design Was Too Vague

Map_01 was treated as a visual layout problem too early. It should first have been a spatial and gameplay specification.

Missing locked decisions:

- Exact playable boundary.
- Natural blockers and why the player cannot pass them.
- Underwater terrain height map.
- Road height versus water level.
- Which structures are fully submerged, partially exposed, or above water.
- How the player reads Start, H1, M, H2, H3, and D from the gameplay camera.
- How sky, fog, water clarity, lighting, and distant silhouettes support the map boundary.

The project had a water level (`Y=0`) but not a real underwater ground model. Because of this, roads, roofs, plants, and debris were repeatedly placed by feel. That made the process unstable.

### 3. Underwater Terrain Should Have Come First

The correct order should have been:

1. Lock water level.
2. Lock underwater terrain height by zone.
3. Place submerged road/ground surfaces.
4. Test gameplay camera readability.
5. Add large landmark silhouettes.
6. Add water material and depth/fog tuning.
7. Add vegetation and small dressing.

Instead, water shader, roads, props, Flooded Grounds assets, and vegetation were introduced before the terrain rules were stable.

### 4. 3D Art And Material Pipeline Were Underspecified

AfterBlue needs material-heavy environmental art:

- Wet wood.
- Rusted metal.
- Mossy roofs.
- Submerged asphalt.
- Concrete under transparent water.
- Depth-based water color.
- Underwater vegetation.

The project did not yet have enough Blender, UV, Substance Painter, texture, and Unity URP material knowledge to control those looks intentionally. This made the prototype dependent on whatever imported assets happened to provide.

### 5. Asset Store Packs Helped But Also Added Risk

Flooded Grounds and the water shader were useful references, but they also introduced problems:

- Old Built-in render pipeline materials appeared pink in URP.
- TreeCreator materials needed local shader patching.
- Some imported packages were large and increased local disk pressure.
- Full Asset Store source packages should not be committed to the public repository.
- Converted review scenes/materials are useful for local study, but they are not a clean production asset pipeline.

### 6. AI Assistance Needed A Stronger Creative Frame

AI accelerated implementation, but it could not replace the director-level decisions.

The missing constraints were:

- What the game must feel like.
- What the player should notice first.
- What should stay out of scope.
- Which visual compromises are acceptable.
- What the map must prove before art dressing starts.

Without those constraints, AI-generated implementation kept moving forward, but not always toward a coherent authored game.

## System Status

Fishing loop:

- Worth preserving.
- Needs UI/feedback pass.
- Needs future economy/progression validation.

Map_01:

- Current Week06 scale and boat speed findings are useful.
- Current Week07 water/asset work is a useful study, not a locked production direction.
- Underwater ground and art direction should be restarted from a stricter specification.

External assets:

- `Assets/Flooded_Grounds/` is a local ignored source import and is not committed.
- `Assets/IgniteCoders/Simple Water Shader/` exists in the repository history and should be reviewed for license/public repository safety before any public release.

## Restart Conditions

Restart AfterBlue only after these are true:

- One-page art bible is locked.
- One-page Map_01 top-down design is locked.
- Underwater height map is locked.
- A small material test board exists for water, road, concrete, wood, rust, moss, and vegetation.
- At least one small environment slice proves the intended look from the gameplay camera.
- Fishing UI feedback is prototyped.

## Recommended Restart Scope

Do not restart with the full flooded open map first.

Start with a smaller vertical slice:

- One 40 x 40m fishing location.
- One boat.
- One dock or buoy.
- One submerged road/roof cue.
- One visible fish shadow.
- One complete catch-to-reward loop.

Only expand after the small scene proves the art direction, water readability, and fishing UX.

## Final Judgment

AfterBlue's core idea remains viable, but the current Week07 map pipeline is not stable enough to continue directly.

Archive this branch as a learning snapshot. Future work should restart from stricter design locks, not from more asset placement.
