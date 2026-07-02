import math
from pathlib import Path

import bpy
from mathutils import Vector


PROJECT_ROOT = Path(r"C:\Users\minil\GameMaking\AfterBlue")
BLEND_PATH = PROJECT_ROOT / "Assets" / "Art" / "Blender" / "boat_small_v01.blend"
FBX_PATH = PROJECT_ROOT / "Assets" / "Art" / "Exports" / "boat_small_v01.fbx"


def clear_scene():
    bpy.ops.object.select_all(action="SELECT")
    bpy.ops.object.delete()


def make_material(name, color):
    material = bpy.data.materials.new(name)
    material.diffuse_color = color
    return material


def create_cube(name, location, scale, material):
    bpy.ops.mesh.primitive_cube_add(size=1, location=location)
    obj = bpy.context.object
    obj.name = name
    obj.dimensions = scale
    bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    obj.data.materials.append(material)
    return obj


def create_hull(material):
    # Low-poly wedge hull. Z forward, Y up for Blender; exported to Unity as Y up by FBX.
    verts = [
        (-0.95, 0.00, -1.35),
        (0.95, 0.00, -1.35),
        (-0.72, 0.00, 0.95),
        (0.72, 0.00, 0.95),
        (0.00, 0.00, 1.55),
        (-0.72, 0.42, -1.15),
        (0.72, 0.42, -1.15),
        (-0.55, 0.42, 0.78),
        (0.55, 0.42, 0.78),
        (0.00, 0.42, 1.28),
        (-0.35, -0.22, -1.05),
        (0.35, -0.22, -1.05),
        (-0.24, -0.18, 0.8),
        (0.24, -0.18, 0.8),
        (0.00, -0.15, 1.15),
    ]

    faces = [
        (0, 1, 6, 5),
        (1, 3, 8, 6),
        (3, 4, 9, 8),
        (4, 2, 7, 9),
        (2, 0, 5, 7),
        (5, 6, 8, 9, 7),
        (0, 10, 11, 1),
        (0, 2, 12, 10),
        (2, 4, 14, 12),
        (4, 3, 13, 14),
        (3, 1, 11, 13),
        (10, 12, 14, 13, 11),
    ]

    mesh = bpy.data.meshes.new("boat_small_v01_hull_mesh")
    mesh.from_pydata(verts, [], faces)
    mesh.update()
    obj = bpy.data.objects.new("boat_small_v01_hull", mesh)
    bpy.context.collection.objects.link(obj)
    obj.data.materials.append(material)
    return obj


def create_rod_holder(material):
    bpy.ops.mesh.primitive_cylinder_add(vertices=8, radius=0.035, depth=1.1, location=(0.58, 0.62, -0.55), rotation=(math.radians(20), 0, math.radians(8)))
    obj = bpy.context.object
    obj.name = "boat_small_v01_rod_holder"
    obj.data.materials.append(material)
    return obj


def create_lantern(material_body, material_glow):
    body = create_cube("boat_small_v01_lantern_body", (-0.5, 0.58, 0.45), (0.16, 0.18, 0.16), material_body)
    glow = create_cube("boat_small_v01_lantern_glow", (-0.5, 0.68, 0.45), (0.11, 0.11, 0.11), material_glow)
    return [body, glow]


def add_origin_marker():
    bpy.ops.object.empty_add(type="PLAIN_AXES", location=(0, 0, 0))
    bpy.context.object.name = "boat_small_v01_origin"


def set_viewable_normals():
    for obj in bpy.context.scene.objects:
        if obj.type == "MESH":
            bpy.context.view_layer.objects.active = obj
            obj.select_set(True)
            bpy.ops.object.shade_flat()
            obj.select_set(False)


def export_assets():
    bpy.ops.wm.save_as_mainfile(filepath=str(BLEND_PATH))
    bpy.ops.object.select_all(action="DESELECT")
    for obj in bpy.context.scene.objects:
        if obj.type == "MESH":
            obj.select_set(True)
    bpy.ops.export_scene.fbx(
        filepath=str(FBX_PATH),
        use_selection=True,
        object_types={"MESH"},
        apply_scale_options="FBX_SCALE_ALL",
        bake_space_transform=False,
        add_leaf_bones=False,
    )


def main():
    BLEND_PATH.parent.mkdir(parents=True, exist_ok=True)
    FBX_PATH.parent.mkdir(parents=True, exist_ok=True)

    clear_scene()

    hull_mat = make_material("AB_Boat_Dark_Wood", (0.42, 0.22, 0.12, 1.0))
    deck_mat = make_material("AB_Boat_Warm_Wood", (0.66, 0.42, 0.22, 1.0))
    dark_mat = make_material("AB_Boat_Dark_Detail", (0.16, 0.10, 0.07, 1.0))
    glow_mat = make_material("AB_Lantern_Glow", (1.0, 0.68, 0.28, 1.0))

    hull = create_hull(hull_mat)
    seat_front = create_cube("boat_small_v01_front_seat", (0, 0.52, 0.48), (1.05, 0.12, 0.25), deck_mat)
    seat_back = create_cube("boat_small_v01_back_seat", (0, 0.52, -0.7), (1.15, 0.12, 0.28), deck_mat)
    crate = create_cube("boat_small_v01_crate", (0.35, 0.68, -0.28), (0.36, 0.28, 0.36), deck_mat)
    rod_holder = create_rod_holder(dark_mat)
    lantern_parts = create_lantern(dark_mat, glow_mat)

    for obj in [hull, seat_front, seat_back, crate, rod_holder, *lantern_parts]:
        obj.location.y += 0.02

    add_origin_marker()
    set_viewable_normals()
    export_assets()


if __name__ == "__main__":
    main()

