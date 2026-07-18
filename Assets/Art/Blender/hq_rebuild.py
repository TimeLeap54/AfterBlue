"""
AfterBlue Fisherman - HIGH QUALITY REBUILD
Layer 0: Scene Setup with 4-directional cameras
Layer 1-3: Character modeling using BMesh, Subdivision, proper topology

Reference analysis:
- Big round head ~40% of 1.2m height
- Soft vinyl/clay toy aesthetic
- Knitted teal beanie with cream fluffy pompom, ribbed brim
- Dark messy hair visible under brim and sides
- Large black oval eyes, tiny brows, small nose, neutral/slight frown mouth
- Teal/dark-teal hooded raincoat, A-line silhouette
- Hood draped behind neck
- Drawstrings at collar with small toggles
- Two front pockets, center snap buttons
- Dark pants, brown leather boots with darker soles
- Khaki canvas crossbody satchel bag
- Overall soft roundness, no sharp edges anywhere
"""

import math
import bmesh
import bpy
from mathutils import Vector, Matrix
from pathlib import Path

PROJECT_ROOT = Path(r"C:\Users\minil\GameMaking\AfterBlue")
BLEND_PATH = PROJECT_ROOT / "Assets" / "Art" / "Blender" / "character_v01.blend"
TEMP = PROJECT_ROOT / "Temp"
TEMP.mkdir(parents=True, exist_ok=True)

# ============================================================
# CLEAR EVERYTHING
# ============================================================
bpy.ops.object.select_all(action='SELECT')
bpy.ops.object.delete(use_global=False)
for block_type in [bpy.data.meshes, bpy.data.materials, bpy.data.armatures,
                   bpy.data.cameras, bpy.data.lights, bpy.data.curves,
                   bpy.data.node_groups]:
    for b in list(block_type):
        block_type.remove(b)
for c in list(bpy.data.collections):
    bpy.data.collections.remove(c)
print("Scene cleared")

# ============================================================
# LAYER 0: SCENE SETUP
# ============================================================
bpy.context.scene.unit_settings.system = 'METRIC'
bpy.context.scene.unit_settings.scale_length = 1.0
bpy.context.scene.render.engine = 'BLENDER_EEVEE'
bpy.context.scene.render.resolution_x = 800
bpy.context.scene.render.resolution_y = 800

# World background
world = bpy.data.worlds.get("World")
if world and world.use_nodes:
    bg = world.node_tree.nodes.get("Background")
    if bg:
        bg.inputs[0].default_value = (0.85, 0.85, 0.85, 1)
        bg.inputs[1].default_value = 1.0

# Collections
def get_col(name):
    c = bpy.data.collections.get(name)
    if not c:
        c = bpy.data.collections.new(name)
        bpy.context.scene.collection.children.link(c)
    return c

col_ref = get_col("Reference")
col_char = get_col("Character")
col_props = get_col("Props")
col_rig = get_col("Rig")

# 4 Cameras: Front, Back, Left, Right
cam_defs = [
    ("Cam_Front", (0, -3.2, 0.6), (math.radians(90), 0, 0)),
    ("Cam_Back",  (0,  3.2, 0.6), (math.radians(90), 0, math.radians(180))),
    ("Cam_Left",  (-3.2, 0, 0.6), (math.radians(90), 0, math.radians(-90))),
    ("Cam_Right", ( 3.2, 0, 0.6), (math.radians(90), 0, math.radians(90))),
]
for cname, loc, rot in cam_defs:
    cd = bpy.data.cameras.new(cname)
    cd.lens = 80
    co = bpy.data.objects.new(cname, cd)
    co.location = loc
    co.rotation_euler = rot
    col_ref.objects.link(co)

# Lights - 3-point setup
sun = bpy.data.lights.new("Key_Light", 'SUN')
sun.energy = 3.0
sun.color = (1.0, 0.97, 0.92)
sun_obj = bpy.data.objects.new("Key_Light", sun)
sun_obj.location = (2, -2, 4)
sun_obj.rotation_euler = (math.radians(50), math.radians(10), math.radians(30))
col_ref.objects.link(sun_obj)

fill = bpy.data.lights.new("Fill_Light", 'AREA')
fill.energy = 50.0
fill.color = (0.8, 0.85, 1.0)
fill.size = 3.0
fill_obj = bpy.data.objects.new("Fill_Light", fill)
fill_obj.location = (-2.5, -1.5, 2)
fill_obj.rotation_euler = (math.radians(60), 0, math.radians(-45))
col_ref.objects.link(fill_obj)

rim = bpy.data.lights.new("Rim_Light", 'AREA')
rim.energy = 30.0
rim.color = (1.0, 0.95, 0.85)
rim.size = 2.0
rim_obj = bpy.data.objects.new("Rim_Light", rim)
rim_obj.location = (1, 3, 2.5)
rim_obj.rotation_euler = (math.radians(50), 0, math.radians(160))
col_ref.objects.link(rim_obj)

# Floor plane (subtle shadow catcher)
bpy.ops.mesh.primitive_plane_add(size=10, location=(0, 0, 0))
floor = bpy.context.object
floor.name = "Floor"
for ec in list(floor.users_collection):
    ec.objects.unlink(floor)
col_ref.objects.link(floor)
floor_mat = bpy.data.materials.new("M_Floor")
floor_mat.use_nodes = True
floor_bsdf = floor_mat.node_tree.nodes.get("Principled BSDF")
if floor_bsdf:
    floor_bsdf.inputs["Base Color"].default_value = (0.9, 0.9, 0.9, 1)
    floor_bsdf.inputs["Roughness"].default_value = 0.95
floor.data.materials.append(floor_mat)
floor.select_set(False)

print("Layer 0: Scene setup with 4 cameras + 3-point lighting")


# ============================================================
# MATERIALS - Full Shader Nodes
# ============================================================

def make_mat(name, color, rough=0.7, subsurface=0.0, ss_color=None, bump_scale=0.0, bump_noise_scale=5.0):
    """Create a material with optional subsurface scattering and procedural bump."""
    m = bpy.data.materials.new(name)
    m.use_nodes = True
    m.diffuse_color = (*color, 1.0)
    nt = m.node_tree
    nodes = nt.nodes
    links = nt.links
    
    bsdf = nodes.get("Principled BSDF")
    out = nodes.get("Material Output")
    bsdf.inputs["Base Color"].default_value = (*color, 1.0)
    bsdf.inputs["Roughness"].default_value = rough
    
    if subsurface > 0:
        bsdf.inputs["Subsurface Weight"].default_value = subsurface
        if ss_color:
            bsdf.inputs["Subsurface Radius"].default_value = ss_color
    
    if bump_scale > 0:
        # Add noise texture -> bump node
        noise = nodes.new('ShaderNodeTexNoise')
        noise.inputs["Scale"].default_value = bump_noise_scale
        noise.inputs["Detail"].default_value = 8.0
        noise.inputs["Roughness"].default_value = 0.6
        
        bump = nodes.new('ShaderNodeBump')
        bump.inputs["Strength"].default_value = bump_scale
        
        links.new(noise.outputs["Fac"], bump.inputs["Height"])
        links.new(bump.outputs["Normal"], bsdf.inputs["Normal"])
    
    return m


def make_knit_mat(name, color, rough=0.85):
    """Create a knitted fabric material with ribbed bump pattern."""
    m = bpy.data.materials.new(name)
    m.use_nodes = True
    m.diffuse_color = (*color, 1.0)
    nt = m.node_tree
    nodes = nt.nodes
    links = nt.links
    
    bsdf = nodes.get("Principled BSDF")
    bsdf.inputs["Base Color"].default_value = (*color, 1.0)
    bsdf.inputs["Roughness"].default_value = rough
    
    # Knit pattern: wave texture (ribbing) + noise (yarn variation)
    tex_coord = nodes.new('ShaderNodeTexCoord')
    mapping = nodes.new('ShaderNodeMapping')
    mapping.inputs["Scale"].default_value = (20, 40, 20)
    links.new(tex_coord.outputs["Object"], mapping.inputs["Vector"])
    
    wave = nodes.new('ShaderNodeTexWave')
    wave.wave_type = 'BANDS'
    wave.bands_direction = 'Z'
    wave.inputs["Scale"].default_value = 12.0
    wave.inputs["Distortion"].default_value = 2.0
    wave.inputs["Detail"].default_value = 4.0
    wave.inputs["Detail Scale"].default_value = 3.0
    links.new(mapping.outputs["Vector"], wave.inputs["Vector"])
    
    noise = nodes.new('ShaderNodeTexNoise')
    noise.inputs["Scale"].default_value = 60.0
    noise.inputs["Detail"].default_value = 6.0
    links.new(mapping.outputs["Vector"], noise.inputs["Vector"])
    
    mix_bump = nodes.new('ShaderNodeMath')
    mix_bump.operation = 'ADD'
    links.new(wave.outputs["Fac"], mix_bump.inputs[0])
    links.new(noise.outputs["Fac"], mix_bump.inputs[1])
    
    bump = nodes.new('ShaderNodeBump')
    bump.inputs["Strength"].default_value = 0.15
    links.new(mix_bump.outputs["Value"], bump.inputs["Height"])
    links.new(bump.outputs["Normal"], bsdf.inputs["Normal"])
    
    # Color variation
    color_noise = nodes.new('ShaderNodeTexNoise')
    color_noise.inputs["Scale"].default_value = 8.0
    color_noise.inputs["Detail"].default_value = 3.0
    links.new(tex_coord.outputs["Object"], color_noise.inputs["Vector"])
    
    ramp = nodes.new('ShaderNodeValToRGB')
    ramp.color_ramp.elements[0].color = (*[c * 0.85 for c in color], 1.0)
    ramp.color_ramp.elements[1].color = (*[c * 1.1 for c in color], 1.0)
    links.new(color_noise.outputs["Fac"], ramp.inputs["Fac"])
    links.new(ramp.outputs["Color"], bsdf.inputs["Base Color"])
    
    return m


# Materials palette
M = {}
M["skin"] = make_mat("M_Skin", (0.95, 0.78, 0.62), rough=0.45, subsurface=0.15,
                       ss_color=(0.8, 0.4, 0.2))
M["beanie"] = make_knit_mat("M_Beanie", (0.22, 0.45, 0.52))
M["coat"] = make_mat("M_Coat", (0.28, 0.48, 0.50), rough=0.72,
                      bump_scale=0.05, bump_noise_scale=15.0)
M["coat_dark"] = make_mat("M_CoatDark", (0.20, 0.38, 0.42), rough=0.75)
M["pants"] = make_mat("M_Pants", (0.15, 0.18, 0.17), rough=0.82,
                       bump_scale=0.03, bump_noise_scale=20.0)
M["boots"] = make_mat("M_Boots", (0.30, 0.22, 0.15), rough=0.75,
                       bump_scale=0.04, bump_noise_scale=12.0)
M["sole"] = make_mat("M_Sole", (0.12, 0.10, 0.08), rough=0.92)
M["bag"] = make_mat("M_Bag", (0.42, 0.40, 0.32), rough=0.80,
                     bump_scale=0.06, bump_noise_scale=10.0)
M["strap"] = make_mat("M_Strap", (0.35, 0.33, 0.27), rough=0.78)
M["pompom"] = make_mat("M_Pompom", (0.88, 0.82, 0.72), rough=0.95,
                        bump_scale=0.20, bump_noise_scale=30.0)
M["hair"] = make_mat("M_Hair", (0.05, 0.04, 0.04), rough=0.65,
                      bump_scale=0.08, bump_noise_scale=25.0)
M["eye"] = make_mat("M_Eye", (0.02, 0.02, 0.02), rough=0.3)
M["white"] = make_mat("M_White", (0.92, 0.90, 0.88), rough=0.6)
M["metal"] = make_mat("M_Metal", (0.55, 0.55, 0.55), rough=0.3)

print("Materials created with procedural shaders")


# ============================================================
# HELPER: Link to collection + smooth shade + subdiv
# ============================================================

def finalize(obj, collection, material, subdiv_level=2, smooth=True):
    """Move obj to collection, assign material, add subdiv, shade smooth."""
    for c in list(obj.users_collection):
        c.objects.unlink(obj)
    collection.objects.link(obj)
    
    if material:
        obj.data.materials.clear()
        obj.data.materials.append(material)
    
    if subdiv_level > 0:
        mod = obj.modifiers.new("Subdiv", 'SUBSURF')
        mod.levels = subdiv_level
        mod.render_levels = subdiv_level + 1
    
    if smooth:
        bpy.context.view_layer.objects.active = obj
        obj.select_set(True)
        bpy.ops.object.shade_smooth()
        obj.select_set(False)
    
    return obj


# ============================================================
# BMESH HELPERS
# ============================================================

def bmesh_sphere(name, loc, radius, segments=16, rings=12):
    """Create a UV sphere using BMesh for clean topology."""
    mesh = bpy.data.meshes.new(name)
    bm = bmesh.new()
    bmesh.ops.create_uvsphere(bm, u_segments=segments, v_segments=rings, radius=radius)
    bm.to_mesh(mesh)
    bm.free()
    obj = bpy.data.objects.new(name, mesh)
    obj.location = loc
    bpy.context.scene.collection.objects.link(obj)
    return obj


def bmesh_cone(name, loc, r1, r2, depth, segments=24):
    """Create a cone/frustum using BMesh."""
    mesh = bpy.data.meshes.new(name)
    bm = bmesh.new()
    bmesh.ops.create_cone(bm, cap_ends=True, cap_tris=False,
                          segments=segments, radius1=r1, radius2=r2, depth=depth)
    bm.to_mesh(mesh)
    bm.free()
    obj = bpy.data.objects.new(name, mesh)
    obj.location = loc
    bpy.context.scene.collection.objects.link(obj)
    return obj


def bmesh_cylinder(name, loc, radius, depth, segments=16, rot=None):
    """Create a cylinder using BMesh."""
    mesh = bpy.data.meshes.new(name)
    bm = bmesh.new()
    bmesh.ops.create_cone(bm, cap_ends=True, cap_tris=False,
                          segments=segments, radius1=radius, radius2=radius, depth=depth)
    bm.to_mesh(mesh)
    bm.free()
    obj = bpy.data.objects.new(name, mesh)
    obj.location = loc
    if rot:
        obj.rotation_euler = rot
    bpy.context.scene.collection.objects.link(obj)
    return obj


# ============================================================
# LAYER 1-3: CHARACTER BUILD
# ============================================================

# --- HEAD (big chibi sphere) ---
head = bmesh_sphere("Head", (0, 0, 0.96), 0.21, segments=24, rings=16)
head.scale = (1.0, 0.95, 1.05)
bpy.context.view_layer.objects.active = head
head.select_set(True)
bpy.ops.object.transform_apply(scale=True)
head.select_set(False)
finalize(head, col_char, M["skin"], subdiv_level=2)

# --- BEANIE (top hemisphere) ---
beanie = bmesh_sphere("Beanie", (0, 0.01, 1.06), 0.225, segments=24, rings=16)
beanie.scale = (1.0, 0.95, 0.80)
bpy.context.view_layer.objects.active = beanie
beanie.select_set(True)
bpy.ops.object.transform_apply(scale=True)
beanie.select_set(False)
finalize(beanie, col_char, M["beanie"], subdiv_level=2)

# --- BEANIE BRIM (thick torus band) ---
bpy.ops.mesh.primitive_torus_add(
    major_radius=0.22, minor_radius=0.032,
    major_segments=32, minor_segments=12,
    location=(0, -0.005, 0.955))
brim = bpy.context.object
brim.name = "BeanieBrim"
brim.scale = (1.0, 0.95, 0.7)
bpy.ops.object.transform_apply(scale=True)
finalize(brim, col_char, M["beanie"], subdiv_level=1)

# --- POMPOM (fluffy ball - high noise bump) ---
pompom = bmesh_sphere("Pompom", (0.01, 0.02, 1.23), 0.065, segments=16, rings=12)
# Deform slightly for organic look
bpy.context.view_layer.objects.active = pompom
pompom.select_set(True)
bpy.ops.object.transform_apply(scale=True)
# Add Displace modifier for fluffy look
disp = pompom.modifiers.new("Displace", 'DISPLACE')
tex = bpy.data.textures.new("PompomNoise", 'CLOUDS')
tex.noise_scale = 0.3
disp.texture = tex
disp.strength = 0.02
disp.mid_level = 0.5
pompom.select_set(False)
finalize(pompom, col_char, M["pompom"], subdiv_level=2)

# --- HAIR - Curves-based clumps ---
# Front hair tufts (3 rounded wedge shapes peeking from brim)
for i, (x, rz) in enumerate([(-0.08, 15), (0, 0), (0.08, -15)]):
    mesh = bpy.data.meshes.new(f"Hair_Front_{i}")
    bm = bmesh.new()
    # Create a pointed oval shape
    bmesh.ops.create_uvsphere(bm, u_segments=8, v_segments=6, radius=0.035)
    # Scale to be flatter and pointed
    for v in bm.verts:
        v.co.y *= 0.4  # flatten front-back
        v.co.z *= 1.8  # elongate vertically
        if v.co.z < 0:
            v.co.z *= 0.3  # taper bottom
    bm.to_mesh(mesh)
    bm.free()
    obj = bpy.data.objects.new(f"Hair_Front_{i}", mesh)
    obj.location = (x, -0.17, 0.945)
    obj.rotation_euler = (math.radians(15), 0, math.radians(rz))
    bpy.context.scene.collection.objects.link(obj)
    finalize(obj, col_char, M["hair"], subdiv_level=2)

# Side hair (covers ears area)
for side, sx in [("L", -1), ("R", 1)]:
    mesh = bpy.data.meshes.new(f"Hair_Side_{side}")
    bm = bmesh.new()
    bmesh.ops.create_uvsphere(bm, u_segments=8, v_segments=6, radius=0.04)
    for v in bm.verts:
        v.co.x *= 0.5
        v.co.z *= 1.5
        if v.co.z < 0:
            v.co.z *= 0.5
    bm.to_mesh(mesh)
    bm.free()
    obj = bpy.data.objects.new(f"Hair_Side_{side}", mesh)
    obj.location = (sx * 0.185, -0.04, 0.91)
    obj.rotation_euler = (0, 0, math.radians(sx * -15))
    bpy.context.scene.collection.objects.link(obj)
    finalize(obj, col_char, M["hair"], subdiv_level=2)

# Back hair volume
back_hair = bmesh_sphere("Hair_Back", (0, 0.12, 0.89), 0.14, segments=12, rings=8)
back_hair.scale = (1.2, 0.5, 0.65)
bpy.context.view_layer.objects.active = back_hair
back_hair.select_set(True)
bpy.ops.object.transform_apply(scale=True)
back_hair.select_set(False)
finalize(back_hair, col_char, M["hair"], subdiv_level=2)

# --- FACE DETAILS ---
# Eyes (large ovals, slightly glossy)
for side, sx in [("L", -0.072), ("R", 0.072)]:
    eye = bmesh_sphere(f"Eye_{side}", (sx, -0.195, 0.90), 0.030, segments=12, rings=8)
    eye.scale = (0.85, 0.35, 1.25)
    bpy.context.view_layer.objects.active = eye
    eye.select_set(True)
    bpy.ops.object.transform_apply(scale=True)
    eye.select_set(False)
    finalize(eye, col_char, M["eye"], subdiv_level=2)

# Eyebrows (thin curved strips above eyes)
for side, sx, rz in [("L", -0.072, -8), ("R", 0.072, 8)]:
    mesh = bpy.data.meshes.new(f"Brow_{side}")
    bm = bmesh.new()
    bmesh.ops.create_cube(bm, size=1.0)
    for v in bm.verts:
        v.co.x *= 0.045
        v.co.y *= 0.008
        v.co.z *= 0.010
    bm.to_mesh(mesh)
    bm.free()
    obj = bpy.data.objects.new(f"Brow_{side}", mesh)
    obj.location = (sx, -0.20, 0.945)
    obj.rotation_euler = (0, 0, math.radians(rz))
    bpy.context.scene.collection.objects.link(obj)
    finalize(obj, col_char, M["hair"], subdiv_level=2)

# Nose (tiny bump)
nose = bmesh_sphere("Nose", (0, -0.215, 0.87), 0.012, segments=8, rings=6)
finalize(nose, col_char, M["skin"], subdiv_level=1)

# --- TORSO (A-line raincoat) ---
torso = bmesh_cone("Torso", (0, 0, 0.52), r1=0.21, r2=0.115, depth=0.42, segments=24)
torso.scale = (1.0, 0.85, 1.0)
bpy.context.view_layer.objects.active = torso
torso.select_set(True)
bpy.ops.object.transform_apply(scale=True)
torso.select_set(False)
# Add Bevel modifier for rounded edges
bev = torso.modifiers.new("Bevel", 'BEVEL')
bev.width = 0.015
bev.segments = 3
finalize(torso, col_char, M["coat"], subdiv_level=2)

# --- COLLAR ---
collar = bmesh_cylinder("Collar", (0, 0, 0.735), 0.105, 0.05, segments=20)
finalize(collar, col_char, M["coat_dark"], subdiv_level=1)

# --- HOOD (draped behind neck) ---
hood = bmesh_sphere("Hood", (0, 0.14, 0.72), 0.15, segments=16, rings=10)
hood.scale = (1.15, 0.55, 0.85)
bpy.context.view_layer.objects.active = hood
hood.select_set(True)
bpy.ops.object.transform_apply(scale=True)
hood.select_set(False)
finalize(hood, col_char, M["coat_dark"], subdiv_level=2)

# --- ARMS ---
for side, sx, angle in [("L", -1, 12), ("R", 1, -12)]:
    arm = bmesh_cone(f"Arm_{side}", (sx * 0.20, 0, 0.58),
                     r1=0.045, r2=0.055, depth=0.26, segments=12)
    arm.rotation_euler = (0, math.radians(angle), 0)
    finalize(arm, col_char, M["coat"], subdiv_level=1)
    
    # Cuff
    cuff = bmesh_cylinder(f"Cuff_{side}", (sx * 0.22, 0, 0.43), 0.055, 0.025, segments=12)
    cuff.rotation_euler = (0, math.radians(angle), 0)
    finalize(cuff, col_char, M["coat_dark"], subdiv_level=1)
    
    # Hand
    hand = bmesh_sphere(f"Hand_{side}", (sx * 0.235, 0, 0.39), 0.038, segments=10, rings=8)
    finalize(hand, col_char, M["skin"], subdiv_level=1)

# --- LEGS ---
for side, sx in [("L", -0.065), ("R", 0.065)]:
    leg = bmesh_cylinder(f"Leg_{side}", (sx, 0, 0.22), 0.048, 0.20, segments=12)
    finalize(leg, col_char, M["pants"], subdiv_level=1)

# --- BOOTS ---
for side, sx in [("L", -0.065), ("R", 0.065)]:
    boot = bmesh_cone(f"Boot_{side}", (sx, -0.005, 0.065),
                      r1=0.058, r2=0.052, depth=0.13, segments=12)
    bev = boot.modifiers.new("Bevel", 'BEVEL')
    bev.width = 0.008
    bev.segments = 2
    finalize(boot, col_char, M["boots"], subdiv_level=1)
    
    sole = bmesh_cylinder(f"Sole_{side}", (sx, -0.005, 0.005), 0.06, 0.01, segments=12)
    finalize(sole, col_char, M["sole"], subdiv_level=0, smooth=False)

# --- COAT DETAILS ---
# Buttons (4 snaps down center)
for i, z in enumerate([0.64, 0.58, 0.52, 0.46]):
    btn = bmesh_cylinder(f"Button_{i}", (0, -0.19, z), 0.008, 0.005, segments=8)
    btn.rotation_euler = (math.radians(90), 0, 0)
    finalize(btn, col_char, M["metal"], subdiv_level=1)

# Pockets (rounded rectangles)
for side, sx in [("L", -0.095), ("R", 0.095)]:
    mesh = bpy.data.meshes.new(f"Pocket_{side}")
    bm = bmesh.new()
    bmesh.ops.create_cube(bm, size=1.0)
    for v in bm.verts:
        v.co.x *= 0.055
        v.co.y *= 0.008
        v.co.z *= 0.040
    bm.to_mesh(mesh)
    bm.free()
    obj = bpy.data.objects.new(f"Pocket_{side}", mesh)
    obj.location = (sx, -0.185, 0.42)
    bpy.context.scene.collection.objects.link(obj)
    bev = obj.modifiers.new("Bevel", 'BEVEL')
    bev.width = 0.005
    bev.segments = 3
    finalize(obj, col_char, M["coat_dark"], subdiv_level=1)

# Drawstrings
for side, sx in [("L", -0.02), ("R", 0.02)]:
    ds = bmesh_cylinder(f"String_{side}", (sx, -0.14, 0.69), 0.004, 0.06, segments=6)
    ds.rotation_euler = (math.radians(5), 0, 0)
    finalize(ds, col_char, M["white"], subdiv_level=0)
    
    # Toggle at bottom
    toggle = bmesh_cylinder(f"Toggle_{side}", (sx, -0.145, 0.655), 0.006, 0.012, segments=6)
    finalize(toggle, col_char, M["white"], subdiv_level=1)

# --- SATCHEL BAG ---
mesh = bpy.data.meshes.new("Satchel")
bm = bmesh.new()
bmesh.ops.create_cube(bm, size=1.0)
for v in bm.verts:
    v.co.x *= 0.075
    v.co.y *= 0.040
    v.co.z *= 0.070
bm.to_mesh(mesh)
bm.free()
satchel = bpy.data.objects.new("Satchel", mesh)
satchel.location = (0.21, 0.04, 0.46)
bpy.context.scene.collection.objects.link(satchel)
bev = satchel.modifiers.new("Bevel", 'BEVEL')
bev.width = 0.012
bev.segments = 4
finalize(satchel, col_char, M["bag"], subdiv_level=2)

# Bag flap
mesh = bpy.data.meshes.new("BagFlap")
bm = bmesh.new()
bmesh.ops.create_cube(bm, size=1.0)
for v in bm.verts:
    v.co.x *= 0.078
    v.co.y *= 0.042
    v.co.z *= 0.012
bm.to_mesh(mesh)
bm.free()
flap = bpy.data.objects.new("BagFlap", mesh)
flap.location = (0.21, 0.003, 0.525)
bpy.context.scene.collection.objects.link(flap)
bev = flap.modifiers.new("Bevel", 'BEVEL')
bev.width = 0.008
bev.segments = 3
finalize(flap, col_char, M["bag"], subdiv_level=1)

# Buckle
buckle = bmesh_cylinder("Buckle", (0.21, -0.02, 0.50), 0.008, 0.015, segments=6)
buckle.rotation_euler = (math.radians(90), 0, 0)
finalize(buckle, col_char, M["metal"], subdiv_level=1)

# Strap
bpy.ops.mesh.primitive_torus_add(
    major_radius=0.20, minor_radius=0.010,
    major_segments=24, minor_segments=8,
    location=(0.02, 0.0, 0.58))
strap = bpy.context.object
strap.name = "Strap"
strap.scale = (0.7, 0.9, 1.2)
strap.rotation_euler = (math.radians(75), math.radians(-15), 0)
bpy.ops.object.transform_apply(scale=True, rotation=True)
finalize(strap, col_char, M["strap"], subdiv_level=1)


# ============================================================
# SAVE & RENDER 4 VIEWS
# ============================================================

BLEND_PATH.parent.mkdir(parents=True, exist_ok=True)
bpy.ops.wm.save_as_mainfile(filepath=str(BLEND_PATH))
print(f"Saved to {BLEND_PATH}")

for cam_name, suffix in [("Cam_Front","front"), ("Cam_Back","back"),
                          ("Cam_Left","left"), ("Cam_Right","right")]:
    cam = bpy.data.objects.get(cam_name)
    if cam:
        bpy.context.scene.camera = cam
        path = TEMP / f"view_{suffix}.png"
        bpy.context.scene.render.filepath = str(path)
        bpy.ops.render.render(write_still=True)
        print(f"Rendered {suffix}")

print("Layer 0-3 HIGH QUALITY build complete!")
