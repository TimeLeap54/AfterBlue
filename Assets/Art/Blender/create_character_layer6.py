import math
import os
from pathlib import Path

import bpy


PROJECT_ROOT = Path(r"C:\Users\minil\GameMaking\AfterBlue")
BLEND_PATH = PROJECT_ROOT / "Assets" / "Art" / "Blender" / "character_v01.blend"


def ensure_dirs():
    BLEND_PATH.parent.mkdir(parents=True, exist_ok=True)


def make_flat_material(name, color_rgba, roughness=0.8, metallic=0.0, emission_color=None, emission_strength=1.0):
    # Check if material already exists, if so delete to recreate cleanly
    mat = bpy.data.materials.get(name)
    if mat:
        bpy.data.materials.remove(mat)
        
    mat = bpy.data.materials.new(name)
    mat.diffuse_color = color_rgba
    mat.use_nodes = True
    
    nodes = mat.node_tree.nodes
    links = mat.node_tree.links
    
    bsdf = nodes.get("Principled BSDF")
    if bsdf:
        bsdf.inputs["Base Color"].default_value = color_rgba
        bsdf.inputs["Roughness"].default_value = roughness
        bsdf.inputs["Metallic"].default_value = metallic
        
        # Setup emission (glowing) for lantern light
        if emission_color:
            bsdf.inputs["Emission Color"].default_value = emission_color
            bsdf.inputs["Emission Strength"].default_value = emission_strength
            
    return mat


def setup_palette():
    # Palette definition from the reference sheet
    palette = {
        "Char_Beanie": make_flat_material("Char_Beanie", (0.28, 0.42, 0.47, 1.0), roughness=0.95), # #476B78
        "Char_Raincoat": make_flat_material("Char_Raincoat", (0.37, 0.52, 0.57, 1.0), roughness=0.80), # #5F8591
        "Char_Pants": make_flat_material("Char_Pants", (0.24, 0.29, 0.28, 1.0), roughness=0.85), # #3E4948
        "Char_Boots": make_flat_material("Char_Boots", (0.35, 0.29, 0.22, 1.0), roughness=0.80), # #5A4A39
        "Char_Bag": make_flat_material("Char_Bag", (0.48, 0.45, 0.38, 1.0), roughness=0.90), # #7A7360
        "Char_Pompom": make_flat_material("Char_Pompom", (0.92, 0.85, 0.75, 1.0), roughness=0.95), # Warm Cream
        "Char_Skin": make_flat_material("Char_Skin", (1.0, 0.85, 0.69, 1.0), roughness=0.60), # Warm Peach
        "Char_Dark": make_flat_material("Char_Dark", (0.07, 0.07, 0.07, 1.0), roughness=0.80), # Eyes, Hair, Eyebrows
        
        "Prop_Wood": make_flat_material("Prop_Wood", (0.35, 0.29, 0.22, 1.0), roughness=0.85), # Muted Brown
        "Prop_Metal": make_flat_material("Prop_Metal", (0.18, 0.24, 0.27, 1.0), roughness=0.50, metallic=0.7), # #2F3E46
        "Prop_Light": make_flat_material("Prop_Light", (1.0, 0.79, 0.47, 1.0), roughness=0.1, emission_color=(1.0, 0.79, 0.47, 1.0), emission_strength=4.0), # Glow
        "Prop_Red": make_flat_material("Prop_Red", (0.5, 0.06, 0.06, 1.0), roughness=0.70), # Bobber Red
        "Prop_White": make_flat_material("Prop_White", (0.88, 0.88, 0.88, 1.0), roughness=0.80), # Bobber White
    }
    return palette


def assign_materials(palette):
    assignments = {
        # --- Character meshes ---
        "Char_Block_Head": palette["Char_Skin"],
        "Char_Block_Beanie": palette["Char_Beanie"],
        "Char_Block_Torso": palette["Char_Raincoat"],
        "Char_Block_Leg_L": palette["Char_Pants"],
        "Char_Block_Leg_R": palette["Char_Pants"],
        "Char_Block_Boot_L": palette["Char_Boots"],
        "Char_Block_Boot_R": palette["Char_Boots"],
        "Char_Block_Arm_L": palette["Char_Raincoat"],
        "Char_Block_Arm_R": palette["Char_Raincoat"],
        "Char_Block_Hand_L": palette["Char_Skin"],
        "Char_Block_Hand_R": palette["Char_Skin"],
        "Char_Block_Satchel": palette["Char_Bag"],
        
        "Char_Silhouette_Hood": palette["Char_Raincoat"],
        "Char_Silhouette_CoatBottom": palette["Char_Raincoat"],
        "Char_Silhouette_BeanieBrim": palette["Char_Beanie"],
        "Char_Silhouette_Pompom": palette["Char_Pompom"],
        "Char_Silhouette_Strap": palette["Char_Bag"],
        "Char_Silhouette_BootSole_L": palette["Char_Dark"],
        "Char_Silhouette_BootSole_R": palette["Char_Dark"],
        
        "Char_Detail_Eye_L": palette["Char_Dark"],
        "Char_Detail_Eye_R": palette["Char_Dark"],
        "Char_Detail_Brow_L": palette["Char_Dark"],
        "Char_Detail_Brow_R": palette["Char_Dark"],
        "Char_Detail_Hair_1": palette["Char_Dark"],
        "Char_Detail_Hair_2": palette["Char_Dark"],
        "Char_Detail_Hair_3": palette["Char_Dark"],
        "Char_Detail_Button_1": palette["Prop_White"],
        "Char_Detail_Button_2": palette["Prop_White"],
        "Char_Detail_Pocket_L": palette["Char_Raincoat"],
        "Char_Detail_Pocket_R": palette["Char_Raincoat"],
        "Char_Detail_String_L": palette["Prop_White"],
        "Char_Detail_String_R": palette["Prop_White"],
        "Char_Detail_SatchelFlap": palette["Char_Bag"],
        "Char_Detail_SatchelBuckle": palette["Prop_Metal"],
        
        # --- Prop meshes ---
        "Prop_FishingRod_Grip": palette["Char_Beanie"], # Wrapped grip
        "Prop_FishingRod_Reel": palette["Prop_Metal"],
        "Prop_FishingRod_ReelHandle": palette["Prop_Metal"],
        "Prop_FishingRod_Shaft_Seg1": palette["Prop_Wood"],
        "Prop_FishingRod_Shaft_Seg2": palette["Prop_Wood"],
        "Prop_FishingRod_Shaft_Seg3": palette["Prop_Wood"],
        "Prop_FishingRod_Guide1": palette["Prop_Metal"],
        "Prop_FishingRod_Guide2": palette["Prop_Metal"],
        "Prop_FishingRod_BobberBody": palette["Prop_Red"],
        "Prop_FishingRod_BobberPin": palette["Prop_White"],
        "Prop_FishingRod_Hook": palette["Prop_Metal"],
        "Prop_FishingRod_Line": palette["Prop_White"],
        
        "Prop_Lantern_Base": palette["Prop_Metal"],
        "Prop_Lantern_Glass": palette["Prop_Light"],
        "Prop_Lantern_Cap": palette["Prop_Metal"],
        "Prop_Lantern_Bar1": palette["Prop_Metal"],
        "Prop_Lantern_Bar2": palette["Prop_Metal"],
        "Prop_Lantern_Bar3": palette["Prop_Metal"],
        "Prop_Lantern_Bar4": palette["Prop_Metal"],
        "Prop_Lantern_Handle": palette["Prop_Metal"],
        
        "Prop_TackleTin": palette["Prop_Metal"],
        "Prop_TackleTin_Latch": palette["Prop_Metal"],
        "Prop_TackleTin_HandleBase": palette["Prop_Metal"],
        "Prop_TackleTin_Handle": palette["Prop_Metal"],
    }
    
    for obj_name, material in assignments.items():
        obj = bpy.data.objects.get(obj_name)
        if obj:
            # Clear existing materials
            obj.data.materials.clear()
            # Assign the new material
            obj.data.materials.append(material)
            print(f"Assigned material {material.name} to {obj_name}")
        else:
            print(f"Warning: Object {obj_name} not found in scene")


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
    palette = setup_palette()
    assign_materials(palette)

    # Save the file
    bpy.ops.wm.save_as_mainfile(filepath=str(BLEND_PATH))
    print(f"Saved Layer 6 flat colors to {BLEND_PATH}")
    
    # Render preview cameras
    render_all_views()


# Execute directly in Blender MCP environment
main()
