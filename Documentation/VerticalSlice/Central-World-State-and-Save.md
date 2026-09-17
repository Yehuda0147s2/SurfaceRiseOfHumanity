# Central World-State and Versioned Save — Inspection and Implementation

## Inspection report

The existing project already contained Kael progression, Overclock, inventory, scanner targets, POIs, missions, region/chunk metadata, world streaming, day/night, weather, and a prototype `VerticalSliceSaveSystem`. Authored Unity scenes, prefabs, and ScriptableObject instances are not present in the repository.

The old save component only stored a few progression integers and loaded scanner IDs from currently loaded objects. It was not sufficient as an authoritative save database. This milestone therefore adds one central `WorldStateManager` and a versioned file service while preserving the existing gameplay components.

## Architecture

`WorldStateManager` owns one `GameWorldState`. Runtime components are sampled into it through `CaptureRuntime()`. The state is serialized as a data-only JSON document by `VersionedSaveService`; Unity scene objects and MonoBehaviour references are never serialized.

`ApplyRuntime()` restores transform position, inventory, scanned objects, and POI discoveries for objects currently loaded. Region/chunk systems can query `State.world` by stable IDs when they load later.

## Save schema

```text
GameWorldState
├── metadata: SaveMetadata
├── player: PlayerState
├── missions: MissionState
├── world: WorldState (regions, chunks, gates, terminals, story flags)
├── discoveries: DiscoveryState
├── scanner: ScannerState (raw Machine Data + scan IDs)
├── machineKnowledge: MachineKnowledgeState
├── inventory: InventoryState
├── equipment: EquipmentState
├── encounters: EncounterState
├── events: WorldEventState
└── time: WorldTimeState
```

Current format is version **1**. `SaveMigrationManager` is the ordered migration seam; future versions must add deterministic transformations instead of interpreting old data as new data.

## Atomic storage

Files are written to `surface_default.save.json.tmp`, validated by deserializing, copied over the main save, and backed up to `.backup` first. Loads fall back to the backup when the main file is unavailable or invalid.

## Unity setup

1. Add `WorldStateManager` to a persistent `WorldSystems` GameObject.
2. Assign Kael's `KaelProgression`, `KaelInventory`, and root transform.
3. Add `NewGameInitializer` to the same object.
4. UI Save/Load buttons call `WorldStateManager.SaveGame()` and `LoadGame()`.
5. Give every POI and Scannable a stable serialized ID. Do not rely on generated object names for shipped content.
6. Call `UnlockRegion(WorldRegionId.GreenZone)` and set `State.world.firstLightSeen = true` from the First Light completion handler.

## Integration notes

- Scanner persistence currently records loaded `Scannable` objects by stable ID; the central list remains valid after chunks unload.
- POI persistence records stable POI IDs and restores discoveries when their chunk is loaded.
- Inventory records use item ID/category/quantity and do not serialize ScriptableObject references.
- Mission, equipment, encounter, event, chunk, and world-time DTOs are present in the schema for the next adapters; existing mission and streaming components should write stable IDs into those lists.
- The legacy `VerticalSliceSaveSystem` remains in the repository for compatibility but should be replaced on scenes by `WorldStateManager`.

## Testing status

No Unity Editor, build pipeline, or runtime was available in this API session, so tests were not executed. The implementation requires Unity compilation and Play Mode validation before being considered production-ready.

## Known limitations

The current pass does not yet apply progression scalar fields back into `KaelProgression`, because that component needs explicit public capture/apply APIs. It also does not asynchronously load scenes, restore day/night lighting, or provide automated Unity Test Framework fixtures. Those are intentionally documented follow-up integrations rather than claimed complete features.
