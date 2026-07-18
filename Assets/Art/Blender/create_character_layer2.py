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


def create_cube(collection, name, location, scale, rotation=(0.0, 0.0, 0.0)):
    bpy.ops.mesh.primitive_cube_add(size=1, location=location, rotation=rotation)
    obj = link_to(collection, bpy.context.object)
    obj.name = name
    obj.dimensions = scale
    bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    return obj


def create_sphere(collection, name, location, radius, scale=(1.0, 1.0, 1.0), rotation=(0.0, 0.0, 0.0)):
    bpy.ops.mesh.primitive_uv_sphere_add(radius=radius, location=location, rotation=rotation)
    obj = link_to(collection, bpy.context.object)
    obj.name = name
    obj.scale = scale
    bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    return obj


def create_cylinder(collection, name, location, radius, depth, rotation=(0.0, 0.0, 0.0), scale=(1.0, 1.0, 1.0)):
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
    return obj


def build_character_layer2(collection):
    # Retrieve blockout material
    block_mat = bpy.data.materials.get("M_Blockout_Gray")
    if not block_mat:
        block_mat = bpy.data.materials.new("M_Blockout_Gray")
        block_mat.diffuse_color = (0.5, 0.5, 0.5, 1.0)

    # 1. Raincoat Hood Volume (Sphere placed behind the head/neck area)
    # Head center Z = 0.96. Hood is behind it, Z = 0.88, Y = 0.18
    hood = create_sphere(collection, "Char_Silhouette_Hood", (0.0, 0.18, 0.88), 0.16, scale=(1.1, 0.9, 1.2))
    hood.data.materials.append(block_mat)

    # 2. Raincoat torse flared bottom (Cylinder representing coat overlap)
    # Placed under torso (Z = 0.32 to 0.72) at Z = 0.36
    coat_bottom = create_cylinder(collection, "Char_Silhouette_CoatBottom", (0.0, 0.0, 0.36), 0.19, 0.12)
    coat_bottom.data.materials.append(block_mat)

    # 3. Beanie Brim/Cuff (A disk shape representing the rolled cuff of the hat)
    # Head center Z = 0.96. Beanie cuff sits around the head forehead.
    beanie_brim = create_cylinder(collection, "Char_Silhouette_BeanieBrim", (0.0, 0.02, 0.88), 0.26, 0.07, rotation=(math.radians(10), 0.0, 0.0))
    beanie_brim.data.materials.append(block_mat)

    # 4. Beanie Pompom Cluster
    # Add minor pompom detail: A main pompom sphere at Z = 1.32
    pompom = create_sphere(collection, "Char_Silhouette_Pompom", (0.0, 0.08, 1.32), 0.07)
    pompom.data.materials.append(block_mat)

    # 5. Diagonal Satchel Strap
    # Cylinder running from right shoulder to left hip bag
    # Right shoulder: (0.16, -0.05, 0.65), Left hip: (-0.16, 0.08, 0.48)
    strap_start = Vector((0.16, -0.06, 0.66))
    strap_end = Vector((-0.18, 0.08, 0.48))
    direction = strap_end - strap_start
    midpoint = strap_start + direction * 0.5
    
    # Calculate rotation to point along direction
    rot = direction.to_track_quat("Z", "Y").to_euler()
    
    strap = create_cylinder(collection, "Char_Silhouette_Strap", midpoint, 0.015, direction.length, rotation=rot)
    strap.scale = (1.5, 0.4, 1.0) # Flatten to look like a strap
    bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    strap.data.materials.append(block_mat)

    # 6. Boots Rounded Soles
    # Thin cuboids placed under the boot blocks
    sole_l = create_cube(collection, "Char_Silhouette_BootSole_L", (-0.09, -0.02, 0.015), (0.10, 0.15, 0.03))
    sole_r = create_cube(collection, "Char_Silhouette_BootSole_R", (0.09, -0.02, 0.015), (0.10, 0.15, 0.03))
    sole_l.data.materials.append(block_mat)
    sole_r.data.materials.append(block_mat)


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
    
    # Remove existing silhouette objects if running repeatedly,
    # but keep the Layer 1 blockout objects.
    sil_names = [
        "Char_Silhouette_Hood",
        "Char_Silhouette_CoatBottom",
        "Char_Silhouette_BeanieBrim",
        "Char_Silhouette_Pompom",
        "Char_Silhouette_Strap",
        "Char_Silhouette_BootSole_L",
        "Char_Silhouette_BootSole_R"
    ]
    for name in sil_names:
        obj = bpy.data.objects.get(name)
        if obj:
            bpy.data.objects.remove(obj, do_unlink=True)

    build_character_layer2(char_col)

    # Save the file
    bpy.ops.wm.save_as_mainfile(filepath=str(BLEND_PATH))
    print(f"Saved Layer 2 blockout to {BLEND_PATH}")
    
    # Render preview cameras
    render_all_views()


# Execute directly in Blender MCP environment
main()
