"""
Phase 1 Fix 2: Camera 3/4 further back, refine coat hem, minor tweaks.
"""
import math
import bpy
from pathlib import Path

PROJECT_ROOT = Path(r"C:\Users\minil\GameMaking\AfterBlue")

# Fix 3/4 camera - even further back to capture full body
cam34 = bpy.data.objects.get("Camera_3_4")
if cam34:
    cam34.location = (2.8, -2.8, 1.0)
    cam34.rotation_euler = (math.radians(78), 0, math.radians(45))

# Remove CoatHem (too prominent torus at bottom of coat)
hem = bpy.data.objects.get("CoatHem")
if hem:
    bpy.data.objects.remove(hem, do_unlink=True)
    print("Removed CoatHem")

# Move ears slightly inward so they don't intersect with eyes from side
for ear_name, x in [("Ear_L", -0.195), ("Ear_R", 0.195)]:
    ear = bpy.data.objects.get(ear_name)
    if ear:
        ear.location.x = x
        ear.location.z = 0.92

# Save and re-render
BLEND_PATH = PROJECT_ROOT / "Assets" / "Art" / "Blender" / "character_v01.blend"
bpy.ops.wm.save_as_mainfile(filepath=str(BLEND_PATH))

temp = PROJECT_ROOT / "Temp"
for cam_name, suffix in [("Camera_Front","front"), ("Camera_Side","side"), ("Camera_3_4","3")]:
    cam = bpy.data.objects.get(cam_name)
    if cam:
        bpy.context.scene.camera = cam
        path = temp / f"scene_setup_{suffix}.png"
        bpy.context.scene.render.filepath = str(path)
        bpy.ops.render.render(write_still=True)
        print(f"Rendered {suffix}")

print("Fix 2 done!")
