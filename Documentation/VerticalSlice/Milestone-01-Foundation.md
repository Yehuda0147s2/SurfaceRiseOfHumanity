# Vertical Slice Foundation — Milestone 01

This commit establishes the first modular runtime foundation for Kael and the connected world.

## Files

- `Assets/Scripts/Core/GameEnums.cs` — shared progression, region, and mission enums.
- `Assets/Scripts/Progression/KaelProgressionDefinition.cs` — expandable ScriptableObject tuning data.
- `Assets/Scripts/Progression/KaelProgression.cs` — XP, levels, skill points, attributes, and energy.
- `Assets/Scripts/Progression/KaelOverclock.cs` — energy-costed, timed Overclock ability.
- `Assets/Scripts/World/WorldRegion.cs` — region-level streaming configuration.
- `Assets/Scripts/World/WorldChunk.cs` — lightweight chunk metadata asset.
- `Assets/Scripts/World/WorldStreamingManager.cs` — low-frequency distance evaluation suitable for Android.

## Unity setup

1. Create a `KaelProgressionDefinition` asset via **Assets > Create > Surface > Kael Progression Definition**.
2. Create a Kael root GameObject and add `KaelProgression` and `KaelOverclock`.
3. Assign the definition to both components. Assign the progression component to `KaelOverclock`.
4. Create `WorldRegion` and `WorldChunk` assets via **Assets > Create > Surface**.
5. Add a `WorldStreamingManager` to a persistent `WorldSystems` GameObject and assign Kael's transform.
6. Add the region assets to the manager. Set The Deep, The Ascent, and Green Zone as the first three region IDs.

The current manager intentionally logs load/unload requests instead of calling scene APIs. This keeps the first milestone safe in an empty repository and provides a seam for additive Addressables or scene loading in the next milestone.

## Testing

- Enter Play mode and confirm Kael starts at level 1 with the configured stats.
- Call `AddExperience(100)` from a temporary debug button or script and confirm a level-up and skill point.
- Call `TryActivate()` on `KaelOverclock`; verify energy is consumed and the state ends after the configured duration.
- Move the player around chunk anchors and verify load/unload request logs occur no more than once per evaluation interval.
