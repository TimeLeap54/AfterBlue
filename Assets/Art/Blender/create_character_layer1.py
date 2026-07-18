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


def clear_collection_objects(collection):
    for obj in list(collection.objects):
        bpy.data.objects.remove(obj, do_unlink=True)


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


def build_character_layer1(collection):
    # Gray material for pure blockout
    block_mat = bpy.data.materials.get("M_Blockout_Gray")
    if not block_mat:
        block_mat = bpy.data.materials.new("M_Blockout_Gray")
        block_mat.diffuse_color = (0.5, 0.5, 0.5, 1.0)

    # Height proportions target: Total height = 1.2m
    # Head should take 40-45% of total height (~0.48m height guide)
    # Z coordinate ranges for clean proportions:
    # - Boots: Z = 0 to 0.12 (Height: 0.12)
    # - Legs: Z = 0.12 to 0.32 (Height: 0.20)
    # - Torso: Z = 0.32 to 0.72 (Height: 0.40)
    # - Head: Z = 0.72 to 1.20 (Height: 0.48, exactly 40% of 1.2m)

    # 1. Head (Sphere)
    head_z = 0.72 + (0.48 / 2)  # Z = 0.96
    head = create_sphere(collection, "Char_Block_Head", (0.0, 0.0, head_z), 0.24, block_mat, scale=(1.0, 0.95, 1.0))

    # 2. Beanie (Large sphere overlapping top of the head)
    beanie = create_sphere(collection, "Char_Block_Beanie", (0.0, 0.03, head_z + 0.08), 0.26, block_mat, scale=(1.05, 1.05, 1.0))

    # 3. Torso (Simple cylinder)
    torso_z = 0.32 + (0.40 / 2)  # Z = 0.52
    torso = create_cylinder(collection, "Char_Block_Torso", (0.0, 0.0, torso_z), 0.16, 0.40, block_mat)

    # 4. Legs (Short cylinders)
    leg_z = 0.12 + (0.20 / 2)  # Z = 0.22
    leg_l = create_cylinder(collection, "Char_Block_Leg_L", (-0.09, 0.0, leg_z), 0.06, 0.20, block_mat)
    leg_r = create_cylinder(collection, "Char_Block_Leg_R", (0.09, 0.0, leg_z), 0.06, 0.20, block_mat)

    # 5. Boots (Simple box blocks)
    boot_l = create_cube(collection, "Char_Block_Boot_L", (-0.09, -0.02, 0.06), (0.09, 0.14, 0.12), block_mat)
    boot_r = create_cube(collection, "Char_Block_Boot_R", (0.09, -0.02, 0.06), (0.09, 0.14, 0.12), block_mat)

    # 6. Arms (Short cylinders angled slightly outward)
    arm_angle = math.radians(20)
    arm_l = create_cylinder(collection, "Char_Block_Arm_L", (-0.21, 0.0, torso_z + 0.05), 0.05, 0.26, block_mat, rotation=(0.0, arm_angle, 0.0))
    arm_r = create_cylinder(collection, "Char_Block_Arm_R", (0.21, 0.0, torso_z + 0.05), 0.05, 0.26, block_mat, rotation=(0.0, -arm_angle, 0.0))

    # 7. Hands (Simple mitten spheres)
    hand_l = create_sphere(collection, "Char_Block_Hand_L", (-0.26, -0.02, torso_z - 0.08), 0.045, block_mat)
    hand_r = create_sphere(collection, "Char_Block_Hand_R", (0.26, -0.02, torso_z - 0.08), 0.045, block_mat)

    # 8. Satchel (Simple block hanging at the hip/side)
    satchel = create_cube(collection, "Char_Block_Satchel", (-0.20, 0.10, torso_z - 0.05), (0.06, 0.14, 0.14), block_mat, rotation=(0.0, 0.0, math.radians(15)))


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
    clear_collection_objects(char_col)

    build_character_layer1(char_col)

    # Save the file
    bpy.ops.wm.save_as_mainfile(filepath=str(BLEND_PATH))
    print(f"Saved Layer 1 blockout to {BLEND_PATH}")
    
    # Render preview cameras
    render_all_views()


# Execute directly in Blender MCP environment
main()
