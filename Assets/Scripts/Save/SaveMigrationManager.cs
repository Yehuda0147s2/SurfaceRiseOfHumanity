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
            // Future migrations must be ordered: v1 -> v2 -> v3.
            return state;
        }
    }

    public static class SaveValidator
    {
        public static bool IsValid(GameWorldState state)
        {
            if (state == null || state.metadata == null || state.metadata.saveFormatVersion > VersionedSaveService.CurrentVersion) return false;
            if (state.player == null || state.inventory == null || state.missions == null || state.world == null) return false;
            if (state.player.level < 1 || state.player.experience < 0 || state.player.health < 0) return false;
            if (state.inventory.items == null || state.inventory.items.Exists(x => x == null || x.quantity < 0 || string.IsNullOrWhiteSpace(x.itemId))) return false;
            return true;
        }
    }
}
