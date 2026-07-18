"""
Phase 3 Fix: Animation creation with Blender 5.1 compatible API.
The walk action fcurves are accessed through animation data, not directly on Action.
"""
import math
import bpy
from pathlib import Path

PROJECT_ROOT = Path(r"C:\Users\minil\GameMaking\AfterBlue")
BLEND_PATH = PROJECT_ROOT / "Assets" / "Art" / "Blender" / "character_v01.blend"

arm_obj = bpy.data.objects.get("Fisherman_Rig")
if not arm_obj:
    raise RuntimeError("Armature 'Fisherman_Rig' not found!")

bpy.context.view_layer.objects.active = arm_obj
arm_obj.select_set(True)

if not arm_obj.animation_data:
    arm_obj.animation_data_create()

fps = 24
bpy.context.scene.render.fps = fps

# Helper: safely add CYCLES modifier to all fcurves of an action
def make_cyclic(action):
    """Add CYCLES modifier to all fcurves, compatible with Blender 5.x"""
    try:
        # Try Blender 5.1+ way: fcurves may be on animation layers/strips
        if hasattr(action, 'fcurves') and action.fcurves:
            for fc in action.fcurves:
                mod = fc.modifiers.new('CYCLES')
            print(f"  Made {action.name} cyclic via action.fcurves")
            return
    except (AttributeError, TypeError):
        pass
    
    # Try via NLA or animation data
    try:
        # In Blender 5.x, set action on anim data and access fcurves from there
        arm_obj.animation_data.action = action
        bpy.context.view_layer.update()
        if arm_obj.animation_data.action:
            # Access fcurves through the action that's now bound
            act = arm_obj.animation_data.action
            # Check if fcurves are iterable
            try:
                for fc in act.fcurves:
                    if not any(m.type == 'CYCLES' for m in fc.modifiers):
                        mod = fc.modifiers.new('CYCLES')
                print(f"  Made {action.name} cyclic via bound action fcurves")
                return
            except:
                pass
    except:
        pass
    
    print(f"  Warning: Could not make {action.name} cyclic (API incompatibility)")


# ---- ACTION 1: IDLE ----
bpy.ops.object.mode_set(mode='POSE')

# Reset all bones
for pb in arm_obj.pose.bones:
    pb.location = (0, 0, 0)
    pb.rotation_euler = (0, 0, 0)
    pb.rotation_mode = 'XYZ'

idle_action = bpy.data.actions.get("Fisherman_Idle")
if not idle_action:
    idle_action = bpy.data.actions.new("Fisherman_Idle")
arm_obj.animation_data.action = idle_action

spine = arm_obj.pose.bones.get("Spine")
head_bone = arm_obj.pose.bones.get("Head")

for frame in [1, 24, 48]:
    bpy.context.scene.frame_set(frame)
    if spine:
        spine.location.z = 0.005 if frame == 24 else 0.0
        spine.keyframe_insert(data_path="location", frame=frame)
    if head_bone:
        head_bone.rotation_mode = 'XYZ'
        head_bone.rotation_euler.x = math.radians(-2) if frame == 24 else 0
        head_bone.keyframe_insert(data_path="rotation_euler", frame=frame)

make_cyclic(idle_action)
print("Idle animation done (48 frames)")


# ---- ACTION 2: WALK ----
walk_action = bpy.data.actions.get("Fisherman_Walk")
if not walk_action:
    walk_action = bpy.data.actions.new("Fisherman_Walk")
arm_obj.animation_data.action = walk_action

# Reset
for pb in arm_obj.pose.bones:
    pb.location = (0, 0, 0)
    pb.rotation_euler = (0, 0, 0)

ul_l = arm_obj.pose.bones.get("UpperLeg.L")
ul_r = arm_obj.pose.bones.get("UpperLeg.R")
ll_l = arm_obj.pose.bones.get("LowerLeg.L")
ll_r = arm_obj.pose.bones.get("LowerLeg.R")
ua_l = arm_obj.pose.bones.get("UpperArm.L")
ua_r = arm_obj.pose.bones.get("UpperArm.R")
root = arm_obj.pose.bones.get("Root")

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

make_cyclic(walk_action)
print("Walk animation done (24 frames)")


# ---- ACTION 3: CAST ----
cast_action = bpy.data.actions.get("Fisherman_Cast")
if not cast_action:
    cast_action = bpy.data.actions.new("Fisherman_Cast")
arm_obj.animation_data.action = cast_action

for pb in arm_obj.pose.bones:
    pb.location = (0, 0, 0)
    pb.rotation_euler = (0, 0, 0)

ua_r_bone = arm_obj.pose.bones.get("UpperArm.R")
la_r_bone = arm_obj.pose.bones.get("LowerArm.R")
hand_r = arm_obj.pose.bones.get("Hand.R")
chest_bone = arm_obj.pose.bones.get("Chest")

cast_frames = {
    1:  {"ua_r": 0, "la_r": 0, "hand_r": 0, "chest": 0},
    8:  {"ua_r": -80, "la_r": -30, "hand_r": -20, "chest": 10},
    14: {"ua_r": 60, "la_r": 15, "hand_r": 30, "chest": -8},
    20: {"ua_r": 30, "la_r": 5, "hand_r": 10, "chest": -3},
    30: {"ua_r": 15, "la_r": 0, "hand_r": 5, "chest": 0},
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

print("Cast animation done (30 frames)")

# Return to Object mode, set idle action, frame 1
bpy.ops.object.mode_set(mode='OBJECT')
arm_obj.animation_data.action = idle_action
bpy.context.scene.frame_set(1)

# Render walk pose at frame 7 for verification
arm_obj.animation_data.action = walk_action
bpy.context.scene.frame_set(7)

bpy.ops.wm.save_as_mainfile(filepath=str(BLEND_PATH))
print(f"Saved to {BLEND_PATH}")

# Render views
temp = PROJECT_ROOT / "Temp"
bpy.context.scene.render.engine = 'BLENDER_EEVEE'
bpy.context.scene.render.resolution_x = 800
bpy.context.scene.render.resolution_y = 600

for cam_name, suffix in [("Camera_Front","front"), ("Camera_Side","side"), ("Camera_3_4","3")]:
    cam = bpy.data.objects.get(cam_name)
    if cam:
        bpy.context.scene.camera = cam
        path = temp / f"scene_setup_{suffix}.png"
        bpy.context.scene.render.filepath = str(path)
        bpy.ops.render.render(write_still=True)
        print(f"Rendered {suffix}")

# Set back to idle
arm_obj.animation_data.action = idle_action
bpy.context.scene.frame_set(1)
bpy.ops.wm.save_as_mainfile(filepath=str(BLEND_PATH))

print("Phase 3 (Layer 8-11) complete!")
