"""
Phase 1 Fix: Camera angle, hair softening, eye position, hide guide.
"""
import math
import bpy
from pathlib import Path

PROJECT_ROOT = Path(r"C:\Users\minil\GameMaking\AfterBlue")

# Fix 3/4 camera - bring closer and lower
cam34 = bpy.data.objects.get("Camera_3_4")
if cam34:
    cam34.location = (2.2, -2.2, 0.9)
    cam34.rotation_euler = (math.radians(72), 0, math.radians(45))

# Hide guide
guide = bpy.data.objects.get("Guide_1.2m")
if guide:
    guide.hide_render = True
    guide.hide_viewport = True

# Fix hair - add subdivision to make cubes rounder
for name in ["Hair_L", "Hair_R", "Hair_C"]:
    obj = bpy.data.objects.get(name)
    if obj:
        mod = obj.modifiers.get("Subdiv")
        if not mod:
            mod = obj.modifiers.new("Subdiv", 'SUBSURF')
        mod.levels = 2
        mod.render_levels = 2

# Also soften sideburns, brows, pockets
for name in ["Brow_L", "Brow_R", "Pocket_L", "Pocket_R"]:
    obj = bpy.data.objects.get(name)
    if obj:
        mod = obj.modifiers.get("Subdiv")
        if not mod:
            mod = obj.modifiers.new("Subdiv", 'SUBSURF')
        mod.levels = 1
        mod.render_levels = 1

# Save and re-render
BLEND_PATH = PROJECT_ROOT / "Assets" / "Art" / "Blender" / "character_v01.blend"
bpy.ops.wm.save_as_mainfile(filepath=str(BLEND_PATH))

temp = PROJECT_ROOT / "Temp"
bpy.context.scene.render.engine = 'BLENDER_EEVEE'
bpy.context.scene.render.resolution_x = 800
bpy.context.scene.render.resolution_y = 600

for cam_name, suffix in [("Camera_Front","front"), ("Camera_Side","side"), ("Camera_3_4","3")]:
    cam = bpy.data.objects.get(cam_name)
    if cam:
        bpy.context.scene.camera = cam
        path = temp / f"scene_setup_{suffix}.png"
        bpy.context.scene.render.filepath = str(path)
        bpy.ops.render.render(write_still=True)
        print(f"Rendered {suffix}")

print("Phase 1 fixes done!")
