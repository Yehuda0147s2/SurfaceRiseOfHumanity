# Open-World Gameplay Integration Map

## Existing architecture reused

The milestone preserves `GameWorldState`, `WorldStateManager`, `Scannable`, `ScannerController`, `PointOfInterest`, `KaelInventory`, `MissionObjective`, `WorldRegion`, and `WorldChunk`. No second save manager or parallel persistent database was introduced.

## Added integration seams

- `MachineDataLedger` subscribes to scanner results and writes stable scan IDs, machine knowledge, and raw Machine Data to `GameWorldState`.
- `POIDefinition` provides data-driven authoring metadata; `POIRegistry` restores discovered runtime POIs from persistent IDs.
- `EncounterDefinition` and `EncounterZone` separate encounter authoring from Robot Scout runtime spawning.
- `IWorldStreamingProvider` and `SceneWorldStreamingProvider` provide additive asynchronous chunk loading/unloading.
- `MapDataProvider` exposes player position, region state, and discovered POIs to map UI without direct world queries.
- `PerformanceOverlay` reports development-only FPS, frame time, memory, and loaded chunk count.

## Setup

1. Add `MachineDataLedger` beside `ScannerController` and assign `WorldStateManager`.
2. Create `POIDefinition` assets from **Assets > Create > Surface > POI Definition**. Give every POI a stable ID.
3. Create `EncounterDefinition` assets and place `EncounterZone` components in intentional Green Zone locations.
4. Add `SceneWorldStreamingProvider` to the world systems object. Use additive scenes for chunk `sceneName` values.
5. Add `MapDataProvider` to the map system and assign Kael, world state, and POI registry objects.
6. Add `PerformanceOverlay` only to development scenes/builds.

## Persistence integration

Runtime scan results update the central state immediately. POI discovery remains owned by `GameWorldState`; the registry only applies that state to loaded objects. Inventory, progression, missions, and scanner records continue through `WorldStateManager.SaveGame()` and `LoadGame()`.

## Testing status

No Unity Editor, Play Mode, Android device, or profiler was available in this API session. Runtime integration tests and performance profiling are therefore **NOT RUN**. The added code should be compiled and tested in Unity before this milestone is considered complete.

## Known limitations

The current pass does not author scenes, terrain, prefabs, final HUD, final map visuals, First Light cinematic assets, or a complete equipment modifier pipeline. `WorldStreamingManager` still needs to delegate its distance decisions to `IWorldStreamingProvider`; the provider is available as an integration seam. Encounter spawning is intentionally lightweight and requires authored spawn points and Robot Scout prefabs.
