import math
import os
from pathlib import Path

import bpy
from mathutils import Vector


PROJECT_ROOT = Path(r"C:\Users\minil\GameMaking\AfterBlue")
BLEND_PATH = PROJECT_ROOT / "Assets" / "Art" / "Blender" / "boat_small_v01.blend"
FBX_PATH = PROJECT_ROOT / "Assets" / "Art" / "Exports" / "boat_small_v01.fbx"
PREVIEW_PATH = PROJECT_ROOT / "Temp" / "boat_small_v01_mcp_preview.png"


def clear_scene():
    bpy.ops.object.select_all(action="SELECT")
    bpy.ops.object.delete()
    for collection in list(bpy.data.collections):
        if collection.name.startswith("boat_small_v01"):
            bpy.data.collections.remove(collection)


def make_material(name, color):
    material = bpy.data.materials.new(name)
    material.diffuse_color = color
    material.use_nodes = True
    bsdf = material.node_tree.nodes.get("Principled BSDF")
    if bsdf:
        bsdf.inputs["Base Color"].default_value = color
        bsdf.inputs["Roughness"].default_value = 0.85
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


def create_cylinder(collection, name, location, radius, depth, material, rotation=(0.0, 0.0, 0.0), vertices=12):
    bpy.ops.mesh.primitive_cylinder_add(
        vertices=vertices,
        radius=radius,
        depth=depth,
        location=location,
        rotation=rotation,
    )
    obj = link_to(collection, bpy.context.object)
    obj.name = name
    obj.data.materials.append(material)
    return obj


def create_mesh(collection, name, verts, faces, material):
    mesh = bpy.data.meshes.new(f"{name}Mesh")
    mesh.from_pydata(verts, [], faces)
    mesh.update()
    obj = bpy.data.objects.new(name, mesh)
    collection.objects.link(obj)
    obj.data.materials.append(material)

    bpy.context.view_layer.objects.active = obj
    obj.select_set(True)
    bpy.ops.object.mode_set(mode="EDIT")
    bpy.ops.mesh.select_all(action="SELECT")
    bpy.ops.mesh.normals_make_consistent(inside=False)
    bpy.ops.object.mode_set(mode="OBJECT")
    obj.select_set(False)
    return obj


def build_boat(collection):
    # Blender axes: X = width, Y = length, Z = height.
    # The rowboat is built for high-angle quarter-view readability.
    hull_mat = make_material("AB_dark_hull_wood", (0.34, 0.18, 0.08, 1.0))
    plank_mat = make_material("AB_warm_plank_wood", (0.50, 0.31, 0.15, 1.0))
    rail_mat = make_material("AB_dark_rail_rope", (0.18, 0.10, 0.05, 1.0))
    post_mat = make_material("AB_weathered_post_wood", (0.36, 0.23, 0.13, 1.0))
    oar_mat = make_material("AB_oar_light_wood", (0.58, 0.40, 0.22, 1.0))

    stations = [
        (-1.70, 0.00, 0.04),
        (-1.25, 0.36, 0.16),
        (-0.65, 0.55, 0.30),
        (0.00, 0.64, 0.36),
        (0.65, 0.55, 0.30),
        (1.25, 0.36, 0.16),
        (1.70, 0.00, 0.04),
    ]

    verts = []
    for y, half_width, bottom_width in stations:
        verts.extend(
            [
                (-half_width, y, 0.42),
                (half_width, y, 0.42),
                (-bottom_width, y, 0.06),
                (bottom_width, y, 0.06),
            ]
        )

    faces = []
    for i in range(len(stations) - 1):
        a = i * 4
        b = (i + 1) * 4
        faces.extend(
            [
                (a, b, b + 2, a + 2),
                (a + 3, b + 3, b + 1, a + 1),
                (a + 2, b + 2, b + 3, a + 3),
            ]
        )
    faces.append((0, 2, 3, 1))
    last = (len(stations) - 1) * 4
    faces.append((last, last + 1, last + 3, last + 2))
    create_mesh(collection, "boat_hull_open_canoe", verts, faces, hull_mat)

    for side, sx in [("left", -1), ("right", 1)]:
        for i in range(len(stations) - 1):
            y0, w0, _ = stations[i]
            y1, w1, _ = stations[i + 1]
            mid_y = (y0 + y1) / 2
            mid_x = sx * ((w0 + w1) / 2 + 0.035)
            seg_len = abs(y1 - y0) * 0.98
            angle_z = -math.atan2(sx * (w1 - w0), y1 - y0)
            create_cube(
                collection,
                f"boat_{side}_curved_rail_{i + 1}",
                (mid_x, mid_y, 0.48),
                (0.10, seg_len, 0.10),
                rail_mat,
                rotation=(0.0, 0.0, angle_z),
            )

    for index, x in enumerate([-0.32, -0.16, 0.0, 0.16, 0.32]):
        create_cube(collection, f"boat_floor_plank_{index + 1}", (x, 0.0, 0.18), (0.13, 2.35, 0.055), plank_mat)

    for index, x in enumerate([-0.24, -0.08, 0.08, 0.24]):
        create_cube(collection, f"boat_floor_dark_gap_{index + 1}", (x, 0.0, 0.215), (0.025, 2.18, 0.012), rail_mat)

    for name, y, width in [("stern", -0.82, 0.92), ("middle", 0.0, 1.04), ("bow", 0.82, 0.86)]:
        create_cube(collection, f"boat_{name}_cross_seat", (0.0, y, 0.53), (width, 0.22, 0.12), plank_mat)

    for side, sx in [("left", -1), ("right", 1)]:
        for index, y in enumerate([-1.15, -0.82, -0.48, -0.16, 0.16, 0.48, 0.82, 1.15]):
            taper = 1 - min(abs(y) / 1.35, 1)
            x = sx * (0.28 + 0.34 * taper)
            create_cube(
                collection,
                f"boat_{side}_inner_rib_{index + 1}",
                (x, y, 0.35),
                (0.075, 0.075, 0.34),
                rail_mat,
                rotation=(0.0, math.radians(sx * 6), 0.0),
            )

    for side, sx in [("left", -1), ("right", 1)]:
        for index, y in enumerate([-1.15, -0.75, -0.35, 0.05, 0.45, 0.85, 1.20]):
            taper = 1 - min(abs(y) / 1.55, 1)
            x = sx * (0.30 + 0.32 * taper)
            create_cube(
                collection,
                f"boat_{side}_outer_side_plank_{index + 1}",
                (x, y, 0.30),
                (0.055, 0.30, 0.11),
                plank_mat,
                rotation=(0.0, math.radians(sx * 5), 0.0),
            )

    create_cube(collection, "boat_front_stem_post", (0.0, 1.62, 0.62), (0.16, 0.13, 0.50), post_mat, rotation=(math.radians(7), 0.0, 0.0))
    create_cube(collection, "boat_back_stem_post", (0.0, -1.62, 0.62), (0.16, 0.13, 0.50), post_mat, rotation=(math.radians(-7), 0.0, 0.0))

    create_cylinder(collection, "boat_center_oar_shaft", (0.04, 0.02, 0.58), 0.035, 2.60, oar_mat, rotation=(math.radians(90), 0.0, math.radians(-6)), vertices=12)
    create_cube(collection, "boat_oar_blade", (-0.10, -1.20, 0.56), (0.18, 0.34, 0.035), oar_mat, rotation=(0.0, 0.0, math.radians(-6)))
    create_cube(collection, "boat_oar_handle", (0.18, 1.20, 0.59), (0.08, 0.18, 0.045), oar_mat, rotation=(0.0, 0.0, math.radians(-6)))

    for y in [-1.47, 1.47]:
        create_cylinder(collection, f"boat_rope_wrap_{y}", (0.0, y, 0.58), 0.25, 0.035, rail_mat, rotation=(math.radians(90), 0.0, 0.0), vertices=16)

    root = bpy.data.objects.new("boat_small_v01_root", None)
    collection.objects.link(root)
    for obj in collection.objects:
        if obj != root:
            obj.parent = root

    for obj in collection.objects:
        if obj.type == "MESH":
            bpy.context.view_layer.objects.active = obj
            obj.select_set(True)
            bpy.ops.object.shade_flat()
            obj.select_set(False)


def add_preview_setup(collection):
    bpy.ops.object.light_add(type="AREA", location=(0.0, -3.8, 4.5))
    key = link_to(collection, bpy.context.object)
    key.name = "Preview_Key_Light"
    key.data.energy = 700
    key.data.size = 5

    bpy.ops.object.light_add(type="POINT", location=(-3.0, 2.0, 2.5))
    fill = link_to(collection, bpy.context.object)
    fill.name = "Preview_Fill_Light"
    fill.data.energy = 160

    cam_loc = Vector((2.55, -3.65, 2.35))
    bpy.ops.object.camera_add(location=cam_loc)
    cam = link_to(collection, bpy.context.object)
    cam.name = "Preview_Camera"
    center = Vector((0.0, 0.0, 0.38))
    cam.rotation_euler = (center - cam_loc).to_track_quat("-Z", "Y").to_euler()
    cam.data.lens = 43
    bpy.context.scene.camera = cam

    bpy.context.scene.render.engine = "BLENDER_EEVEE"
    bpy.context.scene.render.resolution_x = 1100
    bpy.context.scene.render.resolution_y = 850
    bpy.context.scene.world.color = (0.70, 0.76, 0.80)
    bpy.context.scene.view_settings.view_transform = "Standard"
    bpy.context.scene.view_settings.look = "Medium High Contrast"
    bpy.context.scene.view_settings.exposure = 0
    bpy.context.scene.view_settings.gamma = 1


def export_assets(collection):
    BLEND_PATH.parent.mkdir(parents=True, exist_ok=True)
    FBX_PATH.parent.mkdir(parents=True, exist_ok=True)
    PREVIEW_PATH.parent.mkdir(parents=True, exist_ok=True)

    bpy.ops.wm.save_as_mainfile(filepath=str(BLEND_PATH))

    bpy.ops.object.select_all(action="DESELECT")
    for obj in collection.objects:
        if obj.type == "MESH" and not obj.name.startswith("Preview_"):
            obj.select_set(True)
    bpy.ops.export_scene.fbx(
        filepath=str(FBX_PATH),
        use_selection=True,
        object_types={"MESH"},
        add_leaf_bones=False,
        bake_space_transform=False,
    )

    bpy.context.scene.render.filepath = str(PREVIEW_PATH)
    bpy.ops.render.render(write_still=True)


def main():
    clear_scene()
    collection = bpy.data.collections.new("boat_small_v01_model")
    bpy.context.scene.collection.children.link(collection)
    build_boat(collection)
    add_preview_setup(collection)
    export_assets(collection)


if __name__ == "__main__":
    main()
