using System;
using System.Collections.Generic;
using UnityEngine;
using SurfaceRiseOfHumanity.Discovery;
using SurfaceRiseOfHumanity.Exploration;
using SurfaceRiseOfHumanity.Inventory;
using SurfaceRiseOfHumanity.Missions;
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
        [SerializeField] private bool loadOnStart;
        [SerializeField, Min(0.1f)] private float autosaveCooldown = 10f;

        public GameWorldState State { get; private set; }
        public event Action<GameWorldState> StateChanged;
        private readonly Save.VersionedSaveService saveService = new Save.VersionedSaveService();
        private float nextAutosaveTime;

        private void Awake()
        {
            if (kael == null && progression != null) kael = progression.transform;
            State = GameWorldState.CreateNew();
        }

        private void Start()
        {
            if (loadOnStart && saveService.HasSave()) LoadGame();
            else ApplyRuntime();
        }

        public void NewGame()
        {
            State = GameWorldState.CreateNew();
            ApplyRuntime();
            StateChanged?.Invoke(State);
        }

        public void CaptureRuntime()
        {
            if (State == null) State = GameWorldState.CreateNew();
            State.metadata.currentScene = currentScene;
            State.metadata.currentRegion = currentRegion;

            if (kael != null)
            {
                State.player.position = kael.position;
                State.player.rotationEuler = kael.eulerAngles;
            }

            if (progression != null)
            {
                PlayerState player = progression.CapturePlayerState();
                player.position = State.player.position;
                player.rotationEuler = State.player.rotationEuler;
                player.currentChunk = State.player.currentChunk;
                player.currentWeapon = State.player.currentWeapon;
                player.magazine = State.player.magazine;
                player.reserveAmmo = State.player.reserveAmmo;
                State.player = player;
            }

            if (inventory != null)
            {
                State.inventory.items.Clear();
                foreach (InventoryEntry item in inventory.Entries)
                {
                    if (item == null || item.quantity <= 0) continue;
                    State.inventory.items.Add(new InventoryRecord { itemId = item.itemId, category = item.category, quantity = item.quantity });
                }
            }

            foreach (Scannable scan in FindObjectsOfType<Scannable>())
            {
                if (scan.Knowledge == ScanKnowledge.Unknown) continue;
                State.scanner.AddScan(scan.DiscoveryId);
                MachineKnowledgeRecord knowledge = State.machineKnowledge.GetOrCreate(scan.DiscoveryId);
                knowledge.encountered = true;
                knowledge.scanned = true;
                knowledge.analysisComplete = scan.Knowledge == ScanKnowledge.AnalysisComplete;
                if (knowledge.analysisComplete) State.scanner.rawMachineData = Mathf.Max(1, State.scanner.rawMachineData);
            }

            foreach (PointOfInterest poi in FindObjectsOfType<PointOfInterest>())
                if (poi.IsDiscovered) State.discoveries.Add(poi.PoiId);

            foreach (MissionObjective objective in FindObjectsOfType<MissionObjective>())
                if (objective.IsComplete && !State.missions.objectiveFlags.Contains(objective.ObjectiveId))
                    State.missions.objectiveFlags.Add(objective.ObjectiveId);
        }

        public bool SaveGame()
        {
            CaptureRuntime();
            bool saved = saveService.Save(State);
            if (saved) StateChanged?.Invoke(State);
            return saved;
        }

        public bool TryAutosave()
        {
            if (Time.unscaledTime < nextAutosaveTime) return false;
            nextAutosaveTime = Time.unscaledTime + autosaveCooldown;
            return SaveGame();
        }

        public bool LoadGame()
        {
            if (!saveService.TryLoad(out GameWorldState loaded)) return false;
            State = loaded;
            ApplyRuntime();
            StateChanged?.Invoke(State);
            return true;
        }

        public void ApplyRuntime()
        {
            if (State == null) return;
            if (kael != null)
            {
                kael.position = State.player.position;
                kael.eulerAngles = State.player.rotationEuler;
            }
            if (progression != null) progression.ApplyPlayerState(State.player);
            if (inventory != null)
            {
                inventory.Clear();
                foreach (InventoryRecord item in State.inventory.items)
                    if (item != null && item.quantity > 0) inventory.Add(item.itemId, item.category, item.quantity);
            }

            foreach (Scannable scan in FindObjectsOfType<Scannable>())
            {
                MachineKnowledgeRecord knowledge = State.machineKnowledge.machines.Find(x => x.machineId == scan.DiscoveryId);
                if (knowledge != null && knowledge.analysisComplete)
                    scan.RestoreKnowledge(ScanKnowledge.AnalysisComplete);
                else if (State.scanner.scannedObjectIds.Contains(scan.DiscoveryId))
                    scan.RestoreKnowledge(ScanKnowledge.Scanned);
            }

            foreach (PointOfInterest poi in FindObjectsOfType<PointOfInterest>())
                if (State.discoveries.Has(poi.PoiId)) poi.RestoreDiscovered();

            foreach (MissionObjective objective in FindObjectsOfType<MissionObjective>())
                objective.RestoreCompleted(State.missions.objectiveFlags.Contains(objective.ObjectiveId));
        }

        public void CompleteMission(string missionId)
        {
            if (string.IsNullOrWhiteSpace(missionId)) return;
            if (!State.missions.completed.Contains(missionId)) State.missions.completed.Add(missionId);
            if (State.missions.activeMission == missionId) State.missions.activeMission = string.Empty;
            TryAutosave();
        }

        public bool IsMissionComplete(string missionId) => State != null && State.missions.completed.Contains(missionId);
        public bool HasSave() => saveService.HasSave();
        public bool DeleteSave() => saveService.DeleteSave();
        public void Discover(string id) { State.discoveries.Add(id); TryAutosave(); }
        public void AddMachineData(int amount) { State.scanner.rawMachineData = Mathf.Max(0, State.scanner.rawMachineData + amount); }
        public void UnlockRegion(WorldRegionId id)
        {
            RegionState region = State.world.regions.Find(x => x.regionId == id);
            if (region == null) State.world.regions.Add(new RegionState { regionId = id, unlocked = true, discovered = true });
            else { region.unlocked = true; region.discovered = true; }
        }
        public void PrintWorldState() => Debug.Log(JsonUtility.ToJson(State, true), this);
    }
}
