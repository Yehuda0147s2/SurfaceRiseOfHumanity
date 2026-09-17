# Open World + Kael Core — Milestone 02

## New systems

- `Assets/Scripts/Player/KaelController.cs` — existing controller remains the gameplay root; use its mobile-callable input methods.
- `Assets/Scripts/Player/ThirdPersonCamera.cs` — smooth follow, look input, aim distance, and sphere-cast camera collision.
- `Assets/Scripts/Scanner/ScannerController.cs` — center-reticle scanning with progress, Overclock speed integration, and result events.
- `Assets/Scripts/Inventory/KaelInventory.cs` — stackable category-based inventory foundation.
- `Assets/Scripts/Exploration/PointOfInterest.cs` and `PointOfInterestDiscovery.cs` — persistent-discovery seam for landmarks and map updates.
- `Assets/Scripts/NPC/NPCInteractable.cs` — placeholder NPC name, faction, and dialogue interaction.
- `Assets/Scripts/Hacking/HackableTerminal.cs` — scan-then-hack terminal flow.
- `Assets/Scripts/World/DayNightCycle.cs` — low-cost directional-light cycle.
- `Assets/Scripts/World/WeatherController.cs` — clear/rain toggle using a single particle system and audio source.

## Kael prefab hierarchy

```text
Kael (Tag: Player)
├── CharacterController
├── KaelController
├── KaelProgression
├── KaelOverclock
├── Damageable
├── KaelInventory
├── ScannerController
├── CameraTarget
├── WeaponHand
│   └── AR7SurvivorRifle (placeholder mesh + muzzle)
├── BackWeaponMount
├── BackpackMount
└── ScannerMount
```

Create a separate `MainCamera` with `ThirdPersonCamera`; assign `CameraTarget` as `target`. Replace only the child placeholder model and Animator when final Kael art is available.

## Inspector setup

- `KaelController`: camera transform = MainCamera, walk 2.2, run 4.2, sprint 6.2, standing height 1.8, crouch height 1.1.
- `AR7SurvivorRifle`: damage 25, magazine size 30, reserve 120, range 150, reload 1.6 seconds.
- `RobotScout` + `Damageable`: health 100, detection 20, attack range 12, XP reward 100.
- `ScannerController`: range 35, scan duration 2 seconds; add `Scannable` to robots, terminals, resources, machines, and POIs.
- `DayNightCycle`: assign the directional sun and use a 900-second day for a calm first slice.
- `WeatherController`: assign a pooled rain particle system and looping rain audio. Keep rain emission low on Android.

## Scene layout

Use three connected additive scenes or chunk scenes: `TheDeep_Main`, `TheAscent_Main`, and `GreenZone_Main`. Build a physical maintenance tunnel and elevator transition between them. Put each scene's authored placeholder geometry, lights, NPCs, POIs, terminals, and encounter groups in its own scene. Do not claim these scenes or assets exist yet; they must be authored in Unity.

## Testing

1. Verify camera collision against walls and camera-relative movement in a boxed test area.
2. Aim at a `Scannable`, press `Q`, and confirm progress/result events.
3. Add and remove inventory entries from a temporary UI button or debug script.
4. Walk into a POI radius and confirm its discovery event fires once.
5. Scan a terminal, call `Hack(true)`, and verify `Activated`.
6. Run the day/night cycle with a short day length in the Inspector, then restore 900 seconds.
7. Toggle rain and verify particles/audio change.
8. Profile Android with the Unity Profiler; keep active AI and particle counts bounded.

## Known limitations

This commit adds runtime architecture and integration seams; it does not create final 3D art, authored terrain, scene files, animation clips, audio assets, mobile-control prefabs, or a production map UI. Those require Unity asset authoring and should be added as the next focused content pass.
