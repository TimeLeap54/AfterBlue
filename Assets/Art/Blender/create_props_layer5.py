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


def create_torus(collection, name, location, major_rad, minor_rad, material, rotation=(0.0, 0.0, 0.0), scale=(1.0, 1.0, 1.0)):
    bpy.ops.mesh.primitive_torus_add(
        major_radius=major_rad,
        minor_radius=minor_rad,
        location=location,
        rotation=rotation
    )
    obj = link_to(collection, bpy.context.object)
    obj.name = name
    obj.scale = scale
    bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    obj.data.materials.append(material)
    return obj


def build_props_layer5(collection):
    # Setup Materials
    wood_mat = bpy.data.materials.get("M_Prop_Wood")
    if not wood_mat:
        wood_mat = bpy.data.materials.new("M_Prop_Wood")
        wood_mat.diffuse_color = (0.5, 0.35, 0.2, 1.0)
        
    metal_mat = bpy.data.materials.get("M_Prop_Metal")
    if not metal_mat:
        metal_mat = bpy.data.materials.new("M_Prop_Metal")
        metal_mat.diffuse_color = (0.25, 0.25, 0.25, 1.0)
        
    light_mat = bpy.data.materials.get("M_Prop_Light")
    if not light_mat:
        light_mat = bpy.data.materials.new("M_Prop_Light")
        light_mat.diffuse_color = (1.0, 0.8, 0.2, 1.0)
        
    red_mat = bpy.data.materials.get("M_Prop_Red")
    if not red_mat:
        red_mat = bpy.data.materials.new("M_Prop_Red")
        red_mat.diffuse_color = (0.8, 0.1, 0.1, 1.0)

    white_mat = bpy.data.materials.get("M_Prop_White")
    if not white_mat:
        white_mat = bpy.data.materials.new("M_Prop_White")
        white_mat.diffuse_color = (0.9, 0.9, 0.9, 1.0)

    # ==========================================
    # 1. DETAILED FISHING ROD
    # ==========================================
    rod_base_x, rod_base_y, rod_base_z = 0.5, -0.2, 0.8
    grip_len = 0.3
    
    # 1a. Handle/Grip (Dark teal or wrapped wood look)
    grip = create_cylinder(collection, "Prop_FishingRod_Grip", (rod_base_x, rod_base_y, rod_base_z), 0.025, grip_len, wood_mat)

    # 1b. Reel Mount & Cylinder
    reel_y = rod_base_y - 0.03
    reel_z = rod_base_z + 0.08
    reel_body = create_cylinder(collection, "Prop_FishingRod_Reel", (rod_base_x, reel_y, reel_z), 0.045, 0.06, metal_mat, rotation=(0.0, math.radians(90), 0.0))
    # Reel handle pin
    reel_handle = create_cylinder(collection, "Prop_FishingRod_ReelHandle", (rod_base_x + 0.04, reel_y - 0.03, reel_z), 0.01, 0.05, metal_mat, rotation=(math.radians(90), 0.0, 0.0))

    # 1c. Tapered Rod Shaft (3 segments progressively thinner)
    # Segment 1
    seg1_len = 0.6
    seg1_z = rod_base_z + grip_len/2 + seg1_len/2
    seg1_y = rod_base_y - math.sin(math.radians(5)) * (seg1_len / 2)
    seg1 = create_cylinder(collection, "Prop_FishingRod_Shaft_Seg1", (rod_base_x, seg1_y, seg1_z), 0.016, seg1_len, wood_mat, rotation=(math.radians(5), 0.0, 0.0))
    # Segment 2
    seg2_len = 0.6
    seg2_z = seg1_z + seg1_len/2 + seg2_len/2 - 0.02
    seg2_y = rod_base_y - math.sin(math.radians(5)) * (seg1_len)
    seg2 = create_cylinder(collection, "Prop_FishingRod_Shaft_Seg2", (rod_base_x, seg2_y, seg2_z), 0.011, seg2_len, wood_mat, rotation=(math.radians(8), 0.0, 0.0))
    # Segment 3 (Tip)
    seg3_len = 0.6
    seg3_z = seg2_z + seg2_len/2 + seg3_len/2 - 0.02
    seg3_y = seg2_y - math.sin(math.radians(8)) * (seg2_len)
    seg3 = create_cylinder(collection, "Prop_FishingRod_Shaft_Seg3", (rod_base_x, seg3_y, seg3_z), 0.007, seg3_len, wood_mat, rotation=(math.radians(12), 0.0, 0.0))

    # 1d. Guide Rings (Placed along the shaft segments)
    ring_rot = (math.radians(102), 0.0, 0.0) # Aligned with the angled shaft
    create_torus(collection, "Prop_FishingRod_Guide1", (rod_base_x, seg1_y, seg1_z), 0.03, 0.005, metal_mat, rotation=ring_rot)
    create_torus(collection, "Prop_FishingRod_Guide2", (rod_base_x, seg2_y, seg2_z), 0.024, 0.004, metal_mat, rotation=ring_rot)
    
    # Rod Tip end coordinate
    tip_y = seg3_y - math.sin(math.radians(12)) * (seg3_len/2)
    tip_z = seg3_z + math.cos(math.radians(12)) * (seg3_len/2)

    # 1e. Bobber (Float: Red and White sphere with a center pin)
    bobber_loc = (rod_base_x, tip_y - 0.2, 0.3)
    bobber_body = create_sphere(collection, "Prop_FishingRod_BobberBody", bobber_loc, 0.045, red_mat)
    bobber_pin = create_cylinder(collection, "Prop_FishingRod_BobberPin", bobber_loc, 0.006, 0.12, white_mat)
    
    # 1f. Hook
    hook_loc = (rod_base_x, bobber_loc[1], 0.1)
    hook = create_torus(collection, "Prop_FishingRod_Hook", hook_loc, 0.015, 0.004, metal_mat, rotation=(0.0, math.radians(90), 0.0))

    # 1g. Fishing Line (Bezier Curve from Tip -> Bobber -> Hook)
    curve_data = bpy.data.curves.new('Prop_FishingRod_Line', type='CURVE')
    curve_data.dimensions = '3D'
    spline = curve_data.splines.new('BEZIER')
    spline.bezier_points.add(2)
    
    # Point 0: Rod Tip
    p0 = spline.bezier_points[0]
    p0.co = (rod_base_x, tip_y, tip_z)
    p0.handle_left = p0.co
    p0.handle_right = (rod_base_x, tip_y - 0.05, tip_z - 0.2)
    
    # Point 1: Bobber Top
    p1 = spline.bezier_points[1]
    p1.co = (bobber_loc[0], bobber_loc[1], bobber_loc[2] + 0.06)
    p1.handle_left = (bobber_loc[0], bobber_loc[1], bobber_loc[2] + 0.15)
    p1.handle_right = p1.co

    # Point 2: Hook Top
    p2 = spline.bezier_points[2]
    p2.co = (hook_loc[0], hook_loc[1], hook_loc[2] + 0.015)
    p2.handle_left = p2.co
    p2.handle_right = p2.co

    curve_obj = bpy.data.objects.new('Prop_FishingRod_Line', curve_data)
    link_to(collection, curve_obj)
    curve_obj.data.materials.append(white_mat)
    # Add thickness to the curve to make it visible
    curve_obj.data.bevel_depth = 0.002
    curve_obj.data.bevel_resolution = 2

    # ==========================================
    # 2. DETAILED LANTERN
    # ==========================================
    lant_x, lant_y = -0.5, -0.3
    # Base
    lant_base = create_cylinder(collection, "Prop_Lantern_Base", (lant_x, lant_y, 0.04), 0.08, 0.08, metal_mat)
    # Glass chamber
    lant_glass = create_cylinder(collection, "Prop_Lantern_Glass", (lant_x, lant_y, 0.17), 0.065, 0.18, light_mat)
    # Cap
    lant_cap = create_cylinder(collection, "Prop_Lantern_Cap", (lant_x, lant_y, 0.28), 0.075, 0.04, metal_mat)
    
    # 2a. Cage/Bars (4 thin metal rods around the glass)
    create_cylinder(collection, "Prop_Lantern_Bar1", (lant_x + 0.07, lant_y, 0.17), 0.007, 0.2, metal_mat)
    create_cylinder(collection, "Prop_Lantern_Bar2", (lant_x - 0.07, lant_y, 0.17), 0.007, 0.2, metal_mat)
    create_cylinder(collection, "Prop_Lantern_Bar3", (lant_x, lant_y + 0.07, 0.17), 0.007, 0.2, metal_mat)
    create_cylinder(collection, "Prop_Lantern_Bar4", (lant_x, lant_y - 0.07, 0.17), 0.007, 0.2, metal_mat)
    
    # 2b. Handle Arch (curved metal handle on top of the cap)
    create_torus(collection, "Prop_Lantern_Handle", (lant_x, lant_y, 0.36), 0.06, 0.008, metal_mat, rotation=(math.radians(90), 0.0, 0.0))

    # ==========================================
    # 3. DETAILED TACKLE TIN
    # ==========================================
    tin_x, tin_y = -0.5, 0.3
    # Main Box
    tin_body = create_cube(collection, "Prop_TackleTin", (tin_x, tin_y, 0.1), (0.28, 0.18, 0.2), metal_mat)
    
    # 3a. Lock Latch (Front side, Y = 0.21)
    tin_latch = create_cube(collection, "Prop_TackleTin_Latch", (tin_x, tin_y - 0.092, 0.12), (0.04, 0.01, 0.06), metal_mat)
    
    # 3b. Top Carry Handle
    tin_handle_base = create_cube(collection, "Prop_TackleTin_HandleBase", (tin_x, tin_y, 0.205), (0.12, 0.02, 0.01), metal_mat)
    tin_handle = create_torus(collection, "Prop_TackleTin_Handle", (tin_x, tin_y, 0.23), 0.05, 0.008, metal_mat, scale=(1.2, 0.4, 0.8))


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
    props_col = get_or_create_collection("03_Props")
    clear_collection_objects(props_col)

    build_props_layer5(props_col)

    # Save the file
    bpy.ops.wm.save_as_mainfile(filepath=str(BLEND_PATH))
    print(f"Saved Layer 5 props to {BLEND_PATH}")
    
    # Render preview cameras
    render_all_views()


# Execute directly in Blender MCP environment
main()
