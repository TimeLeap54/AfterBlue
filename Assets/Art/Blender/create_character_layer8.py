import math
import os
from pathlib import Path

import bpy


PROJECT_ROOT = Path(r"C:\Users\minil\GameMaking\AfterBlue")
BLEND_PATH = PROJECT_ROOT / "Assets" / "Art" / "Blender" / "character_v01.blend"


def ensure_dirs():
    BLEND_PATH.parent.mkdir(parents=True, exist_ok=True)


def clean_and_prepare_rig():
    # Deselect all objects first
    bpy.ops.object.select_all(action='DESELECT')
    
    # 1. Apply all transforms to all objects in the scene to freeze scales/rotations
    for obj in bpy.data.objects:
        if obj.type == 'MESH' or obj.type == 'CURVE':
            bpy.context.view_layer.objects.active = obj
            obj.select_set(True)
            bpy.ops.object.transform_apply(location=True, rotation=True, scale=True)
            obj.select_set(False)
    print("Applied all transforms successfully")

    # 2. Define the main body parts to join
    body_parts_names = [
        "Char_Block_Head",
        "Char_Block_Beanie",
        "Char_Silhouette_BeanieBrim",
        "Char_Silhouette_Pompom",
        "Char_Block_Torso",
        "Char_Silhouette_Hood",
        "Char_Silhouette_CoatBottom",
        "Char_Block_Arm_L",
        "Char_Block_Arm_R",
        "Char_Block_Hand_L",
        "Char_Block_Hand_R",
        "Char_Block_Leg_L",
        "Char_Block_Leg_R",
        "Char_Block_Boot_L",
        "Char_Block_Boot_R",
        "Char_Silhouette_BootSole_L",
        "Char_Silhouette_BootSole_R",
        "Char_Detail_Ear_L",
        "Char_Detail_Ear_R",
        "Char_Detail_Nose",
        "Char_Detail_Collar",
        "Char_Detail_Cuff_L",
        "Char_Detail_Cuff_R",
        "Char_Detail_Sideburn_L",
        "Char_Detail_Sideburn_R",
        "Char_Detail_BackHair"
    ]
    
    body_parts = []
    for name in body_parts_names:
        obj = bpy.data.objects.get(name)
        if obj:
            body_parts.append(obj)

    if not body_parts:
        print("Error: No body parts found to join!")
        return

    # 3. Select all body parts and make one of them active
    bpy.ops.object.select_all(action='DESELECT')
    for obj in body_parts:
        obj.select_set(True)
    
    # Set the first found object as the active one
    bpy.context.view_layer.objects.active = body_parts[0]
    
    # Join them into a single object
    bpy.ops.object.join()
    
    # Rename the joined mesh
    joined_body = bpy.context.view_layer.objects.active
    joined_body.name = "AB_CHAR_Fisherman_Body"
    print(f"Joined {len(body_parts)} objects into {joined_body.name}")

    # 4. Remove old Subdivision modifiers to prepare for Voxel Remesh
    for mod in list(joined_body.modifiers):
        joined_body.modifiers.remove(mod)

    # 5. Apply Voxel Remesh modifier to melt the seams together cleanly
    remesh_mod = joined_body.modifiers.new(name="Rig_Remesh", type='REMESH')
    remesh_mod.mode = 'VOXEL'
    remesh_mod.voxel_size = 0.012 # High resolution to keep nose, ears, and boots shape clean
    remesh_mod.use_smooth_shade = True
    
    # 6. Parent helper details (eyes, bag, buttons) to the main body
    details_to_parent = [
        "Char_Detail_Eye_L", "Char_Detail_Eye_R",
        "Char_Detail_Brow_L", "Char_Detail_Brow_R",
        "Char_Detail_Hair_1", "Char_Detail_Hair_2", "Char_Detail_Hair_3",
        "Char_Detail_Button_1", "Char_Detail_Button_2",
        "Char_Detail_Pocket_L", "Char_Detail_Pocket_R",
        "Char_Detail_String_L", "Char_Detail_String_R",
        "Char_Block_Satchel", "Char_Silhouette_Strap",
        "Char_Detail_SatchelFlap", "Char_Detail_SatchelBuckle"
    ]
    
    for det_name in details_to_parent:
        det_obj = bpy.data.objects.get(det_name)
        if det_obj:
            bpy.ops.object.select_all(action='DESELECT')
            det_obj.select_set(True)
            joined_body.select_set(True)
            bpy.context.view_layer.objects.active = joined_body
            bpy.ops.object.parent_set(type='OBJECT', keep_transform=True)
            print(f"Parented {det_name} to {joined_body.name}")


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
    clean_and_prepare_rig()
    
    # Save the file
    bpy.ops.wm.save_as_mainfile(filepath=str(BLEND_PATH))
    print(f"Rig Preparation (Layer 8) complete and saved to {BLEND_PATH}")
    
    # Render preview cameras
    render_all_views()


# Execute directly in Blender MCP environment
main()
