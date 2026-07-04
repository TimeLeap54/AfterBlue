from pathlib import Path
import math

import bpy
from mathutils import Vector


ROOT = Path(__file__).resolve().parents[2]
BLENDER_DIR = ROOT / "Assets" / "Art" / "Blender"
EXPORT_DIR = ROOT / "Assets" / "Art" / "Exports"
PREVIEW_DIR = ROOT / "Temp"


def ensure_dirs():
    BLENDER_DIR.mkdir(parents=True, exist_ok=True)
    EXPORT_DIR.mkdir(parents=True, exist_ok=True)
    PREVIEW_DIR.mkdir(parents=True, exist_ok=True)


def clear_scene():
    bpy.ops.object.select_all(action="SELECT")
    bpy.ops.object.delete()


def material(name, color, roughness=0.75):
    mat = bpy.data.materials.new(name)
    mat.diffuse_color = color
    mat.use_nodes = True
    bsdf = mat.node_tree.nodes.get("Principled BSDF")
    if bsdf:
        bsdf.inputs["Base Color"].default_value = color
        bsdf.inputs["Roughness"].default_value = roughness
    return mat


def cube(name, location, scale, mat, rotation=(0, 0, 0)):
    bpy.ops.mesh.primitive_cube_add(size=1, location=location, rotation=rotation)
    obj = bpy.context.object
    obj.name = name
    obj.dimensions = scale
    bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    if mat:
        obj.data.materials.append(mat)
    return obj


def cylinder(name, location, radius, depth, mat, vertices=10, rotation=(0, 0, 0)):
    bpy.ops.mesh.primitive_cylinder_add(vertices=vertices, radius=radius, depth=depth, location=location, rotation=rotation)
    obj = bpy.context.object
    obj.name = name
    if mat:
        obj.data.materials.append(mat)
    return obj


def cylinder_between(name, start, end, radius, mat, vertices=8):
    start = Vector(start)
    end = Vector(end)
    direction = end - start
    midpoint = start + direction * 0.5
    bpy.ops.mesh.primitive_cylinder_add(vertices=vertices, radius=radius, depth=direction.length, location=midpoint)
    obj = bpy.context.object
    obj.name = name
    obj.rotation_euler = direction.to_track_quat("Z", "Y").to_euler()
    if mat:
        obj.data.materials.append(mat)
    return obj


def set_origin_to_bottom_center():
    bpy.ops.object.select_all(action="SELECT")
    bpy.ops.object.origin_set(type="ORIGIN_GEOMETRY", center="BOUNDS")
    for obj in bpy.context.selected_objects:
        obj.location.z = max(obj.location.z, 0)


def setup_camera(target=(0, 0, 0), distance=6.0):
    bpy.ops.object.light_add(type="AREA", location=(0, -3.5, 5.5))
    light = bpy.context.object
    light.name = "Preview Softbox"
    light.data.energy = 450
    light.data.size = 4

    bpy.ops.object.camera_add(location=(3.9, -4.2, 3.1), rotation=(math.radians(61), 0, math.radians(42)))
    camera = bpy.context.object
    direction = Vector(target) - camera.location
    camera.rotation_euler = direction.to_track_quat("-Z", "Y").to_euler()
    camera.data.lens = 42
    bpy.context.scene.camera = camera
    bpy.context.scene.render.resolution_x = 1200
    bpy.context.scene.render.resolution_y = 900
    bpy.context.scene.eevee.taa_render_samples = 32


def export_asset(blend_path, fbx_path, preview_path):
    bpy.ops.wm.save_as_mainfile(filepath=str(blend_path))
    bpy.ops.export_scene.fbx(
        filepath=str(fbx_path),
        use_selection=False,
        object_types={"MESH"},
        apply_unit_scale=True,
        bake_space_transform=False,
    )
    bpy.context.scene.render.filepath = str(preview_path)
    bpy.ops.render.render(write_still=True)


def create_roof():
    clear_scene()

    wet_roof = material("wet dark roof concrete", (0.11, 0.18, 0.20, 1))
    edge = material("damp concrete edge", (0.28, 0.34, 0.35, 1))
    glass = material("dark broken skylight glass", (0.08, 0.16, 0.18, 0.72), roughness=0.35)
    moss = material("moss algae streaks", (0.22, 0.37, 0.18, 1))
    metal = material("wet oxidized metal", (0.32, 0.33, 0.31, 1))
    black = material("black roof void", (0.02, 0.03, 0.035, 1))

    cube("main flat flooded roof slab", (0, 0, 0.04), (3.5, 2.25, 0.08), wet_roof)
    cube("raised front concrete lip", (0, -1.13, 0.13), (3.7, 0.12, 0.12), edge)
    cube("raised back concrete lip", (0, 1.13, 0.13), (3.7, 0.12, 0.12), edge)
    cube("raised left concrete lip", (-1.75, 0, 0.13), (0.12, 2.1, 0.12), edge)
    cube("broken right concrete lip", (1.45, 0.42, 0.13), (0.72, 0.12, 0.12), edge, rotation=(0, 0, math.radians(3)))

    cube("jagged black roof breach", (-0.78, 0.38, 0.18), (0.95, 0.55, 0.035), black, rotation=(0, 0, math.radians(-8)))
    cube("collapsed board across breach", (-0.55, 0.38, 0.25), (1.0, 0.10, 0.08), edge, rotation=(0, 0, math.radians(18)))
    cube("short broken board", (-0.92, 0.12, 0.25), (0.55, 0.09, 0.07), edge, rotation=(0, 0, math.radians(-28)))

    cube("square skylight frame", (0.78, -0.24, 0.2), (0.68, 0.52, 0.08), edge)
    cube("dark skylight glass inset", (0.78, -0.24, 0.25), (0.52, 0.36, 0.035), glass)
    cube("small rooftop vent base", (1.18, 0.55, 0.2), (0.38, 0.30, 0.10), metal)
    cylinder("round roof turbine vent", (1.18, 0.55, 0.38), 0.16, 0.16, metal, vertices=12, rotation=(math.radians(90), 0, 0))

    cube("long algae streak", (-0.35, -0.62, 0.22), (1.25, 0.13, 0.035), moss, rotation=(0, 0, math.radians(6)))
    cube("corner algae patch", (-1.22, 0.92, 0.22), (0.68, 0.18, 0.035), moss, rotation=(0, 0, math.radians(-15)))
    cube("waterline underside hint", (0.0, 0.0, -0.035), (3.25, 1.95, 0.035), black)

    setup_camera(target=(0, 0, 0.12), distance=5.5)
    export_asset(
        BLENDER_DIR / "flooded_roof_modern_v01.blend",
        EXPORT_DIR / "flooded_roof_modern_v01.fbx",
        PREVIEW_DIR / "flooded_roof_modern_v01_preview.png",
    )


def create_utility_pole():
    clear_scene()

    rust = material("rusted wet metal", (0.42, 0.25, 0.17, 1))
    dark_rust = material("dark corroded metal", (0.18, 0.13, 0.10, 1))
    cable = material("black rubber cable", (0.015, 0.014, 0.013, 1), roughness=0.9)
    algae = material("algae waterline", (0.22, 0.36, 0.16, 1))
    ceramic = material("dirty ceramic insulator", (0.66, 0.63, 0.55, 1))

    cylinder("leaning rusted pole shaft", (0, 0, 1.05), 0.075, 2.1, rust, vertices=10)
    cube("top broken cap", (0, 0, 2.18), (0.22, 0.22, 0.18), dark_rust, rotation=(0, 0, math.radians(8)))
    cube("main cross arm", (0, 0, 1.82), (1.35, 0.11, 0.12), dark_rust, rotation=(0, 0, math.radians(-4)))
    cube("splintered secondary arm", (0.16, 0.02, 1.56), (0.95, 0.08, 0.09), rust, rotation=(0, 0, math.radians(13)))

    for x in (-0.52, 0.0, 0.52):
        cylinder(f"ceramic insulator {x}", (x, -0.07, 1.93), 0.055, 0.12, ceramic, vertices=8, rotation=(math.radians(90), 0, 0))

    cube("hanging transformer box", (0.34, 0.12, 1.18), (0.32, 0.22, 0.46), dark_rust)
    cylinder("transformer side cap a", (0.53, 0.12, 1.18), 0.08, 0.05, rust, vertices=10, rotation=(0, math.radians(90), 0))
    cylinder("transformer side cap b", (0.15, 0.12, 1.18), 0.08, 0.05, rust, vertices=10, rotation=(0, math.radians(90), 0))

    cylinder_between("cut cable left", (-0.62, -0.06, 1.90), (-1.15, -0.10, 1.56), 0.018, cable, vertices=6)
    cylinder_between("cut cable right", (0.56, -0.06, 1.90), (1.05, -0.08, 1.72), 0.018, cable, vertices=6)
    cylinder_between("dangling cable", (0.15, -0.08, 1.78), (0.02, -0.12, 1.05), 0.014, cable, vertices=6)
    cylinder_between("lamp bent arm", (-0.28, 0.02, 1.35), (-0.85, 0.02, 1.22), 0.035, rust, vertices=8)
    cylinder("broken lamp head", (-0.98, 0.02, 1.17), 0.10, 0.12, dark_rust, vertices=10, rotation=(0, math.radians(90), 0))
    cube("green algae wet base", (0, 0, 0.13), (0.20, 0.20, 0.26), algae)

    root = bpy.data.objects.new("rusted_utility_pole_v01_root_tilt", None)
    bpy.context.collection.objects.link(root)
    for obj in [obj for obj in bpy.context.scene.objects if obj.type == "MESH"]:
        obj.parent = root
    root.rotation_euler[1] = math.radians(-13)

    setup_camera(target=(0, 0, 1.1), distance=5.5)
    export_asset(
        BLENDER_DIR / "rusted_utility_pole_v01.blend",
        EXPORT_DIR / "rusted_utility_pole_v01.fbx",
        PREVIEW_DIR / "rusted_utility_pole_v01_preview.png",
    )


def main():
    ensure_dirs()
    create_roof()
    create_utility_pole()


if __name__ == "__main__":
    main()
