"""
AfterBlue Fisherman - Complete Rebuild Phase 1
Layer 0: Scene Setup
Layer 1: Character Proportions Blockout
Layer 2: Character Silhouette Confirmation  
Layer 3: Character Detail Layer

Reference proportions (1.2m chibi):
- Boots: Z 0.00 ~ 0.10
- Legs:  Z 0.10 ~ 0.32
- Torso: Z 0.32 ~ 0.72  (A-line raincoat, wider at bottom)
- Neck:  Z 0.72 ~ 0.76
- Head center: Z 0.96, radius 0.20
- Beanie covers upper head: Z 0.96 ~ 1.16
- Beanie brim band: Z 0.96
- Face exposed: Z 0.76 ~ 0.96
- Eyes: Z 0.87, Nose: Z 0.82, Brows: Z 0.92
- Hair tufts peek under brim: Z 0.95
- Pompom on top: Z 1.20
"""

import math
from pathlib import Path
import bpy
from mathutils import Vector, Matrix

PROJECT_ROOT = Path(r"C:\Users\minil\GameMaking\AfterBlue")
BLEND_PATH = PROJECT_ROOT / "Assets" / "Art" / "Blender" / "character_v01.blend"

# ============================================================
# UTILITY FUNCTIONS
# ============================================================

def clear_scene():
    """Delete everything in the scene for a clean start."""
    bpy.ops.object.select_all(action='SELECT')
    bpy.ops.object.delete(use_global=False)
    # Remove orphan data
    for mesh in bpy.data.meshes:
        bpy.data.meshes.remove(mesh)
    for mat in bpy.data.materials:
        bpy.data.materials.remove(mat)
    for arm in bpy.data.armatures:
        bpy.data.armatures.remove(arm)
    for cam in bpy.data.cameras:
        bpy.data.cameras.remove(cam)
    for light in bpy.data.lights:
        bpy.data.lights.remove(light)
    for curve in bpy.data.curves:
        bpy.data.curves.remove(curve)
    for col in list(bpy.data.collections):
        bpy.data.collections.remove(col)
    print("Scene cleared")


def col(name):
    """Get or create a collection and link it to the scene."""
    c = bpy.data.collections.get(name)
    if not c:
        c = bpy.data.collections.new(name)
        bpy.context.scene.collection.children.link(c)
    return c


def link(collection, obj):
    """Move obj into collection, removing from others."""
    for ex in list(obj.users_collection):
        ex.objects.unlink(obj)
    collection.objects.link(obj)
    return obj


def mat(name, rgba, roughness=0.8, metallic=0.0, emission=None, emission_str=0.0):
    """Create a Principled BSDF material."""
    m = bpy.data.materials.get(name)
    if m:
        bpy.data.materials.remove(m)
    m = bpy.data.materials.new(name)
    m.diffuse_color = rgba
    m.use_nodes = True
    bsdf = m.node_tree.nodes.get("Principled BSDF")
    if bsdf:
        bsdf.inputs["Base Color"].default_value = rgba
        bsdf.inputs["Roughness"].default_value = roughness
        bsdf.inputs["Metallic"].default_value = metallic
        if emission:
            bsdf.inputs["Emission Color"].default_value = emission
            bsdf.inputs["Emission Strength"].default_value = emission_str
    return m


def sphere(c, name, loc, r, material, scl=(1,1,1)):
    bpy.ops.mesh.primitive_uv_sphere_add(segments=24, ring_count=16, radius=r, location=loc)
    o = link(c, bpy.context.object)
    o.name = name
    o.scale = scl
    bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    o.data.materials.append(material)
    bpy.ops.object.shade_smooth()
    o.select_set(False)
    return o


def cyl(c, name, loc, r, depth, material, rot=(0,0,0), verts=16):
    bpy.ops.mesh.primitive_cylinder_add(vertices=verts, radius=r, depth=depth, location=loc, rotation=rot)
    o = link(c, bpy.context.object)
    o.name = name
    bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    o.data.materials.append(material)
    bpy.ops.object.shade_smooth()
    o.select_set(False)
    return o


def cone(c, name, loc, r1, r2, depth, material, rot=(0,0,0)):
    bpy.ops.mesh.primitive_cone_add(vertices=24, radius1=r1, radius2=r2, depth=depth, location=loc, rotation=rot)
    o = link(c, bpy.context.object)
    o.name = name
    bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    o.data.materials.append(material)
    bpy.ops.object.shade_smooth()
    o.select_set(False)
    return o


def cube(c, name, loc, dims, material, rot=(0,0,0)):
    bpy.ops.mesh.primitive_cube_add(size=1, location=loc, rotation=rot)
    o = link(c, bpy.context.object)
    o.name = name
    o.dimensions = dims
    bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    o.data.materials.append(material)
    o.select_set(False)
    return o


def torus(c, name, loc, major, minor, material, rot=(0,0,0), scl=(1,1,1)):
    bpy.ops.mesh.primitive_torus_add(major_radius=major, minor_radius=minor, location=loc, rotation=rot)
    o = link(c, bpy.context.object)
    o.name = name
    o.scale = scl
    bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    o.data.materials.append(material)
    bpy.ops.object.shade_smooth()
    o.select_set(False)
    return o


def subdiv(obj, lvl=1):
    if not obj:
        return
    bpy.context.view_layer.objects.active = obj
    obj.select_set(True)
    mod = obj.modifiers.get("Subdiv")
    if not mod:
        mod = obj.modifiers.new("Subdiv", 'SUBSURF')
    mod.levels = lvl
    mod.render_levels = lvl
    obj.select_set(False)


# ============================================================
# LAYER 0: SCENE SETUP
# ============================================================

def layer0_scene_setup():
    # Units
    bpy.context.scene.unit_settings.system = 'METRIC'
    bpy.context.scene.unit_settings.scale_length = 1.0
    
    # Collections
    col("00_Reference")
    col("01_Blockout")
    col("02_Character")
    col("03_Props")
    col("04_Rig")
    
    # 1.2m height guide (wireframe)
    ref = col("00_Reference")
    bpy.ops.mesh.primitive_cube_add(size=1, location=(0.6, 0, 0.6))
    guide = link(ref, bpy.context.object)
    guide.name = "Guide_1.2m"
    guide.dimensions = (0.02, 0.02, 1.2)
    guide.display_type = 'WIRE'
    guide.select_set(False)
    
    # Cameras
    for cam_name, loc, rot in [
        ("Camera_Front", (0, -4, 0.6), (math.radians(90), 0, 0)),
        ("Camera_Side",  (4, 0, 0.6), (math.radians(90), 0, math.radians(90))),
        ("Camera_3_4",   (3, -3, 1.2), (math.radians(65), 0, math.radians(45))),
    ]:
        cam_data = bpy.data.cameras.new(cam_name)
        cam_data.lens = 85
        cam_obj = bpy.data.objects.new(cam_name, cam_data)
        cam_obj.location = loc
        cam_obj.rotation_euler = rot
        link(ref, cam_obj)
    
    # Sun light
    light_data = bpy.data.lights.new("Sun", 'SUN')
    light_data.energy = 3.0
    light_obj = bpy.data.objects.new("Sun", light_data)
    light_obj.location = (2, -2, 4)
    light_obj.rotation_euler = (math.radians(45), 0, math.radians(30))
    link(ref, light_obj)
    
    print("Layer 0: Scene setup complete")


# ============================================================
# MATERIALS PALETTE
# ============================================================

def create_palette():
    p = {}
    p["skin"]     = mat("M_Skin",     (1.0,  0.82, 0.65, 1), roughness=0.55)
    p["beanie"]   = mat("M_Beanie",   (0.27, 0.42, 0.47, 1), roughness=0.95)
    p["coat"]     = mat("M_Coat",     (0.35, 0.50, 0.55, 1), roughness=0.75)
    p["pants"]    = mat("M_Pants",    (0.22, 0.27, 0.26, 1), roughness=0.85)
    p["boots"]    = mat("M_Boots",    (0.33, 0.27, 0.20, 1), roughness=0.80)
    p["bag"]      = mat("M_Bag",      (0.47, 0.44, 0.36, 1), roughness=0.90)
    p["pompom"]   = mat("M_Pompom",   (0.90, 0.83, 0.73, 1), roughness=0.95)
    p["dark"]     = mat("M_Dark",     (0.06, 0.06, 0.06, 1), roughness=0.80)
    p["white"]    = mat("M_White",    (0.88, 0.88, 0.88, 1), roughness=0.80)
    p["wood"]     = mat("M_Wood",     (0.40, 0.30, 0.20, 1), roughness=0.85)
    p["metal"]    = mat("M_Metal",    (0.18, 0.22, 0.25, 1), roughness=0.45, metallic=0.7)
    p["red"]      = mat("M_Red",      (0.55, 0.08, 0.08, 1), roughness=0.70)
    p["glow"]     = mat("M_Glow",     (1.0,  0.78, 0.45, 1), roughness=0.1,
                         emission=(1.0, 0.78, 0.45, 1), emission_str=4.0)
    p["sole"]     = mat("M_Sole",     (0.15, 0.13, 0.10, 1), roughness=0.90)
    return p


# ============================================================
# LAYER 1: CHARACTER PROPORTIONS BLOCKOUT
# ============================================================

def layer1_blockout(p):
    ch = col("02_Character")
    
    # Head (big chibi sphere, ~40% of 1.2m = 0.48m diameter)
    sphere(ch, "Head", (0, 0, 0.96), 0.20, p["skin"])
    
    # Beanie (hemisphere on top of head)
    sphere(ch, "Beanie", (0, 0.01, 1.06), 0.215, p["beanie"], scl=(1, 1, 0.85))
    
    # Torso - A-line raincoat (wider at bottom)
    cone(ch, "Torso", (0, 0, 0.52), r1=0.20, r2=0.12, depth=0.40, material=p["coat"])
    
    # Arms (short stubby cylinders angled down)
    cyl(ch, "Arm_L", (-0.20, 0, 0.56), 0.055, 0.28, p["coat"], rot=(0, math.radians(15), 0))
    cyl(ch, "Arm_R", ( 0.20, 0, 0.56), 0.055, 0.28, p["coat"], rot=(0, math.radians(-15), 0))
    
    # Hands (mitten spheres)
    sphere(ch, "Hand_L", (-0.24, 0, 0.38), 0.042, p["skin"])
    sphere(ch, "Hand_R", ( 0.24, 0, 0.38), 0.042, p["skin"])
    
    # Legs
    cyl(ch, "Leg_L", (-0.07, 0, 0.22), 0.050, 0.20, p["pants"])
    cyl(ch, "Leg_R", ( 0.07, 0, 0.22), 0.050, 0.20, p["pants"])
    
    # Boots (slightly wider cylinders)
    cyl(ch, "Boot_L", (-0.07, 0, 0.06), 0.058, 0.12, p["boots"])
    cyl(ch, "Boot_R", ( 0.07, 0, 0.06), 0.058, 0.12, p["boots"])
    
    print("Layer 1: Character blockout complete")


# ============================================================
# LAYER 2: SILHOUETTE CONFIRMATION
# ============================================================

def layer2_silhouette(p):
    ch = col("02_Character")
    
    # Beanie brim band (thin torus at hat-face boundary)
    torus(ch, "BeanieBrim", (0, -0.01, 0.96), 0.215, 0.025, p["beanie"])
    
    # Pompom on top
    sphere(ch, "Pompom", (0, 0.02, 1.22), 0.06, p["pompom"])
    
    # Hood draped behind neck (flattened sphere)
    sphere(ch, "Hood", (0, 0.16, 0.72), 0.16, p["coat"], scl=(1.1, 0.6, 0.9))
    
    # Coat bottom flare (wider ring at coat hem)
    torus(ch, "CoatHem", (0, 0, 0.33), 0.20, 0.03, p["coat"])
    
    # Boot soles (flat dark discs)
    cyl(ch, "Sole_L", (-0.07, 0, 0.005), 0.062, 0.01, p["sole"])
    cyl(ch, "Sole_R", ( 0.07, 0, 0.005), 0.062, 0.01, p["sole"])
    
    # Satchel bag
    cube(ch, "Satchel", (0.22, 0.04, 0.46), (0.14, 0.07, 0.14), p["bag"])
    
    # Strap (diagonal torus across chest)
    torus(ch, "Strap", (0.02, 0.0, 0.60), 0.20, 0.012, p["bag"],
          rot=(math.radians(75), math.radians(-20), 0), scl=(0.65, 0.85, 1.2))
    
    print("Layer 2: Silhouette confirmation complete")


# ============================================================
# LAYER 3: DETAIL LAYER
# ============================================================

def layer3_details(p):
    ch = col("02_Character")
    
    # === FACE ===
    # Eyes (large cute ovals, on front of face)
    sphere(ch, "Eye_L", (-0.075, -0.19, 0.87), 0.028, p["dark"], scl=(0.8, 0.4, 1.3))
    sphere(ch, "Eye_R", ( 0.075, -0.19, 0.87), 0.028, p["dark"], scl=(0.8, 0.4, 1.3))
    
    # Eyebrows (small tilted cubes, sad slant)
    cube(ch, "Brow_L", (-0.075, -0.195, 0.92), (0.05, 0.008, 0.012), p["dark"],
         rot=(0, 0, math.radians(-12)))
    cube(ch, "Brow_R", ( 0.075, -0.195, 0.92), (0.05, 0.008, 0.012), p["dark"],
         rot=(0, 0, math.radians(12)))
    
    # Nose (tiny button)
    sphere(ch, "Nose", (0, -0.20, 0.83), 0.014, p["skin"])
    
    # Ears (on sides of head)
    sphere(ch, "Ear_L", (-0.20, -0.02, 0.90), 0.035, p["skin"], scl=(0.6, 1.0, 1.0))
    sphere(ch, "Ear_R", ( 0.20, -0.02, 0.90), 0.035, p["skin"], scl=(0.6, 1.0, 1.0))
    
    # === HAIR ===
    # Hair tufts peeking under brim onto forehead
    cube(ch, "Hair_L", (-0.10, -0.18, 0.95), (0.04, 0.02, 0.06), p["dark"],
         rot=(math.radians(10), 0, math.radians(-10)))
    cube(ch, "Hair_R", ( 0.10, -0.18, 0.95), (0.04, 0.02, 0.06), p["dark"],
         rot=(math.radians(10), 0, math.radians(10)))
    cube(ch, "Hair_C", (0, -0.19, 0.955), (0.06, 0.025, 0.07), p["dark"],
         rot=(math.radians(15), 0, 0))
    
    # Sideburns beside ears
    sphere(ch, "Sideburn_L", (-0.19, -0.07, 0.90), 0.03, p["dark"], scl=(0.7, 1.0, 1.4))
    sphere(ch, "Sideburn_R", ( 0.19, -0.07, 0.90), 0.03, p["dark"], scl=(0.7, 1.0, 1.4))
    
    # Back hair volume under beanie
    sphere(ch, "BackHair", (0, 0.15, 0.88), 0.13, p["dark"], scl=(1.1, 0.6, 0.7))
    
    # === COAT DETAILS ===
    # Collar (neck wrap)
    cyl(ch, "Collar", (0, 0, 0.73), 0.10, 0.04, p["coat"])
    
    # Buttons
    cyl(ch, "Btn1", (0, -0.175, 0.60), 0.010, 0.008, p["white"], rot=(math.radians(90), 0, 0))
    cyl(ch, "Btn2", (0, -0.195, 0.50), 0.010, 0.008, p["white"], rot=(math.radians(90), 0, 0))
    
    # Pockets
    cube(ch, "Pocket_L", (-0.10, -0.18, 0.42), (0.06, 0.008, 0.05), p["coat"])
    cube(ch, "Pocket_R", ( 0.10, -0.18, 0.42), (0.06, 0.008, 0.05), p["coat"])
    
    # Drawstrings
    cyl(ch, "String_L", (-0.025, -0.155, 0.69), 0.004, 0.07, p["white"],
        rot=(math.radians(8), 0, 0))
    cyl(ch, "String_R", ( 0.025, -0.155, 0.69), 0.004, 0.07, p["white"],
        rot=(math.radians(8), 0, 0))
    
    # Sleeve cuffs
    cyl(ch, "Cuff_L", (-0.22, 0, 0.41), 0.058, 0.03, p["coat"],
        rot=(0, math.radians(15), 0))
    cyl(ch, "Cuff_R", ( 0.22, 0, 0.41), 0.058, 0.03, p["coat"],
        rot=(0, math.radians(-15), 0))
    
    # Bag flap & buckle
    cube(ch, "BagFlap", (0.22, -0.002, 0.52), (0.145, 0.075, 0.02), p["bag"])
    cube(ch, "BagBuckle", (0.22, -0.04, 0.49), (0.025, 0.008, 0.03), p["metal"])
    
    print("Layer 3: Detail layer complete")


# ============================================================
# RENDER VIEWS
# ============================================================

def render_views():
    bpy.context.scene.render.engine = 'BLENDER_EEVEE'
    bpy.context.scene.render.resolution_x = 800
    bpy.context.scene.render.resolution_y = 600
    
    temp = PROJECT_ROOT / "Temp"
    temp.mkdir(parents=True, exist_ok=True)
    
    for cam_name, suffix in [("Camera_Front","front"), ("Camera_Side","side"), ("Camera_3_4","3")]:
        cam = bpy.data.objects.get(cam_name)
        if cam:
            bpy.context.scene.camera = cam
            path = temp / f"scene_setup_{suffix}.png"
            bpy.context.scene.render.filepath = str(path)
            bpy.ops.render.render(write_still=True)
            print(f"Rendered {suffix} to {path}")


# ============================================================
# MAIN
# ============================================================

def main():
    BLEND_PATH.parent.mkdir(parents=True, exist_ok=True)
    
    clear_scene()
    layer0_scene_setup()
    
    p = create_palette()
    
    layer1_blockout(p)
    layer2_silhouette(p)
    layer3_details(p)
    
    bpy.ops.wm.save_as_mainfile(filepath=str(BLEND_PATH))
    print(f"Saved to {BLEND_PATH}")
    
    render_views()
    print("Phase 1 (Layer 0-3) complete!")

main()
