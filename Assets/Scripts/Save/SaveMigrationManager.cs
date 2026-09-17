using UnityEngine;
using SurfaceRiseOfHumanity.WorldState;

namespace SurfaceRiseOfHumanity.Save
{
    public static class SaveMigrationManager
    {
        public static GameWorldState Migrate(GameWorldState state)
        {
            if (state == null) return null;
            if (state.metadata == null) state.metadata = new SaveMetadata();
            if (state.metadata.saveFormatVersion <= 0) state.metadata.saveFormatVersion = 1;
            if (state.player == null) state.player = new PlayerState();
            if (state.missions == null) state.missions = new MissionState();
            if (state.world == null) state.world = new WorldState();
            if (state.inventory == null) state.inventory = new InventoryState();
            if (state.inventory.items == null) state.inventory.items = new System.Collections.Generic.List<InventoryRecord>();
            if (state.discoveries == null) state.discoveries = new DiscoveryState();
            if (state.scanner == null) state.scanner = new ScannerState();
            if (state.machineKnowledge == null) state.machineKnowledge = new MachineKnowledgeState();
            if (state.time == null) state.time = new WorldTimeState();
            return state;
        }
    }

    public static class SaveValidator
    {
        public static bool IsValid(GameWorldState state)
        {
            if (state == null || state.metadata == null || state.metadata.saveFormatVersion < 1 || state.metadata.saveFormatVersion > VersionedSaveService.CurrentVersion) return false;
            if (state.player == null || state.missions == null || state.world == null || state.inventory == null) return false;
            if (state.player.level < 1 || state.player.experience < 0 || state.player.health < 0 || state.player.maxHealth < 1 || state.player.energy < 0f || state.player.maxEnergy < 1f || state.player.stamina < 0f || state.player.maxStamina < 1f) return false;
            if (state.inventory.items == null) return false;
            foreach (InventoryRecord item in state.inventory.items) if (item == null || item.quantity < 0 || string.IsNullOrWhiteSpace(item.itemId)) return false;
            return true;
        }
    }
}
