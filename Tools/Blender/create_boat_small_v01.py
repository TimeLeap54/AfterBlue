from pathlib import Path
import math

import bpy
from mathutils import Vector


PROJECT_ROOT = Path(r"C:\Users\minil\GameMaking\AfterBlue")
BLEND_PATH = PROJECT_ROOT / "Assets" / "Art" / "Blender" / "boat_small_v01.blend"
FBX_PATH = PROJECT_ROOT / "Assets" / "Art" / "Exports" / "boat_small_v01.fbx"


def clear_scene():
    bpy.ops.object.select_all(action="SELECT")
    bpy.ops.object.delete()


def make_material(name, color):
    mat = bpy.data.materials.new(name)
    mat.diffuse_color = color
    return mat


def cube(name, location, scale, mat, rotation=(0, 0, 0)):
    bpy.ops.mesh.primitive_cube_add(size=1, location=location, rotation=rotation)
    obj = bpy.context.object
    obj.name = name
    obj.dimensions = scale
    bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    obj.data.materials.append(mat)
    return obj


def cylinder(name, location, radius, depth, mat, rotation=(0, 0, 0), vertices=10):
    bpy.ops.mesh.primitive_cylinder_add(vertices=vertices, radius=radius, depth=depth, location=location, rotation=rotation)
    obj = bpy.context.object
    obj.name = name
    obj.data.materials.append(mat)
    return obj


def make_canoe_hull(mat):
    # Blender axes: X width, Y height, Z length. Unity imports this as a readable horizontal boat.
    stations = [
        (-1.65, 0.00),
        (-1.10, 0.52),
        (0.00, 0.76),
        (1.10, 0.52),
        (1.65, 0.00),
    ]

    verts = []
    for z, half_width in stations:
        bottom_width = max(half_width * 0.45, 0.04)
        verts.extend(
            [
                (-half_width, 0.36, z),   # outer left rim
                (half_width, 0.36, z),    # outer right rim
                (-bottom_width, 0.03, z), # lower left
                (bottom_width, 0.03, z),  # lower right
            ]
        )

    faces = []
    for i in range(len(stations) - 1):
        a = i * 4
        b = (i + 1) * 4
        faces.extend(
            [
                (a + 0, b + 0, b + 2, a + 2), # left side
                (a + 3, b + 3, b + 1, a + 1), # right side
                (a + 2, b + 2, b + 3, a + 3), # bottom
            ]
        )

    # Close bow and stern.
    faces.append((0, 2, 3, 1))
    last = (len(stations) - 1) * 4
    faces.append((last + 0, last + 1, last + 3, last + 2))

    mesh = bpy.data.meshes.new("boat_small_v01_hull_mesh")
    mesh.from_pydata(verts, [], faces)
    mesh.update()
    obj = bpy.data.objects.new("boat_small_v01_hull", mesh)
    bpy.context.collection.objects.link(obj)
    obj.data.materials.append(mat)
    return obj


def make_boat():
    hull_mat = make_material("AB_Hull_Dark_Wood", (0.36, 0.18, 0.08, 1))
    rim_mat = make_material("AB_Rim_Dark_Wood", (0.22, 0.10, 0.05, 1))
    deck_mat = make_material("AB_Deck_Warm_Wood", (0.63, 0.38, 0.17, 1))
    detail_mat = make_material("AB_Detail_Dark", (0.10, 0.07, 0.05, 1))
    glow_mat = make_material("AB_Lantern_Glow", (1.0, 0.66, 0.25, 1))

    make_canoe_hull(hull_mat)

    # Side rails, slightly angled so the top silhouette reads from a high camera.
    cube("boat_left_rail", (-0.56, 0.43, 0), (0.12, 0.10, 2.35), rim_mat, rotation=(0, 0, math.radians(-5)))
    cube("boat_right_rail", (0.56, 0.43, 0), (0.12, 0.10, 2.35), rim_mat, rotation=(0, 0, math.radians(5)))

    # Seats and small readable props.
    cube("boat_front_seat", (0, 0.50, 0.58), (1.00, 0.12, 0.26), deck_mat)
    cube("boat_back_seat", (0, 0.50, -0.68), (1.00, 0.12, 0.26), deck_mat)
    cube("boat_crate", (0.33, 0.66, -0.12), (0.34, 0.28, 0.34), deck_mat)
    cube("boat_lantern_base", (-0.33, 0.64, 0.08), (0.16, 0.16, 0.16), detail_mat)
    cube("boat_lantern_glow", (-0.33, 0.78, 0.08), (0.11, 0.11, 0.11), glow_mat)
    cylinder("boat_rod_holder", (0.56, 0.66, -0.48), 0.035, 0.82, detail_mat, rotation=(math.radians(68), 0, math.radians(8)), vertices=8)

    # Simple bow plank so forward direction is obvious.
    cube("boat_bow_plank", (0, 0.48, 1.05), (0.58, 0.08, 0.18), deck_mat)

    for obj in bpy.context.scene.objects:
        if obj.type == "MESH":
            bpy.context.view_layer.objects.active = obj
            obj.select_set(True)
            bpy.ops.object.shade_flat()
            bpy.ops.object.mode_set(mode="EDIT")
            bpy.ops.mesh.select_all(action="SELECT")
            bpy.ops.mesh.normals_make_consistent(inside=False)
            bpy.ops.object.mode_set(mode="OBJECT")
            obj.select_set(False)


def export_assets():
    BLEND_PATH.parent.mkdir(parents=True, exist_ok=True)
    FBX_PATH.parent.mkdir(parents=True, exist_ok=True)
    bpy.ops.wm.save_as_mainfile(filepath=str(BLEND_PATH))

    bpy.ops.object.select_all(action="DESELECT")
    for obj in bpy.context.scene.objects:
        if obj.type == "MESH":
            obj.select_set(True)

    bpy.ops.export_scene.fbx(
        filepath=str(FBX_PATH),
        use_selection=True,
        object_types={"MESH"},
        add_leaf_bones=False,
        bake_space_transform=False,
    )


def main():
    clear_scene()
    make_boat()
    export_assets()


if __name__ == "__main__":
    main()
