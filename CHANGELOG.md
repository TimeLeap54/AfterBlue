# Changelog

## 2026-07-03

- Initialized AfterBlue Unity URP project baseline.
- Added project design, technical, art direction, data schema, Codex rules, and task documents.
- Added initial boat movement, follow camera, fishing state, and data scripts.
- Started Week 1 boat feel pass on `codex/week1-boat-feel`.
- Installed and verified local Blender MCP bridge for scene inspection and asset editing.
- Generated first Blender low-poly boat model and exported a Unity FBX.
- Applied first-pass flooded scenery props and updated camera/movement defaults.

## 2026-07-04

- Added Unity MCP project setup and editor settings.
- Started Week 2 fishing loop implementation on `codex/week2-fishing-loop`.
- Added runtime fishing state machine, bobber casting, fishing line, bite timing, temporary fish roll table, and debug fishing HUD.
- Started Week 3 flooded village visual pass on `codex/week3-flooded-village`.
- Added cyan flooded village palette setup, primitive modern ruin props, and bobber water ripple effects.
- Removed Week 3 fish shadow scene generation after readability testing; keep the script for a later polish pass.
- Replaced the over-strong water line overlay with softer cyan water patches and subtle surface glints.
- Added a broader moving wave-band overlay and brightened the Week 3 water base so the surface reads as water instead of a dark flat plane.
- Switched the Week 3 water surface from stacked transparent texture overlays to a lightweight procedural URP water shader with sine/cosine wave color bands and vertex motion.
- Removed the club-like straight wave-band overlay path and replaced it with subtle procedural cellular caustics plus low-amplitude Y-axis wave motion.
- Ensured Week 3 setup restores the Blender boat FBX and reduced water caustic density/strength for readability.
- Added Blender-generated low-poly flooded roof and rusted utility pole assets for the Week 3 modern submerged village pass.
- Added generated grime/slate/rust texture atlases and rebuilt the Week 3 roof and utility pole assets with more visible surface detail.

## 2026-07-06

- Switched Blender integration to the official Blender Lab MCP server and verified `execute_blender_code`.
- Started Week 4 collection-data pass.
- Added FishData catch weights and bite windows.
- Added BaitData and RodData schemas plus starter data assets.
- Added eight flooded-village fish data assets and the first LocationData asset.
- Replaced the hardcoded fish roll table with a FishData-driven roll table.
- Added a prototype fish collection log, PlayerPrefs JSON persistence, and a Tab/J debug codex UI.
