import math
import os
from pathlib import Path

import bpy
from mathutils import Vector


PROJECT_ROOT = Path(r"C:\Users\minil\GameMaking\AfterBlue")
BLEND_PATH = PROJECT_ROOT / "Assets" / "Art" / "Blender" / "character_v01.blend"


def ensure_dirs():
    BLEND_PATH.parent.mkdir(parents=True, exist_ok=True)


def get_or_create_collection(name):
    col = bpy.data.collections.get(name)
    if not col:
        col = bpy.data.collections.new(name)
        bpy.context.scene.collection.children.link(col)
    return col


def apply_subdiv_and_smooth(obj, levels=1):
    if not obj:
        return
    bpy.context.view_layer.objects.active = obj
    obj.select_set(True)
    bpy.ops.object.shade_smooth()
    
    subdiv = obj.modifiers.get("Subdiv_Preview")
    if not subdiv:
        subdiv = obj.modifiers.new(name="Subdiv_Preview", type='SUBSURF')
    subdiv.levels = levels
    subdiv.render_levels = levels
    obj.select_set(False)


def fix_heights():
    # 1. Beanie Brim
    brim = bpy.data.objects.get("Char_Silhouette_BeanieBrim")
    if brim:
        # Move up to the forehead
        brim.location = (0.0, 0.02, 1.04)
        brim.rotation_euler = (math.radians(10), 0.0, 0.0)
        apply_subdiv_and_smooth(brim, levels=1)
        print("Fixed Beanie Brim height")

    # 2. Beanie Pompom
    pompom = bpy.data.objects.get("Char_Silhouette_Pompom")
    if pompom:
        # Reposition on top of the beanie sphere
        pompom.location = (0.0, 0.06, 1.34)
        apply_subdiv_and_smooth(pompom, levels=1)

    # 3. Eyes (Z = 0.88, Y = -0.22)
    eye_l = bpy.data.objects.get("Char_Detail_Eye_L")
    eye_r = bpy.data.objects.get("Char_Detail_Eye_R")
    if eye_l and eye_r:
        eye_l.location = (-0.08, -0.226, 0.88)
        eye_r.location = (0.08, -0.226, 0.88)
        # Scale to make them cute oval buttons
        eye_l.scale = (1.0, 0.4, 1.7)
        eye_r.scale = (1.0, 0.4, 1.7)
        bpy.context.view_layer.objects.active = eye_l
        eye_l.select_set(True)
        bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
        eye_l.select_set(False)
        
        bpy.context.view_layer.objects.active = eye_r
        eye_r.select_set(True)
        bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
        eye_r.select_set(False)

    # 4. Eyebrows (Z = 0.94, Y = -0.23)
    brow_l = bpy.data.objects.get("Char_Detail_Brow_L")
    brow_r = bpy.data.objects.get("Char_Detail_Brow_R")
    if brow_l and brow_r:
        brow_l.location = (-0.08, -0.232, 0.94)
        brow_r.location = (0.08, -0.232, 0.94)

    # 5. Hair Clumps (Z = 1.00, Y = -0.22)
    # Reshaping to drape down under the brim onto the forehead
    hair_1 = bpy.data.objects.get("Char_Detail_Hair_1")
    hair_2 = bpy.data.objects.get("Char_Detail_Hair_2")
    hair_3 = bpy.data.objects.get("Char_Detail_Hair_3")
    
    if hair_1 and hair_2 and hair_3:
        # Left Hair Clump
        hair_1.location = (-0.11, -0.226, 0.99)
        hair_1.dimensions = (0.05, 0.025, 0.08)
        hair_1.rotation_euler = (math.radians(15), 0.0, math.radians(-15))
        bpy.context.view_layer.objects.active = hair_1
        hair_1.select_set(True)
        bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
        hair_1.select_set(False)
        
        # Right Hair Clump
        hair_2.location = (0.11, -0.226, 0.99)
        hair_2.dimensions = (0.05, 0.025, 0.08)
        hair_2.rotation_euler = (math.radians(15), 0.0, math.radians(15))
        bpy.context.view_layer.objects.active = hair_2
        hair_2.select_set(True)
        bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
        hair_2.select_set(False)
        
        # Center Hair Clump (fringe)
        hair_3.location = (0.0, -0.232, 0.98)
        hair_3.dimensions = (0.065, 0.03, 0.11)
        hair_3.rotation_euler = (math.radians(20), 0.0, 0.0)
        bpy.context.view_layer.objects.active = hair_3
        hair_3.select_set(True)
        bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
        hair_3.select_set(False)


def render_all_views():
    bpy.context.scene.render.engine = 'BLENDER_EEVEE'
    bpy.context.scene.render.resolution_x = 800
    bpy.context.scene.render.resolution_y = 600

    cameras = ["Camera_Front", "Camera_Side", "Camera_3_4"]
    temp_dir = PROJECT_ROOT / "Temp"
    temp_dir.mkdir(parents=True, exist_ok=True)

    for cam_name in cameras:
        cam_obj = bpy.data.objects.get(cam_name)
        if cam_obj:
            bpy.context.scene.camera = cam_obj
            out_name = f"scene_setup_{cam_name.split('_')[1].lower()}.png"
            out_path = temp_dir / out_name
            bpy.context.scene.render.filepath = str(out_path)
            
            # Render still frame
            bpy.ops.render.render(write_still=True)
            print(f"Rendered {cam_name} view to {out_path}")


def main():
    ensure_dirs()
    fix_heights()
    
    # Save the file
    bpy.ops.wm.save_as_mainfile(filepath=str(BLEND_PATH))
    print(f"Fixed face heights and saved to {BLEND_PATH}")
    
    # Render preview cameras
    render_all_views()


# Execute directly in Blender MCP environment
main()
