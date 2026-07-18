import math
import os
from pathlib import Path

import bpy
from mathutils import Vector


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


def apply_subdiv_and_smooth(obj, levels=1):
    if not obj:
        return
    # Apply Shade Smooth
    bpy.context.view_layer.objects.active = obj
    obj.select_set(True)
    bpy.ops.object.shade_smooth()
    
    # Add Subdivision Surface modifier
    subdiv = obj.modifiers.get("Subdiv_Preview")
    if not subdiv:
        subdiv = obj.modifiers.new(name="Subdiv_Preview", type='SUBSURF')
    subdiv.levels = levels
    subdiv.render_levels = levels
    obj.select_set(False)


def refine_silhouette():
    collection = get_or_create_collection("02_Character")
    
    # Retrieve materials
    raincoat_mat = bpy.data.materials.get("Char_Raincoat")
    beanie_mat = bpy.data.materials.get("Char_Beanie")
    pompom_mat = bpy.data.materials.get("Char_Pompom")
    bag_mat = bpy.data.materials.get("Char_Bag")
    boots_mat = bpy.data.materials.get("Char_Boots")
    skin_mat = bpy.data.materials.get("Char_Skin")
    dark_mat = bpy.data.materials.get("Char_Dark")
    
    head_z = 0.96
    torso_z = 0.52

    # ==========================================
    # 1. REFINE TORSO: Replace cylinder with a tapered A-line coat
    # ==========================================
    old_torso = bpy.data.objects.get("Char_Block_Torso")
    if old_torso:
        bpy.data.objects.remove(old_torso, do_unlink=True)
        
    # Create tapered cylinder (Cone with radius1=bottom, radius2=top)
    bpy.ops.mesh.primitive_cone_add(radius1=0.21, radius2=0.13, depth=0.40, location=(0.0, 0.0, torso_z))
    torso = link_to(collection, bpy.context.object)
    torso.name = "Char_Block_Torso"
    torso.data.materials.append(raincoat_mat)
    apply_subdiv_and_smooth(torso, levels=1)

    # ==========================================
    # 2. REFINE HOOD: Move down to neck and flatten
    # ==========================================
    hood = bpy.data.objects.get("Char_Silhouette_Hood")
    if hood:
        # Reposition to back of neck/shoulders: Z = 0.72, Y = 0.16
        hood.location = (0.0, 0.15, 0.74)
        hood.scale = (0.18, 0.12, 0.20)
        bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
        apply_subdiv_and_smooth(hood, levels=1)

    # ==========================================
    # 3. REFINE BEANIE BRIM: Make snug and thin
    # ==========================================
    brim = bpy.data.objects.get("Char_Silhouette_BeanieBrim")
    if brim:
        # Scale down radius to 0.245, depth to 0.05
        # The original cylinder radius was 0.26, depth 0.07.
        # We can scale the object dimensions directly
        brim.dimensions = (0.49, 0.49, 0.05)
        bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
        apply_subdiv_and_smooth(brim, levels=1)

    # ==========================================
    # 4. REFINE POMPOM: Shrink slightly
    # ==========================================
    pompom = bpy.data.objects.get("Char_Silhouette_Pompom")
    if pompom:
        pompom.location = (0.0, 0.05, 1.28)
        pompom.dimensions = (0.13, 0.13, 0.13)
        bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
        apply_subdiv_and_smooth(pompom, levels=1)

    # ==========================================
    # 5. REFINE HEAD & OTHER SHAPES (Shade Smooth & Subdiv)
    # ==========================================
    apply_subdiv_and_smooth(bpy.data.objects.get("Char_Block_Head"), levels=1)
    apply_subdiv_and_smooth(bpy.data.objects.get("Char_Block_Beanie"), levels=1)
    apply_subdiv_and_smooth(bpy.data.objects.get("Char_Block_Hand_L"), levels=1)
    apply_subdiv_and_smooth(bpy.data.objects.get("Char_Block_Hand_R"), levels=1)
    apply_subdiv_and_smooth(bpy.data.objects.get("Char_Block_Boot_L"), levels=1)
    apply_subdiv_and_smooth(bpy.data.objects.get("Char_Block_Boot_R"), levels=1)
    
    # Coat bottom flare subdiv
    apply_subdiv_and_smooth(bpy.data.objects.get("Char_Silhouette_CoatBottom"), levels=1)


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
    refine_silhouette()
    
    # Save the file
    bpy.ops.wm.save_as_mainfile(filepath=str(BLEND_PATH))
    print(f"Refined character shapes and saved to {BLEND_PATH}")
    
    # Render preview cameras
    render_all_views()


# Execute directly in Blender MCP environment
main()
