# AfterBlue Tasks

## Week 0 - Project Constitution

- [x] Create Unity 3D URP project
- [x] Confirm project folder
- [x] Initialize Git
- [x] Connect GitHub remote
- [x] Add project documents
- [x] Add base Unity folder structure
- [x] Add initial scripts
- [x] Create FishingScene
- [x] Place temporary boat, water, light, and camera
- [x] Verify Play Mode manually in Unity
- [x] Commit and push baseline

## Week 1 - Boat Feel & First Visual Pass

- [x] Add BoatController script
- [x] Add FollowCamera script
- [x] Install and verify Blender MCP bridge
- [x] Create first Blender boat model
- [x] Export first Unity boat FBX
- [x] Replace prototype cube boat with Blender boat model
- [x] Add first-pass flooded scenery props
- [x] Tune initial movement/camera defaults
- [ ] Test WASD boat movement in Play Mode after visual pass
- [ ] Tune camera manually in Unity
- [ ] Capture 10-second GIF

## Week 2 - Fishing Core Loop

- [x] Create fishing state machine states: Idle, Casting, Waiting, Bite, Hooked, Result
- [x] Add Space / left click fishing action input
- [x] Add runtime Bobber creation
- [x] Add bobber arc cast from PlayerBoat forward direction
- [x] Add LineRenderer fishing line between RodTip and Bobber
- [x] Add random bite delay
- [x] Add rarity-based bite timing window
- [x] Add temporary fish roll table
- [x] Add success / missed result model
- [x] Add OnGUI debug fishing HUD
- [x] Add runtime bootstrap for FishingScene
- [ ] Verify in Unity Play Mode
- [ ] Tune cast distance, rod tip position, and bite timing
- [ ] Tag week 2 milestone

## Week 3 - Flooded Village Visual Pass

- [x] Define cyan flooded modern village color direction
- [x] Add WaterRipple runtime effect
- [x] Connect Bobber land, bite, success, and fail events to ripples
- [x] Add FishShadow movement script for later reuse
- [x] Add FloodedVillageScenery render-setting script
- [x] Add Week 3 editor setup menu
- [x] Add primitive flooded village prop set
- [x] Remove fish shadow scene setup from Week 3 after readability test
- [x] Replace crack-like water line texture with soft patch and glint overlays
- [ ] Apply Week 3 setup to FishingScene in Unity
- [ ] Verify water color, fog, and camera readability
- [ ] Verify ripples during fishing loop
- [ ] Revisit fish shadows in a later polish pass
- [ ] Capture screenshot candidate
- [ ] Tag week 3 milestone
