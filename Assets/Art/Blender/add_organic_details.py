import math
import os
from pathlib import Path

import bpy


PROJECT_ROOT = Path(r"C:\Users\minil\GameMaking\AfterBlue")
BLEND_PATH = PROJECT_ROOT / "Assets" / "Art" / "Blender" / "character_v01.blend"


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


def add_details():
    col_char = get_or_create_collection("02_Character")
    
    # Retrieve materials
    skin_mat = bpy.data.materials.get("Char_Skin")
    dark_mat = bpy.data.materials.get("Char_Dark")
    raincoat_mat = bpy.data.materials.get("Char_Raincoat")
    
    # Delete old versions of details if they exist to prevent duplicates
    detail_names = [
        "Char_Detail_Ear_L", "Char_Detail_Ear_R", "Char_Detail_Nose",
        "Char_Detail_Collar", "Char_Detail_Cuff_L", "Char_Detail_Cuff_R",
        "Char_Detail_Sideburn_L", "Char_Detail_Sideburn_R", "Char_Detail_BackHair"
    ]
    for d_name in detail_names:
        old_obj = bpy.data.objects.get(d_name)
        if old_obj:
            bpy.data.objects.remove(old_obj, do_unlink=True)

    # ==========================================
    # 1. ADD EARS (귀)
    # ==========================================
    # Placed on sides of the head (X = +/- 0.22, Z = 0.86, Y = -0.02)
    ear_l = create_sphere(col_char, "Char_Detail_Ear_L", (-0.22, -0.02, 0.86), 0.04, skin_mat, scale=(0.8, 1.0, 1.1))
    ear_r = create_sphere(col_char, "Char_Detail_Ear_R", (0.22, -0.02, 0.86), 0.04, skin_mat, scale=(0.8, 1.0, 1.1))
    apply_subdiv_and_smooth(ear_l, levels=1)
    apply_subdiv_and_smooth(ear_r, levels=1)

    # ==========================================
    # 2. ADD NOSE (코)
    # ==========================================
    # Small cute nose between eyes (Z = 0.84, Y = -0.235)
    nose = create_sphere(col_char, "Char_Detail_Nose", (0.0, -0.235, 0.84), 0.016, skin_mat)
    apply_subdiv_and_smooth(nose, levels=1)

    # ==========================================
    # 3. ADD COLLAR (목 칼라 깃)
    # ==========================================
    # Wrap neck in raincoat collar
    collar = create_cylinder(col_char, "Char_Detail_Collar", (0.0, 0.0, 0.72), 0.09, 0.05, raincoat_mat)
    apply_subdiv_and_smooth(collar, levels=1)

    # ==========================================
    # 4. ADD SLEEVE CUFFS (소매 깃)
    # ==========================================
    # Left cuff (end of arm, near hand)
    cuff_l = create_cylinder(
        col_char, "Char_Detail_Cuff_L", (-0.21, 0.0, 0.38), 0.062, 0.04, raincoat_mat,
        rotation=(0.0, math.radians(20), 0.0)
    )
    # Right cuff
    cuff_r = create_cylinder(
        col_char, "Char_Detail_Cuff_R", (0.21, 0.0, 0.38), 0.062, 0.04, raincoat_mat,
        rotation=(0.0, math.radians(-20), 0.0)
    )
    apply_subdiv_and_smooth(cuff_l, levels=1)
    apply_subdiv_and_smooth(cuff_r, levels=1)

    # ==========================================
    # 5. ADD SIDEBURNS & BACK HAIR (구레나룻 및 뒷머리)
    # ==========================================
    # Sideburns framing ears
    sb_l = create_sphere(col_char, "Char_Detail_Sideburn_L", (-0.21, -0.08, 0.89), 0.035, dark_mat, scale=(0.8, 1.2, 1.6))
    sb_r = create_sphere(col_char, "Char_Detail_Sideburn_R", (0.21, -0.08, 0.89), 0.035, dark_mat, scale=(0.8, 1.2, 1.6))
    apply_subdiv_and_smooth(sb_l, levels=1)
    apply_subdiv_and_smooth(sb_r, levels=1)
    
    # Back hair peeking under beanie
    back_hair = create_sphere(col_char, "Char_Detail_BackHair", (0.0, 0.17, 0.86), 0.14, dark_mat, scale=(1.2, 0.7, 0.8))
    apply_subdiv_and_smooth(back_hair, levels=1)


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
    add_details()
    
    # Save the file
    bpy.ops.wm.save_as_mainfile(filepath=str(BLEND_PATH))
    print(f"Added organic details and saved to {BLEND_PATH}")
    
    # Render preview cameras
    render_all_views()


# Execute directly in Blender MCP environment
main()
