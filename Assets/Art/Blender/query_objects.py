import bpy

print("Active Scene:", bpy.context.scene.name)
print("Collections in Scene:")
for col in bpy.context.scene.collection.children:
    print("- ", col.name)

print("All Objects in Database:")
for obj in bpy.data.objects:
    print("- ", obj.name, "(visible:", obj.visible_get(), ")")

__result__ = {
    "active_scene": bpy.context.scene.name,
    "collections": [col.name for col in bpy.context.scene.collection.children],
    "objects": [obj.name for obj in bpy.data.objects]
}
