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


def apply_pose_keyframes(arm_obj):
    # Go to Pose mode
    bpy.context.view_layer.objects.active = arm_obj
    bpy.ops.object.mode_set(mode='POSE')
    
    bones = arm_obj.pose.bones
    for bone in bones:
        reset_pose_bone(bone)

    # ==========================================
    # FRAME 1: NEUTRAL POSE (A-POSE)
    # ==========================================
    for bone in bones:
        insert_bone_keyframes(bone, 1)

    # ==========================================
    # FRAME 10: HOLD POSE (HOLDING FISHING ROD)
    # ==========================================
    # Bring arms forward and bent to grip the rod
    # Left Arm
    bones["UpperArm_L"].rotation_euler = (math.radians(35), 0.0, math.radians(-15))
    bones["Forearm_L"].rotation_euler = (math.radians(45), 0.0, 0.0)
    # Right Arm
    bones["UpperArm_R"].rotation_euler = (math.radians(45), 0.0, math.radians(15))
    bones["Forearm_R"].rotation_euler = (math.radians(55), 0.0, 0.0)
    
    # Spine forward tilt
    bones["Spine"].rotation_euler = (math.radians(5), 0.0, 0.0)
    
    for bone in bones:
        insert_bone_keyframes(bone, 10)
    
    # Reset
    for bone in bones:
        reset_pose_bone(bone)

    # ==========================================
    # FRAME 20: SIT POSE (SITTING IN BOAT)
    # ==========================================
    # Lower hips
    bones["Hips"].location = (0.0, 0.0, -0.14)
    # Bend thighs 90 degrees forward
    bones["Thigh_L"].rotation_euler = (math.radians(-85), 0.0, 0.0)
    bones["Thigh_R"].rotation_euler = (math.radians(-85), 0.0, 0.0)
    # Bend shins 90 degrees back
    bones["Shin_L"].rotation_euler = (math.radians(90), 0.0, 0.0)
    bones["Shin_R"].rotation_euler = (math.radians(90), 0.0, 0.0)
    
    # Arm resting
    bones["UpperArm_L"].rotation_euler = (math.radians(15), 0.0, math.radians(-10))
    bones["UpperArm_R"].rotation_euler = (math.radians(15), 0.0, math.radians(10))
    
    for bone in bones:
        insert_bone_keyframes(bone, 20)

    # Reset
    for bone in bones:
        reset_pose_bone(bone)

    # ==========================================
    # FRAME 30: CAST POSE (CASTING THE LINE)
    # ==========================================
    # Body tilt back
    bones["Hips"].rotation_euler = (math.radians(-10), 0.0, 0.0)
    bones["Spine"].rotation_euler = (math.radians(-15), 0.0, 0.0)
    
    # Right arm raised high overhead holding rod
    bones["UpperArm_R"].rotation_euler = (math.radians(-70), 0.0, math.radians(25))
    bones["Forearm_R"].rotation_euler = (math.radians(45), 0.0, 0.0)
    
    # Left arm stabilization
    bones["UpperArm_L"].rotation_euler = (math.radians(40), 0.0, math.radians(-20))
    bones["Forearm_L"].rotation_euler = (math.radians(30), 0.0, 0.0)
    
    # Legs stance (one foot back slightly)
    bones["Thigh_L"].rotation_euler = (math.radians(-15), 0.0, 0.0)
    bones["Thigh_R"].rotation_euler = (math.radians(10), 0.0, 0.0)
    bones["Foot_R"].rotation_euler = (math.radians(-10), 0.0, 0.0)

    for bone in bones:
        insert_bone_keyframes(bone, 30)

    # Return to Object mode
    bpy.ops.object.mode_set(mode='OBJECT')
    print("Pose keyframes created on timeline")


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

    apply_pose_keyframes(arm_obj)

    # Save the file
    bpy.ops.wm.save_as_mainfile(filepath=str(BLEND_PATH))
    print(f"Layer 10 pose test complete and saved to {BLEND_PATH}")
    
    # Set frame to 30 (Cast Pose) and render views to verify deformations
    render_all_views_at_frame(30)


# Execute directly in Blender MCP environment
main()
