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


def link_to(collection, obj):
    for existing in list(obj.users_collection):
        existing.objects.unlink(obj)
    collection.objects.link(obj)
    return obj


def create_cube(collection, name, location, scale, material, rotation=(0.0, 0.0, 0.0)):
    bpy.ops.mesh.primitive_cube_add(size=1, location=location, rotation=rotation)
    obj = link_to(collection, bpy.context.object)
    obj.name = name
    obj.dimensions = scale
    bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    obj.data.materials.append(material)
    return obj


def create_sphere(collection, name, location, radius, material, scale=(1.0, 1.0, 1.0), rotation=(0.0, 0.0, 0.0)):
    bpy.ops.mesh.primitive_uv_sphere_add(radius=radius, location=location, rotation=rotation)
    obj = link_to(collection, bpy.context.object)
    obj.name = name
    obj.scale = scale
    bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    obj.data.materials.append(material)
    return obj


def create_cylinder(collection, name, location, radius, depth, material, rotation=(0.0, 0.0, 0.0), scale=(1.0, 1.0, 1.0)):
    bpy.ops.mesh.primitive_cylinder_add(
        vertices=12,
        radius=radius,
        depth=depth,
        location=location,
        rotation=rotation
    )
    obj = link_to(collection, bpy.context.object)
    obj.name = name
    obj.scale = scale
    bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    obj.data.materials.append(material)
    return obj


def build_character_layer3(collection):
    # Setup Materials
    block_mat = bpy.data.materials.get("M_Blockout_Gray")
    if not block_mat:
        block_mat = bpy.data.materials.new("M_Blockout_Gray")
        block_mat.diffuse_color = (0.5, 0.5, 0.5, 1.0)
        
    dark_mat = bpy.data.materials.get("M_Detail_Dark")
    if not dark_mat:
        dark_mat = bpy.data.materials.new("M_Detail_Dark")
        dark_mat.diffuse_color = (0.1, 0.1, 0.1, 1.0)
        
    white_mat = bpy.data.materials.get("M_Detail_White")
    if not white_mat:
        white_mat = bpy.data.materials.new("M_Detail_White")
        white_mat.diffuse_color = (0.9, 0.9, 0.9, 1.0)

    head_z = 0.72 + (0.48 / 2)  # Z = 0.96
    torso_z = 0.32 + (0.40 / 2)  # Z = 0.52

    # --- 1. Face Details ---
    # Oval black eyes (placed on front forehead, Y = -0.23, Z = 0.94)
    eye_y = -0.228
    eye_z = head_z - 0.02 # 0.94
    eye_l = create_sphere(collection, "Char_Detail_Eye_L", (-0.08, eye_y, eye_z), 0.02, dark_mat, scale=(1.0, 0.4, 1.85))
    eye_r = create_sphere(collection, "Char_Detail_Eye_R", (0.08, eye_y, eye_z), 0.02, dark_mat, scale=(1.0, 0.4, 1.85))

    # Sad slanting eyebrows (angled down at the outer ends)
    brow_y = -0.234
    brow_z = head_z + 0.05 # 1.01
    brow_l = create_cube(collection, "Char_Detail_Brow_L", (-0.08, brow_y, brow_z), (0.05, 0.012, 0.012), dark_mat, rotation=(0.0, math.radians(-15), 0.0))
    brow_r = create_cube(collection, "Char_Detail_Brow_R", (0.08, brow_y, brow_z), (0.05, 0.012, 0.012), dark_mat, rotation=(0.0, math.radians(15), 0.0))

    # Hair clumps peaking out under beanie (forehead area, Z = 1.04)
    hair_y = -0.22
    hair_z = head_z + 0.08 # 1.04
    hair_1 = create_cube(collection, "Char_Detail_Hair_1", (-0.11, hair_y, hair_z), (0.04, 0.03, 0.05), dark_mat, rotation=(math.radians(10), 0.0, math.radians(-15)))
    hair_2 = create_cube(collection, "Char_Detail_Hair_2", (0.11, hair_y, hair_z), (0.04, 0.03, 0.05), dark_mat, rotation=(math.radians(10), 0.0, math.radians(15)))
    hair_3 = create_cube(collection, "Char_Detail_Hair_3", (0.0, hair_y - 0.01, hair_z + 0.01), (0.05, 0.03, 0.07), dark_mat, rotation=(math.radians(15), 0.0, 0.0))

    # --- 2. Raincoat Details ---
    # Front Buttons (2 buttons along center torso, Y = -0.16)
    button_y = -0.162
    btn_1 = create_cylinder(collection, "Char_Detail_Button_1", (0.0, button_y, torso_z + 0.06), 0.018, 0.01, white_mat, rotation=(math.radians(90), 0.0, 0.0))
    btn_2 = create_cylinder(collection, "Char_Detail_Button_2", (0.0, button_y, torso_z - 0.06), 0.018, 0.01, white_mat, rotation=(math.radians(90), 0.0, 0.0))

    # Pockets (2 square pockets on lower front coat, Z = 0.38)
    pocket_y = -0.182
    pocket_z = torso_z - 0.14 # 0.38
    pocket_l = create_cube(collection, "Char_Detail_Pocket_L", (-0.09, pocket_y, pocket_z), (0.065, 0.015, 0.065), block_mat, rotation=(0.0, 0.0, math.radians(-5)))
    pocket_r = create_cube(collection, "Char_Detail_Pocket_R", (0.09, pocket_y, pocket_z), (0.065, 0.015, 0.065), block_mat, rotation=(0.0, 0.0, math.radians(5)))

    # Drawstrings hanging from under chin (Z = 0.65, Y = -0.16)
    string_y = -0.16
    string_z = torso_z + 0.13 # 0.65
    string_l = create_cylinder(collection, "Char_Detail_String_L", (-0.04, string_y, string_z - 0.06), 0.006, 0.12, white_mat, rotation=(math.radians(10), 0.0, 0.0))
    string_r = create_cylinder(collection, "Char_Detail_String_R", (0.04, string_y, string_z - 0.06), 0.006, 0.12, white_mat, rotation=(math.radians(10), 0.0, 0.0))

    # --- 3. Satchel Details ---
    # The bag is at (-0.20, 0.10, 0.47)
    # Satchel Flap (overlapping top cover)
    flap = create_cube(collection, "Char_Detail_SatchelFlap", (-0.20, 0.09, 0.49), (0.065, 0.15, 0.06), block_mat, rotation=(0.0, 0.0, math.radians(15)))
    # Satchel Buckle (small dark rectangle on the front flap)
    buckle = create_cube(collection, "Char_Detail_SatchelBuckle", (-0.26, 0.04, 0.44), (0.02, 0.01, 0.035), dark_mat, rotation=(0.0, 0.0, math.radians(15)))


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
    char_col = get_or_create_collection("02_Character")
    
    # Remove existing detail objects if running repeatedly
    detail_names = [
        "Char_Detail_Eye_L", "Char_Detail_Eye_R",
        "Char_Detail_Brow_L", "Char_Detail_Brow_R",
        "Char_Detail_Hair_1", "Char_Detail_Hair_2", "Char_Detail_Hair_3",
        "Char_Detail_Button_1", "Char_Detail_Button_2",
        "Char_Detail_Pocket_L", "Char_Detail_Pocket_R",
        "Char_Detail_String_L", "Char_Detail_String_R",
        "Char_Detail_SatchelFlap", "Char_Detail_SatchelBuckle"
    ]
    for name in detail_names:
        obj = bpy.data.objects.get(name)
        if obj:
            bpy.data.objects.remove(obj, do_unlink=True)

    build_character_layer3(char_col)

    # Save the file
    bpy.ops.wm.save_as_mainfile(filepath=str(BLEND_PATH))
    print(f"Saved Layer 3 details to {BLEND_PATH}")
    
    # Render preview cameras
    render_all_views()


# Execute directly in Blender MCP environment
main()
