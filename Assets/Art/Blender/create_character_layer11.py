import math
import os
from pathlib import Path

import bpy


PROJECT_ROOT = Path(r"C:\Users\minil\GameMaking\AfterBlue")
BLEND_PATH = PROJECT_ROOT / "Assets" / "Art" / "Blender" / "character_v01.blend"


def ensure_dirs():
    BLEND_PATH.parent.mkdir(parents=True, exist_ok=True)


def reset_pose_bone(bone):
    bone.rotation_mode = 'XYZ'
    bone.rotation_euler = (0.0, 0.0, 0.0)
    bone.location = (0.0, 0.0, 0.0)


def insert_bone_keyframes(bone, frame):
    bone.keyframe_insert(data_path="rotation_euler", frame=frame)
    bone.keyframe_insert(data_path="location", frame=frame)


def build_animations(arm_obj):
    # Ensure animation data exists
    if not arm_obj.animation_data:
        arm_obj.animation_data_create()
        
    bpy.context.view_layer.objects.active = arm_obj
    bpy.ops.object.mode_set(mode='POSE')
    bones = arm_obj.pose.bones

    # ==========================================
    # 1. CREATE IDLE ACTION (Breathing loop)
    # ==========================================
    idle_action = bpy.data.actions.get("Fisherman_Idle")
    if idle_action:
        bpy.data.actions.remove(idle_action)
    idle_action = bpy.data.actions.new("Fisherman_Idle")
    
    arm_obj.animation_data.action = idle_action

    # Frame 1: Base Pose
    for bone in bones:
        reset_pose_bone(bone)
        insert_bone_keyframes(bone, 1)

    # Frame 15: Breathing peak (Compressed down/forward)
    bones["Hips"].location = (0.0, 0.0, -0.012)
    bones["Spine"].rotation_euler = (math.radians(2.0), 0.0, 0.0)
    bones["Head"].rotation_euler = (math.radians(3.0), 0.0, 0.0)
    bones["UpperArm_L"].rotation_euler = (math.radians(4.0), 0.0, math.radians(-3.0))
    bones["UpperArm_R"].rotation_euler = (math.radians(4.0), 0.0, math.radians(3.0))
    for bone in bones:
        insert_bone_keyframes(bone, 15)

    # Frame 30: Loop end (back to base)
    for bone in bones:
        reset_pose_bone(bone)
        insert_bone_keyframes(bone, 30)

    print("Created 'Fisherman_Idle' Action clip")

    # ==========================================
    # 2. CREATE WALK cycle ACTION (Walk loop)
    # ==========================================
    walk_action = bpy.data.actions.get("Fisherman_Walk")
    if walk_action:
        bpy.data.actions.remove(walk_action)
    walk_action = bpy.data.actions.new("Fisherman_Walk")
    
    arm_obj.animation_data.action = walk_action

    # --- Frame 1: Contact Point A (Left foot forward, Right foot back) ---
    for bone in bones:
        reset_pose_bone(bone)
    # Left Leg forward
    bones["Thigh_L"].rotation_euler = (math.radians(-25), 0.0, 0.0)
    bones["Shin_L"].rotation_euler = (math.radians(10), 0.0, 0.0)
    bones["Foot_L"].rotation_euler = (math.radians(15), 0.0, 0.0)
    # Right Leg back
    bones["Thigh_R"].rotation_euler = (math.radians(20), 0.0, 0.0)
    bones["Shin_R"].rotation_euler = (math.radians(20), 0.0, 0.0)
    bones["Foot_R"].rotation_euler = (math.radians(-5), 0.0, 0.0)
    # Arms (Right arm forward, Left arm back)
    bones["UpperArm_L"].rotation_euler = (math.radians(-15), 0.0, math.radians(-5))
    bones["UpperArm_R"].rotation_euler = (math.radians(25), 0.0, math.radians(5))
    for bone in bones:
        insert_bone_keyframes(bone, 1)

    # --- Frame 7: Passing Position A (Right foot swings forward) ---
    for bone in bones:
        reset_pose_bone(bone)
    bones["Hips"].location = (0.0, 0.0, 0.008) # Hip rise
    bones["Thigh_L"].rotation_euler = (0.0, 0.0, 0.0)
    bones["Thigh_R"].rotation_euler = (math.radians(-10), 0.0, 0.0) # Right knee lift
    bones["Shin_R"].rotation_euler = (math.radians(35), 0.0, 0.0)
    for bone in bones:
        insert_bone_keyframes(bone, 7)

    # --- Frame 13: Contact Point B (Right foot forward, Left foot back) ---
    for bone in bones:
        reset_pose_bone(bone)
    # Right Leg forward
    bones["Thigh_R"].rotation_euler = (math.radians(-25), 0.0, 0.0)
    bones["Shin_R"].rotation_euler = (math.radians(10), 0.0, 0.0)
    bones["Foot_R"].rotation_euler = (math.radians(15), 0.0, 0.0)
    # Left Leg back
    bones["Thigh_L"].rotation_euler = (math.radians(20), 0.0, 0.0)
    bones["Shin_L"].rotation_euler = (math.radians(20), 0.0, 0.0)
    bones["Foot_L"].rotation_euler = (math.radians(-5), 0.0, 0.0)
    # Arms (Left arm forward, Right arm back)
    bones["UpperArm_L"].rotation_euler = (math.radians(25), 0.0, math.radians(-5))
    bones["UpperArm_R"].rotation_euler = (math.radians(-15), 0.0, math.radians(5))
    for bone in bones:
        insert_bone_keyframes(bone, 13)

    # --- Frame 19: Passing Position B (Left foot swings forward) ---
    for bone in bones:
        reset_pose_bone(bone)
    bones["Hips"].location = (0.0, 0.0, 0.008)
    bones["Thigh_R"].rotation_euler = (0.0, 0.0, 0.0)
    bones["Thigh_L"].rotation_euler = (math.radians(-10), 0.0, 0.0) # Left knee lift
    bones["Shin_L"].rotation_euler = (math.radians(35), 0.0, 0.0)
    for bone in bones:
        insert_bone_keyframes(bone, 19)

    # --- Frame 25: Loop end (Identical to Frame 1) ---
    for bone in bones:
        reset_pose_bone(bone)
    bones["Thigh_L"].rotation_euler = (math.radians(-25), 0.0, 0.0)
    bones["Shin_L"].rotation_euler = (math.radians(10), 0.0, 0.0)
    bones["Foot_L"].rotation_euler = (math.radians(15), 0.0, 0.0)
    bones["Thigh_R"].rotation_euler = (math.radians(20), 0.0, 0.0)
    bones["Shin_R"].rotation_euler = (math.radians(20), 0.0, 0.0)
    bones["Foot_R"].rotation_euler = (math.radians(-5), 0.0, 0.0)
    bones["UpperArm_L"].rotation_euler = (math.radians(-15), 0.0, math.radians(-5))
    bones["UpperArm_R"].rotation_euler = (math.radians(25), 0.0, math.radians(5))
    for bone in bones:
        insert_bone_keyframes(bone, 25)

    print("Created 'Fisherman_Walk' Action clip")
    
    # Return to Object mode
    bpy.ops.object.mode_set(mode='OBJECT')


def render_all_views_at_frame(frame):
    bpy.context.scene.frame_set(frame)
    bpy.context.scene.render.engine = 'BLENDER_EEVEE'
    bpy.context.scene.render.resolution_x = 800
    bpy.context.scene.render.resolution_y = 600

    cameras = ["Camera_Front", "Camera_Side", "Camera_3_4"]
    temp_dir = PROJECT_ROOT / "Temp"
    temp_dir.mkdir(parents=True, exist_ok=True)

    for cam_name in cameras:
        cam_obj = bpy.data.objects.get(cam_name)
        if cam_obj:
            bpy.context.scene.camera = cam_obj
            out_name = f"scene_setup_{cam_name.split('_')[1].lower()}.png"
            out_path = temp_dir / out_name
            bpy.context.scene.render.filepath = str(out_path)
            
            # Render still frame
            bpy.ops.render.render(write_still=True)
            print(f"Rendered {cam_name} view at frame {frame} to {out_path}")


def main():
    ensure_dirs()
    
    arm_obj = bpy.data.objects.get("AB_CHAR_Armature")
    if not arm_obj:
        print("Error: AB_CHAR_Armature not found!")
        return

    build_animations(arm_obj)

    # Save the file
    bpy.ops.wm.save_as_mainfile(filepath=str(BLEND_PATH))
    print(f"Layer 11 animations complete and saved to {BLEND_PATH}")
    
    # Render Walk Action Frame 13 (Right leg stride forward)
    render_all_views_at_frame(13)


# Execute directly in Blender MCP environment
main()
