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


def create_cube(collection, name, location, scale, material, rotation=(0.0, 0.0, 0.0)):
    bpy.ops.mesh.primitive_cube_add(size=1, location=location, rotation=rotation)
    obj = link_to(collection, bpy.context.object)
    obj.name = name
    obj.dimensions = scale
    bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    obj.data.materials.append(material)
    return obj


def create_sphere(collection, name, location, radius, material, scale=(1.0, 1.0, 1.0), rotation=(0.0, 0.0, 0.0)):
    bpy.ops.mesh.primitive_uv_sphere_add(radius=radius, location=location, rotation=rotation)
    obj = link_to(collection, bpy.context.object)
    obj.name = name
    obj.scale = scale
    bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    obj.data.materials.append(material)
    return obj


def create_cylinder(collection, name, location, radius, depth, material, rotation=(0.0, 0.0, 0.0), scale=(1.0, 1.0, 1.0)):
    bpy.ops.mesh.primitive_cylinder_add(
        vertices=12,
        radius=radius,
        depth=depth,
        location=location,
        rotation=rotation
    )
    obj = link_to(collection, bpy.context.object)
    obj.name = name
    obj.scale = scale
    bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    obj.data.materials.append(material)
    return obj


def create_torus(collection, name, location, major_rad, minor_rad, material, rotation=(0.0, 0.0, 0.0), scale=(1.0, 1.0, 1.0)):
    bpy.ops.mesh.primitive_torus_add(
        major_radius=major_rad,
        minor_radius=minor_rad,
        location=location,
        rotation=rotation
    )
    obj = link_to(collection, bpy.context.object)
    obj.name = name
    obj.scale = scale
    bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    obj.data.materials.append(material)
    return obj


def parent_to_bone_safe(obj_name, armature_obj, bone_name):
    obj = bpy.data.objects.get(obj_name)
    if obj:
        # Save the absolute world matrix
        world_matrix = obj.matrix_world.copy()
        
        # Assign parent to bone
        obj.parent = armature_obj
        obj.parent_type = 'BONE'
        obj.parent_bone = bone_name
        
        # Restore the world matrix to prevent jumping/warping
        obj.matrix_world = world_matrix
        print(f"Safely parented {obj_name} to bone {bone_name} maintaining position")


def rebuild_and_parent_details():
    col_char = get_or_create_collection("02_Character")
    arm_obj = bpy.data.objects.get("AB_CHAR_Armature")
    if not arm_obj:
        print("Error: AB_CHAR_Armature not found!")
        return

    # Retrieve materials
    skin_mat = bpy.data.materials.get("Char_Skin")
    dark_mat = bpy.data.materials.get("Char_Dark")
    raincoat_mat = bpy.data.materials.get("Char_Raincoat")
    bag_mat = bpy.data.materials.get("Char_Bag")
    metal_mat = bpy.data.materials.get("Prop_Metal")
    white_mat = bpy.data.materials.get("Prop_White")

    # 1. Delete old distorted details
    details_to_rebuild = [
        "Char_Detail_Eye_L", "Char_Detail_Eye_R",
        "Char_Detail_Brow_L", "Char_Detail_Brow_R",
        "Char_Detail_Hair_1", "Char_Detail_Hair_2", "Char_Detail_Hair_3",
        "Char_Detail_Button_1", "Char_Detail_Button_2",
        "Char_Detail_Pocket_L", "Char_Detail_Pocket_R",
        "Char_Detail_String_L", "Char_Detail_String_R",
        "Char_Block_Satchel", "Char_Silhouette_Strap",
        "Char_Detail_SatchelFlap", "Char_Detail_SatchelBuckle"
    ]
    for d_name in details_to_rebuild:
        old_obj = bpy.data.objects.get(d_name)
        if old_obj:
            bpy.data.objects.remove(old_obj, do_unlink=True)

    # 2. Re-create all details at correct world locations
    # Bag
    satchel = create_cube(col_char, "Char_Block_Satchel", (-0.21, 0.05, 0.48), (0.16, 0.08, 0.16), bag_mat)
    strap = create_torus(col_char, "Char_Silhouette_Strap", (-0.02, 0.02, 0.65), 0.22, 0.012, bag_mat, rotation=(math.radians(70), math.radians(20), 0.0), scale=(0.7, 0.8, 1.3))
    flap = create_cube(col_char, "Char_Detail_SatchelFlap", (-0.21, 0.05, 0.545), (0.17, 0.09, 0.03), bag_mat)
    buckle = create_cube(col_char, "Char_Detail_SatchelBuckle", (-0.21, -0.002, 0.50), (0.03, 0.01, 0.04), metal_mat)

    # Eyes
    eye_l = create_sphere(col_char, "Char_Detail_Eye_L", (-0.08, -0.226, 0.88), 0.022, dark_mat, scale=(1.0, 0.4, 1.7))
    eye_r = create_sphere(col_char, "Char_Detail_Eye_R", (0.08, -0.226, 0.88), 0.022, dark_mat, scale=(1.0, 0.4, 1.7))

    # Eyebrows
    brow_l = create_cube(col_char, "Char_Detail_Brow_L", (-0.08, -0.232, 0.94), (0.045, 0.01, 0.012), dark_mat, rotation=(0.0, 0.0, math.radians(-15)))
    brow_r = create_cube(col_char, "Char_Detail_Brow_R", (0.08, -0.232, 0.94), (0.045, 0.01, 0.012), dark_mat, rotation=(0.0, 0.0, math.radians(15)))

    # Hair Tufts
    hair_1 = create_cube(col_char, "Char_Detail_Hair_1", (-0.11, -0.226, 0.99), (0.05, 0.025, 0.08), dark_mat, rotation=(math.radians(15), 0.0, math.radians(-15)))
    hair_2 = create_cube(col_char, "Char_Detail_Hair_2", (0.11, -0.226, 0.99), (0.05, 0.025, 0.08), dark_mat, rotation=(math.radians(15), 0.0, math.radians(15)))
    hair_3 = create_cube(col_char, "Char_Detail_Hair_3", (0.0, -0.232, 0.98), (0.065, 0.03, 0.11), dark_mat, rotation=(math.radians(20), 0.0, 0.0))

    # Buttons
    btn_1 = create_cylinder(col_char, "Char_Detail_Button_1", (0.0, -0.19, 0.62), 0.012, 0.01, white_mat, rotation=(math.radians(90), 0.0, 0.0))
    btn_2 = create_cylinder(col_char, "Char_Detail_Button_2", (0.0, -0.21, 0.46), 0.012, 0.01, white_mat, rotation=(math.radians(90), 0.0, 0.0))

    # Pockets
    pocket_l = create_cube(col_char, "Char_Detail_Pocket_L", (-0.13, -0.18, 0.42), (0.07, 0.01, 0.07), raincoat_mat)
    pocket_r = create_cube(col_char, "Char_Detail_Pocket_R", (0.13, -0.18, 0.42), (0.07, 0.01, 0.07), raincoat_mat)

    # Strings
    str_l = create_cylinder(col_char, "Char_Detail_String_L", (-0.03, -0.19, 0.68), 0.005, 0.08, white_mat, rotation=(math.radians(10), 0.0, 0.0))
    str_r = create_cylinder(col_char, "Char_Detail_String_R", (0.03, -0.19, 0.68), 0.005, 0.08, white_mat, rotation=(math.radians(10), 0.0, 0.0))

    # 3. Parent them SAFELY to the bones preserving their world matrices
    parent_to_bone_safe("Char_Detail_Eye_L", arm_obj, "Head")
    parent_to_bone_safe("Char_Detail_Eye_R", arm_obj, "Head")
    parent_to_bone_safe("Char_Detail_Brow_L", arm_obj, "Head")
    parent_to_bone_safe("Char_Detail_Brow_R", arm_obj, "Head")
    parent_to_bone_safe("Char_Detail_Hair_1", arm_obj, "Head")
    parent_to_bone_safe("Char_Detail_Hair_2", arm_obj, "Head")
    parent_to_bone_safe("Char_Detail_Hair_3", arm_obj, "Head")

    parent_to_bone_safe("Char_Detail_Button_1", arm_obj, "Spine")
    parent_to_bone_safe("Char_Detail_Button_2", arm_obj, "Spine")
    parent_to_bone_safe("Char_Detail_Pocket_L", arm_obj, "Spine")
    parent_to_bone_safe("Char_Detail_Pocket_R", arm_obj, "Spine")
    parent_to_bone_safe("Char_Detail_String_L", arm_obj, "Spine")
    parent_to_bone_safe("Char_Detail_String_R", arm_obj, "Spine")

    parent_to_bone_safe("Char_Block_Satchel", arm_obj, "Spine")
    parent_to_bone_safe("Char_Silhouette_Strap", arm_obj, "Spine")
    parent_to_bone_safe("Char_Detail_SatchelFlap", arm_obj, "Spine")
    parent_to_bone_safe("Char_Detail_SatchelBuckle", arm_obj, "Spine")


def reset_pose_bone(bone):
    bone.rotation_mode = 'XYZ'
    bone.rotation_euler = (0.0, 0.0, 0.0)
    bone.location = (0.0, 0.0, 0.0)


def insert_bone_keyframes(bone, frame):
    bone.keyframe_insert(data_path="rotation_euler", frame=frame)
    bone.keyframe_insert(data_path="location", frame=frame)


def apply_animations_and_poses(arm_obj):
    # Go to Pose mode
    bpy.context.view_layer.objects.active = arm_obj
    bpy.ops.object.mode_set(mode='POSE')
    bones = arm_obj.pose.bones
    
    # 1. Clear any old keyframes from the armature
    if arm_obj.animation_data:
        arm_obj.animation_data.action = None

    # ==========================================
    # REBUILD IDLE ACTION
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

    # Frame 15: Breathing peak
    bones["Hips"].location = (0.0, 0.0, -0.012)
    bones["Spine"].rotation_euler = (math.radians(2.0), 0.0, 0.0)
    bones["Head"].rotation_euler = (math.radians(3.0), 0.0, 0.0)
    bones["UpperArm_L"].rotation_euler = (math.radians(4.0), 0.0, math.radians(-3.0))
    bones["UpperArm_R"].rotation_euler = (math.radians(4.0), 0.0, math.radians(3.0))
    for bone in bones:
        insert_bone_keyframes(bone, 15)

    # Frame 30: Loop end
    for bone in bones:
        reset_pose_bone(bone)
        insert_bone_keyframes(bone, 30)

    # ==========================================
    # REBUILD WALK ACTION
    # ==========================================
    walk_action = bpy.data.actions.get("Fisherman_Walk")
    if walk_action:
        bpy.data.actions.remove(walk_action)
    walk_action = bpy.data.actions.new("Fisherman_Walk")
    arm_obj.animation_data.action = walk_action

    # Frame 1: Contact Point A
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
        insert_bone_keyframes(bone, 1)

    # Frame 7: Passing Position A
    for bone in bones:
        reset_pose_bone(bone)
    bones["Hips"].location = (0.0, 0.0, 0.008)
    bones["Thigh_L"].rotation_euler = (0.0, 0.0, 0.0)
    bones["Thigh_R"].rotation_euler = (math.radians(-10), 0.0, 0.0)
    bones["Shin_R"].rotation_euler = (math.radians(35), 0.0, 0.0)
    for bone in bones:
        insert_bone_keyframes(bone, 7)

    # Frame 13: Contact Point B
    for bone in bones:
        reset_pose_bone(bone)
    bones["Thigh_R"].rotation_euler = (math.radians(-25), 0.0, 0.0)
    bones["Shin_R"].rotation_euler = (math.radians(10), 0.0, 0.0)
    bones["Foot_R"].rotation_euler = (math.radians(15), 0.0, 0.0)
    bones["Thigh_L"].rotation_euler = (math.radians(20), 0.0, 0.0)
    bones["Shin_L"].rotation_euler = (math.radians(20), 0.0, 0.0)
    bones["Foot_L"].rotation_euler = (math.radians(-5), 0.0, 0.0)
    bones["UpperArm_L"].rotation_euler = (math.radians(25), 0.0, math.radians(-5))
    bones["UpperArm_R"].rotation_euler = (math.radians(-15), 0.0, math.radians(5))
    for bone in bones:
        insert_bone_keyframes(bone, 13)

    # Frame 19: Passing Position B
    for bone in bones:
        reset_pose_bone(bone)
    bones["Hips"].location = (0.0, 0.0, 0.008)
    bones["Thigh_R"].rotation_euler = (0.0, 0.0, 0.0)
    bones["Thigh_L"].rotation_euler = (math.radians(-10), 0.0, 0.0)
    bones["Shin_L"].rotation_euler = (math.radians(35), 0.0, 0.0)
    for bone in bones:
        insert_bone_keyframes(bone, 19)

    # Frame 25: Loop end
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
    rebuild_and_parent_details()
    
    arm_obj = bpy.data.objects.get("AB_CHAR_Armature")
    if arm_obj:
        apply_animations_and_poses(arm_obj)

    # Save the file
    bpy.ops.wm.save_as_mainfile(filepath=str(BLEND_PATH))
    print(f"Rigging fix complete and saved to {BLEND_PATH}")
    
    # Set back to frame 13 (Walk cycle Contact) and render views
    render_all_views_at_frame(13)


# Execute directly in Blender MCP environment
main()
