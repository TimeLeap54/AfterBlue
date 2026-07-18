import math
import os
from pathlib import Path

import bpy
from mathutils import Vector


PROJECT_ROOT = Path(r"C:\Users\minil\GameMaking\AfterBlue")
BLEND_PATH = PROJECT_ROOT / "Assets" / "Art" / "Blender" / "character_v01.blend"


def ensure_dirs():
    BLEND_PATH.parent.mkdir(parents=True, exist_ok=True)


def clear_scene():
    # Remove all objects in the scene
    bpy.ops.object.select_all(action="SELECT")
    bpy.ops.object.delete()
    
    # Remove existing collections
    for collection in list(bpy.data.collections):
        bpy.data.collections.remove(collection)


def setup_scene_properties():
    # Set scene units to Metric with 1.0 scale (1 unit = 1 meter)
    bpy.context.scene.unit_settings.system = 'METRIC'
    bpy.context.scene.unit_settings.scale_length = 1.0
    bpy.context.scene.unit_settings.length_unit = 'METERS'


def create_collections():
    collections = [
        "00_Reference",
        "01_Blockout",
        "02_Character",
        "03_Props",
        "04_Rig",
        "05_Test_Poses",
        "06_Export"
    ]
    created = {}
    for name in collections:
        col = bpy.data.collections.new(name)
        bpy.context.scene.collection.children.link(col)
        created[name] = col
    return created


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


def make_material(name, color, roughness=0.8):
    material = bpy.data.materials.new(name)
    material.diffuse_color = color
    material.use_nodes = True
    bsdf = material.node_tree.nodes.get("Principled BSDF")
    if bsdf:
        bsdf.inputs["Base Color"].default_value = color
        bsdf.inputs["Roughness"].default_value = roughness
    return material


def build_scene_and_props(cols):
    # Base Materials for simple representation
    guide_mat = make_material("M_Guide", (1.0, 0.5, 0.0, 0.3), roughness=1.0)
    wood_mat = make_material("M_Prop_Wood", (0.5, 0.35, 0.2, 1.0))
    metal_mat = make_material("M_Prop_Metal", (0.25, 0.25, 0.25, 1.0), roughness=0.5)
    light_mat = make_material("M_Prop_Light", (1.0, 0.8, 0.2, 1.0))

    # --- Layer 0: 1.2m Height Guide ---
    # Cylinder representing the character's bounding volume
    guide_height = 1.2
    guide = create_cylinder(cols["00_Reference"], "Guide_Fisherman_1.2m", (0.0, 0.0, guide_height / 2), 0.25, guide_height)
    guide.data.materials.append(guide_mat)
    # Set to display as wireframe so it doesn't block final meshes
    guide.display_type = 'WIRE'

    # --- Layer 4: Props Blockout ---
    # 1. Fishing Rod (placed next to the guide's hand position, e.g. x = 0.5, y = -0.2)
    # The Grip/Handle
    grip_len = 0.3
    grip = create_cylinder(cols["03_Props"], "Prop_FishingRod_Grip", (0.5, -0.2, 0.8), 0.025, grip_len, rotation=(0.0, 0.0, 0.0))
    grip.data.materials.append(wood_mat)
    
    # Simple Reel
    reel = create_cylinder(cols["03_Props"], "Prop_FishingRod_Reel", (0.5, -0.23, 0.88), 0.05, 0.06, rotation=(0.0, math.radians(90), 0.0))
    reel.data.materials.append(metal_mat)
    
    # Rod Shaft (Tapered long cylinder pointing slightly forward)
    rod_len = 1.8
    rod = create_cylinder(cols["03_Props"], "Prop_FishingRod_Shaft", (0.5, -0.2, 0.8 + grip_len/2 + rod_len/2), 0.015, rod_len, rotation=(math.radians(5), 0.0, 0.0))
    rod.data.materials.append(wood_mat)

    # 2. Lantern (placed on the floor, x = -0.5, y = -0.3)
    lantern_base = create_cylinder(cols["03_Props"], "Prop_Lantern_Base", (-0.5, -0.3, 0.04), 0.08, 0.08)
    lantern_base.data.materials.append(metal_mat)
    
    lantern_glass = create_cylinder(cols["03_Props"], "Prop_Lantern_Glass", (-0.5, -0.3, 0.17), 0.065, 0.18)
    lantern_glass.data.materials.append(light_mat)
    
    lantern_cap = create_cylinder(cols["03_Props"], "Prop_Lantern_Cap", (-0.5, -0.3, 0.28), 0.075, 0.04)
    lantern_cap.data.materials.append(metal_mat)

    # 3. Tackle Tin (placed on the floor, x = -0.5, y = 0.3)
    tackle_tin = create_cube(cols["03_Props"], "Prop_TackleTin", (-0.5, 0.3, 0.1), (0.28, 0.18, 0.2))
    tackle_tin.data.materials.append(metal_mat)


def setup_lighting_and_cameras():
    # Set setup camera configurations
    camera_configs = [
        {"name": "Camera_Front", "loc": (0.0, -3.5, 0.6), "rot": (90, 0, 0)},
        {"name": "Camera_Side",  "loc": (-3.5, 0.0, 0.6), "rot": (90, 0, -90)},
        {"name": "Camera_3_4",   "loc": (-2.2, -2.2, 1.4), "rot": (68, 0, -45)}
    ]
    
    for config in camera_configs:
        cam_data = bpy.data.cameras.new(config["name"])
        cam_obj = bpy.data.objects.new(config["name"], cam_data)
        bpy.context.scene.collection.objects.link(cam_obj)
        cam_obj.location = config["loc"]
        cam_obj.rotation_euler = (
            math.radians(config["rot"][0]),
            math.radians(config["rot"][1]),
            math.radians(config["rot"][2])
        )

    # Setup Sun Lights (Key and Fill)
    sun_data = bpy.data.lights.new("Light_Key", type='SUN')
    sun_obj = bpy.data.objects.new("Light_Key", sun_data)
    bpy.context.scene.collection.objects.link(sun_obj)
    sun_obj.location = (4.0, -4.0, 6.0)
    sun_data.energy = 2.5
    sun_obj.rotation_euler = (math.radians(45), 0.0, math.radians(45))

    fill_data = bpy.data.lights.new("Light_Fill", type='SUN')
    fill_obj = bpy.data.objects.new("Light_Fill", fill_data)
    bpy.context.scene.collection.objects.link(fill_obj)
    fill_obj.location = (-4.0, -2.0, 4.0)
    fill_data.energy = 1.0
    fill_obj.rotation_euler = (math.radians(60), 0.0, math.radians(-45))


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
    clear_scene()
    setup_scene_properties()
    cols = create_collections()
    
    build_scene_and_props(cols)
    setup_lighting_and_cameras()
    
    # Save the file
    bpy.ops.wm.save_as_mainfile(filepath=str(BLEND_PATH))
    print(f"Saved base scene to {BLEND_PATH}")
    
    # Render preview cameras
    render_all_views()


if __name__ == "__main__":
    main()
