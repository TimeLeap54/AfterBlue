"""
HQ Rebuild Fix: Color saturation, hair visibility, eye position, then rigging + animation.
"""
import math
import bmesh
import bpy
from mathutils import Vector
from pathlib import Path

PROJECT_ROOT = Path(r"C:\Users\minil\GameMaking\AfterBlue")
BLEND_PATH = PROJECT_ROOT / "Assets" / "Art" / "Blender" / "character_v01.blend"
TEMP = PROJECT_ROOT / "Temp"

# ============================================================
# COLOR FIX - Deeper, more saturated teal like reference
# ============================================================

def fix_color(mat_name, new_color):
    m = bpy.data.materials.get(mat_name)
    if not m or not m.use_nodes:
        return
    bsdf = m.node_tree.nodes.get("Principled BSDF")
    if bsdf:
        bsdf.inputs["Base Color"].default_value = (*new_color, 1.0)
    m.diffuse_color = (*new_color, 1.0)
    # Also update color ramp if it exists (knit material)
    for node in m.node_tree.nodes:
        if node.type == 'VALTORGB':
            node.color_ramp.elements[0].color = (*[c * 0.85 for c in new_color], 1.0)
            node.color_ramp.elements[1].color = (*[min(c * 1.15, 1.0) for c in new_color], 1.0)

# Reference colors are deeper/more saturated teal
fix_color("M_Beanie", (0.15, 0.38, 0.48))
fix_color("M_Coat", (0.18, 0.40, 0.42))
fix_color("M_CoatDark", (0.12, 0.30, 0.34))
fix_color("M_Pants", (0.10, 0.12, 0.11))
fix_color("M_Boots", (0.25, 0.18, 0.12))
fix_color("M_Bag", (0.38, 0.35, 0.26))
fix_color("M_Strap", (0.30, 0.27, 0.20))
fix_color("M_Skin", (0.93, 0.76, 0.60))

print("Colors fixed to match reference")


# ============================================================
# HAIR FIX - More visible front tufts
# ============================================================

# Move front hair tufts further forward and bigger
for i in range(3):
    h = bpy.data.objects.get(f"Hair_Front_{i}")
    if h:
        h.location.y = -0.19  # more forward
        h.location.z = 0.94
        h.scale = (1.2, 1.2, 1.2)

# Make back hair not clip through hood
bh = bpy.data.objects.get("Hair_Back")
if bh:
    bh.location = (0, 0.10, 0.88)
    bh.scale = (1.0, 0.4, 0.6)

# Eyes slightly lower
for side in ["L", "R"]:
    eye = bpy.data.objects.get(f"Eye_{side}")
    if eye:
        eye.location.z = 0.885

# Brows lower to match
for side in ["L", "R"]:
    brow = bpy.data.objects.get(f"Brow_{side}")
    if brow:
        brow.location.z = 0.93

print("Hair and face positions fixed")


# ============================================================
# LAYER 8-9: RIGGING
# ============================================================

# Apply all transforms (MESH objects only to avoid Area Light error)
bpy.ops.object.select_all(action='DESELECT')
for obj in bpy.data.objects:
    if obj.type == 'MESH':
        obj.select_set(True)
bpy.ops.object.transform_apply(location=True, rotation=True, scale=True)
bpy.ops.object.select_all(action='DESELECT')
print("Layer 8: Transforms applied")

# Create armature
for obj in list(bpy.data.objects):
    if obj.type == 'ARMATURE':
        bpy.data.objects.remove(obj, do_unlink=True)
for arm in list(bpy.data.armatures):
    bpy.data.armatures.remove(arm)

rig_col = bpy.data.collections.get("Rig")
if not rig_col:
    rig_col = bpy.data.collections.new("Rig")
    bpy.context.scene.collection.children.link(rig_col)

arm_data = bpy.data.armatures.new("Fisherman_Armature")
arm_obj = bpy.data.objects.new("Fisherman_Rig", arm_data)
arm_obj.location = (0, 0, 0)
rig_col.objects.link(arm_obj)

bpy.context.view_layer.objects.active = arm_obj
arm_obj.select_set(True)
bpy.ops.object.mode_set(mode='EDIT')

bones_def = [
    ("Root",       (0, 0, 0),      (0, 0, 0.10),    None,     False),
    ("Hips",       (0, 0, 0.10),   (0, 0, 0.32),    "Root",   True),
    ("Spine",      (0, 0, 0.32),   (0, 0, 0.52),    "Hips",   True),
    ("Chest",      (0, 0, 0.52),   (0, 0, 0.72),    "Spine",  True),
    ("Neck",       (0, 0, 0.72),   (0, 0, 0.76),    "Chest",  True),
    ("Head",       (0, 0, 0.76),   (0, 0, 1.16),    "Neck",   True),
    ("UpperLeg.L", (-0.065, 0, 0.32), (-0.065, 0, 0.22), "Hips", False),
    ("LowerLeg.L", (-0.065, 0, 0.22), (-0.065, 0, 0.10), "UpperLeg.L", True),
    ("Foot.L",     (-0.065, 0, 0.10), (-0.065, -0.04, 0.0), "LowerLeg.L", True),
    ("UpperLeg.R", (0.065, 0, 0.32), (0.065, 0, 0.22), "Hips", False),
    ("LowerLeg.R", (0.065, 0, 0.22), (0.065, 0, 0.10), "UpperLeg.R", True),
    ("Foot.R",     (0.065, 0, 0.10), (0.065, -0.04, 0.0), "LowerLeg.R", True),
    ("UpperArm.L", (-0.12, 0, 0.66), (-0.20, 0, 0.56), "Chest", False),
    ("LowerArm.L", (-0.20, 0, 0.56), (-0.235, 0, 0.43), "UpperArm.L", True),
    ("Hand.L",     (-0.235, 0, 0.43), (-0.235, 0, 0.36), "LowerArm.L", True),
    ("UpperArm.R", (0.12, 0, 0.66), (0.20, 0, 0.56), "Chest", False),
    ("LowerArm.R", (0.20, 0, 0.56), (0.235, 0, 0.43), "UpperArm.R", True),
    ("Hand.R",     (0.235, 0, 0.43), (0.235, 0, 0.36), "LowerArm.R", True),
]

created_bones = {}
for bname, head, tail, parent_name, connected in bones_def:
    bone = arm_data.edit_bones.new(bname)
    bone.head = Vector(head)
    bone.tail = Vector(tail)
    if parent_name and parent_name in created_bones:
        bone.parent = created_bones[parent_name]
        bone.use_connect = connected
    created_bones[bname] = bone

bpy.ops.object.mode_set(mode='OBJECT')
arm_obj.select_set(False)
print(f"Layer 9: Armature with {len(bones_def)} bones")

# Bone parenting map
bone_map = {
    "Head": "Head", "Beanie": "Head", "BeanieBrim": "Head", "Pompom": "Head",
    "Eye_L": "Head", "Eye_R": "Head", "Brow_L": "Head", "Brow_R": "Head",
    "Nose": "Head", "Hair_Front_0": "Head", "Hair_Front_1": "Head",
    "Hair_Front_2": "Head", "Hair_Side_L": "Head", "Hair_Side_R": "Head",
    "Hair_Back": "Head",
    "Torso": "Spine", "Collar": "Chest", "Hood": "Chest",
    "Button_0": "Spine", "Button_1": "Spine", "Button_2": "Spine", "Button_3": "Spine",
    "Pocket_L": "Spine", "Pocket_R": "Spine",
    "String_L": "Chest", "String_R": "Chest",
    "Toggle_L": "Chest", "Toggle_R": "Chest",
    "Satchel": "Spine", "BagFlap": "Spine", "Buckle": "Spine", "Strap": "Chest",
    "Arm_L": "UpperArm.L", "Arm_R": "UpperArm.R",
    "Cuff_L": "LowerArm.L", "Cuff_R": "LowerArm.R",
    "Hand_L": "Hand.L", "Hand_R": "Hand.R",
    "Leg_L": "UpperLeg.L", "Leg_R": "UpperLeg.R",
    "Boot_L": "Foot.L", "Boot_R": "Foot.R",
    "Sole_L": "Foot.L", "Sole_R": "Foot.R",
}

parented = 0
for obj_name, bone_name in bone_map.items():
    obj = bpy.data.objects.get(obj_name)
    if not obj:
        continue
    saved = obj.matrix_world.copy()
    obj.parent = arm_obj
    obj.parent_type = 'BONE'
    obj.parent_bone = bone_name
    bpy.context.view_layer.update()
    obj.matrix_world = saved
    parented += 1

print(f"Layer 9: Parented {parented} objects (matrix_world preserved)")


# ============================================================
# LAYER 10-11: ANIMATIONS
# ============================================================

bpy.context.view_layer.objects.active = arm_obj
arm_obj.select_set(True)

if not arm_obj.animation_data:
    arm_obj.animation_data_create()

bpy.ops.object.mode_set(mode='POSE')
for pb in arm_obj.pose.bones:
    pb.rotation_mode = 'XYZ'
    pb.location = (0, 0, 0)
    pb.rotation_euler = (0, 0, 0)

# --- IDLE ACTION ---
idle = bpy.data.actions.new("Fisherman_Idle")
arm_obj.animation_data.action = idle

spine_pb = arm_obj.pose.bones.get("Spine")
head_pb = arm_obj.pose.bones.get("Head")

for frame in [1, 30, 60]:
    bpy.context.scene.frame_set(frame)
    if spine_pb:
        spine_pb.location.z = 0.004 if frame == 30 else 0.0
        spine_pb.keyframe_insert(data_path="location", frame=frame)
    if head_pb:
        head_pb.rotation_euler.x = math.radians(-1.5) if frame == 30 else 0
        head_pb.keyframe_insert(data_path="rotation_euler", frame=frame)

print("Idle action: 60 frames")

# --- WALK ACTION ---
walk = bpy.data.actions.new("Fisherman_Walk")
arm_obj.animation_data.action = walk

for pb in arm_obj.pose.bones:
    pb.location = (0, 0, 0)
    pb.rotation_euler = (0, 0, 0)

walk_data = {
    1:  {"UL.L":-20, "UL.R":20, "LL.L":10, "LL.R":-5, "UA.L":15, "UA.R":-15, "root_z":0, "sp_x":2},
    7:  {"UL.L":-10, "UL.R":10, "LL.L":5, "LL.R":-10, "UA.L":8, "UA.R":-8, "root_z":-0.004, "sp_x":0},
    13: {"UL.L":5, "UL.R":-5, "LL.L":0, "LL.R":0, "UA.L":-5, "UA.R":5, "root_z":0, "sp_x":-1},
    19: {"UL.L":20, "UL.R":-20, "LL.L":-5, "LL.R":10, "UA.L":-15, "UA.R":15, "root_z":0, "sp_x":-2},
    24: {"UL.L":-20, "UL.R":20, "LL.L":10, "LL.R":-5, "UA.L":15, "UA.R":-15, "root_z":0, "sp_x":2},
}

bone_alias = {
    "UL.L": "UpperLeg.L", "UL.R": "UpperLeg.R",
    "LL.L": "LowerLeg.L", "LL.R": "LowerLeg.R",
    "UA.L": "UpperArm.L", "UA.R": "UpperArm.R",
}

for frame, data in walk_data.items():
    bpy.context.scene.frame_set(frame)
    for alias, bname in bone_alias.items():
        pb = arm_obj.pose.bones.get(bname)
        if pb:
            pb.rotation_euler.x = math.radians(data[alias])
            pb.keyframe_insert(data_path="rotation_euler", frame=frame)
    root_pb = arm_obj.pose.bones.get("Root")
    if root_pb:
        root_pb.location.z = data["root_z"]
        root_pb.keyframe_insert(data_path="location", frame=frame)
    if spine_pb:
        spine_pb.rotation_euler.x = math.radians(data["sp_x"])
        spine_pb.keyframe_insert(data_path="rotation_euler", frame=frame)

print("Walk action: 24 frames")

# --- CAST ACTION ---
cast = bpy.data.actions.new("Fisherman_Cast")
arm_obj.animation_data.action = cast

for pb in arm_obj.pose.bones:
    pb.location = (0, 0, 0)
    pb.rotation_euler = (0, 0, 0)

cast_data = {
    1:  {"ua_r": 0, "la_r": 0, "chest": 0},
    10: {"ua_r": -75, "la_r": -25, "chest": 8},
    16: {"ua_r": 55, "la_r": 12, "chest": -6},
    22: {"ua_r": 25, "la_r": 5, "chest": -2},
    30: {"ua_r": 12, "la_r": 0, "chest": 0},
}

for frame, data in cast_data.items():
    bpy.context.scene.frame_set(frame)
    ua_r = arm_obj.pose.bones.get("UpperArm.R")
    la_r = arm_obj.pose.bones.get("LowerArm.R")
    chest = arm_obj.pose.bones.get("Chest")
    if ua_r:
        ua_r.rotation_euler.x = math.radians(data["ua_r"])
        ua_r.keyframe_insert(data_path="rotation_euler", frame=frame)
    if la_r:
        la_r.rotation_euler.x = math.radians(data["la_r"])
        la_r.keyframe_insert(data_path="rotation_euler", frame=frame)
    if chest:
        chest.rotation_euler.x = math.radians(data["chest"])
        chest.keyframe_insert(data_path="rotation_euler", frame=frame)

print("Cast action: 30 frames")

# Back to object mode
bpy.ops.object.mode_set(mode='OBJECT')

# Set walk action at frame 7 for render verification
arm_obj.animation_data.action = walk
bpy.context.scene.frame_set(7)

# Save
bpy.ops.wm.save_as_mainfile(filepath=str(BLEND_PATH))
print(f"Saved to {BLEND_PATH}")

# Render 4 views
for cam_name, suffix in [("Cam_Front","front"), ("Cam_Back","back"),
                          ("Cam_Left","left"), ("Cam_Right","right")]:
    cam = bpy.data.objects.get(cam_name)
    if cam:
        bpy.context.scene.camera = cam
        path = TEMP / f"view_{suffix}.png"
        bpy.context.scene.render.filepath = str(path)
        bpy.ops.render.render(write_still=True)
        print(f"Rendered {suffix}")

# Set back to idle
arm_obj.animation_data.action = idle
bpy.context.scene.frame_set(1)
bpy.ops.wm.save_as_mainfile(filepath=str(BLEND_PATH))

print("ALL LAYERS COMPLETE (0-11)!")
