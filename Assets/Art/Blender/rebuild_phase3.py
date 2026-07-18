"""
AfterBlue Fisherman - Phase 3
Layer 8:  Rig Preparation (apply transforms, clean names, check intersections)
Layer 9:  Basic Rigging (create armature, bind meshes)
Layer 10: Pose Test (neutral, walk contact, cast)
Layer 11: Minimal Animations (Idle, Walk, Cast actions with keyframes)

CRITICAL: Uses matrix_world preservation for bone parenting to avoid displacement bugs.
"""
import math
import bpy
from pathlib import Path
from mathutils import Vector, Matrix

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
# LAYER 8: RIG PREPARATION
# ============================================================

def layer8_rig_prep():
    """Apply all transforms, clean naming, verify mesh integrity."""
    bpy.ops.object.select_all(action='DESELECT')
    
    mesh_objects = [o for o in bpy.data.objects if o.type == 'MESH']
    
    for obj in mesh_objects:
        bpy.context.view_layer.objects.active = obj
        obj.select_set(True)
        bpy.ops.object.transform_apply(location=True, rotation=True, scale=True)
        obj.select_set(False)
    
    print(f"Layer 8: Applied transforms to {len(mesh_objects)} objects")
    return mesh_objects


# ============================================================
# LAYER 9: BASIC RIGGING
# ============================================================

def layer9_rigging():
    """Create a simple armature and parent objects to bones."""
    rig_col = col("04_Rig")
    
    # Delete existing armature
    for obj in list(bpy.data.objects):
        if obj.type == 'ARMATURE':
            bpy.data.objects.remove(obj, do_unlink=True)
    for arm in list(bpy.data.armatures):
        bpy.data.armatures.remove(arm)
    
    # Create armature
    arm_data = bpy.data.armatures.new("Fisherman_Armature")
    arm_obj = bpy.data.objects.new("Fisherman_Rig", arm_data)
    arm_obj.location = (0, 0, 0)
    link(rig_col, arm_obj)
    
    bpy.context.view_layer.objects.active = arm_obj
    arm_obj.select_set(True)
    bpy.ops.object.mode_set(mode='EDIT')
    
    # Bone definitions: (name, head, tail, parent_name, connected)
    bones_def = [
        # Spine chain
        ("Root",       (0, 0, 0),      (0, 0, 0.10),    None,     False),
        ("Hips",       (0, 0, 0.10),   (0, 0, 0.32),    "Root",   True),
        ("Spine",      (0, 0, 0.32),   (0, 0, 0.52),    "Hips",   True),
        ("Chest",      (0, 0, 0.52),   (0, 0, 0.72),    "Spine",  True),
        ("Neck",       (0, 0, 0.72),   (0, 0, 0.76),    "Chest",  True),
        ("Head",       (0, 0, 0.76),   (0, 0, 1.16),    "Neck",   True),
        
        # Left leg
        ("UpperLeg.L", (-0.07, 0, 0.32), (-0.07, 0, 0.22), "Hips", False),
        ("LowerLeg.L", (-0.07, 0, 0.22), (-0.07, 0, 0.10), "UpperLeg.L", True),
        ("Foot.L",     (-0.07, 0, 0.10), (-0.07, -0.04, 0.0), "LowerLeg.L", True),
        
        # Right leg
        ("UpperLeg.R", (0.07, 0, 0.32), (0.07, 0, 0.22), "Hips", False),
        ("LowerLeg.R", (0.07, 0, 0.22), (0.07, 0, 0.10), "UpperLeg.R", True),
        ("Foot.R",     (0.07, 0, 0.10), (0.07, -0.04, 0.0), "LowerLeg.R", True),
        
        # Left arm
        ("UpperArm.L", (-0.12, 0, 0.66), (-0.20, 0, 0.56), "Chest", False),
        ("LowerArm.L", (-0.20, 0, 0.56), (-0.24, 0, 0.42), "UpperArm.L", True),
        ("Hand.L",     (-0.24, 0, 0.42), (-0.24, 0, 0.35), "LowerArm.L", True),
        
        # Right arm
        ("UpperArm.R", (0.12, 0, 0.66), (0.20, 0, 0.56), "Chest", False),
        ("LowerArm.R", (0.20, 0, 0.56), (0.24, 0, 0.42), "UpperArm.R", True),
        ("Hand.R",     (0.24, 0, 0.42), (0.24, 0, 0.35), "LowerArm.R", True),
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
    
    print(f"Layer 9: Created armature with {len(bones_def)} bones")
    
    # ---- PARENTING ----
    # Map each mesh to its controlling bone
    bone_map = {
        # Head group
        "Head":       "Head",
        "Beanie":     "Head",
        "BeanieBrim": "Head",
        "Pompom":     "Head",
        "Eye_L":      "Head",
        "Eye_R":      "Head",
        "Brow_L":     "Head",
        "Brow_R":     "Head",
        "Nose":       "Head",
        "Ear_L":      "Head",
        "Ear_R":      "Head",
        "Hair_L":     "Head",
        "Hair_R":     "Head",
        "Hair_C":     "Head",
        "Sideburn_L": "Head",
        "Sideburn_R": "Head",
        "BackHair":   "Head",
        
        # Torso group
        "Torso":     "Spine",
        "Collar":    "Chest",
        "Hood":      "Chest",
        "Btn1":      "Spine",
        "Btn2":      "Spine",
        "Pocket_L":  "Spine",
        "Pocket_R":  "Spine",
        "String_L":  "Chest",
        "String_R":  "Chest",
        "Satchel":   "Spine",
        "Strap":     "Chest",
        "BagFlap":   "Spine",
        "BagBuckle": "Spine",
        
        # Arms
        "Arm_L":   "UpperArm.L",
        "Arm_R":   "UpperArm.R",
        "Cuff_L":  "LowerArm.L",
        "Cuff_R":  "LowerArm.R",
        "Hand_L":  "Hand.L",
        "Hand_R":  "Hand.R",
        
        # Legs/boots
        "Leg_L":   "UpperLeg.L",
        "Leg_R":   "UpperLeg.R",
        "Boot_L":  "Foot.L",
        "Boot_R":  "Foot.R",
        "Sole_L":  "Foot.L",
        "Sole_R":  "Foot.R",
    }
    
    # Safe parenting with world matrix preservation
    parented = 0
    for obj_name, bone_name in bone_map.items():
        obj = bpy.data.objects.get(obj_name)
        if not obj:
            continue
        
        # Save the world matrix BEFORE parenting
        saved_matrix = obj.matrix_world.copy()
        
        obj.parent = arm_obj
        obj.parent_type = 'BONE'
        obj.parent_bone = bone_name
        
        # Restore world matrix AFTER parenting
        bpy.context.view_layer.update()
        obj.matrix_world = saved_matrix
        
        parented += 1
    
    # Parent fishing rod parts to right hand
    rod_parts = [
        "Rod_Handle", "Rod_Grip", "Rod_Shaft", "Rod_Tip", "Rod_Reel",
        "Rod_ReelKnob", "Rod_Line", "Bobber", "Bobber_Stripe",
    ]
    rod_parts += [f"Rod_Guide_{i}" for i in range(4)]
    
    for rp in rod_parts:
        obj = bpy.data.objects.get(rp)
        if not obj:
            continue
        saved_matrix = obj.matrix_world.copy()
        obj.parent = arm_obj
        obj.parent_type = 'BONE'
        obj.parent_bone = "Hand.R"
        bpy.context.view_layer.update()
        obj.matrix_world = saved_matrix
        parented += 1
    
    print(f"Layer 9: Parented {parented} objects to bones (world matrix preserved)")
    return arm_obj


# ============================================================
# LAYER 10 & 11: POSE TEST + ANIMATIONS
# ============================================================

def layer10_11_animations(arm_obj):
    """Create animation actions: Idle, Walk, Cast."""
    bpy.context.view_layer.objects.active = arm_obj
    arm_obj.select_set(True)
    
    # Ensure we're in OBJECT mode first
    bpy.ops.object.mode_set(mode='OBJECT')
    
    # Set up animation data
    if not arm_obj.animation_data:
        arm_obj.animation_data_create()
    
    fps = 24
    bpy.context.scene.render.fps = fps
    
    # ---- ACTION 1: IDLE ----
    idle_action = bpy.data.actions.new("Fisherman_Idle")
    arm_obj.animation_data.action = idle_action
    
    bpy.ops.object.mode_set(mode='POSE')
    
    # Slight breathing bob on spine
    spine = arm_obj.pose.bones.get("Spine")
    head = arm_obj.pose.bones.get("Head")
    
    for frame in [1, 24, 48]:
        bpy.context.scene.frame_set(frame)
        
        if spine:
            if frame == 24:
                spine.location.z = 0.005
            else:
                spine.location.z = 0.0
            spine.keyframe_insert(data_path="location", frame=frame)
        
        if head:
            if frame == 24:
                head.rotation_euler.x = math.radians(-2)
            else:
                head.rotation_euler.x = 0
            head.rotation_euler.order = 'XYZ'
            head.rotation_mode = 'XYZ'
            head.keyframe_insert(data_path="rotation_euler", frame=frame)
    
    # Make cyclic
    if idle_action.fcurves:
        for fc in idle_action.fcurves:
            mod = fc.modifiers.new('CYCLES')
    
    print("Layer 11: Idle animation created (48 frames)")
    
    # ---- ACTION 2: WALK ----
    walk_action = bpy.data.actions.new("Fisherman_Walk")
    arm_obj.animation_data.action = walk_action
    
    # Reset all bones first
    for pb in arm_obj.pose.bones:
        pb.location = (0, 0, 0)
        pb.rotation_euler = (0, 0, 0)
        pb.rotation_mode = 'XYZ'
    
    ul_l = arm_obj.pose.bones.get("UpperLeg.L")
    ul_r = arm_obj.pose.bones.get("UpperLeg.R")
    ll_l = arm_obj.pose.bones.get("LowerLeg.L")
    ll_r = arm_obj.pose.bones.get("LowerLeg.R")
    ua_l = arm_obj.pose.bones.get("UpperArm.L")
    ua_r = arm_obj.pose.bones.get("UpperArm.R")
    root = arm_obj.pose.bones.get("Root")
    
    walk_bones = [ul_l, ul_r, ll_l, ll_r, ua_l, ua_r, root, spine]
    
    # Walk cycle: 24 frames
    # Frame 1: Contact (L foot forward)
    # Frame 7: Down
    # Frame 13: Passing
    # Frame 19: Contact (R foot forward)
    # Frame 24: = Frame 1
    
    walk_data = {
        1:  {"ul_l": -20, "ul_r": 20, "ll_l": 10, "ll_r": -5, "ua_l": 15, "ua_r": -15, "root_z": 0.0, "spine_x": 2},
        7:  {"ul_l": -10, "ul_r": 10, "ll_l": 5, "ll_r": -10, "ua_l": 8, "ua_r": -8, "root_z": -0.005, "spine_x": 0},
        13: {"ul_l": 5, "ul_r": -5, "ll_l": 0, "ll_r": 0, "ua_l": -5, "ua_r": 5, "root_z": 0.0, "spine_x": -1},
        19: {"ul_l": 20, "ul_r": -20, "ll_l": -5, "ll_r": 10, "ua_l": -15, "ua_r": 15, "root_z": 0.0, "spine_x": -2},
        24: {"ul_l": -20, "ul_r": 20, "ll_l": 10, "ll_r": -5, "ua_l": 15, "ua_r": -15, "root_z": 0.0, "spine_x": 2},
    }
    
    for frame, data in walk_data.items():
        bpy.context.scene.frame_set(frame)
        
        if ul_l:
            ul_l.rotation_euler.x = math.radians(data["ul_l"])
            ul_l.keyframe_insert(data_path="rotation_euler", frame=frame)
        if ul_r:
            ul_r.rotation_euler.x = math.radians(data["ul_r"])
            ul_r.keyframe_insert(data_path="rotation_euler", frame=frame)
        if ll_l:
            ll_l.rotation_euler.x = math.radians(data["ll_l"])
            ll_l.keyframe_insert(data_path="rotation_euler", frame=frame)
        if ll_r:
            ll_r.rotation_euler.x = math.radians(data["ll_r"])
            ll_r.keyframe_insert(data_path="rotation_euler", frame=frame)
        if ua_l:
            ua_l.rotation_euler.x = math.radians(data["ua_l"])
            ua_l.keyframe_insert(data_path="rotation_euler", frame=frame)
        if ua_r:
            ua_r.rotation_euler.x = math.radians(data["ua_r"])
            ua_r.keyframe_insert(data_path="rotation_euler", frame=frame)
        if root:
            root.location.z = data["root_z"]
            root.keyframe_insert(data_path="location", frame=frame)
        if spine:
            spine.rotation_euler.x = math.radians(data["spine_x"])
            spine.keyframe_insert(data_path="rotation_euler", frame=frame)
    
    if walk_action.fcurves:
        for fc in walk_action.fcurves:
            mod = fc.modifiers.new('CYCLES')
    
    print("Layer 11: Walk animation created (24 frames)")
    
    # ---- ACTION 3: CAST ----
    cast_action = bpy.data.actions.new("Fisherman_Cast")
    arm_obj.animation_data.action = cast_action
    
    # Reset
    for pb in arm_obj.pose.bones:
        pb.location = (0, 0, 0)
        pb.rotation_euler = (0, 0, 0)
    
    ua_r_bone = arm_obj.pose.bones.get("UpperArm.R")
    la_r_bone = arm_obj.pose.bones.get("LowerArm.R")
    hand_r = arm_obj.pose.bones.get("Hand.R")
    chest_bone = arm_obj.pose.bones.get("Chest")
    
    cast_frames = {
        1:  {"ua_r": 0, "la_r": 0, "hand_r": 0, "chest": 0},     # Neutral
        8:  {"ua_r": -80, "la_r": -30, "hand_r": -20, "chest": 10},  # Wind up (arm back)
        14: {"ua_r": 60, "la_r": 15, "hand_r": 30, "chest": -8},   # Forward cast
        20: {"ua_r": 30, "la_r": 5, "hand_r": 10, "chest": -3},    # Follow through
        30: {"ua_r": 15, "la_r": 0, "hand_r": 5, "chest": 0},      # Settle
    }
    
    for frame, data in cast_frames.items():
        bpy.context.scene.frame_set(frame)
        if ua_r_bone:
            ua_r_bone.rotation_euler.x = math.radians(data["ua_r"])
            ua_r_bone.keyframe_insert(data_path="rotation_euler", frame=frame)
        if la_r_bone:
            la_r_bone.rotation_euler.x = math.radians(data["la_r"])
            la_r_bone.keyframe_insert(data_path="rotation_euler", frame=frame)
        if hand_r:
            hand_r.rotation_euler.x = math.radians(data["hand_r"])
            hand_r.keyframe_insert(data_path="rotation_euler", frame=frame)
        if chest_bone:
            chest_bone.rotation_euler.x = math.radians(data["chest"])
            chest_bone.keyframe_insert(data_path="rotation_euler", frame=frame)
    
    print("Layer 11: Cast animation created (30 frames)")
    
    # Return to object mode, set to idle
    bpy.ops.object.mode_set(mode='OBJECT')
    arm_obj.animation_data.action = idle_action
    bpy.context.scene.frame_set(1)


# ============================================================
# RENDER FINAL VIEWS
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
            print(f"Rendered {suffix}")


# ============================================================
# MAIN
# ============================================================

def main():
    layer8_rig_prep()
    arm_obj = layer9_rigging()
    layer10_11_animations(arm_obj)
    
    bpy.ops.wm.save_as_mainfile(filepath=str(BLEND_PATH))
    print(f"Saved to {BLEND_PATH}")
    
    render_views()
    print("Phase 3 (Layer 8-11) complete!")

main()
