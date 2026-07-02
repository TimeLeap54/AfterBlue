# Codex Rules for AfterBlue

## Project Goal

AfterBlue is a small 3D atmospheric fishing collection game. Do not expand the project into open-world exploration, combat, farming, survival, or NPC quest systems.

## Architecture Rules

- Keep systems modular.
- Do not create large manager classes that control everything.
- Prefer data-driven design using ScriptableObjects or JSON.
- Do not hardcode fish, bait, rod, or location data in gameplay logic.
- All gameplay systems must expose debug logs or debug UI when practical.

## Scope Rules

- No land walking.
- No free camera.
- No combat.
- No complex physics simulation.
- No multiplayer.
- No procedural open world.
- No large dialogue system.

## Coding Rules

- One task equals one feature.
- Do not modify unrelated files.
- Explain modified files after each change.
- Add simple test or debug instructions.
- Keep class names explicit.
- Use clear folders and namespaces.

## Acceptance Rule

A feature is done only when:

1. It works in Play Mode.
2. It has visible feedback.
3. It does not break existing controls.
4. It can be tested in under 30 seconds.

## Codex Task Format

```md
[Goal]
One sentence describing the feature to implement.

[Current State]
Existing scenes, scripts, prefabs, or data.

[Requirements]
Concrete behavior list.

[Allowed Files]
Files Codex may modify.

[Do Not Modify]
Systems or files Codex must not touch.

[Done When]
Play Mode conditions that prove the feature works.

[Debug Method]
How to test the feature.
```

