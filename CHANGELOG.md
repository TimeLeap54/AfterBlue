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
