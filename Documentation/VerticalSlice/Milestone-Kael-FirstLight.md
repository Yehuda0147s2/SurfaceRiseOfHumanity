# Milestone: Kael + First Light implementation foundation

This milestone adds the runtime seams for a playable vertical slice:

- `KaelController` — camera-relative movement, run, sprint, crouch, jump, gravity, dodge, and mobile-callable input methods.
- `Damageable` — reusable health, armor, damage, death, and damage events.
- `AR7SurvivorRifle` — 25 damage, 30-round magazine, 120 reserve rounds, raycast fire, reload, and empty-magazine handling.
- `RobotScout` — proximity detect/chase/attack, damage response, death, and 100 XP reward.
- `Scannable` — unknown/scanned/analysis-complete discovery state.
- `VerticalSliceSaveSystem` — version-ready JSON-shaped save seam using PlayerPrefs for the first slice.
- `MissionObjective` — reusable mission objective interaction/completion seam.

## Prefab setup

Create a Kael root with tag `Player`, then add `CharacterController`, `KaelController`, `KaelProgression`, `KaelOverclock`, and `Damageable`. Add child transforms named `WeaponHand`, `BackWeaponMount`, `BackpackMount`, `ScannerMount`, and `CameraTarget`. Add the placeholder mesh and Animator as children so the gameplay root remains replaceable.

Create a Robot Scout prefab with a collider, `Damageable` (max health 100), `RobotScout`, and `Scannable`. Keep patrol points and final navigation polish as the next environment pass; this first AI uses lightweight direct movement for Android-safe validation.

Create an AR-7 child object under `WeaponHand`, add `AR7SurvivorRifle`, and assign its muzzle and aim camera. UI buttons can call `TryFire`, `TryReload`, and the controller's `Jump`, `SetSprint`, `SetCrouch`, and `TryDodge` methods through a small event relay or Unity UI events.

## Important validation

The repository contains source code and setup documentation, not authored art, scenes, audio, terrain, or prefabs. Those assets must be created in Unity and clearly marked as placeholders until replaced. The direct-movement Robot Scout is intentionally a first playable validation implementation; NavMesh patrols, loot prefab drops, additive scene loading, HUD, and First Light sequencing are the next milestone integration work.
