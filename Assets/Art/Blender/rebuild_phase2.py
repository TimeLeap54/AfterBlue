"""
AfterBlue Fisherman - Phase 2
Layer 4: Prop Scale Blockout (fishing rod)
Layer 5: Prop Detail Layer (rod segments, reel, bobber)
Layer 6: Flat Color Blockout (verify color harmony)
Layer 7: Hand-painted Wear & Aging (skip, flat style game)

Also fixes camera 3/4 final time.
"""
import math
import bpy
from pathlib import Path
from mathutils import Vector

PROJECT_ROOT = Path(r"C:\Users\minil\GameMaking\AfterBlue")
BLEND_PATH = PROJECT_ROOT / "Assets" / "Art" / "Blender" / "character_v01.blend"


def col(name):
    c = bpy.data.collections.get(name)
    if not c:
        c = bpy.data.collections.new(name)
        bpy.context.scene.collection.children.link(c)
    return c


def link(collection, obj):
    for ex in list(obj.users_collection):
        ex.objects.unlink(obj)
    collection.objects.link(obj)
    return obj


# ============================================================
# FIX CAMERA ONCE AND FOR ALL
# ============================================================

cam34 = bpy.data.objects.get("Camera_3_4")
if cam34:
    cam34.location = (3.0, -3.0, 0.8)
    cam34.rotation_euler = (math.radians(80), 0, math.radians(45))
    cam34.data.lens = 65  # wider angle to capture full body

# Also adjust front/side cameras slightly
cam_f = bpy.data.objects.get("Camera_Front")
if cam_f:
    cam_f.location = (0, -3.5, 0.6)
    cam_f.data.lens = 75

cam_s = bpy.data.objects.get("Camera_Side")
if cam_s:
    cam_s.location = (3.5, 0, 0.6)
    cam_s.data.lens = 75


# ============================================================
# LAYER 4 & 5: FISHING ROD PROP
# ============================================================

props = col("03_Props")

# Get existing materials
m_wood = bpy.data.materials.get("M_Wood")
m_metal = bpy.data.materials.get("M_Metal")
m_red = bpy.data.materials.get("M_Red")
m_white = bpy.data.materials.get("M_White")
m_glow = bpy.data.materials.get("M_Glow")

# Rod position: held by right hand at (-0.24, 0, 0.38) going up
rod_base = Vector((0.24, -0.02, 0.38))

# Rod handle (thick part in hand)
bpy.ops.mesh.primitive_cylinder_add(
    vertices=12, radius=0.018, depth=0.12,
    location=(rod_base.x, rod_base.y, rod_base.z + 0.06),
    rotation=(0, 0, 0))
handle = link(props, bpy.context.object)
handle.name = "Rod_Handle"
handle.data.materials.append(m_wood)
bpy.ops.object.shade_smooth()

# Cork grip
bpy.ops.mesh.primitive_cylinder_add(
    vertices=12, radius=0.016, depth=0.06,
    location=(rod_base.x, rod_base.y, rod_base.z + 0.15))
grip = link(props, bpy.context.object)
grip.name = "Rod_Grip"
# Make a cork-colored material
m_cork = bpy.data.materials.get("M_Cork")
if not m_cork:
    m_cork = bpy.data.materials.new("M_Cork")
    m_cork.use_nodes = True
    m_cork.diffuse_color = (0.55, 0.42, 0.30, 1)
    bsdf = m_cork.node_tree.nodes.get("Principled BSDF")
    if bsdf:
        bsdf.inputs["Base Color"].default_value = (0.55, 0.42, 0.30, 1)
        bsdf.inputs["Roughness"].default_value = 0.95
grip.data.materials.append(m_cork)
bpy.ops.object.shade_smooth()

# Rod shaft (thin, extends upward)
bpy.ops.mesh.primitive_cylinder_add(
    vertices=8, radius=0.008, depth=0.60,
    location=(rod_base.x, rod_base.y, rod_base.z + 0.48))
shaft = link(props, bpy.context.object)
shaft.name = "Rod_Shaft"
shaft.data.materials.append(m_wood)
bpy.ops.object.shade_smooth()

# Rod tip (very thin)
bpy.ops.mesh.primitive_cylinder_add(
    vertices=8, radius=0.004, depth=0.30,
    location=(rod_base.x, rod_base.y, rod_base.z + 0.93))
tip = link(props, bpy.context.object)
tip.name = "Rod_Tip"
tip.data.materials.append(m_wood)
bpy.ops.object.shade_smooth()

# Reel (small disc on handle)
bpy.ops.mesh.primitive_cylinder_add(
    vertices=12, radius=0.025, depth=0.015,
    location=(rod_base.x + 0.025, rod_base.y, rod_base.z + 0.12),
    rotation=(0, math.radians(90), 0))
reel = link(props, bpy.context.object)
reel.name = "Rod_Reel"
reel.data.materials.append(m_metal)
bpy.ops.object.shade_smooth()

# Reel handle (tiny knob)
bpy.ops.mesh.primitive_uv_sphere_add(
    radius=0.008,
    location=(rod_base.x + 0.05, rod_base.y, rod_base.z + 0.12))
rknob = link(props, bpy.context.object)
rknob.name = "Rod_ReelKnob"
rknob.data.materials.append(m_metal)
bpy.ops.object.shade_smooth()

# Line guides (tiny tori along the shaft)
for i, z_off in enumerate([0.25, 0.45, 0.65, 0.85]):
    bpy.ops.mesh.primitive_torus_add(
        major_radius=0.010, minor_radius=0.003,
        location=(rod_base.x, rod_base.y, rod_base.z + z_off))
    guide = link(props, bpy.context.object)
    guide.name = f"Rod_Guide_{i}"
    guide.data.materials.append(m_metal)
    bpy.ops.object.shade_smooth()

# Fishing line (very thin cylinder hanging from tip)
bpy.ops.mesh.primitive_cylinder_add(
    vertices=6, radius=0.002, depth=0.35,
    location=(rod_base.x, rod_base.y - 0.02, rod_base.z + 0.90))
line = link(props, bpy.context.object)
line.name = "Rod_Line"
line.data.materials.append(m_white)

# Bobber (red/white sphere at end of line)
bobber_z = rod_base.z + 0.72
bpy.ops.mesh.primitive_uv_sphere_add(
    segments=16, ring_count=12, radius=0.018,
    location=(rod_base.x, rod_base.y - 0.02, bobber_z))
bobber = link(props, bpy.context.object)
bobber.name = "Bobber"
bobber.data.materials.append(m_red)
bpy.ops.object.shade_smooth()

# Bobber white stripe
bpy.ops.mesh.primitive_cylinder_add(
    vertices=12, radius=0.019, depth=0.006,
    location=(rod_base.x, rod_base.y - 0.02, bobber_z))
bstripe = link(props, bpy.context.object)
bstripe.name = "Bobber_Stripe"
bstripe.data.materials.append(m_white)
bpy.ops.object.shade_smooth()

print("Layer 4-5: Props complete (fishing rod)")


# ============================================================
# LAYER 6: FLAT COLOR VERIFICATION
# ============================================================

# Colors are already set via materials in Phase 1.
# Verify overall color harmony by checking all materials exist.
color_check = [
    "M_Skin", "M_Beanie", "M_Coat", "M_Pants", "M_Boots",
    "M_Bag", "M_Pompom", "M_Dark", "M_White", "M_Wood",
    "M_Metal", "M_Red", "M_Glow", "M_Sole", "M_Cork"
]
for name in color_check:
    m = bpy.data.materials.get(name)
    if m:
        print(f"  Color OK: {name} = {tuple(round(c,2) for c in m.diffuse_color)}")
    else:
        print(f"  WARNING: {name} not found!")

print("Layer 6: Flat color check complete")
print("Layer 7: Hand-painted wear skipped (flat style game)")


# ============================================================
# SAVE & RENDER
# ============================================================

bpy.ops.wm.save_as_mainfile(filepath=str(BLEND_PATH))
print(f"Saved to {BLEND_PATH}")

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
        print(f"Rendered {suffix}")

print("Phase 2 (Layer 4-7) complete!")
