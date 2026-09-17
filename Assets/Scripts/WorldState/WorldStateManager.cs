using System.Collections.Generic;
using UnityEngine;
using SurfaceRiseOfHumanity.Discovery;
using SurfaceRiseOfHumanity.Exploration;
using SurfaceRiseOfHumanity.Inventory;
using SurfaceRiseOfHumanity.Progression;
using SurfaceRiseOfHumanity.WorldState;

namespace SurfaceRiseOfHumanity.WorldState
{
    public sealed class WorldStateManager : MonoBehaviour
    {
        [SerializeField] private KaelProgression progression;
        [SerializeField] private KaelInventory inventory;
        [SerializeField] private Transform kael;
        [SerializeField] private WorldRegionId currentRegion = WorldRegionId.TheDeep;
        [SerializeField] private string currentScene = "TheDeep_Main";
        public GameWorldState State { get; private set; }
        public event System.Action<GameWorldState> StateChanged;
        private readonly Save.VersionedSaveService saveService = new Save.VersionedSaveService();

        private void Awake()
        {
            if (kael == null && progression != null) kael = progression.transform;
            State = GameWorldState.CreateNew();
        }

        public void NewGame() { State = GameWorldState.CreateNew(); StateChanged?.Invoke(State); }
        public void CaptureRuntime()
        {
            if (State == null) State = GameWorldState.CreateNew();
            State.metadata.currentScene = currentScene; State.metadata.currentRegion = currentRegion;
            if (kael != null) { State.player.position = kael.position; State.player.rotationEuler = kael.eulerAngles; }
            if (progression != null) { State.player.level = progression.Level; State.player.experience = progression.Experience; State.player.skillPoints = progression.SkillPoints; State.player.health = progression.Health; State.player.armor = progression.Armor; State.player.stamina = progression.Stamina; State.player.energy = progression.Energy; }
            if (inventory != null) { State.inventory.items.Clear(); foreach (InventoryEntry item in inventory.Entries) State.inventory.items.Add(new InventoryRecord { itemId = item.itemId, category = item.category, quantity = item.quantity }); }
            foreach (Scannable scan in FindObjectsOfType<Scannable>()) if (scan.Knowledge != ScanKnowledge.Unknown) { State.scanner.AddScan(scan.DiscoveryId); State.scanner.rawMachineData++; }
            foreach (PointOfInterest poi in FindObjectsOfType<PointOfInterest>()) if (poi.IsDiscovered) State.discoveries.Add(poi.PoiId);
        }

        public bool SaveGame() { CaptureRuntime(); return saveService.Save(State); }
        public bool LoadGame()
        {
            if (!saveService.TryLoad(out GameWorldState loaded)) return false;
            State = loaded; ApplyRuntime(); StateChanged?.Invoke(State); return true;
        }

        public void ApplyRuntime()
        {
            if (kael != null) { kael.position = State.player.position; kael.eulerAngles = State.player.rotationEuler; }
            if (inventory != null) { inventory.Clear(); foreach (InventoryRecord item in State.inventory.items) inventory.Add(item.itemId, item.category, item.quantity); }
            foreach (Scannable scan in FindObjectsOfType<Scannable>()) if (State.scanner.scannedObjectIds.Contains(scan.DiscoveryId)) scan.RestoreKnowledge(ScanKnowledge.Scanned);
            foreach (PointOfInterest poi in FindObjectsOfType<PointOfInterest>()) if (State.discoveries.Has(poi.PoiId)) poi.RestoreDiscovered();
        }

        public bool HasSave() => saveService.HasSave();
        public bool DeleteSave() => saveService.DeleteSave();
        public void Discover(string id) => State.discoveries.Add(id);
        public void AddMachineData(int amount) => State.scanner.rawMachineData = Mathf.Max(0, State.scanner.rawMachineData + amount);
        public void UnlockRegion(WorldRegionId id) { RegionState region = State.world.regions.Find(x => x.regionId == id); if (region == null) State.world.regions.Add(new RegionState { regionId = id, unlocked = true, discovered = true }); else { region.unlocked = true; region.discovered = true; } }
        public void PrintWorldState() => Debug.Log(JsonUtility.ToJson(State, true), this);
    }
}
