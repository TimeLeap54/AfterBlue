import math
import os
from pathlib import Path

import bpy


PROJECT_ROOT = Path(r"C:\Users\minil\GameMaking\AfterBlue")
BLEND_PATH = PROJECT_ROOT / "Assets" / "Art" / "Blender" / "character_v01.blend"


def ensure_dirs():
    BLEND_PATH.parent.mkdir(parents=True, exist_ok=True)


def get_nodes_and_links(material_name):
    mat = bpy.data.materials.get(material_name)
    if not mat:
        mat = bpy.data.materials.new(material_name)
    mat.use_nodes = True
    nodes = mat.node_tree.nodes
    links = mat.node_tree.links
    nodes.clear() # Clear default nodes to rebuild cleanly
    
    # Base output and BSDF
    output = nodes.new(type="ShaderNodeOutputMaterial")
    output.location = (400, 0)
    bsdf = nodes.new(type="ShaderNodeBsdfPrincipled")
    bsdf.location = (100, 0)
    links.new(bsdf.outputs["BSDF"], output.inputs["Surface"])
    
    return mat, nodes, links, bsdf


def apply_boots_mud():
    mat, nodes, links, bsdf = get_nodes_and_links("Char_Boots")
    bsdf.inputs["Roughness"].default_value = 0.85
    
    # 1. Texture Coordinate (Generated)
    tex_coord = nodes.new(type="ShaderNodeTexCoord")
    tex_coord.location = (-600, 0)
    
    # 2. Separate XYZ
    sep_xyz = nodes.new(type="ShaderNodeSeparateXYZ")
    sep_xyz.location = (-400, 0)
    links.new(tex_coord.outputs["Generated"], sep_xyz.inputs["Vector"])
    
    # 3. Color Ramp (Mud gradient)
    color_ramp = nodes.new(type="ShaderNodeValToRGB")
    color_ramp.location = (-200, 0)
    # Stop 0 (Mud at bottom): Z = 0 to 0.15
    color_ramp.color_ramp.elements[0].position = 0.05
    color_ramp.color_ramp.elements[0].color = (0.22, 0.18, 0.13, 1.0) # Mud brown
    color_ramp.color_ramp.elements[1].position = 0.25
    color_ramp.color_ramp.elements[1].color = (0.35, 0.29, 0.22, 1.0) # Base boots color
    links.new(sep_xyz.outputs["Z"], color_ramp.inputs["Fac"])
    
    # Connect to base color
    links.new(color_ramp.outputs["Color"], bsdf.inputs["Base Color"])
    print("Applied boots mud shader")


def apply_raincoat_painterly():
    mat, nodes, links, bsdf = get_nodes_and_links("Char_Raincoat")
    bsdf.inputs["Roughness"].default_value = 0.78
    
    # 1. Texture Coordinate
    tex_coord = nodes.new(type="ShaderNodeTexCoord")
    tex_coord.location = (-800, 0)
    
    # 2. Noise Texture for soft painterly variations
    noise = nodes.new(type="ShaderNodeTexNoise")
    noise.location = (-600, 0)
    noise.inputs["Scale"].default_value = 6.0
    noise.inputs["Detail"].default_value = 0.0
    noise.inputs["Roughness"].default_value = 0.2
    links.new(tex_coord.outputs["Object"], noise.inputs["Vector"])
    
    # 3. Color Ramp
    color_ramp = nodes.new(type="ShaderNodeValToRGB")
    color_ramp.location = (-350, 0)
    color_ramp.color_ramp.elements[0].position = 0.3
    color_ramp.color_ramp.elements[0].color = (0.33, 0.46, 0.51, 1.0) # Darker worn blue-green
    color_ramp.color_ramp.elements[1].position = 0.7
    color_ramp.color_ramp.elements[1].color = (0.37, 0.52, 0.57, 1.0) # Base raincoat blue-green
    links.new(noise.outputs["Color"], color_ramp.inputs["Fac"])
    
    # 4. Separate XYZ for bottom-wear gradient (coat bottom Z gradient)
    sep_xyz = nodes.new(type="ShaderNodeSeparateXYZ")
    sep_xyz.location = (-350, -250)
    links.new(tex_coord.outputs["Generated"], sep_xyz.inputs["Vector"])
    
    grad_ramp = nodes.new(type="ShaderNodeValToRGB")
    grad_ramp.location = (-150, -250)
    grad_ramp.color_ramp.elements[0].position = 0.0
    grad_ramp.color_ramp.elements[0].color = (0.85, 0.85, 0.85, 1.0) # Darken bottom
    grad_ramp.color_ramp.elements[1].position = 0.35
    grad_ramp.color_ramp.elements[1].color = (1.0, 1.0, 1.0, 1.0) # Normal top
    links.new(sep_xyz.outputs["Z"], grad_ramp.inputs["Fac"])
    
    # 5. Mix Color to multiply Z gradient and Noise
    mix_node = nodes.new(type="ShaderNodeMix")
    mix_node.data_type = 'RGBA'
    mix_node.blend_type = 'MULTIPLY'
    mix_node.location = (100, -250)
    mix_node.inputs["Factor"].default_value = 1.0
    links.new(color_ramp.outputs["Color"], mix_node.inputs[0])
    links.new(grad_ramp.outputs["Color"], mix_node.inputs[1])
    
    # Connect mix to base color
    links.new(mix_node.outputs[2], bsdf.inputs["Base Color"])
    print("Applied raincoat painterly shader")


def apply_wood_grain():
    mat, nodes, links, bsdf = get_nodes_and_links("Prop_Wood")
    bsdf.inputs["Roughness"].default_value = 0.85
    
    # 1. Texture Coordinate
    tex_coord = nodes.new(type="ShaderNodeTexCoord")
    tex_coord.location = (-800, 0)
    
    # 2. Mapping to stretch waves along Z axis (grain look)
    mapping = nodes.new(type="ShaderNodeMapping")
    mapping.location = (-600, 0)
    # Stretch X/Y so waves look like long wood fibers
    mapping.inputs["Scale"].default_value = (8.0, 8.0, 0.1)
    links.new(tex_coord.outputs["Object"], mapping.inputs["Vector"])
    
    # 3. Wave Texture (Wood grain generator)
    wave = nodes.new(type="ShaderNodeTexWave")
    wave.location = (-400, 0)
    wave.wave_type = 'BANDS'
    wave.wave_profile = 'SIN'
    wave.inputs["Scale"].default_value = 12.0
    wave.inputs["Distortion"].default_value = 4.0
    wave.inputs["Detail"].default_value = 2.0
    links.new(mapping.outputs["Vector"], wave.inputs["Vector"])
    
    # 4. Color Ramp (Wood tones)
    color_ramp = nodes.new(type="ShaderNodeValToRGB")
    color_ramp.location = (-150, 0)
    color_ramp.color_ramp.elements[0].position = 0.4
    color_ramp.color_ramp.elements[0].color = (0.28, 0.22, 0.16, 1.0) # Dark grain line
    color_ramp.color_ramp.elements[1].position = 0.65
    color_ramp.color_ramp.elements[1].color = (0.38, 0.31, 0.23, 1.0) # Base wood brown
    links.new(wave.outputs["Color"], color_ramp.inputs["Fac"])
    
    links.new(color_ramp.outputs["Color"], bsdf.inputs["Base Color"])
    print("Applied wood grain shader")


def apply_metal_rust():
    mat, nodes, links, bsdf = get_nodes_and_links("Prop_Metal")
    bsdf.inputs["Metallic"].default_value = 0.75
    
    # 1. Texture Coordinate
    tex_coord = nodes.new(type="ShaderNodeTexCoord")
    tex_coord.location = (-800, 0)
    
    # 2. Noise Texture for rust patches
    noise = nodes.new(type="ShaderNodeTexNoise")
    noise.location = (-600, 0)
    noise.inputs["Scale"].default_value = 25.0
    noise.inputs["Detail"].default_value = 4.0
    noise.inputs["Roughness"].default_value = 0.8
    links.new(tex_coord.outputs["Object"], noise.inputs["Vector"])
    
    # 3. Color Ramp (Rust patch map)
    color_ramp = nodes.new(type="ShaderNodeValToRGB")
    color_ramp.location = (-400, 0)
    color_ramp.color_ramp.elements[0].position = 0.48
    color_ramp.color_ramp.elements[0].color = (0.18, 0.24, 0.27, 1.0) # Base metal gray
    color_ramp.color_ramp.elements[1].position = 0.62
    color_ramp.color_ramp.elements[1].color = (0.32, 0.18, 0.12, 1.0) # Rusty red-brown
    links.new(noise.outputs["Color"], color_ramp.inputs["Fac"])
    
    # 4. Color Ramp for Roughness (rusted parts are rougher)
    rough_ramp = nodes.new(type="ShaderNodeValToRGB")
    rough_ramp.location = (-400, -250)
    rough_ramp.color_ramp.elements[0].position = 0.48
    rough_ramp.color_ramp.elements[0].color = (0.42, 0.42, 0.42, 1.0) # Shinier metal
    rough_ramp.color_ramp.elements[1].position = 0.62
    rough_ramp.color_ramp.elements[1].color = (0.95, 0.95, 0.95, 1.0) # Rough rust
    links.new(noise.outputs["Color"], rough_ramp.inputs["Fac"])
    
    links.new(color_ramp.outputs["Color"], bsdf.inputs["Base Color"])
    links.new(rough_ramp.outputs["Color"], bsdf.inputs["Roughness"])
    print("Applied metal rust shader")


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
    
    # Build and assign procedural shaders for aging
    apply_boots_mud()
    apply_raincoat_painterly()
    apply_wood_grain()
    apply_metal_rust()

    # Save the file
    bpy.ops.wm.save_as_mainfile(filepath=str(BLEND_PATH))
    print(f"Saved Layer 7 textures and saved to {BLEND_PATH}")
    
    # Render preview cameras
    render_all_views()


# Execute directly in Blender MCP environment
main()
