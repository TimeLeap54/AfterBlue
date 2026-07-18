import math
import os
from pathlib import Path

import bpy
from mathutils import Vector


PROJECT_ROOT = Path(r"C:\Users\minil\GameMaking\AfterBlue")
BLEND_PATH = PROJECT_ROOT / "Assets" / "Art" / "Blender" / "character_v01.blend"
EXPORT_PATH = PROJECT_ROOT / "Assets" / "Art" / "Exports" / "character_v01.fbx"
PREVIEW_PATH = PROJECT_ROOT / "Temp" / "character_blockout_v01_preview.png"


def ensure_dirs():
    BLEND_PATH.parent.mkdir(parents=True, exist_ok=True)
    EXPORT_PATH.parent.mkdir(parents=True, exist_ok=True)
    PREVIEW_PATH.parent.mkdir(parents=True, exist_ok=True)


def clear_scene():
    bpy.ops.object.select_all(action="SELECT")
    bpy.ops.object.delete()
    for collection in list(bpy.data.collections):
        if collection.name.startswith("Character"):
            bpy.data.collections.remove(collection)


def make_material(name, color, roughness=0.8):
    material = bpy.data.materials.new(name)
    material.diffuse_color = color
    material.use_nodes = True
    bsdf = material.node_tree.nodes.get("Principled BSDF")
    if bsdf:
        bsdf.inputs["Base Color"].default_value = color
        bsdf.inputs["Roughness"].default_value = roughness
    return material


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


def create_sphere(collection, name, location, radius, material, scale=(1.0, 1.0, 1.0)):
    bpy.ops.mesh.primitive_uv_sphere_add(radius=radius, location=location)
    obj = link_to(collection, bpy.context.object)
    obj.name = name
    obj.scale = scale
    bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    obj.data.materials.append(material)
    return obj


def create_cylinder(collection, name, location, radius, depth, material, rotation=(0.0, 0.0, 0.0), scale=(1.0, 1.0, 1.0)):
    bpy.ops.mesh.primitive_cylinder_add(
        vertices=16,
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


def build_character(collection):
    # Palette colors matching the reference image
    skin_mat = make_material("Char_Skin", (0.95, 0.82, 0.73, 1.0), roughness=0.6)
    knit_mat = make_material("Char_Knit", (0.28, 0.42, 0.47, 1.0), roughness=0.9)  # #476B78
    raincoat_mat = make_material("Char_Raincoat", (0.37, 0.52, 0.57, 1.0), roughness=0.8)  # #5F8591
    pants_mat = make_material("Char_Pants", (0.24, 0.29, 0.28, 1.0), roughness=0.85)  # #3E4948
    canvas_mat = make_material("Char_Canvas", (0.48, 0.45, 0.38, 1.0), roughness=0.9)  # #7A7360
    boots_mat = make_material("Char_Boots", (0.35, 0.29, 0.22, 1.0), roughness=0.8)  # #5A4A39
    pompom_mat = make_material("Char_Pompom", (0.83, 0.77, 0.69, 1.0), roughness=0.95)
    eye_mat = make_material("Char_Eye", (0.05, 0.05, 0.05, 1.0), roughness=0.2)

    # 1. Head & Face
    # Chibi proportions: Large head at Z = 1.95
    head = create_sphere(collection, "Head", (0.0, 0.0, 1.95), 0.55, skin_mat, scale=(1.0, 0.95, 1.0))
    
    # Simple eyes to check facial direction
    create_sphere(collection, "Eye_L", (-0.18, -0.48, 1.95), 0.045, eye_mat)
    create_sphere(collection, "Eye_R", (0.18, -0.48, 1.95), 0.045, eye_mat)

    # 2. Knit Hat & Pompom
    # Hat fits over the head
    hat = create_sphere(collection, "Hat", (0.0, 0.05, 2.15), 0.6, knit_mat, scale=(1.05, 1.05, 1.0))
    pompom = create_sphere(collection, "Pompom", (0.0, 0.1, 2.85), 0.18, pompom_mat)

    # 3. Torso (Raincoat body)
    # Bulky cylinder representing the raincoat
    body = create_cylinder(collection, "Torso", (0.0, 0.0, 1.15), 0.38, 0.8, raincoat_mat)

    # 4. Arms
    # Left and Right arms angled slightly forward/outward
    arm_l = create_cylinder(collection, "Arm_L", (-0.46, -0.05, 1.15), 0.11, 0.6, raincoat_mat, rotation=(0.1, 0.2, 0.0))
    arm_r = create_cylinder(collection, "Arm_R", (0.46, -0.05, 1.15), 0.11, 0.6, raincoat_mat, rotation=(0.1, -0.2, 0.0))
    
    # Simple hands
    hand_l = create_sphere(collection, "Hand_L", (-0.52, -0.08, 0.85), 0.08, skin_mat)
    hand_r = create_sphere(collection, "Hand_R", (0.52, -0.08, 0.85), 0.08, skin_mat)

    # 5. Legs & Boots
    # Pants / Legs
    leg_l = create_cylinder(collection, "Leg_L", (-0.2, 0.0, 0.65), 0.15, 0.3, pants_mat)
    leg_r = create_cylinder(collection, "Leg_R", (0.2, 0.0, 0.65), 0.15, 0.3, pants_mat)

    # Boots
    boot_l = create_cube(collection, "Boot_L", (-0.2, -0.05, 0.4), (0.18, 0.28, 0.2), boots_mat)
    boot_r = create_cube(collection, "Boot_R", (0.2, -0.05, 0.4), (0.18, 0.28, 0.2), boots_mat)

    # 6. Sling Bag (Canvas)
    # The bag hangs at the hip/side
    bag = create_cube(collection, "SlingBag", (-0.4, 0.2, 1.0), (0.12, 0.26, 0.28), canvas_mat, rotation=(0.0, 0.0, 0.2))


def setup_lighting_and_camera():
    # Setup Camera
    if "Camera" in bpy.data.objects:
        camera = bpy.data.objects["Camera"]
    else:
        bpy.ops.object.camera_add()
        camera = bpy.context.object
        camera.name = "Camera"
    
    # Position camera for a good front-quarter view
    camera.location = (0.0, -3.2, 1.7)
    camera.rotation_euler = (math.radians(78), 0.0, 0.0)
    
    # Set as active camera for the scene
    bpy.context.scene.camera = camera

    # Setup Key Light
    if "KeyLight" in bpy.data.objects:
        key_light = bpy.data.objects["KeyLight"]
    else:
        bpy.ops.object.light_add(type='SUN')
        key_light = bpy.context.object
        key_light.name = "KeyLight"
    
    key_light.location = (3.0, -3.0, 5.0)
    key_light.data.energy = 3.0
    key_light.rotation_euler = (math.radians(45), 0.0, math.radians(45))

    # Setup Fill Light
    if "FillLight" in bpy.data.objects:
        fill_light = bpy.data.objects["FillLight"]
    else:
        bpy.ops.object.light_add(type='SUN')
        fill_light = bpy.context.object
        fill_light.name = "FillLight"
    
    fill_light.location = (-3.0, -2.0, 3.0)
    fill_light.data.energy = 1.0
    fill_light.rotation_euler = (math.radians(60), 0.0, math.radians(-45))


def render_preview():
    # Set render engine to EEVEE
    bpy.context.scene.render.engine = 'BLENDER_EEVEE'
    bpy.context.scene.render.resolution_x = 800
    bpy.context.scene.render.resolution_y = 600
    bpy.context.scene.render.filepath = str(PREVIEW_PATH)
    
    # Render scene
    bpy.ops.render.render(write_still=True)
    print(f"Viewport preview rendered to: {PREVIEW_PATH}")


def main():
    ensure_dirs()
    clear_scene()

    char_collection = bpy.data.collections.new("Character_Blockout")
    bpy.context.scene.collection.children.link(char_collection)

    build_character(char_collection)
    setup_lighting_and_camera()

    # Save blender scene
    bpy.ops.wm.save_as_mainfile(filepath=str(BLEND_PATH))
    print(f"Blender project saved to: {BLEND_PATH}")

    # Render thumbnail preview
    render_preview()


if __name__ == "__main__":
    main()
