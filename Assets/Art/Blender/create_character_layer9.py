import math
import os
from pathlib import Path

import bpy


PROJECT_ROOT = Path(r"C:\Users\minil\GameMaking\AfterBlue")
BLEND_PATH = PROJECT_ROOT / "Assets" / "Art" / "Blender" / "character_v01.blend"


def ensure_dirs():
    BLEND_PATH.parent.mkdir(parents=True, exist_ok=True)


def get_or_create_collection(name):
    col = bpy.data.collections.get(name)
    if not col:
        col = bpy.data.collections.new(name)
        bpy.context.scene.collection.children.link(col)
    return col


def link_to(collection, obj):
    for existing in list(obj.users_collection):
        existing.objects.unlink(obj)
    collection.objects.link(obj)
    return obj


def parent_to_bone(obj_name, armature_obj, bone_name):
    obj = bpy.data.objects.get(obj_name)
    if obj:
        obj.parent = armature_obj
        obj.parent_type = 'BONE'
        obj.parent_bone = bone_name
        print(f"Parented {obj_name} to bone {bone_name}")


def build_rig():
    # 1. Create armature data and object
    arm_name = "AB_CHAR_Armature"
    
    # Delete old armature if it exists
    old_arm = bpy.data.objects.get(arm_name)
    if old_arm:
        bpy.data.objects.remove(old_arm, do_unlink=True)
        
    arm_data = bpy.data.armatures.new(f"{arm_name}_Data")
    arm_obj = bpy.data.objects.new(arm_name, arm_data)
    
    rig_col = get_or_create_collection("04_Rig")
    link_to(rig_col, arm_obj)
    
    bpy.context.view_layer.objects.active = arm_obj
    arm_obj.select_set(True)
    
    # Show bones in front of geometry for easier viewport check
    arm_obj.show_in_front = True
    
    # Go to EDIT mode to build bones
    bpy.ops.object.mode_set(mode='EDIT')
    
    # 2. Build Backbone (Hips -> Spine -> Neck -> Head)
    hips = arm_data.edit_bones.new("Hips")
    hips.head = (0.0, 0.0, 0.32)
    hips.tail = (0.0, 0.0, 0.45)
    
    spine = arm_data.edit_bones.new("Spine")
    spine.head = (0.0, 0.0, 0.45)
    spine.tail = (0.0, 0.0, 0.72)
    spine.parent = hips
    
    neck = arm_data.edit_bones.new("Neck")
    neck.head = (0.0, 0.0, 0.72)
    neck.tail = (0.0, 0.0, 0.76)
    neck.parent = spine
    
    head = arm_data.edit_bones.new("Head")
    head.head = (0.0, 0.0, 0.76)
    head.tail = (0.0, 0.0, 1.15)
    head.parent = neck
    
    # 3. Build Left Arm (Shoulder -> UpperArm -> Forearm -> Hand)
    shoulder_l = arm_data.edit_bones.new("Shoulder_L")
    shoulder_l.head = (0.0, 0.0, 0.68)
    shoulder_l.tail = (-0.08, 0.0, 0.68)
    shoulder_l.parent = spine
    
    upperarm_l = arm_data.edit_bones.new("UpperArm_L")
    upperarm_l.head = (-0.08, 0.0, 0.68)
    upperarm_l.tail = (-0.15, 0.0, 0.52)
    upperarm_l.parent = shoulder_l
    
    forearm_l = arm_data.edit_bones.new("Forearm_L")
    forearm_l.head = (-0.15, 0.0, 0.52)
    forearm_l.tail = (-0.21, 0.0, 0.38)
    forearm_l.parent = upperarm_l
    
    hand_l = arm_data.edit_bones.new("Hand_L")
    hand_l.head = (-0.21, 0.0, 0.38)
    hand_l.tail = (-0.25, 0.0, 0.30)
    hand_l.parent = forearm_l

    # 4. Build Right Arm (Shoulder -> UpperArm -> Forearm -> Hand)
    shoulder_r = arm_data.edit_bones.new("Shoulder_R")
    shoulder_r.head = (0.0, 0.0, 0.68)
    shoulder_r.tail = (0.08, 0.0, 0.68)
    shoulder_r.parent = spine
    
    upperarm_r = arm_data.edit_bones.new("UpperArm_R")
    upperarm_r.head = (0.08, 0.0, 0.68)
    upperarm_r.tail = (0.15, 0.0, 0.52)
    upperarm_r.parent = shoulder_r
    
    forearm_r = arm_data.edit_bones.new("Forearm_R")
    forearm_r.head = (0.15, 0.0, 0.52)
    forearm_r.tail = (0.21, 0.0, 0.38)
    forearm_r.parent = upperarm_r
    
    hand_r = arm_data.edit_bones.new("Hand_R")
    hand_r.head = (0.21, 0.0, 0.38)
    hand_r.tail = (0.25, 0.0, 0.30)
    hand_r.parent = forearm_r

    # 5. Build Left Leg (Thigh -> Shin -> Foot)
    thigh_l = arm_data.edit_bones.new("Thigh_L")
    thigh_l.head = (-0.08, 0.0, 0.30)
    thigh_l.tail = (-0.08, 0.0, 0.16)
    thigh_l.parent = hips
    
    shin_l = arm_data.edit_bones.new("Shin_L")
    shin_l.head = (-0.08, 0.0, 0.16)
    shin_l.tail = (-0.08, 0.0, 0.05)
    shin_l.parent = thigh_l
    
    foot_l = arm_data.edit_bones.new("Foot_L")
    foot_l.head = (-0.08, 0.0, 0.05)
    foot_l.tail = (-0.08, -0.08, 0.01)
    foot_l.parent = shin_l

    # 6. Build Right Leg (Thigh -> Shin -> Foot)
    thigh_r = arm_data.edit_bones.new("Thigh_R")
    thigh_r.head = (0.08, 0.0, 0.30)
    thigh_r.tail = (0.08, 0.0, 0.16)
    thigh_r.parent = hips
    
    shin_r = arm_data.edit_bones.new("Shin_R")
    shin_r.head = (0.08, 0.0, 0.16)
    shin_r.tail = (0.08, 0.0, 0.05)
    shin_r.parent = thigh_r
    
    foot_r = arm_data.edit_bones.new("Foot_R")
    foot_r.head = (0.08, 0.0, 0.05)
    foot_r.tail = (0.08, -0.08, 0.01)
    foot_r.parent = shin_r

    # Exit edit mode
    bpy.ops.object.mode_set(mode='OBJECT')
    print("Armature skeleton constructed")

    # 7. Bind body mesh to armature using automatic weights
    body = bpy.data.objects.get("AB_CHAR_Fisherman_Body")
    if body:
        bpy.ops.object.select_all(action='DESELECT')
        body.select_set(True)
        arm_obj.select_set(True)
        bpy.context.view_layer.objects.active = arm_obj
        
        # Parent with Auto Weights
        bpy.ops.object.parent_set(type='ARMATURE_AUTO')
        print("Bound body mesh to armature with automatic weights")

    # 8. Re-parent details to target bones (decoupled from remesh body)
    parent_to_bone("Char_Detail_Eye_L", arm_obj, "Head")
    parent_to_bone("Char_Detail_Eye_R", arm_obj, "Head")
    parent_to_bone("Char_Detail_Brow_L", arm_obj, "Head")
    parent_to_bone("Char_Detail_Brow_R", arm_obj, "Head")
    parent_to_bone("Char_Detail_Hair_1", arm_obj, "Head")
    parent_to_bone("Char_Detail_Hair_2", arm_obj, "Head")
    parent_to_bone("Char_Detail_Hair_3", arm_obj, "Head")
    
    parent_to_bone("Char_Detail_Button_1", arm_obj, "Spine")
    parent_to_bone("Char_Detail_Button_2", arm_obj, "Spine")
    parent_to_bone("Char_Detail_Pocket_L", arm_obj, "Spine")
    parent_to_bone("Char_Detail_Pocket_R", arm_obj, "Spine")
    parent_to_bone("Char_Detail_String_L", arm_obj, "Spine")
    parent_to_bone("Char_Detail_String_R", arm_obj, "Spine")
    
    parent_to_bone("Char_Block_Satchel", arm_obj, "Spine")
    parent_to_bone("Char_Silhouette_Strap", arm_obj, "Spine")
    parent_to_bone("Char_Detail_SatchelFlap", arm_obj, "Spine")
    parent_to_bone("Char_Detail_SatchelBuckle", arm_obj, "Spine")


def render_all_views():
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
            print(f"Rendered {cam_name} view to {out_path}")


def main():
    ensure_dirs()
    build_rig()
    
    # Save the file
    bpy.ops.wm.save_as_mainfile(filepath=str(BLEND_PATH))
    print(f"Rigging complete and saved to {BLEND_PATH}")
    
    # Render preview cameras
    render_all_views()


# Execute directly in Blender MCP environment
main()
